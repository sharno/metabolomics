using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Metabol.DbModels.DB3
{
    public partial class ec_numbers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ec_numbers()
        {
            ec_number_name_lookups = new HashSet<ec_number_name_lookups>();
            catalyzes = new HashSet<catalyzes>();
            MapReactionECNumber = new HashSet<MapReactionECNumber>();
        }

        [Key]
        [StringLength(20)]
        public string ec_number { get; set; }

        [Required]
        [StringLength(255)]
        public string name { get; set; }

        [StringLength(20)]
        public string nodeCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ec_number_name_lookups> ec_number_name_lookups { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<catalyzes> catalyzes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapReactionECNumber> MapReactionECNumber { get; set; }
    }
}
