namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AnnotationQualifier")]
    public partial class AnnotationQualifier
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AnnotationQualifier()
        {
            MapModelsPathways = new HashSet<MapModelsPathway>();
            MapReactionECNumbers = new HashSet<MapReactionECNumber>();
            MapReactionsProcessEntities = new HashSet<MapReactionsProcessEntity>();
            ModelOrganism = new HashSet<ModelOrganism>();
            MapSpeciesMolecularEntities = new HashSet<MapSpeciesMolecularEntity>();
        }

        public short id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapModelsPathway> MapModelsPathways { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapReactionECNumber> MapReactionECNumbers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapReactionsProcessEntity> MapReactionsProcessEntities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModelOrganism> ModelOrganism { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapSpeciesMolecularEntity> MapSpeciesMolecularEntities { get; set; }
    }
}
