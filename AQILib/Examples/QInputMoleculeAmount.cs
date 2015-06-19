using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;

using AQILib;
using AQILib.Gui;

namespace PathwaysLib.AQI
{
    public class QInputMoleculeAmount : QInputSelect
    {
        public QInputMoleculeAmount()
            : base("amount", new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputMoleculeAmount(string name)
            : base(name, new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputMoleculeAmount(string name, string defaultValue)
            : base(name, defaultValue, new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputMoleculeAmount(QInputMoleculeAmount input)
            : base(input)
        { }

        public override Dictionary<string, string> GetInitialValues()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            retVal.Add("1", "1");
            retVal.Add("2", "2");
            retVal.Add("3", "3");
            retVal.Add("6", "6");
            return retVal;
        }

        public override object Clone()
        {
            return new QInputMoleculeAmount(this);
        }
    }
}