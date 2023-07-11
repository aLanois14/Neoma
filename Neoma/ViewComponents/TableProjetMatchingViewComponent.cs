using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Models;
using Neoma.Models.ProjetViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Neoma.ViewComponents
{
    public class TableProjetMatchingViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public TableProjetMatchingViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(SelectedFilterProject Select = null)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<ItemProjetViewModel> ListItemProjet = new List<ItemProjetViewModel>();

            var User = await _db.Users.Where(u => u.Id == claim).FirstOrDefaultAsync();
            var SpecialiteUser = await _db.SpecialiteUtilisateur.Where(su => su.UtilisateurId == User.Id).Select(x => x.SpecialiteId).ToListAsync();
            var Specialites = await _db.Specialite.Where(s => SpecialiteUser.Contains(s.Id) && (s.Valide || s.UtilisateurId == claim)).ToListAsync();

            if (Select == null)
            {
                var listProjet = await _db.Projet.Where(p => p.Actif == true && p.Complet == false && p.Termine == false && p.UtilisateurId != claim).ToListAsync();
                ListItemProjet = await formatProjet(listProjet, Specialites);
            }
            else
            {
                if (Select.Organisme == 0 && Select.Role == 0 && Select.TypeProjet == 0)
                {
                    var Projets = await _db.Projet.Where(p => p.Actif == true && p.Complet == false && p.Termine == false && p.UtilisateurId != claim).ToListAsync();
                    ListItemProjet = await formatProjet(Projets, Specialites);
                }
                else
                {
                    //Récupération de tous les utilisateurs appartenant à l'organisme choisi
                    var UserOrganisme = Select.Organisme != 0 ? 
                        await _db.Users.Where(u => u.OrganismeId == Select.Organisme && u.Id != claim).ToListAsync() : 
                        await _db.Users.Where(u => u.Id != claim).ToListAsync();

                    //Récupération des projets des utilisateurs sélectionnés
                    var ListProjet = await _db.Projet.Where(p => UserOrganisme.Exists(x => x.Id == p.UtilisateurId) && p.Actif == true && p.Complet == false && p.Termine == false).ToListAsync();

                    //Récupération des besoins en fonction du role sélectionné et des projets
                    var Besoins = Select.Role != 0 ? 
                        await _db.Besoins.Where(b => b.RoleId == Select.Role && ListProjet.Exists(x => x.Id == b.ProjetId)).ToListAsync() : 
                        await _db.Besoins.Where(b => ListProjet.Exists(x => x.Id == b.ProjetId)).ToListAsync();

                    //Récupération des projets répondant aux critères 
                    var Projets = Select.TypeProjet == 0 ? 
                        await _db.Projet.Where(p => UserOrganisme.Exists(u => u.Id == p.UtilisateurId) && Besoins.Exists(b => b.ProjetId == p.Id)).ToListAsync() : 
                        await _db.Projet.Where(p => UserOrganisme.Exists(u => u.Id == p.UtilisateurId) && Besoins.Exists(b => b.ProjetId == p.Id) && p.TypeProjetId == Select.TypeProjet).ToListAsync();


                    ListItemProjet = await formatProjet(Projets, Specialites);

                    ListItemProjet = ListItemProjet.Distinct().ToList();
                }
            }
            ViewBag.TestFilter = Select != null ? Select.TexteRecherche : null;

            return View(ListItemProjet);
        }

        public async Task<List<ItemProjetViewModel>> formatProjet(List<Projet> listProjet, List<Specialite> Specialites)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            List<ItemProjetViewModel> ListItemProjet = new List<ItemProjetViewModel>();

            foreach (var projet in listProjet)
            {
                var Besoin = await _db.Besoins.Where(b => b.ProjetId == projet.Id).ToListAsync();
                bool stop = false;
                List<ItemBesoinViewModel> ListItemBesoin = new List<ItemBesoinViewModel>();
                foreach (var besoin in Besoin)
                {
                    var listSpec = await _db.BesoinsSpecialite.Where(s => s.Besoins == besoin).Select(x => x.SpecialiteId).ToListAsync();
                    besoin.Specialite = _db.Specialite.Where(s => listSpec.Contains(s.Id) && (s.Valide || s.UtilisateurId == claim)).ToList();
                    if (!stop)
                    {
                        foreach (var specialite in besoin.Specialite)
                        {
                            if (Specialites.Any(s => s == specialite))
                            {
                                stop = true;
                                break;
                            }
                        }
                    }
                    ListItemBesoin.Add(new ItemBesoinViewModel
                    {
                        Besoins = besoin,
                        Role = await _db.Role.Where(r => r.Id == besoin.RoleId).FirstOrDefaultAsync(),
                        Candidatures = await _db.Candidature.Where(c => c.BesoinsId == besoin.Id && c.Statut.ToString() == "Pending").ToListAsync(),
                        User = besoin.UtilisateurId == null ? null : await _db.Users.Where(u => u.Id == besoin.UtilisateurId).FirstOrDefaultAsync(),
                        Specialites = await _db.Specialite.Where(s => listSpec.Contains(s.Id) && (s.Valide || s.UtilisateurId == claim)).ToListAsync(),
                        IsCandidat = (besoin.UtilisateurId == null) && ((_db.Candidature.Where(c => c.BesoinsId == besoin.Id && c.UtilisateurId == claim && c.Statut == "Pending")).Count() > 0)
                    });
                }
                if (stop)
                {
                    projet.PresentationProjet = projet.PresentationProjet != null && projet.PresentationProjet.Length > (200 - "... (voir la suite)".Length) ? projet.PresentationProjet.Substring(0, (200 - "... (voir la suite)".Length)) + "... (voir la suite)" : projet.PresentationProjet;
                    var user = await _db.ApplicationUser.Where(u => u.Id == projet.UtilisateurId).FirstOrDefaultAsync();
                    ListItemProjet.Add(new ItemProjetViewModel
                    {
                        Projet = projet,
                        User = user,
                        Organisme = await _db.Organisme.Where(o => o.Id == user.OrganismeId).FirstOrDefaultAsync(),
                        TypeProjet = await _db.TypeProjet.Where(tp => tp.Id == projet.TypeProjetId).FirstOrDefaultAsync(),
                        ItemBesoin = ListItemBesoin
                    });
                }               
            }

            return ListItemProjet;
        }
    }
}
