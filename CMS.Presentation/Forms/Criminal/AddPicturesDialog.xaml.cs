#nullable disable

using CMS.Application.UseCases.Criminal;
using CMS.Domain.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        private string _criminalId;
        private readonly CriminalUsecaces _criminalUsecaces;

        public AddPicturesDialog(string criminalId, CriminalUsecaces criminalUsecaces)
        {
            this._criminalId = criminalId;
            _criminalUsecaces = criminalUsecaces;
            InitializeComponent();
        }

        private void UploadMugShot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Select Image",
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    mugShotImagePath = openFileDialog.FileName;
                    imgCriminalPhoto.Source = new BitmapImage(new Uri(mugShotImagePath));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void UploadAdditionalImages_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Up to 4 Images",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string[] selectedFiles = openFileDialog.FileNames;
                additionalImageBytesList.Clear();
                for (int i = 0; i < selectedFiles.Length; i++)
                {
                    additionalImageBytesList.Add(File.ReadAllBytes(selectedFiles[i]));
                    BitmapImage bitmap = new BitmapImage(new Uri(selectedFiles[i]));
                    switch (i)
                    {
                        case 0: img1.Source = bitmap; break;
                        case 1: img2.Source = bitmap; break;
                        case 2: img3.Source = bitmap; break;
                        case 3: img4.Source = bitmap; break;
                    }
                }
            }
        }

        private async void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            LoaderOverlay.Visibility = Visibility.Visible;
            try
            {
                await Task.Run(() => SimulateDataProcessing());
                byte[] mugShotImageBytes = null;
                if (!string.IsNullOrEmpty(mugShotImagePath))
                {
                    mugShotImageBytes = File.ReadAllBytes(mugShotImagePath);
                }
                Domain.Entities.CriminalPictures criminalPictures = new CriminalPictures()
                {
                    CriminalID = _criminalId,
                    Mugshot = mugShotImageBytes,
                    AdditionalPictures = additionalImageBytesList
                };
                _criminalUsecaces.AddCriminalImages(criminalPictures);
                MessageBox.Show("Criminal Pictures Saved Successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {

            }
        }

        private void CancleButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void SimulateDataProcessing()
        {
            System.Threading.Thread.Sleep(3000);
        }
    }
}
