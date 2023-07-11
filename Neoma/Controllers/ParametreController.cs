using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Extensions;
using Neoma.Models;
using Neoma.Models.AccountViewModel;
using Neoma.Models.CandidatViewModel;
using Neoma.RazorClassLib.Services;
using Neoma.RazorClassLib.Views.Emails.NewSpecialite;
using Neoma.Services;
using Neoma.Utility;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.CommonEndUser + "," + SD.AdminEndUser + "," + SD.SuperEndUser + "," + SD.SuperAdminEndUser)]
    public class ParametreController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

        public ParametreController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db, IEmailSender emailSender, IRazorViewToStringRenderer razorViewToStringRenderer) : base(db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _emailSender = emailSender;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public async Task<IActionResult> Index()
        {
            var user = User.getUserId();

            var userInfo = _db.Users.Where(u => u.Id == user).FirstOrDefault();
            userInfo.Organisme = await _db.Organisme.Where(o => o.Id == userInfo.OrganismeId).FirstOrDefaultAsync();

            userInfo.OrganismeId = userInfo.Organisme != null ? userInfo.Organisme.Id : 0;

            var SpecUser = await _db.SpecialiteUtilisateur.Where(su => su.UtilisateurId == userInfo.Id).Select(x => x.SpecialiteId).ToListAsync();
            var RoleUser = await _db.RoleUtilisateur.Where(ru => ru.UtilisateurId == userInfo.Id).Select(x => x.RoleId).ToListAsync();
            var parameterViewModel = new ParameterViewModel
            {
                User = userInfo,
                Email = userInfo.Email,
                Phone = userInfo.PhoneNumber,
                Specialite = await _db.Specialite.Where(s => SpecUser.Contains(s.Id)).ToListAsync(),
                Role = await _db.Role.Where(r => RoleUser.Contains(r.Id)).ToListAsync(),
                Projet = await _db.Projet.Where(p => p.UtilisateurId == userInfo.Id).ToListAsync(),
                Organismes = await _db.Organisme.ToListAsync()
            };
            foreach (var projet in parameterViewModel.Projet)
            {
                projet.TypeProjet = await _db.TypeProjet.Where(tp => tp.Id == projet.TypeProjetId).FirstOrDefaultAsync();
                projet.Utilisateur = userInfo;
                projet.Besoins = await _db.Besoins.Where(b => b.ProjetId == projet.Id).ToListAsync();
                foreach (var besoin in projet.Besoins)
                {
                    besoin.Role = await _db.Role.Where(r => r.Id == besoin.RoleId).FirstOrDefaultAsync();
                }
            }

            var testSpecs = (from bs in _db.BesoinsSpecialite
                        join b in _db.Besoins on bs.BesoinsId equals b.Id
                        where b.UtilisateurId == userInfo.Id
                        select bs.SpecialiteId).Distinct().ToList();
            foreach (Specialite CetteSpecialite in parameterViewModel.Specialite)
            {
                CetteSpecialite.Supprimable = !testSpecs.Contains(CetteSpecialite.Id);
            }

            return View(parameterViewModel);
        }

        public IActionResult TableSpecialiteAdd()
        {
            return ViewComponent("TableSpecialiteUser");
        }

        public async Task<IActionResult> UpdateSpecialite(int role)
        {
            var SpecialiteUser = await _db.SpecialiteUtilisateur.Where(su => su.UtilisateurId == User.getUserId()).ToListAsync();
            var Specialite = await _db.Specialite.Where(s => s.RoleId == role && (s.Valide == true || s.UtilisateurId == User.getUserId()) && !SpecialiteUser.Exists(x => x.SpecialiteId == s.Id)).ToListAsync();
            return new JsonResult(Specialite);
        }

        [HttpPost]
        public async Task<JsonResult> addSpecialite([FromBody] CandidatViewModel model)
        {
            var specialiteCreate = new List<string>();
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _db.ApplicationUser.Where(u => u.Id == model.Id).FirstOrDefaultAsync();
                //user.Description = model.Description;
                //user.EstCandidat = true;
                user.EstAficionado = false;
                _db.Update(user);

                string[] IdsTexte = model.IdsDeleteSpecialites.Split("|", StringSplitOptions.RemoveEmptyEntries);
                if (IdsTexte.Length > 0)
                {
                    List<int> IdsSpecs = new List<int>();
                    foreach (string CetId in IdsTexte)
                        IdsSpecs.Add(Convert.ToInt32(CetId));

                    _db.SpecialiteUtilisateur.RemoveRange(_db.SpecialiteUtilisateur.Where(su => IdsSpecs.Contains(su.SpecialiteId) && su.UtilisateurId == user.Id));
                    await _db.SaveChangesAsync();
                }

                foreach (var item in model.Role)
                {
                    if (item.Specialite.Count > 0)
                    {
                        item.UtilisateurId = user.Id;
                        if (_db.RoleUtilisateur.Where(ru => ru.RoleId == item.RoleId && ru.UtilisateurId == user.Id).FirstOrDefault() == null)
                        {
                            await _db.RoleUtilisateur.AddAsync(item);
                        }

                        foreach (var specialite in item.Specialite)
                        {
                            if (!_db.Specialite.Any(s => s.Name == specialite.Name))
                            {
                                specialite.RoleId = item.RoleId;
                                specialite.Valide = false;
                                specialite.UtilisateurId = user.Id;
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
                        _db.SaveChanges();
                    }
                }

                var IdsRolesUtilisateurs = await _db.RoleUtilisateur.Where(ru => ru.UtilisateurId == user.Id).Select(ru => ru.RoleId).ToListAsync();
                var IdsSpecialitesUtilisateur = await _db.SpecialiteUtilisateur.Where(su => su.UtilisateurId == user.Id).Select(su => su.SpecialiteId).ToListAsync();
                foreach (int CetId in IdsRolesUtilisateurs)
                {
                    int CombienDeSpecialites = await _db.Specialite.Where(s => IdsSpecialitesUtilisateur.Contains(s.Id) && s.RoleId == CetId).CountAsync();
                    if (CombienDeSpecialites == 0)
                        _db.RoleUtilisateur.Remove(_db.RoleUtilisateur.Where<RoleUtilisateur>(ru => ru.RoleId == CetId && ru.UtilisateurId == user.Id).FirstOrDefault());
                }

                await _db.SaveChangesAsync();

                /*byte[] roles = HttpContext.Session.Get("Roles");
                roles[0] = 1;
                roles[3] = 0;
                HttpContext.Session.Set("Roles", roles);*/
                if (specialiteCreate.Count > 0)
                {
                    var UsersSuperAdmin = _userManager.GetUsersInRoleAsync(SD.SuperAdminEndUser).Result;
                    foreach (var userAdmin in UsersSuperAdmin)
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
                return Json(new { success = true, result = "/Parametre" });
            }
            else
            {
                return Json(new { success = false, issue = model, errors = ModelState.Values.Where(i => i.Errors.Count > 0) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditParameter(ParameterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var User = await _db.Users.Where(u => u.Id == model.User.Id).FirstOrDefaultAsync();
                User.Nom = model.User.Nom;
                User.Prenom = model.User.Prenom;
                User.Email = model.Email;
                User.PhoneNumber = model.Phone;
                User.Description = model.User.Description;
                User.EstCandidat = model.User.EstCandidat;
                User.OrganismeId = model.User.OrganismeId;

                byte[] roles = HttpContext.Session.Get("Roles");
                if (User.EstCandidat)
                {
                    //roles[0] = 1;
                    User.RoleActuel = "Co-surfeur";
                }
                else
                {
                    //roles[0] = 0;
                    if (User.EstPorteur)
                    {
                       User.RoleActuel = "Fondateur";
                    }
                }
                
                //HttpContext.Session.Set("Roles", roles);

                var files = Regex.Match(model.User.PhotoStr, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                if (files != null && files.Length > 0)
                {
                    var binData = Convert.FromBase64String(files);

                    User.Photo = binData;
                }
                _db.Users.Update(User);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}