using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    public class SingleRootQuerier : Querier
    {
        public override IQueryResults Query(QNode node)
        {
            if(node.Children.Count > 0)
                return node.Children[0].Query();
            else
                return null;
        }
    }
}