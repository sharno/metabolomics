using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    public partial class gene_products
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public gene_products()
        {
            gene_encodings = new HashSet<gene_encodings>();
            catalyzes = new HashSet<catalyzes>();
        }

        public Guid id { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<gene_encodings> gene_encodings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<catalyzes> catalyzes { get; set; }

        public virtual molecular_entities molecular_entities { get; set; }

        public virtual proteins proteins { get; set; }

        public virtual rnas rnas { get; set; }
    }
}
