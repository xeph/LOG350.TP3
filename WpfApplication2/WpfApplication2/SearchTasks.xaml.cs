﻿using System.Windows;
using System.Windows.Controls;

namespace WpfApplication2
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    public partial class SearchTasks : UserControl
    {
        private System.Data.SQLite.SQLiteConnection connection;
        private System.Data.SQLite.SQLiteDataAdapter dataAdapter = new System.Data.SQLite.SQLiteDataAdapter();
        private System.Data.DataSet dataSet = new System.Data.DataSet();
        private MainWindow parent;

        public SearchTasks(MainWindow parent)
        {
            this.parent = parent;
            InitializeComponent();
        }
        private void Save()
        {
            foreach (System.Data.DataRowView x in MainDataGrid.Items)
            {
                x.Row[2] = x.Row[2].ToString().Replace(">", "");
            }
            try
            {
                dataAdapter.Update(dataSet, "task");
            }
            catch
            {
            }
            this.parent.MassReloadTasks();
        }
        public void Load()
        {
            dataAdapter.Dispose();
            dataSet.Dispose();
            dataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT *, CASE WHEN child_of IS NULL THEN ID ELSE CHILD_OF END AS SORT1, CASE WHEN child_of IS NULL THEN 1 ELSE 2 END AS SORT2 FROM tasks ORDER BY SORT1 ASC, SORT2 ASC", connection);
            dataSet = new System.Data.DataSet();

            var commandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(dataAdapter);

            dataAdapter.Fill(dataSet, "task");
            dataAdapter.AcceptChangesDuringUpdate = true;

            MainDataGrid.ItemsSource = dataSet.Tables["task"].DefaultView;

            foreach (System.Data.DataRowView x in MainDataGrid.Items)
            {
                if (x.Row[1].ToString().Length > 0)
                    x.Row[2] = ">"+x.Row[2];
            }
        }
        private void DataGrid_Initialized(object sender, System.EventArgs e)
        {
            connection = new System.Data.SQLite.SQLiteConnection("Data source=" + System.Environment.CurrentDirectory.ToString() + "\\ToDoAny.sqlite;Version=3;");
            Load();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //Get top priority
            System.Data.SQLite.SQLiteDataAdapter DA = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM priorities WHERE active=1 LIMIT 1", connection);
            System.Data.DataSet DS = new System.Data.DataSet();
            var commandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(DA);
            DA.Fill(DS, "priorities");
            long priorityID = 1;
            try
            {
                priorityID = (long)DS.Tables["priorities"].Rows[0]["ID"];
            }
            catch { priorityID = 1; }

            //Add row and save
            dataSet.Tables["task"].Rows.Add(System.DBNull.Value, System.DBNull.Value, NewTaskName.Text, System.DBNull.Value, priorityID, System.DBNull.Value);
            NewTaskName.Text = "";
            Save();
            Load();
        }

        private void MainDataGrid_Row_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task taskWindow = new Task((long)((System.Data.DataRowView)((DataGridRow)sender).Item)["ID"], this.parent);
            taskWindow.ShowDialog();
            taskWindow.Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Priority priorityWindow = new Priority();
            priorityWindow.ShowDialog();
            priorityWindow.Close();
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((System.Data.DataRowView)MainDataGrid.SelectedItem).Delete();
            Save();
        }

        private void NewTaskName_KeyUp_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                AddButton_Click(null, null);
        }
    }
}
