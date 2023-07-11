using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.AccountViewModel
{
    public class LoginViewModel
    {
        public string Mail { get; set; }
        public List<string> Statut { get; set; }

        [Required(ErrorMessage = "Veuillez saisir un mot de passe")]
        public string Password { get; set; }
        public string SelectedStatut { get; set; }
    }
}
