using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neoma.Models
{
    public class Specialite
    {
        [Required]
        [Key]
        [Column("SpecialiteId")]
        public int Id { get; set; }

        [Required]
        [Column("LibelleSpecialite")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        public bool Valide { get; set; }

        [NotMapped]
        public bool Match { get; set; }

        [NotMapped]
        public bool Supprimable { get; set; }

        [Display(Name = "Utilisateur")]
        public string UtilisateurId { get; set; }

        [ForeignKey("UtilisateurId")]
        public ApplicationUser Utilisateur { get; set; }

        [NotMapped]
        public bool CanDelete { get; set; }
    }
}
