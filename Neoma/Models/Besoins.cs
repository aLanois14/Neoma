using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neoma.Models
{
    public class Besoins
    {
        [Required]
        [Key]
        [Column("BesoinId")]
        public int Id { get; set; }

        [NotMapped]
        public List<Specialite> Specialite { get; set; }

        [NotMapped]
        public bool PeutPostuler { get; set; }

        [NotMapped]
        public bool DejaPostuler { get; set; }

        [NotMapped]
        public bool JeSuisCandidat { get; set; }

        [NotMapped]
        public List<Candidature> Candidatures { get; set; }

        #region Clé étrangère
            [Required]
            [Display(Name = "Projet")]
            public int ProjetId { get; set; }

            [ForeignKey("ProjetId")]
            public virtual Projet Projet { get; set; }

            [Required]
            [Display(Name = "Role")]
            public int RoleId { get; set; }

            [ForeignKey("RoleId")]
            public virtual Role Role { get; set; }
                    
            [Display(Name = "Utilisateur")]
            public string UtilisateurId { get; set; }

            [ForeignKey("UtilisateurId")]
            public virtual ApplicationUser Utilisateur { get; set; }
        #endregion
    }
}
