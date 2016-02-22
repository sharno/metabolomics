using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    class Model
    {
        public HashSet<Reaction> Reactions = new HashSet<Reaction>();
        public HashSet<Metabolite> Metabolites = new HashSet<Metabolite>();
    }
}
