namespace Metabol.DbModels.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class chromosome
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public chromosome()
        {
            genes = new HashSet<gene>();
        }

        public Guid id { get; set; }

        public Guid? organism_group_id { get; set; }

        [Required]
        [StringLength(100)]
        public string name { get; set; }

        public long? length { get; set; }

        public int centromere_location { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<gene> genes { get; set; }
    }
}
