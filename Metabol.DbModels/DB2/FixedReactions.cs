using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabol.DbModels.DB2
{
    public partial class FixedReactions
    {
        [Key]
        [Column(Order = 0)]
        public Guid id { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid reactionId { get; set; }

        [Key]
        [Column(Order = 2)]
        public float lowerBound { get; set; }

        [Key]
        [Column(Order = 3)]
        public float upperBound { get; set; }

        public virtual Reaction Reaction { get; set; }
    }
}
