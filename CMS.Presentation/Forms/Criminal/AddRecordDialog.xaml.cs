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
    public partial class AddRecordDialog : Window
    {
        private readonly CriminalUsecaces _criminalUsecaces;
        private string mugShotImagePath;
        private List<byte[]> additionalImageBytesList = new List<byte[]>();
        public AddRecordDialog(CriminalUsecaces criminalUsecaces)
        {
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
            catch(Exception ex)
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

                if (selectedFiles.Length > 4)
                {
                    MessageBox.Show("You can upload a maximum of 4 images!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

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
                Domain.Entities.Criminal criminal = new Domain.Entities.Criminal()
                {
                    CriminalID = Guid.NewGuid().ToString().Split("-")[0].ToUpper(),
                    FullName = txtFullName.Text.Trim(),
                    DateOfBirth = (DateTime)dpDOB.SelectedDate,
                    Gender = cbGender.Text.ToString(),
                    NationalID = txtNationalID.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Offenses = txtOffenses.Text.Trim(),
                    Status = txtStatus.Text.Trim(),
                    Notes = txtNotes.Text.Trim(),
                    WatchlistStatus = (bool)chkWatchlist.IsChecked
                };

                byte[] mugShotImageBytes = null;
                if (!string.IsNullOrEmpty(mugShotImagePath))
                {
                    mugShotImageBytes = File.ReadAllBytes(mugShotImagePath);
                }

                Domain.Entities.CriminalPictures criminalPictures = new CriminalPictures()
                {
                    CriminalId = criminal.Id,
                    Criminal = criminal,
                    Mugshot = mugShotImageBytes,
                    AdditionalPictures = additionalImageBytesList
                };

                _criminalUsecaces.AddCriminalRecord(criminal);
                _criminalUsecaces.AddCriminalImages(criminalPictures);
                this.DialogResult = true;
                MessageBox.Show("Criminal Record Saved Successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoaderOverlay.Visibility = Visibility.Collapsed;
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
