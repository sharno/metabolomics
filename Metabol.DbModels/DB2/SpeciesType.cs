using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    [Table("SpeciesType")]
    public partial class SpeciesType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SpeciesType()
        {
            Species = new HashSet<Species>();
        }

        public Guid id { get; set; }

        public Guid modelId { get; set; }

        [Required]
        [StringLength(100)]
        public string sbmlId { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        public virtual Model Model { get; set; }

        public virtual Sbase Sbase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Species> Species { get; set; }
    }
}
