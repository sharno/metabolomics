﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SolverFoundation.Services;
using PathwaysLib.ServerObjects;

namespace Metabol
{
    public partial class Program
    {
      
        private static void SelectedMeta()
        {
            var rand = new Random((int)DateTime.UtcNow.ToBinary());
            var guid = new Guid[Util.AllReactionCache.Count];
            Util.AllReactionCache.Keys.CopyTo(guid, 0);
            Array.Sort(guid);
            var sguid = new Guid[1000];
            Array.Copy(guid, 0, sguid, 0, 1000);
            var lines = sguid.ToList().Select(s => s.ToString() + ";" + (rand.NextDouble() >= 0.5 ? 1 : -1));
            File.AppendAllLines(Util.SelectedMetaFile, lines);
        }

        #region main
        internal static void Main1()
        {
            var p = InitProgram();
            p.Start();
            do
            {
                var it = p.Step(1).First();
                if (it.Fba == 0)
                {
                    Console.ReadKey();
                }
            } while (true); //count != sm.Edges.Count
        }

        private static Program InitProgram()
        {
            var user = new User(Guid.NewGuid().ToString());
            var d = DateTime.Now.TimeOfDay.Ticks;
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

        internal static void Main()
        {
            //var context = SolverContext.GetContext();
            //context.LoadModel(FileFormat.OML, @"C:\Users\f\Desktop\model2\130790525917374819model.txt");
            //var solution = context.Solve(new SimplexDirective());
            //var report = solution.GetReport();
            //Console.WriteLine(report);
            //Console.ReadKey();
            Console.WriteLine(Util.Dir);
        }

        #endregion
    }
}
