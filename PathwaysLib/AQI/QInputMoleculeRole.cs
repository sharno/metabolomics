using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;

using AQILib;
using AQILib.Gui;

namespace PathwaysLib.AQI
{
    // For an example input class with comments, see QInputMoleculeAmount
    public class QInputMoleculeRole : QInputSelect
    {
        public QInputMoleculeRole()
            : base("role", new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputMoleculeRole(string name)
            : base(name, new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputMoleculeRole(string name, string defaultValue)
            : base(name, defaultValue, new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputMoleculeRole(QInputMoleculeRole input)
            : base(input)
        { }

        public override Dictionary<string, string> GetInitialValues()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            retVal.Add("activator", "Activator");
            retVal.Add("cofactor", "Cofactor");
            retVal.Add("cofactor in", "Cofactor in");
            retVal.Add("cofactor out", "Cofactor out");
            retVal.Add("inhibitor", "Inhibitor");
            retVal.Add("product", "Product");
            retVal.Add("regulator", "Regulator");
            retVal.Add("substrate", "Substrate");
            return retVal;
        }

        public override object Clone()
        {
            return new QInputMoleculeRole(this);
        }
    }
}