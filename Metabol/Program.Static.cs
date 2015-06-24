using System;
using System.Linq;
using Microsoft.SolverFoundation.Services;
using PathwaysLib.ServerObjects;

namespace Metabol
{
    public class Program
    {
        /// <summary>
        /// The selected meta.
        /// </summary>
        private static void SelectedMeta()
        {
            //var rand = new Random((int)DateTime.UtcNow.ToBinary());
            //var guid = new Guid[TheAlgorithm.AllReactionCache.Count];
            //TheAlgorithm.AllReactionCache.Keys.CopyTo(guid, 0);
            //Array.Sort(guid);
            //var sguid = new Guid[1000];
            //Array.Copy(guid, 0, sguid, 0, 1000);
            //var lines = sguid.ToList().Select(s => s.ToString() + ";" + (rand.NextDouble() >= 0.5 ? 1 : -1));
            //File.AppendAllLines(TheAlgorithm.SelectedMetaFile, lines);
        }

        #region main

        /// <summary>
        /// The main.
        /// </summary>
        internal static void Main()
        {
            var p = InitProgram();
            p.Start();
            do
            {
                var it = p.Step(1).First();
                //if (it.Id>=4)
                //{
                //    p.Fba.RemoveConstraints = true;
                //    Console.ReadKey();
                //}
            }
            while (true); //count != sm.Edges.Count
        }

        /// <summary>
        /// The init program.
        /// </summary>
        /// <returns>
        /// The <see cref="Program"/>.
        /// </returns>
        private static TheAlgorithm InitProgram()
        {
            var user = new User(Guid.NewGuid().ToString());
            //var d = DateTime.Now.TimeOfDay.Ticks;
            //user.Worker.file1 = $"C:\\Users\\f\\Desktop\\1bench{d}.txt";
            //user.Worker.file2 = $"C:\\Users\\f\\Desktop\\2bench{d}.txt";
            return user.Worker;
        }

        #endregion

        #region test

        internal static void Main3()
        {
            string[] zn = { "M_h2o_e", "M_HC01441_e", "M_h_g", "M_h_e", "M_udpgal_g", "M_glc_D_e", "M_udp_g", "M_gal_e", "M_glc_D_c", "M_pi_e" };
            var zlist = (from s in zn select ServerSpecies.AllSpeciesByName(s) into spec where spec.Length > 0 select spec[0]).ToList();
            zlist.Sort((s1, s2) => Util.TotalReactions(s1.ID).CompareTo(Util.TotalReactions(s2.ID)));
            //var nodes = zlist.Select(s => HGraph.Node.Create(s.ID, s.SbmlId)).ToList();
            zlist.ForEach(s => Console.WriteLine("{0}: {1}", s.SbmlId, Util.TotalReactions(s.ID)));
            //var node = Util.LonelyMetabolite(nodes);
            //Console.WriteLine(node);
            Console.ReadKey();

        }

        internal static void Main4()
        {
            var context = SolverContext.GetContext();
            context.LoadModel(FileFormat.OML, @"C:\Users\f\Desktop\model2\130794925373060005model.txt");
            var solution = context.Solve(new SimplexDirective());
            var report = solution.GetReport();
            Console.WriteLine(report);
            Console.ReadKey();
            //Console.WriteLine(Util.Dir);
        }

        #endregion
    }
}
