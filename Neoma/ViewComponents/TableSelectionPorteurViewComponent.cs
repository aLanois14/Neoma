using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Models;
using Neoma.Models.CandidatsViewModel;
using Neoma.Models.SelectionViewModel;

namespace Neoma.ViewComponents
{
    public class TableSelectionPorteurViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public TableSelectionPorteurViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(FilterViewModel Filter = null)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var ItemRoleSelection = new List<ItemRoleSelectionViewModel>();
            var Roles = new List<Role>();
            var Projet = Filter == null || (Filter != null && (Filter.Projet == null || Filter.Projet.Id == 0)) ?
                await _db.Projet.Where(p => p.UtilisateurId == claim && p.Complet == false && p.Termine == false && p.Actif == true).Select(x => x.Id).ToListAsync() :
                await _db.Projet.Where(p => p.UtilisateurId == claim && p.Id == Filter.Projet.Id).Select(x => x.Id).ToListAsync();
            if (Filter == null)
            {
                
                var Selections = await _db.Selection.Where(s => Projet.Contains(s.ProjetId)).ToListAsync();             
                foreach(var selection in Selections)
                {
                    selection.Besoins = await _db.Besoins.Where(b => b.Id == selection.BesoinsId).FirstOrDefaultAsync();
                    Roles.Add(await _db.Role.Where(r => r.Id == selection.Besoins.RoleId).FirstOrDefaultAsync());
                }
                Roles = Roles.Distinct().ToList();
                foreach(var role in Roles)
                {
                    var ItemSelection = new List<ItemSelectionViewModel>();
                    foreach (var select in Selections)
                    {
                        if(select.Besoins.RoleId == role.Id)
                        {
                            ItemSelection.Add(await FormatSelection(select, claim));
                        }                       
                    }
                    ItemRoleSelection.Add(new ItemRoleSelectionViewModel
                    {
                        ItemSelection = ItemSelection,
                        Role = role
                    });
                }
            }
            else
            {
                if(Filter.Besoins.Id != 0 && (Filter.Projet == null))
                {
                    var Selections = await _db.Selection.Where(s => Projet.Contains(s.ProjetId) && s.BesoinsId == Filter.Besoins.Id).ToListAsync();
                    var ItemSelection = new List<ItemSelectionViewModel>();
                    Filter.Besoins = await _db.Besoins.Where(b => b.Id == Filter.Besoins.Id).FirstOrDefaultAsync();
                    var Role = await _db.Role.Where(r => r.Id == Filter.Besoins.RoleId).FirstOrDefaultAsync();
                    foreach (var selection in Selections)
                    {
                        selection.Besoins = await _db.Besoins.Where(b => b.Id == Filter.Besoins.Id).FirstOrDefaultAsync();
                        selection.Utilisateur = await _db.Users.Where(u => u.Id == selection.UtilisateurId).FirstOrDefaultAsync();
                    }
                    if(Filter.Specialite != null)
                    {
                        if(Filter.Specialite.Count != 0)
                        {
                            foreach(var selection in Selections)
                            {
                                bool add = true;
                                foreach(var specialite in Filter.Specialite)
                                {
                                    if (!_db.SpecialiteUtilisateur.Any(su => su.SpecialiteId == specialite.Id && su.UtilisateurId == selection.UtilisateurId))
                                    {
                                        add = false;
                                        break;
                                    }
                                }
                                if (add)
                                {
                                    ItemSelection.Add(await FormatSelection(selection, claim));
                                }
                            }
                            if (ItemSelection.Count > 0)
                            {
                                ItemRoleSelection.Add(new ItemRoleSelectionViewModel
                                {
                                    Role = Role,
                                    ItemSelection = ItemSelection
                                });
                            }
                        }
                        else
                        {
                            foreach (var selection in Selections)
                            {
                                ItemSelection.Add(await FormatSelection(selection, claim));
                            }
                            if (ItemSelection.Count > 0)
                            {
                                ItemRoleSelection.Add(new ItemRoleSelectionViewModel
                                {
                                    Role = Role,
                                    ItemSelection = ItemSelection
                                });
                            }
                        }
                    }
                    else
                    {
                        foreach (var selection in Selections)
                        {
                            ItemSelection.Add(await FormatSelection(selection, claim));
                        }
                        if (ItemSelection.Count > 0)
                        {
                            ItemRoleSelection.Add(new ItemRoleSelectionViewModel
                            {
                                Role = Role,
                                ItemSelection = ItemSelection
                            });
                        }
                    }
                }
                else
                {
                    var Selections = await _db.Selection.Where(s => Projet.Contains(s.ProjetId)).ToListAsync();
                    foreach (var selection in Selections)
                    {
                        selection.Besoins = await _db.Besoins.Where(b => b.Id == selection.BesoinsId).FirstOrDefaultAsync();
                        Roles.Add(await _db.Role.Where(r => r.Id == selection.Besoins.RoleId).FirstOrDefaultAsync());
                    }
                    Roles = Roles.Distinct().ToList();
                    foreach (var role in Roles)
                    {
                        var ItemSelection = new List<ItemSelectionViewModel>();
                        foreach (var select in Selections)
                        {
                            if (select.Besoins.RoleId == role.Id)
                            {
                                ItemSelection.Add(await FormatSelection(select, claim));
                            }
                        }
                        ItemRoleSelection.Add(new ItemRoleSelectionViewModel
                        {
                            ItemSelection = ItemSelection,
                            Role = role
                        });
                    }
                }
            }

