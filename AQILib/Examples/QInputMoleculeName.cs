using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;

using AQILib;
using AQILib.Gui;


namespace PathwaysLib.AQI
{
    public class QInputMoleculeName : QInputAutoComplete
    {
        public QInputMoleculeName()
            : base("name", new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputMoleculeName(string name)
            : base(name, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputMoleculeName(string name, string defaultValue)
            : base(name, defaultValue, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputMoleculeName(QInputMoleculeName input)
            : base(input)
        { }

        //public override Dictionary<string, string> GetValues(string currentValue, IAQIUtil util)
        //{
        //    Dictionary<string, string> retVal = new Dictionary<string, string>();

        //    foreach(ServerMolecularEntity sme in ServerMolecularEntity.FindMolecularEntities(currentValue, SearchMethod.Contains, 30))
        //    {
        //        if(!retVal.ContainsValue(sme.Name.ToString()))
        //            retVal.Add(sme.ID.ToString(), sme.Name);
        //    }

        //    return retVal;
        //}

        public override object Clone()
        {
            return new QInputMoleculeName(this);
        }
    }
}