using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neoma.Models
{
    public class SpecialiteUtilisateur
    {
        [Required]
        [Key]
        [Column("SpecialiteUtilisateurId")]
        public int Id { get; set; }

        #region Clé étrangère
            [Required]
            [Display(Name = "Utisateur")]
            public string UtilisateurId { get; set; }

            [ForeignKey("UtilisateurId")]
            public virtual ApplicationUser Utilisateur { get; set; }

            [Required]
            [Display(Name = "Specialite")]
            public int SpecialiteId { get; set; }

            [ForeignKey("SpecialiteId")]
            public virtual Specialite Specialite { get; set; }
        #endregion
    }
}
