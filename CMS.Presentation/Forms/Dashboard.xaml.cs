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
using Microsoft.Extensions.DependencyInjection;
using CMS.Presentation.Forms.FaceDetector;
using Emgu.CV.Structure;
using Emgu.CV;

namespace CMS.Presentation.Forms
{
    public partial class Dashboard : Window
    {
        private bool _isLoading;
        public Dashboard()
        {
            InitializeComponent();
            MainFrame.Navigate(new IndexPage());
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

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new IndexPage());
        }

        private void CriminalButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsLoading = true;
                var services = new ServiceCollection();
                DependencyInjection.ConfigureServices(services);
                var serviceProvider = services.BuildServiceProvider();

                CriminalPage criminalPage = serviceProvider.GetRequiredService<CriminalPage>();
                Dispatcher.Invoke(() => MainFrame.Navigate(criminalPage));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FaceDetectorButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsLoading = true;

                var services = new ServiceCollection();
                DependencyInjection.ConfigureServices(services);
                var serviceProvider = services.BuildServiceProvider();

                FaceDetectorPage faceDetectorPage = serviceProvider.GetRequiredService<FaceDetectorPage>();

                MainFrame.Navigate(faceDetectorPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
