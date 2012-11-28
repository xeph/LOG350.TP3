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
            CloseableTab newTab = new CloseableTab();
            newTab.Content = new Search();
            newTab.Title = "new search";

            tabcontrol.Items.Insert(tabcontrol.Items.Count - 1, newTab);
            tabcontrol.SelectedIndex = tabcontrol.Items.Count-2;
        }
    }
}
