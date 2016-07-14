using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB
{
    public partial class pathway
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public pathway()
        {
            MapModelsPathways = new HashSet<MapModelsPathway>();
            pathway_links = new HashSet<pathway_links>();
            pathway_links1 = new HashSet<pathway_links>();
            pathway_processes = new HashSet<pathway_processes>();
            pathway_groups = new HashSet<pathway_groups>();
        }

        public Guid id { get; set; }

        [Required]
        [StringLength(100)]
        public string name { get; set; }

        public byte pathway_type_id { get; set; }

        [StringLength(255)]
        public string status { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }

        [Column(TypeName = "text")]
        public string layout { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapModelsPathway> MapModelsPathways { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pathway_links> pathway_links { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pathway_links> pathway_links1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pathway_processes> pathway_processes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pathway_groups> pathway_groups { get; set; }
    }
}
