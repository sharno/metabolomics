using System;
using System.Collections.Generic;
using System.Text;

using AQILib;
using AQILib.Gui;

namespace PathwaysLib.AQI.Path
{
    public class QInputPathFindType : QInputDropDown
    {
        public QInputPathFindType()
            : base("find_type", new QInputDropDownRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathFindType(string name)
            : base(name, new QInputDropDownRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathFindType(string name, string defaultValue)
            : base(name, defaultValue, new QInputDropDownRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputPathFindType(QInputPathFindType input)
            : base(input)
        { }

        public override Dictionary<string,string> GetInitialValues()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();

            retVal.Add(string.Empty, string.Empty);
            retVal.Add("Molecules", "Molecules");
            retVal.Add("Processes", "Processes");

            return retVal;
        }

        public override object Clone()
        {
            return new QInputPathFindType(this);
        }    
    }
}
