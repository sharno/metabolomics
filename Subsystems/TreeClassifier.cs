using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.MachineLearning.DecisionTrees.Rules;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Subsystems
{
    class TreeClassifier
    {
        List<int[]> X = new List<int[]>();
        List<int> Y = new List<int>();
        List<string> subsystems = new List<string>();

        public void Classify ()
        {
            var inputs = X.ToArray<int[]>();
            var outputs = Y.ToArray<int>();
            
            var attributes = subsystems.Select(s => new DecisionVariable(s, 2)).ToArray();
            int classCount = 2;
            DecisionTree tree = new DecisionTree(inputs: attributes, classes: classCount);

            var teacher = new C45Learning(tree);
            teacher.Learn(inputs, outputs);

            int[] predicted = tree.Decide(inputs);

            // And the classification error can be computed as 
            double error = new ZeroOneLoss(outputs) // 0.0266
            {
                Mean = true
            }.Loss(tree.Decide(inputs));

            // Moreover, we may decide to convert our tree to a set of rules:
            DecisionSet rules = tree.ToRules();

            // And using the codebook, we can inspect the tree reasoning:
            string ruleText = rules.ToString();

            Console.WriteLine(error);
            Console.WriteLine(ruleText);
        }
    }
}
