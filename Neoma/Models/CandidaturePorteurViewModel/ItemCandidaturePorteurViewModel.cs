using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.CandidaturePorteurViewModel
{
    public class ItemCandidaturePorteurViewModel
    {
        public ItemUserViewModel ItemUser { get; set; }
        public Projet Projet { get; set; }
        public List<ItemCandidatureViewModel> ItemCandidature { get; set; }
    }
}
