using System;
using System.Collections.Generic;
using System.Linq;

namespace PetCare.Models
{
    public class StockService
    {
        public List<StockModel> GetStocks(int clinicaID)
        {
            using (var context = new PetCareEntities())
            {
                return context.Stocuris
                    .Where(s => s.ClinicaID == clinicaID)
                    .Select(s => new StockModel
                    {
                        StocID = s.StocID,
                        ClinicaID = s.ClinicaID.Value,
                        DenumireProdus = s.DenumireProdus,
                        UnitateMasura = s.UnitateMasura,
                        CantitateDisponibila = s.CantitateDisponibila.Value,
                        PretUnitar = s.PretUnitar.Value
                    }).ToList();
            }
        }

        public bool AddStock(int clinicaID, string denumireProdus, string unitateMasura, decimal cantitate, decimal pret)
        {
            using (var context = new PetCareEntities())
            {
                try
                {
                    var newStock = new Stocuri
                    {
                        ClinicaID = clinicaID,
                        DenumireProdus = denumireProdus,
                        UnitateMasura = unitateMasura,
                        CantitateDisponibila = cantitate,
                        PretUnitar = pret
                    };
                    context.Stocuris.Add(newStock);
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool UpdateStock(int stocID, string denumireProdus, string unitateMasura, decimal cantitate, decimal pret)
        {
            using (var context = new PetCareEntities())
            {
                try
                {
                    var stock = context.Stocuris.Find(stocID);
                    if (stock != null)
                    {
                        stock.DenumireProdus = denumireProdus;
                        stock.UnitateMasura = unitateMasura;
                        stock.CantitateDisponibila = cantitate;
                        stock.PretUnitar = pret;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool DeleteStock(int stocID)
        {
            using (var context = new PetCareEntities())
            {
                try
                {
                    var stock = context.Stocuris.Find(stocID);
                    if (stock != null)
                    {
                        context.Stocuris.Remove(stock);
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
