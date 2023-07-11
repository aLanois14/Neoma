using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.ProjetViewModel
{
    public class ListeProjetViewModel
    {
        public Projet Projet { get; set; }
        public Besoins Besoins { get; set; }
        public List<Specialite> Specialite { get; set; }
    }
}
