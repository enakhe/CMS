#nullable disable

using CMS.Application.UseCases.Criminal;
using CMS.Domain.Entities;
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
    public partial class AddRecordDialog : Window
    {
        private readonly CriminalUsecaces _criminalUsecaces;
        public AddRecordDialog(CriminalUsecaces criminalUsecaces)
        {
            _criminalUsecaces = criminalUsecaces;
            InitializeComponent();
        }

        private void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Domain.Entities.Criminal criminal = new Domain.Entities.Criminal()
                {
                    CriminalID = Guid.NewGuid().ToString().Split("-")[0],
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
                _criminalUsecaces.AddCriminalRecord(criminal);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
