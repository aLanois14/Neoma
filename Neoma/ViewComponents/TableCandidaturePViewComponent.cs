using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Models;
using Neoma.Models.CandidatsViewModel;
using Neoma.Models.CandidaturePorteurViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Neoma.ViewComponents
{
    public class TableCandidaturePViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public TableCandidaturePViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(FilterViewModel Filter = null)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var ItemCandidatureViewModel = new List<ItemCandidaturePorteurViewModel>();
            var Projets = Filter == null || (Filter != null && (Filter.Projet == null || Filter.Projet.Id == 0)) ?
                await _db.Projet.Where(p => p.UtilisateurId == claim && p.Complet == false && p.Termine == false && p.Actif == true).ToListAsync() :
                await _db.Projet.Where(p => p.UtilisateurId == claim && p.Id == Filter.Projet.Id).ToListAsync();
            if (Filter == null)
            {
                foreach(var projet in Projets)
                {
                    var Candidatures = await _db.Candidature.Where(c => c.ProjetId == projet.Id && c.Statut == EValidation.Pending.ToString()).ToListAsync();
                    if (Candidatures != null)
                    {
                        var Users = await _db.Users.Where(u => Candidatures.Exists(c => c.UtilisateurId == u.Id)).ToListAsync();
                       foreach(var user in Users)
                        {
                            ItemCandidatureViewModel.Add(await List(user, projet, claim));
                        }
                    }
                }
                
            }
            else
            {
                if(Filter.Besoins.Id != 0 && (Filter.Projet == null))
                {
                    var Projet = await _db.Projet.Where(p => p.Id == Filter.Besoins.ProjetId).FirstOrDefaultAsync();
                    var Candidatures = await _db.Candidature.Where(c => c.ProjetId == Projet.Id && c.Statut == EValidation.Pending.ToString()).ToListAsync();
                    if (Candidatures != null)
                    {
                        var userselect = new List<ApplicationUser>();
                        var Users = await _db.Users.Where(u => Candidatures.Exists(c => c.UtilisateurId == u.Id)).ToListAsync();                      

                        if (Filter.Specialite != null)
                        {
                            if (Filter.Specialite.Count != 0)
                            {                               
                                foreach (var user in Users)
                                {
                                    bool delete = false;
                                    foreach(var test in Filter.Specialite)
                                    {
                                        if(!_db.SpecialiteUtilisateur.Any(s => s.SpecialiteId == test.Id && s.UtilisateurId == user.Id))
                                        {
                                            delete = true;
                                        }
                                    }
                                    if (!delete)
                                    {
                                        userselect.Add(user);
                                    }
                                }
                                foreach (var user in userselect)
                                {
                                    ItemCandidatureViewModel.Add(await List(user, Projet, claim));
                                }
                            }
                            else
                            {
                                foreach (var user in Users)
                                {
                                    ItemCandidatureViewModel.Add(await List(user, Projet, claim));
                                }
                            }
                        }
                        else
                        {
                            foreach (var user in Users)
                            {
                                ItemCandidatureViewModel.Add(await List(user, Projet, claim));
                            }
                        }
                                            
                    }
                }
                else
                {
                    foreach (var projet in Projets)
                    {
                        var Candidatures = await _db.Candidature.Where(c => c.ProjetId == projet.Id && c.Statut == EValidation.Pending.ToString()).ToListAsync();
                        if (Candidatures != null)
                        {
                            var Users = await _db.Users.Where(u => Candidatures.Exists(c => c.UtilisateurId == u.Id)).ToListAsync();
                            foreach (var user in Users)
                            {
                                ItemCandidatureViewModel.Add(await List(user, projet, claim));
                            }
                                
                        }
                    }
                }
            }
            ViewBag.TestFilter = Filter != null ? Filter.TexteRecherche : null;
            return View(ItemCandidatureViewModel);
        }


        private async Task<ItemCandidaturePorteurViewModel> List(ApplicationUser User, Projet Projet, string claim)
        {
            var ItemCandidaturePorteur = new ItemCandidaturePorteurViewModel();
            var ItemUser = new ItemUserViewModel();
            ItemCandidaturePorteur.Projet = Projet;
            ItemUser.User = User;
            ItemUser.Role = new List<ItemRoleViewModel>();
            ItemCandidaturePorteur.ItemCandidature = new List<ItemCandidatureViewModel>();
            var Candidatures = await _db.Candidature.Where(c => c.UtilisateurId == User.Id && c.ProjetId == Projet.Id && c.Statut == EValidation.Pending.ToString()).ToListAsync();
            foreach (var candidature in Candidatures)
            {
                var ItemCandidature = new ItemCandidatureViewModel();
                var Besoins = await _db.Besoins.Where(b => b.Id == candidature.BesoinsId).FirstOrDefaultAsync();
                var ItemRole = new ItemRoleViewModel();
                ItemRole.Role = await _db.Role.Where(r => r.Id == Besoins.RoleId).FirstOrDefaultAsync();
                var BesoinsSpec = await _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == Besoins.Id).ToListAsync();
                ItemRole.Specialites = await _db.Specialite.Where(s => BesoinsSpec.Exists(x => x.SpecialiteId == s.Id)).ToListAsync();
                ItemCandidature.Candidature = candidature;
                ItemCandidature.ItemRole = ItemRole;
                ItemCandidaturePorteur.ItemCandidature.Add(ItemCandidature);
            }

            var SpecialiteUser = _db.SpecialiteUtilisateur.Where(su => su.UtilisateurId == User.Id).ToList();
            var RoleUser = _db.RoleUtilisateur.Where(ru => ru.UtilisateurId == User.Id).ToList();
            //List<Specialite> spec = new List<Specialite>();
            //List<Role> RolesUser = new List<Role>();
            foreach(var roleItem in RoleUser)
            {
                var ItemRole = new ItemRoleViewModel();
                ItemRole.Specialites = new List<Specialite>();
                ItemRole.Role = await _db.Role.Where(r => r.Id == roleItem.RoleId).FirstOrDefaultAsync();
                foreach(var specialite in SpecialiteUser)
                {
                    var special = await _db.Specialite.Where(s => s.Id == specialite.SpecialiteId && (s.Valide || s.UtilisateurId == claim)).FirstOrDefaultAsync();
                    if(special != null && special.RoleId == ItemRole.Role.Id)
                    {
                        ItemRole.Specialites.Add(special);
                    }
                }
                ItemRole.Specialites = ItemRole.Specialites.OrderBy(r => r.Name).ToList();
                ItemUser.Role.Add(ItemRole);
            }

            ItemUser.Role = ItemUser.Role.OrderBy(x => x.Role.Name).ToList();

            ItemCandidaturePorteur.ItemUser = ItemUser;
            //foreach (var specialite in SpecialiteUser)
            //{
                
                
            //    Specialite special = _db.Specialite.Where(s => s.Id == specialite.SpecialiteId && (s.Valide || s.UtilisateurId == claim)).FirstOrDefault();
            //    if (special != null)
            //        spec.Add(special);
            //}
            //foreach (var roleItem in RoleUser)
            //{
            //    RolesUser.Add(_db.Role.Where(r => r.Id == roleItem.RoleId).FirstOrDefault());
            //}

            //var item = new CandidaturesItemViewModel
            //{
            //    User = User,
            //    Projet = Projet,
            //    Candidatures = new List<Candidature>(),
            //    Role = RolesUser.OrderBy(r => r.Name).ToList(),
            //    Specialite = spec.OrderBy(s => s.Name).ToList()
            //};

            //var CandidatureUsers = _db.Candidature.Where(c => c.UtilisateurId == User.Id && c.ProjetId == Projet.Id).ToList();
            //foreach (var CandidatureUser in CandidatureUsers)
            //{
            //    CandidatureUser.Besoins = _db.Besoins.Where(b => b.Id == CandidatureUser.BesoinsId).FirstOrDefault();
            //    CandidatureUser.Besoins.Role = _db.Role.Where(r => r.Id == CandidatureUser.Besoins.RoleId).FirstOrDefault();
            //    var BesoinsSpecs = _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == CandidatureUser.BesoinsId).ToList();
            //    CandidatureUser.Besoins.Specialite = new List<Specialite>();
            //    foreach (var besoinspec in BesoinsSpecs)
            //    {
            //        CandidatureUser.Besoins.Specialite.Add(_db.Specialite.Where(s => s.Id == besoinspec.SpecialiteId).FirstOrDefault());
            //    }
            //    item.Candidatures.Add(CandidatureUser);
            //}

            
            return ItemCandidaturePorteur;
        }
    }
}
