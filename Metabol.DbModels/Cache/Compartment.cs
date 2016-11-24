using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol.DbModels.Cache
{
    [Serializable]
    public class Compartment
    {
        public Guid id { get; set; }

        public Guid modelId { get; set; }

        public string sbmlId { get; set; }

        public string name { get; set; }
    }
}
