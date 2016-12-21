using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol.DbModels.Cache
{
    [Serializable]
    public class ReactionBound
    {
        public Guid id { get; set; }

        public Guid reactionId { get; set; }

        public int lowerBound { get; set; }

        public int upperBound { get; set; }

        public virtual Reaction Reaction { get; set; }
    }
}
