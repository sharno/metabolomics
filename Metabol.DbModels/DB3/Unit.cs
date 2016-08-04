using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    [Table("Unit")]
    public partial class Unit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Unit()
        {
            UnitDefinition1 = new HashSet<UnitDefinition>();
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
        public virtual ICollection<UnitDefinition> UnitDefinition1 { get; set; }
    }
}
