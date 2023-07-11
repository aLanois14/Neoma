using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Neoma.Models;
using Neoma.Models.AccountViewModel;
using Neoma.Data;
using Neoma.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.EntityFrameworkCore;
using Neoma.Extensions;

namespace Neoma.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public string returnUrl { get; set; }

        public HomeController(ILogger<LoginModel> logger, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db) : base(db)
        {
            _db = db;
            _signInManager = signInManager;
            _logger = logger;
        }

        #region Affichage vue
            public IActionResult Index()
            {
                return View();
            }

            public async Task<IActionResult> Login(string index)
            {
                LoginViewModel login = new LoginViewModel();
                login.Statut = new List<string>();
                login.Mail = index;
                var user = await _db.Users.Where(u => u.Email == index).FirstOrDefaultAsync();
                var roleAdmin = await _db.Roles.Where(r => r.Name == SD.AdminEndUser).FirstOrDefaultAsync();
                var roleSuperUser = await _db.Roles.Where(r => r.Name == SD.SuperEndUser).FirstOrDefaultAsync();

                var userRoleAdmin = await _db.UserRoles.Where(ur => ur.RoleId == roleAdmin.Id && ur.UserId == user.Id).FirstOrDefaultAsync();                
                var userRoleSuperUser = await _db.UserRoles.Where(ur => ur.RoleId == roleSuperUser.Id && ur.UserId == user.Id).FirstOrDefaultAsync();  

                if (await _db.Users.Where(u => u.Email == index).Select(x => x.EstCandidat).FirstOrDefaultAsync())
                {
                    login.Statut.Add("Co-surfeur");
                }
                if (await _db.Users.Where(u => u.Email == index).Select(x => x.EstPorteur).FirstOrDefaultAsync())
                {
                    login.Statut.Add("Fondateur");
                }
                if (await _db.Users.Where(u => u.Email == index).Select(x => x.EstAficionado).FirstOrDefaultAsync())
                {
                    login.Statut.Add("Swimmer");
                }
                if (userRoleSuperUser != null)
                {
                    login.Statut.Add("Super utilisateur");
                }
                if (userRoleAdmin != null)
                {
                    login.Statut.Add("Administrateur");
                }

                return View(login);
            }

            public async Task<IActionResult> ChangeRole(string role)
            {
                ApplicationUser currentUser = await _db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefaultAsync();

                if (currentUser != null)
                {
                    //InitializeCount(role, currentUser);
                    if (role == "Co-surfeur" && currentUser.EstCandidat)
                    {
                        currentUser.RoleActuel = role;
                        await _db.SaveChangesAsync();
                        return RedirectToAction("Index", "ListeProjets");
                    }
                    else if (role == "Fondateur" && currentUser.EstPorteur)
                    {
                        currentUser.RoleActuel = role;
                        await _db.SaveChangesAsync();
                        return RedirectToAction("Index", "Candidat");
                    }
                    else if(role == "Swimmer" && currentUser.EstAficionado)
                    {
                        currentUser.RoleActuel = role;
                        await _db.SaveChangesAsync();
                        return RedirectToAction("Index", "Aficionado");
                    }
                    else if (role == "Administrateur" && User.IsInRole(SD.AdminEndUser))
                    {
                        currentUser.RoleActuel = role;
                        await _db.SaveChangesAsync();
                        if (!User.IsInRole(SD.SuperEndUser))
                            return RedirectToAction("Index", "Utilisateur", new { Area = "Admin" });
                        else
                            return RedirectToAction("Index", "SuperUser", new { Area = "Admin" });
                    }
                    else if (role == "Super utilisateur" && User.IsInRole(SD.SuperEndUser))
                    {
                        currentUser.RoleActuel = role;
                        await _db.SaveChangesAsync();
                        return RedirectToAction("Index", "ListeProjets");
                    }
                }
                
                if (currentUser.RoleActuel == "Candidat")
                    return RedirectToAction("Index", "ListeProjets");
                else if (currentUser.RoleActuel == "Porteur")
                    return RedirectToAction("Index", "Candidat");
                else
                    return RedirectToAction("Index", "Utilisateur", new { Area = "Admin" });
            }
        #endregion

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(login.Mail, login.Password, true, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    ApplicationUser currentUser = await _signInManager.UserManager.Users.FirstOrDefaultAsync(u => u.UserName == login.Mail);
                    /*if (currentUser != null)
                        HttpContext.Session.Set("Roles", new byte[5] {
                            currentUser.EstCandidat ? (byte)1 : (byte)0,
                            currentUser.EstPorteur ? (byte)1 : (byte)0,
                            await _signInManager.UserManager.IsInRoleAsync(currentUser, SD.AdminEndUser) ? (byte)1 : (byte)0,
                            currentUser.EstAficionado ? (byte)1 : (byte)0,
                            await _signInManager.UserManager.IsInRoleAsync(currentUser, SD.SuperEndUser) ? (byte)1 : (byte)0,
                        });*/
                    _logger.LogInformation("User logged in.");
                    currentUser.RoleActuel = login.SelectedStatut;
                    await _db.SaveChangesAsync();
                    if (login.SelectedStatut == "Co-surfeur")
                    {
                        return RedirectToAction("Index", "ListeProjets");
                    }
                    else if(login.SelectedStatut == "Fondateur")
                    {
                        return RedirectToAction("Index", "Candidat");
                    }
                    else if(login.SelectedStatut == "Swimmer")
                    {
                        return RedirectToAction("Index", "Aficionado");
                    }
                    else if (login.SelectedStatut == "Super utilisateur")
                    {
                        return RedirectToAction("Index", "ListeProjets");
                    }
                    else if(login.SelectedStatut == "Administrateur")
                    {
                        return RedirectToAction("Index", "Utilisateur", new { Area = "Admin" });
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("/Account/LoginWith2fa", new { Area = "Identity", ReturnUrl = returnUrl, RememberMe = false });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("/Account/Lockout", new { Area = "Identity" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Le mot de passe saisi est incorrect.");
                    return View(login);
                }
            }
            return RedirectToAction("Login", new { index = login.Mail });
        }

        public IActionResult TableBesoinAdd()
        {
            return ViewComponent("TableBesoin");
        }        

        public async Task<IActionResult> InitializeCount()
        {
            ApplicationUser currentUser = await _signInManager.UserManager.Users.FirstOrDefaultAsync(u => u.Id == User.getUserId());
            var role = currentUser.RoleActuel;
            int Count1 = 0;
            int Count2 = 0;
            if (role == "Fondateur")
            {
                var Projets = await _db.Projet.Where(p => p.UtilisateurId == User.getUserId() && !p.Complet).Select(x => x.Id).ToListAsync();

                foreach (var projet in Projets)
                {
                    var besoins = await _db.Besoins.Where(b => b.ProjetId == projet).Select(x => x.Id).ToListAsync();
                    foreach (var besoin in besoins)
                    {
                        var Candidatures = await _db.Candidature.Where(c => c.BesoinsId == besoin && c.Statut.ToString() == "Pending").ToListAsync();
                        Count1 += Candidatures.Count;

                    }
                    var Selections = await _db.Selection.Where(s => s.ProjetId == projet).ToListAsync();
                    Count2 += Selections.Count;

                }
            }
            else
            {
                var Candidatures = await _db.Candidature.Where(c => c.UtilisateurId == User.getUserId()).ToListAsync();
                foreach (var candidature in Candidatures)
                {
                    if (candidature.Statut.ToString() == "Accepted")
                    {
                        Count1 += 1;
                    }
                    else
                    {
                        Count2 += 1;
                    }
                }
                var Propositions = await _db.Proposition.Where(p => p.UtilisateurId == User.getUserId()).ToListAsync();
                foreach (var proposition in Propositions)
                {
                    if (proposition.Statut.ToString() == "Accepted")
                    {
                        Count1 += 1;
                    }
                }
            }

            return Ok(new { Count1 = Count1, Count2 = Count2, Role = role });
        }

        public async Task<IActionResult> GetNbMessage()
        {
            var userId = User.getUserId();
            var conversationsId = await _db.ConversationUtilisateur.Where(cu => cu.UtilisateurId == userId).Select(x => x.ConversationId).ToListAsync();
            var conversations = await _db.Conversation.Where(c => conversationsId.Contains(c.Id)).ToListAsync();
            int count = 0;
            foreach(var conversation in conversations)
            {
                var message = await _db.Message.Where(m => !m.MessageLu && m.UtilisateurId != userId && m.ConversationId == conversation.Id).ToListAsync();
                if (message.Count > 0)
                {
                    count++;
                }
            }
            return Ok(new { Count = count });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
