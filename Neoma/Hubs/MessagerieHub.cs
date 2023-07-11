using Microsoft.AspNetCore.SignalR;
using Neoma.Data;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Neoma.Models;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Neoma.Hubs
{
    public class MessagerieHub : Hub
    {
        private readonly ApplicationDbContext _db;

        public MessagerieHub(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task SendMessageToGroup(Message message)
        {
            //Récupération de l'utilisateur 
            var convUser = await _db.ConversationUtilisateur.Where(cu => cu.UtilisateurId != message.UtilisateurId && cu.ConversationId == message.ConversationId).FirstOrDefaultAsync();
            if(convUser.UtilisateurId == Context.UserIdentifier || message.UtilisateurId == Context.UserIdentifier)
            {

                //Récupération de la photo de l'utilisateur qui envoi le message
                var senderPhoto64 = Convert.ToBase64String(message.Utilisateur.Photo);
                var senderPhoto = string.Format("data:image/jpg;base64,{0}", senderPhoto64);

                //Formatage de la date pour afficher l'heure d'envoi
                var dateMessage = message.DateEnvoi.ToString("HH:mm");

                //Formatage de la date pour le séparateur
                var dateEnvoi = message.DateEnvoi.ToString("D", CultureInfo.CreateSpecificCulture("fr-FR"));

                //Formatage de la date pour l'affichage dans la liste des contacts
                var dateLast = message.DateEnvoi.ToString("dd/MM/yy");

                //Récupération de l'utilisateur afin d'afficher ses informations dans la liste des contacts lors de l'envoie du message
                var User = await _db.Users.Where(u => u.Id == convUser.UtilisateurId).FirstOrDefaultAsync();
                var receiverPhoto64 = Convert.ToBase64String(User.Photo);
                var receiverPhoto = string.Format("data:image/jpg;base64,{0}", receiverPhoto64);
                //Si le client qui reçoit le signal a le même ID que celui qui envoie le message affiche le message à droite
                if (Context.UserIdentifier != message.UtilisateurId)
                {

                    await Clients.Caller.SendAsync("ReceiverMessage", message, dateMessage, dateEnvoi, senderPhoto);

                    
                    await Clients.Caller.SendAsync("UpdateContactReceiver", message, dateLast, senderPhoto);
                }
                else
                {

                    await Clients.Caller.SendAsync("SenderMessage", message, dateMessage, dateEnvoi, senderPhoto);

                    
                    await Clients.Caller.SendAsync("UpdateContactSender", message.Text, dateLast, message.ConversationId, User, receiverPhoto);
                }
            }
        }

        public async Task SendUpdateContact()
        {
            var conversationId = await _db.ConversationUtilisateur.Where(cu => cu.UtilisateurId == Context.UserIdentifier).ToListAsync();
            var ConversationUser = await _db.ConversationUtilisateur.Where(c => conversationId.Exists(x => x.ConversationId == c.ConversationId && x.UtilisateurId != c.UtilisateurId)).ToListAsync();
            foreach (var conversation in ConversationUser)
            {
                conversation.Utilisateur = await _db.Users.Where(u => u.Id == conversation.UtilisateurId).FirstOrDefaultAsync();
                conversation.Utilisateur.Presence = conversation.Utilisateur.LastAction.AddSeconds(30) >= DateTime.Now ? 1 : conversation.Utilisateur.LastAction.AddMinutes(5) <= DateTime.Now ? 2 : 3;
                conversation.Conversation = await _db.Conversation.Where(c => c.Id == conversation.ConversationId).FirstOrDefaultAsync();
                var Message = await _db.Message.Where(m => m.ConversationId == conversation.ConversationId).OrderByDescending(d => d.DateEnvoi).FirstOrDefaultAsync();
                conversation.LastMessage = Message;
            }

            await Clients.Caller.SendAsync("ReceiveUpdateContact", ConversationUser);
        }
    }
}
