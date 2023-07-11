using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Models;
using Neoma.Models.MembreProjetViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Neoma.ViewComponents
{
    public class TableBesoinViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public TableBesoinViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(Besoins besoin = null)
        {
            MembreProjetViewModel Member = new MembreProjetViewModel();
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            string claim = "";
            var findClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            claim = findClaim != null ? findClaim.Value : "";
            if (besoin == null)
            {
                Member.Role = await _db.Role.ToListAsync();
                Member.Specialite = await _db.Specialite.Where(s => s.RoleId == Member.Role[0].Id && (s.Valide == true || s.UtilisateurId == claim)).ToListAsync();
            }
            else
            {
                Member.Besoins = new List<Besoins>();
                Member.Besoins.Add(besoin);
                foreach(var besoins in Member.Besoins)
                {
                    besoins.Projet = await _db.Projet.Where(p => p.Id == besoins.ProjetId).FirstOrDefaultAsync();
                    besoins.Utilisateur = besoin.UtilisateurId == null ? null : await _db.Users.Where(u => u.Id == besoins.UtilisateurId).FirstOrDefaultAsync();
                }
                Member.Role = await _db.Role.ToListAsync();
                var Role = besoin.Specialite[0].RoleId;
                Member.Specialite = await _db.Specialite.Where(s => s.RoleId == Role && (s.Valide == true || s.UtilisateurId == claim)).ToListAsync();
                foreach (var specialite in Member.Specialite)
                {
                    specialite.Match = false;
                    foreach (var item in besoin.Specialite)
                    {
                        if (item == specialite)
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
