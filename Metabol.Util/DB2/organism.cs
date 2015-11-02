namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("organisms")]
    public partial class organism
    {
        public Guid id { get; set; }

        [StringLength(20)]
        public string taxonomy_id { get; set; }

        public int cM_unit_length { get; set; }
    }
}
