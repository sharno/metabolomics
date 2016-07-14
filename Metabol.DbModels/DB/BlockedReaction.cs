using System;

namespace Metabol.DbModels.DB
{
    public partial class BlockedReaction
    {
        public Guid id { get; set; }

        public Guid? reactionId { get; set; }
    }
}
