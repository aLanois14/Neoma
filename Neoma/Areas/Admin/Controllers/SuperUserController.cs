using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neoma.Data;
using Neoma.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Neoma.Services;
using Neoma.Extensions;
using Neoma.Models.ApplicationUsersViewModel;
using Neoma.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Neoma.RazorClassLib.Services;
using Neoma.RazorClassLib.Views.Emails.SuperUser;

namespace Neoma.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    [Area("Admin")]
    public class SuperUserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

        public SuperUserController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IEmailSender emailSender,IHostingEnvironment hostingEnvironment, IRazorViewToStringRenderer razorViewToStringRenderer)
        {
            _db = db;
            _emailSender = emailSender;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        public async Task<IActionResult> Index()
        {
            var roleId = await _db.Roles.Where(x => x.Name == SD.SuperEndUser).Select(x => x.Id).FirstOrDefaultAsync();
            var usersId = await _db.UserRoles.Where(r => r.RoleId == roleId).Select(r => r.UserId).ToListAsync();
            var users = await _db.Users.Where(u => usersId.Contains(u.Id)).ToListAsync();
            foreach(var user in users)
            {
                user.Organisme = await _db.Organisme.Where(c => c.Id == user.OrganismeId).SingleOrDefaultAsync();
            }
            return View(users);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Organisme = _db.Organisme.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser User)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                if (User.PhotoStr != null)
                {
                    var files = Regex.Match(User.PhotoStr, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

                    if (files != null && files.Length > 0)
                    {
                        var binData = Convert.FromBase64String(files);

                        User.Photo = binData;
                    }
                }
                else
                {
                    var file = Path.Combine(webRootPath, @"images\" + SD.DefaultAvatarImage);
                    var bin = System.IO.File.ReadAllBytes(file);
                    User.Photo = bin;
                }
                User.UserName = User.Email;
                User.EstPorteur = false;
                User.EstCandidat = false;
                User.EstAficionado = false;
                User.Valide = true;

                await _userManager.CreateAsync(User, "Azerty@2018");
                await _userManager.AddToRoleAsync(User, SD.SuperEndUser);

                var user = await _userManager.FindByEmailAsync(User.Email);

                var code = await _userManager.GeneratePasswordResetTokenAsync(User);

                var link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { Area = "Identity", code = code },
                    protocol: Request.Scheme);

                var SuperUserEmail = new SuperUserEmailViewModel(link);
                string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/SuperUser/SuperUserEmail.cshtml", SuperUserEmail);
                await _emailSender.SendNewEmail(User.Email, "Compte super utilisateur", body);

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Create));
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
    }
}