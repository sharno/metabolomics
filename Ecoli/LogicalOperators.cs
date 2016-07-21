using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ILOG.Concert;
using ILOG.CPLEX;
using Metabol.DbModels;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Services;
using Microsoft.SolverFoundation.Solvers;

namespace Ecoli
{
    class LogicalOperators
    {
        public static void TestGeneReg()
        {
            var model = new Cplex();
            var v = new Dictionary<string, INumVar>();
            var g = new ConcurrentDictionary<string, INumVar>();
            //var Gr = new Dictionary<string, string>();
            var fva = new Dictionary<string, Tuple<float, float>>();

            for (var i = 1; i < 17; i++)
            {
                v[$"r{i}"] = model.NumVar(0, 100, NumVarType.Float, $"r{i}");
                g[$"g{i}"] = model.BoolVar($"g{i}");
                //Gr[$"r{i}"] = $"g{i}";
            }
            //v["r10"].LB = 1;

            #region steady state

            var A = model.LinearNumExpr();
            A.AddTerm(v["r1"], 1);
            A.AddTerm(v["r4"], 1);
            A.AddTerm(v["r2"], -1);
            model.Add(model.Eq(A, 0, "A"));

            var B = model.LinearNumExpr();
            B.AddTerm(v["r2"], 1);
            B.AddTerm(v["r3"], -1);
            B.AddTerm(v["r5"], -1);
            B.AddTerm(v["r11"], -1);
            model.Add(model.Eq(B, 0, "B"));

            var C = model.LinearNumExpr();
            C.AddTerm(v["r3"], 1);
            C.AddTerm(v["r4"], -1);
            model.Add(model.Eq(C, 0, "C"));

            //model.Add(model.Eq(v["r3"], v["r4"], "C"));
            var D = model.LinearNumExpr();
            D.AddTerm(v["r5"], 1);
            D.AddTerm(v["r6"], -1);
            D.AddTerm(v["r8"], -1);
            model.Add(model.Eq(D, 0, "D"));

            var E = model.LinearNumExpr();
            E.AddTerm(v["r6"], 1);
            E.AddTerm(v["r7"], -1);
            model.Add(model.Eq(E, 0, "E"));
            //model.Add(model.Eq(v["r6"], v["r7"], "E"));

            var F = model.LinearNumExpr();
            F.AddTerm(v["r8"], 1);
            F.AddTerm(v["r9"], -1);
            F.AddTerm(v["r12"], 1);
            F.AddTerm(v["r14"], -1);
            model.Add(model.Eq(F, 0, "F"));

            var G = model.LinearNumExpr();
            G.AddTerm(v["r7"], 1);
            G.AddTerm(v["r9"], 1);
            G.AddTerm(v["r10"], -1);
            model.Add(model.Eq(G, 0, "G"));

            var H = model.LinearNumExpr();
            H.AddTerm(v["r12"], -1);
            H.AddTerm(v["r11"], 1);
            H.AddTerm(v["r13"], 1);
            model.Add(model.Eq(H, 0, "H"));

            var I = model.LinearNumExpr();
            I.AddTerm(v["r13"], -1);
            I.AddTerm(v["r14"], 1);
            I.AddTerm(v["r15"], 1);
            I.AddTerm(v["r16"], 1);
            model.Add(model.Eq(I, 0, "I"));

            //model.Add(model.Eq(v["r15"], v["r18"], "J"));

            //model.Add(model.Eq(v["r16"], v["r19"], "K"));

            #endregion

            //for (var i = 1; i < 17; i++)
            //{
            //    if (i == 8) continue;
            //    //if gi==false then ri=0
            //    model.Add(model.IfThen(model.Eq(g[$"g{i}"], 0), model.Eq(v[$"r{i}"], 0)));
            //}

            model.Add(model.IfThen(model.Eq(g["g12"], 0), model.Eq(v["r12"], 0)));
            model.Add(model.IfThen(model.Eq(g["g6"], 0), model.Eq(v["r6"], 0)));

            var p = BooleanParser.Parse("g8 or g6 or g12", g, model);
            //model.Add(p.Constraints.ToArray());
            model.Add(model.IfThen(model.Eq(p.RootVar, 0), model.Eq(v["r8"], 0), "Gr8"));

            model.Add(model.Eq(p.RootVar, 0));

            //model.Add(model.Eq(g["g6"], 0));
            //model.Add(model.Eq(g["g8"], 1));

            //model.Add(model.IfThen(model.Eq(g["g6"], 1), model.Eq(g["g11"], 0)));
            //model.Add(model.IfThen(model.Eq(g["g2"], 1), model.Eq(g["g13"], 0)));
            //model.Add(model.IfThen(model.Eq(g["g3"], 1), model.Eq(g["g14"], 0)));
            //model.Add(model.IfThen(model.Eq(g["g8"], 1), model.Eq(g["g11"], 0)));
            Console.WriteLine(p.RootVar);
            model.AddObjective(ObjectiveSense.Maximize, v["r1"]);

            model.ExportModel("A:\\model2.lp");
            //model.EndModel();
            var results = new Dictionary<string, Tuple<string, string>>();
            foreach (var r in v)
            {
                var model2 = new Cplex();
                model2.ImportModel("A:\\model2.lp");

                GetValue2(model2, r, fva);
            }

            foreach (var tuple in fva)
            {
                Console.WriteLine("{0}\t[{1}\t{2}]", tuple.Key, tuple.Value.Item1, tuple.Value.Item2);
                //Console.WriteLine("{0}", results[tuple.Key].Item1);
                //Console.WriteLine("{0}", results[tuple.Key].Item2);
                //Console.WriteLine();
            }

            Console.ReadKey();
        }

