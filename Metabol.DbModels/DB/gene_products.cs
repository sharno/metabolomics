using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB
{
    public partial class gene_products
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public gene_products()
        {
            gene_encodings = new HashSet<gene_encodings>();
            catalyzes = new HashSet<catalyze>();
        }

        public Guid id { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<gene_encodings> gene_encodings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<catalyze> catalyzes { get; set; }

        public virtual molecular_entities molecular_entities { get; set; }

        public virtual protein protein { get; set; }

        public virtual rna rna { get; set; }
    }
}
