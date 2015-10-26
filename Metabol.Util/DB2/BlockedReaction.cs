namespace Metabol.Util.DB2
{
    using System;

    public partial class BlockedReaction
    {
        public Guid id { get; set; }

        public Guid? reactionId { get; set; }
    }
}
