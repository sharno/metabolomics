using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// A container for the parameters to the neighborhood query
    /// </summary>
    [SerializableAttribute]
    public class QueryNeighborhoodParameters : IQueryParameters
    {
        private int _minLength;
        private int _maxLength;
        private Guid _startNode;
        private List<Guid> _includedNodes;
        private List<Guid> _excludedNodes;
        private NodeType _findType;

        private int _maxResultLimit;
        private int _maxGraphLimit;
        private int _timeoutLimit;

        public int MinLength
        {
            get { return _minLength; }
        }

        public int MaxLength
        {
            get { return _maxLength; }
        }

        public Guid StartNodeId
        {
            get { return _startNode; }
        }

        public List<Guid> IncludedNodes
        {
            get { return _includedNodes; }
        }

        public List<Guid> ExcludedNodes
        {
            get { return _excludedNodes; }
        }

        public NodeType FindType
        {
            get { return _findType; }
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

        private QueryNeighborhoodParameters()
        { }

        public QueryNeighborhoodParameters(int minLength, int maxLength, Guid startNode, List<Guid> includedNodes, List<Guid> excludedNodes, NodeType findType, int maxResultLimit, int maxGraphLimit, int timeoutLimit)
        {
            _minLength = minLength;
            _maxLength = maxLength;
            _startNode = startNode;
            _includedNodes = includedNodes;
            _excludedNodes = excludedNodes;
            _findType = findType;

            _maxResultLimit = maxResultLimit;
            _maxGraphLimit = maxGraphLimit;
            _timeoutLimit = timeoutLimit;
        }

        public void VerifySatisfiability(IGraph graph)
        {
            List<string> errorStrings = new List<string>();

            // Individual Length Restrictions
            if(!(MinLength >= 0))
                errorStrings.Add("The minimum length must be greater than or equal to zero (Individual Length Restriction)");
            if(!(MaxLength >= 0))
                errorStrings.Add("The maximum length must be greater than or equal to zero (Individual Length Restriction)");
            if(!(MinLength <= MaxLength))
                errorStrings.Add("The minimum length must be less than or equal to the maximum length (Individual Length Restriction)");

            // Individual Inclusion Restrictions
            foreach(Guid includedNode in IncludedNodes)
                foreach (Guid excludedNode in ExcludedNodes)
                    if(includedNode.Equals(excludedNode))
                        errorStrings.Add(String.Format("The node <i>{0}</i> cannot appear in both the including and excluding restrictions (Individual Inclusion Restriction)", graph.GetNodeLabel(includedNode)));

            // From Node Conflicts with Overall Path Exclusion Restriction
            foreach (Guid excludedNode in ExcludedNodes)
                if(StartNodeId.Equals(excludedNode))
                    errorStrings.Add("The from node cannot also appear in the excluding restrictions (From Node Conflict with Exclusion Restriction)");

            if(errorStrings.Count > 0)
                throw new SatisfiabilityException(String.Format("Unsatisfiable Query - {0}", String.Join("; ", errorStrings.ToArray())));

            return;
        }
    }
}