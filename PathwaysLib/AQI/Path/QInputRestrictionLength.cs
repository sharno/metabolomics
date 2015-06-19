using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    public class QInputRestrictionLength : QInputDropDown
    {
        public QInputRestrictionLength()
            : base("restriction_length", new QInputDropDownRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputRestrictionLength(string name)
            : base(name, new QInputDropDownRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputRestrictionLength(string name, string defaultValue)
            : base(name, defaultValue, new QInputDropDownRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputRestrictionLength(QInputRestrictionLength input)
            : base(input)
        { }

        public override Dictionary<string, string> GetInitialValues()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            retVal.Add(String.Empty, String.Empty);
            retVal.Add("0", "0");
            retVal.Add("1", "1");
            retVal.Add("2", "2");
            retVal.Add("3", "3");
            retVal.Add("4", "4");
            retVal.Add("5", "5");
            retVal.Add("6", "6");
            retVal.Add("7", "7");
            retVal.Add("8", "8");
            retVal.Add("9", "9");
            retVal.Add("10", "10");
            retVal.Add("11", "11");
            retVal.Add("12", "12");
            retVal.Add("13", "13");
            retVal.Add("14", "14");
            retVal.Add("15", "15");
            retVal.Add("16", "16");
            retVal.Add("17", "17");
            retVal.Add("18", "18");
            retVal.Add("19", "19");
            retVal.Add("20", "20");
            return retVal;
        }

        public override object Clone()
        {
            return new QInputRestrictionLength(this);
        }
    }
}