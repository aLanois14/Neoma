using Neoma.Models.CandidatViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.ProjetViewModel
{
    public class ProjetDetailViewModel
    {
        public Projet Projet { get; set; }
        public ListCandidatViewModel ListCandidat { get; set; }
    }
}
