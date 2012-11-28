namespace WpfApplication2
{
    public partial class Task : System.Windows.Window
    {
        private class AutomaticOpenClose : System.IDisposable
        {
            public AutomaticOpenClose(System.Data.SQLite.SQLiteConnection conn)
            {
                connection = conn;
                connection.Open();
            }

            public void Dispose()
            {
                connection.Close();
            }

            private System.Data.SQLite.SQLiteConnection connection;
        }

        private System.Data.SQLite.SQLiteConnection connection;
        private System.Data.SQLite.SQLiteDataAdapter taskDataAdapter;
        private System.Data.SQLite.SQLiteDataAdapter alertsDataAdapter;
        private System.Data.SQLite.SQLiteDataAdapter subTasksDataAdapter;
        private System.Data.DataSet dataSet;
        private System.Data.DataRow row;
        private long id;

        public System.Collections.ObjectModel.ObservableCollection<System.Tuple<long, string, System.Nullable<System.DateTime>>> PossibleDeadlines { get; private set; }

        public Task() : this(1)
        {
        }

        public Task(long id)
        {
            this.id = id;
            PossibleDeadlines = new System.Collections.ObjectModel.ObservableCollection<System.Tuple<long, string, System.Nullable<System.DateTime>>>();
            connection = new System.Data.SQLite.SQLiteConnection("Data source=" + System.Environment.CurrentDirectory.ToString() + "\\ToDoAny.sqlite;Version=3;");

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            InitializeComponent();
        }

        private void Save()
        {
            // --------------------------------------------------
            // Deadline
            if (DeadlineIsSelectedDateRadioButton.IsChecked.Value)
            {
                using (AutomaticOpenClose aoc = new AutomaticOpenClose(connection))
                {

                    var command = new System.Data.SQLite.SQLiteCommand("INSERT INTO deadlines(deadline) VALUES(@deadline); SELECT last_insert_rowid()", connection);
                    command.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@deadline", DeadlineDatePicker.SelectedDate));
                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        row["deadline_id"] = reader.GetInt64(0);
                    }
                }
            }
            else if (DeadlineIsEventRadioButton.IsChecked.Value)
            {
                row["deadline_id"] = EventsComboBox.SelectedValue;
            }
            else
            {
                row["deadline_id"] = System.DBNull.Value;
            }

            taskDataAdapter.Update(dataSet, "task");

            // --------------------------------------------------
            // Tags
            using (AutomaticOpenClose aoc = new AutomaticOpenClose(connection))
            {
                var currentTags = new System.Collections.Generic.List<string>(Util.SplitTags(TagsTextBox.Text));
                var whereInTuple = Util.SqlParametersList(currentTags);
                var rows = new System.Collections.Generic.List<System.Tuple<System.Nullable<long>, System.Nullable<long>, System.Nullable<long>, string>>();

                {
                    var command = new System.Data.SQLite.SQLiteCommand(@"
                        SELECT tasks_tags.ID, tasks_tags.task_id, tasks_tags.tag_id, tags.name
                            FROM tasks_tags LEFT JOIN tags ON tags.ID = tasks_tags.tag_id
                            WHERE tasks_tags.task_id=@id  AND tags.name IN(" + whereInTuple.Item1 + @")
                        UNION ALL
                        SELECT NULL, NULL, ID, name
                            FROM tags
                            WHERE ID NOT IN(SELECT tag_id FROM tasks_tags WHERE task_id=@id) AND name IN(" + whereInTuple.Item1 + ")", connection);

                    command.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@id", id));
                    foreach (var parameter in whereInTuple.Item2)
                        command.Parameters.Add(parameter);

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var tuple = new System.Tuple<System.Nullable<long>, System.Nullable<long>, System.Nullable<long>, string>(null, null, null, "");

                        System.Nullable<long> tasksTagsID = null;
                        System.Nullable<long> taskID = null;
                        System.Nullable<long> tagID = null;
                        string name = "";

                        if (!reader.IsDBNull(0))
                            tasksTagsID = reader.GetInt64(0);
                        if (!reader.IsDBNull(1))
                            taskID = reader.GetInt64(1);
                        if (!reader.IsDBNull(2))
                            tagID = reader.GetInt64(2);
                        if (!reader.IsDBNull(3))
                            name = reader.GetString(3);

                        rows.Add(System.Tuple.Create(tasksTagsID, taskID, tagID, name));
                    }
                }

                // delete all old tasks_tags not need for new tags
                {
                    var oldTasksTagsIDs = new System.Collections.Generic.List<long>();
                    foreach (var tuple in rows)
                    {
                        if (tuple.Item1.HasValue)
                            oldTasksTagsIDs.Add(tuple.Item1.Value);
                    }

                    if (oldTasksTagsIDs.Count != 0)
                    {
                        var whereInTuple2 = Util.SqlParametersList(oldTasksTagsIDs);
                        var command = new System.Data.SQLite.SQLiteCommand("DELETE FROM tasks_tags WHERE task_id=@id AND ID NOT IN(" + whereInTuple2.Item1 + ")", connection);

                        command.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@id", id));
                        foreach (var parameter in whereInTuple2.Item2)
                            command.Parameters.Add(parameter);

                        command.ExecuteNonQuery();
                    }
                }

                // link existing new tags
                    foreach (var tuple in rows)
                {
                    if (!tuple.Item1.HasValue && tuple.Item3.HasValue)
                    {
                        var tagID = tuple.Item3.Value;
                        long newTasksTagsID = Util.InsertInto(connection, "tasks_tags", System.Tuple.Create("task_id", id), System.Tuple.Create("tag_id", tagID));
                    }
                }

                // create and link new tags
                {
                    var newTags = new System.Collections.Generic.List<string>();
                    foreach (var tagName in currentTags)
                    {
                        bool found = false;
                        foreach (var row in rows)
                        {
                            if (row.Item4 == tagName)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            long newTagID = Util.InsertInto(connection, "tags", System.Tuple.Create("name", tagName));
                            long newTasksTagsID = Util.InsertInto(connection, "tasks_tags", System.Tuple.Create("task_id", id), System.Tuple.Create("tag_id", newTagID));
                        }
                    }
                }

                /*
                var columnsNames = new System.Text.StringBuilder();

                while (reader.Read())
                    columnsNames.AppendFormat("ID:{0}\tTaskID:{1}\tTagID:{2}\tTagName:{3}\n", reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3));

                System.Windows.MessageBox.Show(columnsNames.ToString());
                */
                /*
                var currentTags = new System.Collections.Generic.List<string>(Util.SplitTags(TagsTextBox.Text));
                var existingTags = new System.Collections.Generic.List<System.Tuple<long, string>>();
                var whereInTuple = Util.SqlParametersList(currentTags);

                // Delete all tags not in the current tags
                {
                    var command = new System.Data.SQLite.SQLiteCommand("DELETE FROM tasks_tags WHERE task_id=@id AND tag_id NOT IN(SELECT ID FROM tags WHERE name IN(" + whereInTuple.Item1 + "))", connection);
                    
                    command.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@id", id));
                    foreach (var parameter in whereInTuple.Item2)
                        command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                }

                // Query for ID of current tags
                if (currentTags.Count != 0)
                {
                    var command = new System.Data.SQLite.SQLiteCommand("SELECT ID, name FROM tags WHERE name IN(" + whereInTuple.Item1 + ")", connection);

                    foreach (var parameter in whereInTuple.Item2)
                        command.Parameters.Add(parameter);

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                        existingTags.Add(new System.Tuple<long, string>(reader.GetInt64(0), reader.GetString(1)));
                }

                // Add current tags not in existing tags
                {
                    currentTags.remov
                }*/
            }
        }

        private void FillDeadline(System.Data.DataRow row)
        {
            if (!row.IsNull("deadline_id"))
            {
                long deadlineID = (long) row["deadline_id"];

                using (var automatic = new AutomaticOpenClose(connection))
                {
                    var command = new System.Data.SQLite.SQLiteCommand("SELECT deadlines.deadline, events.ID FROM deadlines LEFT JOIN events ON deadlines.ID=events.deadline_id WHERE deadlines.ID=@id", connection);
                    command.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@id", deadlineID));

                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        DeadlineDatePicker.SelectedDate = reader.GetDateTime(0);
                        if (reader.IsDBNull(1))
                        {
                            DeadlineIsSelectedDateRadioButton.IsChecked = true;
                        }
                        else
                        {
                            DeadlineIsEventRadioButton.IsChecked = true;
                            EventsComboBox.SelectedValue = reader.GetInt64(1);
                        }
                    }
                }
            }
        }

        private void FillTags(long taskID)
        {
            using (AutomaticOpenClose aoc = new AutomaticOpenClose(connection))
            {
                var command = new System.Data.SQLite.SQLiteCommand("SELECT tags.name FROM tasks_tags INNER JOIN tags ON tasks_tags.tag_id=tags.ID WHERE tasks_tags.task_id=@id", connection);
                command.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@id", taskID));

                var reader = command.ExecuteReader();
                var builder = new System.Text.StringBuilder();

                while (reader.Read())
                {
                    builder.AppendFormat("[{0}]", reader[0]);
                }

                TagsTextBox.Text = builder.ToString();
            }
        }

        private void FillPrioritiesComboBox()
        {
            var dataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT ID, name FROM priorities WHERE active=1", connection);
            var dataSet = new System.Data.DataSet();
            dataAdapter.Fill(dataSet, "priorities");

            var previousValue = PriorityComboBox.SelectedValue;
            PriorityComboBox.ItemsSource = dataSet.Tables["priorities"].DefaultView;
            PriorityComboBox.SelectedValue = previousValue;
        }

        private void FillEventsComboBox()
        {
            var dataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT deadlines.ID, events.name, deadlines.deadline FROM events LEFT JOIN deadlines ON events.deadline_id=deadlines.ID WHERE deadlines.deadline>=date('now') ORDER BY events.name", connection);
            var dataSet = new System.Data.DataSet();
            dataAdapter.Fill(dataSet, "events");

            var previousValue = EventsComboBox.SelectedValue;
            EventsComboBox.ItemsSource = dataSet.Tables["events"].DefaultView;
            EventsComboBox.SelectedValue = previousValue;
        }

        private void AlertsDataGrid_Initialized(object sender, System.EventArgs e)
        {
            // TODO: Delete all this code and replace it with a simple query when database is available.

            var dataTable = new System.Data.DataTable("Priorities");
            dataTable.Columns.Add(new System.Data.DataColumn("ID", typeof(int)));
            dataTable.Columns.Add(new System.Data.DataColumn("action", typeof(int)));
            dataTable.Columns.Add(new System.Data.DataColumn("delta", typeof(string)));

            AlertsDataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Save();
        }

        private void DeleteSubTaskMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void PriorityComboBox_Initialized(object sender, System.EventArgs e)
        {
            FillPrioritiesComboBox();
        }

        private void EventsComboBox_Initialized(object sender, System.EventArgs e)
        {
            FillEventsComboBox();
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            taskDataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM tasks WHERE ID=" + id, connection);
            alertsDataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM tasks_alerts WHERE task_id=" + id, connection);
            subTasksDataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM tasks WHERE child_of=" + id, connection);

            dataSet = new System.Data.DataSet();

            var taskCommandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(taskDataAdapter);
            var alertsCommandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(alertsDataAdapter);
            var subTasksCommandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(subTasksDataAdapter);

            taskDataAdapter.Fill(dataSet, "task");
            alertsDataAdapter.Fill(dataSet, "alerts");
            subTasksDataAdapter.Fill(dataSet, "sub_tasks");

            var parentColumn = dataSet.Tables["task"].Columns["ID"];

            {
                var childColumn = dataSet.Tables["alerts"].Columns["task_id"];
                var relation = new System.Data.DataRelation("task_alerts", parentColumn, childColumn);
                dataSet.Relations.Add(relation);
            }
            {
                var childColumn = dataSet.Tables["sub_tasks"].Columns["child_of"];
                var relation = new System.Data.DataRelation("task_sub_tasks", parentColumn, childColumn);
                dataSet.Relations.Add(relation);
            }

            var table = dataSet.Tables["task"];
            row = table.Rows[0];

            FillDeadline(row);
            FillTags(id);

            TaskGrid.DataContext = table.DefaultView;
            SubTasksDataGrid.ItemsSource = dataSet.Tables["sub_tasks"].DefaultView;
        }

        private void PriorityHyperlink_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Nullable<bool> prioritiesChanged = new Priority().ShowDialog();

            if (prioritiesChanged.HasValue && prioritiesChanged.Value)
                FillPrioritiesComboBox();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }

        private void EventsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var row = EventsComboBox.SelectedItem as System.Data.DataRowView;

            if (row != null)
                DeadlineDatePicker.SelectedDate = (System.DateTime)row["deadline"];
            else
                DeadlineDatePicker.SelectedDate = null;
        }
    }
}
