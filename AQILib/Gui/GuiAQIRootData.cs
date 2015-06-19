using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace AQILib.Gui
{
    public class GuiAQIRootData : IGuiData
    {
        private PlaceHolder _query;
        private CollapsiblePanel _results;
        private CollapsiblePanel _tips;
        private QNodeIdCounter _idCounter;
        private IAQIUtil _util;

        private GuiAQIRootData()
        { }

        public GuiAQIRootData(PlaceHolder query, CollapsiblePanel results, CollapsiblePanel tips, QNodeIdCounter idCounter, IAQIUtil util)
        {
            _query = query;
            _results = results;
            _tips = tips;
            _idCounter = idCounter;
            _util = util;
        }

        public PlaceHolder Query
        {
            get { return _query; }
        }

        public CollapsiblePanel Results
        {
            get { return _results; }
        }

        public CollapsiblePanel Tips
        {
            get { return _tips; }
        }

        public QNodeIdCounter IdCounter
        {
            get { return _idCounter; }
        }

        public IAQIUtil Util
        {
            get { return _util; }
        }

    }
}