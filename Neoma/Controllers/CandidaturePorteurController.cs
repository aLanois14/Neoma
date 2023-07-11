using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Extensions;
using Neoma.Models;
using Neoma.Models.CandidatsViewModel;
using Neoma.Models.MatchingViewModel;
using Neoma.Models.MembreProjetViewModel;
using Neoma.Utility;
using Neoma.Services;
using Microsoft.AspNetCore.SignalR;
using Neoma.Hubs;
using Neoma.RazorClassLib.Services;
using Neoma.RazorClassLib.Views.Emails.RefuseCandidature;
using Neoma.RazorClassLib.Views.Emails.AcceptCandidature;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.CommonEndUser + "," + SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    public class CandidaturePorteurController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private IHubContext<MessagerieHub> _hub;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

        public CandidaturePorteurController(ApplicationDbContext db, IEmailSender emailSender, IHubContext<MessagerieHub> hub, IRazorViewToStringRenderer razorViewToStringRenderer) : base(db)
        {
            _db = db;
            _emailSender = emailSender;
            _hub = hub;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        #region Affichage vue
            public async Task<IActionResult> Index()
            {
                MatchCandidatViewModel CandidatViewModel = new MatchCandidatViewModel();
                CandidatViewModel.Projet = new List<Projet>();
                CandidatViewModel.Role = new List<Role>();
                CandidatViewModel.Specialite = new List<Specialite>();
                CandidatViewModel.Role.Add(new Role
                {
                    Name = "Tous les rôles"
                });
                CandidatViewModel.Projet.Add(new Projet
                {
                    Name = "Tous les projets"
                });
                var Candidatures = await _db.Candidature.Where(c => c.Statut == EValidation.Pending.ToString()).Select(x => x.ProjetId).ToListAsync();
                var Projets = await _db.Projet.Where(p => p.UtilisateurId == User.getUserId() && Candidatures.Contains(p.Id)).ToListAsync();
                foreach (var Projet in Projets)
                {
                    CandidatViewModel.Projet.Add(Projet);
                }

                return View(CandidatViewModel);
            }

            public async Task<IActionResult> DetailCandidature(string user, int projet)
            {
                string userId = User.getUserId();
                var Candidatures = await _db.Candidature.Where(c => c.UtilisateurId == user && c.ProjetId == projet && c.Statut == EValidation.Pending.ToString()).ToListAsync();
                var Utilisateur = _db.Users.Where(u => u.Id == user).FirstOrDefault();
                Utilisateur.Organisme = _db.Organisme.Where(o => o.Id == Utilisateur.OrganismeId).FirstOrDefault();
                var specialiteUser = _db.SpecialiteUtilisateur.Where(s => s.UtilisateurId == Utilisateur.Id).ToList();
                var roleUser = _db.RoleUtilisateur.Where(r => r.UtilisateurId == Utilisateur.Id).ToList();
                List<Specialite> spec = new List<Specialite>();
                List<Role> Role = new List<Role>();
                foreach (var specialite in specialiteUser)
                {
                    Specialite special = _db.Specialite.Where(s => s.Id == specialite.SpecialiteId && (s.Valide || s.UtilisateurId == userId)).FirstOrDefault();
                    if (special != null)
                        spec.Add(special);
                }

                foreach (var role in roleUser)
                {
                    Role.Add(_db.Role.Where(r => r.Id == role.RoleId).FirstOrDefault());
                }

                var item = new CandidaturesItemViewModel
                {
                    User = Utilisateur,
                    Candidatures = new List<Candidature>(),
                    Role = Role.OrderBy(r => r.Name).ToList(),
                    Specialite = spec.OrderBy(s => s.Name).ToList()
                };
                foreach (var candidature in Candidatures)
                {
                    candidature.Besoins = await _db.Besoins.Where(b => b.Id == candidature.BesoinsId).FirstOrDefaultAsync();
                    candidature.Besoins.Role = await _db.Role.Where(r => r.Id == candidature.Besoins.RoleId).FirstOrDefaultAsync();
                    var BesoinsSpecs = await _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == candidature.BesoinsId).ToListAsync();
                    candidature.Besoins.Specialite = new List<Specialite>();
                    foreach (var besoinspec in BesoinsSpecs)
                    {
                        candidature.Besoins.Specialite.Add(await _db.Specialite.Where(s => s.Id == besoinspec.SpecialiteId).FirstOrDefaultAsync());
                    }
                    item.Candidatures.Add(candidature);
                }

                ViewBag.Url = Request.Headers["Referer"].ToString();
                return View(item);
            }

            public IActionResult TableCandidature([FromBody]FilterViewModel Filter)
            {
                return ViewComponent("TableCandidatureP", new { Filter = Filter });
            }
        #endregion

        #region Mise à jour filtre
            public IActionResult UpdateRole([FromBody]RoleModel Role)
            {
                List<Besoins> Besoins = new List<Besoins>();
                Besoins.Add(new Besoins
                {
                    Role = new Role()
                });

                var Candidatures = _db.Candidature.Where(c => c.ProjetId == Role.Projet && c.Statut == EValidation.Pending.ToString()).Select(x => x.BesoinsId).ToList();
                var listBesoin = _db.Besoins.Where(b => b.ProjetId == Role.Projet && Candidatures.Contains(b.Id)).ToList();

                foreach (var besoin in listBesoin)
                {
                    besoin.Role = _db.Role.Where(r => r.Id == besoin.RoleId).FirstOrDefault();
                    Besoins.Add(besoin);
                }

                return new JsonResult(Besoins);
            }

            public IActionResult UpdateSpecialite([FromBody]RoleModel Role)
            {
                List<Specialite> Specialite = new List<Specialite>();
                if (Role.Role != 0)
                {
                    var Besoin = _db.Besoins.Where(b => b.Id == Role.Role).FirstOrDefault();
                    var BesoinSpecialite = _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == Besoin.Id).ToList();

                    foreach (var besoinspec in BesoinSpecialite)
                    {
                        Specialite.Add(_db.Specialite.Where(s => s.Id == besoinspec.SpecialiteId).FirstOrDefault());
                    }
                }
                return new JsonResult(Specialite);
            }
        #endregion

        [HttpPost]
        public async Task<JsonResult> Validate(int id)
        {
            var Candidature = await _db.Candidature.Where(c => c.Id == id).FirstOrDefaultAsync();
            Candidature.Statut = EValidation.Accepted.ToString();
            var Besoin = await _db.Besoins.Where(b => b.Id == Candidature.BesoinsId).FirstOrDefaultAsync();
            Besoin.UtilisateurId = Candidature.UtilisateurId;
            var Projet = await _db.Projet.Where(p => p.Id == Besoin.ProjetId).FirstOrDefaultAsync();
            var userIdBesoin = await _db.Besoins.Where(b => b.ProjetId == Projet.Id).Select(x => x.UtilisateurId).ToListAsync();
            bool complete = true;

            var Candidatures = await _db.Candidature.Where(c => c.BesoinsId == Besoin.Id && c.Id != id).ToListAsync();
            var Propositions = await _db.Proposition.Where(p => p.BesoinsId == Besoin.Id).ToListAsync();
            var Selections = await _db.Selection.Where(s => s.BesoinsId == Besoin.Id).ToListAsync();
            foreach(var candidature in Candidatures)
            {
                candidature.Statut = EValidation.Complete.ToString();
                candidature.Motif = "Ce poste n'est plus disponible.";
                _db.Candidature.Update(candidature);
            }
            foreach(var proposition in Propositions)
            {
                _db.Proposition.Remove(proposition);
            }
            foreach (var selection in Selections)
            {
                _db.Selection.Remove(selection);
            }

            foreach (var userId in userIdBesoin)
            {
                if (userId == null)
                {
                    complete = false;
                }
            }

            if (complete)
            {
                Projet.Complet = true;
            }
            _db.Update(Candidature);
            _db.Update(Besoin);
            _db.Update(Projet);
            await _db.SaveChangesAsync();

            var UrlPage = Url.Action("RedirectPage",
                                     "SharedAction",
                                     new
                                     {
                                         userId = Candidature.UtilisateurId,
                                         page = "Validate",
                                         parameter1 = Candidature.ProjetId
                                     },
                                     protocol: Request.Scheme);
            var link = new AcceptCandidatureEmailViewModel(await _db.Projet.Where(p => p.Id == Candidature.ProjetId).Select(x => x.Name).FirstOrDefaultAsync(), UrlPage);
            string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/AcceptCandidature/AcceptCandidatureEmail.cshtml", link);
            await _emailSender.SendNewEmail(await _db.Users.Where(u => u.Id == Candidature.UtilisateurId).Select(x => x.Email).FirstOrDefaultAsync(), "Candidature acceptée", body);
            await _hub.Clients.All.SendAsync("displayNotification");
            return Json(new { success = true, result = "/CandidaturePorteur" });
        }

        [HttpPost]
        public async Task<JsonResult> Refuse(int id, string motif)
        {
            var Candidature = await _db.Candidature.Where(c => c.Id == id).FirstOrDefaultAsync();
            Candidature.Statut = EValidation.Refused.ToString();
            Candidature.Motif = motif;
            _db.Update(Candidature);
            await _db.SaveChangesAsync();
            var UrlPage = Url.Action("RedirectPage",
                                     "SharedAction",
                                     new
                                     {
                                         userId = Candidature.UtilisateurId,
                                         page = "RefuseC"
                                     },
                                     protocol: Request.Scheme);
            var link = new RefuseCandidatureEmailViewModel(await _db.Projet.Where(p => p.Id == Candidature.ProjetId).Select(x => x.Name).FirstOrDefaultAsync(), Candidature.Motif, UrlPage);
            string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/RefuseCandidature/RefuseCandidatureEmail.cshtml", link);
            await _emailSender.SendNewEmail(await _db.Users.Where(u => u.Id == Candidature.UtilisateurId).Select(x => x.Email).FirstOrDefaultAsync(), "Candidature refusée", body);

            await _hub.Clients.All.SendAsync("displayNotification");
            return Json(new { success = true, result = "/CandidaturePorteur" });
        }
    }
}