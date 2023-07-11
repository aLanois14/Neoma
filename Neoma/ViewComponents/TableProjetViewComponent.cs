using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Models;
using Neoma.Models.ProjetViewModel;
using Neoma.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Neoma.ViewComponents
{
    public class TableProjetViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public TableProjetViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(SelectedFilterProject Select = null)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<ItemProjetViewModel> ListItemProjet = new List<ItemProjetViewModel>();

            if (User.IsInRole(SD.SuperEndUser))
            {
                if (Select == null)
                {
                    Select = new SelectedFilterProject();
                    Select.Specialite = new List<Specialite>();
                }

                Select.Organisme = await _db.ApplicationUser.Where(u => u.Id == claim).Select(u => u.OrganismeId).FirstOrDefaultAsync();
            }

            if (Select == null)
            {
                List<Projet> listProjet = await _db.Projet.Where(p => p.Actif == true && p.Complet == false && p.Termine == false && p.UtilisateurId != claim).ToListAsync();
                ListItemProjet = await formatProjet(listProjet);

            }
            else
            {
                if(Select.Organisme == 0 && Select.Role == 0 && Select.TypeProjet == 0)
                {
                    List<Projet> listProjet = await _db.Projet.Where(p => p.Actif == true && p.Complet == false && p.Termine == false && p.UtilisateurId != claim).ToListAsync();
                    ListItemProjet = await formatProjet(listProjet);
                }
                else
                {
                    //Récupération de tous les utilisateurs appartenant à l'organisme choisi
                    var User = Select.Organisme != 0 ? 
                        await _db.Users.Where(u => u.OrganismeId == Select.Organisme && u.Id != claim).ToListAsync() : 
                        await _db.Users.Where(u => u.Id != claim).ToListAsync();
                    
                    //Récupération des projets des utilisateurs sélectionnés
                    var ListProjet = await _db.Projet.Where(p => User.Exists(x => x.Id == p.UtilisateurId) && p.Actif == true && p.Complet == false && p.Termine == false).ToListAsync();

                    List<int> IdSpecialites = new List<int>();
                    foreach (Specialite Spec in Select.Specialite)
                        IdSpecialites.Add(Spec.Id);

                    var listBesoins = IdSpecialites.Count > 0 ? await _db.BesoinsSpecialite.Where(s => IdSpecialites.Contains(s.SpecialiteId)).Select(x => x.BesoinsId).ToListAsync() : null;
                    if (listBesoins != null)
                        listBesoins = listBesoins.FindAll(lb => listBesoins.FindAll(i => i == lb).Count == IdSpecialites.Count);

                    //Récupération des besoins en fonction du role sélectionné et des projets
                    var Besoins = Select.Role != 0 && Select.Specialite.Count == 0 ? 
                        await _db.Besoins.Where(b => b.RoleId == Select.Role && ListProjet.Exists(x => x.Id == b.ProjetId)).ToListAsync() :
                        Select.Specialite.Count > 0 ?
                        await _db.Besoins.Where(b => b.RoleId == Select.Role && listBesoins.Contains(b.Id) && ListProjet.Exists(x => x.Id == b.ProjetId)).ToListAsync() :
                        await _db.Besoins.Where(b => ListProjet.Exists(x => x.Id == b.ProjetId)).ToListAsync();

                    //Récupération des projets répondant aux critères 
                    var Projets = Select.TypeProjet == 0 ? 
                        await _db.Projet.Where(p => User.Exists(u => u.Id == p.UtilisateurId) && Besoins.Exists(b => b.ProjetId == p.Id)).ToListAsync() : 
                        await _db.Projet.Where(p => User.Exists(u => u.Id == p.UtilisateurId) && Besoins.Exists(b => b.ProjetId == p.Id) && p.TypeProjetId == Select.TypeProjet).ToListAsync();


                    ListItemProjet = await formatProjet(Projets);

                    ListItemProjet = ListItemProjet.Distinct().ToList();
                }
            }

            ViewBag.TestFilter = Select != null ? Select.TexteRecherche : null;

            return View(ListItemProjet);
        }

        public async Task<List<ItemProjetViewModel>> formatProjet(List<Projet> listProjet)
        {
            List<ItemProjetViewModel> ListItemProjet = new List<ItemProjetViewModel>();
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            foreach (var projet in listProjet)
            {
                var Besoin = await _db.Besoins.Where(b => b.ProjetId == projet.Id).ToListAsync();

                List<ItemBesoinViewModel> ListItemBesoin = new List<ItemBesoinViewModel>();
                foreach (var besoin in Besoin)
                {
                    var listSpec = await _db.BesoinsSpecialite.Where(s => s.Besoins == besoin).Select(x => x.SpecialiteId).ToListAsync();
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
            return ListItemProjet;
        }
    }
}
