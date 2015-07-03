namespace Metabol
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Optimization;
    using Optimization.Exporter;
    using Optimization.Solver;
    using Optimization.Solver.GLPK;

    public class Fba3 : IDisposable
    {
        public bool RemoveConstraints = false;
        public string Label { get; set; }
        public double LastRuntime { get; set; }
        public Dictionary<Guid, HGraph.Edge> RemovedConsumerExchange { get; set; }
        public Dictionary<Guid, HGraph.Edge> RemovedProducerExchange { get; set; }
        public Dictionary<string, double> Results { get; set; }
        public Dictionary<string, double> PrevResults { get; set; }
        public Dictionary<Guid, HashSet<Guid>> UpdateExchangeConstraint { get; set; }
        public HashSet<Guid> IgnoreSet { get; set; }

        public Fba3()
        {
            Label = Util.FbaLabel();
            RemovedConsumerExchange = new Dictionary<Guid, HGraph.Edge>();
            RemovedProducerExchange = new Dictionary<Guid, HGraph.Edge>();
            Results = new Dictionary<string, double>();
            PrevResults = new Dictionary<string, double>();
            UpdateExchangeConstraint = new Dictionary<Guid, HashSet<Guid>>();
            IgnoreSet = new HashSet<Guid>();
        }

        public bool Solve(Dictionary<Guid, Reaction> reactions, Dictionary<Guid, int> smz, HGraph sm)
        {
            return false;
        }


        private static double Coefficient(Reaction reaction, Metabolite metabolite1)
        {
            var coefficient = 0.0;
            if (reaction.Products.ContainsKey(metabolite1.Id))
                coefficient = reaction.Products[metabolite1.Id].Stoichiometry;

            if (reaction.Reactants.ContainsKey(metabolite1.Id))
                coefficient = (-1 * reaction.Reactants[metabolite1.Id].Stoichiometry);
            return coefficient;
        }

        public void Dispose()
        {
            RemovedConsumerExchange.Clear();
            RemovedProducerExchange.Clear();
            Results.Clear();
            PrevResults.Clear();
        }
    }
}