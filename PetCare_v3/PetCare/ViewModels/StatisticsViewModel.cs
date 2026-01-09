using System.Collections.ObjectModel;
using System.Linq;
using PetCare.Models;

namespace PetCare.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        private readonly StatisticsService _statisticsService;
        private readonly UserModel _currentUser;

        public ObservableCollection<ClinicStatDTO> TopClinics { get; set; } = new ObservableCollection<ClinicStatDTO>();
        public ObservableCollection<MedicStatDTO> TopMedics { get; set; } = new ObservableCollection<MedicStatDTO>();
        public ObservableCollection<MaterialStatDTO> TopMaterials { get; set; } = new ObservableCollection<MaterialStatDTO>();
        public ObservableCollection<RevenueStatDTO> RevenueStats { get; set; } = new ObservableCollection<RevenueStatDTO>();
        public ObservableCollection<AnimalTypeStatDTO> AnimalStats { get; set; } = new ObservableCollection<AnimalTypeStatDTO>();

        public bool IsAdmin => _currentUser?.Rol == "Admin";

        public StatisticsViewModel(UserModel user)
        {
            _currentUser = user;
            _statisticsService = new StatisticsService();
            LoadStats();
        }

        public void LoadStats()
        {
            // Stats for everyone
            var clinics = _statisticsService.GetTopClinics();
            TopClinics.Clear();
            foreach (var c in clinics) TopClinics.Add(c);

            var medics = _statisticsService.GetTopMedics();
            TopMedics.Clear();
            foreach (var m in medics) TopMedics.Add(m);

            var animals = _statisticsService.GetAnimalTypeDistribution();
            AnimalStats.Clear();
            foreach (var a in animals) AnimalStats.Add(a);

            // Admin only stats
            if (IsAdmin)
            {
                var materials = _statisticsService.GetTopConsumedMaterials();
                TopMaterials.Clear();
                foreach (var mat in materials) TopMaterials.Add(mat);

                int? clinicaId = GetAdminClinicId(_currentUser.UtilizatorID);
                if (clinicaId.HasValue)
                {
                    var revenue = _statisticsService.GetTotalRevenuePerMedic(clinicaId.Value);
                    RevenueStats.Clear();
                    foreach (var r in revenue) RevenueStats.Add(r);
                }
            }
        }

        private int? GetAdminClinicId(int userId)
        {
            using (var context = new PetCareEntities())
            {
                return context.Administratoris
                    .Where(a => a.UtilizatorID == userId)
                    .Select(a => a.ClinicaID)
                    .FirstOrDefault();
            }
        }
    }
}
