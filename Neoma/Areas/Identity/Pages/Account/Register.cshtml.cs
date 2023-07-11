using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Neoma.Data;
using Neoma.Extensions;
using Neoma.Models;
using Neoma.RazorClassLib.Services;
using Neoma.RazorClassLib.Views.Emails.ConfirmAccount;
using Neoma.Services;
using Neoma.Utility;
using static System.Net.Mime.MediaTypeNames;

namespace Neoma.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        public List<Organisme> _organisme;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, 
            ApplicationDbContext db, 
            RoleManager<IdentityRole> roleManager,
            IHostingEnvironment hostingEnvironment,
            IRazorViewToStringRenderer razorViewToStringRenderer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _db = db;
            _roleManager = roleManager;
            _organisme = _db.Organisme.Where(o => o.Valide == true).ToList();
            _hostingEnvironment = hostingEnvironment;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Veuillez saisir une adresse mail.")]
            [EmailAddress(ErrorMessage = "Le format de saisie est incorrect.")]
            [Display(Name = "Adresse Mail")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Veuillez saisir un mot de passe.")]
            [StringLength(100, ErrorMessage = "Le mot de passe doit faire au moins 6 caractères.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "Le mot de passe doit faire au moins 6 caractères.", MinimumLength = 6)]
            [Display(Name = "Confirmez le mot de passe")]
            [Compare("Password", ErrorMessage = "Les deux mots de passes ne correspondent pas.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Veuillez saisir votre prénom.")]
            [Display(Name ="Prénom")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Veuillez saisir votre nom.")]
            [Display(Name = "Nom")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Veuillez saisir votre N° de téléphone.")]
            [Display(Name = "Téléphone portable")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Statut")]
            public string Status { get; set; }

            public string Photo { get; set; }

            [Required]
            [Display(Name ="Organisme")]
            public int Campus { get; set; }         
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var user = new ApplicationUser {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Prenom = Input.FirstName,
                    Nom = Input.LastName,
                    PhoneNumber = Input.PhoneNumber,
                    OrganismeId = Input.Campus,
                    Valide = false
                };

                string Status = string.Empty;
                switch (Input.Status)
                {
                    case "0":
                        user.EstPorteur = true;
                        user.EstCandidat = false;
                        user.EstAficionado = false;
                        break;
                    case "1":
                        user.EstPorteur = false;
                        user.EstCandidat = true;
                        user.EstAficionado = false;
                        break;
                    case "2":
                        user.EstPorteur = false;
                        user.EstCandidat = false;
                        user.EstAficionado = true;
                        break;
                }

                if(Input.Photo != null)
                {
                    var files = Regex.Match(Input.Photo, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

                    if (files != null && files.Length > 0)
                    {
                        var binData = Convert.FromBase64String(files);

                        user.Photo = binData;
                    }
                }
                else
                {
                    var file = Path.Combine(webRootPath, @"images\" + SD.DefaultAvatarImage);
                    var bin = System.IO.File.ReadAllBytes(file);
                    user.Photo = bin;
                }
                              

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    #region Création des différents rôle
                        if (!await _roleManager.RoleExistsAsync(SD.AdminEndUser))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
                        }
                        if (!await _roleManager.RoleExistsAsync(SD.CommonEndUser))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(SD.CommonEndUser));
                        }
                        if (!await _roleManager.RoleExistsAsync(SD.SuperEndUser))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(SD.SuperEndUser));
                        }
                        if (!await _roleManager.RoleExistsAsync(SD.SuperAdminEndUser))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(SD.SuperAdminEndUser));
                        }
                    #endregion

                    await _userManager.AddToRoleAsync(user, SD.CommonEndUser);

                    _logger.LogInformation("User created a new account with password.");

                    #region Génération du mail de confirmation
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        var link = Url.Page("/Account/ConfirmEmail", pageHandler: null,values: new { userId = user.Id, code = code }, protocol: Request.Scheme);

                        var confirmAccountModel = new ConfirmAccountEmailViewModel(link);
                        //await _emailSender.SendEmailConfirmationAsync(Input.Email, link);
                        string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/ConfirmAccount/ConfirmAccountEmail.cshtml", confirmAccountModel);
                        await _emailSender.SendEmailConfirmationAsync(Input.Email, body);
                    #endregion


                    return RedirectToPage("./RegisterConfirmation");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
