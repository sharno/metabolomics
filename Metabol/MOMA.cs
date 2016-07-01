using System.Collections.Generic;
using System.Linq;
using ILOG.Concert;
using ILOG.CPLEX;
using Metabol.DbModels;
using Metabol.Util;
using Db = Metabol.DbModels.Db;

namespace Metabol
{
    class MOMA
    {
        public static bool Calculate(HyperGraph graph, IDictionary<string, double> preValues, INumExpr fobj)
        {
            var model = new Cplex { Name = "MOMA" };
            var v = new Dictionary<string, INumVar>();
            var momaWeight = 0.75;
            foreach (var edge in graph.Edges.Values)
            {
                if (edge.IsPseudo)
                {
                    v[edge.Label] = model.NumVar(0.0, 1000, NumVarType.Float, edge.Label);
                    continue;
                }

                var lb = Db.Context.ReactionBound.Single(e => e.reactionId == edge.Id).lowerBound;
                var ub = Db.Context.ReactionBound.Single(e => e.reactionId == edge.Id).upperBound;
                //v[edge.Label] = Model.NumVar(lb, ub, NumVarType.Float, edge.Label);

                if (lb == 0 && ub == 0 || lb == -1000 && ub == 0 || edge.IsReversible || edge.Label.StartsWith("r") ||
                  edge.Label.StartsWith("RE") || edge.Label.StartsWith("ID") || edge.Label.StartsWith("THYPX") ||
                  edge.Label.StartsWith("DHE") || edge.Label.StartsWith("STS") || edge.Label.StartsWith("UMP") ||
                  edge.Label.StartsWith("EX") || edge.Label.StartsWith("P") || edge.Label.StartsWith("AK"))
                    v[edge.Label] = model.NumVar(lb, ub, NumVarType.Float, edge.Label);
                else
                    v[edge.Label] = model.NumVar(1, ub, NumVarType.Float, edge.Label);
            }

            var momaObj = model.AddObjective(ObjectiveSense.Minimize, "moma_obj");

            foreach (var metabolite in graph.Nodes.Values)
            {
                var exp = model.LinearNumExpr();

                foreach (var reaction in metabolite.Consumers)
                {
                    var coefficient = FbaMoma.Coefficient(reaction, metabolite);
                    exp.AddTerm(v[reaction.Label], coefficient);

                    if (!preValues.ContainsKey(reaction.Label)) continue;

                    var diff = model.Diff(v[reaction.Label], preValues[reaction.Label]);
                    var tmp = model.Prod(momaWeight, model.Square(diff));
                    model.AddToExpr(momaObj, tmp);
                }

                foreach (var reaction in metabolite.Producers)
                {
                    var coefficient = FbaMoma.Coefficient(reaction, metabolite);
                    exp.AddTerm(v[reaction.Label], coefficient);

                    if (!preValues.ContainsKey(reaction.Label)) continue;

                    var diff = model.Diff(v[reaction.Label], preValues[reaction.Label]);
                    //Model.AddToExpr(momaObj, Model.Square(diff));
                    var tmp = model.Prod(momaWeight, model.Square(diff));
                    model.AddToExpr(momaObj, tmp);
                }

                model.AddEq(exp, 0.0, metabolite.Label);
            }
            //Model.SetParam(Cplex.IntParam.RootAlg, Cplex.Algorithm.Auto);
            model.AddToExpr(momaObj, model.Prod(1 - momaWeight, fobj));

            var isfeas = model.Solve();

            if (isfeas)
                graph.Edges.ToList().ForEach(d => d.Value.Flux = model.GetValue(v[d.Value.Label]));
            else
                graph.Edges.ToList().ForEach(d => d.Value.Flux = 0);

            //Model.ExportModel(string.Format("{0}{1}modelmoma.lp", Core.Dir, graph.LastLevel));

            model.End();
            return isfeas;
        }
    }
}
