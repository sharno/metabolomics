namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class basic_molecules
    {
        public Guid id { get; set; }

        public virtual molecular_entities molecular_entities { get; set; }

        public virtual common_molecules common_molecules { get; set; }
    }
}
