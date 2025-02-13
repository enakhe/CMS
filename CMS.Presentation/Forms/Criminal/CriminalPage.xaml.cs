using CMS.Application.UseCases.Criminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
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
    public partial class CriminalPage : Page
    {
        private readonly CriminalUsecaces _criminalUsecaces;
        private bool _isLoading;
        public CriminalPage(CriminalUsecaces criminalUsecaces)
        {
            _criminalUsecaces = criminalUsecaces;
            InitializeComponent();
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                LoaderGrid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                List<Domain.Entities.Criminal> allCriminal = await _criminalUsecaces.GetAllCriminalRecords();
                if (allCriminal != null)
                {
                    dgCriminals.ItemsSource = allCriminal;
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddRecordDialog addRecordDialog = new AddRecordDialog(_criminalUsecaces);
            if (addRecordDialog.ShowDialog() == true)
            {
                await LoadDataAsync();
            }
        }

        //private async void Page_Loaded(object sender, RoutedEventArgs e)
        //{
            
        //}

        private async void dgCriminals_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }
    }
}