            ViewBag.TestFilter = Filter != null ? Filter.TexteRecherche : null;

            return View(ItemRoleSelection);
        }
        
        //private ItemSelectionViewModel Selection(Selection selection, string claim)
        //{
        //    var SelectionItem = new ItemSelectionViewModel();
        //    SelectionItem.User = _db.Users.Where(u => u.Id == selection.UtilisateurId).FirstOrDefault();
        //    SelectionItem.Projet = _db.Projet.Where(p => p.Id == selection.ProjetId).FirstOrDefault();

        //    var SpecialiteUser = _db.SpecialiteUtilisateur.Where(su => su.UtilisateurId == selection.UtilisateurId).ToList();
        //    var RoleUser = _db.RoleUtilisateur.Where(ru => ru.UtilisateurId == selection.UtilisateurId).ToList();
        //    List<Specialite> spec = new List<Specialite>();
        //    List<Role> RolesUser = new List<Role>();
        //    foreach (var specialite in SpecialiteUser)
        //    {
        //        Specialite special = _db.Specialite.Where(s => s.Id == specialite.SpecialiteId && (s.Valide || s.UtilisateurId == claim)).FirstOrDefault();
        //        if (special != null)
        //            spec.Add(special);
        //    }
        //    foreach (var roleItem in RoleUser)
        //    {
        //        RolesUser.Add(_db.Role.Where(r => r.Id == roleItem.RoleId).FirstOrDefault());
        //    }
        //    SelectionItem.Selection = selection;
        //    SelectionItem.Specialite = spec.OrderBy(s => s.Name).ToList();
        //    SelectionItem.Role = RolesUser.OrderBy(r => r.Name).ToList();

        //    return SelectionItem;
        //}

        public async Task<ItemSelectionViewModel> FormatSelection(Selection selection, string claim)
        {
            var ItemSelection = new ItemSelectionViewModel();
            ItemSelection.User = await _db.Users.Where(u => u.Id == selection.UtilisateurId).FirstOrDefaultAsync();
            ItemSelection.Projet = await _db.Projet.Where(p => p.Id == selection.ProjetId).FirstOrDefaultAsync();
            ItemSelection.Selection = selection;
            ItemSelection.ItemRole = new List<ItemRoleViewModel>();

            var SpecialiteUser = await _db.SpecialiteUtilisateur.Where(su => su.UtilisateurId == ItemSelection.User.Id).ToListAsync();
            var RoleUser = await _db.RoleUtilisateur.Where(ru => ru.UtilisateurId == ItemSelection.User.Id).ToListAsync();
            var Specialites = await _db.Specialite.Where(s => SpecialiteUser.Exists(x => x.SpecialiteId == s.Id) && (s.Valide || s.UtilisateurId == claim)).OrderBy(x => x.Name).ToListAsync();
            foreach (var role in RoleUser)
            {
                var ItemRole = new ItemRoleViewModel();
                ItemRole.Role = await _db.Role.Where(r => r.Id == role.RoleId).FirstOrDefaultAsync();
                ItemRole.Specialites = new List<Specialite>();
                foreach (var specialite in Specialites)
                {
                    if (specialite.RoleId == role.RoleId)
                    {
                        ItemRole.Specialites.Add(specialite);
                    }
                }
                ItemSelection.ItemRole.Add(ItemRole);
            }

            ItemSelection.ItemRole = ItemSelection.ItemRole.OrderBy(x => x.Role.Name).ToList();

            return ItemSelection;
        }
    }
}