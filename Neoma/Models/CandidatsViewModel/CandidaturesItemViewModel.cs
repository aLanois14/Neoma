using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.CandidatsViewModel
{
    public class CandidaturesItemViewModel
    {
        public ApplicationUser User { get; set; }
        public Projet Projet { get; set; }
        public List<Candidature> Candidatures { get; set; }
        public List<Specialite> Specialite { get; set; }
        public List<Role> Role { get; set; }
    }
}
