using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Neoma.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name ="Prénom")]
        public string Prenom { get; set; }

        [Required]
        [Display(Name = "Nom de famille")]
        public string Nom { get; set; }

        public string Description { get; set; }

        [Display(Name = "Photo")]
        public byte[] Photo { get; set; }

        [NotMapped]
        public string PhotoStr { get; set; }

        public bool EstPorteur { get; set; }

        public bool EstCandidat { get; set; }

        public bool EstAficionado { get; set; }
        public bool Valide { get; set; }
        public string RoleActuel { get; set; }
        public DateTime LastAction { get; set; }

        [Display(Name = "Organisme")]
        public int OrganismeId { get; set; }

        [ForeignKey("OrganismeId")]
        public virtual Organisme Organisme { get; set; }

        [NotMapped]
        public bool EstAdmin { get; set; }

        [NotMapped]
        public bool EstSuperAdmin { get; set; }

        [NotMapped]
        public int Presence { get; set; }

        [NotMapped]
        public bool CanDelete { get; set; }
    }
}
