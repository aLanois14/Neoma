using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Extensions;
using Neoma.Hubs;
using Neoma.Models;
using Neoma.Models.ProjetViewModel;
using Neoma.Utility;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.CommonEndUser + "," + SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    public class MatchingCandidatController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private IHubContext<MessagerieHub> _hub;

        public MatchingCandidatController(ApplicationDbContext db, IHubContext<MessagerieHub> hub) : base(db)
        {
            _db = db;
            _hub = hub;
        }

        #region Affichage vue
            public async Task<IActionResult> Index()
            {
                ViewProjetViewModel index = new ViewProjetViewModel()
                {
                    TypeProjet = new List<TypeProjet>(),
                    Role = new List<Role>(),
                    Organisme = new List<Organisme>()
                };

                index.Organisme.Add(new Organisme
                {
                    Name = "Tous les organismes"
                });
                index.Role.Add(new Role
                {
                    Name = "Tous les rôles"
                });
                index.TypeProjet.Add(new TypeProjet
                {
                    Name = "Tous les types"
                });

                var Organismes = await _db.Organisme.ToListAsync();
                var Roles = await _db.Role.ToListAsync();
                var TypeProjets = await _db.TypeProjet.ToListAsync();
                
                foreach(var organisme in Organismes)
                {
                    index.Organisme.Add(organisme);
                }

                foreach (var role in Roles)
                {
                    var specialites = await _db.Specialite.Where(s => s.RoleId == role.Id && s.Valide).FirstOrDefaultAsync();
                    if (specialites != null)
                    {
                        index.Role.Add(role);
                    }
                }

                foreach (var type in TypeProjets)
                {
                    index.TypeProjet.Add(type);
                }

                return View(index);
            }          

            public IActionResult TableProjet([FromBody]SelectedFilterProject Filter)
            {
                return ViewComponent("TableProjetMatching", new { Select = Filter });
            }
        #endregion

        [HttpPost]
        public async Task<JsonResult> Postuler(int id)
        {
            var candidature = new Candidature();
            candidature.Besoins = await _db.Besoins.Where(b => b.Id == id).FirstOrDefaultAsync();
            candidature.Utilisateur = await _db.Users.Where(u => u.Id == User.getUserId()).FirstOrDefaultAsync();
            candidature.Projet = await _db.Projet.Where(p => p.Id == candidature.Besoins.ProjetId).FirstOrDefaultAsync();
            candidature.Statut = EValidation.Pending.ToString();
            _db.Add(candidature);
            await _db.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("displayNotification");
            return Json(new { success = true, result = "/MatchingCandidat" });
        }
    }
}