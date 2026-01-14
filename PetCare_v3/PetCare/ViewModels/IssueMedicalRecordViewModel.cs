using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PetCare.Models;

namespace PetCare.ViewModels
{
    public class IssueMedicalRecordViewModel : BaseViewModel
    {
        private readonly Appointment _appointment;
        private readonly MedicalRecordService _medicalRecordService;
        private readonly StockService _stockService;

        private string _diagnostic;
        public string Diagnostic
        {
            get => _diagnostic;
            set { _diagnostic = value; OnPropertyChanged(nameof(Diagnostic)); }
        }

        private string _tratament;
        public string Tratament
        {
            get => _tratament;
            set { _tratament = value; OnPropertyChanged(nameof(Tratament)); }
        }

        public ObservableCollection<MaterialUsage> UsedMaterials { get; set; } = new ObservableCollection<MaterialUsage>();
        public ObservableCollection<StockModel> AvailableMaterials { get; set; } = new ObservableCollection<StockModel>();

        private decimal _totalCost;
        public decimal TotalCost
        {
            get => _totalCost;
            set { _totalCost = value; OnPropertyChanged(nameof(TotalCost)); }
        }

        private bool _addReminder;
        public bool AddReminder
        {
            get => _addReminder;
            set { _addReminder = value; OnPropertyChanged(nameof(AddReminder)); }
        }

        private DateTime _reminderDate = DateTime.Now.AddDays(1);
        public DateTime ReminderDate
        {
            get => _reminderDate;
            set { _reminderDate = value; OnPropertyChanged(nameof(ReminderDate)); }
        }

        private string _reminderMessage;
        public string ReminderMessage
        {
            get => _reminderMessage;
            set { _reminderMessage = value; OnPropertyChanged(nameof(ReminderMessage)); }
        }

        private StockModel _selectedAvailableMaterial;
        public StockModel SelectedAvailableMaterial
        {
            get => _selectedAvailableMaterial;
            set { _selectedAvailableMaterial = value; OnPropertyChanged(nameof(SelectedAvailableMaterial)); }
        }

        private string _materialQuantity;
        public string MaterialQuantity
        {
            get => _materialQuantity;
            set { _materialQuantity = value; OnPropertyChanged(nameof(MaterialQuantity)); }
        }

        public IssueMedicalRecordViewModel(Appointment appointment)
        {
            _appointment = appointment;
            _medicalRecordService = new MedicalRecordService();
            _stockService = new StockService();
            LoadMaterials();
            CalculateInitialCost();
        }

        private void CalculateInitialCost()
        {
             TotalCost = _medicalRecordService.GetAppointmentServicesCost(_appointment.ProgramareID);
        }

        private void LoadMaterials()
        {
            var stocks = _stockService.GetStocks(_appointment.ClinicaID);
            foreach (var s in stocks)
            {
                AvailableMaterials.Add(s);
            }
        }

        public void AddMaterial()
        {
            if (SelectedAvailableMaterial == null) return;
            if (decimal.TryParse(MaterialQuantity, out decimal qty))
            {
                UsedMaterials.Add(new MaterialUsage
                {
                    StocID = SelectedAvailableMaterial.StocID,
                    NumeMaterial = SelectedAvailableMaterial.DenumireProdus,
                    Cantitate = qty
                });

                TotalCost += qty * (SelectedAvailableMaterial.PretUnitar);

                MaterialQuantity = "";
            }
        }

        public string Save()
        {

            List<int> serviceIDs = _medicalRecordService.GetAppointmentServiceIDs(_appointment.ProgramareID); 

            string error = _medicalRecordService.SaveMedicalRecord(
                _appointment.ProgramareID,
                !string.IsNullOrWhiteSpace(Diagnostic) ? Diagnostic : "N/A",
                Tratament,
                serviceIDs,
                UsedMaterials.ToList(),
                AddReminder,
                AddReminder ? (DateTime?)ReminderDate : null,
                ReminderMessage
            );

            return error;
        }
    }
}
