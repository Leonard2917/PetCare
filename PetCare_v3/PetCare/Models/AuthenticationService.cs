using System;
using System.Linq;

namespace PetCare.Models
{
    public class AuthenticationService
    {
        public UserModel Authenticate(string email, string password)
        {
            using (var context = new PetCareEntities())
            {
                string hashedPassword = Utilities.PasswordHelper.HashPassword(password);

                var userEntity = context.Utilizatoris
                                        .FirstOrDefault(u => u.Email == email && u.ParolaHash == hashedPassword);

                if (userEntity != null)
                {

                    return new UserModel
                    {
                        UtilizatorID = userEntity.UtilizatorID,
                        Nume = userEntity.Nume,
                        Prenume = userEntity.Prenume,
                        Email = userEntity.Email,
                        ParolaHash = userEntity.ParolaHash,
                        Rol = userEntity.Rol,
                        Telefon = userEntity.Telefon
                    };
                }

                return null;
            }
        }

        public string RegisterUser(string nume, string prenume, string email, string password, string telefon, string rol, int? clinicaId, string clinicaNume, string clinicaAdresa, string clinicaTelefon, string clinicaCUI = null, string nrParafa = null)
        {
            using (var context = new PetCareEntities())
            {

                if (context.Utilizatoris.Any(u => u.Email == email))
                {
                    return "Acest email este deja utilizat.";
                }


                if (rol == "Admin")
                {
                    if (context.Clinicis.Any(c => c.Nume == clinicaNume))
                    {
                        return "O clinică cu acest nume există deja în sistem.";
                    }
                }

                var hashedPassword = Utilities.PasswordHelper.HashPassword(password);

                var newUser = new Utilizatori
                {
                    Nume = nume,
                    Prenume = prenume,
                    Email = email,
                    ParolaHash = hashedPassword,
                    Telefon = telefon,
                    Rol = rol
                };

                context.Utilizatoris.Add(newUser);
                context.SaveChanges();


                if (rol == "Admin")
                {
                    var newClinic = new Clinici
                    {
                        Nume = clinicaNume,
                        Adresa = clinicaAdresa,
                        Telefon = clinicaTelefon,
                        CUI = clinicaCUI
                    };
                    context.Clinicis.Add(newClinic);
                    context.SaveChanges();

                    var newAdmin = new Administratori
                    {
                        UtilizatorID = newUser.UtilizatorID,
                        ClinicaID = newClinic.ClinicaID
                    };
                    context.Administratoris.Add(newAdmin);
                    context.SaveChanges();
                }

                else if (rol == "Medic")
                {
                    var newMedic = new Medici
                    {
                        UtilizatorID = newUser.UtilizatorID,
                        ClinicaID = null, 
                        NrParafa = nrParafa
                    };
                    context.Medicis.Add(newMedic);
                    context.SaveChanges();
                }

                else if (rol == "Owner")
                {

                }

                return null;
            }
        }
    }
}
