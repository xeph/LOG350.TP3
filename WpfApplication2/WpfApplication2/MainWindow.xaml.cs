using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            this.DataGrid1.ItemsSource = LoadCollectionData();
            //this.MouseLeftButtonDown += delegate { this.DragMove(); };
            this.newTabNumber = 1;
        }

        /*private void HandleAddTab(object sender, RoutedEventArgs e)
        {
            this.chrometabs.AddTab(this.GenerateNewItem(), false);
        }

        private void HandleAddTabAndSelect(object sender, RoutedEventArgs e)
        {
            this.chrometabs.AddTab(this.GenerateNewItem(), true);
        }

        private object GenerateNewItem()
        {
            object itemToAdd = new Button { Content = "Moo " + this.newTabNumber };
            Interlocked.Increment(ref this.newTabNumber);
            if (this.title.Text.Length > 0)
            {
                itemToAdd = new ChromeTabs.ChromeTabItem
                {
                    Header = this.title.Text,
                    Content = itemToAdd
                };
            }
            return itemToAdd;
        }

        private void HandleRemoveTab(object sender, RoutedEventArgs e)
        {
            this.chrometabs.RemoveTab(this.chrometabs.SelectedItem);
        }*/

        private int newTabNumber;

        private List<Author> LoadCollectionData()
        {
            List<Author> authors = new List<Author>();
            authors.Add(new Author()
            {
                ID = 101,
                Name = "Mahesh Chand",
                BookTitle = "Graphics Programming with GDI+",
                DOB = new DateTime(1975, 2, 23),
                IsMVP = false
            });
            authors.Add(new Author()
            {
                ID = 201,
                Name = "Mike Gold",
                BookTitle = "Programming C#",
                DOB = new DateTime(1982, 4, 12),
                IsMVP = true
            });
            authors.Add(new Author()
            {
                ID = 244,
                Name = "Mathew Cochran",
                BookTitle = "LINQ in Vista",
                DOB = new DateTime(1985, 9, 11),
                IsMVP = true
            });
            return authors;
        }
    }

    public class Author
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string BookTitle { get; set; }
        public bool IsMVP { get; set; }
    }
}
