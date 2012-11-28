// BUG : SUPPRESSION D'UN SEUL TAG EXISTANT NE FONCTIONNE PAS, bug supposement regler, il ne reste qu'a l'implementer dans le code

namespace WpfApplication2
{
    public partial class Events : System.Windows.Window
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
        private System.Data.DataSet dataSet;
        private System.Data.DataRow row;
        private long id;

        public System.Collections.ObjectModel.ObservableCollection<System.Tuple<long, string, System.Nullable<System.DateTime>>> PossibleDeadlines { get; private set; }

        public Events() : this(1)
        {
        }

        public Events(long id)
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
            using (AutomaticOpenClose aoc = new AutomaticOpenClose(connection))
            {

                var command = new System.Data.SQLite.SQLiteCommand("INSERT INTO deadlines(deadline) VALUES(@deadline); SELECT last_insert_rowid()", connection);
                command.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@deadline", DeadlineDatePicker.SelectedDate));
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    row["deadline_id"] = reader.GetInt64(0);
                }
                else
                {
                    row["deadline_id"] = System.DBNull.Value;
                }
            }

            taskDataAdapter.Update(dataSet, "event");

            // --------------------------------------------------
            // Tags
            using (AutomaticOpenClose aoc = new AutomaticOpenClose(connection))
            {
                var currentTags = new System.Collections.Generic.List<string>(Util.SplitTags(TagsTextBox.Text));
                var whereInTuple = Util.SqlParametersList(currentTags);
                var rows = new System.Collections.Generic.List<System.Tuple<System.Nullable<long>, System.Nullable<long>, System.Nullable<long>, string>>();

                {
                    var command = new System.Data.SQLite.SQLiteCommand(@"
                        SELECT events_tags.ID, events_tags.event_id, events_tags.tag_id, tags.name
                            FROM events_tags LEFT JOIN tags ON tags.ID = events_tags.tag_id
                            WHERE events_tags.event_id=@id  AND tags.name IN(" + whereInTuple.Item1 + @")
                        UNION ALL
                        SELECT NULL, NULL, ID, name
                            FROM tags
                            WHERE ID NOT IN(SELECT tag_id FROM events_tags WHERE event_id=@id) AND name IN(" + whereInTuple.Item1 + ")", connection);

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
                        var command = new System.Data.SQLite.SQLiteCommand("DELETE FROM events_tags WHERE event_id=@id AND ID NOT IN(" + whereInTuple2.Item1 + ")", connection);

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
                        long newTasksTagsID = Util.InsertInto(connection, "events_tags", System.Tuple.Create("event_id", id), System.Tuple.Create("tag_id", tagID));
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
                            long newTasksTagsID = Util.InsertInto(connection, "events_tags", System.Tuple.Create("event_id", id), System.Tuple.Create("tag_id", newTagID));
                        }
                    }
                }
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
                    }
                }
            }
        }

        private void FillTags(long taskID)
        {
            using (AutomaticOpenClose aoc = new AutomaticOpenClose(connection))
            {
                var command = new System.Data.SQLite.SQLiteCommand("SELECT tags.name FROM events_tags INNER JOIN tags ON events_tags.tag_id=tags.ID WHERE events_tags.event_id=@id", connection);
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

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            taskDataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM events WHERE ID=" + id, connection);
            alertsDataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM events_alerts WHERE event_id=" + id, connection);

            dataSet = new System.Data.DataSet();

            var taskCommandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(taskDataAdapter);
            var alertsCommandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(alertsDataAdapter);

            taskDataAdapter.Fill(dataSet, "event");
            alertsDataAdapter.Fill(dataSet, "alerts");

            var parentColumn = dataSet.Tables["event"].Columns["ID"];

            {
                var childColumn = dataSet.Tables["alerts"].Columns["event_id"];
                var relation = new System.Data.DataRelation("event_alerts", parentColumn, childColumn);
                dataSet.Relations.Add(relation);
            }

            var table = dataSet.Tables["event"];
            row = table.Rows[0];

            FillDeadline(row);
            FillTags(id);

            TaskGrid.DataContext = table.DefaultView;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
    }
}
