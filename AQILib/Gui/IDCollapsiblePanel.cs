using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib.Gui
{
    public class IDCollapsiblePanel : CollapsiblePanel
    {
        // Idea for this from: http://west-wind.com/weblog/posts/4605.aspx

        /// <summary>
        /// UniqueID is overriden to equal ID.
        /// </summary>
        public override string UniqueID
        {
            get { return this.ID; }
        }

        /// <summary>
        /// ClientID is overriden to equal ID.
        /// </summary>
        public override string ClientID
        {
            get { return this.ID; }
        }
    }
}
