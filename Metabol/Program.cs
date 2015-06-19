using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PathwaysLib.ServerObjects;

namespace Metabol
{
    public partial class Program
    {
        internal readonly HGraph Sm = new HGraph();
        //internal readonly Stopwatch Timer = new Stopwatch();

        //internal string file1;
        //internal string file2;
        internal int Iteration = 1;
        private int IterationId => Iteration++;
        private bool init;
        public Dictionary<Guid, int> Z = new Dictionary<Guid, int>();

        public IEnumerable<Iteration> Step(int step)
        {
            if (!init) yield break;

            for (var i = 0; i < step; i++)
            {
                //Timer.Reset();
                //Timer.Start();

                //steps 4,5
                var fba = Util.ApplyFba(Sm, Z, IterationId);
                //while (iteration.Fba == 0)
                //Console.ReadKey();

                //Task.Run(delegate
                //{
                //    //File.AppendAllText(file1, $"=========== {iteration.Id}. iteration ===========\n");
                //    //File.AppendAllText(file1, $"FBA feasable: {iteration.Fba}  time:{Util.Fba.LastRuntime}\n");
                //    //File.AppendAllText(file1, $"Nodes: {sm.Nodes.Count}  BorderM:{iteration.BorderMCount}  Edges: {sm.Edges.Count}\n");
                //    File.AppendAllText(file2,
                //        $"{sm.Nodes.Count},{iteration.BorderMCount},{sm.Edges.Count},{timer.ElapsedMilliseconds * 1.0 / 1000.0}\n");
                //});


                //8. Let m’ be a border metabolite in S(m) involved in the smallest total number of reactions.
                var borderm = Util.GetBorderMetabolites(Sm);
                var m2 = Util.LonelyMetabolite(borderm);
                Util.SaveAsDgs(m2, Sm, Util.Fba.Label);
                Sm.NextStep();

                //Extend S(m) with m’ and its reactions from M.
                var ex = Util.ExtendGraph(m2.ToSpecies, Sm);
                //ex.Wait(int.MaxValue);

                //Remove the exchange reaction that was introduced for m’ in step 4.
                //Add a constraint that total net flux of reactions of m’ should
                //be equal to those of the removed flux exchange reaction.
                Util.RemoveExchangeReaction(Sm, m2);

                //Go to step 4 to add exchange fluxes for the new border metabolites. 
                //If S(m) cannot be extended, then go to step 3.
                yield return fba;
            }
        }

        public void Start2()
        {
            if (init) return;

            var strCon = ConfigurationManager.AppSettings["dbConnectString"];
            DBWrapper.Instance = new DBWrapper(strCon);
            //var reconId = Guid.Parse("c7b42b40-ccd9-42f3-b6bd-9a4111fcbec5");
            //1. Among a user-provided set of observed metabolite changes Z,
            //  let m be the metabolite with the least total number of producer and consumer reactions in the respective metabolic network M
            var m = Util.CachedS(Z.Keys.OrderBy(Util.TotalReactions).First());

            //2. Let S(m) be a subnetwork of the whole metabolic network M. 
            //Initialize S(m) so that iteration contains only m
            Sm.AddNode(m.ID, m.SbmlId);
            //HGraph.Step++;
            //3. Extend S(m) with a subset K of m’s consumers and producers such that K has not been used before to extend the current subnetwork. 
            //HashSet<ServerSpecies> K = new HashSet<ServerSpecies>();
            var ex = Util.ExtendGraph(m, Sm);
            //ex.Wait(int.MaxValue);

            ////If there is no such qualifying subset K, then record the current hypothesis and exit
            //if (sm.Edges.Count == 0)
            //{
            //    Environment.Exit(1);
            //}

            init = true;
        }

        public void Start()
        {
            if (init) return;

            var strCon = ConfigurationManager.AppSettings["dbConnectString"];
            DBWrapper.Instance = new DBWrapper(strCon);
            //1. Among a user-provided set of observed metabolite changes Z,
            var zn = File.ReadAllLines(Util.SelectedMetaFile).Select(s => s.Split(';'));

            foreach (var s in zn)
                Z[Guid.Parse(s[0])] = int.Parse(s[1]);

            //  let m be the metabolite with the least total number of producer and consumer reactions in the respective metabolic network M
            var m = Util.CachedS(Z.Keys
                .Where(guid => Util.AllReactionCache[guid].Item1 + Util.AllReactionCache[guid].Item2 > 0)
                .OrderBy(s => Util.AllReactionCache[s].Item1 + Util.AllReactionCache[s].Item2).First()
                );

            //2. Let S(m) be a subnetwork of the whole metabolic network M. 
            //Initialize S(m) so that iteration contains only m
            Sm.AddNode(m.ID, m.SbmlId);

            //3. Extend S(m) with a subset K of m’s consumers and producers such that K has not been used before to extend the current subnetwork. 
            //HashSet<ServerSpecies> K = new HashSet<ServerSpecies>();
            var ex = Util.ExtendGraph(m, Sm);
            //ex.Wait(int.MaxValue);
            //Util.SaveAsDgs(sm.Nodes[m.ID], sm, "start");

            ////If there is no such qualifying subset K, then record the current hypothesis and exit
            //if (sm.Edges.Count == 0)
            //{
            //    Environment.Exit(1);
            //}

            init = true;
        }

        public void Stop()
        {
            init = false;
            Sm.Clear();
            Z.Clear();
            //Util.ClearCache();
        }
    }
}