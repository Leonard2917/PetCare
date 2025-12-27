using PetCare.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PetCare.ViewModels
{
    public class AdminDashboardViewModel : INotifyPropertyChanged
    {
        private readonly ClinicService _clinicService;
        private readonly StockService _stockService;
        private int _adminID;
        private int _clinicID;

        // Clinic Details
        private string _clinicName;
        private string _clinicAddress;
        private string _clinicPhone;

        public string ClinicName
        {
            get => _clinicName;
            set { _clinicName = value; OnPropertyChanged(nameof(ClinicName)); }
        }
        public string ClinicAddress
        {
            get => _clinicAddress;
            set { _clinicAddress = value; OnPropertyChanged(nameof(ClinicAddress)); }
        }
        public string ClinicPhone
        {
            get => _clinicPhone;
            set { _clinicPhone = value; OnPropertyChanged(nameof(ClinicPhone)); }
        }

        // Services
        public ObservableCollection<ServiceDTO> Services { get; set; }
        private ServiceDTO _selectedService;
        public ServiceDTO SelectedService
        {
            get => _selectedService;
            set { _selectedService = value; OnPropertyChanged(nameof(SelectedService)); }
        }

        // Inputs for Service
        private string _newServiceName;
        private decimal _newServicePrice;
        private int _newServiceDuration = 30;

        public string NewServiceName
        {
            get => _newServiceName;
            set { _newServiceName = value; OnPropertyChanged(nameof(NewServiceName)); }
        }
        public decimal NewServicePrice
        {
            get => _newServicePrice;
            set { _newServicePrice = value; OnPropertyChanged(nameof(NewServicePrice)); }
        }
        public int NewServiceDuration
        {
            get => _newServiceDuration;
            set { _newServiceDuration = value; OnPropertyChanged(nameof(NewServiceDuration)); }
        }

        // Medics
        public ObservableCollection<MedicInfoDTO> Medics { get; set; }

        // Stock / Inventory
        public ObservableCollection<StockModel> StockList { get; set; }
        private StockModel _selectedStock;
        public StockModel SelectedStock
        {
            get => _selectedStock;
            set { _selectedStock = value; OnPropertyChanged(nameof(SelectedStock)); }
        }

        private string _newProdus;
        private string _newUnitate = "buc";
        private decimal _newPret;
        private decimal _newCantitate;

        public string NewProdus
        {
            get => _newProdus;
            set { _newProdus = value; OnPropertyChanged(nameof(NewProdus)); }
        }
        public string NewUnitate
        {
            get => _newUnitate;
            set { _newUnitate = value; OnPropertyChanged(nameof(NewUnitate)); }
        }
        public decimal NewPret
        {
            get => _newPret;
            set { _newPret = value; OnPropertyChanged(nameof(NewPret)); }
        }
         public decimal NewCantitate
        {
            get => _newCantitate;
            set { _newCantitate = value; OnPropertyChanged(nameof(NewCantitate)); }
        }


        // Constructor
        public AdminDashboardViewModel(UserModel user)
        {
            _clinicService = new ClinicService();
            _stockService = new StockService();
            _adminID = user.UtilizatorID; // UserID is passed
            var id = _clinicService.GetAdminClinicID(_adminID);
            if (id.HasValue)
            {
                _clinicID = id.Value;
                LoadClinicDetails();
                Services = new ObservableCollection<ServiceDTO>();
                Medics = new ObservableCollection<MedicInfoDTO>();
                StockList = new ObservableCollection<StockModel>();
                
                LoadServices();
                LoadMedics();
                LoadStock();
            }
            else
            {
                MessageBox.Show("Eroare: Acest administrator nu are o clinică asociată!");
            }
        }

        private void LoadClinicDetails()
        {
            var details = _clinicService.GetClinicDetails(_clinicID);
            ClinicName = details.Nume;
            ClinicAddress = details.Adresa;
            ClinicPhone = details.Telefon;
        }

        private void LoadServices()
        {
            Services.Clear();
            var list = _clinicService.GetClinicServices(_clinicID);
            foreach (var s in list) Services.Add(s);
        }

        private void LoadMedics()
        {
            Medics.Clear();
            var list = _clinicService.GetClinicMedics(_clinicID);
            foreach (var m in list) Medics.Add(m);
        }

        private void LoadStock()
        {
            StockList.Clear();
            var list = _stockService.GetStocks(_clinicID);
            foreach (var s in list) StockList.Add(s);
        }


        // Start Public Methods for Code-Behind

        // Clinic Logic
        public void SaveClinic()
        {
            bool success = _clinicService.UpdateClinicDetails(_clinicID, ClinicName, ClinicAddress, ClinicPhone);
            if (success) MessageBox.Show("Datele clinicii actualizate!");
            else MessageBox.Show("Eroare la salvare.");
        }

        // Service Logic
        public void AddService()
        {
            if (string.IsNullOrWhiteSpace(NewServiceName)) return;
            bool success = _clinicService.AddService(_clinicID, NewServiceName, NewServicePrice, NewServiceDuration);
            if (success)
            {
                LoadServices();
                NewServiceName = "";
                NewServicePrice = 0;
                NewServiceDuration = 30;
                MessageBox.Show("Serviciu adăugat!");
            }
        }

        public void UpdateService()
        {
             if (SelectedService == null) return;
             bool success = _clinicService.UpdateService(SelectedService.ServiciuID, SelectedService.Denumire, SelectedService.PretStandard, SelectedService.DurataEstimataMinute);
             if (success) MessageBox.Show("Serviciu actualizat!");
        }

        public void DeleteService(ServiceDTO service)
        {
            if (service != null)
            {
                bool success = _clinicService.DeleteService(service.ServiciuID);
                if (success) LoadServices();
                else MessageBox.Show("Nu se poate șterge serviciul (posibil folosit în fișe).");
            }
        }

        // Stock Logic
        public void AddStock()
        {
             if (string.IsNullOrWhiteSpace(NewProdus)) return;
             bool success = _stockService.AddStock(_clinicID, NewProdus, NewUnitate, NewCantitate, NewPret);
             if (success)
             {
                 LoadStock();
                 NewProdus = "";
                 NewPret = 0;
                 NewCantitate = 0;
                 MessageBox.Show("Produs adăugat în stoc!");
             }
        }
        
        public void UpdateStock()
        {
            if (SelectedStock == null) return;
            bool success = _stockService.UpdateStock(SelectedStock.StocID, SelectedStock.DenumireProdus, SelectedStock.UnitateMasura, SelectedStock.CantitateDisponibila, SelectedStock.PretUnitar);
            if (success) MessageBox.Show("Stoc actualizat!");
        }

        public void DeleteStock(StockModel stoc)
        {
            if (stoc != null)
            {
                bool success = _stockService.DeleteStock(stoc.StocID);
                if (success) LoadStock();
                else MessageBox.Show("Eroare la ștergere.");
            }
        }

        // Boilerplate INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
