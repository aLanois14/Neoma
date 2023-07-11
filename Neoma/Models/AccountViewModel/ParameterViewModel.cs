using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.AccountViewModel
{
    public class ParameterViewModel
    {
        public ApplicationUser User { get; set; }

        [Required]
        [EmailAddress(ErrorMessage ="Le format saisie est incorrect.")]
        public string Email { get; set; }

        [Required]
        //[DataType(DataType.PhoneNumber)]
        // [RegularExpression(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$", ErrorMessage = "Le format saisie est incorrect")]
        public string Phone { get; set; }

        public List<Specialite> Specialite { get; set; }
        public List<Role> Role { get; set; }
        public List<Projet> Projet { get; set; }
        public List<Organisme> Organismes { get; set; }
    }
}
