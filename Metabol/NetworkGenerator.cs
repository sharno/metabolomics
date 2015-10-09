using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol
{
    using System.Configuration;
    using System.Diagnostics;

    using PathwaysLib.ServerObjects;
    using PathwaysLib.SoapObjects;

    class NetworkGenerator
    {
        readonly HyperGraph graph = new HyperGraph();
        public int CurrentLevel { get; set; }

        public NetworkGenerator()
        {
            CurrentLevel = 0;
        }

        /*
         * 
         * 
         */
        public NetworkGenerator Gen1(string dir)
        {
            var m0 = graph.AddNode(Guid.NewGuid(), "m00");
            //graph.AddProduct(Guid.NewGuid(), "ex00", m0.Id, m0.Label, true);
            graph.AddReactant(Guid.NewGuid(), "r00", m0.Id, m0.Label);
            graph.NextStep();

            var m1 = graph.AddNode(Guid.NewGuid(), "m01");
            graph.AddProduct(m0.Consumers.First().Id, "r00", m1.Id, "m01");
            //graph.AddReactant(Guid.NewGuid(), "ex1", m1.Id, "m1", true);
            HyperGraph.Edge ri = null;
            var mi = m1;
            var step = 2;
            var totalsteps = 10;
            var randr = new Random();
            //var randm = new Random();
            for (; step < totalsteps + 1; step++)
            {
                //
                graph.AddReactant(Guid.NewGuid(), string.Format("r{0:D2}", step - 1), mi.Id, mi.Label);
                graph.NextStep();
                ri = mi.Consumers.First();
                if (step >= 2)
                    for (var k = 0; k < step - 1; k++)
                    {
                        if (randr.NextDouble() > (2.5 / totalsteps)) continue;

                        var label = string.Format("m{0:D2}", k);
                        var mb = graph.Nodes.First(e => e.Value.Label == label);
                        graph.AddProduct(ri.Id, ri.Label, mb.Key, mb.Value.Label);
                    }

                if (step == totalsteps) continue;

                mi = graph.AddNode(Guid.NewGuid(), string.Format("m{0:D2}", step));
                graph.AddProduct(ri.Id, ri.Label, mi.Id, mi.Label);
            }
            Util.SaveAsDgs(mi, graph, string.Format("B:\\gen\\{0}", DateTime.UtcNow.ToString("s").Replace(":", "-")));

            var soapModel = new SoapModel("", "", "", "", "netgen1", "netgen1", 3, 3, 1, "", Guid.NewGuid().ToString());
            var serverModel = new ServerModel(soapModel);
            serverModel.UpdateDatabase();

            var sCompartment = new SoapCompartment("", "", "", "", serverModel.ID, "netgen1com", "netgen1com", Guid.Empty,
                                                      3, 1, Guid.Empty, Guid.Empty, Guid.Empty, false);

            var serCompartment = new ServerCompartment(sCompartment);
            serCompartment.UpdateDatabase();

            var sSpeciesType = new SoapSpeciesType("", "", "", "", serverModel.ID, "netgen1st", "netgen1st");
            var serSpeciesType = new ServerSpeciesType(sSpeciesType);
            serSpeciesType.UpdateDatabase();

            this.UpdateDb(serverModel, serCompartment, serSpeciesType);

            //foreach (var node in graph.Nodes.Values)
            //{
            //    var asreactan = node.Consumers.Count;
            //    var asproduct = node.Producers.Count;

            //    Util.AllReactionCache[node.Id] = Tuple.Create(asreactan, asproduct);
            //    Util.AllStoichiometryCache[node.Id] = Tuple.Create(asreactan * 1.0, asproduct * 1.0);
            //}

            //graph.AddReactant(Guid.NewGuid(), string.Format("r{0}", step-1), mi.Id, mi.Label, true);
            //Util.SaveAsDgs(mi, graph, dir);
            return this;
        }

        public NetworkGenerator Gen2(string dir)
        {
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

            Util.SaveAsDgs(graph.Nodes[a], graph, string.Format("B:\\gen\\{0}", DateTime.UtcNow.ToString("s").Replace(":", "-")));

            var soapModel = new SoapModel("", "", "", "", "netgen1", "netgen1", 3, 3, 1, "", Guid.NewGuid().ToString());
            var serverModel = new ServerModel(soapModel);
            serverModel.UpdateDatabase();

            var sCompartment = new SoapCompartment("", "", "", "", serverModel.ID, "netgen1com", "netgen1com", Guid.Empty,
                                                      3, 1, Guid.Empty, Guid.Empty, Guid.Empty, false);

            var serCompartment = new ServerCompartment(sCompartment);
            serCompartment.UpdateDatabase();

            var sSpeciesType = new SoapSpeciesType("", "", "", "", serverModel.ID, "netgen1st", "netgen1st");
            var serSpeciesType = new ServerSpeciesType(sSpeciesType);
            serSpeciesType.UpdateDatabase();

            this.UpdateDb(serverModel, serCompartment, serSpeciesType);
            return this;
        }

        private void UpdateDb(ServerModel serverModel, ServerCompartment compartment, ServerSpeciesType speciesType)
        {
            DBWrapper.Instance = new DBWrapper(ConfigurationManager.AppSettings["dbConnectString"]);

            foreach (var node in this.graph.Nodes.Values)
            {
                var sSpecies = new SoapSpecies(
                    "",
                    "",
                    "",
                    "",
                    serverModel.ID,
                    node.Label,
                    node.Label,
                    speciesType.ID,
                    compartment.ID,
                    0.0,
                    0.0,
                    Guid.Parse("f4b2f7b6-dd76-44a3-aa1f-03e760b83d59"),
                    false,
                    false,
                    1,
                    false);

                var serSpecies = new ServerSpecies(sSpecies);
                serSpecies.UpdateDatabase();

                foreach (var edge in node.Consumers)
                {
                    var reactions = ServerReaction.FindReactionsByName(serverModel.ID, edge.Label);
                    ServerReaction serReaction;
                    if (reactions.Length == 0)
                    {
                        var sReaction = new SoapReaction(
                            "",
                            "",
                            "",
                            "",
                            serverModel.ID,
                            edge.Label,
                            edge.Label,
                            false,
                            false,
                            Guid.Empty);
                        serReaction = new ServerReaction(sReaction);
                        serReaction.UpdateDatabase();
                    }
                    else
                    {
                        serReaction = reactions[0];
                    }

                    var sRSpecies = new SoapReactionSpecies(
                        "",
                        "",
                        "",
                        "",
                        serReaction.ID,
                        serSpecies.ID,
                        Util.ReactantId,
                        1.0,
                        Guid.Empty,
                        edge.Label + node.Label,
                        edge.Label + node.Label);
                    var serRSpecies = new ServerReactionSpecies(sRSpecies);
                    serRSpecies.UpdateDatabase();
                }

                foreach (var edge in node.Producers)
                {
                    var reactions = ServerReaction.FindReactionsByName(serverModel.ID, edge.Label);
                    ServerReaction serReaction;
                    if (reactions.Length == 0)
                    {
                        var sReaction = new SoapReaction(
                            "",
                            "",
                            "",
                            "",
                            serverModel.ID,
                            edge.Label,
                            edge.Label,
                            false,
                            false,
                            Guid.Empty);
                        serReaction = new ServerReaction(sReaction);
                        serReaction.UpdateDatabase();

                    }
                    else
                    {
                        serReaction = reactions[0];
                    }

                    var sRSpecies = new SoapReactionSpecies(
                        "",
                        "",
                        "",
                        "",
                        serReaction.ID,
                        serSpecies.ID,
                        Util.ProductId,
                        1.0,
                        Guid.Empty,
                        edge.Label + node.Label,
                        edge.Label + node.Label);
                    var serRSpecies = new ServerReactionSpecies(sRSpecies);
                    serRSpecies.UpdateDatabase();
                }

            }
        }

        public IEnumerable<HyperGraph.Node> NextNodes()
        {
            var level = CurrentLevel++;
            return graph.Nodes.Values.Where(n => n.Level == level);
        }
    }
}
