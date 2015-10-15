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


namespace CyclesCacher
{
    class Program
    {
        static void Main(string[] args)
        {
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
            foreach (var sp in ServerSpecies.AllSpecies())
            {
                count++;
                //                if (count == 100)
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



            using (var context = new DB.CycleReactionModel())
            {
                context.Cycles.RemoveRange(context.Cycles.Where(e => true));
                var reactions = context.Reactions.Where(reaction => true).ToList();
                reactions.ForEach(r => r.cycleId = null);
                context.SaveChanges();


                foreach (var cycle in cycles)
                {
                    var cycleModel = new DB.Cycle() { id = cycle.Key };

                    foreach (var reaction in cycle.Value.graph.Edges)
                    {
                        //DB.Reaction reactionModel = new DB.Reaction() { id = reaction.Key };
                        cycleModel.Reactions.Add(context.Reactions.Find(reaction.Key));
                    }
                    foreach (var reaction in cycle.Value.inCycleReactions)
                    {
//                        DB.Reaction reactionModel = new DB.Reaction() { id = reaction.Key };
                        cycleModel.Reactions.Add(context.Reactions.Find(reaction.Key));
                    }
                    foreach (var reaction in cycle.Value.outOfCycleReactions)
                    {
//                        DB.Reaction reactionModel = new DB.Reaction() { id = reaction.Key };
                        cycleModel.Reactions.Add(context.Reactions.Find(reaction.Key));
                    }
                    cycleModel.Reactions = cycleModel.Reactions.Distinct().ToList();
                    context.Cycles.Add(cycleModel);
                }
                context.SaveChanges();
            }

            Console.WriteLine("finished saving to DB");
            Console.ReadKey();
        }
    }
}
