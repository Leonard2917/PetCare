using System.Windows;
using PetCare.ViewModels;
using PetCare.Models;

namespace PetCare.Views
{
    public partial class ViewMedicalRecordView : Window
    {
        public ViewMedicalRecordView(FiseMedicale record)
        {
            InitializeComponent();
            DataContext = new ViewMedicalRecordViewModel(record);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public ViewMedicalRecordView()
        {
            InitializeComponent();
        }
    }
}
