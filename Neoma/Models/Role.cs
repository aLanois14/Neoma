using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neoma.Models
{
    public class Role
    {
        [Required]
        [Key]
        [Column("RoleId")]
        public int Id { get; set; }

        [Required]
        [Column("LibelleRole")]
        public string Name { get; set; }

        public bool Valide { get; set; }

        [NotMapped]
        public bool PeutSelectionner { get; set; }

        [NotMapped]
        public bool CanDelete { get; set; }
    }
}
