using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    public partial class process_entities
    {
        [Key]
        [Column(Order = 0)]
        public Guid process_id { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid entity_id { get; set; }

        [Key]
        [Column(Order = 2)]
        public byte role_id { get; set; }

        [Required]
        [StringLength(10)]
        public string quantity { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }

        public virtual molecular_entities molecular_entities { get; set; }

        public virtual processes processes { get; set; }
    }
}
