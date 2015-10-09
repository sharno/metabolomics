namespace Metabol
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ILOG.Concert;
    using ILOG.CPLEX;

    using PathwaysLib.ServerObjects;
    using System.Configuration;

    using PathwaysLib.SoapObjects;

    public class Program
    {
        public static void Main5()
        {
            var meta = File.ReadAllLines("B:\\down\\meta.csv");
            var metaName = File.ReadAllLines("B:\\down\\metaName.csv");

            var reacts = File.ReadAllLines("B:\\down\\reacts.csv");
            var reactsName = File.ReadAllLines("B:\\down\\reactsName.csv");

            var metaCount = meta.Length;
            var reactCount = reacts.Length;

            var revf = File.ReadAllLines("B:\\down\\rev.csv");
            var rev = new bool[reactCount];

            for (var i = 0; i < reactCount; i++)
                rev[i] = revf[i] == "1";

            var rlb = File.ReadAllLines("B:\\down\\rlb.csv");
            var rub = File.ReadAllLines("B:\\down\\rub.csv");

            var bounds = new Tuple<double, double>[reactCount];
            for (var i = 0; i < reactCount; i++)
                bounds[i] = Tuple.Create(double.Parse(rlb[i]), double.Parse(rub[i]));


            var sf = File.ReadAllLines("B:\\down\\S.csv");
            var S = new double[metaCount, reactCount];

            for (var i = 0; i < metaCount; i++)
            {
                var line = sf[i].Split(',');
                for (var j = 0; j < reactCount; j++)
                    S[i, j] = double.Parse(line[j]);
            }


            DBWrapper.Instance = new DBWrapper(ConfigurationManager.AppSettings["dbConnectString"]);

            #region prefill

            ///*************************  Prefill UnitDefinition Table *************************************************/

            //Console.WriteLine("Populating UnitDefinition Table...");

            //string[] basics =
            //    {
            //        "ampere", "gram", "katal", "metre", "second", "watt", "becquerel", "gray", "kelvin",
            //        "mole", "siemens", "weber", "candela", "henry", "kilogram", "newton", "sievert",
            //        "coulomb", "hertz", "litre", "ohm", "steradian", "dimensionless", "item", "lumen",
            //        "pascal", "tesla", "farad", "joule", "lux", "radian", "volt"
            //    };

            //SoapUnitDefinition su;
            //ServerUnitDefinition srvu;
            //foreach (string unit in basics)
            //{
            //    su = new SoapUnitDefinition("", "", "", "", Guid.Empty, unit, unit, true);
            //    srvu = new ServerUnitDefinition(su);
            //    srvu.UpdateDatabase();
            //}

            ////substance: mole
            //su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "substance", "substance", false);
            //srvu = new ServerUnitDefinition(su);
            //srvu.UpdateDatabase();
            //srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("mole"), 1, 0, 1);

            ////volume: litre
            //su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "volume", "volume", false);
            //srvu = new ServerUnitDefinition(su);
            //srvu.UpdateDatabase();
            //srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("litre"), 1, 0, 1);

            ////area: square metre
            //su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "area", "area", false);
            //srvu = new ServerUnitDefinition(su);
            //srvu.UpdateDatabase();
            //srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("metre"), 2, 0, 1);

            ////length: metre
            //su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "length", "length", false);
            //srvu = new ServerUnitDefinition(su);
            //srvu.UpdateDatabase();
            //srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("metre"), 1, 0, 1);

            ////time: second
            //su = new SoapUnitDefinition("", "", "", "", Guid.Empty, "time", "time", false);
            //srvu = new ServerUnitDefinition(su);
            //srvu.UpdateDatabase();
            //srvu.AddUnit(UnitDefinitionManager.GetBaseUnitId("second"), 1, 0, 1);

            ////************************************** prefill ReactionSpeciesRole Table ************************
            //Console.WriteLine("Populating ReactionSpeciesRole Table...");
            //string[] roles = { "Reactant", "Product", "Modifier" };

            //SoapReactionSpeciesRole rsr;
            //ServerReactionSpeciesRole srvrsr;

            //foreach (string role in roles)
            //{
            //    rsr = new SoapReactionSpeciesRole(role);
            //    srvrsr = new ServerReactionSpeciesRole(rsr);
            //    srvrsr.UpdateDatabase();
            //}


            ////************************************** prefill RuleType Table **********************************
            //Console.WriteLine("Populating RuleType Table...");
            //string[] ruleType = { "Algebraic Rule", "Assignment Rule", "Rate Rule" };

            //SoapRuleType sRuleType;
            //ServerRuleType serRuleType;

            //foreach (string rule in ruleType)
            //{
            //    sRuleType = new SoapRuleType(rule);
            //    serRuleType = new ServerRuleType(sRuleType);
            //    serRuleType.UpdateDatabase();
            //}
            #endregion

            var soapModel = new SoapModel("", "", "", "", "recon2.04", "recon2.04", 2, 4, 1, "", Guid.NewGuid().ToString());
            var serverModel = new ServerModel(soapModel);
            serverModel.UpdateDatabase();

            var sSpeciesType = new SoapSpeciesType("", "", "", "", serverModel.ID, "human", "human");
            var serSpeciesType = new ServerSpeciesType(sSpeciesType);
            serSpeciesType.UpdateDatabase();


            var comps = new[]{"x","peroxisome",
"l","lysosome",
"c","cytoplasm",
"e","extracellular space",
"m","mitochondrion",
"r","endoplasmic reticulum",
"n","nucleus",
"g","Golgi apparatus"};

            var comparts = new Dictionary<string, ServerCompartment>();
            for (var i = 0; i < 16; i += 2)
            {
                var comp = new SoapCompartment(
              "",
              "",
              "",
              "",
              serverModel.ID,
              comps[i],
              comps[i + 1],
              Guid.Empty,
              3,
              1,
              Guid.Empty,
              Guid.Empty,
              Guid.Empty,
              false);
                var scomp = new ServerCompartment(comp);
                scomp.UpdateDatabase();
                comparts[comps[i]] = scomp;
            }

            var react = new Dictionary<string, ServerReaction>();

            for (var i = 0; i < metaCount; i++)
            {
                Console.Write("\r{0}    ", i);
                var co = meta[i].Substring(meta[i].IndexOf("[", StringComparison.Ordinal) + 1, 1);
                var sSpecies = new SoapSpecies(
                  "",
                  "",
                  "",
                  "",
                  serverModel.ID,
                  meta[i],
                  metaName[i],
                  serSpeciesType.ID,
                  comparts[co].ID,
                  0.0,
                  0.0,
                  Guid.Parse("64D77374-9139-403D-AF39-13EF69F972C1"),
                  false,
                  false,
                  1,
                  false);

                var serSpecies = new ServerSpecies(sSpecies);
                serSpecies.UpdateDatabase();
                for (var j = 0; j < reactCount; j++)
                {
                    if (Math.Abs(S[i, j]) < double.Epsilon) continue;
                    //var reactions = ServerReaction.FindReactionsByIds(serverModel.ID, reacts[j]);

                    ServerReaction serReaction;
                    if (react.ContainsKey(reacts[j]))
                    {
                        serReaction = react[reacts[j]];
                    }
                    else
                    {
                        var sReaction = new SoapReaction(
                            "",
                            "",
                            "",
                            "",
                            serverModel.ID,
                            reacts[j],
                            reactsName[j],
                            rev[j],
                            false,
                            Guid.Empty);
                        serReaction = new ServerReaction(sReaction);
                        serReaction.UpdateDatabase();
                        react[reacts[j]] = serReaction;
                    }

                    var sRSpecies = new SoapReactionSpecies(
                  "",
                  "",
                  "",
                  "",
                  serReaction.ID,
                  serSpecies.ID,
                  S[i, j] > 0 ? Util.ProductId : Util.ReactantId,
                  Math.Abs(S[i, j]),
                  Guid.Empty,
                    reacts[i] + meta[i],
                     reacts[i] + meta[i]);
                    var serRSpecies = new ServerReactionSpecies(sRSpecies);
                    serRSpecies.UpdateDatabase();

                }
            }


        }

        public static void Main2()
        {
            #region test graph

            //var a = Guid.NewGuid();
            //var b = Guid.NewGuid();
            //var c = Guid.NewGuid();
            //var d = Guid.NewGuid();
            //var e = Guid.NewGuid();

            //var b1 = Guid.NewGuid();
            //var b2 = Guid.NewGuid();
            //var b3 = Guid.NewGuid();

            //var v1 = Guid.NewGuid();
            //var v2 = Guid.NewGuid();
            //var v3 = Guid.NewGuid();
            //var graph = new HyperGraph();
            //graph.AddNode(a, "A");
            //graph.AddNode(b, "B");
            //graph.AddNode(c, "C");
            //graph.AddNode(d, "D");
            //graph.AddNode(e, "E");

            //graph.AddProduct(b1, "b1", a, "A");
            //graph.AddProduct(v1, "v1", b, "B");
            //graph.AddProduct(v2, "v2", c, "C");
            //graph.AddProduct(v2, "v2", e, "E");
            //graph.AddProduct(v3, "v3", d, "D");

            //graph.AddReactant(v1, "v1", a, "A");
            //graph.AddReactant(v1, "v1", e, "E");

            //graph.AddReactant(v2, "v2", b, "B");

            //graph.AddReactant(v3, "v3", a, "A");
            //graph.AddReactant(v3, "v3", e, "E");

            //graph.AddReactant(b3, "b3", d, "D");
            //graph.AddReactant(b2, "b2", c, "C");

            #endregion

            var graph = new HyperGraph();
            Console.WriteLine("Loading graph...");
            var count = 0;
            foreach (var m in ServerModel.Load(Guid.Parse("18B867B4-CBBE-4140-B753-DDDFC0A49445")).GetAllSpecies())
            {
               graph.AddNode(m.ID, m.SbmlId);
                foreach (var consumer in m.getAllReactions(Util.Reactant))
                {
                    graph.AddReactant(consumer.ID, consumer.SbmlId, m.ID, m.SbmlId);
                }

                foreach (var producer in m.getAllReactions(Util.Product))
                {
                    graph.AddProduct(producer.ID, producer.SbmlId, m.ID, m.SbmlId);
                }

                Console.Write("\r{0} metabolites  ", count++);
            }
            Console.WriteLine("M:{0}   R:{1}", graph.Nodes.Count, graph.Edges.Count);
            var deadend = graph.Nodes.Values.Where(n => n.Consumers.Count == 0 || n.Producers.Count == 0).Select(n => n.Label + " " + Util.GetReactionCountSum(n.Id));
            File.WriteAllLines("B:\\deadend2.txt", deadend);
            var lone = graph.Nodes.Values.Count(n => n.Consumers.Count == 0 && n.Producers.Count == 0);

            foreach (var m in graph.Nodes.Values)
            {
                if (graph.Nodes[m.Id].Producers.Count == 0)
                    graph.AddProduct(Guid.NewGuid(), string.Format("exr_{0}_prod", m.Label), m.Id, m.Label, true);

                if (graph.Nodes[m.Id].Consumers.Count == 0)
                    graph.AddReactant(Guid.NewGuid(), string.Format("exr_{0}_cons", m.Label), m.Id, m.Label, true);
            }

            var deadend2 = graph.Nodes.Values.Count(n => n.Consumers.Count == 0 || n.Producers.Count == 0);
            var pseudo = graph.Edges.Values.Count(e => e.IsPseudo);
            var reacts = graph.Edges.Values.Count();

            Console.WriteLine("deadend:{0}  deadend2:{1}   lone:{2} | pseudo:{3}   reacts:{4}", deadend.Count(), deadend2, lone, pseudo, reacts);


            Console.WriteLine("\nDone!\n");
            //Console.ReadKey();
            //TheAlgorithm.DefineBlockedReactions(graph);
        }

        //public static void Main()
        //{
        //    var dir = Util.Dir;
        //}

        #region main

        /// <summary>
        /// The main.
        /// </summary>
        public static void Main()
        {
            var p = new TheAlgorithm();
            p.Fba.RemoveConstraints = false;
            p.Start();
            do
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                //var it = p.Step(1).First();
                Console.WriteLine("\n************** ITERATION {0} *************** ", p.Iteration);
                p.Step();
                if (p.Iteration >= 8)
                //if (!p.IsFeasable)
                {
                    //p.Fba.RemoveConstraints = true;
                    //foreach (var str in p.Pathway)
                    //{
                    //    Console.Write(str+" => ");
                    //}
                   
                    Console.ReadKey();
                }

                //if (it.Id >= 21)
                //{
                //    p.Fba.RemoveConstraints = true;
                //    Console.ReadKey();
                //}

            }
            while (true); //count != sm.Edges.Count
        }

        private static void Init(TheAlgorithm Worker)
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

            //var id = Guid.Parse("{05954e8b-244a-4b59-b650-315f2c8e0f43}");
            var id = Guid.Parse("1218e2af-e534-41e6-bc59-e9731c95b182");
            Worker.Z[id] = (id.GetHashCode() % 2) == 0 ? -1 : 1; //rand.NextDouble() >= 0.5 ? 1 : -1;
            Console.WriteLine("{0}:{1}", Util.CachedS(id).SbmlId, Worker.Z[id]);
        }

        #endregion

        #region test


        public static void Main3()
        {
            var gen = new NetworkGenerator();
            gen.Gen2("");
        }

        public static void Main4()
        {
            var graph = new HyperGraph();

            var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            var c = Guid.NewGuid();
            var d = Guid.NewGuid();
            var e = Guid.NewGuid();

            var b1 = Guid.NewGuid();
            var b2 = Guid.NewGuid();
            var b3 = Guid.NewGuid();

            var v1 = Guid.NewGuid();
            var v2 = Guid.NewGuid();
            var v3 = Guid.NewGuid();

            graph.AddNode(a, "A");
            graph.AddNode(b, "B");
            graph.AddNode(c, "C");
            graph.AddNode(d, "D");
            graph.AddNode(e, "E");

            graph.AddProduct(b1, "b1", a, "A");
            graph.AddProduct(v1, "v1", b, "B");
            graph.AddProduct(v2, "v2", c, "C");
            graph.AddProduct(v2, "v2", e, "E");
            graph.AddProduct(v3, "v3", d, "D");

            graph.AddReactant(v1, "v1", a, "A");
            graph.AddReactant(v1, "v1", e, "E");

            graph.AddReactant(v2, "v2", b, "B");

            graph.AddReactant(v3, "v3", a, "A");
            graph.AddReactant(v3, "v3", e, "E");

            graph.AddReactant(b3, "b3", d, "D");
            graph.AddReactant(b2, "b2", c, "C");

            //Util.SaveAsDgs(graph.Nodes.Values.First(),graph,Util.Dir);

            var model = new Cplex { Name = "Test" };
            var vars = new Dictionary<string, INumVar>();
            var UpperBound = Double.MaxValue;
            var LowerBound = Double.MinValue;
            foreach (var edge in graph.Edges.Values)
                //if (edge.Label == "b1")
                //    vars[edge.Label] = model.NumVar(10, UpperBound, NumVarType.Float, edge.Label);
                //else
                vars[edge.Label] = model.NumVar(0, UpperBound, NumVarType.Float, edge.Label);

            foreach (var metabolite in graph.Nodes.Values)
            {
                var exp = model.LinearNumExpr();
                foreach (var reaction in metabolite.AllReactions())
                {
                    var coefficient = metabolite.Consumers.Contains(reaction) ? -1 : 1;
                    exp.AddTerm(vars[reaction.Label], coefficient);
                }
                model.AddEq(exp, 0.0, metabolite.Label);
            }

            model.AddEq(vars["b1"], 10);

            //foreach (var edge in graph.Edges.Values)
            {
                var obj = model.AddObjective(ObjectiveSense.Maximize, vars["b3"]);
                var isfeas = model.Solve();
                model.ExportModel(string.Format("{0}{1}model.lp", Util.Dir, vars["b2"]));
                FVA.SaveResult(graph, isfeas, model, vars, graph.Edges[b2]);
                //var max = model.GetValue(vars["b2"]);
                //model.Remove(obj);

                //obj = model.AddObjective(ObjectiveSense.Minimize, vars[edge.Label]);
                //isfeas = model.Solve();
                ////SaveResult(graph, isfeas, model, vars, edge);
                //var min = model.GetValue(vars[edge.Label]);
                //model.Remove(obj);

                //Results[edge.Id] = Tuple.Create(min, max);
            }
        }

        public static double Coefficient(HyperGraph.Edge reaction, HyperGraph.Node metabolite)
        {
            var coefficient = 0.0;

            if (reaction.Products.ContainsKey(metabolite.Id))
            {
                coefficient = metabolite.Weights[reaction.Id];
                if (Math.Abs(coefficient) < double.Epsilon && reaction.IsPseudo)
                    coefficient = 1;
            }

            if (reaction.Reactants.ContainsKey(metabolite.Id))
            {
                coefficient = -1 * metabolite.Weights[reaction.Id];
                if (Math.Abs(coefficient) < double.Epsilon && reaction.IsPseudo)
                    coefficient = -1;
            }
            return coefficient;
        }
        #endregion
    }
}
