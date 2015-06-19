using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;

using AQILib;
using AQILib.Gui;

namespace PathwaysLib.AQI
{
    public class QInputProcessName : QInputAutoComplete
    {
        public QInputProcessName()
            : base("name", new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputProcessName(string name)
            : base(name, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputProcessName(string name, string defaultValue)
            : base(name, defaultValue, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputProcessName(QInputProcessName input)
            : base(input)
        { }

        //public override Dictionary<string, string> GetValues(string currentValue, IAQIUtil util)
        //{
        //    Dictionary<string, string> retVal = new Dictionary<string, string>();

        //    foreach(ServerProcess sp in ServerProcess.FindProcesses(currentValue, SearchMethod.Contains, 30))
        //    {
        //        if(!retVal.ContainsValue(sp.Name.ToString()))
        //            retVal.Add(sp.GenericProcessID.ToString(), sp.Name);
        //    }

        //    return retVal;
        //}

        public override object Clone()
        {
            return new QInputProcessName(this);
        }
    }
}