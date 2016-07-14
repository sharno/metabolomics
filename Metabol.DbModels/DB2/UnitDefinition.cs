using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    [Table("UnitDefinition")]
    public partial class UnitDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UnitDefinition()
        {
            Compartment = new HashSet<Compartment>();
            Parameter = new HashSet<Parameter>();
            Species = new HashSet<Species>();
            Unit = new HashSet<Unit>();
            Unit1 = new HashSet<Unit>();
        }

        public Guid id { get; set; }

        public Guid? modelId { get; set; }

        [Required]
        [StringLength(100)]
        public string sbmlId { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        public bool isBaseUnit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Compartment> Compartment { get; set; }

        public virtual Model Model { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Parameter> Parameter { get; set; }

        public virtual Sbase Sbase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Species> Species { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Unit> Unit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Unit> Unit1 { get; set; }
    }
}