        public static void TestGeneReg1()
        {
            var model = new Cplex();
            var v = new Dictionary<string, INumVar>();
            var g = new ConcurrentDictionary<string, INumVar>();
            var fva = new Dictionary<string, Tuple<float, float>>();

            for (var i = 1; i < 11; i++)
            {
                v[$"r{i}"] = model.NumVar(0, 100, NumVarType.Float, $"r{i}");
                g[$"g{i}"] = model.BoolVar($"g{i}");
                //Gr[$"r{i}"] = $"g{i}";
            }
            //v["r10"].LB = 1;

            #region steady state

            var A = model.LinearNumExpr();
            A.AddTerm(v["r1"], 1);
            A.AddTerm(v["r4"], 1);
            A.AddTerm(v["r2"], -1);
            model.Add(model.Eq(A, 0, "A"));

            var B = model.LinearNumExpr();
            B.AddTerm(v["r2"], 1);
            B.AddTerm(v["r3"], -1);
            B.AddTerm(v["r5"], -1);
            model.Add(model.Eq(B, 0, "B"));

            var C = model.LinearNumExpr();
            C.AddTerm(v["r3"], 1);
            C.AddTerm(v["r4"], -1);
            model.Add(model.Eq(C, 0, "C"));

            //model.Add(model.Eq(v["r3"], v["r4"], "C"));
            var D = model.LinearNumExpr();
            D.AddTerm(v["r5"], 1);
            D.AddTerm(v["r6"], -1);
            D.AddTerm(v["r8"], -1);
            model.Add(model.Eq(D, 0, "D"));

            var Y = model.LinearNumExpr();
            Y.AddTerm(v["r6"], 1);
            Y.AddTerm(v["r7"], -1);
            model.Add(model.Eq(Y, 0, "Y"));
            //model.Add(model.Eq(v["r6"], v["r7"], "E"));

            var F = model.LinearNumExpr();
            F.AddTerm(v["r8"], 1);
            F.AddTerm(v["r9"], -1);
            model.Add(model.Eq(F, 0, "F"));

            var G = model.LinearNumExpr();
            G.AddTerm(v["r7"], 1);
            G.AddTerm(v["r9"], 1);
            G.AddTerm(v["r10"], -1);
            model.Add(model.Eq(G, 0, "G"));

            #endregion

            //for (var i = 1; i < 11; i++)
            //{
            //    if (i == 8) continue;
            //    //if gi==false then ri=0
            //    model.Add(model.IfThen(model.Eq(g[$"g{i}"], 0), model.Eq(v[$"r{i}"], 0)));
            //}

            model.Add(model.IfThen(model.Eq(g["g6"], 0), model.Eq(v["r6"], 0)));

            var p = BooleanParser2.Parse("g8 or g6", g, model);
            //model.Add(p.Constraints.ToArray());
            model.Add(model.IfThen(model.Eq(p.RootVar, 0), model.Eq(v["r8"], 0)));

            model.Add(model.Eq(p.RootVar, 0));

            //model.Add(model.Eq(g["g2"], 0));
            //model.Add(model.Eq(g["g8"], 1));

            Console.WriteLine(p.RootVar);

            model.ExportModel("A:\\model2.lp");
            foreach (var r in v)
            {
                var model2 = new Cplex();
                model2.ImportModel("A:\\model2.lp");
                if (r.Key == "r4")
                    GetValue2(model2, r, fva);
            }

            foreach (var tuple in fva)
            {
                Console.WriteLine("{0}\t[{1}\t{2}]", tuple.Key, tuple.Value.Item1, tuple.Value.Item2);
                //Console.WriteLine("{0}", results[tuple.Key].Item1);
                //Console.WriteLine("{0}", results[tuple.Key].Item2);
                //Console.WriteLine();
            }

            Console.ReadKey();
        }

