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
using Neoma.Models.ProjetViewModel;
using Neoma.RazorClassLib.Services;
using Neoma.RazorClassLib.Views.Emails.Postuler;
using Neoma.Services;
using Neoma.Utility;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.CommonEndUser + "," + SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    public class DetailProjetController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private IHubContext<MessagerieHub> _hub;
        private readonly IEmailSender _emailSender;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;


        public DetailProjetController(ApplicationDbContext db, IHubContext<MessagerieHub> hub, IEmailSender emailSender, IRazorViewToStringRenderer razorViewToStringRendere) :base(db)
        {
            _db = db;
            _hub = hub;
            _emailSender = emailSender;
            _razorViewToStringRenderer = razorViewToStringRendere;
        }

        public async Task<IActionResult> Index(int Id, string path)
        {
            if (Id != 0 || path != null)
            {
                ApplicationUser Utilisateur = await _db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefaultAsync();
                List<int> SpecialitesUtilisateurIds = await _db.SpecialiteUtilisateur.Where(s => s.UtilisateurId == Utilisateur.Id).Select(x => x.SpecialiteId).ToListAsync();

                var projet = await _db.Projet.Where(p => p.Id == Id).FirstOrDefaultAsync();
                var besoins = await _db.Besoins.Where(b => b.Projet == projet).ToListAsync();
                projet.Utilisateur = await _db.Users.Where(u => u.Id == projet.UtilisateurId).FirstOrDefaultAsync();
                projet.Utilisateur.Organisme = await _db.Organisme.Where(o => o.Id == projet.Utilisateur.OrganismeId).FirstOrDefaultAsync();
                projet.TypeProjet = await _db.TypeProjet.Where(t => t.Id == projet.TypeProjetId).FirstOrDefaultAsync();
                foreach (var besoin in besoins)
                {
                    var Candidatures = await _db.Candidature.Where(c => c.BesoinsId == besoin.Id).ToListAsync();
                    var specId = await _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == besoin.Id).Select(x => x.SpecialiteId).ToListAsync();
                    besoin.Specialite = await _db.Specialite.Where(s => specId.Contains(s.Id) && (s.Valide || s.UtilisateurId == Utilisateur.Id)).ToListAsync();
                    besoin.Role = await _db.Role.Where(r => r.Id == besoin.RoleId).FirstOrDefaultAsync();
                    besoin.Utilisateur = await _db.Users.Where(u => u.Id == besoin.UtilisateurId).FirstOrDefaultAsync();

                    besoin.PeutPostuler = false;
                    besoin.DejaPostuler = false;

                    besoin.DejaPostuler = besoin.DejaPostuler || Candidatures.Any(c => c.UtilisateurId == Utilisateur.Id);

                    foreach (int CetId in specId)
                    {
                        besoin.PeutPostuler = besoin.PeutPostuler || SpecialitesUtilisateurIds.Contains(CetId);
                    }

                    foreach (var specialite in besoin.Specialite)
                    {
                        if (SpecialitesUtilisateurIds.Contains(specialite.Id))
                        {
                            specialite.Match = true;
                        }
                        else
                        {
                            specialite.Match = false;
                        }

                    }
                }

                var specialiteUser = await _db.SpecialiteUtilisateur.Where(s => s.UtilisateurId == projet.Utilisateur.Id).ToListAsync();
                var roleUser = await _db.RoleUtilisateur.Where(r => r.UtilisateurId == projet.Utilisateur.Id).ToListAsync();
                foreach (var role in roleUser)
                {
                    role.Role = await _db.Role.Where(r => r.Id == role.RoleId).FirstOrDefaultAsync();
                    role.Specialite = new List<Specialite>();
                    foreach (var specialite in specialiteUser)
                    {
                        var spec = await _db.Specialite.Where(s => s.Id == specialite.SpecialiteId && (s.Valide || s.UtilisateurId == Utilisateur.Id)).FirstOrDefaultAsync();
                        if (spec != null && spec.RoleId == role.RoleId)
                        {
                            role.Specialite.Add(spec);
                        }
                    }
                }

                ListCandidatViewModel user = new ListCandidatViewModel()
                {
                    User = projet.Utilisateur,
                    RoleUser = roleUser
                };

                projet.Besoins = besoins;
                ProjetDetailViewModel pro = new ProjetDetailViewModel()
                {
                    Projet = projet,
                    ListCandidat = user
                };

                ViewBag.ArianeUrl = path == "Liste projets" ? Url.Action("Index", "ListeProjets") : Url.Action("Index", "MatchingCandidat");

                ViewBag.Path = path;
                ViewBag.Url = Request.Headers["Referer"].ToString();
                return View(pro);
            }

            return RedirectToAction("Index", "ListeProjets");
        }

        [HttpPost]
        public async Task<JsonResult> Postuler(int id)
        {
            var candidature = new Candidature();
            candidature.Besoins = await _db.Besoins.Where(b => b.Id == id).FirstOrDefaultAsync();
            candidature.Utilisateur = await _db.Users.Where(u => u.Id == User.getUserId()).FirstOrDefaultAsync();
            candidature.Projet = await _db.Projet.Where(p => p.Id == candidature.Besoins.ProjetId).FirstOrDefaultAsync();
            candidature.Statut = EValidation.Pending.ToString();
            _db.Add(candidature);
            await _db.SaveChangesAsync();

            var UserEmail = await _db.Users.Where(u => u.Id == candidature.Projet.UtilisateurId).Select(x => x.Email).FirstOrDefaultAsync();

            var UrlPage = Url.Action("RedirectPage",
                                     "SharedAction",
                                     new {
                                         userId = candidature.Projet.UtilisateurId,
                                         page = "Postuler",
                                         parameter1 = candidature.ProjetId,
                                         parameter2 = User.getUserId()
                                     }, 
                                     protocol: Request.Scheme);
            var link = new PostulerEmailViewModel(candidature.Projet.Name, candidature.Utilisateur.Prenom + " " + candidature.Utilisateur.Nom, UrlPage);
            string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Postuler/PostulerEmail.cshtml", link);
            await _emailSender.SendNewEmail(UserEmail, "Notification de candidature", body);

            await _hub.Clients.All.SendAsync("displayNotification");
            return Json(new { success = true, result = "/ListeProjets" });
        }
    }
}