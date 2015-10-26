namespace Metabol.Util.DB2
{
    using System;

    public partial class basic_molecules
    {
        public Guid id { get; set; }

        public virtual molecular_entities molecular_entities { get; set; }

        public virtual common_molecules common_molecules { get; set; }
    }
}
