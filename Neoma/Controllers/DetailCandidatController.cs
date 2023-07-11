using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Extensions;
using Neoma.Hubs;
using Neoma.Models;
using Neoma.Models.CandidatViewModel;
using Neoma.RazorClassLib.Services;
using Neoma.RazorClassLib.Views.Emails.AddSelection;
using Neoma.Services;
using Neoma.Utility;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.CommonEndUser + "," + SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    public class DetailCandidatController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private IHubContext<MessagerieHub> _hub;
        private readonly IEmailSender _emailSender;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        public DetailCandidatController(ApplicationDbContext db, IHubContext<MessagerieHub> hub, IEmailSender emailSender, IRazorViewToStringRenderer razorViewToStringRenderer) :base(db)
        {
            _db = db;
            _hub = hub;
            _emailSender = emailSender;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        public async Task<IActionResult> Index(string Id, string path)
        {
            if(Id != null || path != null)
            {
                var Projets = await _db.Projet.Where(p => p.UtilisateurId == User.getUserId() && !p.Complet).ToListAsync();
                var specialiteUser = await _db.SpecialiteUtilisateur.Where(s => s.UtilisateurId == Id).ToListAsync();
                var roleUser = await _db.RoleUtilisateur.Where(r => r.UtilisateurId == Id).ToListAsync();
                var Selections = new List<Selection>();
                foreach (var role in roleUser)
                {
                    role.Role = await _db.Role.Where(r => r.Id == role.RoleId).FirstOrDefaultAsync();
                    role.Role.PeutSelectionner = false;
                    role.Specialite = new List<Specialite>();
                    var Besoins = await _db.Besoins.Where(b => b.RoleId == role.RoleId && Projets.Exists(x => x.Id == b.ProjetId)).ToListAsync();
                    foreach (var specialite in specialiteUser)
                    {
                        var spec = await _db.Specialite.Where(s => s.Id == specialite.SpecialiteId).FirstOrDefaultAsync();
                        if (spec.RoleId == role.RoleId)
                        {
                            role.Specialite.Add(spec);
                        }
                    }
                    foreach (var besoin in Besoins)
                    {
                        var Selection = await _db.Selection.Where(s => s.BesoinsId == besoin.Id && s.UtilisateurId == Id).FirstOrDefaultAsync();
                        var Proposition = await _db.Proposition.Where(p => p.UtilisateurId == Id && p.BesoinsId == besoin.Id).FirstOrDefaultAsync();
                        bool UneSpecialiteRequise = _db.BesoinsSpecialite.Any(bs => bs.BesoinsId == besoin.Id && specialiteUser.Exists(su => su.SpecialiteId == bs.SpecialiteId));
                        if (Selection == null && besoin.UtilisateurId == null && UneSpecialiteRequise && Proposition == null)
                        {
                            role.Role.PeutSelectionner = true;
                        }
                    }
                }

                ListCandidatViewModel user = new ListCandidatViewModel()
                {
                    User = _db.Users.Where(u => u.Id == Id).FirstOrDefault(),
                    RoleUser = roleUser
                };

                ViewBag.ArianeUrl = path == "Liste co-surfeurs" ? Url.Action("Index", "Candidat") : Url.Action("Index", "MatchingPorteur");

                ViewBag.Path = path;
                ViewBag.Url = Request.Headers["Referer"].ToString();
                user.Organisme = await _db.Organisme.Where(o => o.Id == user.User.OrganismeId).FirstOrDefaultAsync();
                return View(user);
            }
            return RedirectToAction("Index", "Candidat");
        }

        [HttpPost]
        public async Task<IActionResult> AddSelection(Selection Selection)
        {
            if (ModelState.IsValid)
            {
                decimal Note = 0;
                bool noteOK = Selection.NoteString == null ? false : decimal.TryParse(Selection.NoteString/*.Replace(".", ",")*/, out Note);

                if (!noteOK)
                    noteOK = Selection.NoteString == null ? false : decimal.TryParse(Selection.NoteString.Replace(".", ","), out Note);

                if (noteOK)
                    Selection.Note = Note;

                Selection.DateCreation = DateTime.Today;
                var UserEmail = await _db.Users.Where(u => u.Id == Selection.UtilisateurId).Select(x => x.Email).FirstOrDefaultAsync();
                await _db.AddAsync(Selection);
                await _db.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("displayNotification");

                var link = new AddSelectionEmailViewModel(null);
                string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/AddSelection/AddSelectionEmail.cshtml", link);
                await _emailSender.SendNewEmail(UserEmail, "Notification de sélection", body);
            }

            return RedirectToAction("Index", "Selections");
        }
    }
}