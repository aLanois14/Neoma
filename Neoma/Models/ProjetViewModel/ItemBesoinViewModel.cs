using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.ProjetViewModel
{
    public class ItemBesoinViewModel
    {
        public Besoins Besoins { get; set; }
        public List<Candidature> Candidatures { get; set; }
        public ApplicationUser User { get; set; }
        public Role Role { get; set; }
        public List<Specialite> Specialites { get; set; }
        public bool IsCandidat { get; set; }
    }
}
