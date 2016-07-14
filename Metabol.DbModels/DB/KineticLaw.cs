using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB
{
    [Table("KineticLaw")]
    public partial class KineticLaw
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KineticLaw()
        {
            Reactions = new HashSet<Reaction>();
        }

        public Guid id { get; set; }

        [Required]
        public string math { get; set; }

        public virtual Sbase Sbase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reaction> Reactions { get; set; }
    }
}
