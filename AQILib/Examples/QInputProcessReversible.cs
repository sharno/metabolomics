using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;

using AQILib;
using AQILib.Gui;

namespace PathwaysLib.AQI
{
    public class QInputProcessReversible : QInputSelect
    {
        public QInputProcessReversible()
            : base("reversible", new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputProcessReversible(string name)
            : base(name, new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputProcessReversible(string name, string defaultValue)
            : base(name, defaultValue, new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputProcessReversible(QInputProcessReversible input)
            : base(input)
        { }

        public override Dictionary<string, string> GetInitialValues()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            retVal.Add("True", "Yes");
            retVal.Add("False", "No");
            return retVal;
        }

        public override object Clone()
        {
            return new QInputProcessReversible(this);
        }
    }
}