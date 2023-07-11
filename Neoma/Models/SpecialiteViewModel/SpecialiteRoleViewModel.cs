using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.SpecialiteViewModel
{
    public class SpecialiteRoleViewModel
    {
        public Specialite Specialite { get; set; }
        public Role Role { get; set; }
        public IEnumerable<Role> RoleList { get; set; }
        public List<Specialite> SpecialiteList { get; set; }
    }
}
