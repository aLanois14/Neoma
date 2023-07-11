using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Models;
using Neoma.Utility;

namespace Neoma.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    [Area("Admin")]
    public class OrganismeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrganismeController(ApplicationDbContext db)
        {
            _db = db;
        }

        #region Affichage vue
        public async Task<IActionResult> Index()
        {
            var Organismes = await _db.Organisme.ToListAsync();
            foreach(var organisme in Organismes)
            {
                organisme.CanDelete = !await _db.ApplicationUser.AnyAsync<ApplicationUser>(u => u.OrganismeId == organisme.Id) ? true : false;
            }
            return View(Organismes);
        }
        #endregion

        #region Action Post
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]Organisme Organisme)
        {
            if (ModelState.IsValid)
            {
                _db.Add(Organisme);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody]Organisme Organisme)
        {
            if (ModelState.IsValid)
            {
                _db.Update(Organisme);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var organisme = await _db.Organisme.SingleOrDefaultAsync(m => m.Id == id);
                bool UtilisateursExistants = await _db.ApplicationUser.AnyAsync<ApplicationUser>(u => u.OrganismeId == id);
                if (!UtilisateursExistants)
                {
                    _db.Organisme.Remove(organisme);
                    await _db.SaveChangesAsync();
                    return Content("OK");
                }
                else
                    return Content("Cet organisme ne peut pas être supprimé, car un ou plusieurs utilisateurs sont liés à celui-ci.");
            }
            catch (Exception ex)
            {
                return Content("Erreur : " + ex.Message);
            }
        }
        #endregion
    }
}