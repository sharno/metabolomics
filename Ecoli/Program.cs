using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecoli
{
    class Program
    {
        static void Main(string[] args)
        {
            //LogicalOperators.ExportDetailsOfCycle(Guid.Parse("81724e05-168b-468b-9cd6-928a4c9ae854"));
            //return;

            Console.BufferHeight = Int16.MaxValue - 1;
            var p = new TheAlgorithm();
            p.Start();
            do
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("\n************** ITERATION {0} *************** ", p.Sm.Step);
                var iterationModel = p.Step();
                if (iterationModel.Fba == 0)
                {
                    Console.ReadKey();
                    return;
                }
            }
            while (true);
        }
    }
}
