using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol
{
    using PathwaysLib.ServerObjects;

    public class Metabolite : IComparable<Metabolite>
    {
        public string Compartment;
        //public string Formula;
        public readonly Guid Id;
        public string Name;
        public double NormalConcentration;

        public Metabolite() { }
        public Metabolite(ServerSpecies s)
        {
            Name = s.SbmlId;//Name.Replace("-","_").Replace("(","").Replace(")","");
            Id = s.ID;
            NormalConcentration = s.InitialConcentration;
            Compartment = s.CompartmentId.ToString();
        }

        public int CompareTo(Metabolite other)
        {
            return Id.CompareTo(other.Id);
        }

        public override string ToString()
        {
            return Name;//"Metabolite: " + Id + " comp:" + Compartment ;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Metabolite)obj);
        }

        protected bool Equals(Metabolite other)
        {
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }


    public class MetaboliteWithStoichiometry
    {
        public Metabolite Metabolite;
        public double Stoichiometry;

        public override string ToString()
        {
            return $"{this.Stoichiometry} times {this.Metabolite}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((MetaboliteWithStoichiometry)obj);
        }

        protected bool Equals(MetaboliteWithStoichiometry other)
        {
            return Equals(Metabolite, other.Metabolite);
        }

        public override int GetHashCode()
        {
            return Metabolite?.GetHashCode() ?? 0;
        }

    }

}
