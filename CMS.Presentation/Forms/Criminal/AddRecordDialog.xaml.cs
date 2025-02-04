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

        public AddRecordDialog(CriminalUsecaces criminalUsecaces)
        {
            _criminalUsecaces = criminalUsecaces;
            InitializeComponent();
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
                criminal.Age = DateTime.Now.Year - criminal.DateOfBirth.Year;
                _criminalUsecaces.AddCriminalRecord(criminal);

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
