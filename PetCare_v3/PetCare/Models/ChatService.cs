using System;
using System.Collections.Generic;
using System.Linq;

namespace PetCare.Models
{
    public class ChatService
    {
        public List<ChatMessage> GetMessages(int programareID, int currentUserId)
        {
            using (var context = new PetCareEntities())
            {
                var messages = context.Mesajes
                    .Include("Utilizatori")
                    .Where(m => m.ProgramareID == programareID)
                    .OrderBy(m => m.DataOra)
                    .ToList();

                return messages.Select(m => new ChatMessage
                {
                    MesajID = m.MesajID,
                    SenderName = m.Utilizatori != null ? m.Utilizatori.Nume + " " + m.Utilizatori.Prenume : "Unknown",
                    Text = m.Continut,
                    DataOra = m.DataOra ?? DateTime.MinValue,
                    IsMe = m.ExpeditorID == currentUserId
                }).ToList();
            }
        }

        public bool SendMessage(int programareID, int expeditorID, string text)
        {
            using (var context = new PetCareEntities())
            {
                try
                {
                    var mesaj = new Mesaje
                    {
                        ProgramareID = programareID,
                        ExpeditorID = expeditorID,
                        Continut = text,
                        DataOra = DateTime.Now
                    };

                    context.Mesajes.Add(mesaj);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public int GetUnreadCount(int programareID, int currentUserId)
        {
             using (var context = new PetCareEntities())
            {

                return context.Mesajes.Count(m => m.ProgramareID == programareID && m.ExpeditorID != currentUserId && (m.IsCitit == null || m.IsCitit == false));
            }
        }

        public void MarkAsRead(int programareID, int currentUserId)
        {
            using (var context = new PetCareEntities())
            {

                var unreadMessages = context.Mesajes
                    .Where(m => m.ProgramareID == programareID && m.ExpeditorID != currentUserId && (m.IsCitit == null || m.IsCitit == false))
                    .ToList();

                foreach (var msg in unreadMessages)
                {
                    msg.IsCitit = true;
                }

                if (unreadMessages.Any())
                {
                    context.SaveChanges();
                }
            }
        }
    }
}
