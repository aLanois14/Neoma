using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.ViewComponents
{
    public class ConversationViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public ConversationViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int ConversationId, string user)
        {
            var Messages = await _db.Message.Where(m => m.ConversationId == ConversationId).OrderBy(m => m.DateEnvoi).ToListAsync();
            foreach(var message in Messages)
            {
                message.Utilisateur = await _db.Users.Where(u => u.Id == message.UtilisateurId).FirstOrDefaultAsync();
            }
            ViewBag.Conversation = ConversationId;
            ViewBag.User = user;
            return View(Messages);
        }
    }
}
