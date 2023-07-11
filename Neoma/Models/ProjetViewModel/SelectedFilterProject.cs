using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.ProjetViewModel
{
    public class SelectedFilterProject
    {
        public int Organisme { get; set; }
        public int Role { get; set; }
        public int TypeProjet { get; set; }
        public string TexteRecherche { get; set; }
        public List<Specialite> Specialite { get; set; }
    }
}
