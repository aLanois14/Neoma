using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.SelectionViewModel
{
    public class SelectionViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Projet> Projets { get; set; }
        public List<Besoins> Besoins { get; set; }
        public Selection Selection { get; set; }
    }
}