        public static void TestGeneReg2()
        {
            var model = new Cplex();
            var v = new Dictionary<string, INumVar>();
            var g = new ConcurrentDictionary<string, INumVar>();
            var fva = new Dictionary<string, Tuple<float, float>>();

            for (var i = 1; i < 7; i++)
            {
                v[$"r{i}"] = model.NumVar(0, 100, NumVarType.Float, $"r{i}");
                g[$"g{i}"] = model.BoolVar($"g{i}");
                //Gr[$"r{i}"] = $"g{i}";
            }
            //v["r10"].LB = 1;

            #region steady state

            var A = model.LinearNumExpr();
            A.AddTerm(v["r1"], 1);
            A.AddTerm(v["r4"], 1);
            A.AddTerm(v["r2"], -1);
            model.Add(model.Eq(A, 0, "A"));

            var B = model.LinearNumExpr();
            B.AddTerm(v["r2"], 1);
            B.AddTerm(v["r3"], -1);
            B.AddTerm(v["r5"], -1);
            model.Add(model.Eq(B, 0, "B"));

            var C = model.LinearNumExpr();
            C.AddTerm(v["r3"], 1);
            C.AddTerm(v["r4"], -1);
            model.Add(model.Eq(C, 0, "C"));

            //model.Add(model.Eq(v["r3"], v["r4"], "C"));
            var D = model.LinearNumExpr();
            D.AddTerm(v["r5"], 1);
            D.AddTerm(v["r6"], -1);
            //D.AddTerm(v["r7"], -1);
            model.Add(model.Eq(D, 0, "D"));

            #endregion

            //for (var i = 1; i < 11; i++)
            //{
            //    if (i == 8) continue;
            //    //if gi==false then ri=0
            //    model.Add(model.IfThen(model.Eq(g[$"g{i}"], 0), model.Eq(v[$"r{i}"], 0)));
            //}

            model.Add(model.IfThen(model.Eq(g["g3"], 0), model.Eq(v["r3"], 0)));

            var p = BooleanParser.Parse("g6 or g3", g, model);
            //var p = BooleanParser2.Parse("g7 or g6", g, model);

            //model.Add(p.Constraints.ToArray());
            model.Add(model.IfThen(model.Eq(p.RootVar, 0), model.Eq(v["r6"], 0), "Gr6"));

            model.Add(model.Eq(p.RootVar, 0));

            //model.Add(model.Eq(g["g6"], 0));
            //model.Add(model.Eq(g["g8"], 1));

            Console.WriteLine(p.RootVar);

            model.ExportModel("A:\\model2.lp");
            //model.EndModel();
            var results = new Dictionary<string, Tuple<string, string>>();
            foreach (var r in v)
            {
                var model2 = new Cplex();
                model2.ImportModel("A:\\model2.lp");

                GetValue2(model, r, fva);
            }

            foreach (var tuple in fva)
            {
                Console.WriteLine("{0}\t[{1}\t{2}]", tuple.Key, tuple.Value.Item1, tuple.Value.Item2);
                //Console.WriteLine("{0}", results[tuple.Key].Item1);
                //Console.WriteLine("{0}", results[tuple.Key].Item2);
                //Console.WriteLine();
            }

            Console.ReadKey();
        }

