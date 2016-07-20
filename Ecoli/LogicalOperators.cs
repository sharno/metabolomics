using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILOG.CPLEX;

namespace Ecoli
{
    class LogicalOperators
    {
        public static void TestLogicalOperators()
        {
            var model = new Cplex { Name = "FBA" };
            var x0 = model.NumVar(-10, 10, "y0");
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
            and.Add(new [] {lowBound, upBound});

            var zeroLeft = model.Eq(exp1, 0);
            var zeroRight = model.Eq(exp2lb, 0);

            var or = model.Or();
            
            or.Add(and);
            or.Add(zeroRight);
            or.Add(zeroLeft);

            model.Add(or);


            // y1 = -2
            //model.AddEq(x1, -2);

            // y0 = -10
            model.AddEq(x0, -10);

            // y0 + y2 = 0
            var expr2 = model.LinearNumExpr();
            expr2.AddTerm(x0, 1);
            expr2.AddTerm(x2, 1);
            model.AddEq(expr2, 0);

            // y2 - y4 = 0
            var expr3 = model.LinearNumExpr();
            expr3.AddTerm(x2, 1);
            expr3.AddTerm(x4, -1);
            model.AddEq(expr3, 0);

            // y3 = y1 + y2
            //var expr = model.LinearNumExpr();
            //expr.AddTerm(x1, 1);
            //expr.AddTerm(x2, 1);
            //model.AddEq(x3, expr);

            model.AddMaximize(x4);

            var solved = model.Solve();
            model.ExportModel("C:/model2/hi.lp");

            if (!solved)
            {
                Console.WriteLine("infeasible");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("solved");
            Console.WriteLine("x0: " + model.GetValue(x0));
            Console.WriteLine("x1: " + model.GetValue(x1));
            Console.WriteLine("x2: " + model.GetValue(x2));
            //Console.WriteLine("x3: " + model.GetValue(x3));
            Console.WriteLine("x4: " + model.GetValue(x4));

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
