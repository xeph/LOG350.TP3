using System.Collections.Generic;
using System.Windows.Input;

namespace OdbcSQLite
{
    public partial class Priorities : System.Windows.Window
    {
        public Priorities()
        {
            InitializeComponent();
        }

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

        private void Save()
        {
            // Here we should synchronize with database
            SourceDataTable.AcceptChanges();
        }

        private void DataGrid_Initialized(object sender, System.EventArgs e)
        {
            // TODO: Delete all this code and replace it with a simple query when database is available.

            SourceDataTable = new System.Data.DataTable("Priorities");
            SourceDataTable.Columns.Add(new System.Data.DataColumn("ID", System.Type.GetType("System.Int32")));
            SourceDataTable.Columns.Add(new System.Data.DataColumn("Name", System.Type.GetType("System.String")));
            SourceDataTable.Columns.Add(new System.Data.DataColumn("Value", System.Type.GetType("System.Int32")));
            SourceDataTable.Columns.Add(new System.Data.DataColumn("Active", System.Type.GetType("System.Boolean")));

            System.Action<int, string, int, bool> AddNewRow = (id, name, value, active) =>
                {
                    var row = SourceDataTable.NewRow();
                    row["ID"] = id;
                    row["Name"] = name;
                    row["Value"] = value;
                    row["Active"] = active;
                    SourceDataTable.Rows.Add(row);
                };

            AddNewRow(1, "Haute", 1, true);
            AddNewRow(2, "Moyenne", 2, true);
            AddNewRow(3, "Faible", 3, true);

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
