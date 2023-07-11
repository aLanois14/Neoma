using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.CandidatureRetenueViewModel
{
    public class ItemCandidatureRetenueViewModel
    {
        public ApplicationUser User { get; set; }
        public Organisme Organisme { get; set; }
        public Projet Projet { get; set; }
        public TypeProjet TypeProjet { get; set; }
        public List<ItemCandidatureViewModel> Candidatures { get; set; }
        public List<ItemPropositionViewModel> Propositions { get; set; }
    }
}
