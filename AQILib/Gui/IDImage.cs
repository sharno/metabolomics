using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace AQILib.Gui
{
    public class IDImage : Image
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