using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Models;
using Neoma.Models.MembreProjetViewModel;
using Neoma.Models.CandidatViewModel;
using Microsoft.AspNetCore.Authorization;
using Neoma.Utility;
using Neoma.Services;
using Neoma.RazorClassLib.Services;
using Neoma.RazorClassLib.Views.Emails.NewSpecialite;

namespace Neoma.Controllers
{   
    public class RegisterController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;

        private readonly IEmailSender _emailSender;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

        public RegisterController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRazorViewToStringRenderer razorViewToStringRenderer) : base(db)
        {
            _userManager = userManager;
            _db = db;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }


        #region Affichage vue
            //Affichage de la vue de création de projet
            public async Task<IActionResult> IndexHolder(string userId)
            {
                if (userId == null)
                {
                    return RedirectToAction("Index", "Home", new { Area = "CommonUser" });
                }

                var user = await _userManager.FindByIdAsync(userId);


                Projet Projet = new Projet();

                Projet.UtilisateurId = userId;


                MembreProjetViewModel MembreProjet = new MembreProjetViewModel()
                {
                    Projet = Projet,
                    TypeProjet = await _db.TypeProjet.ToListAsync()
                };

                return View(MembreProjet);
            }

            //Affichage de la vue de création de profil
            public async Task<IActionResult> IndexSeeker(string userId)
            {
                
                if (userId == null)
                {
                    return RedirectToAction("Index", "Home", new { Area = "CommonUser" });
                }

                var user = await _userManager.FindByIdAsync(userId);


                CandidatViewModel Candidat = new CandidatViewModel()
                {
                    Id = userId
                };

                return View(Candidat);
            }

            //Mis à jour du tableau de membre lors de la suppression ou ajout d'une ligne
            public IActionResult TableMembreAdd()
            {
                return ViewComponent("TableMembre");
            }

            public IActionResult TableBesoinAdd()
            {
                return ViewComponent("TableBesoin");
            }
        #endregion

        #region Action Post
            [HttpPost]
            public async Task<JsonResult> ValidateProject([FromBody] MembreProjetViewModel model)
            {
                var specialiteCreate = new List<string>();
                if (ModelState.IsValid)
                {
                    model.Projet.Actif = true;
                    model.Projet.DateCreation = DateTime.Today;
                    _db.Add(model.Projet);
                
                    foreach (var item in model.Membre)
                    {
                        item.Projet = model.Projet;
                        _db.Membre.Add(item);

                        foreach (var specialite in item.Specialite)
                        {

                            if (!_db.Specialite.Any(s => s.Name == specialite.Name))
                            {
                                specialite.RoleId = item.RoleId;
                                specialite.Valide = false;
                                await _db.Specialite.AddAsync(specialite);
                                specialiteCreate.Add(specialite.Name);
                            }
                            else
                            {
                                if (specialite.Id == 0)
                                {
                                    specialite.Id = await _db.Specialite.Where(s => s.Name == specialite.Name).Select(i => i.Id).FirstOrDefaultAsync();
                                }
                            }
                            MembreSpecialite specialiteMembre = new MembreSpecialite
                            {
                                MembreId = item.Id,
                                SpecialiteId = specialite.Id
                            };
                            await _db.MembreSpecialite.AddAsync(specialiteMembre);
                        }
                    }

                    foreach (var item in model.Besoins)
                    {
                        item.Projet = model.Projet;
                        if(item.UtilisateurId == "")
                        {
                            item.UtilisateurId = null;
                        }
                        await _db.Besoins.AddAsync(item);

                        foreach (var besoin in item.Specialite)
                        {
                            if (!_db.Specialite.Any(s => s.Name == besoin.Name))
                            {
                                besoin.RoleId = item.RoleId;
                                besoin.Valide = false;
                                await _db.Specialite.AddAsync(besoin);
                                specialiteCreate.Add(besoin.Name);
                            }
                            else
                            {
                                if (besoin.Id == 0)
                                {
                                    besoin.Id = await _db.Specialite.Where(s => s.Name == besoin.Name).Select(i => i.Id).FirstOrDefaultAsync();
                                }
                            }
                            BesoinsSpecialite specialiteBesoin = new BesoinsSpecialite
                            {
                                BesoinsId = item.Id,
                                SpecialiteId = besoin.Id
                            };
                            await _db.BesoinsSpecialite.AddAsync(specialiteBesoin);
                        }
                    
                    }

                    ApplicationUser CreateurProjet = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == model.Projet.UtilisateurId);
                    CreateurProjet.EstPorteur = true;
                    CreateurProjet.EstAficionado = false;
                    /*HttpContext.Session.Set("Roles", new byte[5] {
                                                CreateurProjet.EstCandidat ? (byte)1 : (byte)0,
                                                CreateurProjet.EstPorteur ? (byte)1 : (byte)0,
                                                await _signInManager.UserManager.IsInRoleAsync(CreateurProjet, SD.AdminEndUser) ? (byte)1 : (byte)0,
                                                CreateurProjet.EstAficionado ? (byte)1 : (byte)0,
                                                await _signInManager.UserManager.IsInRoleAsync(CreateurProjet, SD.SuperEndUser) ? (byte)1 : (byte)0,
                                            });*/
                    CreateurProjet.RoleActuel = "Fondateur";
                    CreateurProjet.Valide = true;
                    try
                    {
                        await _db.SaveChangesAsync();
                    }
                    catch(Exception ex)
                    {
                        string message = ex.Message;
                    }
                    
                    if(specialiteCreate.Count > 0)
                    {
                        var UsersSuperAdmin = _userManager.GetUsersInRoleAsync(SD.SuperAdminEndUser).Result;
                        foreach(var user in UsersSuperAdmin)
                        {
                            var UrlPage = Url.Action("RedirectPage", "SharedAction", new { userId = user.Id, page = "Specialite" }, protocol: Request.Scheme);
                            var link = new NewSpecialiteEmailViewModel(UrlPage, specialiteCreate);
                            string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/NewSpecialite/NewSpecialiteEmail.cshtml", link);
                            await _emailSender.SendNewEmail(user.Email, "Validation nouvelle spécialité", body);
                        }

                        var UsersAdmin = _userManager.GetUsersInRoleAsync(SD.AdminEndUser).Result;
                        foreach (var user in UsersAdmin)
                        {
                            var UrlPage = Url.Action("RedirectPage", "SharedAction", new { userId = user.Id, page = "Specialite" }, protocol: Request.Scheme);
                            var link = new NewSpecialiteEmailViewModel(UrlPage, specialiteCreate);
                            string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/NewSpecialite/NewSpecialiteEmail.cshtml", link);
                            await _emailSender.SendNewEmail(user.Email, "Validation nouvelle spécialité", body);
                        }
                    }

                    //HttpContext.Session.SetString("Role", "Fondateur");
                    await _signInManager.SignInAsync(CreateurProjet, isPersistent: false);
                    return Json(new { success = true, result = "/Candidat" });
                    
                }
                else
                {
                    return Json(new { success = false, issue = model, errors = ModelState.Values.Where(i => i.Errors.Count > 0) });
                }

            }

            [HttpPost]
            public async Task<JsonResult> ValidateCandidat([FromBody] CandidatViewModel model)
            {
                var specialiteCreate = new List<string>();
                if (ModelState.IsValid)
                {
                    var user = _db.ApplicationUser.Where(u => u.Id == model.Id).FirstOrDefault();
                    user.Description = model.Description;
                    user.Valide = true;
                    _db.Update(user);
                

                    foreach (var item in model.Role)
                    {
                        item.UtilisateurId = user.Id;
                        await _db.RoleUtilisateur.AddAsync(item);

                        foreach (var specialite in item.Specialite)
                        {
                            if(!_db.Specialite.Any(s => s.Name == specialite.Name))
                            {
                                specialite.RoleId = item.RoleId;
                                specialite.Valide = false;
                                specialite.UtilisateurId = model.Id;
                                await _db.Specialite.AddAsync(specialite);
                                specialiteCreate.Add(specialite.Name);
                            }
                            else
                            {
                                if (specialite.Id == 0)
                                {
                                    specialite.Id = await _db.Specialite.Where(s => s.Name == specialite.Name).Select(i => i.Id).FirstOrDefaultAsync();
                                }
                            }

                            SpecialiteUtilisateur specialiteUser = new SpecialiteUtilisateur
                            {
                                UtilisateurId = user.Id,
                                SpecialiteId = specialite.Id
                            };
                            await _db.SpecialiteUtilisateur.AddAsync(specialiteUser);
                        }
                    }

                    if(specialiteCreate.Count > 0)
                    {
                        var UsersSuperAdmin = _userManager.GetUsersInRoleAsync(SD.SuperAdminEndUser).Result;
                        foreach(var userAdmin in UsersSuperAdmin)
                        {
                            var UrlPage = Url.Action("RedirectPage", "SharedAction", new { userId = userAdmin.Id, page = "Specialite" }, protocol: Request.Scheme);
                            var link = new NewSpecialiteEmailViewModel(UrlPage, specialiteCreate);
                            string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/NewSpecialite/NewSpecialiteEmail.cshtml", link);
                            await _emailSender.SendNewEmail(userAdmin.Email, "Validation nouvelle spécialité", body);
                        }

                        var UsersAdmin = _userManager.GetUsersInRoleAsync(SD.AdminEndUser).Result;
                        foreach (var userAdmin in UsersAdmin)
                        {
                            var UrlPage = Url.Action("RedirectPage", "SharedAction", new { userId = userAdmin.Id, page = "Specialite" }, protocol: Request.Scheme);
                            var link = new NewSpecialiteEmailViewModel(UrlPage, specialiteCreate);
                            string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/NewSpecialite/NewSpecialiteEmail.cshtml", link);
                            await _emailSender.SendNewEmail(userAdmin.Email, "Validation nouvelle spécialité", body);
                        }
                    }
                    user.RoleActuel = "Co-surfeur";
                    await _db.SaveChangesAsync();
                    /*HttpContext.Session.Set("Roles", new byte[5] {
                                                user.EstCandidat ? (byte)1 : (byte)0,
                                                user.EstPorteur ? (byte)1 : (byte)0,
                                                await _signInManager.UserManager.IsInRoleAsync(user, SD.AdminEndUser) ? (byte)1 : (byte)0,
                                                user.EstAficionado ? (byte)1 : (byte)0,
                                                await _signInManager.UserManager.IsInRoleAsync(user, SD.SuperEndUser) ? (byte)1 : (byte)0,
                                            });
                    
                    HttpContext.Session.SetString("Role", "Co-surfeur");*/
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return Json(new { success = true, result = "/ListeProjets" });
                }
                else
                {
                    return Json(new { success = false, issue = model, errors = ModelState.Values.Where(i => i.Errors.Count > 0) });
                }
            }
        #endregion
    }
}