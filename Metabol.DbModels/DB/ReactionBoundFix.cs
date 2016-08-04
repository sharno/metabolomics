using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB
{
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
