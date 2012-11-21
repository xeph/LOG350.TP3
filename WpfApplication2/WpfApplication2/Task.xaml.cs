using System.Windows;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for Task.xaml
    /// </summary>
    public partial class Task : Window
    {
        public Task()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine("SAVE BUTTON CLICKED");
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
