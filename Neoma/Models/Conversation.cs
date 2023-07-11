using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models
{
    public class Conversation
    {
        [Required]
        [Key]
        [Column("ConversationId")]
        public int Id { get; set; }

        public DateTime DateCreation { get; set; }

        public DateTime DateLastMessage { get; set; }

        [NotMapped]
        public List<Message> Messages { get; set; }
    }
}
