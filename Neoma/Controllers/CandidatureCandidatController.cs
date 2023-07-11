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
using Neoma.Utility;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.CommonEndUser + "," + SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    public class CandidatureCandidatController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private IHubContext<MessagerieHub> _hub;

        public CandidatureCandidatController(ApplicationDbContext db, IHubContext<MessagerieHub> hub) : base(db)
        {
            _db = db;
            _hub = hub;
        }

        #region Affichage vue
            public async Task<IActionResult> Index()
            {
                var Candidatures = await _db.Candidature.Where(c => c.UtilisateurId == User.getUserId() && c.Statut != EValidation.Accepted.ToString()).ToListAsync();
                foreach(var candidature in Candidatures)
                {
                    candidature.Projet = await _db.Projet.Where(p => p.Id == candidature.ProjetId).FirstOrDefaultAsync();
                    candidature.Projet.Utilisateur = await _db.Users.Where(u => u.Id == candidature.Projet.UtilisateurId).FirstOrDefaultAsync();
                    candidature.Projet.TypeProjet = await _db.TypeProjet.Where(t => t.Id == candidature.Projet.TypeProjetId).FirstOrDefaultAsync();
                    candidature.Projet.Utilisateur.Organisme = await _db.Organisme.Where(o => o.Id == candidature.Projet.Utilisateur.OrganismeId).FirstOrDefaultAsync();
                    candidature.Besoins = await _db.Besoins.Where(b => b.Id == candidature.BesoinsId).FirstOrDefaultAsync();
                    candidature.Besoins.Role = await _db.Role.Where(r => r.Id == candidature.Besoins.RoleId).FirstOrDefaultAsync();
                }
                return View(Candidatures);
            }

            public async Task<IActionResult> DetailProjet(int Id)
            {             
                var SpecialitesUtilisateurIds = await _db.SpecialiteUtilisateur.Where(s => s.UtilisateurId == User.getUserId()).Select(x => x.SpecialiteId).ToListAsync();

                var projet = await _db.Projet.Where(p => p.Id == Id).FirstOrDefaultAsync();
                var besoins = await _db.Besoins.Where(b => b.Projet == projet).ToListAsync();
                projet.Utilisateur = await _db.Users.Where(u => u.Id == projet.UtilisateurId).FirstOrDefaultAsync();
                projet.Utilisateur.Organisme = await _db.Organisme.Where(o => o.Id == projet.Utilisateur.OrganismeId).FirstOrDefaultAsync();
                projet.TypeProjet = await _db.TypeProjet.Where(t => t.Id == projet.TypeProjetId).FirstOrDefaultAsync();
                foreach (var besoin in besoins)
                {
                    var Candidatures = await _db.Candidature.Where(c => c.BesoinsId == besoin.Id).ToListAsync();
                    var specId = await _db.BesoinsSpecialite.Where(bs => bs.BesoinsId == besoin.Id).Select(x => x.SpecialiteId).ToListAsync();
                    besoin.Specialite = await _db.Specialite.Where(s => specId.Contains(s.Id) && (s.Valide || s.UtilisateurId == User.getUserId())).ToListAsync();
                    besoin.Role = await _db.Role.Where(r => r.Id == besoin.RoleId).FirstOrDefaultAsync();

                    besoin.PeutPostuler = false;

                    besoin.PeutPostuler = besoin.PeutPostuler || !Candidatures.Any(c => c.UtilisateurId == User.getUserId());

                    if (besoin.PeutPostuler)
                    {
                        foreach (int CetId in specId)
                        {
                            if (SpecialitesUtilisateurIds.Contains(CetId))
                            {
                                besoin.PeutPostuler = true;
                            }
                            else
                            {
                                besoin.PeutPostuler = false;
                            }
                        }
                    }

                    foreach (var specialite in besoin.Specialite)
                    {
                        specialite.Match = SpecialitesUtilisateurIds.Contains(specialite.Id) ? true : false;
                    }
                }

                ViewBag.Url = Request.Headers["Referer"].ToString();
                projet.Besoins = besoins;
                return View(projet);
            }
        #endregion

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var candidature = await _db.Candidature.Where(c => c.Id == id).FirstOrDefaultAsync();
            _db.Remove(candidature);
            await _db.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("displayNotification");
            return Json(new { success = true, result = "/CandidatureCandidat" });
        }
    }
}