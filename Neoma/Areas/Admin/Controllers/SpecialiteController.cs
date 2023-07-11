using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Models;
using Neoma.Models.SpecialiteViewModel;
using Neoma.Utility;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    [Area("Admin")]
    public class SpecialiteController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SpecialiteController(ApplicationDbContext db)
        {
            _db = db;
        }

        #region Affichage vue
        public async Task<IActionResult> Index()
        {
            List<Role> Role = new List<Role>();
            Role.Add(new Role
            {
                Id = 0,
                Name = "Toutes les spécialités"
            });

            foreach(var role in await _db.Role.ToListAsync())
            {
                Role.Add(role);
            }
            SpecialiteRoleViewModel SpecialiteRole = new SpecialiteRoleViewModel()
            {
                RoleList = Role,
                Role = new Role()
            };
            return View(SpecialiteRole);
        }

        public IActionResult Table(int Role)
        {
            return ViewComponent("TableSpecialite", new { RoleId = Role });
        }
        #endregion

        #region Action Post
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]Specialite Specialite)
        {
            if (ModelState.IsValid)
            {
                //Specialite.Valide = true;
                _db.Add(Specialite);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody]Specialite Specialite)
        {
            if (ModelState.IsValid)
            {
                _db.Update(Specialite);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
      
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Specialite = await _db.Specialite.SingleOrDefaultAsync(m => m.Id == id);
                var BesoinsSpec = await _db.BesoinsSpecialite.AnyAsync<BesoinsSpecialite>(bs => bs.SpecialiteId == id);
                var MembreSpec = await _db.MembreSpecialite.AnyAsync<MembreSpecialite>(ms => ms.SpecialiteId == id);
                var UtilisateurSpec = await _db.SpecialiteUtilisateur.AnyAsync<SpecialiteUtilisateur>(su => su.SpecialiteId == id);
                if (!BesoinsSpec && !MembreSpec && !UtilisateurSpec)
                {
                    //_db.BesoinsSpecialite.RemoveRange(_db.BesoinsSpecialite.Where(bs => bs.SpecialiteId == Specialite.Id));
                    //_db.MembreSpecialite.RemoveRange(_db.MembreSpecialite.Where(ms => ms.SpecialiteId == Specialite.Id));
                    _db.Specialite.Remove(Specialite);
                    await _db.SaveChangesAsync();
                }
                else if (BesoinsSpec)
                    return Content("Cette spécialité ne peut pas être supprimée, car un ou plusieurs besoins sont liés à celle-ci.");
                else if (MembreSpec)
                    return Content("Cette spécialité ne peut pas être supprimée, car un ou plusieurs membres de projets sont liés à celle-ci.");
                else
                    return Content("Cette spécialité ne peut pas être supprimée, car un ou plusieurs utilisateurs sont liés à celle-ci.");
                return Content("OK");
            }
            catch (Exception ex)
            {
                return Content("Erreur : " + ex.Message);
            }
        }
        #endregion
    }
}