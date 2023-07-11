using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.CandidatureViewModel
{
    public class CandidatureViewModel
    {
        public List<Candidature> Candidature { get; set; }
        public List<Proposition> Proposition { get; set; }
        public Projet Projet { get; set; }

    }
}
