namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Unit")]
    public partial class Unit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Unit()
        {
            UnitDefinitions = new HashSet<UnitDefinition>();
        }

        public Guid id { get; set; }

        public Guid? modelId { get; set; }

        public Guid kind { get; set; }

        public int exponent { get; set; }

        public int scale { get; set; }

        public double multiplier { get; set; }

        public virtual Model Model { get; set; }

        public virtual Sbase Sbase { get; set; }

        public virtual UnitDefinition UnitDefinition { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UnitDefinition> UnitDefinitions { get; set; }
    }
}
