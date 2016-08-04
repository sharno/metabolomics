using System;

namespace Metabol.DbModels.DB3
{
    public partial class BlockedReactions
    {
        public Guid id { get; set; }

        public Guid? reactionId { get; set; }
    }
}
