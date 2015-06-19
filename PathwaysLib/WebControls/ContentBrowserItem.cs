using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using PathwaysLib.ServerObjects;
using PathwaysLib.Utilities;

namespace PathwaysLib.WebControls
{
    /// <summary>
    /// A control representing an item on the content browser, such as a pathway or process,
    /// that can contain its own subcontrols to display additional information.
    /// </summary>
    [DefaultProperty("Title"),
    ToolboxData("<{0}:ContentBrowserItem Runat=\"server\" />")]
    public class ContentBrowserItem : System.Web.UI.WebControls.Panel
    {
        private string title = string.Empty;
        private bool selected = false;
        private bool collapsed = true;
        private bool autoload = false;
        private bool forcedautoload = false;
        private bool showtoggle = true;
        private string collapsetype = "block";
        private string collapseimage = "../Images/collapse.gif";
        private string expandimage = "../Images/expand.gif";
        private string currentimage = "../Images/collapse.gif";
        private string noitemimage = "../Images/noitem.gif";
        private Hashtable arguments = null;
        private ControlCollection controls = null;
        private string controlname = string.Empty;
        private bool linkable = true;
        /// <summary>
        /// Default constructor; not all that useful
        /// </summary>
        public ContentBrowserItem()
        {
        }

        public ContentBrowserItem(bool linkable)
        {
            this.linkable = linkable;
        }

