using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// This class is an equality comparer that must be used with any dictionary storing INodes in order to use the custom Equals method versus the built-in equality method.
    /// We need to use the custom equality because of the node cloning and the fact that the source (and destination) nodes are generated on-demand and are not picked off the graph, therefore making two "equivalent" objects be different internal objects.
    /// </summary>
    [SerializableAttribute]
    public class EqualityComparerINode : IEqualityComparer<INode>
    {
        public bool Equals(INode x, INode y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(INode obj)
        {
            return (obj.Id.GetHashCode() + obj.Type.GetHashCode()) / 2;
        }
    }
}