using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neoma.Models;
using Neoma.Models.PorteurViewModel;
using Neoma.Data;
using Microsoft.AspNetCore.Authorization;
using Neoma.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Neoma.Extensions;
using Neoma.Models.CandidatsViewModel;
using Microsoft.AspNetCore.SignalR;
using Neoma.Hubs;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.CommonEndUser + "," + SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    public class PorteurController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private IHubContext<MessagerieHub> _hub;

        public PorteurController(ApplicationDbContext db, IHubContext<MessagerieHub> hub) : base(db)
        {
            _db = db;
            _hub = hub;
        }

        #region Affichage vue
            public IActionResult Index()
            {
                ViewPorteurViewModel index = new ViewPorteurViewModel();
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

            public async Task<IActionResult> DetailPorteur(string Id)
            {
                string userId = User.getUserId();
                var ItemPorteur = new ItemPorteurViewModel();
                ItemPorteur.User = await _db.Users.Where(u => u.Id == Id).FirstOrDefaultAsync();
                ItemPorteur.Organisme = await _db.Organisme.Where(o => o.Id == ItemPorteur.User.OrganismeId).FirstOrDefaultAsync();
                ItemPorteur.ItemRole = new List<ItemRoleViewModel>();
                ItemPorteur.Projet = await _db.Projet.Where(p => p.UtilisateurId == Id && !p.Complet && p.Actif && !p.Termine).ToListAsync();

                var specialiteUser = await _db.SpecialiteUtilisateur.Where(s => s.UtilisateurId == Id).ToListAsync();
                var roleUser = await _db.RoleUtilisateur.Where(r => r.UtilisateurId == Id).ToListAsync();
                var Specialites = await _db.Specialite.Where(s => specialiteUser.Exists(x => x.SpecialiteId == s.Id) && (s.Valide || s.UtilisateurId == userId)).OrderBy(x => x.Name).ToListAsync();
                foreach(var role in roleUser)
                {                  
                    var ItemRole = new ItemRoleViewModel();
                    ItemRole.Role = await _db.Role.Where(r => r.Id == role.RoleId).FirstOrDefaultAsync();
                    ItemRole.Specialites = new List<Specialite>();
                    foreach (var specialite in Specialites)
                    {
                        if (specialite != null && specialite.RoleId == role.RoleId)
                        {
                            ItemRole.Specialites.Add(specialite);
                        }
                    }
                    ItemPorteur.ItemRole.Add(ItemRole);
                }

                foreach (var projet in ItemPorteur.Projet)
                {
                    projet.TypeProjet = await _db.TypeProjet.Where(tp => tp.Id == projet.TypeProjetId).FirstOrDefaultAsync();
                    projet.Besoins = await _db.Besoins.Where(b => b.ProjetId == projet.Id).ToListAsync();
                    foreach (var besoin in projet.Besoins)
                    {
                        besoin.Role = await _db.Role.Where(r => r.Id == besoin.RoleId).FirstOrDefaultAsync();
                    }
                }

                ViewBag.Url = Request.Headers["Referer"].ToString();
                return View(ItemPorteur);
            }

            public IActionResult TablePorteurSpecialite([FromBody]FilterViewModel Filter)
            {
                return ViewComponent("TablePorteur", new { Filter = Filter });
            }
        #endregion

        #region Action
            public async Task<IActionResult> UpdateSpecialite(int role)
            {
                string userId = User.getUserId();
                var Specialite = _db.Specialite.Where(s => s.RoleId == role && (s.Valide || s.UtilisateurId == userId)).ToList();
                return new JsonResult(Specialite);
            }          
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
