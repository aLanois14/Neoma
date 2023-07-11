using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Extensions;
using Neoma.Hubs;
using Neoma.Models;
using Neoma.Models.CandidatsViewModel;
using Neoma.Models.MatchingViewModel;

namespace Neoma.Controllers
{
    public class MatchingPorteurController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private IHubContext<MessagerieHub> _hub;

        public MatchingPorteurController(ApplicationDbContext db, IHubContext<MessagerieHub> hub) : base(db)
        {
            _db = db;
            _hub = hub;
        }

        #region Affichage vue
            public async Task<IActionResult> Index()
            {
                MatchCandidatViewModel CandidatViewModel = new MatchCandidatViewModel();
                CandidatViewModel.Projet = new List<Projet>();
                CandidatViewModel.Role = new List<Role>();
                CandidatViewModel.Specialite = new List<Specialite>();
                CandidatViewModel.Role.Add(new Role
                {
                    Name = "Tous les rôles"
                });
                CandidatViewModel.Projet.Add(new Projet{
                    Name = "Tous les projets"
                });
                List<Projet> Projets = await _db.Projet.Where(p => p.UtilisateurId == User.getUserId()).ToListAsync();
                foreach(var Projet in Projets)
                {
                    Besoins testBesoins = await _db.Besoins.FirstOrDefaultAsync<Besoins>(b => b.ProjetId == Projet.Id && b.UtilisateurId == null);

                    if (testBesoins != null)
                        CandidatViewModel.Projet.Add(Projet);
                }

                return View(CandidatViewModel);
            }

            public IActionResult TableCandidatMatching([FromBody]FilterViewModel Filter)
            {
                return ViewComponent("TableCandidatMatching", new { Filter = Filter });
            }

            public IActionResult SelectionView(int id, string user)
            {
                return ViewComponent("Selection", new { User = _db.Users.Where(u => u.Id == user).FirstOrDefault(), Role = _db.Role.Where(r => r.Id == id).FirstOrDefault() });
            }
        #endregion

        public async Task<IActionResult> UpdateRole(int projet)
        {
            List<Besoins> Besoins = new List<Besoins>();
            Besoins.Add(new Besoins
            {
                Role = new Role()
            });

            var listBesoin = await _db.Besoins.Where(b => b.ProjetId == projet && b.UtilisateurId == null).ToListAsync();
            foreach (var besoin in listBesoin)
            {

                besoin.Role = await _db.Role.Where(r => r.Id == besoin.RoleId).FirstOrDefaultAsync();
                Besoins.Add(besoin);
            }

            return new JsonResult(Besoins);
        }

        public async Task<IActionResult> UpdateSpecialite(int besoins)
        {
            List<Specialite> Specialite = new List<Specialite>();
            if (besoins != 0)
            {
                var Besoin = await _db.Besoins.Where(b => b.Id == besoins).FirstOrDefaultAsync();
                var BesoinSpecialite = await _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == Besoin.Id).ToListAsync();

                foreach (var besoinspec in BesoinSpecialite)
                {
                    Specialite.Add(await _db.Specialite.Where(s => s.Id == besoinspec.SpecialiteId).FirstOrDefaultAsync());
                }
            }
            return new JsonResult(Specialite);
        }

        [HttpPost]
        public async Task<IActionResult> AddSelection(Selection Selection)
        {
            if (ModelState.IsValid)
            {
                await _db.AddAsync(Selection);
                await _db.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("displayNotification");
            }

            return RedirectToAction("Index", "Selections");
        }
    }
}