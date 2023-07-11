using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Models;
using Neoma.Models.MembreProjetViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Neoma.ViewComponents
{
    public class TableSpecialiteUserViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public TableSpecialiteUserViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            MembreProjetViewModel Member = new MembreProjetViewModel();
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            Member.Role = await _db.Role.ToListAsync();
            var SpecialiteUser = await _db.SpecialiteUtilisateur.Where(su => su.UtilisateurId == claim).ToListAsync();

            Member.Specialite = _db.Specialite.Where(s => s.RoleId == Member.Role[0].Id && (s.Valide == true || s.UtilisateurId == claim) && !SpecialiteUser.Exists(x => x.SpecialiteId == s.Id)).ToList();

            return View(Member);
        }
    }
}
