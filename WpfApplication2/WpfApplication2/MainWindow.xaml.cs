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
        Dictionary<int, SearchTasks> listTasks = new Dictionary<int, SearchTasks>();
        Dictionary<int, SearchEvents> listEvents = new Dictionary<int, SearchEvents>();
        int counterTasks = 0;
        int counterEvents = 0;

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);

            SearchTasks nST = new SearchTasks(this);
            listTasks.Add(counterTasks++, nST);
            tabtasks.Content = nST;
            SearchEvents nSE = new SearchEvents(this);
            listEvents.Add(counterEvents++, nSE);
            tabevents.Content = nSE;
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            this.DB.openConnection();
            this.DB.compactDatabase();
            this.DB.closeConnection();
        }

        public void MassReloadTasks()
        {
            foreach (var x in listTasks)
            {
                try
                {
                    x.Value.Load();
                }
                catch { }
            }
        }
        public void MassReloadEvents()
        {
            foreach (var x in listEvents)
            {
                try
                {
                    x.Value.Load();
                }
                catch { }
            }
        }

        private void newtab_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddNewTab();
        }

        public void AddNewTab()
        {
            CloseableTab newTab = new CloseableTab();
            SearchTasks nST = new SearchTasks(this);
            listTasks.Add(counterTasks++, nST);
            newTab.Content = nST;
            newTab.Title = "nouvelle recherche";

            tabcontrol.Items.Insert(tabcontrol.Items.Count - 1, newTab);
            tabcontrol.SelectedIndex = tabcontrol.Items.Count-2;
        }
    }
}
