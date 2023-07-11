using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models
{
    public class BesoinsSpecialite
    {
        [Key]
        public int Id { get; set; }

        #region Clé étrangère
            public int BesoinsId { get; set; }

            [ForeignKey("BesoinsId")]
            public virtual Besoins Besoins { get; set; }
      
            public int SpecialiteId { get; set; }

            [ForeignKey("SpecialiteId")]
            public virtual Specialite Specialite { get; set; }
        #endregion
    }
}
