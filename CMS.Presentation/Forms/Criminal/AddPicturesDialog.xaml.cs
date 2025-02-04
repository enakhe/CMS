#nullable disable

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

namespace CMS.Presentation.Forms.Criminal
{
    public partial class AddPicturesDialog : Window
    {
        private string mugShotImagePath;
        private List<byte[]> additionalImageBytesList = new List<byte[]>();
        private string criminalId;

        public AddPicturesDialog(string criminalId)
        {
            this.criminalId = criminalId;
            InitializeComponent();
        }
    }
}
