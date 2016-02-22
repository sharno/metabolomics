using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    class Reaction
    {
        public readonly string Id;
        public Dictionary<Metabolite, double> Metabolites = new Dictionary<Metabolite, double>();
        public double UpperBound;
        public double LowerBound;

        public string Name;

        Reaction(string id)
        {
            this.Id = id;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Id.Equals(((Reaction)obj).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
