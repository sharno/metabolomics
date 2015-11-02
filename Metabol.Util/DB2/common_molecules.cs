namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class common_molecules
    {
        public Guid id { get; set; }

        public virtual basic_molecules basic_molecules { get; set; }
    }
}
