﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SolverFoundation.Services;
using PathwaysLib.ServerObjects;

namespace Metabol
{
    using Microsoft.SolverFoundation.Common;

    public class Fba : IDisposable
    {
        public bool RemoveConstraints = false;
        public string Label { get; set; }
        public double LastRuntime { get; set; }
        public Dictionary<Guid, HGraph.Edge> RemovedConsumerExchange { get; set; }
        public Dictionary<Guid, HGraph.Edge> RemovedProducerExchange { get; set; }
        public Dictionary<Guid, double> Results { get; set; }
        public Dictionary<Guid, double> PrevResults { get; set; }
        public ConcurrentDictionary<Guid, HashSet<Guid>> UpdateExchangeConstraint { get; set; }

        private double varConst = 100000.0;
        public Fba()
        {
            Label = Util.FbaLabel();
            RemovedConsumerExchange = new Dictionary<Guid, HGraph.Edge>();
            RemovedProducerExchange = new Dictionary<Guid, HGraph.Edge>();
            Results = new Dictionary<Guid, double>();
            PrevResults = new Dictionary<Guid, double>();
            UpdateExchangeConstraint = new ConcurrentDictionary<Guid, HashSet<Guid>>();
        }

        public bool Solve(Dictionary<Guid, Reaction> reactions, Dictionary<Guid, int> smz, HGraph sm)
        {
            var context = SolverContext.GetContext();

            var model = context.CreateModel();

            var fluxes = AddReactionConstraits(model, reactions, sm);

            var fobj = AddMetabolites(model, reactions, fluxes, smz);

            model.AddGoal("Fobj", GoalKind.Maximize, fobj.ToTerm());

            var str = new StringBuilder();
            context.SaveModel(FileFormat.OML, new StringWriter(str));
            File.WriteAllText(Util.Dir + sm.LastLevel + "model.txt", str.ToString());
            str.Clear();
            var sim = new SimplexDirective();
            sim.Arithmetic = Arithmetic.Double;
            var solution = context.Solve(sim);
            Results.ToList().ForEach(d => PrevResults[d.Key] = d.Value);
            solution.Decisions.ToList().ForEach(d => Results[reactions.Values.First(r => r.Name == d.Name).Id] = d.ToDouble());
            var report = solution.GetReport();

            report.WriteTo(new StringWriter(str));
            File.WriteAllText(Util.Dir + sm.LastLevel + "result.txt", str.ToString());

            Console.WriteLine(report);

            var q = solution.Quality;
            context.ClearModel();
            return q == SolverQuality.Feasible || q == SolverQuality.Optimal;
        }

        private SumTermBuilder AddMetabolites(Model model, Dictionary<Guid, Reaction> reactions, Dictionary<Guid, Decision> decisions, IReadOnlyDictionary<Guid, int> smz)
        {
            var metabolites = new SortedSet<Metabolite>();

            foreach (var reaction in reactions.Values)
            {
                metabolites.UnionWith(reaction.Products.Select(s => s.Value.Metabolite));
                metabolites.UnionWith(reaction.Reactants.Select(s => s.Value.Metabolite));
            }

            var fobj = new SumTermBuilder(reactions.Count);
            foreach (var metabolite in metabolites)
            {
                var sv = new SumTermBuilder(reactions.Count);
                var svin = new SumTermBuilder(reactions.Count);
                var svout = new SumTermBuilder(reactions.Count);

                foreach (var react in reactions)
                {
                    var coefficient = Coefficient(react.Value, metabolite);

                    if (Math.Abs(coefficient) < double.Epsilon) continue; // coefficient==0

                    sv.Add(coefficient * decisions[react.Key]);
                    if (smz.ContainsKey(metabolite.Id))
                        fobj.Add(coefficient * smz[metabolite.Id] * decisions[react.Key]);

                    if (RemoveConstraints) continue;

                    if (react.Value.Reactants.ContainsKey(metabolite.Id)
                        && RemovedConsumerExchange.ContainsKey(metabolite.Id)
                        && RemovedConsumerExchange[metabolite.Id].Level < react.Value.Level)
                        svin.Add(Math.Abs(coefficient) * decisions[react.Key]);

                    if (react.Value.Products.ContainsKey(metabolite.Id)
                        && RemovedProducerExchange.ContainsKey(metabolite.Id)
                        && RemovedProducerExchange[metabolite.Id].Level < react.Value.Level)
                        svout.Add(Math.Abs(coefficient) * decisions[react.Key]);

                }
                model.AddConstraint(metabolite.Name, sv.ToTerm() == 0);

                if (RemoveConstraints) continue;

                //Add a constraint that total net flux of reactions of m’ should
                //be equal to those of the removed flux exchange reaction.
                if (RemovedConsumerExchange.ContainsKey(metabolite.Id))
                    model.AddConstraint(metabolite.Name + "_consumer", svin.ToTerm() == Results[RemovedConsumerExchange[metabolite.Id].Id]);

                if (RemovedProducerExchange.ContainsKey(metabolite.Id))
                    model.AddConstraint(metabolite.Name + "_producer", svout.ToTerm() == Results[RemovedProducerExchange[metabolite.Id].Id]);
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

        private Dictionary<Guid, Decision> AddReactionConstraits(Model model, Dictionary<Guid, Reaction> reactions, HGraph sm)
        {
            var decisions = new Dictionary<Guid, Decision>(reactions.Count);
            foreach (var reaction in reactions)
                decisions.Add(reaction.Key, new Decision(Domain.RealRange(1, Rational.PositiveInfinity), reaction.Value.Name));

            model.AddDecisions(decisions.Values.ToArray());

            // add constaint for each reaction
            //for (var i = 0; i < reactions.Count; i++)
            var i = 0;
            foreach (var reaction in reactions)
            {
                if (RemoveConstraints) continue;
                var ctx = Results.ContainsKey(reaction.Key) ? Results[reaction.Key] : 0;
                if (UpdateExchangeConstraint.ContainsKey(reaction.Key))
                {
                    var sv = new SumTermBuilder(UpdateExchangeConstraint[reaction.Key].Count);
                    sv.Add(decisions[reaction.Key]);
                    var meta = reaction.Value.Products.Select(e => e.Value.Metabolite)
                         .Concat(reaction.Value.Reactants.Select(e => e.Value.Metabolite)).First();

                    foreach (var guid in UpdateExchangeConstraint[reaction.Key])
                    {
                        var coefficient = Math.Abs(Coefficient(reactions[guid], meta));
                        sv.Add(coefficient * decisions[guid]); //.Find(d => d.Name == sm.Edges[guid].Label)
                    }

                    model.AddConstraint("update" + i++, sv.ToTerm() == ctx);
                }
                else if (Results.ContainsKey(reaction.Key))
                    model.AddConstraint("prev" + i++, decisions[reaction.Key] == ctx);
                //else if (RemovedConsumerExchange.ContainsKey(reactions[i].Id) ||
                //         RemovedProducerExchange.ContainsKey(reactions[i].Id))
                //    ;
                //else
                //    model.AddConstraint("init" + i, decisions[i] >= 1);

            }

            varConst /= 2;

            return decisions;
        }

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

        public void Dispose()
        {
            RemovedConsumerExchange.Clear();
            RemovedProducerExchange.Clear();
            Results.Clear();
            PrevResults.Clear();
        }
    }
}