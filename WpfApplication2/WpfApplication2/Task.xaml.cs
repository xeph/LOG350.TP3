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

        public System.Collections.ObjectModel.ObservableCollection<System.Tuple<long, string, System.Nullable<System.DateTime>>> PossibleDeadlines { get; private set; }

        public Task()
        {
            PossibleDeadlines = new System.Collections.ObjectModel.ObservableCollection<System.Tuple<long, string, System.Nullable<System.DateTime>>>();
            connection = new System.Data.SQLite.SQLiteConnection("Data source=" + System.Environment.CurrentDirectory.ToString() + "\\ToDoAny.sqlite;Version=3;");

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            InitializeComponent();
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
            var builder = new System.Text.StringBuilder();

            foreach (string tag in Util.SplitTags(TagsTextBox.Text))
                builder.AppendLine(tag);

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
            taskDataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM tasks WHERE ID=1", connection);
            alertsDataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM tasks_alerts WHERE task_id=1", connection);
            subTasksDataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM tasks WHERE child_of=1", connection);

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


            long id = (long) row["ID"];

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
