using System;
using System.ComponentModel.DataAnnotations;

namespace Metabol.DbModels.DB2
{
    public partial class catalyzes
    {
        [Key]
        public Guid process_id { get; set; }

        public Guid? organism_group_id { get; set; }

        public Guid? gene_product_id { get; set; }

        [StringLength(20)]
        public string ec_number { get; set; }

        public virtual ec_numbers ec_numbers { get; set; }

        public virtual gene_products gene_products { get; set; }

        public virtual organism_groups organism_groups { get; set; }

        public virtual processes processes { get; set; }
    }
}
