using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecoli
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new TheAlgorithm();
            p.Fba.RemoveConstraints = false;
            p.Start();
            do
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("\n************** ITERATION {0} *************** ", p.Iteration);
                p.Step();
                //if (p.Iteration >= 8)
                if (!p.IsFeasable)
                {
                    Console.ReadKey();
                }
            }
            while (true);
        }
    }
}
