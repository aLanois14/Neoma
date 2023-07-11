using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.PorteursViewModel
{
    public class TableListPorteur
    {
        public ApplicationUser User { get; set; }
        public List<Specialite> Specialite { get; set; }
        public List<Role> Role { get; set; }
        
        //public Candidature Candidature { get; set; }
    }
}
