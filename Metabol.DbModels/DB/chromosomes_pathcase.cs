namespace Metabol.DbModels.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class chromosomes_pathcase
    {
        [Key]
        [Column(Order = 0)]
        public Guid id { get; set; }

        public Guid? organism_group_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string name { get; set; }

        public long? length { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int centromere_location { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }
    }
}
