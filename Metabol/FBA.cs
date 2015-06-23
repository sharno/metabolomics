using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SolverFoundation.Services;
using PathwaysLib.ServerObjects;

namespace Metabol
{
    public class Fba : IDisposable
    {
        //public bool AddPrevAsConstraint { get; set; }
        public string Label { get; set; }
        public double LastRuntime { get; set; }
        public Dictionary<Guid, double> RemovedExchangeFlux { get; set; }
        public Dictionary<Guid, double> Results { get; set; }
        public Dictionary<Guid, double> PrevResults { get; set; }
        public ConcurrentDictionary<Guid, HashSet<Guid>> UpdateExchangeConstraint { get; set; }

        public Fba()
        {
            RemovedExchangeFlux = new Dictionary<Guid, double>();
            Results = new Dictionary<Guid, double>();
            PrevResults = new Dictionary<Guid, double>();
            UpdateExchangeConstraint = new ConcurrentDictionary<Guid, HashSet<Guid>>();
        }

        public bool Solve(List<Reaction> reactions, Dictionary<Guid, int> smz, HGraph sm)
        {
            var context = SolverContext.GetContext();

            var model = context.CreateModel();

            var fluxes = AddReactionConstraits(model, reactions, sm);

            var fobj = AddMetabolites(model, reactions, fluxes, smz);

            model.AddGoal("Fobj", GoalKind.Maximize, fobj.ToTerm());

            var str = new StringBuilder();
            context.SaveModel(FileFormat.OML, new StringWriter(str));
            File.WriteAllText(Util.Dir + Label + "model.txt", str.ToString());
            str.Clear();

            var solution = context.Solve(new SimplexDirective());
            Results.ToList().ForEach(d => PrevResults[d.Key] = d.Value);
            solution.Decisions.ToList().ForEach(d => Results[reactions.Find(r => r.Name == d.Name).Id] = d.ToDouble());
            var report = solution.GetReport();
            //report.WriteTo(new StringWriter(str));
            //File.WriteAllText(Util.Dir + Label + "result.txt", str.ToString());

            Console.WriteLine(report);

            var q = solution.Quality;
            context.ClearModel();
            return q == SolverQuality.Feasible || q == SolverQuality.Optimal;
        }

        private SumTermBuilder AddMetabolites(Model model, IReadOnlyList<Reaction> reactions, IReadOnlyList<Decision> decisions, IReadOnlyDictionary<Guid, int> smz)
        {
            var metabolites = new SortedSet<Metabolite>();

            foreach (var reaction in reactions)
            {
                metabolites.UnionWith(reaction.Products.Select(s => s.Value.Metabolite));
                metabolites.UnionWith(reaction.Reactants.Select(s => s.Value.Metabolite));
            }

            var fobj = new SumTermBuilder(reactions.Count);
            foreach (var metabolite in metabolites)
            {
                var sv = new SumTermBuilder(reactions.Count);
                var sv2 = new SumTermBuilder(reactions.Count);

                for (var i = 0; i < reactions.Count; i++)
                {
                    var coefficient = Coefficient(reactions[i], metabolite);

                    if (coefficient == 0) continue;

                    sv.Add(coefficient * decisions[i]);
                    sv2.Add(coefficient * decisions[i]);

                    if (smz.ContainsKey(metabolite.Id))
                        fobj.Add(coefficient * smz[metabolite.Id] * decisions[i]);
                }

                //Add a constraint that total net flux of reactions of m’ should
                //be equal to those of the removed flux exchange reaction.

                if (RemovedExchangeFlux.ContainsKey(metabolite.Id))
                    model.AddConstraint(metabolite.Name + "_1", sv2.ToTerm() == RemovedExchangeFlux[metabolite.Id]);
                else
                    model.AddConstraint(metabolite.Name, sv.ToTerm() == 0);
            }
            return fobj;
        }

