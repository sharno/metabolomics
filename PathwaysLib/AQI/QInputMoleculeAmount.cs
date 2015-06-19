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
        /// <summary>
        /// Default constructor: Defines a default input name and renderer
        /// </summary>
        public QInputMoleculeAmount()
            : base("amount", new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputMoleculeAmount(string name)
            : base(name, new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputMoleculeAmount(string name, string defaultValue)
            : base(name, defaultValue, new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        /// <summary>
        /// A copy constructor used for cloning
        /// </summary>
        /// <param name="input"></param>
        protected QInputMoleculeAmount(QInputMoleculeAmount input)
            : base(input)
        { }

        // Used by the base QInput class
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