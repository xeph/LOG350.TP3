using System;
using System.Windows;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for Task.xaml
    /// </summary>
    public partial class Task : Window
    {
        Database.SQLite DB = new Database.SQLite(StaticPath.DBPath);

        public Task()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void DataGridSubTasks_Initialized(object sender, EventArgs e)
        {
            System.Console.WriteLine("DataGridSubTasks_Initialized");

            this.DB.openConnection();
            this.DB.beginTransaction();
            // add sql queries here
            this.DB.endTransaction();
            this.DB.closeConnection();
        }

        private void DataGridAlerts_Initialized(object sender, EventArgs e)
        {
            System.Console.WriteLine("DataGridAlerts_Initialized");

            this.DB.openConnection();
            this.DB.beginTransaction();
            // add sql queries here
            this.DB.endTransaction();
            this.DB.closeConnection();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine("SAVE BUTTON CLICKED");

            this.DB.openConnection();
            this.DB.beginTransaction();
            // add sql queries here
            this.DB.endTransaction();
            this.DB.closeConnection();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine("CANCEL BUTTON CLICKED");
        }

        private void ContextualMenuAdd_Click(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine("CONTEXTUAL MENU ADD CLICKED");
        }

        private void ContextualMenuModify_Click(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine("CONTEXTUAL MENU MODIFY CLICKED");
        }

        private void ContextualMenuDelete_Click(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine("CONTEXTUAL MENU DELETE CLICKED");
        }
    }
}