        public static void TestGeneReg3()
        {

            var model = new Cplex();
            var v = new ConcurrentDictionary<string, INumVar>();
            var g = new ConcurrentDictionary<string, INumVar>();
            var fva = new Dictionary<string, Tuple<float, float>>();

            for (var i = 1; i < 8; i++)
            {
                v[$"r{i}"] = model.NumVar(0, 100, NumVarType.Float, $"r{i}");
                g[$"g{i}"] = model.BoolVar($"g{i}");//model.NumVar(0, 1, NumVarType.Int, $"g{i}");//
            }
            //g[$"g2"] = model.NumVar(0, 1, NumVarType.Int, $"g2");
            //g[$"g5"] = model.NumVar(0, 1, NumVarType.Int, $"g5");

            var A = model.LinearNumExpr();
            A.AddTerm(v["r1"], 1);
            A.AddTerm(v["r4"], 1);
            A.AddTerm(v["r2"], -1);
            model.Add(model.Eq(A, 0, "A"));

            var B = model.LinearNumExpr();
            B.AddTerm(v["r2"], 1);
            B.AddTerm(v["r3"], -1);
            B.AddTerm(v["r5"], -1);
            model.Add(model.Eq(B, 0, "B"));

            var C = model.LinearNumExpr();
            C.AddTerm(v["r3"], 1);
            C.AddTerm(v["r4"], -1);
            model.Add(model.Eq(C, 0, "C"));

            var D = model.LinearNumExpr();
            D.AddTerm(v["r6"], 1);
            D.AddTerm(v["r7"], -1);
            model.Add(model.Eq(D, 0, "D"));

            //for (var i = 1; i < 6; i++)
            //{
            //    if (i == 2) continue;
            //    //if gi==false then ri=0
            //    model.Add(model.IfThen(model.Eq(g[$"g{i}"], 0), model.Eq(v[$"r{i}"], 0)));
            //}
            model.Add(model.IfThen(model.Eq(g["g2"], 0), model.Eq(v["r2"], 0)));
            model.Add(model.IfThen(model.Eq(g["g3"], 0), model.Eq(v["r3"], 0)));
            model.Add(model.IfThen(model.Eq(g["g4"], 0), model.Eq(v["r4"], 0)));

            //var p1 = BooleanParser2.Parse("g1 or g4", g, model);
            var p1 = BooleanParser2.Parse("g1 or g4", g, model);
            model.Add(model.IfThen(model.Eq(p1.RootVar, 0), model.Eq(v["r1"], 0)));

            //var p2 = BooleanParser2.Parse("g3 and g4", g, model);
            var p2 = BooleanParser2.Parse("g4 and g3", g, model);
            model.Add(model.IfThen(model.Eq(p2.RootVar, 0), model.Eq(v["r3"], 0)));

            //model.Add(model.IfThen(model.Eq(g["g5"], 1), model.Eq(g["g1"], 0), "g5_g4"));

            //model.Add(model.Eq(g["g5"], 1));
            //model.Add(model.Eq(g["g2"], 0));
            //model.Add(model.Eq(g["g3"], 1));

            model.Add(model.Eq(p1.RootVar, 0));
            model.Add(model.Eq(p2.RootVar, 1));

            model.ExportModel("A:\\model2.lp");
            var results = new Dictionary<string, Tuple<string, string>>();
            foreach (var r in v)
            {
                var model2 = new Cplex();
                model2.ImportModel("A:\\model2.lp");

                GetValue2(model, r, fva);
            }

            foreach (var tuple in fva)
            {
                Console.WriteLine("{0}\t[{1}\t{2}]", tuple.Key, tuple.Value.Item1, tuple.Value.Item2);
                //Console.WriteLine("{0}", results[tuple.Key].Item1);
                //Console.WriteLine("{0}", results[tuple.Key].Item2);
                //Console.WriteLine();
            }

            Console.ReadKey();
        }

