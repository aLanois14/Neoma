using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neoma.Models
{
    public class MembreSpecialite
    {
        [Required]
        [Key]
        public int MembreSpecialiteId { get; set; }

        #region Clé étrangère
            [Required]
            [Display(Name = "Membre")]
            public int MembreId { get; set; }

            [ForeignKey("MembreId")]
            public virtual Membre Membre { get; set; }

            [Required]
            [Display(Name = "Specialite")]
            public int SpecialiteId { get; set; }

            [ForeignKey("SpecialiteId")]
            public virtual Specialite Specialite { get; set; }
        #endregion
    }
}
