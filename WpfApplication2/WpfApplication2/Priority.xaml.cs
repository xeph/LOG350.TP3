using System.Windows.Input;

namespace WpfApplication2
{
    public partial class Priority : System.Windows.Window
    {
        private System.Data.SQLite.SQLiteConnection connection;
        private System.Data.SQLite.SQLiteDataAdapter dataAdapter;
        private System.Data.DataSet dataSet;

        private System.Data.DataTable sourceDataTable;
        private System.Data.DataTable SourceDataTable
        {
            get
            {
                return sourceDataTable;
            }
            set
            {
                sourceDataTable = value;
                PropertiesDataGrid.ItemsSource = sourceDataTable.DefaultView;
            }
        }

        public Priority()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void Save()
        {
            dataAdapter.Update(dataSet, "priorities");
        }

        private void DataGrid_Initialized(object sender, System.EventArgs e)
        {
            connection = new System.Data.SQLite.SQLiteConnection("Data source=" + System.Environment.CurrentDirectory.ToString() + "\\ToDoAny.sqlite;Version=3;");
            dataAdapter = new System.Data.SQLite.SQLiteDataAdapter("SELECT * FROM priorities", connection);
            dataSet = new System.Data.DataSet();

            dataAdapter.TableMappings.Add("priorities", "priorities");
            var commandBuilder = new System.Data.SQLite.SQLiteCommandBuilder(dataAdapter);
            
            dataAdapter.Fill(dataSet, "priorities");

            SourceDataTable = dataSet.Tables["priorities"];
        }

        private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Save();
        }

        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }

        private void DeleteMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((System.Data.DataRowView)PropertiesDataGrid.SelectedItem).Delete();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (SourceDataTable.GetChanges() != null)
            {
                var result = System.Windows.MessageBox.Show(this,
                    "Voulez-vous enregistrer vos modifications?",
                    "Modifications non-enregistrées",
                    System.Windows.MessageBoxButton.YesNoCancel,
                    System.Windows.MessageBoxImage.None,
                    System.Windows.MessageBoxResult.Yes);

                switch (result)
                {
                    case System.Windows.MessageBoxResult.Yes:
                        Save();
                        break;
                    case System.Windows.MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // For debugging purpose, we can show/hide the ID column with CTRL+SHIFT+F8
            // Ok, this was not necessary for the project but was interesting to do

            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control & ModifierKeys.Shift) == (ModifierKeys.Control & ModifierKeys.Shift) && e.Key == Key.F8)
            {
                var column = PropertiesDataGrid.Columns[0];
                switch (column.Visibility)
                {
                    case System.Windows.Visibility.Visible:
                        column.Visibility = System.Windows.Visibility.Hidden;
                        break;
                    case System.Windows.Visibility.Hidden:
                        column.Visibility = System.Windows.Visibility.Visible;
                        break;
                }
            }
        }
    }
}
