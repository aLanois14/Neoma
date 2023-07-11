using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.PorteurViewModel
{
    public class ItemPorteurViewModel
    {
        public ApplicationUser User { get; set; }
        public Organisme Organisme { get; set; }
        public List<Projet> Projet { get; set; }
        public List<ItemRoleViewModel> ItemRole { get; set; }
    }
}
