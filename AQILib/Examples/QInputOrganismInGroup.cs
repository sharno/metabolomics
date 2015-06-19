using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;

using AQILib;
using AQILib.Gui;

namespace PathwaysLib.AQI
{
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

        //public override Dictionary<string, string> GetValues(string currentValue, IAQIUtil util)
        //{
        //    Dictionary<string, string> results = new Dictionary<string, string>();

        //    foreach(ServerOrganismGroup sog in ServerOrganismGroup.AllOrganismsGroup())
        //        if(sog.Name.Contains(currentValue))
        //            results.Add(sog.ID.ToString(), sog.Name);

        //    return results;
        //}

        public override object Clone()
        {
            return new QInputOrganismInGroup(this);
        }
    }
}