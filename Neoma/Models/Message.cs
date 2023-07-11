using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models
{
    public class Message
    {
        [Required]
        [Key]
        [Column("MessageId")]
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime DateEnvoi { get; set; }

        public bool MessageLu { get; set; }

        public string FileName { get; set; }

        public string Files { get; set; }

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
    }
}
