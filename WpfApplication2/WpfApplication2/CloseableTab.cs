// CITATION: Code provenant en partie de http://www.codeproject.com/Articles/84213/How-to-add-a-Close-button-to-a-WPF-TabItem
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApplication2
{
    class CloseableTab : TabItem
    {
        private CloseableHeader closeableTabHeader;

        public CloseableTab()
        {
            // Create an instance of the usercontrol
            closeableTabHeader = new CloseableHeader();
            // Assign the usercontrol to the tab header
            this.Header = closeableTabHeader;

            closeableTabHeader.button_close.MouseEnter +=
            new MouseEventHandler(button_close_MouseEnter);
            closeableTabHeader.button_close.MouseLeave +=
               new MouseEventHandler(button_close_MouseLeave);
            closeableTabHeader.button_close.Click +=
               new RoutedEventHandler(button_close_Click);
            closeableTabHeader.label_TabTitle.SizeChanged +=
               new SizeChangedEventHandler(label_TabTitle_SizeChanged);
        }
        public string Title
        {
            set
            {
                ((CloseableHeader)this.Header).label_TabTitle.Content = value;
                if (value == "Main")
                {
                    ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Hidden;
                }
            }
            get
            {
                return ((CloseableHeader)this.Header).label_TabTitle.Content.ToString();
            }
        }

        void button_close_MouseEnter(object sender, MouseEventArgs e)
        {
            ((CloseableHeader)this.Header).button_close.Foreground = Brushes.Red;
        }
        // Button MouseLeave - When mouse is no longer over button - change color back to black
        void button_close_MouseLeave(object sender, MouseEventArgs e)
        {
            ((CloseableHeader)this.Header).button_close.Foreground = Brushes.Black;
        }
        // Button Close Click - Remove the Tab - (or raise
        // an event indicating a "CloseTab" event has occurred)
        void button_close_Click(object sender, RoutedEventArgs e)
        {
            if (this.Title != "Main"){
                TabControl parent = ((TabControl)this.Parent);
                parent.Items.Remove(this);
                if (parent.SelectedIndex == parent.Items.Count - 1)
                {
                    parent.SelectedIndex = parent.Items.Count - 2;
                }
                
            }
        }
        // Label SizeChanged - When the Size of the Label changes
        // (due to setting the Title) set position of button properly
        void label_TabTitle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((CloseableHeader)this.Header).button_close.Margin = new Thickness(
               ((CloseableHeader)this.Header).label_TabTitle.ActualWidth + 5, 3, 4, 0);
        }
    }
}
