using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;

using AQILib;
using AQILib.Gui;

namespace PathwaysLib.AQI
{
    public class QInputPathwayName : QInputAutoComplete
    {
        public QInputPathwayName()
            : base("name", new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathwayName(string name)
            : base(name, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathwayName(string name, string defaultValue)
            : base(name, defaultValue, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputPathwayName(QInputPathwayName input)
            : base(input)
        { }

        //public override Dictionary<string, string> GetValues(string currentValue, IAQIUtil util)
        //{
        //    Dictionary<string, string> retVal = new Dictionary<string, string>();

        //    foreach(ServerPathway spw in ServerPathway.FindPathways(currentValue, SearchMethod.Contains))
        //    {
        //        if(!retVal.ContainsValue(spw.Name.ToString()))
        //            retVal.Add(spw.ID.ToString(), spw.Name);
        //    }

        //    return retVal;
        //}

        public override object Clone()
        {
            return new QInputPathwayName(this);
        }
    }
}