using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ILOG.Concert;
using ILOG.CPLEX;
using Newtonsoft.Json;

namespace Metabol.DbModels
{
    public class GeneNetwork
    {
        public IDictionary<int, GeneRelation> Relations { get; }
        public IDictionary<string, Gene> Genes { get; }
        private int ids;
        private static readonly ConcurrentDictionary<string, string> nameid = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> ridname = new ConcurrentDictionary<string, string>();
        private static string RootDir="A:\\"
        static GeneNetwork()
        {
            dynamic ecoli = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(RootDir+"ecoli_core.json"));

            foreach (var gene in ecoli.genes)
                nameid[gene.name.ToString().ToLower()] = gene.id.ToString();

            foreach (var r in ecoli.reactions)
                if (r.gene_reaction_rule.ToString() != String.Empty)
                    ridname[r.id.ToString()] = r.gene_reaction_rule.ToString();

            Console.WriteLine();

        }

        public GeneNetwork()
        {
            Relations = new ConcurrentDictionary<int, GeneRelation>();
            Genes = new ConcurrentDictionary<string, Gene>();
        }

        public static ConcurrentDictionary<string, INumVar> AddRegulationConstraints(Cplex cplex, HyperGraph graph, Dictionary<Guid, INumVar> v)
        {
            var bvars = new ConcurrentDictionary<string, INumVar>();
            if (graph.Step <= 0) return bvars;
            foreach (var edge in graph.Edges.Values.Where(e => !e.IsPseudo && ridname.ContainsKey(e.Label)))
            {
                var p = BooleanParser.Parse(ridname[edge.Label], bvars, cplex);

                //cplex.Add(p.Constraints.ToArray());
                //foreach (var constraint in p.Constraints)
                //    cplex.Add(constraint);

                cplex.Add(cplex.IfThen(cplex.Eq(p.RootVar, 0), cplex.Eq(v[edge.Id], 0)));

                if (Math.Abs(edge.PreFlux) > 0.000001)
                    cplex.Add(cplex.Eq(p.RootVar, 1, $"Gia_{edge.Label}"));
            }

            var net = LoadGraph(RootDir+"ecoli_network_tf_tf.txt");
            foreach (var rel in net.Relations)
            {
                var tfId = nameid.ContainsKey(rel.Value.TF.Name) ? nameid[rel.Value.TF.Name] : rel.Value.TF.Name;
                var tgId = nameid.ContainsKey(rel.Value.TG.Name) ? nameid[rel.Value.TG.Name] : rel.Value.TG.Name;
                //if (!nameid.ContainsKey(tgId) && !nameid.ContainsKey(tfId)) continue;
                var tf = bvars.GetOrAdd(tfId, cplex.BoolVar(tfId));
                var tg = bvars.GetOrAdd(tgId, cplex.BoolVar(tgId));
                //if (nameid.ContainsKey(rel.Value.TF.Name) && nameid.ContainsKey(rel.Value.TG.Name))
                cplex.Add(cplex.IfThen(cplex.Eq(tf, 1), cplex.Eq(tg, 0), $"{tf}{tg}"));
            }

            return bvars;
        }

        public static GeneNetwork LoadGraph(string path)
        {
            //E:\Dropbox\Metabolomics\Data
            var net = new GeneNetwork();
            //var tmp = from line in File.ReadAllLines(path).Skip(34)
            //          select GeneRelation.ParseRegulon(line, net);
            foreach (var line in File.ReadAllLines(path).Skip(34))
                GeneRelation.ParseRegulon(line, net);

            //net.ToDgs("A:\\regnet.dgs");

            return net;
        }

        public void ToDgs(string file)
        {
            var lines = new List<string> {"DGS004", "\"Regulation Network\" 0 0", "#Nodes"};
            lines.AddRange(Genes.Select(gene => $"an \"{gene.Value.Name}\" label:\"{gene.Value.Name}\""));
            lines.Add("#Edges");
            lines.AddRange(Relations.Select(geneRelation => $"ae \"{geneRelation.Value}\" \"{geneRelation.Value.TF.Name}\" > \"{geneRelation.Value.TG.Name}\" label:\"{geneRelation.Value.RelationStr}\" ui.style:\"size: 3px; arrow-size:8px, 6px; \""));
            File.WriteAllLines(file, lines);
        }

        public class Gene
        {
            public string GeneId;
            public string Name;


            public override string ToString()
            {
                return Name;
            }

            protected bool Equals(Gene other)
            {
                return string.Equals(GeneId, other.GeneId);
            }

            public override int GetHashCode()
            {
                return GeneId?.GetHashCode() ?? 0;
            }
        }

        public class GeneRelation
        {
            //public int Oid;
            public Gene TF;
            public Gene TG;
            public int Relation;
            public string RelationStr { get; set; }

            public static GeneRelation ParseHTRIdb(string line, GeneNetwork net)
            {
                var gr = new GeneRelation();
                var g = line.Split(';');
                //gr.Oid = int.Parse(g[0]);
                net.Relations[net.ids++] = gr;

                Gene gene;

                if (net.Genes.ContainsKey(g[1]))
                    gene = net.Genes[g[1]];
                else
                    gene = new Gene
                    {
                        GeneId = g[1],
                        Name = g[2]
                    };

                gr.TF = gene;

                if (net.Genes.ContainsKey(g[3]))
                    gene = net.Genes[g[3]];
                else
                    gene = new Gene
                    {
                        GeneId = g[3],
                        Name = g[4]
                    };

                gr.TG = gene;

                return gr;
            }

            public static void ParseRegulon(string line, GeneNetwork net)
            {
                var gr = new GeneRelation();
                var g = Regex.Split(line, "\t");

                // skip self and up regulation 
                if (g[2] != "-" || string.Equals(g[0], g[1], StringComparison.CurrentCultureIgnoreCase)) return;

                gr.Relation = g[2] == "-" ? -1 : g[2] == "+" ? 1 : 0;//g[2] == "+-" ? 0 : int.MaxValue;
                gr.RelationStr = g[2];
                net.Relations[net.ids++] = gr;

                Gene gene;

                if (net.Genes.ContainsKey(g[0].ToLower()))
                    gene = net.Genes[g[0].ToLower()];
                else
                {
                    gene = new Gene
                    {
                        GeneId = g[0].ToLower(),
                        Name = g[0].ToLower()
                    };
                    net.Genes[gene.GeneId] = gene;
                }


                gr.TF = gene;

                if (net.Genes.ContainsKey(g[1].ToLower()))
                    gene = net.Genes[g[1].ToLower()];
                else
                {
                    gene = new Gene
                    {
                        GeneId = g[1].ToLower(),
                        Name = g[1].ToLower()
                    };
                    net.Genes[gene.GeneId] = gene;
                }

                gr.TG = gene;
            }

            public override string ToString()
            {
                return $"{TF}{RelationStr}>{TG}";
            }
        }

    }
}