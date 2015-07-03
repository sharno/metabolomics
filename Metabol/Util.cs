namespace Metabol
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;

    using PathwaysLib.ServerObjects;

    class Util
    {
        private static readonly MemoryCache CacheSpecieses = new MemoryCache("species");
        internal static readonly MemoryCache CacheReactions = new MemoryCache("reactions");
        private static readonly MemoryCache CacheReactionsSpecies = new MemoryCache("reactionspecies");

        internal static Dictionary<Guid, Tuple<int, int>> AllReactionCache = new Dictionary<Guid, Tuple<int, int>>();
        //internal static readonly Dictionary<string, Metabolite> Meta = new Dictionary<string, Metabolite>();
        internal const string Product = "Product";
        internal const string Reactant = "Reactant";
        internal const string Modifier = "Modifier";

        internal static readonly string Dir = ConfigurationManager.AppSettings["modelOutput"];
        internal static readonly string AllReactionFile = ConfigurationManager.AppSettings["allReaction"];
        internal static readonly string SelectedMetaFile = ConfigurationManager.AppSettings["selection"];

        static Util()
        {
            //var list = (from species in ServerSpecies.AllSpecies()
            //            let product = species.getAllReactions(Util.Product).Length
            //            let reactant = species.getAllReactions(Util.Reactant).Length
            //            select $"{species.ID},{reactant},{product}").ToList();
            //File.AppendAllLines(Util.AllReactionFile, list);
            Directory.CreateDirectory(Dir);
            var lines = File.ReadAllLines(AllReactionFile);
            foreach (var result in lines.Select(s => s.Split(',')))
                AllReactionCache[Guid.Parse(result[0])] = new Tuple<int, int>(Int32.Parse(result[1]), Int32.Parse(result[2]));//int.Parse(result[1]);
        }

        internal static ServerReaction CachedR(Guid id)
        {
            if (CacheReactions.Contains(id.ToString()))
                return (ServerReaction)CacheReactions.Get(id.ToString());

            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(6) };
            var ss = ServerReaction.Load(id);
            CacheReactions.Add(id.ToString(), ss, policy);
            return ss;
        }

        internal static ServerSpecies CachedS(Guid id)
        {
            if (CacheSpecieses.Contains(id.ToString()))
                return (ServerSpecies)CacheSpecieses.Get(id.ToString());

            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(6) };
            var ss = ServerSpecies.Load(id);
            CacheSpecieses.Add(id.ToString(), ss, policy);
            return ss;
        }

        internal static ServerReactionSpecies CachedRs(Guid rid, Guid sid)
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

        internal static string ExchangeLabel()
        {
            return "exr" + ((Guid.NewGuid().GetHashCode() & 0x7fffffff) % 99999999);
        }

        internal static string FbaLabel()
        {
            return DateTime.Now.ToFileTimeUtc() + "";
        }

        internal static int TotalReactions(HGraph.Node k)
        {
            var sum = k.ReactionCount.Item1 + k.ReactionCount.Item2;
            return sum == 0 ? Int32.MaxValue : sum;
        }

        internal static int TotalReactions(Guid id)
        {
            var sum = AllReactionCache[id].Item1 + AllReactionCache[id].Item2;
            //if sum==0, k is probably Modifier
            return sum == 0 ? Int32.MaxValue : sum;
        }

        internal static void SaveAsDgs(HGraph.Node m2, HGraph sm, TheAlgorithm algo)
        {
            var file = Dir + sm.LastLevel + "graph.dgs";
            var maxLevel = sm.LastLevel;// sm.Nodes.Max(n => n.Value.Level);//Math.Max(sm.Nodes.Max(n => n.Value.Level), sm.Edges.Max(e => e.Value.Level));

            var lines = new List<string> { "DGS004", "\"Metabolic Network\" 0 0", "#Nodes", m2.ToDgs(NodeType.Selected) };

            foreach (var node in sm.Nodes.Values)
            {
                if (node.Id == m2.Id)
                    //type = NodeType.Selected;
                    continue;

                var type = NodeType.None;
                if (node.Level == maxLevel && node.IsBorder)
                    type = NodeType.NewBorder;
                //else if (node.Level == maxLevel)
                //    type = NodeType.New;
                else if (node.IsBorder)
                    type = NodeType.Border;

                lines.Add(node.ToDgs(type));
            }

            lines.Add("#Hyperedges");
            foreach (var edge in sm.Edges.Values)
            {
                var type = EdgeType.None;
                if (edge.Level == maxLevel)
                    type = EdgeType.New;

                lines.Add(edge.ToDgs(type, algo));
            }
            File.AppendAllLines(file, lines);
        }
        internal static void SaveAsDgs2(HGraph.Node m2, HGraph sm, TheAlgorithm algo)
        {
         
        }

        internal static object GetPrivate(Type type, object instance, string fieldName)
        {
            const BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                           | BindingFlags.Static;
            var field = type.GetField(fieldName, BindFlags);
            return field?.GetValue(instance);
        }

        public static void SaveAsDgs(HGraph.Node m2, HGraph sm, Dictionary<string, double> results, Dictionary<string, double> prevResults)
        {
            var file = Dir + sm.LastLevel + "graph.dgs";
            var maxLevel = sm.LastLevel;// sm.Nodes.Max(n => n.Value.Level);//Math.Max(sm.Nodes.Max(n => n.Value.Level), sm.Edges.Max(e => e.Value.Level));

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

                lines.Add(edge.ToDgs(type, results,prevResults));
            }
            File.AppendAllLines(file, lines);
        }
    }
}
