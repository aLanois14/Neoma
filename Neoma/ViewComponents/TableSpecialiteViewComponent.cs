using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Models;
using Neoma.Models.SpecialiteViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.ViewComponents
{
    public class TableSpecialiteViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public TableSpecialiteViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int RoleId)
        {
            if (RoleId == 0)
            {
                var Specialites = await _db.Specialite.ToListAsync();
                foreach (var specialite in Specialites)
                {
                    specialite.CanDelete = !await _db.SpecialiteUtilisateur.AnyAsync<SpecialiteUtilisateur>(su => su.SpecialiteId == specialite.Id) && !await _db.BesoinsSpecialite.AnyAsync<BesoinsSpecialite>(bs => bs.SpecialiteId == specialite.Id) && !await _db.MembreSpecialite.AnyAsync<MembreSpecialite>(ms => ms.SpecialiteId == specialite.Id) ? true : false;
                }
                SpecialiteRoleViewModel SpecialiteRole = new SpecialiteRoleViewModel()
                {
                    SpecialiteList = Specialites,
                    RoleList = await _db.Role.ToListAsync()
                };
                return View(SpecialiteRole);
            }
            else
            {
                SpecialiteRoleViewModel SpecialiteRole = new SpecialiteRoleViewModel()
                {
                    SpecialiteList = _db.Specialite.Where(m => m.RoleId == RoleId).ToList(),
                    RoleList = await _db.Role.ToListAsync(),
                    Role = _db.Role.Where(m => m.Id == RoleId).FirstOrDefault()
                };
                return View(SpecialiteRole);
            }
        }
    }
}
