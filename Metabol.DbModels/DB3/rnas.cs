using System;

namespace Metabol.DbModels.DB3
{
    public partial class rnas
    {
        public Guid id { get; set; }

        public byte rna_type_id { get; set; }

        public virtual gene_products gene_products { get; set; }
    }
}
