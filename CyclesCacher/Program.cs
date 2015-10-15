using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabol;
using PathwaysLib.ServerObjects;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Threading;
using CyclesCacher.DB;


namespace CyclesCacher
{
    class Program
    {
        static void Main(string[] args)
        {
            const int Outliear = 61;
            var strCon = ConfigurationManager.AppSettings["dbConnectString"];
            DBWrapper.Instance = new DBWrapper(strCon);

            var g = new HyperGraph();
            string[] zn =
           {
                 "ADP", "ATP(4-)",
                 "D-Fructose 6-phosphate", "D-Fructose 1,6-bisphosphate", "Dihydroxyacetone phosphate",
                 "Glyceraldehyde 3-phosphate", "L-threonine", "taurochenodeoxycholate",
                 "D-glucose", "3-Phospho-D-glycerate", "D-Glycerate 2-phosphate", "Phosphoenolpyruvate", "pyruvate",
                 "Prothrombin", "pantetheine"
             };

            var zlist =
                (from s in zn select ServerSpecies.AllSpeciesByNameOnly(s) into spec where spec.Length > 0 select spec[0])
                    .ToList();
            var rand = new Random((int)DateTime.UtcNow.ToBinary());
            var Z = new Dictionary<Guid, int>();
            foreach (var s in zlist)
                Z[s.ID] = rand.NextDouble() >= 0.5 ? 1 : -1;

            var count = 0;
            foreach (var sp in ServerSpecies.AllSpecies().Where(p => Util.GetReactionCountSum(p.ID) < Outliear))
            {
                count++;
//                if (count == 300)
//                {
//                    break;
//                }
                Console.WriteLine("adding metabolite " + count);


                foreach (var pr in sp.getAllReactions(Util.Product).Where(r => r.SbmlId != "R_biomass_reaction"))
                    g.AddProduct(pr.ID, pr.SbmlId, sp.ID, sp.SbmlId);

                foreach (var re in sp.getAllReactions(Util.Reactant).Where(r => r.SbmlId != "R_biomass_reaction"))
                    g.AddReactant(re.ID, re.SbmlId, sp.ID, sp.SbmlId);
            }

            Console.WriteLine("loaded the whole network");

            // var reactions = "";
            //g.Edges.Values.Select(TheAlgorithm.ToReaction).ToDictionary(e => e.Id);
            //var fba = new Fba();
            //fba.Solve(reactions, Z, g);
            //Console.ReadKey();

            Dictionary<Guid, Cycle> cycles = CyclesFinder.Run(g);

            // removing exchange reactions from non exchange in cycle
            foreach (var cycle in cycles)
            {
                List<KeyValuePair<Guid, HyperGraph.Edge>> toRemove = cycle.Value.inCycleReactions.Where(e => cycle.Value.graph.Edges.ContainsKey(e.Key)).ToList();
                toRemove.AddRange(cycle.Value.outOfCycleReactions.Where(e => cycle.Value.graph.Edges.ContainsKey(e.Key)).ToList());

                foreach (var reaction in toRemove)
                {
                    HyperGraph.Edge _;
                    cycle.Value.graph.Edges.TryRemove(reaction.Key, out _);
                }

                cycle.Value.inCycleReactions =
                    cycle.Value.inCycleReactions.Where(e => !cycle.Value.outOfCycleReactions.ContainsKey(e.Key)).ToDictionary(e => e.Key, e => e.Value);
            }



            using (var context = new DB.CycleReactionModel())
            {
                context.CycleReactions.RemoveRange(context.CycleReactions.Where(e => true));
                context.Cycles.RemoveRange(context.Cycles.Where(e => true));
                // TODO remove cycle reactions
                context.SaveChanges();


                foreach (var cycle in cycles)
                {
                    var cycleModel = new DB.Cycle() { id = cycle.Key };

                    foreach (var reaction in cycle.Value.graph.Edges)
                    {
                        cycleModel.CycleReactions.Add(new CycleReaction() {cycleId = cycle.Key, reactionId = reaction.Key, isExchange = false});
                    }
                    foreach (var reaction in cycle.Value.inCycleReactions)
                    {
                        cycleModel.CycleReactions.Add(new CycleReaction() { cycleId = cycle.Key, reactionId = reaction.Key, isExchange = true });
                    }
                    foreach (var reaction in cycle.Value.outOfCycleReactions)
                    {
                        cycleModel.CycleReactions.Add(new CycleReaction() { cycleId = cycle.Key, reactionId = reaction.Key, isExchange = true });
                    }
                    context.Cycles.Add(cycleModel);
//                    context.SaveChanges();
                }
                context.SaveChanges();
            }

            Console.WriteLine("finished saving to DB");
            Console.ReadKey();
        }
    }
}
