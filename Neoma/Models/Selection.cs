using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models
{
    public class Selection
    {
        [Required]
        [Key]
        [Column("SelectionId")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Projet")]
        public int ProjetId { get; set; }

        [ForeignKey("ProjetId")]
        public virtual Projet Projet { get; set; }

        [Required]
        [Display(Name = "Utilisateur")]
        public string UtilisateurId { get; set; }

        [ForeignKey("UtilisateurId")]
        public virtual ApplicationUser Utilisateur { get; set; }

        [Required]
        [Display(Name = "Besoins")]
        public int BesoinsId { get; set; }

        [ForeignKey("BesoinsId")]
        public virtual Besoins Besoins { get; set; }

        public string Commentaire { get; set; }

        [Range(0, 5, ErrorMessage = "Veuillez saisir une note pour ce candidat."), Column(TypeName = "DECIMAL(2,1)")]
        public decimal Note { get; set; }

        [NotMapped]
        public string NoteString { get; set; }

        public DateTime DateCreation { get; set; }
    }
}
