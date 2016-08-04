using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB3
{
    [Table("CycleReaction")]
    public partial class CycleReaction
    {
        [Key]
        [Column(Order = 0)]
        public Guid cycleId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid otherId { get; set; }

        [Key]
        [Column(Order = 2)]
        public bool isReaction { get; set; }

        public virtual Cycle Cycle { get; set; }
    }
}
