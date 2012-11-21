using System.Windows;
using System.Data;

namespace WpfApplication2
{
    public partial class Priority : Window
    {
        private Database.SQLite DB = new Database.SQLite(@"C:\Users\Don\Documents\GitHub\LOG350.TP3\WpfApplication2\WpfApplication2\ToDoAny.sqlite");
        private DataTable loadedTable = new DataTable();
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
            this.DB.openConnection();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void Save()
        {
            foreach (DataRow x in SourceDataTable.Rows)
            {
                if (loadedTable.Select("ID = " + System.Convert.ToInt32(x["ID"])).Length == 0)
                {
                    //this.DB.executeQuery("insert into ");
                }
                else
                {
                    this.DB.executeQuery("update priorities set name='" + x["name"] + "', value=" + x["value"] + ", active=" + x["active"]);
                }

            }
        }

        private void DataGrid_Initialized(object sender, System.EventArgs e)
        {
            SourceDataTable = new System.Data.DataTable("Priorities");
            SourceDataTable.Columns.Add(new System.Data.DataColumn("ID", System.Type.GetType("System.Int32")));
            SourceDataTable.Columns.Add(new System.Data.DataColumn("Name", System.Type.GetType("System.String")));
            SourceDataTable.Columns.Add(new System.Data.DataColumn("Value", System.Type.GetType("System.Int32")));
            SourceDataTable.Columns.Add(new System.Data.DataColumn("Active", System.Type.GetType("System.Boolean")));

            this.DB.executeQuery("SELECT * FROM 'priorities'");
            loadedTable = DB.getTable();

            foreach (DataRow x in loadedTable.Rows)
            {
                SourceDataTable.Rows.Add(System.Convert.ToInt32(x["ID"]), System.Convert.ToString(x["name"]), System.Convert.ToInt32(x["value"]), System.Convert.ToBoolean(x["active"]));
            }

            this.DB.closeConnection();
        }

        private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Save();
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

            //if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control & ModifierKeys.Shift) == (ModifierKeys.Control & ModifierKeys.Shift) && e.Key == Key.F8)
            //{
            //    var column = PropertiesDataGrid.Columns[0];
            //    switch (column.Visibility)
            //    {
            //        case System.Windows.Visibility.Visible:
            //            column.Visibility = System.Windows.Visibility.Hidden;
            //            break;
            //        case System.Windows.Visibility.Hidden:
            //            column.Visibility = System.Windows.Visibility.Visible;
            //            break;
            //    }
            //}
        }
    }
}
