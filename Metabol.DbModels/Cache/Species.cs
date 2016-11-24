using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol.DbModels.Cache
{
    [Serializable]
    public class Species
    {
        public Species()
        {
            ReactionSpecies = new HashSet<ReactionSpecy>();
        }

        public Guid id { get; set; }

        public string sbmlId { get; set; }

        public string name { get; set; }

        public Guid compartmentId { get; set; }
        
        public int? charge { get; set; }

        public virtual Compartment Compartment { get; set; }
        
        public virtual ICollection<ReactionSpecy> ReactionSpecies { get; set; }
    }
}
