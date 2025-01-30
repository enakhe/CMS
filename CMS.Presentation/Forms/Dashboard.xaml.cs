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
using System.Windows.Shapes;
using CMS.Presentation.Forms.Home;
using CMS.Presentation.Forms.Criminal;

namespace CMS.Presentation.Forms
{
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();
            MainFrame.Navigate(new IndexPage());
        }

        private void CriminalButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CriminalPage());
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new IndexPage());
        }
    }
}
