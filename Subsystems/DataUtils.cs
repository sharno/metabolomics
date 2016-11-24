using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Metabol.DbModels.Cache;
using Metabol.DbModels;

namespace Subsystems
{
    class DataUtils
    {
        //public static void ReadSBML(string filePath)
        //{
        //    SBMLDocument document = libsbml.readSBML(filePath);
        //    SBaseList allElements = document.getListOfAllElements();

        //    var sbmlSpecies = document.getModel().getListOfSpecies();
        //    Console.WriteLine(sbmlSpecies.size());
        //    var species = new Dictionary<string, Metabol.DbModels.DB.Species>();
        //    for (var i = 0; i < sbmlSpecies.size(); i++ )
        //    {
        //        var sbmlMetabolite = sbmlSpecies.get(i);
        //        var tmpMetabolite = new Metabol.DbModels.DB.Species();
        //        tmpMetabolite.sbmlId = sbmlMetabolite.getId();
        //        Console.WriteLine(sbmlMetabolite.getAnnotationString());
        //        Console.WriteLine(sbmlMetabolite.getName());
        //    }
        //}
        

        public static void JsonToCache(string path)
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                string json = streamReader.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);


                Metabol.DbModels.Db.Cache = new CacheModel();
                foreach (var c in items.compartments)
                {
                    var compartment = new Compartment
                    {
                        id = Guid.NewGuid(),
                        sbmlId = c.Name,
                        name = c.Last,
                    };
                    Db.Cache.Compartments.Add(compartment);
                }

                foreach (var m in items.metabolites)
                {
                    var metabolite = new Species
                    {
                        id = Guid.NewGuid(),
                        sbmlId = m.id,
                        name = m.name,
                        charge = m.charge,
                        compartmentId = Db.Cache.Compartments.Find(c => c.sbmlId == m.compartment.Value).id,
                        ReactionSpecies = null,
                    };
                    Db.Cache.Species.Add(metabolite);
                }

                foreach (var r in items.reactions)
                {
                    var reaction = new Reaction
                    {
                        id = Guid.NewGuid(),
                        sbmlId = r.id,
                        name = r.name,
                        subsystem = r.subsystem == null ? "" : r.subsystem,
                        reversible = r.lower_bound < 0,
                        ReactionSpecies = null,
                    };

                    reaction.ReactionBounds.Add(new ReactionBound()
                    {
                        reactionId = reaction.id,
                        lowerBound = r.lower_bound,
                        upperBound = r.upper_bound,
                    });
                    Db.Cache.Reactions.Add(reaction);



                    foreach (var m in r.metabolites)
                    {
                        var rs = new ReactionSpecy
                        {
                            id = Guid.NewGuid(),
                            name = "",
                            sbmlId = "",
                            reactionId = reaction.id,
                            speciesId = Db.Cache.Species.Find(se => se.sbmlId == m.Name).id,
                            stoichiometry = m.Last,
                            roleId = m.Last > 0 ? Db.ProductId : Db.ReactantId,
                            Reaction = Db.Cache.Reactions.Find(re => re.sbmlId == r.id.Value),
                            Species = Db.Cache.Species.Find(se => se.sbmlId == m.Name),
                        };
                        Db.Cache.ReactionSpecies.Add(rs);
                    }
                }


                foreach (var s in Db.Cache.Species)
                {
                    s.ReactionSpecies = Db.Cache.ReactionSpecies.Where(rs => rs.speciesId == s.id).ToList();
                }
                foreach (var r in Db.Cache.Reactions)
                {
                    r.ReactionSpecies = Db.Cache.ReactionSpecies.Where(rs => rs.reactionId == r.id).ToList();
                }


                WriteToBinaryFile("C:\\Users\\sharno\\Downloads\\MODEL1603150001.bin", Db.Cache);
                Console.WriteLine("Finished saving all to DB");
                Console.ReadLine();
            }
        }


        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }
        

        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
    }
}
