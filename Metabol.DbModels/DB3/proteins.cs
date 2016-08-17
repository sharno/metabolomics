using System;

namespace Metabol.DbModels.DB3
{
    public partial class proteins
    {
        public Guid id { get; set; }

        public virtual gene_products gene_products { get; set; }
    }
}
