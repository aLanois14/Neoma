using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neoma.Data;
using Neoma.Extensions;
using Neoma.Models;
using Neoma.Models.CandidatViewModel;
using Neoma.Utility;

namespace Neoma.Controllers
{
    [Authorize(Roles = SD.CommonEndUser + "," + SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    public class AficionadoController : BaseController
    {
        private readonly ApplicationDbContext _db;

        public AficionadoController(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var Users = _db.Users.Where(u => u.EstAficionado && u.Id != User.getUserId()).ToList();
            foreach(var user in Users)
            {
                user.Organisme = _db.Organisme.Where(o => o.Id == user.OrganismeId).FirstOrDefault();
            }
            return View(Users);
        }

        public async Task<IActionResult> DetailAficionado(string Id)
        {
            var specialiteUser = await _db.SpecialiteUtilisateur.Where(s => s.UtilisateurId == Id).ToListAsync();
            var roleUser = await _db.RoleUtilisateur.Where(r => r.UtilisateurId == Id).ToListAsync();
            foreach (var role in roleUser)
            {
                role.Role = await _db.Role.Where(r => r.Id == role.RoleId).FirstOrDefaultAsync();
                role.Specialite = new List<Specialite>();
                foreach (var specialite in specialiteUser)
                {
                    var spec = await _db.Specialite.Where(s => s.Id == specialite.SpecialiteId).FirstOrDefaultAsync();
                    if (spec.RoleId == role.RoleId)
                    {
                        role.Specialite.Add(spec);
                    }
                }
            }

            ListCandidatViewModel user = new ListCandidatViewModel()
            {
                User = await _db.Users.Where(u => u.Id == Id).FirstOrDefaultAsync(),
                RoleUser = roleUser
            };
            ViewBag.Url = Request.Headers["Referer"].ToString();
            user.Organisme = await _db.Organisme.Where(o => o.Id == user.User.OrganismeId).FirstOrDefaultAsync();
            return View(user);
        }
    }
}