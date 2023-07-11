using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Extensions;
using Neoma.Models;
using Neoma.Models.CandidatureViewModel;
using Neoma.Utility;
using Neoma.Hubs;
using Microsoft.AspNetCore.SignalR;
using Neoma.Models.CandidatureRetenueViewModel;
using Neoma.Services;
using Neoma.RazorClassLib.Services;
using Neoma.RazorClassLib.Views.Emails.RefuseProposition;
using Neoma.RazorClassLib.Views.Emails.LeaveProject;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.CommonEndUser + "," + SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    public class CandidatureRetenueController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private IHubContext<MessagerieHub> _hub;
        private readonly IEmailSender _emailSender;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

        public CandidatureRetenueController(ApplicationDbContext db, IHubContext<MessagerieHub> hub, IEmailSender emailSender, IRazorViewToStringRenderer razorViewToStringRenderer) : base(db)
        {
            _db = db;
            _hub = hub;
            _emailSender = emailSender;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        #region Affichage vue
            public async Task<IActionResult> Index()
            {
                ApplicationUser Utilisateur = await _db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefaultAsync();
                var Candidatures = await _db.Candidature.Where(c => c.UtilisateurId == User.getUserId() && c.Statut == EValidation.Accepted.ToString()).ToListAsync();
                var Propositions = await _db.Proposition.Where(p => p.UtilisateurId == User.getUserId() && p.Statut == EValidation.Accepted.ToString()).ToListAsync();
                var Projets = await _db.Projet.Where(p => Candidatures.Exists(c => c.ProjetId == p.Id) || Propositions.Exists(x => x.ProjetId == p.Id)).ToListAsync();
                //var ProjetProposition = _db.Projet.Where(p => Propositions.Exists(x => x.ProjetId == p.Id)).ToList();
                var ListItemCandidatureRetenue = new List<ItemCandidatureRetenueViewModel>();
                var CandidatureViewModel = new List<CandidatureViewModel>();
                foreach(var projet in Projets)
                {
                    var User = await _db.Users.Where(u => u.Id == projet.UtilisateurId).FirstOrDefaultAsync();
                    
                    var CandidatureSelect = new List<ItemCandidatureViewModel>();
                    var PropositionSelect = new List<ItemPropositionViewModel>();
                    projet.Utilisateur = _db.Users.Where(u => u.Id == projet.UtilisateurId).FirstOrDefault();

                    foreach (var candidature in Candidatures)
                    {
                        if (candidature.ProjetId == projet.Id)
                        {
                            var Besoins = await _db.Besoins.Where(b => b.Id == candidature.BesoinsId).FirstOrDefaultAsync();
                            var Role = await _db.Role.Where(r => r.Id == Besoins.RoleId).FirstOrDefaultAsync();

                            var spec = _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == Besoins.Id).Select(x => x.SpecialiteId).ToList();
                            var Specialite = await _db.Specialite.Where(s => spec.Contains(s.Id) && (s.Valide || s.UtilisateurId == Utilisateur.Id)).ToListAsync();
                            CandidatureSelect.Add(new ItemCandidatureViewModel
                            {
                                Candidature = candidature,
                                ItemBesoin = new ItemBesoinViewModel
                                {
                                    Besoins = Besoins,
                                    Role = Role,
                                    Specialites = Specialite
                                }
                            });
                        }
                    }
                    foreach (var proposition in Propositions)
                    {
                        if (proposition.ProjetId == projet.Id)
                        {
                            var Besoins = await _db.Besoins.Where(b => b.Id == proposition.BesoinsId).FirstOrDefaultAsync();
                            var Role = await _db.Role.Where(r => r.Id == Besoins.RoleId).FirstOrDefaultAsync();

                            var spec = _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == Besoins.Id).Select(x => x.SpecialiteId).ToList();
                            var Specialite = await _db.Specialite.Where(s => spec.Contains(s.Id) && (s.Valide || s.UtilisateurId == Utilisateur.Id)).ToListAsync();
                            PropositionSelect.Add(new ItemPropositionViewModel
                            {
                                Proposition = proposition,
                                ItemBesoin = new ItemBesoinViewModel
                                {
                                    Besoins = Besoins,
                                    Role = Role,
                                    Specialites = Specialite
                                }
                            });
                        }
                    }

                    ListItemCandidatureRetenue.Add(new ItemCandidatureRetenueViewModel
                    {
                        User = User,
                        Organisme = await _db.Organisme.Where(o => o.Id == User.OrganismeId).FirstOrDefaultAsync(),
                        Projet = projet,
                        TypeProjet = await _db.TypeProjet.Where(t => t.Id == projet.TypeProjetId).FirstOrDefaultAsync(),
                        Candidatures = CandidatureSelect,
                        Propositions = PropositionSelect
                    });
                }

                return View(ListItemCandidatureRetenue);
            }

            public async Task<IActionResult> DetailProjet(int Id)
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
                    besoin.Utilisateur = await _db.ApplicationUser.Where(u => u.Id == besoin.UtilisateurId).FirstOrDefaultAsync();

                    besoin.PeutPostuler = false;

                    besoin.PeutPostuler = besoin.PeutPostuler || !Candidatures.Any(c => c.UtilisateurId == Utilisateur.Id);

                    if (besoin.PeutPostuler)
                    {
                        foreach (int CetId in specId)
                        {
                            besoin.PeutPostuler = SpecialitesUtilisateurIds.Contains(CetId) ? true : false;
                        }
                    }

                    foreach (var specialite in besoin.Specialite)
                    {
                        specialite.Match = SpecialitesUtilisateurIds.Contains(specialite.Id) ? true : false;
                    }
                }
                projet.Besoins = besoins;
                ViewBag.Url = Request.Headers["Referer"].ToString();
                return View(projet);
            }
        #endregion

        [HttpPost]
        public async Task<JsonResult> Refuse(int id, string motif)
        {
            var Proposition = await _db.Proposition.Where(p => p.Id == id).FirstOrDefaultAsync();
            Proposition.Statut = EValidation.Refused.ToString();
            Proposition.Motif = motif;
            var Projet = await _db.Projet.Where(p => p.Id == Proposition.ProjetId).FirstOrDefaultAsync();
            Projet.Complet = false;
            var Besoin = await _db.Besoins.Where(b => b.Id == Proposition.BesoinsId).FirstOrDefaultAsync();
            var AutresCandidatures = await _db.Candidature.Where(c => c.BesoinsId == Besoin.Id && c.Statut == EValidation.Complete.ToString()).ToListAsync();
            Besoin.UtilisateurId = null;
            if (AutresCandidatures.Count > 0)
            {
                foreach (Candidature cetteCandidature in AutresCandidatures)
                {
                    cetteCandidature.Statut = EValidation.Pending.ToString();
                    cetteCandidature.Motif = null;
                }
                await _db.SaveChangesAsync();
            }
            _db.Update(Projet);
            _db.Update(Besoin);
            _db.Update(Proposition);
            await _db.SaveChangesAsync();
            var User = await _db.Users.Where(u => u.Id == Projet.UtilisateurId).FirstOrDefaultAsync();
            var user = await _db.Users.Where(u => u.Id == Proposition.UtilisateurId).FirstOrDefaultAsync();

            var UrlPage = Url.Action("RedirectPage",
                                     "SharedAction",
                                         new
                                         {
                                             userId = User.Id,
                                             page = "RefuseP"
                                         },
                                         protocol: Request.Scheme);

            var link = new RefusePropositionEmailViewModel(Projet.Name, user.Prenom + " " + user.Nom, Proposition.Motif, UrlPage);
            string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/RefuseProposition/RefusePropositionEmail.cshtml", link);
            await _emailSender.SendNewEmail(User.Email, "Votre offre a été refusée", body);

            await _hub.Clients.All.SendAsync("displayNotification");
            return Json(new { success = true, result = "/CandidatureRetenue" });
        }

        [HttpPost]
        public async Task<JsonResult> Leave(int id, string motif)
        {
            var Candidature = await _db.Candidature.Where(c => c.Id == id).FirstOrDefaultAsync();
            Candidature.Statut = EValidation.Refused.ToString();
            Candidature.Motif = motif;
            var Projet = await _db.Projet.Where(p => p.Id == Candidature.ProjetId).FirstOrDefaultAsync();
            Projet.Complet = false;
            var Besoin = await _db.Besoins.Where(b => b.Id == Candidature.BesoinsId).FirstOrDefaultAsync();
            var AutresCandidatures = await _db.Candidature.Where(c => c.BesoinsId == Besoin.Id && c.Statut == EValidation.Complete.ToString()).ToListAsync();
            Besoin.UtilisateurId = null;
            if (AutresCandidatures.Count > 0)
            {
                foreach (Candidature cetteCandidature in AutresCandidatures)
                {
                    cetteCandidature.Statut = EValidation.Pending.ToString();
                    cetteCandidature.Motif = null;
                }
                await _db.SaveChangesAsync();
            }
            _db.Update(Projet);
            _db.Update(Besoin);
            _db.Update(Candidature);
            await _db.SaveChangesAsync();

            var User = await _db.Users.Where(u => u.Id == Projet.UtilisateurId).FirstOrDefaultAsync();
            var user = await _db.Users.Where(u => u.Id == Candidature.UtilisateurId).FirstOrDefaultAsync();

            var UrlPage = Url.Action("RedirectPage",
                                     "SharedAction",
                                         new
                                         {
                                             userId = User.Id,
                                             page = "Leave"
                                         },
                                         protocol: Request.Scheme);

            var link = new LeaveProjectEmailViewModel(Projet.Name, user.Prenom + " " + user.Nom, Candidature.Motif, UrlPage);
            string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/LeaveProject/LeaveProjectEmail.cshtml", link);
            await _emailSender.SendNewEmail(User.Email, "Un co-surfeur a quitté votre projet", body);

            await _hub.Clients.All.SendAsync("displayNotification");
            return Json(new { success = true, result = "/CandidatureRetenue" });
        }
    }
}