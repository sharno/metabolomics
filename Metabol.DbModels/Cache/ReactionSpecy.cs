using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol.DbModels.Cache
{
    [Serializable]
    public class ReactionSpecy
    {
        public Guid id { get; set; }

        public Guid reactionId { get; set; }

        public Guid speciesId { get; set; }

        public byte roleId { get; set; }

        public double stoichiometry { get; set; }

        public string sbmlId { get; set; }

        public string name { get; set; }

        public virtual Reaction Reaction { get; set; }

        public virtual Species Species { get; set; }
    }
}
