using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    [Table("StoichiometryMath")]
    public partial class StoichiometryMath
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StoichiometryMath()
        {
            ReactionSpecies = new HashSet<ReactionSpecies>();
        }

        public Guid id { get; set; }

        [Required]
        public string math { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReactionSpecies> ReactionSpecies { get; set; }

        public virtual Sbase Sbase { get; set; }
    }
}
