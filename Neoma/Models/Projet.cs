using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neoma.Models
{
    public class Projet
    {
        [Required]
        [Key]
        [Column("ProjetId")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Veuillez saisir un nom pour votre projet.")]
        [Column("NomProjet")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Veuillez décrire votre projet.")]
        public string PresentationProjet { get; set; }      

        public string CommentaireProjet { get; set; }

        [Required]
        public bool Termine { get; set; }

        [Required]
        public bool Complet { get; set; }        

        [Required]
        public bool Actif { get; set; }

        [NotMapped]
        public List<Besoins> Besoins { get; set; }        
        #region Date
            [Required]
            public DateTime DateCreation { get; set; }

            [Required]
            public DateTime ProjectStart { get; set; }

            [Required]
            public DateTime ProjectEnd { get; set; }
        #endregion

        #region Roles
            public int RolesProposes { get; set; }
            
            public int RolesPourvus { get; set; }
        
            public int RolesNonPourvus { get; set; }
        #endregion

        #region Clé étrangère
            [Required]
            [Display(Name = "Utilisateur")]
            public string UtilisateurId { get; set; }

            [ForeignKey("UtilisateurId")]
            public ApplicationUser Utilisateur { get; set; }

            [Required]
            [Display(Name = "Type de projet")]
            public int TypeProjetId { get; set; }

            [ForeignKey("TypeProjetId")]
            public TypeProjet TypeProjet { get; set; }
        #endregion
    }
}
