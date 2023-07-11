using System.Collections.Generic;

namespace Neoma.Models.CandidatureRetenueViewModel
{
    public class ItemBesoinViewModel
    {
        public Besoins Besoins { get; set; }
        public Role Role { get; set; }
        public List<Specialite> Specialites { get; set; }
    }
}