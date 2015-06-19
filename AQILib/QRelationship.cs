using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// A construct to model relationships between parent and child nodes
    /// </summary>
    public class QRelationship
    {
        #region Constraints

        public static bool ConstraintNotDuplicatedInPathToRoot(QNode parentNode, string childNodeTypeName)
        {
            bool retVal = true;

            for(QNode parent = parentNode; parent != null; parent = parent.Parent)
                if(parent.NodeTypeName == childNodeTypeName)
                    retVal = false;

            return retVal;
        }

        #endregion

        public delegate bool RelationshipConstraintDelegate(QNode parentNode, string childNodeTypeName);

        private QNode _parentNode;
        private string _childNodeTypeName;
        private List<RelationshipConstraintDelegate> _relationshipConstraints;

        public QNode ParentNode
        {
            get { return _parentNode; }
        }

        public string ChildNodeTypeName
        {
            get { return _childNodeTypeName; }
        }

        public bool IsValid
        {
            get
            {
                bool retVal = true;
                foreach(RelationshipConstraintDelegate relationshipConstraint in _relationshipConstraints)
                    retVal = retVal && relationshipConstraint(_parentNode, _childNodeTypeName);
                return retVal;
            }
        }

        private QRelationship() { }

        public QRelationship(QNode parentNode, string childNodeTypeName, List<RelationshipConstraintDelegate> relationshipConstraints)
        {
            _parentNode = parentNode;
            _childNodeTypeName = childNodeTypeName;
            _relationshipConstraints = relationshipConstraints;
        }
    }
}