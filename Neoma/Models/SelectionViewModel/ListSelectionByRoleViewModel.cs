using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.SelectionViewModel
{
    public class ListSelectionByRoleViewModel
    {
        public Role Role { get; set; }
        public List<ItemSelectionViewModel> ItemSelection { get; set; }
    }
}
