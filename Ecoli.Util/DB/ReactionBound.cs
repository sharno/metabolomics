namespace Ecoli.Util.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReactionBound")]
    public partial class ReactionBound
    {
        public Guid id { get; set; }

        public Guid reactionId { get; set; }

        public int lowerBound { get; set; }

        public int upperBound { get; set; }

        public virtual Reaction Reaction { get; set; }
    }
}
