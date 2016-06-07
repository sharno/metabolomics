namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReactionBoundFix")]
    public partial class ReactionBoundFix
    {
        [Key]
        public Guid reactionId { get; set; }

        public double lowerbound { get; set; }

        public double upperbound { get; set; }

        public virtual Reaction Reaction { get; set; }
    }
}
