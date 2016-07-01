using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    public partial class processes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public processes()
        {
            MapReactionsProcessEntities = new HashSet<MapReactionsProcessEntities>();
            pathway_processes = new HashSet<pathway_processes>();
            process_entities = new HashSet<process_entities>();
        }

        public Guid id { get; set; }

        [Required]
        [StringLength(800)]
        public string name { get; set; }

        public bool? reversible { get; set; }

        [StringLength(100)]
        public string location { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }

        public Guid? generic_process_id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapReactionsProcessEntities> MapReactionsProcessEntities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pathway_processes> pathway_processes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<process_entities> process_entities { get; set; }

        public virtual catalyzes catalyzes { get; set; }
    }
}
