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
            LogicalOperators.ExportDetailsOfCycle(Guid.Parse("889f9316-4417-4fe3-8bbf-a551bd0de2d1"));
            return;

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
