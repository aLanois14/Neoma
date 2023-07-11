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
    public class RedirectPageModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public RedirectPageModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db)
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
            public string Page { get; set; }
            public int Parameter1 { get; set; }
            public string Parameter2 { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string userId, string page, int parameter1 = 0, string parameter2 = null)
        {
            if(userId == null || page == null)
            {
                return RedirectToPage("/Index");
            }

            Input = new InputModel
            {
                Id = userId,
                Page = page,
                Parameter1 = parameter1,
                Parameter2 = parameter2
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(Input.Id);
            await _signInManager.SignInAsync(user, isPersistent: false);
            /*HttpContext.Session.Set("Roles", new byte[5] {
                                                user.EstCandidat ? (byte)1 : (byte)0,
                                                user.EstPorteur ? (byte)1 : (byte)0,
                                                await _signInManager.UserManager.IsInRoleAsync(user, SD.AdminEndUser) ? (byte)1 : (byte)0,
                                                user.EstAficionado ? (byte)1 : (byte)0,
                                                await _signInManager.UserManager.IsInRoleAsync(user, SD.SuperEndUser) ? (byte)1 : (byte)0,
                                            });*/

            if(Input.Page == "RefuseC")
            {
                user.RoleActuel = "Co-surfeur";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "CandidatureCandidat", new { Area = "" });
            }
            else if(Input.Page == "Accept" || Input.Page == "Validate")
            {
                user.RoleActuel = "Co-surfeur";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("DetailProjet", "CandidatureRetenue", new { Area = "", Id = Input.Parameter1 });
            }
            else if (Input.Page == "Leave" || Input.Page == "RefuseP")
            {
                user.RoleActuel = "Fondateur";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "MesProjets", new { Area = "" });
            }
            else if(Input.Page == "Specialite")
            {
                user.RoleActuel = "Administrateur";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Specialite", new { Area = "Admin" });
            }
            else if(Input.Page == "Postuler")
            {
                user.RoleActuel = "Fondateur";
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("DetailCandidature", "CandidaturePorteur", new { Area = "", user = Input.Parameter2, projet = Input.Parameter1 });
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
    }
}