        public static void TestGeneReg2MSF()
        {
            var context = SolverContext.GetContext();
            var model = context.CreateModel();

            var v = new Dictionary<string, Decision>();
            var g = new ConcurrentDictionary<string, Decision>();
            var fva = new Dictionary<string, Tuple<float, float>>();

            for (var i = 1; i < 8; i++)
            {
                v[$"r{i}"] = new Decision(Domain.Real, $"r{i}");
                g[$"g{i}"] = new Decision(Domain.Boolean, $"g{i}");
            }

            model.AddDecisions(v.Values.ToArray());
            model.AddDecisions(g.Values.ToArray());

            model.AddConstraint("A", v["r2"] == v["r1"] + v["r4"]);
            model.AddConstraint("B", v["r2"] == v["r3"] + v["r5"]);
            model.AddConstraint("C", v["r3"] == v["r4"]);
            model.AddConstraint("D", v["r5"] == v["r6"] + v["r7"]);


            model.AddConstraint("Gr3", "r3 <= If[g3 == 0, 0, 100]");
            model.AddConstraint("Gr6", "r6 <= If[Not[Or[g3, g6]], 0, 100]");
            //model.Add(model.Eq(p.RootVar, 0));
            //model.Add(model.Eq(g["g6"], 0));
            //model.Add(model.Eq(g["g8"], 1));
            //model.ExportModel("A:\\model2.lp");

            foreach (var r in v)
                GetValue3(context, model, r, fva);

            foreach (var tuple in fva)
                Console.WriteLine("{0}\t[{1}\t{2}]", tuple.Key, tuple.Value.Item1, tuple.Value.Item2);

            Console.ReadKey();
        }

        private static void GetValue3(SolverContext context, Model model, KeyValuePair<string, Decision> r, Dictionary<string, Tuple<float, float>> fva)
        {
            Directive dir = new MixedIntegerProgrammingDirective();

            var maxg = model.AddGoal("max", GoalKind.Maximize, r.Value);
            var solution = context.Solve(dir);
            var max = (float)r.Value.GetDouble();//solution.Decisions.Where(decision => decision.Name == r.Key).Select(decision => (float)decision.GetDouble());

            var report = solution.GetReport();
            Console.Write("{0}", report);
            model.RemoveGoal(maxg);

            var ming = model.AddGoal("min", GoalKind.Minimize, r.Value);
            solution = context.Solve(dir);
            var min = (float)r.Value.GetDouble();//solution.Decisions.Where(decision => decision.Name == r.Key).Select(decision => (float)decision.GetDouble());

            report = solution.GetReport();
            Console.Write("{0}", report);
            fva[r.Key] = Tuple.Create(min, max);
        }

        public static void cplexMSF()
        {
            SolverContext context = SolverContext.GetContext();
            Model model = context.CreateModel();

            Decision x1 = new Decision(Domain.RealRange(0, 40), "x1");
            Decision x2 = new Decision(Domain.RealRange(0, Rational.PositiveInfinity), "x2");
            Decision x3 = new Decision(Domain.RealRange(0, Rational.PositiveInfinity), "x3");
            Decision x4 = new Decision(Domain.IntegerRange(2, 3), "x4");

            model.AddDecisions(x1, x2, x3, x4);

            model.AddConstraint("Row1", -x1 + x2 + x3 + 10 * x4 <= 20);
            model.AddConstraint("Row2", x1 - 3 * x2 + x3 <= 30);
            model.AddConstraint("Row3", x2 - 3.5 * x4 == 0);

            Goal goal = model.AddGoal("Goal", GoalKind.Maximize, x1 + 2 * x2 + 3 * x3 + x4);

            // Turn on CPLEX log
            Directive dir = new MixedIntegerProgrammingDirective();
            //CplexDirective cplexDirective = new CplexDirective();

            Solution solution = context.Solve(dir);
            Report report = solution.GetReport();
            Console.WriteLine("x: {0}, {1}, {2}", x1, x2, x3, x4);
            Console.Write("{0}", report);
            context.ClearModel();

            Console.ReadKey();
        }

