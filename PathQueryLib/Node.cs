using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// This is a basic generic implementation of the INode interface
    /// </summary>
    [SerializableAttribute]
    public class Node : INode
    {
        private Guid _id;
        private NodeType _type;
        private string _label;

        public Guid Id
        {
            get { return _id; }
        }

        public NodeType Type
        {
            get { return _type; }
        }

        public string Label
        {
            get { return _label; }
        }

        private Node()
        { }

        public Node(Guid id, NodeType type)
        {
            _id = id;
            _type = type;
            _label = "";
        }

        public Node(Guid id, NodeType type, string label)
        {
            _id = id;
            _type = type;
            _label = label;
        }

        /// <summary>
        /// Creates a new node object with the same three parameters
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Node(this._id, this._type, this._label);
        }

        /// <summary>
        /// Equality based on the two IDs matching and the types matching. The names can be different!
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(obj == null || this.GetType() != obj.GetType())
                return false;

            Node nodeObj = (Node) obj;

            return (this.Id.Equals(nodeObj.Id) && this.Type == nodeObj.Type);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() + this.Type.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", _type.ToString(), _id.ToString(), _label);
        }

        /// <summary>
        /// Overrides the == operator to use the Equals method
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator==(Node o1, Node o2)
        {
            if(Object.ReferenceEquals(o1, o2))
                return true;

            if((Object) o1 == null || (Object) o2 == null)
                return false;

            return o1.Equals(o2);
        }

        /// <summary>
        /// Overrides the != operator to use the == operator, which in turn uses the Equals method
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator!=(Node o1, Node o2)
        {
            return !(o1 == o2);
        }
    }
}