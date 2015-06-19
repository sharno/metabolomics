using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;

using AQILib;
using AQILib.Gui;

namespace PathwaysLib.AQI
{
    // For an example input class with comments, see QInputMoleculeAmount
    // Contains no implementation of GetValues because this is now done by the querier
    public class QInputOrganismInGroup : QInputAutoComplete
    {
        public QInputOrganismInGroup()
            : base("group", new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputOrganismInGroup(string name)
            : base(name, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputOrganismInGroup(string name, string defaultValue)
            : base(name, defaultValue, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputOrganismInGroup(QInputOrganismInGroup input)
            : base(input)
        { }

        public override object Clone()
        {
            return new QInputOrganismInGroup(this);
        }
    }
}