using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Extensions;
using Neoma.Models;
using Neoma.Utility;
using Neoma.Models.ApplicationUsersViewModel;
using System.Collections.Generic;
using System;

namespace Neoma.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    [Area("Admin")]
    public class UtilisateurController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UtilisateurController(ApplicationDbContext db, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        #region Affichage vue
        public async Task<IActionResult> Index()
        {
            ApplicationUsersViewModel model = new ApplicationUsersViewModel();
            model.Utilisateurs = await _db.ApplicationUser.ToListAsync();

            var IdRoleAdmin = await _db.Roles.Where(r => r.Name == SD.AdminEndUser).Select(r => r.Id).FirstOrDefaultAsync();
            var IdRoleSuperAdmin = await _db.Roles.Where(r => r.Name == SD.SuperAdminEndUser).Select(r => r.Id).FirstOrDefaultAsync();

            foreach (ApplicationUser CetUtilisateur in model.Utilisateurs)
            {
                CetUtilisateur.EstAdmin = await _db.UserRoles.Where(ur => ur.RoleId == IdRoleAdmin && ur.UserId == CetUtilisateur.Id).FirstOrDefaultAsync() != null;
                CetUtilisateur.EstSuperAdmin = await _db.UserRoles.Where(ur => ur.RoleId == IdRoleSuperAdmin && ur.UserId == CetUtilisateur.Id).FirstOrDefaultAsync() != null;
                CetUtilisateur.CanDelete = !await _db.Projet.AnyAsync<Projet>(p => p.UtilisateurId == CetUtilisateur.Id) && !await _db.Besoins.AnyAsync<Besoins>(b => b.UtilisateurId == CetUtilisateur.Id) ? true : false;
            }

            var UserCurrent = await _db.Users.Where(u => u.Id == User.getUserId()).FirstOrDefaultAsync();
            UserCurrent.Valide = true;
            _db.Update(UserCurrent);
            await _db.SaveChangesAsync();
            model.Organismes = await _db.Organisme.ToListAsync();

            return View(model);
        }
        #endregion

        #region Action Post

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody]ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var User = await _db.Users.Where(u => u.Id == model.Id).FirstOrDefaultAsync();
                User.Nom = model.Nom;
                User.Prenom = model.Prenom;
                User.Email = model.Email;
                User.OrganismeId = model.OrganismeId;
                User.Valide = model.Valide;
                if(!User.EmailConfirmed && model.Valide)
                {
                    User.EmailConfirmed = true;
                }
                _db.Update(User);
                await _db.SaveChangesAsync();

                var RoleAdmin = await _db.Roles.Where(r => r.Name == SD.AdminEndUser).FirstOrDefaultAsync();
                var RoleSuperAdmin = await _db.Roles.Where(r => r.Name == SD.SuperAdminEndUser).FirstOrDefaultAsync();

                if (!await _roleManager.RoleExistsAsync(SD.AdminEndUser))
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
                }
                if (!await _roleManager.RoleExistsAsync(SD.SuperAdminEndUser))
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.SuperAdminEndUser));
                }
                User.EstAdmin = await _db.UserRoles.Where(ur => ur.RoleId == RoleAdmin.Id && ur.UserId == User.Id).FirstOrDefaultAsync() != null;
                User.EstSuperAdmin = await _db.UserRoles.Where(ur => ur.RoleId == RoleSuperAdmin.Id && ur.UserId == User.Id).FirstOrDefaultAsync() != null;

                if (model.EstAdmin && !User.EstAdmin)
                {
                    await _userManager.AddToRoleAsync(User, SD.AdminEndUser);
                }
                else if(!model.EstAdmin && User.EstAdmin)
                {
                    await _userManager.RemoveFromRoleAsync(User, SD.AdminEndUser);
                }

                if (model.EstSuperAdmin && !User.EstSuperAdmin)
                {
                    await _userManager.AddToRoleAsync(User, SD.SuperAdminEndUser);
                }
                else if (!model.EstSuperAdmin && User.EstSuperAdmin)
                {
                    await _userManager.RemoveFromRoleAsync(User, SD.SuperAdminEndUser);
                }

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var utilisateur = await _db.ApplicationUser.SingleOrDefaultAsync(m => m.Id == id);
                _db.Selection.RemoveRange(_db.Selection.Where(s => s.UtilisateurId == utilisateur.Id));
                _db.Candidature.RemoveRange(_db.Candidature.Where(c => c.UtilisateurId == utilisateur.Id));
                var Specs = _db.Specialite.Where(s => s.UtilisateurId == id);
                foreach (Specialite CetteSpec in Specs)
                    CetteSpec.UtilisateurId = null;
                var BesoinsUtilisateur = _db.Besoins.Where(s => s.UtilisateurId == id);
                foreach (Besoins CeBesoin in BesoinsUtilisateur)
                    CeBesoin.UtilisateurId = null;
                var Propositions = _db.Proposition.Where(s => s.UtilisateurId == id);
                _db.Proposition.RemoveRange(Propositions);
                _db.ApplicationUser.Remove(utilisateur);
                await _db.SaveChangesAsync();
                return Content("OK"); //RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content("Erreur : " + ex.Message);
            }
        }
        #endregion
    }
}