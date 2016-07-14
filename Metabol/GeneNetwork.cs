using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Ecoli;
using ILOG.Concert;
using ILOG.CPLEX;
using Metabol.DbModels;
using Newtonsoft.Json;

namespace Metabol
{
    public class GeneNetwork
    {
        public IDictionary<int, GeneRelation> Relations { get; }
        public IDictionary<string, Gene> Genes { get; }
        private int ids;
        private static readonly ConcurrentDictionary<string, string> nameid = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> ridname = new ConcurrentDictionary<string, string>();

        static GeneNetwork()
        {
            dynamic ecoli = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText("E:\\Dropbox\\Metabolomics\\Data\\ecoli_core.json"));

            foreach (var gene in ecoli.genes)
                nameid[gene.name.ToString().ToLower()] = gene.id.ToString();

            foreach (var r in ecoli.reactions)
                if (r.gene_reaction_rule.ToString() != string.Empty)
                    ridname[r.id.ToString()] = r.gene_reaction_rule.ToString();

        }
        public GeneNetwork()
        {
            Relations = new ConcurrentDictionary<int, GeneRelation>();
            Genes = new ConcurrentDictionary<string, Gene>();
        }

        public static GeneNetwork LoadGraph(string path)
        {
            //E:\Dropbox\Metabolomics\Data
            var net = new GeneNetwork();
            //var tmp = from line in File.ReadAllLines(path).Skip(34)
            //          select GeneRelation.ParseRegulon(line, net);
            foreach (var line in File.ReadAllLines(path).Skip(34))
                GeneRelation.ParseRegulon(line, net);

            return net;
        }

        public static ConcurrentDictionary<string, IIntVar> AddRegulationConstraints(Cplex cplex, HyperGraph graph, Dictionary<Guid, INumVar> v)
        {
            if (graph.Step <= 0)
            {
                return new ConcurrentDictionary<string, IIntVar>();
            }

            var bvars = new ConcurrentDictionary<string, IIntVar>();

            foreach (var edge in graph.Edges.Values.Where(e => !e.IsPseudo && ridname.ContainsKey(e.Label)))
            {
                //if (Regex.Split(ridname[edge.Label], "and|or").Length > 2) continue;
                var p = BooleanParser.Parse(ridname[edge.Label], bvars);
                //foreach (var kv in p.Vars)
                //    bvars[kv.Key] = kv.Value;

                cplex.Add(p.Constraints.ToArray());
                cplex.Add(cplex.IfThen((cplex.Eq(p.RootVar, 0)), cplex.Eq(v[edge.Id], 0), $"Gr{edge.Label}"));
                //if (graph.Step <= 0) continue;

                if (Math.Abs(edge.PreFlux) > 0.000001)
                    cplex.Add(cplex.Eq(p.RootVar, 1, $"Gia{edge.Label}"));
            }

            var net = LoadGraph("E:\\Dropbox\\Metabolomics\\Data\\ecoli_network_tf_gene.txt");
            foreach (var rel in net.Relations)
            {
                var tfId = nameid.ContainsKey(rel.Value.TF.Name) ? nameid[rel.Value.TF.Name] : rel.Value.TF.Name;
                var tgId = nameid.ContainsKey(rel.Value.TG.Name) ? nameid[rel.Value.TG.Name] : rel.Value.TG.Name;
                if (!nameid.ContainsKey(tgId) && !nameid.ContainsKey(tfId)) continue;
                var tf = bvars.GetOrAdd(tfId, cplex.BoolVar(tfId));
                var tg = bvars.GetOrAdd(tgId, cplex.BoolVar(tgId));
                //if (nameid.ContainsKey(rel.Value.TF.Name) && nameid.ContainsKey(rel.Value.TG.Name))
                cplex.Add(cplex.IfThen(cplex.Eq(tf, 1), cplex.Eq(tg, 0)));
            }

            return bvars;
        }

        public class Gene
        {
            public string GeneId;
            public string Name;
            public override string ToString()
            {
                return Name;
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

                if (g[2] == "?")
                    return;

                gr.Relation = g[2] == "-" ? -1 : g[2] == "+" ? 1 : g[2] == "+-" ? 0 : int.MaxValue;
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