using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PetCare.Models;

namespace PetCare.ViewModels
{
    public class BookAppointmentViewModel : BaseViewModel
    {
        private readonly UserModel _loggedInUser;
        private int? _proprietarID;
        
        private readonly OwnerService _ownerService;
        private readonly ClinicService _clinicService;
        private readonly MedicService _medicService;
        private readonly AppointmentService _appointmentService;

        // Step 1: Animal & Clinic
        public ObservableCollection<AnimalModel> MyAnimals { get; set; } = new ObservableCollection<AnimalModel>();
        private AnimalModel _selectedAnimal;
        public AnimalModel SelectedAnimal
        {
            get => _selectedAnimal;
            set
            {
                _selectedAnimal = value;
                OnPropertyChanged(nameof(SelectedAnimal));
                OnPropertyChanged(nameof(IsStep2Enabled));
                if (_selectedAnimal != null) LoadClinics();
            }
        }

        public ObservableCollection<ClinicDTO> AllClinics { get; set; } = new ObservableCollection<ClinicDTO>();
        private ClinicDTO _selectedClinic;
        public ClinicDTO SelectedClinic
        {
            get => _selectedClinic;
            set
            {
                _selectedClinic = value;
                OnPropertyChanged(nameof(SelectedClinic));
                OnPropertyChanged(nameof(IsStep3Enabled));
                if (_selectedClinic != null) LoadServices();
            }
        }

        // Step 2: Services
        public ObservableCollection<ServiceDTO> ClinicServices { get; set; } = new ObservableCollection<ServiceDTO>();
        public ObservableCollection<ServiceDTO> SelectedServices { get; set; } = new ObservableCollection<ServiceDTO>();

        private int _totalDuration;
        public int TotalDuration
        {
            get => _totalDuration;
            set { _totalDuration = value; OnPropertyChanged(nameof(TotalDuration)); OnPropertyChanged(nameof(TotalDurationDisplay)); }
        }
        public string TotalDurationDisplay => $"{TotalDuration} minute";

        // Step 3: Medic
        public ObservableCollection<MedicInfoDTO> AvailableMedics { get; set; } = new ObservableCollection<MedicInfoDTO>();
        private MedicInfoDTO _selectedMedic;
        public MedicInfoDTO SelectedMedic
        {
            get => _selectedMedic;
            set
            {
                _selectedMedic = value;
                OnPropertyChanged(nameof(SelectedMedic));
                OnPropertyChanged(nameof(IsStep5Enabled));
                if (_selectedMedic != null) LoadAvailableDates();
            }
        }

