using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.SelectionViewModel
{
    public class ItemSelectionViewModel
    {
        public ApplicationUser User { get; set; }
        public Projet Projet { get; set; }
        public Selection Selection { get; set; }
        public List<ItemRoleViewModel> ItemRole { get; set; }
    }
}
