using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Models;
using Neoma.Models.CandidatsViewModel;
using Neoma.Models.MatchingCandidatViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Neoma.ViewComponents
{
    public class TableCandidatMatchingViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public TableCandidatMatchingViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(FilterViewModel Filter = null)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var ListItemCandidat = new List<ItemCandidatViewModel>();

            var userId = new List<string>();
            var Projets = Filter == null || (Filter != null && Filter.Projet == null) ? await _db.Projet.Where(p => p.UtilisateurId == claim).ToListAsync() :
                await _db.Projet.Where(p => p.UtilisateurId == claim && p.Id == Filter.Projet.Id).ToListAsync();
            foreach(var projet in Projets)
            {
                projet.Besoins = await _db.Besoins.Where(b => b.ProjetId == projet.Id && b.UtilisateurId == null).ToListAsync();
                foreach(var besoin in projet.Besoins)
                {
                    var SpecialiteId = await _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == besoin.Id).Select(x => x.SpecialiteId).ToListAsync();
                    besoin.Specialite = await _db.Specialite.Where(s => SpecialiteId.Contains(s.Id)).ToListAsync();
                }
            }
            var Users = await _db.Users.Where(u => u.EstCandidat == true && u.Id != claim && u.Valide).ToListAsync();
            if (Filter == null)
            {
                foreach(var user in Users)
                {
                    var SpecialiteId = await _db.SpecialiteUtilisateur.Where(su => su.UtilisateurId == user.Id).Select(x => x.SpecialiteId).ToListAsync();
                    var Specialites = await _db.Specialite.Where(s => SpecialiteId.Contains(s.Id)).ToListAsync();
                    bool stop = false;
                    foreach (var projet in Projets)
                    {
                        if (!stop)
                        {
                            foreach (var besoin in projet.Besoins)
                            {
                                if (!stop)
                                {
                                    foreach (var specialite in Specialites)
                                    {
                                        if (besoin.Specialite.Any(s => s == specialite))
                                        {
                                            ListItemCandidat.Add(await FormatCandidat(user));
                                            stop = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }                       
                    }
                }
            }
            else
            {
                if(Filter.Besoins.Id != 0 && Filter.Projet == null)
                {
                    foreach (var user in Users)
                    {
                        var SpecialiteId = await _db.SpecialiteUtilisateur.Where(su => su.UtilisateurId == user.Id).Select(x => x.SpecialiteId).ToListAsync();
                        var Specialites = await _db.Specialite.Where(s => SpecialiteId.Contains(s.Id)).ToListAsync();
                        bool stop = false;
                        foreach (var projet in Projets)
                        {
                            if (projet.Besoins.Exists(b => b.Id == Filter.Besoins.Id))
                            {
                                if (!stop)
                                {
                                    foreach (var besoin in projet.Besoins)
                                    {
                                        if(besoin.Id == Filter.Besoins.Id)
                                        {
                                            if (!stop)
                                            {
                                                if(Filter.Specialite != null)
                                                {
                                                    if(Filter.Specialite.Count != 0)
                                                    {
                                                        var add = true;
                                                        foreach (var specialite in Filter.Specialite)
                                                        {
                                                            if (!Specialites.Any(s => s.Id == specialite.Id))
                                                            {
                                                                add = false;
                                                                stop = true;
                                                                break;
                                                            }
                                                        }
                                                        if (add)
                                                        {
                                                            ListItemCandidat.Add(await FormatCandidat(user));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        foreach (var specialite in Specialites)
                                                        {
                                                            if (besoin.Specialite.Any(s => s == specialite))
                                                            {
                                                                ListItemCandidat.Add(await FormatCandidat(user));
                                                                stop = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    foreach (var specialite in Specialites)
                                                    {
                                                        if (besoin.Specialite.Any(s => s == specialite))
                                                        {
                                                            ListItemCandidat.Add(await FormatCandidat(user));
                                                            stop = true;
                                                            break;
                                                        }
                                                    }
                                                }                                              
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }                                       
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }                            
                        }
                    }
                }
                else
                {
                    foreach (var user in Users)
                    {
                        var SpecialiteId = await _db.SpecialiteUtilisateur.Where(su => su.UtilisateurId == user.Id).Select(x => x.SpecialiteId).ToListAsync();
                        var Specialites = await _db.Specialite.Where(s => SpecialiteId.Contains(s.Id)).ToListAsync();
                        bool stop = false;
                        foreach (var projet in Projets)
                        {
                            if (!stop)
                            {
                                foreach (var besoin in projet.Besoins)
                                {
                                    if (!stop)
                                    {
                                        foreach (var specialite in Specialites)
                                        {
                                            if (besoin.Specialite.Any(s => s == specialite))
                                            {
                                                ListItemCandidat.Add(await FormatCandidat(user));
                                                stop = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            ViewBag.TestFilter = Filter != null ? Filter.TexteRecherche : null;
            return View(ListItemCandidat);
        }

        private async Task<ItemCandidatViewModel> FormatCandidat(ApplicationUser user)
        {
            var ItemCandidat = new ItemCandidatViewModel();
            ItemCandidat.User = user;
            ItemCandidat.Roles = new List<ItemRoleViewModel>();

            var SpecialiteUser = await _db.SpecialiteUtilisateur.Where(su => su.UtilisateurId == user.Id).ToListAsync();
            var RoleUser = await _db.RoleUtilisateur.Where(ru => ru.UtilisateurId == user.Id).ToListAsync();
            var Specialites = await _db.Specialite.Where(s => SpecialiteUser.Exists(x => x.SpecialiteId == s.Id)).OrderBy(x => x.Name).ToListAsync();
            foreach(var role in RoleUser)
            {
                var ItemRole = new ItemRoleViewModel();
                ItemRole.Role = await _db.Role.Where(r => r.Id == role.RoleId).FirstOrDefaultAsync();
                ItemRole.Specialites = new List<Specialite>();
                foreach(var specialite in Specialites)
                {
                    if(specialite.RoleId == role.RoleId)
                    {
                        ItemRole.Specialites.Add(specialite);
                    }
                }
                ItemCandidat.Roles.Add(ItemRole);
            }

            ItemCandidat.Roles = ItemCandidat.Roles.OrderBy(x => x.Role.Name).ToList();

            return ItemCandidat;
        }
    }
}
