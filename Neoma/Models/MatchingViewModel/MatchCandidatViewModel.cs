using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.MatchingViewModel
{
    public class MatchCandidatViewModel
    {
        public List<Projet> Projet { get; set; }
        public List<Besoins> Besoins { get; set; }
        public List<Specialite> Specialite { get; set; }
        public List<Role> Role { get; set; }
    }
}
