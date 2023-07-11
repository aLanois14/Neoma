using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models
{
    public class TypeProjet
    {
        [Required]
        [Key]
        [Column("TypeProjetId")]
        public int Id { get; set; }

        [Required]
        [Column("LibelleTypeProjet")]
        public string Name { get; set; }

        [Required]
        public bool Valide { get; set; }

        [NotMapped]
        public bool CanDelete { get; set; }
    }
}
