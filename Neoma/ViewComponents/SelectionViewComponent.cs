using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Extensions;
using Neoma.Models;
using Neoma.Models.SelectionViewModel;

namespace Neoma.ViewComponents
{
    public class SelectionViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public SelectionViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(ApplicationUser User, Role Role)
        {
            HttpContext.Session.SetInt32("RoleSelect", Role.Id);
            var SpecialitesCandidat = await _db.SpecialiteUtilisateur.Where(s => s.UtilisateurId == User.Id).Select(s => s.SpecialiteId).ToListAsync();
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var Projets = await _db.Projet.Where(p => p.UtilisateurId == claim && !p.Complet && !p.Termine && p.Actif).ToListAsync();

            var ProjetsSelected = new List<Projet>();

            foreach (var projet in Projets)
            {
                var exist = true;
                var ProjetSelect = projet;
                ProjetSelect.Besoins = new List<Besoins>();
                var BesoinsProjet = await _db.Besoins.Where(b => b.ProjetId == projet.Id && b.RoleId == Role.Id && _db.BesoinsSpecialite.Any(bs => bs.BesoinsId == b.Id && SpecialitesCandidat.Contains(bs.SpecialiteId))).ToListAsync();
                foreach(var besoin in BesoinsProjet)
                {
                    var selection = await _db.Selection.Where(s => s.UtilisateurId == User.Id && s.BesoinsId == besoin.Id).FirstOrDefaultAsync();
                    var Proposition = await _db.Proposition.Where(p => p.UtilisateurId == User.Id && p.BesoinsId == besoin.Id).FirstOrDefaultAsync();
                    if (selection == null && besoin.UtilisateurId == null && Proposition == null)
                    {
                        ProjetSelect.Besoins.Add(besoin);
                        exist = false;
                    }
                }
             
                if (!exist)
                {
                    ProjetsSelected.Add(ProjetSelect);
                }
            }

            var Besoins = ProjetsSelected.First().Besoins;
            foreach (var besoin in Besoins)
            {
                var BesoinsSpec = await _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == besoin.Id).ToListAsync();
                besoin.Specialite = await _db.Specialite.Where(s => BesoinsSpec.Exists(x => x.SpecialiteId == s.Id)).ToListAsync();
            }

            SelectionViewModel model = new SelectionViewModel
            {
                User = User,
                Projets = ProjetsSelected,
                Besoins = Besoins
            };

            return View(model);
        }
    }
    
}