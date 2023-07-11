using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neoma.Data;
using Microsoft.EntityFrameworkCore;
using Neoma.Models;
using Neoma.Utility;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RoleController(ApplicationDbContext db)
        {
            _db = db;
        }

        #region Affichage vue
        public async Task<IActionResult> Index()
        {
            var Roles = await _db.Role.ToListAsync();
            foreach (var role in Roles)
            {
                role.CanDelete = !await _db.Specialite.AnyAsync<Specialite>(s => s.RoleId == role.Id) ? true : false;
            }
            return View(Roles);
        }
        #endregion

        #region Action Post
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]Role Role)
        {
            if (ModelState.IsValid)
            {
                _db.Add(Role);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody]Role Role)
        {
            if (ModelState.IsValid)
            {
                _db.Update(Role);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var role = await _db.Role.SingleOrDefaultAsync(m => m.Id == id);
                bool BesoinsExistants = await _db.Besoins.AnyAsync<Besoins>(b => b.RoleId == id);
                bool MembresExistants = await _db.Membre.AnyAsync<Membre>(m => m.RoleId == id);
                if (!BesoinsExistants && !MembresExistants)
                {
                    _db.Role.Remove(role);
                    await _db.SaveChangesAsync();
                    return Content("OK");
                }
                else if (BesoinsExistants)
                    return Content("Ce rôle ne peut pas être supprimé, car un ou plusieurs besoins sont liés à celui-ci.");
                else
                    return Content("Ce rôle ne peut pas être supprimé, car un ou plusieurs membres de projets sont liés à celui-ci.");
            }
            catch (Exception ex)
            {
                return Content("Erreur : " + ex.Message);
            }
        }
        #endregion
    }
}