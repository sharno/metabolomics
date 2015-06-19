using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;

using AQILib;
using AQILib.Gui;


namespace PathwaysLib.AQI
{
    // For an example input class with comments, see QInputMoleculeAmount
    public class QInputMoleculeNameType : QInputSelect
    {
        public QInputMoleculeNameType()
            : base("nameType", new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputMoleculeNameType(string name)
            : base(name, new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputMoleculeNameType(string name, string defaultValue)
            : base(name, defaultValue, new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputMoleculeNameType(QInputMoleculeNameType input)
            : base(input)
        { }

        public override Dictionary<string, string> GetInitialValues()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            retVal.Add("primary name", "Primary name");
            retVal.Add("common name", "Common name");
            retVal.Add("other name", "Other name");
            retVal.Add("gene symbol", "Gene symbol");
            return retVal;
        }

        public override object Clone()
        {
            return new QInputMoleculeNameType(this);
        }
    }
}