using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using libsbmlcs;
using LPsolveSBML;

namespace Metabol
{
    public class FbaSbw : IDisposable
    {
        //public bool AddPrevAsConstraint { get; set; }
        public string Label { get; set; }
        public double LastRuntime { get; set; }
        public Dictionary<Guid, double> RemovedExchangeFlux { get; set; }
        public Dictionary<Guid, double> Results { get; set; }

        public ConcurrentDictionary<Guid, HashSet<Guid>> UpdateExchangeConstraint { get; set; }

        public FbaSbw()
        {
            RemovedExchangeFlux = new Dictionary<Guid, double>();
            Results = new Dictionary<Guid, double>();
            UpdateExchangeConstraint = new ConcurrentDictionary<Guid, HashSet<Guid>>();
        }

        public bool Solve(HGraph graph, IReadOnlyDictionary<Guid, int> smz)
        {
            var sbmlns = new SBMLNamespaces(3, 1, "fbc", 1);

            var document = new SBMLDocument(sbmlns);

            var model = document.createModel();

            AddMetabolites(graph, model, smz);

            //AddReactions(graph, model);
            var str = libsbml.writeSBMLToString(document);
            var fba = new FluxBalance(str) { Mode = FbaMode.Maximize, PrintDebug = true };

            AddConstraints(graph, fba);

            var r = fba.Solve();
            
            r.Names.ToList().ForEach(d => Results[Guid.Parse(d)] = r[d]);
            return !fba.LastResultHadError;
        }

        private void AddConstraints(HGraph graph, FluxBalance fba)
        {
            foreach (var reaction in graph.Edges.Values)
            {
                if (Results.ContainsKey(reaction.Id) && reaction.IsImaginary)
                    fba.AddContraint(new LPsolveConstraint(reaction.Label, lpsolve_constr_types.GE, Results[reaction.Id]));
                else if (Results.ContainsKey(reaction.Id))
                    fba.AddContraint(new LPsolveConstraint(reaction.Label, lpsolve_constr_types.EQ, Results[reaction.Id]));
                else
                    fba.AddContraint(new LPsolveConstraint(reaction.Label, lpsolve_constr_types.GE, 1.0));
            }
        }

        private void AddReactions(HGraph graph, Model model)
        {
            graph.Edges.Select(e =>
            {
                var reaction = model.createReaction();
                reaction.setId(e.Value.Id.ToString());
                reaction.setReversible(e.Value.ToServerReaction.Reversible);
                reaction.setFast(false);
                reaction.setName(e.Value.Label);

                e.Value.InputNodes.Select(n =>
                {
                    var reactant = reaction.createReactant();
                    reactant.setSpecies(n.Value.Id.ToString());
                    reactant.setStoichiometry(Util.CachedRs(e.Key, n.Key).Stoichiometry);
                    reactant.setConstant(true);
                    reactant.setName(n.Value.Label);
                    return reactant;
                });

                e.Value.OuputNodes.Select(n =>
                {
                    var product = reaction.createProduct();
                    product.setSpecies(n.Value.Id.ToString());
                    product.setStoichiometry(Util.CachedRs(e.Key, n.Key).Stoichiometry);
                    product.setConstant(true);
                    product.setName(n.Value.Label);
                    return product;
                });


                return reaction;
            });
        }

        private void AddMetabolites(HGraph graph, Model model, IReadOnlyDictionary<Guid, int> smz)
        {
            var mplugin = (FbcModelPlugin)(model.getPlugin("fbc"));
            var objective = mplugin.createObjective();
            objective.setType("maximize");
            objective.setId("Fobj");

            graph.Nodes.ToList().ForEach(n =>
            {
                var spe = model.createSpecies();
                spe.setId(n.Value.Id.ToString());
                //species.setCompartment("compartment");
                spe.setName(n.Value.Label);
                spe.setConstant(false);
                spe.setHasOnlySubstanceUnits(false);
                spe.setBoundaryCondition(false);
                foreach (var consumer in n.Value.InputToEdge)
                {
                    var reaction = model.createReaction();
                    reaction.setId(consumer.Id.ToString());
                    reaction.setReversible(!consumer.IsImaginary && consumer.ToServerReaction.Reversible);
                    reaction.setFast(false);
                    reaction.setName(consumer.Label);

                    var reactant = reaction.createReactant();
                    reactant.setId(n.Value.Id.ToString());
                    reactant.setSpecies(n.Value.Label);
                    reactant.setName(n.Value.Label);
                    reactant.setConstant(true);

                    var coefficient = Coefficient(consumer, n.Value);
                    reactant.setStoichiometry(coefficient);

                    if (!smz.ContainsKey(n.Value.Id)) continue;
                    var fluxObjective = objective.createFluxObjective();
                    fluxObjective.setReaction(consumer.Label);
                    fluxObjective.setCoefficient(coefficient * smz[n.Value.Id]);
                }

                foreach (var producer in n.Value.OutputFromEdge)
                {
                    var reaction = model.createReaction();
                    reaction.setId(producer.Id.ToString());
                    reaction.setReversible(!producer.IsImaginary && producer.ToServerReaction.Reversible);
                    reaction.setFast(false);
                    reaction.setName(producer.Label);

                    var product = reaction.createProduct();
                    product.setId(n.Value.Id.ToString());
                    product.setSpecies(n.Value.Label);
                    product.setName(n.Value.Label);
                    product.setConstant(true);

                    var coefficient = Coefficient(producer, n.Value);
                    product.setStoichiometry(coefficient);

                    if (!smz.ContainsKey(n.Value.Id)) continue;
                    var fluxObjective = objective.createFluxObjective();
                    fluxObjective.setReaction(producer.Label);
                    fluxObjective.setCoefficient(coefficient * smz[n.Value.Id]);
                }
            });

            mplugin.setActiveObjectiveId("Fobj");
        }

        private static double Coefficient(HGraph.Edge reaction, HGraph.Node meta)
        {
            //var co = reaction.InputNodes.ContainsKey(meta.Id) ? -1 : reaction.OuputNodes.ContainsKey(meta.Id) ? 1 : 0;
            return reaction.IsImaginary ? 1.0 : Util.CachedRs(reaction.Id, meta.Id).Stoichiometry;
        }

        public void Dispose()
        {
            RemovedExchangeFlux.Clear();
            Results.Clear();
            UpdateExchangeConstraint.Clear();
        }
    }
}