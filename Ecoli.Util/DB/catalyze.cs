namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class catalyze
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

        public virtual process process { get; set; }
    }
}
