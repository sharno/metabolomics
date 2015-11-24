using System;

namespace Metabol.Util.SimpleCycle
{
    public class Edge
    {
        public Guid Id = Guid.NewGuid();
        public Node Source;
        public Node Destination;

        public bool IsReversible;

        public Edge(Node source, Node destination, bool isReversible)
        {
            Source = source;
            Destination = destination;
            IsReversible = isReversible;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.Id.Equals(((Edge)obj).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}