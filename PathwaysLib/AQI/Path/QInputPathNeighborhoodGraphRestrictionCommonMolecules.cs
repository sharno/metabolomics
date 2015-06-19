using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    public class QInputPathNeighborhoodGraphRestrictionCommonMolecules : QInputCheckbox
    {
        public QInputPathNeighborhoodGraphRestrictionCommonMolecules()
            : base("common_molecules", new QInputCheckboxRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathNeighborhoodGraphRestrictionCommonMolecules(string name)
            : base(name, new QInputCheckboxRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathNeighborhoodGraphRestrictionCommonMolecules(string name, string defaultValue)
            : base(name, defaultValue.Equals("true"), new QInputCheckboxRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputPathNeighborhoodGraphRestrictionCommonMolecules(QInputPathNeighborhoodGraphRestrictionCommonMolecules input)
            : base(input)
        { }

        public override object Clone()
        {
            return new QInputPathNeighborhoodGraphRestrictionCommonMolecules(this);
        }
    }
}