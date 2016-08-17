using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    public partial class gene_encodings
    {
        [Key]
        [Column(Order = 0)]
        public Guid gene_id { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid gene_product_id { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }

        public virtual gene_products gene_products { get; set; }
    }
}
