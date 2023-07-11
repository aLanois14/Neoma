using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.MesProjetsViewModel
{
    public class ItemProjetViewModel
    {
        public Projet Projet { get; set; }
        public Organisme Organisme { get; set; }
        public List<ItemBesoinViewModel> Besoins { get; set; }
    }
}
