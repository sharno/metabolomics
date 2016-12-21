using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol.DbModels.Cache
{
    [Serializable]
    public class Reaction
    {
        public Reaction()
        {
            ReactionBounds = new HashSet<ReactionBound>();
            ReactionSpecies = new HashSet<ReactionSpecy>();
        }

        public Guid id { get; set; }

        public Guid modelId { get; set; }

        public string sbmlId { get; set; }

        public string name { get; set; }

        public string subsystem { get; set; }
        
        public bool reversible { get; set; }

        public virtual ReactionBoundFix ReactionBoundFix { get; set; }

        public virtual ICollection<ReactionBound> ReactionBounds { get; set; }

        public virtual ICollection<ReactionSpecy> ReactionSpecies { get; set; }
    }
}
