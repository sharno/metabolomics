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
    public class QInputOrganismName : QInputAutoComplete
    {
        public QInputOrganismName()
            : base("name", new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputOrganismName(string name)
            : base(name, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputOrganismName(string name, string defaultValue)
            : base(name, defaultValue, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputOrganismName(QInputOrganismName input)
            : base(input)
        { }

        public override object Clone()
        {
            return new QInputOrganismName(this);
        }
    }
}