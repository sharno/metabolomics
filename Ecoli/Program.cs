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
            p.Start();
            do
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("\n************** ITERATION {0} *************** ", p.Sm.Step);
                p.Step();
                if (!p.IsFeasable)
                {
                    Console.ReadKey();
                }
            }
            while (true);
        }
    }
}
