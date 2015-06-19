using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// A container for the parameters that represent each 'to node' or 'hop' in a path query
    /// </summary>
    [SerializableAttribute]
    public class QueryPathParametersToNode
    {
        private Guid _nodeId;
        private int _minLength;
        private int _maxLength;
        private List<Guid> _includedNodes;
        private List<Guid> _excludedNodes;

        public Guid NodeId
        {
            get { return _nodeId; }
        }

        public int MinLength
        {
            get { return _minLength; }
        }

        public int MaxLength
        {
            get { return _maxLength; }
        }

        public List<Guid> IncludedNodes
        {
            get { return _includedNodes; }
        }

        public List<Guid> ExcludedNodes
        {
            get { return _excludedNodes; }
        }

        public QueryPathParametersToNode(Guid node, int minLength, int maxLength, List<Guid> includedNodes, List<Guid> excludedNodes)
        {
            _nodeId = node;
            _minLength = minLength;
            _maxLength = maxLength;
            _includedNodes = includedNodes;
            _excludedNodes = excludedNodes;
        }
    }

    [SerializableAttribute]
    public class QueryPathParameters : IQueryParameters
    {
        private Guid _fromNodeId;
        private List<QueryPathParametersToNode> _toNodes;
        private int _minLength;
        private int _maxLength;
        private List<Guid> _includedNodes;
        private List<Guid> _excludedNodes;

        private int _maxResultLimit;
        private int _maxGraphLimit;
        private int _timeoutLimit;

        public Guid FromNodeId
        {
            get { return _fromNodeId; }
        }

        public List<QueryPathParametersToNode> ToNodes
        {
            get { return _toNodes; }
        }

        public int MinLength
        {
            get { return _minLength; }
        }

        public int MaxLength
        {
            get { return _maxLength; }
        }

        public List<Guid> IncludedNodes
        {
            get { return _includedNodes; }
        }

        public List<Guid> ExcludedNodes
        {
            get { return _excludedNodes; }
        }

        public int MaxResultLimit
        {
            get { return _maxResultLimit; }
        }

        public int MaxGraphLimit
        {
            get { return _maxGraphLimit; }
        }

        public int TimeoutLimit
        {
            get { return _timeoutLimit; }
        }

        private QueryPathParameters()
        { }

        public QueryPathParameters(Guid fromNode, List<QueryPathParametersToNode> toNodes, int minLength, int maxLength, List<Guid> includedNodes, List<Guid> excludedNodes, int maxResultLimit, int maxGraphLimit, int timeoutLimit)
        {
            _fromNodeId = fromNode;
            _toNodes = toNodes;
            _minLength = minLength;
            _maxLength = maxLength;
            _includedNodes = includedNodes;
            _excludedNodes = excludedNodes;

            _maxResultLimit = maxResultLimit;
            _maxGraphLimit = maxGraphLimit;
            _timeoutLimit = timeoutLimit;
        }

        public void VerifySatisfiability(IGraph graph)
        {
            List<string> errorStrings = new List<string>();
            int k = ToNodes.Count;
            List<Guid> N = new List<Guid>();
            N.Add(FromNodeId);
            foreach(QueryPathParametersToNode toNode in ToNodes)
                N.Add(toNode.NodeId);

            // Individual Length Restrictions for L(P)
            if(!(MinLength >= 0))
                errorStrings.Add("The minimum length over the entire path must be greater than or equal to zero (Individual Length Restriction)");
            if(!(MaxLength >= 0))
                errorStrings.Add("The maximum length over the entire path must be greater than or equal to zero (Individual Length Restriction)");
            if(!(MinLength <= MaxLength))
                errorStrings.Add("The minimum length over the entire path must be less than or equal to the maximum length (Individual Length Restriction)");

            // Individual Length Restrictions for L(i), i = 1, 2, ..., k
            foreach(QueryPathParametersToNode toNode in ToNodes)
            {
                if(!(toNode.MinLength >= 0))
                    errorStrings.Add(String.Format("The minimum length for the segment ending at <i>{0}</i> must be greater than or equal to zero (Individual Length Restriction)", graph.GetNodeLabel(toNode.NodeId)));
                if(!(toNode.MaxLength >= 0))
                    errorStrings.Add(String.Format("The maximum length for the segment ending at <i>{0}</i> must be greater than or equal to zero (Individual Length Restriction)", graph.GetNodeLabel(toNode.NodeId)));
                if(!(toNode.MinLength <= toNode.MaxLength))
                    errorStrings.Add(String.Format("The minimum length for the segment ending at <i>{0}</i> must be less than or equal to the maximum length (Individual Length Restriction)", graph.GetNodeLabel(toNode.NodeId)));
            }

            // Segment Length Restriction with the Overall Path Length Restriction
            double LPmin = MinLength + (graph.GetNode(FromNodeId).Type != graph.GetNode(ToNodes[ToNodes.Count - 1].NodeId).Type ? 0.5 : 0.0);
            double LPmax = MaxLength + (graph.GetNode(FromNodeId).Type != graph.GetNode(ToNodes[ToNodes.Count - 1].NodeId).Type ? 0.5 : 0.0);
            double LKmin = 0;
            double LKmax = 0;
            INode tempFromNode = graph.GetNode(FromNodeId);
            INode tempToNode = null;
            foreach(QueryPathParametersToNode toNode in ToNodes)
            {
                tempToNode = graph.GetNode(toNode.NodeId);
                LKmin += toNode.MinLength + (tempFromNode.Type != tempToNode.Type ? 0.5 : 0.0);
                LKmax += toNode.MaxLength + (tempFromNode.Type != tempToNode.Type ? 0.5 : 0.0);
                tempFromNode = tempToNode;
            }
            if(!((LKmin <= LPmax) && (LPmin <= LKmax)))
                errorStrings.Add("There is an inconsistency between the sum of the lengths of the individual segments and the length of the total path; adjust any length values accordingly");

            // Individual Inclusion and Exclusion Restrictions for L(P)
            foreach(Guid includedNode in IncludedNodes)
                foreach (Guid excludedNode in ExcludedNodes)
                    if(includedNode.Equals(excludedNode))
                        errorStrings.Add(String.Format("The node <i>{0}</i> appears in both the including and excluding sets over the entire path; please remove one or the other", graph.GetNodeLabel(includedNode)));

            // Individual Inclusion and Exclusion Restrictions for L(i), i = 1, 2, ..., k
            foreach(QueryPathParametersToNode toNode in ToNodes)
                foreach (Guid includedNode in toNode.IncludedNodes)
                    foreach (Guid excludedNode in toNode.ExcludedNodes)
                        if(includedNode.Equals(excludedNode))
                            errorStrings.Add(String.Format("The node <i>{0}</i> appears in both the including and excluding sets for the segment ending at <i>{1}</i>; please remove one or the other", graph.GetNodeLabel(includedNode), graph.GetNodeLabel(toNode.NodeId)));

            // From or To Node Conflicts with Exclusion Restrictions (E(P) intersect N = empty set)
            foreach (Guid excludedNode in ExcludedNodes)
                foreach (Guid nNode in N)
                    if(excludedNode.Equals(nNode))
                        errorStrings.Add(String.Format("The node <i>{0}</i> appears as both a segment endpoint and an excluded node over the entire path; please remove either the excluded node or the segment involved", graph.GetNodeLabel(excludedNode)));

            // From or To Node Conflicts with Exclusion Restrictions (To(i) is not a member of E(i), i = 1, 2, ..., k)
            foreach(QueryPathParametersToNode toNode in ToNodes)
                foreach (Guid excludedNode in toNode.ExcludedNodes)
                    if(toNode.NodeId.Equals(excludedNode))
                        errorStrings.Add(String.Format("The node <i>{0}</i> appears as both a segment endpoint and an excluded node in the same segment; please remove either the excluded node or the segment involved", graph.GetNodeLabel(toNode.NodeId)));

            // From or To Node Conflicts with Exclusion Restrictions (To(i-1) is not a member of E(i), i = 2, 3, ..., k)
            for(int i = 1; i < ToNodes.Count; i++)
                foreach (Guid excludedNode in ToNodes[i].ExcludedNodes)
                    if(ToNodes[i - 1].NodeId.Equals(excludedNode))
                        errorStrings.Add(String.Format("The node <i>{0}</i> appears as both a segment endpoint and an excluded node in the same segment; please remove either the excluded node or the segment involved", graph.GetNodeLabel(ToNodes[i - 1].NodeId)));

            // From or To Node Conflicts with Exclusion Restrictions (From is not a member of E(1))
            foreach(Guid excludedNode in ToNodes[0].ExcludedNodes)
                if(excludedNode.Equals(FromNodeId))
                    errorStrings.Add(String.Format("The node <i>{0}</i> appears as both the from node and an excluded node in the first segment; please either change the from node or remove the excluded node from the first segment", graph.GetNodeLabel(FromNodeId)));

            // Segment Inclusion/Exclusion Restrictions with the Overall Path Inclusion/Exclusion Restriction (E(P) intersect (The union from i = 1 to k of I(i)) = empty set)
            foreach (Guid excludedNode in ExcludedNodes)
                foreach(QueryPathParametersToNode toNode in ToNodes)
                    foreach (Guid includedNode in IncludedNodes)
                        if(excludedNode.Equals(includedNode))
                            errorStrings.Add(String.Format("The node <i>{0}</i> appears in the inclusion set for the segment ending at <i>{1}</i> and in the exclusion set over the entire path; please remove one restriction or the other", graph.GetNodeLabel(excludedNode), graph.GetNodeLabel(toNode.NodeId)));

            // Segment Inclusion/Exclusion Restrictions with the Overall Path Inclusion/Exclusion Restriction (E(P) intersect (The intersection from i = 1 to k of I(i)) = empty set)
            Dictionary<Guid, bool> intersectionEiDict = new Dictionary<Guid, bool>(); // (new EqualityComparerINode());
            // Start computing the intersection by having the first toNode set in the dictionary
            foreach (Guid e1Node in ToNodes[0].ExcludedNodes)
                intersectionEiDict.Add(e1Node, false);
            // Next, for each successive excluding set, check the nodes with the dictionary and remove any that aren't seen in a single pass
            for(int i = 1; i < ToNodes.Count; i++)
            {
                foreach (Guid eiNode in ToNodes[i].ExcludedNodes)
                    if(intersectionEiDict.ContainsKey(eiNode))
                        intersectionEiDict[eiNode] = true;

                List<Guid> intersectionEiDictKeys = new List<Guid>(intersectionEiDict.Keys);
                List<bool> intersectionEiDictValues = new List<bool>(intersectionEiDict.Values);

                for(int j = 0; j < intersectionEiDictKeys.Count; j++)
                {
                    if(!intersectionEiDictValues[j])
                        intersectionEiDict.Remove(intersectionEiDictKeys[j]);
                    else
                        intersectionEiDict[intersectionEiDictKeys[j]] = false;
                }
            }
            // Now, perform the verification for this restriction
            foreach (Guid includedNode in IncludedNodes)
                foreach (Guid excludedNode in intersectionEiDict.Keys)
                    if(includedNode.Equals(excludedNode))
                        errorStrings.Add(String.Format("The node <i>{0}</i> appears in the exclusion set for all of the individual segments and in the inclusion set over the entire path; please remove one restriction or the other", graph.GetNodeLabel(includedNode)));

            if(errorStrings.Count > 0)
                throw new SatisfiabilityException(String.Format("Unsatisfiable Query - {0}", String.Join("; ", errorStrings.ToArray())));

            return;
        }
    }
}