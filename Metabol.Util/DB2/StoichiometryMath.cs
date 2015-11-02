namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StoichiometryMath")]
    public partial class StoichiometryMath
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StoichiometryMath()
        {
            ReactionSpecies = new HashSet<ReactionSpecy>();
        }

        public Guid id { get; set; }

        [Required]
        public string math { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReactionSpecy> ReactionSpecies { get; set; }

        public virtual Sbase Sbase { get; set; }
    }
}
