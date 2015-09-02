using System;
using System.Linq;
using PathwaysLib.ServerObjects;

namespace Metabol
{
    using System.Configuration;

    public partial class User : IDisposable
    {
        public string Id;

        public TheAlgorithm Worker;

        public User(string id)
        {
            Id = id;
            Worker = new TheAlgorithm();
            //Init();
        }

        public void Init()
        {
            var strCon = ConfigurationManager.AppSettings["dbConnectString"];
            DBWrapper.Instance = new DBWrapper(strCon);


            string[] zn =
            {
                "ADP", "ATP(4-)",
                "D-Fructose 6-phosphate", "D-Fructose 1,6-bisphosphate", "Dihydroxyacetone phosphate",
                "Glyceraldehyde 3-phosphate", "L-threonine", "taurochenodeoxycholate",
                "D-glucose", "3-Phospho-D-glycerate", "D-Glycerate 2-phosphate", "Phosphoenolpyruvate", "pyruvate",
                "Prothrombin", "pantetheine"
            };

            var zlist =
                (from s in zn select ServerSpecies.AllSpeciesByNameOnly(s) into spec where spec.Length > 0 select spec[0])
                    .ToList();
            zlist.Sort((species, serverSpecies) => string.Compare(species.SbmlId, serverSpecies.SbmlId, StringComparison.Ordinal));
            //var rand = new Random((int)DateTime.UtcNow.ToBinary());

            foreach (var s in zlist)
            {
                Worker.Z[s.ID] = (s.ID.GetHashCode() % 2) == 0 ? -1 : 1;//rand.NextDouble() >= 0.5 ? 1 : -1;
                Console.WriteLine("{0}:{1}", s.SbmlId, Worker.Z[s.ID]);
            }

            var id = Guid.Parse("{05954e8b-244a-4b59-b650-315f2c8e0f43}");
            Worker.Z[id] = (id.GetHashCode() % 2) == 0 ? -1 : 1; //rand.NextDouble() >= 0.5 ? 1 : -1;
            Console.WriteLine("{0}:{1}", Util.CachedS(id).SbmlId, Worker.Z[id]);
        }

        public void Dispose()
        {
            Worker.Stop();
        }
    }
}