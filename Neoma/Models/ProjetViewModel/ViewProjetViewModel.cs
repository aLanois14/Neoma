using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.ProjetViewModel
{
    public class ViewProjetViewModel
    {
        public List<TypeProjet> TypeProjet { get; set; }
        public List<Role> Role { get; set; }
        public List<Organisme> Organisme { get; set; }
        public List<Specialite> Specialite { get; set; }
    }
}
