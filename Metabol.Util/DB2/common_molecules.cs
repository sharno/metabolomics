namespace Metabol.Util.DB2
{
    using System;

    public partial class common_molecules
    {
        public Guid id { get; set; }

        public virtual basic_molecules basic_molecules { get; set; }
    }
}
