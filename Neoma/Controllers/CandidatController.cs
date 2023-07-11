using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neoma.Models;
using Neoma.Models.CandidatViewModel;
using Neoma.Data;
using Microsoft.AspNetCore.Authorization;
using Neoma.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Neoma.Extensions;
using Neoma.Models.CandidatsViewModel;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.CommonEndUser + "," + SD.AdminEndUser + "," + SD.SuperEndUser + "," + SD.SuperAdminEndUser)]
    public class CandidatController : BaseController
    {
        private readonly ApplicationDbContext _db;

        public CandidatController(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        #region Affichage vue
            public IActionResult Index()
            {
                ViewCandidatViewModel index = new ViewCandidatViewModel();
                index.Role = new List<Role>();
                index.Role.Add(new Role {
                    Name = "Tous les rôles"
                });
                var roles = _db.Role.ToList();
                foreach(var role in roles)
                {
                    index.Role.Add(role);
                }

                index.Specialite = new List<Specialite>();
                return View(index);
            }

            public IActionResult SelectionView(int id, string user)
            {
                return ViewComponent("Selection", new { User = _db.Users.Where(u => u.Id == user).FirstOrDefault(), Role = _db.Role.Where(r => r.Id == id).FirstOrDefault() });
            }

            public IActionResult TableCandidatSpecialite([FromBody]FilterViewModel Filter)
            {
                return ViewComponent("TableCandidat", new { Filter = Filter });
            }
        #endregion

        #region Action
            public async Task<IActionResult> UpdateSpecialite(int role)
            {
                string userId = User.getUserId();
                var Specialite = _db.Specialite.Where(s => s.RoleId == role && (s.Valide || s.UtilisateurId == userId)).ToList();
                return new JsonResult(Specialite);
            }

            public async Task<IActionResult> UpdateSpecialiteBesoins(int role)
            {
                var SpecialiteBesoin = await _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == role).ToListAsync();
                var Specialite = await _db.Specialite.Where(s => SpecialiteBesoin.Exists(x => x.SpecialiteId == s.Id)).ToListAsync();
                return new JsonResult(Specialite);
            }

            public async Task<IActionResult> UpdateRoleProjet(int projet, string user)
            {
                var BesoinsShow = new List<Besoins>();
                var Besoins = await _db.Besoins.Where(b => b.ProjetId == projet && b.RoleId == HttpContext.Session.GetInt32("RoleSelect")).ToListAsync();

                foreach (var besoin in Besoins)
                {
                    var selection = await _db.Selection.Where(s => s.UtilisateurId == user && s.BesoinsId == besoin.Id).FirstOrDefaultAsync();
                    var Proposition = await _db.Proposition.Where(p => p.UtilisateurId == user && p.BesoinsId == besoin.Id).FirstOrDefaultAsync();
                    if (selection == null && besoin.UtilisateurId == null && Proposition == null)
                    {
                        besoin.Role = await _db.Role.Where(r => r.Id == besoin.RoleId).FirstOrDefaultAsync();
                        var BesoinsSpec = await _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == besoin.Id).ToListAsync();
                        besoin.Specialite = await _db.Specialite.Where(s => BesoinsSpec.Exists(x => x.SpecialiteId == s.Id)).ToListAsync();
                        var specialiteText = "(";
                        if(besoin.Specialite.Count == 1)
                        {
                            specialiteText += besoin.Specialite[0].Name + ")";
                        }
                        else
                        {
                            if(besoin.Specialite.Count <= 3)
                            {
                                var last = besoin.Specialite.Last();

                                foreach (var special in besoin.Specialite)
                                {
                                    specialiteText += !special.Equals(last) ? special.Name + " - " : special.Name + ")";
                                }
                            }
                            else
                            {
                                for(int i = 0; i < 3; i++)
                                {
                                    specialiteText += besoin.Specialite[i].Name + " - ";
                                }
                                specialiteText += " ... )";
                            }
                        }
                        BesoinsShow.Add(new Besoins
                        {
                            Id = besoin.Id,
                            Specialite = besoin.Specialite,
                            Role = new Role
                            {
                                Name = besoin.Role.Name + " " + specialiteText
                            }
                        });
                    }

                    
                }
                return new JsonResult(BesoinsShow);
            }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
