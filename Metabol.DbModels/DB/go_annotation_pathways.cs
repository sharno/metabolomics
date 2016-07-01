using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB
{
    public partial class go_annotation_pathways
    {
        [Key]
        [Column(Order = 0)]
        public Guid pathway_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int go_level { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "image")]
        public byte[] serialized_image { get; set; }

        [Key]
        [Column(Order = 3, TypeName = "ntext")]
        public string serialized_image_map { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTime date_generated { get; set; }
    }
}
