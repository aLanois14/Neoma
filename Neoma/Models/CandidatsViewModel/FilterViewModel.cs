using System.Collections.Generic;

namespace Neoma.Models.CandidatsViewModel
{
    public class FilterViewModel
    {
        public List<Specialite> Specialite { get; set; }
        public Role Role { get; set; }
        public Besoins Besoins { get; set; }
        public Projet Projet { get; set; }
        public string TexteRecherche { get; set; }
    }
}
