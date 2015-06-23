using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using PathwaysLib.ServerObjects;

namespace Metabol
{
    class Util
    {
        private static readonly MemoryCache CacheSpecieses = new MemoryCache("species");
        internal static readonly MemoryCache CacheReactions = new MemoryCache("reactions");
        private static readonly MemoryCache CacheReactionsSpecies = new MemoryCache("reactionspecies");

        internal static Dictionary<Guid, Tuple<int, int>> AllReactionCache = new Dictionary<Guid, Tuple<int, int>>();
        internal static readonly Dictionary<string, Fba.Metabolite> Meta = new Dictionary<string, Fba.Metabolite>();
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
            var sum = k.AllReactions.Item1 + k.AllReactions.Item2;
            return sum == 0 ? int.MaxValue : sum;
        }

        internal static int TotalReactions(Guid id)
        {
            var sum = AllReactionCache[id].Item1 + AllReactionCache[id].Item2;
            //if sum==0, k is probably Modifier
            return sum == 0 ? int.MaxValue : sum;
        }
    }
}
