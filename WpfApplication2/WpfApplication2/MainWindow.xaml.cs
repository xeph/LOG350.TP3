using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.IO;
using System.Windows.Markup;
using System.Windows.Controls;

namespace WpfApplication2
{
    public partial class MainWindow
    {

        Database.SQLite DB = new Database.SQLite(StaticPath.DBPath);

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
            tabmain.Content = new Search();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            this.DB.openConnection();
            this.DB.compactDatabase();
            this.DB.closeConnection();
        }

        private void newtab_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddNewTab();
        }

        public void AddNewTab()
        {
            TabItem test = new TabItem();
            test.Content = new Search();
            test.Header = "new search";
            
            tabcontrol.Items.Add(test);
        }
    }
}
