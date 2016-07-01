using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Metabol.DbModels.DB2
{
    public partial class go_terms
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public go_terms()
        {
            GONodeCodes = new HashSet<GONodeCodes>();
        }

        [StringLength(7)]
        public string ID { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public int SubtreeHeight { get; set; }

        public int TotalDescendants { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GONodeCodes> GONodeCodes { get; set; }
    }
}
