using System;
using System.Collections.ObjectModel;
using System.Linq;
using PetCare.Models;

namespace PetCare.ViewModels
{
    public class ViewMedicalRecordViewModel : BaseViewModel
    {
        private FiseMedicale _record;

        public string Diagnostic => _record.Diagnostic;
        public string Tratament => _record.Tratament;
        public string DataCrearii => _record.DataCrearii?.ToString("dd.MM.yyyy") ?? "N/A";

        public ObservableCollection<string> Services { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<MaterialUsage> UsedMaterials { get; set; } = new ObservableCollection<MaterialUsage>();

        public decimal TotalCost { get; set; }

        public ViewMedicalRecordViewModel(FiseMedicale record)
        {
            _record = record;
            LoadDetails();
        }

        private void LoadDetails()
        {

            if (_record.DetaliiFisa_Servicii != null)
            {
                foreach (var s in _record.DetaliiFisa_Servicii)
                {
                    Services.Add($"{s.Servicii.Denumire} - {s.PretAplicat:F2} RON");
                    TotalCost += s.PretAplicat ?? 0;
                }
            }


            if (_record.DetaliiFisa_Materiale != null)
            {
                foreach (var m in _record.DetaliiFisa_Materiale)
                {
                    decimal cost = (m.CantitateConsumata * (m.Stocuri.PretUnitar ?? 0));
                    UsedMaterials.Add(new MaterialUsage
                    {
                        NumeMaterial = m.Stocuri.DenumireProdus,
                        Cantitate = m.CantitateConsumata,
                        Cost = cost
                    });
                     TotalCost += cost;
                }
            }
        }
    }
}
