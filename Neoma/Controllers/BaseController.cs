using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Neoma.Data;
using Neoma.Extensions;
using Neoma.Models;

namespace Neoma.Controllers
{
    public class BaseController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BaseController(ApplicationDbContext db)
        {
            _db = db;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var UserId = User.getUserId();
            if (UserId != null)
            {
                var user = _db.Users.Where(u => u.Id == UserId).FirstOrDefault();
                user.LastAction = DateTime.Now;
                _db.Update(user);
                _db.SaveChanges();
            }
            
            base.OnActionExecuting(filterContext);
        }
    }
}