        /// <summary>
        /// The item name to display.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(null),
            Description("The item name to display.")]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Whether the item is currently selected.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(false),
            Description("Whether the item is currently selected.")]
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        /// <summary>
        /// Whether the item is collapsed.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(true),
            Description("Whether the item is collapsed.")]
        public bool Collapsed
        {
            get { return collapsed; }
            set
            {
                collapsed = value;
                if (collapsed) currentimage = expandimage;
                else currentimage = collapseimage;
            }
        }

        /// <summary>
        /// Whether this panel should automatically load after the page has loaded.
        /// </summary>
        [Bindable(true), Category("Settings"), DefaultValue(false),
            Description("Whether this panel should automatically load after the page has loaded.")]
        public bool AutoLoad
        {
            get { return autoload; }
            set { autoload = value; }
        }

        /// <summary>
        /// Whether this panel should automatically load immediately after its control is built.
        /// </summary>
        [Bindable(true), Category("Settings"), DefaultValue(false),
            Description("Whether this panel should automatically load immediately after its control is built.")]
        public bool ForcedAutoLoad
        {
            get { return forcedautoload; }
            set { forcedautoload = value; }
        }

        /// <summary>
        /// Whether or not to show the expand/collapse button.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(true),
            Description("Whether or not to show the expand/collapse button.")]
        public bool ShowToggle
        {
            get { return showtoggle; }
            set { showtoggle = value; }
        }

        /// <summary>
        /// The CSS display type to use when re-expanding the panel.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("block"),
        Description("The CSS display type to use when re-expanding the panel.")]
        public string CollapseType
        {
            get { return collapsetype; }
            set { collapsetype = value; }
        }

        /// <summary>
        /// The image to use when the panel can be expanded.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("../Images/expand.gif"),
        Description("The image to use when the panel can be expanded.")]
        public string ExpandImage
        {
            get { return expandimage; }
            set { expandimage = value; }
        }

        /// <summary>
        /// The image to use when the panel can be collapsed.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("../Images/collapse.gif"),
        Description("The image to use when the panel can be collapsed.")]
        public string CollapseImage
        {
            get { return collapseimage; }
            set { collapseimage = value; }
        }

        /// <summary>
        /// The image to use when the control toggle should not be displayed.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue("../Images/NoImage.gif"),
        Description("The image to use when the control toggle should not be displayed.")]
        public string NoItemImage
        {
            get { return noitemimage; }
            set { noitemimage = value; }
        }

        /// <summary>
        /// A hashtable of additional arguments to pass when clicked.
        /// </summary>
        [Bindable(false), Category("Settings"), DefaultValue(null),
            Description("A hashtable of additional arguments to pass when clicked.")]
        public Hashtable Arguments
        {
            get { return arguments; }
            set { arguments = value; }
        }

        /// <summary>
        /// Controls this panel will render.
        /// </summary>
        [Bindable(false), Category("Settings"), DefaultValue(null),
            Description("Controls this panel will render.")]
        public override ControlCollection Controls
        {
            get
            {
                if (controls == null) controls = new ControlCollection(this);
                return controls;
            }
        }

        /// <summary>
        /// The name of the control to call via JavaScript.
        /// </summary>
        [Bindable(true), Category("Settings"), DefaultValue(null),
            Description("The name of the control to call via JavaScript.")]
        public string ControlName
        {
            get { return controlname; }
            set { controlname = value; }
        }

        /// <summary> 
        /// Render this control to the output parameter specified.
        /// </summary>
        /// <param name="output">The HTML writer to write out to.</param>
        protected override void Render(HtmlTextWriter output)
        {
            LinkHelper LH;
            if (arguments["LH"] != null && arguments["LH"].GetType() != typeof(string))
            {
                LH = (LinkHelper)(arguments["LH"]);
            }
            else
            {
                LH = new LinkHelper(new Guid(arguments["viewid"].ToString()));
            }
            if (arguments.Contains("pwgid")) LH.SetParameter("pwgid", arguments["pwgid"].ToString());
            if (arguments.Contains("terms")) LH.SetParameter("terms", arguments["terms"].ToString());
            if (arguments.Contains("type")) LH.SetParameter("type", arguments["type"].ToString());
            if (arguments.Contains("page")) LH.SetParameter("page", arguments["page"].ToString());
            string graphtype = arguments["viewgraph"].ToString();
            Tribool graph = graphtype == "True" ? Tribool.True : graphtype == "False" ? Tribool.False : Tribool.Null;
            string displayitemtype = arguments["displayitemtype"].ToString() == "none" ? null : arguments["displayitemtype"].ToString();
            string node1type = arguments["node1type"].ToString() == "none" ? null : arguments["node1type"].ToString();
            string node2type = arguments["node2type"].ToString() == "none" ? null : arguments["node2type"].ToString();
            string node3type = arguments["node3type"].ToString() == "none" ? null : arguments["node3type"].ToString();

            output.WriteBeginTag("div");
            if (this.CssClass != string.Empty) output.WriteAttribute("class", this.CssClass);
            output.Write(HtmlTextWriter.TagRightChar);
            output.Write("\n");

            output.WriteBeginTag("img");
            output.WriteAttribute("src", (showtoggle ? currentimage : noitemimage));
            output.WriteAttribute("class", "collapsebutton");
            if (showtoggle)
            {
                output.WriteAttribute("id", this.ID + "Image");
                output.WriteAttribute("alt", "Click to expand/collapse");
                output.WriteAttribute("title", "Click to expand/collapse");
                output.WriteAttribute("onclick", string.Format("toggleRegion('{0}','{1}','{2}','{3}',true)",
                    this.ID, controlname, PaginationHelper.ArgumentList(arguments), collapsetype));
            }
            else
            {
                output.WriteAttribute("alt", string.Empty);
                output.WriteAttribute("title", string.Empty);
            }
            output.Write(HtmlTextWriter.SelfClosingTagEnd);
            output.Write(" ");


            output.WriteBeginTag("a");
            //output.WriteAttribute("href", 
           // LH.AlterOpenPathAndDisplayItem(new Guid(arguments["node1"].ToString()),
             // node1type, new Guid(arguments["node2"].ToString()),
             // node2type, new Guid(arguments["node3"].ToString()),
             // node3type, new Guid(arguments["displayitem"].ToString()),
             // displayitemtype, graph).Replace("&", "&amp;"));
            Hashtable openNode = new Hashtable();
            Hashtable openNodeType = new Hashtable();
            for (int i = 1; i <= Convert.ToInt32(arguments["level"].ToString()); i++)
            {
                openNode[i] = new Guid(arguments["node" + i].ToString());
                openNodeType[i] = arguments["node" + i + "type"].ToString() == "none" ? null : arguments["node" + i + "type"].ToString();
            }
            output.WriteAttribute("href", LH.AlterOpenPathAndDisplayItem2(openNode, openNodeType, new Guid(arguments["displayitem"].ToString()),
            displayitemtype, graph).Replace("&", "&amp;"));

            if (selected) output.WriteAttribute("class", "bold");

            output.WriteAttribute("id", this.ID + "Title");
            output.Write(HtmlTextWriter.TagRightChar);

            output.Write(title);

            output.WriteEndTag("a");

            output.WriteBeginTag("div");
            output.WriteAttribute("id", this.ID + "Region");
            if (collapsed) output.WriteAttribute("style", "display:none");
            output.Write(HtmlTextWriter.TagRightChar);
            output.Write("\n");

            if (title.IndexOf("notloaded") != -1)
            {
                output.WriteBeginTag("a");
                output.WriteAttribute("id", this.ID + "Loading");
                output.Write(HtmlTextWriter.TagRightChar);
                output.WriteEndTag("a");
                output.Write("\n");
            }

            if (controls != null) foreach (Control c in controls) c.RenderControl(output);

            output.WriteEndTag("div");
            output.Write("\n");

            if (autoload || forcedautoload)
            {
                output.WriteBeginTag("img");
                output.WriteAttribute("src", "../Images/loading.gif");
                output.WriteAttribute("style", "display:none");
                output.WriteAttribute("onload", string.Format("{4}('{0}','{1}','{2}','{3}')",
                    this.ID, controlname, PaginationHelper.ArgumentList(arguments), collapsetype,
                    (forcedautoload ? "toggleRegion" : "addToLoQueue")));
                output.WriteAttribute("alt", "");
                output.Write(HtmlTextWriter.SelfClosingTagEnd);
            }

            output.WriteEndTag("div");
            output.Write("\n");
        }
    }
}