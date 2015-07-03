using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol
{
    public class Reaction
    {
        public Guid Id;
        public string Name;
        public Dictionary<Guid, MetaboliteWithStoichiometry> Products;
        public Dictionary<Guid, MetaboliteWithStoichiometry> Reactants;
        public bool Reversible;
        public int Level;

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Reaction: " + Id + " name:" + Name + " reversible:" + Reversible + " (");
            //sb.AppendLine("\treactants: ");

            //foreach (var meta in Reactants)
            //{
            //    sb.AppendLine("\t" + meta);
            //}
            //sb.AppendLine("\tproducts: ");

            //foreach (var meta in Products)
            //{
            //    sb.AppendLine("\t" + meta);
            //}
            sb.AppendLine(")");
            return sb.ToString();
        }
    }

}
