using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;

using AQILib;
using AQILib.Gui;

namespace PathwaysLib.AQI
{
    // For an example input class with comments, see QInputMoleculeAmount
    public class QInputMoleculeType : QInputSelect
    {
        public QInputMoleculeType()
            : base("type", new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputMoleculeType(string name)
            : base(name, new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputMoleculeType(string name, string defaultValue)
            : base(name, defaultValue, new QInputSelectRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputMoleculeType(QInputMoleculeType input)
            : base(input)
        { }

        public override Dictionary<string, string> GetInitialValues()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            retVal.Add("amino_acids", "Amino Acids");
            retVal.Add("basic_molecules", "Basic molecules");
            retVal.Add("genes", "Genes");
            retVal.Add("proteins", "Proteins");
            retVal.Add("rnas", "RNAs");
            return retVal;
        }

        public override object Clone()
        {
            return new QInputMoleculeType(this);
        }
    }
}