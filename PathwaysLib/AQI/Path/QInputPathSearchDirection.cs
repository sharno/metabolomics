using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    public class QInputPathSearchDirection : QInputDropDown
    {
        public QInputPathSearchDirection()
            : base("search_direction", new QInputDropDownRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathSearchDirection(string name)
            : base(name, new QInputDropDownRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathSearchDirection(string name, string defaultValue)
            : base(name, defaultValue, new QInputDropDownRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputPathSearchDirection(QInputPathSearchDirection input)
            : base(input)
        { }

        public override Dictionary<string,string> GetInitialValues()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();

            retVal.Add("Undirected", "Undirected");
            retVal.Add("Downstream", "Downstream");
            retVal.Add("Upstream", "Upstream");

            return retVal;
        }

        public override object Clone()
        {
            return new QInputPathSearchDirection(this);
        }
    }
}