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
        private MainWindow parent;

        public static readonly System.Windows.DependencyProperty IsDirtyProperty = System.Windows.DependencyProperty.Register("IsDirty", typeof(bool), typeof(Task));
        private bool IsDirty
        {
            get
            {
                return (bool)GetValue(IsDirtyProperty);
            }
            set
            {
                SetValue(IsDirtyProperty, value);
            }
        }
        private bool IsLoading = false;

        public Task() : this(1, null)
        {
        }

        public Task(long id, MainWindow parent)
        {
            this.parent = parent;

            IsDirty = false;
            this.id = id;
            connection = new System.Data.SQLite.SQLiteConnection("Data source=" + System.Environment.CurrentDirectory.ToString() + "\\ToDoAny.sqlite;Version=3;");

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            IsLoading = true;
            InitializeComponent();
            LoadData();
            IsLoading = false;
        }

        private void LoadData()
        {
            taskDataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM tasks WHERE ID=" + id, connection);
            alertsDataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM tasks_alerts WHERE task_id=" + id, connection);
            subTasksDataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT ID, child_of, name FROM tasks WHERE child_of=" + id, connection);

            dataSet = new System.Data.DataSet();

            var taskCommandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(taskDataAdapter);
            var alertsCommandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(alertsDataAdapter);
            var subTasksCommandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(subTasksDataAdapter);

            taskDataAdapter.Fill(dataSet, "task");
            taskDataAdapter.AcceptChangesDuringUpdate = true;
            alertsDataAdapter.Fill(dataSet, "alerts");
            alertsDataAdapter.AcceptChangesDuringUpdate = true;
            subTasksDataAdapter.Fill(dataSet, "sub_tasks");
            subTasksDataAdapter.AcceptChangesDuringUpdate = true;

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

            row = dataSet.Tables["task"].Rows[0];
            dataSet.Tables["task"].RowChanged += table_RowChanged;
            dataSet.Tables["alerts"].RowChanged += table_RowChanged;
            dataSet.Tables["alerts"].RowDeleted += table_RowDeleted;
            dataSet.Tables["alerts"].TableNewRow += table_TableNewRow;
            dataSet.Tables["sub_tasks"].RowChanged += table_RowChanged;
            dataSet.Tables["sub_tasks"].RowDeleted += table_RowDeleted;
            dataSet.Tables["sub_tasks"].TableNewRow += table_TableNewRow;

            FillDeadline(row);
            FillTags(id);

            TaskGrid.DataContext = dataSet.Tables["task"].DefaultView;
            AlertsDataGrid.ItemsSource = dataSet.Tables["alerts"].DefaultView;
            SubTasksDataGrid.ItemsSource = dataSet.Tables["sub_tasks"].DefaultView;

            FillPrioritiesComboBox();
        }

        private bool Save()
        {
            System.Data.SQLite.SQLiteTransaction transaction = null;
            using (AutomaticOpenClose aoc = new AutomaticOpenClose(connection))
            {
                try
                {
                    transaction = connection.BeginTransaction();
                    // --------------------------------------------------
                    // Deadline
                    if (DeadlineIsSelectedDateRadioButton.IsChecked.Value)
                    {
                        var command = new System.Data.SQLite.SQLiteCommand("INSERT INTO deadlines(deadline) VALUES(@deadline); SELECT last_insert_rowid()", connection);
                        command.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@deadline", DeadlineDatePicker.SelectedDate));
                        var reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            row["deadline_id"] = reader.GetInt64(0);
                        }
                    }
                    else if (DeadlineIsEventRadioButton.IsChecked.Value)
                    {
                        row["deadline_id"] = (EventsComboBox.SelectedValue != null) ? EventsComboBox.SelectedValue : System.DBNull.Value;
                    }
                    else
                    {
                        row["deadline_id"] = System.DBNull.Value;
                    }

                    taskDataAdapter.Update(dataSet, "task");

                    // --------------------------------------------------
                    // Tags
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

                        var whereInTuple2 = Util.SqlParametersList(oldTasksTagsIDs);
                        var command = new System.Data.SQLite.SQLiteCommand("DELETE FROM tasks_tags WHERE task_id=@id AND ID NOT IN(" + whereInTuple2.Item1 + ")", connection);

                        command.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@id", id));
                        foreach (var parameter in whereInTuple2.Item2)
                            command.Parameters.Add(parameter);

                        command.ExecuteNonQuery();
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

                    // --------------------------------------------------
                    // Alerts
                    foreach (System.Data.DataRow row in dataSet.Tables["alerts"].Rows)
                    {
                        if (row.RowState == System.Data.DataRowState.Added)
                            row["task_id"] = id;
                    }

                    alertsDataAdapter.Update(dataSet, "alerts");
                    dataSet.Tables["alerts"].Clear();
                    alertsDataAdapter.Fill(dataSet, "alerts");

                    // --------------------------------------------------
                    // Sub-Tasks
                    foreach (System.Data.DataRow row in dataSet.Tables["sub_tasks"].Rows)
                    {
                        if (row.RowState == System.Data.DataRowState.Added)
                            row["child_of"] = id;
                    }

                    subTasksDataAdapter.Update(dataSet, "sub_tasks");
                    dataSet.Tables["sub_tasks"].Clear();
                    subTasksDataAdapter.Fill(dataSet, "sub_tasks");

                    // --------------------------------------------------
                    // Clean state
                    IsDirty = false;
                    transaction.Commit();
                }
                catch (System.Data.SQLite.SQLiteException e)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    switch (e.ErrorCode)
                    {
                        case System.Data.SQLite.SQLiteErrorCode.Constraint:
                            Util.ShowFieldMustBeUniqueMessage(this, Util.ExtractColumnName(e.Message));
                            break;
                    }
                }
            }

            try
            {
                this.parent.MassReloadTasks();
            }
            catch
            {
            }

            return !IsDirty;
        }

        private void FillDeadline(System.Data.DataRow row)
        {
            FillEventsComboBox();

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
            string query = "SELECT ID, name FROM priorities WHERE active=1";
            if (!row.IsNull("priority_id"))
                query += " OR ID=" + row["priority_id"];
            var dataAdapter = new System.Data.SQLite.SQLiteDataAdapter(query, connection);
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

        private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Save();
        }

        private void DeleteSubTaskMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                ((System.Data.DataRowView)SubTasksDataGrid.SelectedItem).Delete();
            }
            catch { }
        }

        private void DeleteAlertMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try{
                ((System.Data.DataRowView)AlertsDataGrid.SelectedItem).Delete();
            }
            catch { }
        }

        private void table_RowChanged(object sender, System.Data.DataRowChangeEventArgs e)
        {
            if (!IsLoading)
                IsDirty = true;
        }

        private void table_RowDeleted(object sender, System.Data.DataRowChangeEventArgs e)
        {
            if (!IsLoading)
                IsDirty = true;
        }

        private void table_TableNewRow(object sender, System.Data.DataTableNewRowEventArgs e)
        {
            if (!IsLoading)
                IsDirty = true;
        }

        private void PriorityHyperlink_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Nullable<bool> prioritiesChanged = new Priority().ShowDialog();

            if (prioritiesChanged.HasValue && prioritiesChanged.Value)
                FillPrioritiesComboBox();
        }

        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Close();
        }

        private void EventsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!IsLoading)
                IsDirty = true;

            var dataRowView = EventsComboBox.SelectedItem as System.Data.DataRowView;

            if (dataRowView != null)
                DeadlineDatePicker.SelectedDate = (System.DateTime)dataRowView["deadline"];
            else
                DeadlineDatePicker.SelectedDate = null;
        }

        private void SubTasksDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dataRowView = ((System.Windows.Controls.DataGridRow)sender).Item as System.Data.DataRowView;
            if (dataRowView != null)
            {
                var dataRow = dataRowView.Row;
                if (!dataRow.IsNull("ID"))
                {
                    System.Nullable<bool> taskChanged = new Task((long)dataRow["ID"], this.parent).ShowDialog();

                    if (taskChanged.HasValue && taskChanged.Value)
                    {
                        // Update the view since something changed in the selected task
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsDirty)
            {
                switch (Util.AskToSaveModification(this))
                {
                    case System.Windows.MessageBoxResult.Yes:
                        if (!Save())
                            e.Cancel = true;
                        break;
                    case System.Windows.MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }

            /*
            if (prioritiesUpdated)
                DialogResult = true;
            */
        }

        private void RadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsLoading)
                IsDirty = true;
        }

        private void DeadlineDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!IsLoading)
                IsDirty = true;
        }

        private void TagsTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!IsLoading)
                IsDirty = true;
        }
    }
}
