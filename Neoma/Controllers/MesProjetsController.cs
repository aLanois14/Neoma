using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Extensions;
using Neoma.Hubs;
using Neoma.Models;
using Neoma.Models.MembreProjetViewModel;
using Neoma.Models.MesProjetsViewModel;
using Neoma.RazorClassLib.Services;
using Neoma.RazorClassLib.Views.Emails.NewSpecialite;
using Neoma.Services;
using Neoma.Utility;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.AdminEndUser + "," + SD.CommonEndUser + "," + SD.SuperAdminEndUser + "," + SD.SuperAdminEndUser)]
    public class MesProjetsController : BaseController
    {
        private IHubContext<MessagerieHub> _hub;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;

        private readonly IEmailSender _emailSender;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

        public MesProjetsController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRazorViewToStringRenderer razorViewToStringRenderer, IHubContext<MessagerieHub> hub) : base(db)
        {
            _userManager = userManager;
            _db = db;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _hub = hub;
        }

        #region Affichage vue
        public async Task<IActionResult> Index()
        {
            var ListItemProjet = new List<ItemProjetViewModel>();          
            var Projet = await _db.Projet.Where(p => p.UtilisateurId == User.getUserId()).ToListAsync();
            foreach (var item in Projet)
            {
                var ItemProjet = new ItemProjetViewModel();
                item.PresentationProjet = item.PresentationProjet != null && item.PresentationProjet.Length > (200 - "... (voir la suite)".Length) ? item.PresentationProjet.Substring(0, (200 - "... (voir la suite)".Length)) + "... (voir la suite)" : item.PresentationProjet;
                item.TypeProjet = await _db.TypeProjet.Where(t => t.Id == item.TypeProjetId).FirstOrDefaultAsync();
                var organismeId = await _db.Users.Where(u => u.Id == item.UtilisateurId).Select(x => x.OrganismeId).FirstOrDefaultAsync();
                ItemProjet.Projet = item;
                ItemProjet.Organisme = await _db.Organisme.Where(o => o.Id == organismeId).FirstOrDefaultAsync();
                ItemProjet.Besoins = new List<ItemBesoinViewModel>();
                var Besoins = await _db.Besoins.Where(b => b.ProjetId == item.Id).ToListAsync();

                foreach (var besoin in Besoins)
                {
                    var ItemBesoin = new ItemBesoinViewModel();
                    besoin.Utilisateur = besoin.UtilisateurId == null ? null : await _db.Users.Where(u => u.Id == besoin.UtilisateurId).FirstOrDefaultAsync();
                    ItemBesoin.Besoin = besoin;
                    ItemBesoin.Candidatures = await _db.Candidature.Where(c => c.BesoinsId == besoin.Id && c.Statut.ToString() == "Pending").ToListAsync();

                    var ItemRole = new ItemRoleViewModel();
                    ItemRole.Role = await _db.Role.Where(r => r.Id == besoin.RoleId).FirstOrDefaultAsync();
                    var BesoinsSpec = await _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == besoin.Id).ToListAsync();
                    ItemRole.Specialites = new List<Specialite>();
                    foreach (var besoinspec in BesoinsSpec)
                    {
                        ItemRole.Specialites.Add(await _db.Specialite.Where(s => s.Id == besoinspec.SpecialiteId).FirstOrDefaultAsync());
                    }
                    foreach (var candidature in ItemBesoin.Candidatures)
                    {
                        candidature.Utilisateur = await _db.ApplicationUser.Where(u => u.Id == candidature.UtilisateurId).FirstOrDefaultAsync();
                    }
                    ItemBesoin.ItemRole = ItemRole;
                    ItemProjet.Besoins.Add(ItemBesoin);
                }
                ListItemProjet.Add(ItemProjet);
            }
            return View(ListItemProjet);
        }

        public async Task<IActionResult> EditProject(int projetId)
        {
            Projet Projet = new Projet();

            if (projetId == 0)
            {
                Projet.UtilisateurId = User.getUserId();
            }
            else
            {
                Projet = _db.Projet.Where(p => p.Id == projetId).FirstOrDefault();
            }

            MembreProjetViewModel MembreProjet = new MembreProjetViewModel()
            {
                Projet = Projet,
                TypeProjet = await _db.TypeProjet.ToListAsync()
            };
            MembreProjet.Membre = await _db.Membre.Where(m => m.Projet == MembreProjet.Projet).ToListAsync();
            foreach (var membre in MembreProjet.Membre)
            {
                var SpecUser = await _db.MembreSpecialite.Where(ms => ms.MembreId == membre.Id).Select(x => x.SpecialiteId).ToListAsync();
                membre.Specialite = await _db.Specialite.Where(s => SpecUser.Contains(s.Id)).ToListAsync();
            }

            MembreProjet.Besoins = await _db.Besoins.Where(b => b.Projet == MembreProjet.Projet).ToListAsync();
            foreach (var besoin in MembreProjet.Besoins)
            {
                var SpecUser = await _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == besoin.Id).Select(x => x.SpecialiteId).ToListAsync();
                besoin.Specialite = await _db.Specialite.Where(s => SpecUser.Contains(s.Id)).ToListAsync();
            }
            return View(MembreProjet);
        }
        #endregion

        #region Action
        [HttpPost]
        public async Task<JsonResult> SaveEdit([FromBody] MembreProjetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var specialiteCreate = new List<string>();
                if (model.Projet != null)
                {
                    _db.Update(model.Projet);

                    var Membres = await _db.Membre.AsNoTracking().Where(m => m.ProjetId == model.Projet.Id).ToListAsync();
                    var Besoins = await _db.Besoins.AsNoTracking().Where(b => b.ProjetId == model.Projet.Id).ToListAsync();

                    foreach (var membre in Membres)
                    {
                        if (!model.Membre.Any(m => m.Id == membre.Id))
                        {
                            _db.Remove(membre);
                        }
                    }

                    foreach (var besoin in Besoins)
                    {
                        if (!model.Besoins.Any(m => m.Id == besoin.Id))
                        {
                            _db.Remove(besoin);
                        }
                    }

                    foreach (var membre in model.Membre)
                    {
                        membre.ProjetId = model.Projet.Id;
                        _db.Update(membre);

                        var Specialites = await _db.MembreSpecialite.Where(ms => ms.MembreId == membre.Id).ToListAsync();
                        foreach (var specialite in Specialites)
                        {
                            specialite.Specialite = await _db.Specialite.Where(s => s.Id == specialite.SpecialiteId).FirstOrDefaultAsync();

                            if (!membre.Specialite.Any(s => s.Id == specialite.Specialite.Id))
                            {
                                _db.MembreSpecialite.Remove(specialite);
                            }
                        }
                        foreach (var specialite in membre.Specialite)
                        {
                            if (!_db.Specialite.Any(s => s.Name == specialite.Name))
                            {
                                specialite.RoleId = membre.RoleId;
                                specialite.Valide = false;
                                specialite.UtilisateurId = model.Projet.UtilisateurId;
                                _db.Specialite.Update(specialite);
                                specialiteCreate.Add(specialite.Name);
                            }
                            else
                            {
                                if (specialite.Id == 0)
                                {
                                    specialite.Id = _db.Specialite.Where(s => s.Name == specialite.Name).Select(i => i.Id).FirstOrDefault();
                                }
                            }
                            MembreSpecialite specialiteMembre = new MembreSpecialite
                            {
                                MembreId = membre.Id,
                                SpecialiteId = specialite.Id
                            };
                            if (!Specialites.Exists(x => x.MembreId == specialiteMembre.MembreId && x.SpecialiteId == specialiteMembre.SpecialiteId))
                            {
                                _db.MembreSpecialite.Update(specialiteMembre);
                            }

                        }
                    }

                    foreach (var besoin in model.Besoins)
                    {
                        besoin.Projet = model.Projet;
                        if (besoin.UtilisateurId == "")
                        {
                            besoin.UtilisateurId = null;
                        }
                        _db.Besoins.Update(besoin);

                        var Specialites = await _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == besoin.Id).ToListAsync();
                        foreach (var specialite in Specialites)
                        {
                            specialite.Specialite = await _db.Specialite.Where(s => s.Id == specialite.SpecialiteId).FirstOrDefaultAsync();

                            if (!besoin.Specialite.Any(s => s.Id == specialite.Specialite.Id))
                            {
                                _db.BesoinsSpecialite.Remove(specialite);
                            }
                        }

                        foreach (var specialite in besoin.Specialite)
                        {
                            if (!_db.Specialite.Any(s => s.Name == specialite.Name))
                            {
                                specialite.RoleId = besoin.RoleId;
                                specialite.Valide = false;
                                specialite.UtilisateurId = model.Projet.UtilisateurId;
                                _db.Specialite.Update(specialite);
                                specialiteCreate.Add(specialite.Name);
                            }
                            else
                            {
                                if (specialite.Id == 0)
                                {
                                    specialite.Id = _db.Specialite.Where(s => s.Name == specialite.Name).Select(i => i.Id).FirstOrDefault();
                                }
                            }
                            BesoinsSpecialite specialiteBesoins = new BesoinsSpecialite
                            {
                                BesoinsId = besoin.Id,
                                SpecialiteId = specialite.Id
                            };
                            if (!Specialites.Exists(x => x.BesoinsId == specialiteBesoins.BesoinsId && x.SpecialiteId == specialiteBesoins.SpecialiteId))
                            {
                                _db.BesoinsSpecialite.Update(specialiteBesoins);
                            }
                        }
                    }
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                    }

                }

                if (specialiteCreate.Count > 0)
                {
                    var UsersAdmin = _userManager.GetUsersInRoleAsync(SD.SuperAdminEndUser).Result;
                    foreach (var user in UsersAdmin)
                    {
                        var UrlPage = Url.Action("RedirectPage", "SharedAction", new { userId = user.Id, page = "Specialite" }, protocol: Request.Scheme);
                        var link = new NewSpecialiteEmailViewModel(UrlPage, specialiteCreate);
                        string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/NewSpecialite/NewSpecialiteEmail.cshtml", link);
                        await _emailSender.SendNewEmail(user.Email, "Validation nouvelle spécialité", body);
                    }
                }
                return Json(new { success = true, result = "/MesProjets" });
            }
            else
            {
                return Json(new { success = false, issue = model, errors = ModelState.Values.Where(i => i.Errors.Count > 0) });
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id, string motif)
        {
            var Candidatures = await _db.Candidature.Where(c => c.BesoinsId == id && c.Statut == EValidation.Accepted.ToString()).FirstOrDefaultAsync();
            var AutresCandidatures = await _db.Candidature.Where(c => c.BesoinsId == id && c.Statut == EValidation.Complete.ToString()).ToListAsync();
            var Propositions = await _db.Proposition.Where(p => p.BesoinsId == id && p.Statut == EValidation.Accepted.ToString()).FirstOrDefaultAsync();

            if (Candidatures != null)
            {
                Candidatures.Statut = EValidation.Refused.ToString();
                Candidatures.Motif = motif;
                _db.Update(Candidatures);

                if (AutresCandidatures.Count > 0)
                {
                    foreach (Candidature cetteCandidature in AutresCandidatures)
                    {
                        cetteCandidature.Statut = EValidation.Pending.ToString();
                        cetteCandidature.Motif = null;
                    }
                    await _db.SaveChangesAsync();
                }
            }
            else
            {
                Propositions.Statut = EValidation.Refused.ToString();
                Propositions.Motif = motif;
                _db.Update(Propositions);
            }

            var Besoin = await _db.Besoins.Where(b => b.Id == id).FirstOrDefaultAsync();
            var User = await _db.Users.Where(u => u.Id == Besoin.UtilisateurId).FirstOrDefaultAsync();
            await _emailSender.SendNewEmail(User.Email, "Notification de suppression", motif);
            Besoin.Projet = await _db.Projet.Where(p => p.Id == Besoin.ProjetId).FirstOrDefaultAsync();
            Besoin.Projet.Complet = false;
            var Porteur = await _db.Users.Where(u => u.Id == Besoin.Projet.UtilisateurId).FirstOrDefaultAsync();
            Besoin.UtilisateurId = null;

            _db.Update(Besoin);
            await _db.SaveChangesAsync();

            await _hub.Clients.All.SendAsync("displayNotification");

            return Json(new { success = true, result = "/MesProjets" });
        }
        #endregion
    }
}
