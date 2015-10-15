namespace Metabol
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;

    using PathwaysLib.ServerObjects;

    public class Util
    {
        private static readonly MemoryCache CacheSpecieses = new MemoryCache("species");
        private static readonly MemoryCache CacheReactions = new MemoryCache("reactions");
        private static readonly MemoryCache CacheReactionsSpecies = new MemoryCache("reactionspecies");
        private static readonly MemoryCache CacheFba = new MemoryCache("fba");


        //private static readonly Dictionary<Guid, Tuple<int, int>> AllReactionCache = new Dictionary<Guid, Tuple<int, int>>();
        private static readonly Dictionary<Guid, dynamic> AllReactionCache2 = new Dictionary<Guid, dynamic>();

        //public static readonly Dictionary<Guid, Tuple<double, double>> AllStoichiometryCache = new Dictionary<Guid, Tuple<double, double>>();
        public static readonly Dictionary<Guid, dynamic> AllStoichiometryCache2 = new Dictionary<Guid, dynamic>();

        //public static readonly Dictionary<string, Metabolite> Meta = new Dictionary<string, Metabolite>();
        public const string Product = "Product";
        public const string Reactant = "Reactant";
        public const string Modifier = "Modifier";
        public const short ProductId = 2;
        public const short ReactantId = 1;
        public const short ModifierId = 3;


        public static string Dir = ConfigurationManager.AppSettings["modelOutput"];//"model2/";//

        public static string AllReactionFile = ConfigurationManager.AppSettings["allReaction"]; //"allReaction.csv";//
        public static string AllStoichiometryFile = ConfigurationManager.AppSettings["allStoichiometry"];// "allStoichiometry.csv";//
        public static string SelectedMetaFile = ConfigurationManager.AppSettings["selection"];// "selected.csv";//
        public static string BlockedReactionsFile = ConfigurationManager.AppSettings["blockedReactions"];
        static Util()
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

        public static HashSet<ServerReaction> GetAllReaction(ServerSpecies m, string role)
        {
            return new HashSet<ServerReaction>(m.getAllReactions(role));
        }

        public static int GetReactionCountSum(Guid id)
        {
            return InvolvedReactionCount(id).Consumers + InvolvedReactionCount(id).Producers;
        }

        public static dynamic GetReactionCount(Guid id)
        {
            return InvolvedReactionCount(id);
        }

        public static dynamic GetStoichiometry(Guid id)
        {
            return InvolvedReactionStoch(id);
        }

        public static void Init()
        {
            try
            {
                Directory.CreateDirectory(Dir);
                var lines = File.ReadAllLines(AllReactionFile);
                foreach (var result in lines.Select(s => s.Split(',')))
                    AllReactionCache2[Guid.Parse(result[0])] =
                        new { Consumers = int.Parse(result[1]), Producers = int.Parse(result[2]) };
                //Tuple.Create(
                //Int32.Parse(result[1]),
                //Int32.Parse(result[2]));

                lines = File.ReadAllLines(AllStoichiometryFile);
                foreach (var result in lines.Select(s => s.Split(',')))
                    AllStoichiometryCache2[Guid.Parse(result[0])] =
                         new { Consumers = double.Parse(result[1]), Producers = double.Parse(result[2]) };
                //Tuple.Create(
                //Double.Parse(result[1]),
                //Double.Parse(result[2]));
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        private static dynamic InvolvedReactionCount(Guid id)
        {
            if (AllReactionCache2.ContainsKey(id)) return AllReactionCache2[id];
            var species = ServerSpecies.Load(id);
            var product = species.getAllReactions(Product).Length;
            var reactant = species.getAllReactions(Reactant).Length;
            AllReactionCache2[id] = new { Consumers = reactant, Producers = product }; //Tuple.Create(reactant, product);
            return AllReactionCache2[id];
        }

        private static dynamic InvolvedReactionStoch(Guid id)
        {
            if (AllStoichiometryCache2.ContainsKey(id)) return AllStoichiometryCache2[id];
            var rs = ServerReactionSpecies.GetAllReactionsSpeciesForOneSpecies(id);
            var rsum = rs.Where(r => r.RoleId == ReactantId).Sum(e => e.Stoichiometry);
            var psum = rs.Where(r => r.RoleId == ProductId).Sum(e => e.Stoichiometry);
            AllStoichiometryCache2[id] = new { Consumers = rsum, Producers = psum }; //Tuple.Create(rsum, psum);
            return AllStoichiometryCache2[id];
        }

        public static TheAlgorithm CachedFba(string id)
        {
            if (CacheFba.Contains(id))
                return (TheAlgorithm)CacheFba.Get(id);

            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(6) };
            var fba = new TheAlgorithm();
            CacheReactions.Add(id, fba, policy);
            return fba;
        }

        public static ServerReaction CachedR(Guid id)
        {
            if (CacheReactions.Contains(id.ToString()))
                return (ServerReaction)CacheReactions.Get(id.ToString());

            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(6) };
            var ss = ServerReaction.Load(id);
            CacheReactions.Add(id.ToString(), ss, policy);
            return ss;
        }

        public static ServerSpecies CachedS(Guid id)
        {
            if (CacheSpecieses.Contains(id.ToString()))
                return (ServerSpecies)CacheSpecieses.Get(id.ToString());

            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(6) };
            var ss = ServerSpecies.Load(id);
            CacheSpecieses.Add(id.ToString(), ss, policy);
            return ss;
        }

        public static ServerReactionSpecies CachedRs(Guid rid, Guid sid)
        {
            var key = rid.ToString() + sid;
            if (CacheReactionsSpecies.Contains(key))
                return (ServerReactionSpecies)CacheReactionsSpecies.Get(key);

            foreach (var ss in ServerReactionSpecies.GetAllReactionsSpeciesForOneReaction(rid))
            {
                var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(6) };
                CacheReactionsSpecies.Add(rid.ToString() + ss.SpeciesId, ss, policy);
            }

            return (ServerReactionSpecies)CacheReactionsSpecies.Get(key);
        }

        public static string ExchangeLabel()
        {
            return "exr" + ((Guid.NewGuid().GetHashCode() & 0x7fffffff) % 99999999);
        }

        public static string FbaLabel()
        {
            return DateTime.Now.ToFileTimeUtc() + "";
        }

        public static int TotalReactions(HyperGraph.Node k)
        {
            var sum = k.ReactionCount.Consumers + k.ReactionCount.Producers;
            return sum == 0 ? int.MaxValue : sum;
        }

        public static int TotalReactions(Guid id)
        {
            var sum = AllReactionCache2[id].Consumers + AllReactionCache2[id].Producers;
            //if sum==0, k is probably Modifier
            return sum == 0 ? int.MaxValue : sum;
        }

        public static void SaveAsDgs(HyperGraph.Node m2, HyperGraph sm, TheAlgorithm algo)
        {
            var file = string.Format("{0}{1}graph.dgs", Dir, sm.LastLevel);
            var maxLevel = sm.LastLevel;// sm.Nodes.Max(n => n.Flux.Level);//Math.Max(sm.Nodes.Max(n => n.Flux.Level), sm.Edges.Max(e => e.Flux.Level));

            var lines = new List<string> { "DGS004", "\"Metabolic Network\" 0 0", "#Nodes", m2.ToDgs(NodeType.Selected) };

            foreach (var node in sm.Nodes.Values)
            {
                if (node.Id == m2.Id)
                    //type = NodeType.Selected;
                    continue;

                var type = NodeType.None;
                //if (!node.IsCommon)
                if (node.Level == maxLevel && node.IsBorder)
                    type = NodeType.NewBorder;
                //else if (node.Level == maxLevel)
                //    type = NodeType.New;
                else if (node.IsBorder)
                    type = NodeType.Border;

                lines.Add(node.ToDgs(type));
            }
            if (sm.HasCycle)
                Console.WriteLine();
            lines.Add("#Hyperedges");
            foreach (var edge in sm.Edges.Values)
            {
                var type = EdgeType.None;
                if (edge.Level == maxLevel)
                    type = EdgeType.New;

                lines.Add(edge.ToDgs(type, sm));
            }
            File.AppendAllLines(file, lines);
        }

        public static object GetPrivate(Type type, object instance, string fieldName)
        {
            const BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                           | BindingFlags.Static;
            var field = type.GetField(fieldName, BindFlags);
            Debug.Assert(field != null, "field != null");
            return field.GetValue(instance);
        }

        public static void SaveAsDgs(HyperGraph.Node m2, HyperGraph sm, Dictionary<string, double> results, Dictionary<string, double> prevResults)
        {
            var file = Dir + sm.LastLevel + "graph.dgs";
            var maxLevel = sm.LastLevel;// sm.Nodes.Max(n => n.Flux.Level);//Math.Max(sm.Nodes.Max(n => n.Flux.Level), sm.Edges.Max(e => e.Flux.Level));

            var lines = new List<string> { "DGS004", "\"Metabolic Network\" 0 0", "#Nodes", m2.ToDgs(NodeType.Selected) };

            foreach (var node in sm.Nodes.Values)
            {
                if (node.Id == m2.Id)
                    //type = NodeType.Selected;
                    continue;

                var type = NodeType.None;
                //if (node.Level == maxLevel && node.IsBorder)
                //    type = NodeType.NewBorder;
                //else if (node.Level == maxLevel)
                //    type = NodeType.New;
                //else if (node.IsBorder)
                //type = NodeType.Border;

                lines.Add(node.ToDgs(type));
            }

            lines.Add("#Hyperedges");
            foreach (var edge in sm.Edges.Values)
            {
                var type = EdgeType.None;
                if (edge.Level == maxLevel)
                    type = EdgeType.New;

                lines.Add(edge.ToDgs1(type));
            }
            File.AppendAllLines(file, lines);
        }

        public static void SaveAsDgs(HyperGraph.Node mi, HyperGraph graph, string dir)
        {
            var file = dir + graph.LastLevel + "graph.dgs";
            var maxLevel = graph.LastLevel;// sm.Nodes.Max(n => n.Flux.Level);//Math.Max(sm.Nodes.Max(n => n.Flux.Level), sm.Edges.Max(e => e.Flux.Level));

            var lines = new List<string> { "DGS004", "\"Metabolic Network\" 0 0", "#Nodes" };
            lines.AddRange(from node in graph.Nodes.Values let type = NodeType.None select node.ToDgs(type));

            lines.Add("#Hyperedges");
            foreach (var edge in graph.Edges.Values)
            {
                var type = EdgeType.None;
                if (edge.Level == maxLevel)
                    type = EdgeType.New;

                lines.Add(edge.ToDgs2(type));
            }

            File.AppendAllLines(file, lines);
        }

        public static double PowerLaw(double n)
        {
            return 1.0 / Math.Pow(1 - new Random().NextDouble(), 1.0 / (n + 1));
        }




    }
}
