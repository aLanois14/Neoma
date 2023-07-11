using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Neoma.Extensions;

namespace Neoma.Models
{
    public class Candidature
    {
        [Required]
        [Key]
        [Column("CandidatureId")]
        public int Id{ get; set; }

        [Required]
        public string Statut  { get; set; }

        [RequiredIf("Statut", EValidation.Refused, "Veuillez saisir un motif de refus de la candidature.")]
        public string Motif { get; set; }

        public DateTime DateCreation { get; set; }

        #region Clé étrangère
            [Required]
            [Display(Name ="Utilisateur")]
            public string UtilisateurId { get; set; }

            [ForeignKey("UtilisateurId")]
            public virtual ApplicationUser Utilisateur { get; set; }

            [Required]
            [Display(Name = "Besoins")]
            public int BesoinsId { get; set; }

            [ForeignKey("BesoinsId")]
            public virtual Besoins Besoins { get; set; }

            [Required]
            [Display(Name = "Projet")]
            public int ProjetId { get; set; }

            [ForeignKey("ProjetId")]
            public virtual Projet Projet { get; set; }
        #endregion
    }

    public enum EValidation
    {
        Pending = 0,
        Accepted = 1,
        Refused = 2,
        Complete = 3
    }
}
