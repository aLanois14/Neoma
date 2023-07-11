using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.CandidatViewModel
{
    public class ListCandidatViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Specialite> Specialite { get; set; }
        public List<Role> Role { get; set; }
        public List<RoleUtilisateur> RoleUser { get; set; }
        public Organisme Organisme { get; set; }
    }
}
