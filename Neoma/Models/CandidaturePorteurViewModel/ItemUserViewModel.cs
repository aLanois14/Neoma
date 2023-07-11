using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.CandidaturePorteurViewModel
{
    public class ItemUserViewModel
    {
        public ApplicationUser User { get; set; }
        public List<ItemRoleViewModel> Role { get; set; }
    }
}
