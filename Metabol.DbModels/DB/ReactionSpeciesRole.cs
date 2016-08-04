using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB
{
    [Table("ReactionSpeciesRole")]
    public partial class ReactionSpeciesRole
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReactionSpeciesRole()
        {
            ReactionSpecies = new HashSet<ReactionSpecy>();
        }

        public byte id { get; set; }

        [StringLength(100)]
        public string role { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReactionSpecy> ReactionSpecies { get; set; }
    }
}
