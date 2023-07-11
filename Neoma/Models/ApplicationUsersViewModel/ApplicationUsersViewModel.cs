using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.ApplicationUsersViewModel
{
    public class ApplicationUsersViewModel
    {
        public List<ApplicationUser> Utilisateurs { get; set; }
        public List<Organisme> Organismes { get; set; }
        public ApplicationUser User { get; set; }
    }
}
