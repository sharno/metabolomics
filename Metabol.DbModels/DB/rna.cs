namespace Metabol.DbModels.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class rna
    {
        public Guid id { get; set; }

        public byte rna_type_id { get; set; }

        public virtual gene_products gene_products { get; set; }
    }
}
