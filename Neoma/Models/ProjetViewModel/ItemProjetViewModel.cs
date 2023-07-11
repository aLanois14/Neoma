using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.ProjetViewModel
{
    public class ItemProjetViewModel
    {
        public ApplicationUser User { get; set; }
        public Organisme Organisme { get; set; }
        public Projet Projet { get; set; }
        public TypeProjet TypeProjet { get; set; }
        public List<ItemBesoinViewModel> ItemBesoin { get; set; }

    }
}
