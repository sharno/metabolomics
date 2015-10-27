namespace Metabol.Util.DB2
{
    using System;

    public partial class rna
    {
        public Guid id { get; set; }

        public byte rna_type_id { get; set; }

        public virtual gene_products gene_products { get; set; }
    }
}
