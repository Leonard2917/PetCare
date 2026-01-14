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


        private string _clinicName;
        private string _clinicAddress;
        private string _clinicPhone;
        private string _clinicCUI;

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
        public string ClinicCUI
        {
            get => _clinicCUI;
            set { _clinicCUI = value; OnPropertyChanged(nameof(ClinicCUI)); }
        }


        public ObservableCollection<Service> Services { get; set; }
        private Service _selectedService;
        public Service SelectedService
        {
            get => _selectedService;
            set { _selectedService = value; OnPropertyChanged(nameof(SelectedService)); }
        }


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


        public ObservableCollection<MedicInfo> Medics { get; set; }


        public ObservableCollection<MedicRequest> IncomingRequests { get; set; } = new ObservableCollection<MedicRequest>();
        public int IncomingRequestsCount => IncomingRequests.Count;


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

        public StatisticsViewModel Statistics { get; set; }



        public AdminDashboardViewModel(UserModel user)
        {
            _clinicService = new ClinicService();
            _stockService = new StockService();
            _adminID = user.UtilizatorID; 
            
            Statistics = new StatisticsViewModel(user);

            var id = _clinicService.GetAdminClinicID(_adminID);
            if (id.HasValue)
            {
                _clinicID = id.Value;
                LoadClinicDetails();
                Services = new ObservableCollection<Service>();
                Medics = new ObservableCollection<MedicInfo>();
                StockList = new ObservableCollection<StockModel>();
                
                LoadServices();
                LoadMedics();
                LoadStock();
                LoadIncomingRequests();
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
            ClinicCUI = details.CUI;
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

        public void LoadIncomingRequests()
        {
            var medicService = new MedicService();
            var list = medicService.GetIncomingRequestsForClinic(_clinicID);
            IncomingRequests.Clear();
            foreach (var r in list) IncomingRequests.Add(r);
            OnPropertyChanged(nameof(IncomingRequestsCount));
        }





        public void SaveClinic()
        {
            bool success = _clinicService.UpdateClinicDetails(_clinicID, ClinicName, ClinicAddress, ClinicPhone, ClinicCUI);
            if (success) MessageBox.Show("Datele clinicii actualizate!");
            else MessageBox.Show("Eroare la salvare.");
        }


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

        public void DeleteService(Service service)
        {
            if (service != null)
            {
                bool success = _clinicService.DeleteService(service.ServiciuID);
                if (success) LoadServices();
                else MessageBox.Show("Nu se poate șterge serviciul (posibil folosit în fișe).");
            }
        }


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


        public void ApproveRequest(MedicRequest request)
        {
            if (request == null) return;
            var medicService = new MedicService();
            if (medicService.RespondToRequest(request.CerereID, true))
            {
                MessageBox.Show("Medic aprobat cu succes!");
                LoadIncomingRequests();
                LoadMedics();
            }
        }

        public void RejectRequest(MedicRequest request)
        {
            if (request == null) return;
            var medicService = new MedicService();
            if (medicService.RespondToRequest(request.CerereID, false))
            {
                MessageBox.Show("Cerere respinsă.");
                LoadIncomingRequests();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
