using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.MatchingCandidatViewModel
{
    public class ItemCandidatViewModel
    {
        public ApplicationUser User { get; set; }
        public List<ItemRoleViewModel> Roles { get; set; }
    }
}
