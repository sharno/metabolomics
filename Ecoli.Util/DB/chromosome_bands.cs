namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class chromosome_bands
    {
        [Key]
        public Guid chromosome_id { get; set; }

        [StringLength(50)]
        public string chromosome_name { get; set; }

        [StringLength(10)]
        public string arm { get; set; }

        [StringLength(20)]
        public string band { get; set; }

        public int? iscn_start { get; set; }

        public int? iscn_stop { get; set; }

        public int? bp_start { get; set; }

        public int? bp_stop { get; set; }

        [StringLength(20)]
        public string stain { get; set; }

        public double? density { get; set; }

        public long? bases { get; set; }
    }
}
