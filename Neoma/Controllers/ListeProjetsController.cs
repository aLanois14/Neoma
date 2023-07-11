using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neoma.Models;
using Neoma.Data;
using Neoma.Models.ProjetViewModel;
using Microsoft.AspNetCore.Authorization;
using Neoma.Utility;
using Microsoft.AspNetCore.Http;
using Neoma.Extensions;
using Microsoft.EntityFrameworkCore;
using Neoma.Models.CandidatViewModel;
using Microsoft.AspNetCore.SignalR;
using Neoma.Hubs;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.CommonEndUser + "," + SD.AdminEndUser + "," + SD.SuperEndUser + "," + SD.SuperAdminEndUser)]
    public class ListeProjetsController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private IHubContext<MessagerieHub> _hub;

        public ListeProjetsController(ApplicationDbContext db, IHubContext<MessagerieHub> hub) : base(db)
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
                    Id = 0,
                    Name = "Tous les organismes"
                });
                index.Role.Add(new Role
                {
                    Id = 0,
                    Name = "Tous les rôles"
                });
                index.TypeProjet.Add(new TypeProjet
                {
                    Id = 0,
                    Name = "Tous les types"
                });

                var Organismes = await _db.Organisme.ToListAsync();
                var Roles = await _db.Role.ToListAsync();
                var TypeProjets = await _db.TypeProjet.ToListAsync();

                foreach (var organisme in Organismes)
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

                index.Specialite = new List<Specialite>();

                return View(index);
            }
         
            public IActionResult TableProjet([FromBody]SelectedFilterProject Filter)
            {
                return ViewComponent("TableProjet", new { Select = Filter });
            }
        #endregion      

        public async Task<IActionResult> UpdateSpecialite(int role)
        {
            string userId = User.getUserId();
            var Specialite = await _db.Specialite.Where(s => s.RoleId == role && (s.Valide || s.UtilisateurId == userId)).ToListAsync();
            return new JsonResult(Specialite);
        }
    }
}
