using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication2
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    public partial class Search : UserControl
    {
        public Search()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            System.Nullable<bool> taskChanged = new Task(1).ShowDialog();

            if (taskChanged.HasValue && taskChanged.Value)
            {
                // Update the view since something changed in the selected task
            }
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
