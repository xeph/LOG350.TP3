using System.Windows;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for Priority.xaml
    /// </summary>
    public partial class Priority : Window
    {
        public Priority()
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
    }
}
