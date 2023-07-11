using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using System.Linq;
using System.Threading.Tasks;
using Neoma.Services;
using Neoma.Extensions;
using Microsoft.AspNetCore.Http;
using Neoma.Models;
using System;
using Neoma.Utility;
using Microsoft.AspNetCore.Identity;

namespace Neoma.Controllers
{
    public class SharedActionController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SharedActionController(ApplicationDbContext db, IEmailSender emailSender, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : base(db)
        {
            _db = db;
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> UpdateSpecialite(int role)
        {
            var Specialite = _db.Specialite.Where(s => s.RoleId == role).ToList();
            return new JsonResult(Specialite);
        }

        public async Task<IActionResult> NewConversation(string user)
        {
            var conversationUser = await _db.ConversationUtilisateur.Where(c => c.UtilisateurId == User.getUserId()).ToListAsync();
            var conversationContact = await _db.ConversationUtilisateur.Where(c => conversationUser.Exists(x => x.ConversationId == c.ConversationId) && c.UtilisateurId == user).FirstOrDefaultAsync();
            if(conversationContact == null)
            {
                
                return RedirectToAction("Index", "Messagerie", new { Area = "", conversationId = -1, user = user });
            }
            else
            {
                return RedirectToAction("Index", "Messagerie", new { Area = "", conversationId = conversationContact.ConversationId, user = user });
            }          
        }

        public async Task<IActionResult> RedirectPage(string userId, string page, int parameter1 = 0, string parameter2 = null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _signInManager.SignInAsync(user, isPersistent: false);
            /*HttpContext.Session.Set("Roles", new byte[5] {
                                                user.EstCandidat ? (byte)1 : (byte)0,
                                                user.EstPorteur ? (byte)1 : (byte)0,
                                                await _signInManager.UserManager.IsInRoleAsync(user, SD.AdminEndUser) ? (byte)1 : (byte)0,
                                                user.EstAficionado ? (byte)1 : (byte)0,
                                                await _signInManager.UserManager.IsInRoleAsync(user, SD.SuperEndUser) ? (byte)1 : (byte)0,
                                            });*/

            if (page == "RefuseC")
            {
                user.RoleActuel = "Co-surfeur";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "CandidatureCandidat", new { Area = "" });
            }
            else if (page == "Accept" || page == "Validate")
            {
                user.RoleActuel = "Co-surfeur";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("DetailProjet", "CandidatureRetenue", new { Area = "", Id = parameter1 });
            }
            else if (page == "Leave" || page == "RefuseP")
            {
                user.RoleActuel = "Fondateur";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "MesProjets", new { Area = "" });
            }
            else if (page == "Specialite")
            {
                user.RoleActuel = "Administrateur";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Specialite", new { Area = "Admin" });
            }
            else if (page == "Postuler")
            {
                user.RoleActuel = "Fondateur";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("DetailCandidature", "CandidaturePorteur", new { Area = "", user = parameter2, projet = parameter1 });
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
    }
}
