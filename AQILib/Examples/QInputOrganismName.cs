using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;

using AQILib;
using AQILib.Gui;

namespace PathwaysLib.AQI
{
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

        //public override Dictionary<string, string> GetValues(string currentValue, IAQIUtil util)
        //{
        //    Dictionary<string, string> results = new Dictionary<string, string>();

        //    foreach(ServerOrganismGroup sog in ServerOrganismGroup.AllOrganisms())
        //        if(sog.Name.Contains(currentValue))
        //            results.Add(sog.ID.ToString(), sog.Name);

        //    return results;
        //}

        public override object Clone()
        {
            return new QInputOrganismName(this);
        }
    }
}