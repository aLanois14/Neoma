using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Neoma.Data;
using Neoma.Models;
using Neoma.Utility;

namespace Neoma.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Id { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
            }

            Input = new InputModel
            {
                Id = userId
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(Input.Id);
            await _signInManager.SignInAsync(user, isPersistent: false);
            if (await _signInManager.UserManager.IsInRoleAsync(user, SD.AdminEndUser))
            {
                /*HttpContext.Session.Set("Roles", new byte[5] {
                                                user.EstCandidat ? (byte)1 : (byte)0,
                                                user.EstPorteur ? (byte)1 : (byte)0,
                                                await _signInManager.UserManager.IsInRoleAsync(user, SD.AdminEndUser) ? (byte)1 : (byte)0,
                                                user.EstAficionado ? (byte)1 : (byte)0,
                                                await _signInManager.UserManager.IsInRoleAsync(user, SD.SuperEndUser) ? (byte)1 : (byte)0,
                                            });*/
                user.RoleActuel = "Administrateur";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Utilisateur", new { Area = "Admin"});
            }
            if (user.EstCandidat)
            {
                user.RoleActuel = "Co-surfeur";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("IndexSeeker", "Register", new { Area = "", userId = user.Id });
            }
            else if (user.EstPorteur)
            {
                user.RoleActuel = "Fondateur";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("IndexHolder", "Register", new { Area = "", userId = user.Id });
            }
            else
            {

                user.RoleActuel = "Swimmer";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Aficionado", new { Area = "" });
            }           
        }
    }
}
