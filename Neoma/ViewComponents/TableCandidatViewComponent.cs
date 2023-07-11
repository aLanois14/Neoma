using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Utility;
using Neoma.Extensions;
using Neoma.Models;
using Neoma.Models.CandidatsViewModel;
using Neoma.Models.CandidatViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Neoma.ViewComponents
{
    public class TableCandidatViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public TableCandidatViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(FilterViewModel Filter = null)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            int OrganismeId = 0;
            if (User.IsInRole(SD.SuperEndUser))
                OrganismeId = await _db.ApplicationUser.Where(u => u.Id == claim).Select(u => u.OrganismeId).FirstOrDefaultAsync();

            List<ItemCandidatViewModel> ListItemCandidat = new List<ItemCandidatViewModel>();
            List<ApplicationUser> userSelected = new List<ApplicationUser>();
            if (Filter == null)
            {
                var user = _db.RoleUtilisateur.Where(r => r.UtilisateurId != claim).ToList();

                if (OrganismeId == 0)
                    userSelected = await _db.Users.Where(u => u.EstCandidat && user.Exists(x => x.UtilisateurId == u.Id) && u.Valide).ToListAsync();
                else
                    userSelected = await _db.Users.Where(u => u.EstCandidat && user.Exists(x => x.UtilisateurId == u.Id) && u.Valide && u.OrganismeId == OrganismeId).ToListAsync();
            }
            else
            {
                if (Filter.Role.Id != 0)
                {
                    var userRole = await _db.RoleUtilisateur.Where(ru => ru.RoleId == Filter.Role.Id && ru.UtilisateurId != claim).ToListAsync();
                    var users = OrganismeId == 0 ? await _db.Users.Where(u => u.EstCandidat && userRole.Exists(x => x.UtilisateurId == u.Id) && u.Valide).ToListAsync() :
                         await _db.Users.Where(u => u.EstCandidat && userRole.Exists(x => x.UtilisateurId == u.Id) && u.Valide && u.OrganismeId == OrganismeId).ToListAsync();

                    if (Filter.Specialite != null)
                    {
                        if (Filter.Specialite.Count != 0)
                        {
                            foreach (var user in users)
                            {
                                bool delete = false;
                                foreach (var test in Filter.Specialite)
                                {
                                    if (!_db.SpecialiteUtilisateur.Any(s => s.SpecialiteId == test.Id && s.UtilisateurId == user.Id))
                                    {
                                        delete = true;
                                    }
                                }
                                if (!delete)
                                {
                                    userSelected.Add(user);
                                }
                            }
                        }
                        else
                        {
                            userSelected = users;
                        }
                    }
                    else
                    {
                        userSelected = OrganismeId == 0 ? await _db.Users.Where(u => u.EstCandidat && users.Exists(x => x.Id == u.Id) && u.Valide).ToListAsync() :
                            await _db.Users.Where(u => u.EstCandidat && users.Exists(x => x.Id == u.Id) && u.Valide && u.OrganismeId == OrganismeId).ToListAsync();
                    }
                }
                else
                {
                    var user = _db.RoleUtilisateur.Where(r => r.UtilisateurId != claim).ToList();
                    userSelected = OrganismeId == 0 ? await _db.Users.Where(u => u.EstCandidat && user.Exists(x => x.UtilisateurId == u.Id) && u.Valide).ToListAsync() :
                        await _db.Users.Where(u => u.EstCandidat && user.Exists(x => x.UtilisateurId == u.Id) && u.Valide && u.OrganismeId == OrganismeId).ToListAsync();
                }
            }
            ListItemCandidat = ListCandidat(userSelected, claim);

            ViewBag.TestFilter = Filter != null ? Filter.TexteRecherche : null;

            return View(ListItemCandidat);
        }       

        private List<ItemCandidatViewModel> ListCandidat(List<ApplicationUser> listUser, string claim)
        {
            List<ItemCandidatViewModel> ListItemCandidat = new List<ItemCandidatViewModel>();
            foreach (var item in listUser)
            {
                var specialiteUser = _db.SpecialiteUtilisateur.Where(s => s.UtilisateurId == item.Id).ToList();
                var roleUser = _db.RoleUtilisateur.Where(r => r.UtilisateurId == item.Id).ToList();
                List<Specialite> spec = new List<Specialite>();
                List<Role> Roles = new List<Role>();
                foreach (var specialite in specialiteUser)
                {
                    Specialite special = _db.Specialite.Where(s => s.Id == specialite.SpecialiteId && (s.Valide || s.UtilisateurId == claim)).FirstOrDefault();
                    if (special != null)
                        spec.Add(special);
                }

                foreach (var role in roleUser)
                {
                    Roles.Add(_db.Role.Where(r => r.Id == role.RoleId).FirstOrDefault());
                }

                spec = spec.OrderBy(s => s.Name).ToList();
                Roles = Roles.OrderBy(r => r.Name).ToList();
                ListItemCandidat.Add(new ItemCandidatViewModel
                {
                    User = item,
                    Specialite = spec,
                    Role = Roles
                });
            }

            return ListItemCandidat;
        }
    }
}
