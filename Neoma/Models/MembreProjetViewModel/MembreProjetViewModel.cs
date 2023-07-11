using System.Collections.Generic;

namespace Neoma.Models.MembreProjetViewModel
{
    public class MembreProjetViewModel
    {
        public Projet Projet { get; set; }
        public List<Membre> Membre { get; set; }
        public List<Besoins> Besoins { get; set; }
        public List<Role> Role { get; set; }
        public List<Specialite> Specialite { get; set; }
        public List<TypeProjet> TypeProjet { get; set; }
    }
}
