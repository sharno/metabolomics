namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ModelMetadata")]
    public partial class ModelMetadata
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ModelMetadata()
        {
            DesignedBies = new HashSet<DesignedBy>();
        }

        public Guid Id { get; set; }

        [StringLength(500)]
        public string ModelName { get; set; }

        public int? PublicationId { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? ModificationDate { get; set; }

        public string Notes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DesignedBy> DesignedBies { get; set; }
    }
}