        // Step 4: Date
        public ObservableCollection<DateTime> AvailableDates { get; set; } = new ObservableCollection<DateTime>();
        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                OnPropertyChanged(nameof(IsStep6Enabled));
                if (_selectedDate.HasValue) LoadTimeSlots();
            }
        }

        // Step 5: Time
        public ObservableCollection<TimeSlot> AvailableTimeSlots { get; set; } = new ObservableCollection<TimeSlot>();
        private TimeSlot _selectedTimeSlot;
        public TimeSlot SelectedTimeSlot
        {
            get => _selectedTimeSlot;
            set
            {
                _selectedTimeSlot = value;
                OnPropertyChanged(nameof(SelectedTimeSlot));
                OnPropertyChanged(nameof(CanConfirmBooking));
            }
        }

        // UI State
        public bool IsStep2Enabled => SelectedAnimal != null;
        public bool IsStep3Enabled => SelectedClinic != null;
        public bool IsStep4Enabled => SelectedServices.Count > 0;
        public bool IsStep5Enabled => SelectedMedic != null;
        public bool IsStep6Enabled => SelectedDate.HasValue;
        public bool CanConfirmBooking => SelectedTimeSlot != null;

        public BookAppointmentViewModel(UserModel user)
        {
            _loggedInUser = user;
            _ownerService = new OwnerService();
            _clinicService = new ClinicService();
            _medicService = new MedicService();
            _appointmentService = new AppointmentService();

            LoadAnimals();
            
            // Listen to SelectedServices changes
            SelectedServices.CollectionChanged += (s, e) => 
            {
                CalculateTotalDuration();
                OnPropertyChanged(nameof(IsStep4Enabled));
                if (SelectedServices.Count > 0 && SelectedAnimal != null && SelectedClinic != null)
                    LoadMedics();
            };
        }

        private void LoadAnimals()
        {
            _proprietarID = _ownerService.GetProprietarID(_loggedInUser.UtilizatorID);

            if (_proprietarID.HasValue)
            {
                var animals = _ownerService.GetAnimals(_proprietarID.Value);
                MyAnimals.Clear();
                foreach (var animal in animals)
                {
                    MyAnimals.Add(animal);
                }
            }
        }

        private void LoadClinics()
        {
            var clinics = _clinicService.GetAllClinicsForBooking();
            AllClinics.Clear();
            foreach (var clinic in clinics)
            {
                AllClinics.Add(clinic);
            }
        }

        private void LoadServices()
        {
            if (SelectedClinic == null) return;

            var services = _clinicService.GetClinicServices(SelectedClinic.ClinicaID);
            ClinicServices.Clear();
            foreach (var service in services)
            {
                ClinicServices.Add(service);
            }
        }

        private void CalculateTotalDuration()
        {
            TotalDuration = SelectedServices.Sum(s => s.DurataEstimataMinute);
        }

        private void LoadMedics()
        {
            if (SelectedAnimal == null || SelectedClinic == null || SelectedServices.Count == 0) return;

            var medics = _medicService.GetAvailableMedicsForAnimal(SelectedClinic.ClinicaID, SelectedAnimal.SpecieID);
            AvailableMedics.Clear();
            foreach (var medic in medics)
            {
                AvailableMedics.Add(medic);
            }

            if (AvailableMedics.Count == 0)
            {
                MessageBox.Show("Nu există medici disponibili pentru această specie la clinica selectată.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadAvailableDates()
        {
            if (SelectedMedic == null || SelectedClinic == null) return;

            var workingDays = _medicService.GetMedicWorkingDays(SelectedMedic.MedicID, SelectedClinic.ClinicaID);

            AvailableDates.Clear();
            var today = DateTime.Today;
            for (int i = 0; i < 30; i++) // Next 30 days
            {
                var date = today.AddDays(i);
                var dayOfWeek = ((int)date.DayOfWeek == 0) ? 7 : (int)date.DayOfWeek;
                
                if (workingDays.Contains(dayOfWeek))
                {
                    AvailableDates.Add(date);
                }
            }
        }

        private void LoadTimeSlots()
        {
            if (!SelectedDate.HasValue || SelectedMedic == null || SelectedClinic == null || TotalDuration == 0) return;

            var slots = _appointmentService.GetAvailableTimeSlots(
                SelectedMedic.MedicID, 
                SelectedClinic.ClinicaID, 
                SelectedDate.Value, 
                TotalDuration);

            AvailableTimeSlots.Clear();
            foreach (var slot in slots)
            {
                AvailableTimeSlots.Add(slot);
            }

            if (AvailableTimeSlots.Count == 0)
            {
                MessageBox.Show("Nu există ore disponibile pentru data selectată. Vă rugăm să alegeți o altă dată.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void ConfirmBooking()
        {
            if (!CanConfirmBooking) return;

            var appointmentDateTime = SelectedDate.Value.Date.Add(SelectedTimeSlot.StartTime);
            var serviceIDs = SelectedServices.Select(s => s.ServiciuID).ToList();

            bool success = _appointmentService.CreateAppointment(
                SelectedAnimal.AnimalID,
                SelectedClinic.ClinicaID,
                SelectedMedic.MedicID,
                appointmentDateTime,
                serviceIDs);

            if (success)
            {
                MessageBox.Show("Programare creată cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                // Navigate back to owner dashboard
                Application.Current.MainWindow.Content = new Views.OwnerDashboardView(_loggedInUser);
            }
            else
            {
                MessageBox.Show("Eroare la crearea programării. Vă rugăm să încercați din nou.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Cancel()
        {
            Application.Current.MainWindow.Content = new Views.OwnerDashboardView(_loggedInUser);
        }
    }
}
