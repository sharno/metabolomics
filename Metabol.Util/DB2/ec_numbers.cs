namespace Metabol.Util.DB2
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class ec_numbers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ec_numbers()
        {
            this.ec_number_name_lookups = new HashSet<ec_number_name_lookups>();
            this.catalyzes = new HashSet<catalyze>();
            this.MapReactionECNumbers = new HashSet<MapReactionECNumber>();
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
        public virtual ICollection<catalyze> catalyzes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MapReactionECNumber> MapReactionECNumbers { get; set; }
    }
}
