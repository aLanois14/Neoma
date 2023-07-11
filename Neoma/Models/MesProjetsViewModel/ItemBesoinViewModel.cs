using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.MesProjetsViewModel
{
    public class ItemBesoinViewModel
    {
        public Besoins Besoin { get; set; }
        public ItemRoleViewModel ItemRole { get; set; }
        public List<Candidature> Candidatures { get; set; }
    }
}
