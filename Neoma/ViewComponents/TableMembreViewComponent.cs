using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using System.Linq;
using System.Threading.Tasks;
using Neoma.Models.MembreProjetViewModel;
using Neoma.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace Neoma.ViewComponents
{
    public class TableMembreViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public TableMembreViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(Membre membre = null)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            MembreProjetViewModel Member = new MembreProjetViewModel();
            if (membre == null)
            {              
                Member.Role = await _db.Role.ToListAsync();
                Member.Specialite = _db.Specialite.Where(s => s.RoleId == Member.Role[0].Id && (s.Valide == true || s.UtilisateurId == claim)).ToList();
            }
            else
            {
                Member.Membre = new List<Membre>();
                Member.Membre.Add(membre);
                Member.Role = await _db.Role.ToListAsync();
                var Role = membre.Specialite[0].RoleId;
                Member.Specialite = _db.Specialite.Where(s => s.RoleId == Role && (s.Valide == true || s.UtilisateurId == claim)).ToList();
                foreach(var specialite in Member.Specialite)
                {
                    specialite.Match = false;
                    foreach(var item in membre.Specialite)
                    {
                        if(item == specialite)
                        {
                            specialite.Match = true;
                        }
                    }
                }
            }
            return View(Member);
        }
    }
}
