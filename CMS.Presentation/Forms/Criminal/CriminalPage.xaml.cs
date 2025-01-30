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

namespace CMS.Presentation.Forms.Criminal
{
    /// <summary>
    /// Interaction logic for CriminalPage.xaml
    /// </summary>
    public partial class CriminalPage : Page
    {
        public CriminalPage()
        {
            InitializeComponent();
        }

        private void dgCriminals_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddRecordDialog addRecordDialog = new AddRecordDialog();
            if (addRecordDialog.ShowDialog() == true)
            {
                
            }
        }
    }
}
