using ILOG.Concert;
using ILOG.CPLEX;
using Metabol.DbModels;
using Metabol.DbModels.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subsystems
{
    class Solve
    {
        public static void FixReactionBoundsByFVA(CacheModel db)
        {
            Cplex model = new Cplex();
            HyperGraph hypergraph = new HyperGraph();

            foreach (var s in db.Species)
            {
                hypergraph.AddSpeciesWithConnections(s);
            }

            var vars = new Dictionary<Guid, INumVar>();
            foreach (var r in hypergraph.Edges.Values)
            {
                vars[r.Id] = model.NumVar(r.LowerBound, r.UpperBound, NumVarType.Float, r.Label);
            }



            Program.AddMetabolitesSteadyStateConstraints(hypergraph, model, vars);

            // Objective function
            foreach (var r in hypergraph.Edges.Values)
            {
                var rbf = new ReactionBoundFix();

                model.Remove(model.GetObjective());
                model.AddObjective(ObjectiveSense.Maximize, vars[r.Id], "fobj");
                model.Solve();
                rbf.upperbound = model.GetValue(vars[r.Id]);

                model.Remove(model.GetObjective());
                model.AddObjective(ObjectiveSense.Minimize, vars[r.Id], "fobj");
                model.Solve();
                rbf.lowerbound = model.GetValue(vars[r.Id]);

                db.Reactions.Single(x => x.sbmlId == r.Label).ReactionBoundFix = rbf;
                Console.WriteLine($"{r.Label}: {rbf.lowerbound}\t{rbf.upperbound}");
            }
        }
    }
}
