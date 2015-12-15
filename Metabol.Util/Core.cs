namespace Metabol.Util
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    public class Core
    {
        //public const string Product = "Product";
        //public const string Reactant = "Reactant";
        //public const string Modifier = "Modifier";

        public static string Dir = ConfigurationManager.AppSettings["modelOutput"];//"model2/";//

        //public static string AllReactionFile = ConfigurationManager.AppSettings["allReaction"]; //"allReaction.csv";//
        //public static string AllStoichiometryFile = ConfigurationManager.AppSettings["allStoichiometry"];// "allStoichiometry.csv";//
        //public static string SelectedMetaFile = ConfigurationManager.AppSettings["selection"];// "selected.csv";//
        //public static string BlockedReactionsFile = ConfigurationManager.AppSettings["blockedReactions"];
        static Core()
        {
            //var list = (from species in ServerSpecies.AllSpecies()
            //            let reactant = species.getAllReactions(Util.Reactant).Length
            //            let product = species.getAllReactions(Util.Product).Length
            //            select string.Format("{0},{1},{2}", species.ID, reactant, product)).ToList();
            //File.AppendAllLines(Util.AllReactionFile, list);

            //var list = (from s in ServerSpecies.AllSpecies()
            //        let rs = ServerReactionSpecies.GetAllReactionsSpeciesForOneSpecies(s.ID)
            //        let rsum = rs.Where(r => r.RoleId == ReactantId).Sum(e => e.Stoichiometry)
            //        let psum = rs.Where(r => r.RoleId == ProductId).Sum(e => e.Stoichiometry)
            //        select string.Format("{0},{1},{2}", s.ID, rsum, psum)
            //            ).ToList();
            //File.AppendAllLines(AllStoichiometryFile, list);
            Init();

            //Directory.CreateDirectory(Dir);
            //var lines = File.ReadAllLines(AllReactionFile);
            //foreach (var result in lines.Select(s => s.Split(',')))
            //    AllReactionCache[Guid.Parse(result[0])] = Tuple.Create(int.Parse(result[1]), int.Parse(result[2]));

            //lines = File.ReadAllLines(AllStoichiometryFile);
            //foreach (var result in lines.Select(s => s.Split(',')))
            //    AllStoichiometryCache[Guid.Parse(result[0])] = Tuple.Create(double.Parse(result[1]), double.Parse(result[2]));

        }

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

        public static void Init()
        {
            try
            {
                Directory.CreateDirectory(Dir);
                //var lines = File.ReadAllLines(AllReactionFile);
                //foreach (var result in lines.Select(s => s.Split(',')))
                //    AllReactionCache2[Guid.Parse(result[0])] =
                //        new { Consumers = int.Parse(result[1]), Producers = int.Parse(result[2]) };
                //Tuple.Create(
                //Int32.Parse(result[1]),
                //Int32.Parse(result[2]));

                //lines = File.ReadAllLines(AllStoichiometryFile);
                //foreach (var result in lines.Select(s => s.Split(',')))
                //    AllStoichiometryCache2[Guid.Parse(result[0])] =
                //         new { Consumers = double.Parse(result[1]), Producers = double.Parse(result[2]) };
                //Tuple.Create(
                //Double.Parse(result[1]),
                //Double.Parse(result[2]));
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        //public static void SaveAsDgs(HyperGraph.Node m2, HyperGraph sm)
        //{
        //    var file = string.Format("{0}{1}graph.dgs", Dir, sm.LastLevel);
        //    var maxLevel = sm.LastLevel;// sm.Nodes.Max(n => n.Flux.Level);//Math.Max(sm.Nodes.Max(n => n.Flux.Level), sm.Edges.Max(e => e.Flux.Level));

        //    var lines = new List<string> { "DGS004", "\"Metabolic Network\" 0 0", "#Nodes", m2.ToDgs(NodeType.Selected) };

        //    foreach (var node in sm.Nodes.Values)
        //    {
        //        if (node.Id == m2.Id)
        //            //type = NodeType.Selected;
        //            continue;

        //        var type = NodeType.None;
        //        //if (!node.IsCommon)
        //        if (node.Level == maxLevel && node.IsBorder)
        //            type = NodeType.NewBorder;
        //        //else if (node.Level == maxLevel)
        //        //    type = NodeType.New;
        //        else if (node.IsBorder)
        //            type = NodeType.Border;

        //        lines.Add(node.ToDgs(type));
        //    }
        //    if (sm.HasCycle)
        //        Console.WriteLine();
        //    lines.Add("#Hyperedges");
        //    foreach (var edge in sm.Edges.Values)
        //    {
        //        var type = EdgeType.None;
        //        if (edge.Level == maxLevel)
        //            type = EdgeType.New;

        //        lines.Add(edge.ToDgs(type, sm));
        //    }
        //    File.AppendAllLines(file, lines);
        //}

        public static void SaveAsDgs(HyperGraph.Node mi, HyperGraph graph, string dir)
        {
            var file = dir + graph.LastLevel + "graph.dgs";
            var maxLevel = graph.LastLevel;

            var lines = new List<string> { "DGS004", "\"Metabolic Network\" 0 0", "#Nodes" };
            lines.Add(mi.ToDgs(NodeType.Selected));
            lines.AddRange(graph.Nodes.Values
                .Where(n => n.Id != mi.Id)
                .Select(node => new { node, type = node.IsBorder ? NodeType.Border : NodeType.None })
                .Select(@t => @t.node.ToDgs(@t.type)));

            lines.Add("#Hyperedges");
            foreach (var edge in graph.Edges.Values)
            {
                var type = EdgeType.None;
                if (edge.Level == maxLevel)
                    type = EdgeType.New;

                lines.Add(edge.ToDgs(type));
            }

            File.AppendAllLines(file, lines);
        }

        public static void SaveAsDgsTest(HyperGraph.Node mi, HyperGraph graph, string dir)
        {
            var file = dir + graph.LastLevel + "graph.dgs";
            var maxLevel = graph.LastLevel;

            var lines = new List<string> { "DGS004", "\"Metabolic Network\" 0 0", "#Nodes" };
            lines.Add(mi.ToDgs(NodeType.Selected));
            lines.AddRange(graph.Nodes.Values
                .Where(n => n.Id != mi.Id)
                .Select(node => new { node, type = NodeType.None })
                .Select(@t => @t.node.ToDgs(@t.type)));

            lines.Add("#Hyperedges");
            foreach (var edge in graph.Edges.Values.Union(graph.Cycles.Values))
            {
                var type = EdgeType.None;
                if (edge.Level == maxLevel)
                    type = EdgeType.New;

                lines.Add(edge.ToDgs(type));
            }

            File.AppendAllLines(file, lines);
        }

        public static double PowerLaw(double n)
        {
            return 1.0 / Math.Pow(1 - new Random().NextDouble(), 1.0 / (n + 1));
        }
    }
}
