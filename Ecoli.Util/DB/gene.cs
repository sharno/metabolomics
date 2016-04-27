namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class gene
    {
        public Guid id { get; set; }

        public Guid organism_group_id { get; set; }

        public Guid? chromosome_id { get; set; }

        public Guid homologue_group_id { get; set; }

        [StringLength(8000)]
        public string raw_address { get; set; }

        [StringLength(100)]
        public string cytogenic_address { get; set; }

        public long? genetic_address { get; set; }

        public long? relative_address { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }

        public virtual chromosome chromosome { get; set; }

        public virtual organism_groups organism_groups { get; set; }
    }
}
