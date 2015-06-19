using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol
{
    public class Iteration
    {
        private static Iteration empty;

        public int Id;
        public int BorderMCount;
        public int Fba;
        public double Time;
        public IEnumerable<object> Nodes;
        public IEnumerable<object> Links;

        public Iteration(int iteration)
        {
            this.Id = iteration;
        }

        public static Iteration Empty
        {
            get
            {
                return empty ?? (empty = new Iteration(-1));
            }
            private set
            {
                empty = value;
            }
        }
    }
}
