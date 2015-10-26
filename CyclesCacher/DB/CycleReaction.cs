namespace CyclesCacher.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CycleReaction")]
    public partial class CycleReaction
    {
        [Key]
        [Column(Order = 0)]
        public Guid cycleId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid reactionId { get; set; }

        public bool isReaction { get; set; }

        public virtual Cycle Cycle { get; set; }
    }
}
