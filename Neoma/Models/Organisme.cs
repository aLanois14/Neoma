using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neoma.Models
{
    public class Organisme
    {
        [Required]
        [Key]
        [Column("OrganismeId")]
        public int Id { get; set; }

        [Required]
        [Column("LibelleOrganisme")]
        public string Name { get; set; }

        #region Adresse
            public string Ville { get; set; }
            public string Adresse { get; set; }
            public string CodePostal { get; set; }
        #endregion

        [Required]
        public bool Valide { get; set; }

        [NotMapped]
        public bool CanDelete { get; set; }
    }
}
