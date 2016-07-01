using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILOG.Concert;
using ILOG.CPLEX;
using Metabol.DbModels;
using Metabol.Util;
using Db = Metabol.DbModels.Db;

namespace Metabol
{
    class ROOM
    {
        public static bool Calculate(HyperGraph graph, IDictionary<string, double> preValues)
        {
            var model = new Cplex { Name = "ROOM" };
            var v = new Dictionary<string, INumVar>();
            var y = new Dictionary<string, IIntVar>();
            var delta = 0.05;
            var eps = 0.0001;

            foreach (var edge in graph.Edges.Values)
            {
                if (edge.IsPseudo)
                {
                    v[edge.Label] = model.NumVar(0.0, 1000, NumVarType.Float, edge.Label);
                    continue;
                }
                var vmax = Db.Context.ReactionBound.Single(e => e.reactionId == edge.Id).lowerBound;
                var vmin = Db.Context.ReactionBound.Single(e => e.reactionId == edge.Id).upperBound;

                v[edge.Label] = model.NumVar(vmax, vmin, NumVarType.Float, edge.Label);

                //skip blocked reactions
                if (vmin == 0 && vmax == 0) continue;
                y[edge.Label] = model.BoolVar("y_" + edge.Label);
            }

            // S*v=0
            foreach (var metabolite in graph.Nodes.Values)
            {
                var exp = model.LinearNumExpr();

                foreach (var reaction in metabolite.Consumers)
                {
                    var coefficient = FbaMoma.Coefficient(reaction, metabolite);
                    exp.AddTerm(v[reaction.Label], coefficient);
                }

                foreach (var reaction in metabolite.Producers)
                {
                    var coefficient = FbaMoma.Coefficient(reaction, metabolite);
                    exp.AddTerm(v[reaction.Label], coefficient);
                }

                model.AddEq(exp, 0.0, metabolite.Label);
            }

            var fobj = model.LinearIntExpr();

            foreach (var edge in graph.Edges.Values.Where(e => !e.IsPseudo))
            {
                if (!preValues.ContainsKey(edge.Label)) continue;

                var w = preValues[edge.Label];
                var wu = w + delta * Math.Abs(w) + eps;
                var wl = w - delta * Math.Abs(w) + eps;

                var vmax = Db.Context.ReactionBound.Single(e => e.reactionId == edge.Id).lowerBound;
                var vmin = Db.Context.ReactionBound.Single(e => e.reactionId == edge.Id).upperBound;

                //skip blocked reactions
                if (vmin == 0 && vmax == 0) continue;

                // v - y*(vmax-wu)
                var wuexp = model.LinearNumExpr();
                wuexp.AddTerm(v[edge.Label], 1);
                wuexp.AddTerm(y[edge.Label], -vmax);
                wuexp.AddTerm(y[edge.Label], wu);


                // v - y*(vmin-wl)
                var wlexp = model.LinearNumExpr();
                wlexp.AddTerm(v[edge.Label], 1);
                wlexp.AddTerm(y[edge.Label], -vmin);
                wlexp.AddTerm(y[edge.Label], wl);

                model.AddGe(wuexp, wu);
                model.AddLe(wlexp, wl);
                fobj.AddTerm(y[edge.Label], 1);
            }
            model.AddObjective(ObjectiveSense.Minimize, fobj);

            model.SetParam(Cplex.IntParam.RootAlg, Cplex.Algorithm.Auto);
            var isfeas = model.Solve();

            if (isfeas)
                graph.Edges.ToList().ForEach(d => d.Value.Flux = model.GetValue(v[d.Value.Label]));
            else
                graph.Edges.ToList().ForEach(d => d.Value.Flux = 0);
            model.ExportModel(string.Format("{0}{1}Model.lp", Core.Dir, graph.LastLevel));
            model.End();
            return isfeas;
        }

    }
}
