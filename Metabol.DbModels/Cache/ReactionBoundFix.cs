using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol.DbModels.Cache
{
    [Serializable]
    public class ReactionBoundFix
    {
        public Guid reactionId { get; set; }

        public double lowerbound { get; set; }

        public double upperbound { get; set; }

        public virtual Reaction Reaction { get; set; }
    }
}
