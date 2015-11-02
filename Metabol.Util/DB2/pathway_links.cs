namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pathway_links
    {
        [Key]
        [Column(Order = 0)]
        public Guid pathway_id_1 { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid pathway_id_2 { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid entity_id { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }

        public virtual molecular_entities molecular_entities { get; set; }

        public virtual pathway pathway { get; set; }

        public virtual pathway pathway1 { get; set; }
    }
}
