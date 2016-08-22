namespace Metabol.DbModels.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Compartment")]
    public partial class Compartment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Compartment()
        {
            Compartment1 = new HashSet<Compartment>();
            Species = new HashSet<Species>();
        }

        public Guid id { get; set; }

        public Guid modelId { get; set; }

        [StringLength(100)]
        public string sbmlId { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        public Guid? compartmentTypeId { get; set; }

        public byte spatialDimensions { get; set; }

        public double? size { get; set; }

        public Guid? unitsId { get; set; }

        public Guid? compartmentClassId { get; set; }

        public Guid? outside { get; set; }

        public bool constant { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Compartment> Compartment1 { get; set; }

        public virtual Compartment Compartment2 { get; set; }

        public virtual Model Model { get; set; }

        public virtual Sbase Sbase { get; set; }

        public virtual UnitDefinition UnitDefinition { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Species> Species { get; set; }
    }
}
