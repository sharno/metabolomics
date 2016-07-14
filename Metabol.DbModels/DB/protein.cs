using System;

namespace Metabol.DbModels.DB
{
    public partial class protein
    {
        public Guid id { get; set; }

        public virtual gene_products gene_products { get; set; }
    }
}
