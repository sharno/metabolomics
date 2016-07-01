using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
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
