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
using Neoma.Models.CandidatsViewModel;
using Neoma.Models.CandidatViewModel;
using Neoma.Models.MembreProjetViewModel;
using Neoma.Utility;
using Neoma.Services;
using Neoma.Models.SelectionViewModel;
using Neoma.RazorClassLib.Services;
using Neoma.RazorClassLib.Views.Emails.ValidateSelection;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.CommonEndUser + "," + SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    public class SelectionsController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private IHubContext<MessagerieHub> _hub;
        private readonly IEmailSender _emailSender;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

        public SelectionsController(ApplicationDbContext db, IHubContext<MessagerieHub> hub, IEmailSender emailSender, IRazorViewToStringRenderer razorViewToStringRenderer) : base(db)
        {
            _db = db;
            _hub = hub;
            _emailSender = emailSender;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        #region Affichage vue
            public async Task<IActionResult> Index()
            {
                List<Projet> Projet = new List<Projet>();
                Projet.Add(new Projet
                {
                    Name = "Tous les projets"
                });
                var Selections = await _db.Selection.Select(x => x.ProjetId).ToListAsync();
                var Projets = await _db.Projet.Where(p => p.UtilisateurId == User.getUserId() && Selections.Contains(p.Id)).ToListAsync();
                foreach (var projet in Projets)
                {
                    Projet.Add(projet);
                }

                return View(Projet);
            }

            public async Task<IActionResult> DetailSelection(string Id)
            {
                string userId = User.getUserId();
                var ItemSelection = new ItemSelectionViewModel();
                ItemSelection.User = await _db.Users.Where(u => u.Id == Id).FirstOrDefaultAsync();
                ItemSelection.User.Organisme = await _db.Organisme.Where(o => o.Id == ItemSelection.User.OrganismeId).FirstOrDefaultAsync();
                ItemSelection.ItemRole = new List<ItemRoleViewModel>();
                var specialiteUser = await _db.SpecialiteUtilisateur.Where(s => s.UtilisateurId == Id).ToListAsync();
                var roleUser = await _db.RoleUtilisateur.Where(r => r.UtilisateurId == Id).ToListAsync();
                var Specialites = await _db.Specialite.Where(s => specialiteUser.Exists(x => x.SpecialiteId == s.Id) && (s.Valide || s.UtilisateurId == userId)).OrderBy(x => x.Name).ToListAsync();
                foreach (var role in roleUser)
                {                  
                    var ItemRole = new ItemRoleViewModel();
                    ItemRole.Role = await _db.Role.Where(r => r.Id == role.RoleId).FirstOrDefaultAsync();
                    ItemRole.Specialites = new List<Specialite>();
                    foreach (var specialite in Specialites)
                    {
                        if (specialite != null && specialite.RoleId == role.RoleId)
                        {
                            ItemRole.Specialites.Add(specialite);
                        }
                    }
                    ItemSelection.ItemRole.Add(ItemRole);
                }

                ViewBag.Url = Request.Headers["Referer"].ToString();
                return View(ItemSelection);
            }

            public IActionResult TableSelection([FromBody]FilterViewModel Filter)
            {
                return ViewComponent("TableSelectionPorteur", new { Filter = Filter });
            }
        #endregion

        #region Action
            public async Task<IActionResult> Validate(int id)
            {
                var Selection = await _db.Selection.Where(s => s.Id == id).FirstOrDefaultAsync();
                var Besoins = await _db.Besoins.Where(b => b.Id == Selection.BesoinsId).FirstOrDefaultAsync();
                var Candidatures = await _db.Candidature.Where(c => c.BesoinsId == Besoins.Id && c.Statut == EValidation.Pending.ToString()).ToListAsync();

                var UserEmail = await _db.Users.Where(u => u.Id == Selection.UtilisateurId).Select(x => x.Email).FirstOrDefaultAsync();
                foreach(var candidature in Candidatures)
                {
                    candidature.Statut = EValidation.Complete.ToString();
                    candidature.Motif = "Ce poste n'est plus disponible";
                    _db.Candidature.Update(candidature);
                }

                Besoins.UtilisateurId = Selection.UtilisateurId;
                var Proposition = new Proposition{
                    ProjetId = Besoins.ProjetId,
                    UtilisateurId = Selection.UtilisateurId,
                    BesoinsId = Besoins.Id,
                    Statut = EValidation.Accepted.ToString()
                };
            
                _db.Besoins.Update(Besoins);
                _db.Selection.Remove(Selection);
                _db.Proposition.Add(Proposition);
                var projet = await _db.Projet.Where(p => p.Id == Selection.ProjetId).FirstOrDefaultAsync();
                var user = await _db.Users.Where(u => u.Id == projet.UtilisateurId).FirstOrDefaultAsync();
                var UrlPage = Url.Action("RedirectPage",
                                         "SharedAction",
                                         new {
                                             userId = Besoins.UtilisateurId,
                                             page = "Validate",
                                             parameter1 = Besoins.ProjetId
                                         }, 
                                         protocol: Request.Scheme);

                var link = new ValidateSelectionEmailViewModel(projet.Name, user.Prenom + " " + user.Nom, UrlPage);
                string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/ValidateSelection/ValidateSelectionEmail.cshtml", link);
                await _emailSender.SendNewEmail(UserEmail, "Vous avez été ajouté dans un projet", body);
                await _db.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("displayNotification");
                return RedirectToAction(nameof(Index));
            }

            public async Task<IActionResult> Delete(int id)
            {
                var Selection = await _db.Selection.Where(s => s.Id == id).FirstOrDefaultAsync();
                _db.Selection.Remove(Selection);
                await _db.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("displayNotification");
                return RedirectToAction(nameof(Index));
            }
        #endregion

        #region Mise à jour filtre
            public IActionResult UpdateRole(int projet)
            {
                List<Besoins> Besoins = new List<Besoins>();
                Besoins.Add(new Besoins
                {
                    Role = new Role()
                });

                var Selections = _db.Selection.Where(c => c.ProjetId == projet).Select(x => x.BesoinsId).ToList();
                var listBesoin = _db.Besoins.Where(b => b.ProjetId == projet && Selections.Contains(b.Id)).ToList();

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
    }
}