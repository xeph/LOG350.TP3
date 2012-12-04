using System.Windows;
using System.Windows.Controls;

namespace WpfApplication2
{
    public partial class SearchEvents : UserControl
    {
        private System.Data.SQLite.SQLiteConnection connection;
        private System.Data.SQLite.SQLiteDataAdapter dataAdapter = new System.Data.SQLite.SQLiteDataAdapter();
        private System.Data.DataSet dataSet = new System.Data.DataSet();

        public SearchEvents()
        {
            InitializeComponent();
        }
        private void Save()
        {
                dataAdapter.Update(dataSet, "events");
        }
        private void Load()
        {
            dataAdapter.Dispose();
            dataSet.Dispose();
            dataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM events", connection);
            dataSet = new System.Data.DataSet();

            var commandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(dataAdapter);

            dataAdapter.Fill(dataSet, "events");
            dataAdapter.AcceptChangesDuringUpdate = true;

            MainDataGrid.ItemsSource = dataSet.Tables["events"].DefaultView;
        }
        private void DataGrid_Initialized(object sender, System.EventArgs e)
        {
            connection = new System.Data.SQLite.SQLiteConnection("Data source=" + System.Environment.CurrentDirectory.ToString() + "\\ToDoAny.sqlite;Version=3;");
            Load();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

            //Insert and save
            dataSet.Tables["events"].Rows.Add(System.DBNull.Value, NewEventName.Text, "0", "");
            NewEventName.Text = "";
            Save();
            Load();
        }

        private void MainDataGrid_Row_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Events eventWindow = new Events((long)((System.Data.DataRowView)((DataGridRow)sender).Item)["ID"]);
            eventWindow.ShowDialog();
            eventWindow.Close();
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
    }
}
