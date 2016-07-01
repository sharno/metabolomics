using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    [Table("AnnotationQualifier")]
    public partial class AnnotationQualifier
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AnnotationQualifier()
        {
            MapModelsPathways = new HashSet<MapModelsPathways>();
            MapReactionECNumber = new HashSet<MapReactionECNumber>();
            MapReactionsProcessEntities = new HashSet<MapReactionsProcessEntities>();
            ModelOrganism = new HashSet<ModelOrganism>();
            MapSpeciesMolecularEntities = new HashSet<MapSpeciesMolecularEntities>();
        }

        public short id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapModelsPathways> MapModelsPathways { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapReactionECNumber> MapReactionECNumber { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapReactionsProcessEntities> MapReactionsProcessEntities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModelOrganism> ModelOrganism { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapSpeciesMolecularEntities> MapSpeciesMolecularEntities { get; set; }
    }
}