        private static double Coefficient(Reaction reaction, Metabolite metabolite1)
        {
            var coefficient = 0.0;
            if (reaction.Products.ContainsKey(metabolite1.Id))
                coefficient = reaction.Products[metabolite1.Id].Stoichiometry;

            if (reaction.Reactants.ContainsKey(metabolite1.Id))
                coefficient = (-1 * reaction.Reactants[metabolite1.Id].Stoichiometry);
            return coefficient;
        }

        private List<Decision> AddReactionConstraits(Model model, IReadOnlyList<Reaction> reactions, HGraph sm)
        {
            var decisions = new List<Decision>(reactions.Count); // 
            decisions.AddRange(reactions.Select(t => new Decision(Domain.RealNonnegative, t.Name)));
            model.AddDecisions(decisions.ToArray());

            // add constaint for each reaction
            for (var i = 0; i < reactions.Count; i++)
            {
                if (UpdateExchangeConstraint.ContainsKey(reactions[i].Id))
                {
                    var sv = new SumTermBuilder(UpdateExchangeConstraint[reactions[i].Id].Count);
                    sv.Add(reactions[i].Name);
                    var meta = reactions[i].Products.Select(e => e.Value.Metabolite)
                         .Concat(reactions[i].Reactants.Select(e => e.Value.Metabolite)).First();
                    var coefficient = Coefficient(reactions[i], meta);

                    foreach (var guid in UpdateExchangeConstraint[reactions[i].Id])
                        sv.Add(coefficient * decisions.Find(d => d.Name == sm.Edges[guid].Label));

                    model.AddConstraint("c" + i, sv.ToTerm() == Results[reactions[i].Id]);
                }
                else if (Results.ContainsKey(reactions[i].Id)) // && !RemovedExchangeFlux.ContainsKey(reactions[i].Id)
                    model.AddConstraint("c" + i, decisions[i] == Results[reactions[i].Id]);
                else
                    model.AddConstraint("c" + i, 2 <= decisions[i]);
            }

            return decisions;
        }

        //private static bool ReactionHasMetabolite(Reaction reaction, Metabolite metabolite)
        //{
        //    return reaction.Products.Any(ms => ms.Metabolite.Id.Equals(metabolite.Id)) || reaction.Reactants.Any(ms => ms.Metabolite.Id.Equals(metabolite.Id));
        //}

        //private static List<CountedMetabolite> CountAndSort(List<Reaction> reactions)
        //{
        //    var counts = new Dictionary<string, int>();
        //    foreach (var reaction in reactions)
        //    {
        //        foreach (var id in reaction.Reactants.Select(meta => meta.Metabolite.Id))
        //        {
        //            if (counts.ContainsKey(id))
        //            {
        //                counts[id] = counts[id] + 1;
        //            }
        //            else
        //            {
        //                counts[id] = 1;
        //            }
        //        }
        //        foreach (var id in reaction.Products.Select(meta => meta.Metabolite.Id))
        //        {
        //            if (counts.ContainsKey(id))
        //            {
        //                counts[id] = counts[id] + 1;
        //            }
        //            else
        //            {
        //                counts[id] = 1;
        //            }
        //        }
        //    }
        //    var counteds = new List<CountedMetabolite>();
        //    foreach (var key in counts.Keys)
        //    {
        //        var countedMetabolite = new CountedMetabolite();
        //        countedMetabolite.Count = counts[key];
        //        countedMetabolite.Metabolite = Metabolites[key];
        //        counteds.Add(countedMetabolite);
        //    }
        //    counteds.Sort();
        //    return counteds;
        //}

        public class Metabolite : IComparable<Metabolite>
        {
            public string Compartment;
            //public string Formula;
            public Guid Id;
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
                return "" + Stoichiometry + " times " + Metabolite;
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

        //public class CountedMetabolite : IComparable<CountedMetabolite>
        //{
        //    public int Count;
        //    public Metabolite Metabolite;

        //    public int CompareTo(CountedMetabolite other)
        //    {
        //        return Count.CompareTo(other.Count);
        //    }
        //}

        public void Dispose()
        {
            RemovedExchangeFlux.Clear();
            Results.Clear();
            PrevResults.Clear();
        }
    }
}