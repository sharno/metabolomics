using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    class Metabolite
    {
        public readonly string Id;
        public string Name;
        public double Charge;
        public string Formula;
        public Compartment Compartment;
        public Dictionary<Reaction, double> Reactions = new Dictionary<Reaction, double>();

        //public Dictionary<Reaction, double> Consumers
        //{
        //    get { return Reactions.Where(e => e.Value < 0).ToDictionary(e => e.Key, e => e.Value); }
        //}
        //public Dictionary<Reaction, double> Producers
        //{
        //    get { return Reactions.Where(e => e.Value > 0).ToDictionary(e => e.Key, e => e.Value); }
        //}

        Metabolite(string id)
        {
            this.Id = id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Id.Equals(((Metabolite)obj).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
