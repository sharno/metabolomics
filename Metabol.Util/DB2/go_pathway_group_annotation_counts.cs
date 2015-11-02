namespace Metabol.Util.DB2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class go_pathway_group_annotation_counts
    {
        [Key]
        [Column(Order = 0)]
        public Guid pathway_group_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(7)]
        public string go_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int hierarchy_level { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int number_annotations { get; set; }
    }
}
