namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ec_go_orig
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string ec_number { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(7)]
        public string go_id { get; set; }
    }
}