        private static void GetValue2(Cplex model, KeyValuePair<string, INumVar> r, Dictionary<string, Tuple<float, float>> fva)
        {
            var exp = model.LinearNumExpr();
            exp.AddTerm(r.Value, 1);

            model.Remove(model.GetObjective());
            model.AddObjective(ObjectiveSense.Maximize, exp, "fobj");
            model.Solve();
            model.WriteSolution($"A:\\model2x{r.Key}.sol");

            var max = (float)model.GetValues(new[] { r.Value })[0];

            exp = model.LinearNumExpr();
            exp.AddTerm(r.Value, 1);

            model.Remove(model.GetObjective());
            model.AddObjective(ObjectiveSense.Minimize, exp, "fobj");
            model.Solve();
            model.WriteSolution($"A:\\model2n{r.Key}.sol");

            var min = (float)model.GetValues(new[] { r.Value })[0];
            fva[r.Key] = Tuple.Create(min, max);
        }
        private static Dictionary<string, double> GetSols(Cplex model)
        {
            var m = model.GetLPMatrixEnumerator();
            m.MoveNext();
            var mat = (CpxLPMatrix)m.Current;

            var sols = model.GetValues(mat.NumVars);

            return mat.NumVars.Select((t, i) => new Tuple<string, double>(t.Name, sols[i])).ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        public static void TestLogicalOperators()
        {
            var model = new Cplex { Name = "FBA" };
            var x1 = model.NumVar(-10, 10, "y1");
            var x2 = model.NumVar(-10, 10, "y2");
            var x3 = model.NumVar(-10, 10, "y3");
            var x4 = model.NumVar(-10, 10, "y4");

            var exp1 = model.LinearNumExpr();
            exp1.AddTerm(x1, 1);

            var exp2ub = model.LinearNumExpr();
            exp2ub.AddTerm(x2, (2.0 / 3) * 1.1);
            var upBound = model.Le(model.Abs(exp1), exp2ub);

            var exp2lb = model.LinearNumExpr();
            exp2lb.AddTerm(x2, (2.0 / 3) * 0.9);
            var lowBound = model.Ge(exp1, exp2lb);


            var and = model.And();
            and.Add(new[] { lowBound, upBound });

            var zeroLeft = model.Eq(exp1, 0);
            var zeroRight = model.Eq(exp2lb, 0);

            var or = model.Or();

            or.Add(and);
            or.Add(zeroRight);
            or.Add(zeroLeft);

            //model.Add(or);
            model.AddEq(x1, -2);
            model.AddEq(model.Abs(exp1), exp2ub);

            model.AddMinimize(x2);

            var solved = model.Solve();
            model.ExportModel("C:/model2/hi.lp");

            if (!solved)
            {
                Console.WriteLine("infeasible");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("solved");

            Console.WriteLine("x1: " + model.GetValue(x1));
            Console.WriteLine("x2: " + model.GetValue(x2));
            //Console.WriteLine("x3: " + model.GetValue(x3));
            //Console.WriteLine("x4: " + model.GetValue(x4));

            Console.ReadKey();
        }

        public static void TestLogic()
        {
            var model = new Cplex { Name = "Logiczzz" };
            var y1 = model.Eq(model.BoolVar(), 1);
            var y2 = model.Eq(model.BoolVar(), 1);

            // oring
            var or = model.Or();
            or.Add(y1);
            or.Add(y2);

            var y3 = model.Eq(model.BoolVar(), 1);

            // anding
            var and = model.And();
            and.Add(or);
            and.Add(y3);

            model.AddEq(and, 0);

            model.AddMaximize(y3);

            var solved = model.Solve();
            model.ExportModel("C:/model2/hi.lp");

            if (!solved)
            {
                Console.WriteLine("infeasible");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("solved");

            Console.WriteLine("y1: " + model.GetValue(y1));
            Console.WriteLine("y2: " + model.GetValue(y2));
            Console.WriteLine("y3: " + model.GetValue(y3));
            Console.ReadKey();

        }
    }
}
