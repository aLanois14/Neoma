using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.CandidatViewModel
{
    public class CandidatViewModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public List<RoleUtilisateur> Role { get; set; }
        public List<Specialite> Specialite { get; set; }
        public string IdsDeleteSpecialites { get; set; }
    }
}
