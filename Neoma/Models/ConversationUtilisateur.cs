using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models
{
    public class ConversationUtilisateur
    {
        [Required]
        [Key]
        [Column("ConversationUtilisateurId")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Utilisateur")]
        public string UtilisateurId { get; set; }

        [ForeignKey("UtilisateurId")]
        public virtual ApplicationUser Utilisateur { get; set; }

        [Required]
        [Display(Name = "Conversation")]
        public int ConversationId { get; set; }

        [ForeignKey("ConversationId")]
        public virtual Conversation Conversation { get; set; }

        [NotMapped]
        public Message LastMessage { get; set; }
    }
}
