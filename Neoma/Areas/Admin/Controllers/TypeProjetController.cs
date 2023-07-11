using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Models;
using Neoma.Utility;
using System;
using System.Threading.Tasks;

namespace Neoma.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    [Area("Admin")]
    public class TypeProjetController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TypeProjetController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var TypeProjets = await _db.TypeProjet.ToListAsync();
            foreach (var typeProjet in TypeProjets)
            {
                typeProjet.CanDelete = !await _db.Projet.AnyAsync<Projet>(p => p.TypeProjetId == typeProjet.Id) ? true : false;
            }
            return View(TypeProjets);
        }

        #region Action Post
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]TypeProjet TypeProjet)
        {
            if (ModelState.IsValid)
            {
                TypeProjet.Valide = true;
                _db.Add(TypeProjet);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody]TypeProjet TypeProjet)
        {
            if (ModelState.IsValid)
            {
                _db.Update(TypeProjet);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var typeprojet = await _db.TypeProjet.SingleOrDefaultAsync(m => m.Id == id);
                bool ProjetsExistants = await _db.Projet.AnyAsync<Projet>(p => p.TypeProjetId == id);
                if (!ProjetsExistants)
                {
                    _db.TypeProjet.Remove(typeprojet);
                    await _db.SaveChangesAsync();
                    return Content("OK");
                }
                else
                    return Content("Ce type de projet ne peut pas être supprimé, car un ou plusieurs projets sont liés à celui-ci.");
            }
            catch (Exception ex)
            {
                return Content("Erreur : " + ex.Message);
            }
        }
        #endregion
    }
}