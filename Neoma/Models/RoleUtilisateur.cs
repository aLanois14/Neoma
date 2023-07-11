using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models
{
    public class RoleUtilisateur
    {
        [Required]
        [Key]
        [Column("RoleUtilisateurId")]
        public int Id { get; set; }

        [NotMapped]
        public List<Specialite> Specialite { get; set; }

        #region Clé étrangère
        [Required]
        [Display(Name = "Utisateur")]
        public string UtilisateurId { get; set; }

        [ForeignKey("UtilisateurId")]
        public virtual ApplicationUser Utilisateur { get; set; }

        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        #endregion
    }
}
