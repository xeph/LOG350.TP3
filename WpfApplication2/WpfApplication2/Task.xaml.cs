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

        private const long DeadlineIsNull = -2;
        private const long DeadlineIsSelectedDate = -1;

        private System.Data.SQLite.SQLiteConnection connection;
        private System.Data.SQLite.SQLiteDataAdapter TaskDataAdapter;
        private System.Data.SQLite.SQLiteDataAdapter AlertsDataAdapter;
        private System.Data.DataSet dataSet;

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
            if (!System.DBNull.Value.Equals(row["deadline_id"]))
            {
                long deadlineID = (long)row["deadline_id"];
                System.Nullable<System.DateTime> date = null;
                System.Nullable<long> eventID = null;
                using (var automatic = new AutomaticOpenClose(connection))
                {
                    var command = new System.Data.SQLite.SQLiteCommand("SELECT deadlines.deadline, events.ID FROM deadlines LEFT JOIN events ON deadlines.ID=events.deadline_id WHERE deadlines.ID=@id", connection);
                    command.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@id", deadlineID));

                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        date = reader.GetDateTime(0);
                        if (!reader.IsDBNull(1))
                            eventID = reader.GetInt64(1);
                    }
                }

                if (date.HasValue)
                    DeadlineDatePicker.SelectedDate = date.Value;
                if (eventID.HasValue)
                    DeadlineEventComboBox.SelectedValue = eventID.Value;
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

        private void SubTasksDataGrid_Initialized(object sender, System.EventArgs e)
        {
            // TODO: Delete all this code and replace it with a simple query when database is available.

            var dataTable = new System.Data.DataTable("Priorities");
            dataTable.Columns.Add(new System.Data.DataColumn("ID", typeof(int)));
            dataTable.Columns.Add(new System.Data.DataColumn("action", typeof(int)));
            dataTable.Columns.Add(new System.Data.DataColumn("delta", typeof(string)));

            AlertsDataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void AlertsDataGrid_Initialized(object sender, System.EventArgs e)
        {
            // TODO: Delete all this code and replace it with a simple query when database is available.

            var dataTable = new System.Data.DataTable("Priorities");
            dataTable.Columns.Add(new System.Data.DataColumn("ID", typeof(int)));
            dataTable.Columns.Add(new System.Data.DataColumn("child_id", typeof(int)));
            dataTable.Columns.Add(new System.Data.DataColumn("name", typeof(string)));
            dataTable.Columns.Add(new System.Data.DataColumn("deadline_id", typeof(int)));
            dataTable.Columns.Add(new System.Data.DataColumn("priority_id", typeof(int)));
            dataTable.Columns.Add(new System.Data.DataColumn("completion", typeof(double)));
            dataTable.Columns.Add(new System.Data.DataColumn("description", typeof(string)));

            AlertsDataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var builder = new System.Text.StringBuilder();

            foreach (string tag in Util.SplitTags(TagsTextBox.Text))
                builder.AppendLine(tag);

            System.Windows.MessageBox.Show(builder.ToString());
        }

        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }

        private void ContextualMenuAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void ContextualMenuModify_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void ContextualMenuDelete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void PriorityComboBox_Initialized(object sender, System.EventArgs e)
        {
            var connection = new System.Data.SQLite.SQLiteConnection("Data source=" + System.Environment.CurrentDirectory.ToString() + "\\ToDoAny.sqlite;Version=3;");
            var dataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT ID, name FROM priorities", connection);
            var dataSet = new System.Data.DataSet();
            var commandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(dataAdapter);

            dataAdapter.Fill(dataSet, "priorities");

            PriorityComboBox.ItemsSource = dataSet.Tables["priorities"].DefaultView;
        }

        private void PriorityComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            System.Windows.MessageBox.Show(PriorityComboBox.SelectedValue + " selected");
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            TaskDataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM tasks WHERE ID=1", connection);
            AlertsDataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM tasks_alerts WHERE task_id=1", connection);

            dataSet = new System.Data.DataSet();

            var taskCommandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(TaskDataAdapter);
            var alertsCommandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(AlertsDataAdapter);

            TaskDataAdapter.Fill(dataSet, "task");
            AlertsDataAdapter.Fill(dataSet, "alerts");

            var parentColumn = dataSet.Tables["task"].Columns["ID"];
            var childColumn = dataSet.Tables["alerts"].Columns["task_id"];
            var relation = new System.Data.DataRelation("task_alerts", parentColumn, childColumn);
            dataSet.Relations.Add(relation);

            table = dataSet.Tables["task"];

            long id = (long)table.Rows[0]["ID"];

            FillDeadline(table.Rows[0]);
            FillTags(id);

            TaskGrid.DataContext = table.DefaultView;
        }

        private System.Data.DataTable table;

        private void DeadlineEventComboBox_Initialized(object sender, System.EventArgs e)
        {
            DeadlineEventComboBox.DataContext = this;
            PossibleDeadlines.Add(new System.Tuple<long, string, System.Nullable<System.DateTime>>(DeadlineIsNull, "Aucune", null));
            PossibleDeadlines.Add(new System.Tuple<long, string, System.Nullable<System.DateTime>>(DeadlineIsSelectedDate, "Date précise", null));

            using (AutomaticOpenClose aoc = new AutomaticOpenClose(connection))
            {
                var command = new System.Data.SQLite.SQLiteCommand("SELECT deadlines.ID, events.name, deadlines.deadline FROM events LEFT JOIN deadlines ON events.deadline_id=deadlines.ID WHERE deadlines.deadline>=date('now') ORDER BY events.name", connection);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var id = reader.GetInt64(0);
                    var name = reader.GetString(1);
                    System.Nullable<System.DateTime> date = null;

                    if (!reader.IsDBNull(2))
                        date = reader.GetDateTime(2);

                    PossibleDeadlines.Add(new System.Tuple<long, string, System.Nullable<System.DateTime>>(id, name, date));
                }
            }

            DeadlineEventComboBox.SelectedValue = (long)-2;
        }

        private void DeadlineEventComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var tuple = (System.Tuple<long, string, System.Nullable<System.DateTime>>) DeadlineEventComboBox.SelectedItem;

            if (tuple.Item1 == DeadlineIsSelectedDate)
            {
                DeadlineDatePicker.IsEnabled = true;
            }
            else
            {
                DeadlineDatePicker.IsEnabled = false;
                DeadlineDatePicker.SelectedDate = tuple.Item3;
            }
        }
    }
}
