namespace Metabol.Util.DB2
{
    using System;

    public partial class protein
    {
        public Guid id { get; set; }

        public virtual gene_products gene_products { get; set; }
    }
}
