namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class molecular_entity_names
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public molecular_entity_names()
        {
            ec_number_name_lookups = new HashSet<ec_number_name_lookups>();
            entity_name_lookups = new HashSet<entity_name_lookups>();
        }

        public Guid id { get; set; }

        [Required]
        [StringLength(255)]
        public string name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ec_number_name_lookups> ec_number_name_lookups { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<entity_name_lookups> entity_name_lookups { get; set; }
    }
}
