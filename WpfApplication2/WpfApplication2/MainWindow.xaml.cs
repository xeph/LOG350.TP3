using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace WpfApplication2
{
    public partial class MainWindow
    {

        private int newTabNumber;
        Database.SQLite DB = new Database.SQLite(StaticPath.DBPath);

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
            this.newTabNumber = 1;
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            this.DB.openConnection();
            this.DB.compactDatabase();
            this.DB.closeConnection();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Task taskWindow = new Task();
            taskWindow.ShowDialog();
            taskWindow.Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Priority priorityWindow = new Priority();
            priorityWindow.ShowDialog();
            priorityWindow.Close();
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
