using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neoma.Models
{
    public class Membre
    {
        [Required]
        [Key]
        [Column("MembreId")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom d'un membre est manquant")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom d'un membre est manquant")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "Le mail d'un membre est manquant")]
        [EmailAddress]
        public string Mail { get; set; }

        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [Required]
        [Display(Name = "Projet")]
        public int ProjetId { get; set; }

        [ForeignKey("ProjetId")]
        public virtual Projet Projet { get; set; }

        [NotMapped]
        public List<Specialite> Specialite { get; set; }
    }
}
