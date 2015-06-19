using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
//using PathwaysLib.Utilities;

namespace AQILib.Gui
{
	/// <summary>
	/// A button to trigger collapsing panels; DEPRECATED **
	/// </summary>
	[DefaultProperty("Title"), 
		ToolboxData("<{0}:CollapsiblePanelButton Runat=\"server\"></{0}:CollapsiblePanelButton>")]
	public class CollapsiblePanelButton : System.Web.UI.WebControls.Panel
	{
		private string title = null;
		private bool collapsed = false;	
		private bool showToggle = true;
		private string collapseType = "block";
		private string collapseImage = "../images/collapse.gif";
		private string expandImage = "../images/expand.gif";
		private string targetid = null;
		private string toggleImage = "../images/collapse.gif";

		private ControlCollection titleControls = null;

		/// <summary>
		/// Default constructor
		/// </summary>
		public CollapsiblePanelButton() {}

		/// <summary>
		/// 
		/// </summary>
		[Bindable(true),
			Category("Appearance"), 
			DefaultValue(null),
			Description("The title string to display.")] 
		public string Title
		{
			get
			{
				return title;
			}

			set
			{
				title = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Bindable(true), 
			Category("Appearance"), 
			DefaultValue(false),
			Description("Whether the panel is currently collapsed.")] 
		public bool Collapsed
		{
			get
			{
				return collapsed;
			}

			set
			{
				collapsed = value;
				if (collapsed)
				{
					toggleImage = expandImage;
				}
				else
				{
					toggleImage = collapseImage;
				}

			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Bindable(true), 
			Category("Appearance"), 
			DefaultValue("block"),
			Description("The CSS display type to use when re-expanding the panel.")] 
		public string CollapseType
		{
			get
			{
				return collapseType;
			}

			set
			{
				collapseType = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Bindable(true), 
			Category("Appearance"), 
			DefaultValue("../images/expand.gif"),
			Description("The image to use when the panel can be expanded.")]
		public string ExpandImage
		{
			get
			{
				return expandImage;
			}

			set
			{
				expandImage = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Bindable(true), 
			Category("Appearance"), 
			DefaultValue("../images/collapse.gif"),
			Description("The image to use when the panel can be collapsed.")] 
		public string CollapsImage
		{
			get
			{
				return collapseImage;
			}

			set
			{
				collapseImage = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Bindable(true),
			Category("Appearance"),
			DefaultValue(null),
			Description("The ID of the object this will collapse/expand when clicked.")]
		public string TargetID
		{
			get
			{
				return targetid;
			}

			set
			{
				targetid = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Bindable(true), 
			Category("Appearance"), 
			DefaultValue(true),
			Description("Whether or not to show the expand/collapse button.")] 
		public bool ShowToggle
		{
			get
			{
				return showToggle;
			}

			set
			{
				showToggle = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ControlCollection TitleControls
		{
			get
			{
				if(titleControls == null)
					titleControls = new ControlCollection(this);
				return titleControls;
			}
		}

		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output">The HTML writer to write out to.</param>
		protected override void Render(HtmlTextWriter output)
		{
			output.WriteBeginTag("span");
			output.WriteAttribute("class", "collapseitemdetail");
			output.WriteAttribute("onclick", "javascript:toggleElementPic('" + targetid + "','img_" + targetid + "','" + collapseType + "')");
			output.WriteAttribute("title", "Click to expand/collapse");
			output.Write(HtmlTextWriter.TagRightChar);

			if(showToggle)
			{
				output.WriteBeginTag("img");
				output.WriteAttribute("src", toggleImage);
				output.WriteAttribute("id", "img_" + targetid);
				output.WriteAttribute("class", "collapsebutton");
				output.WriteAttribute("alt", "Click to expand/collapse");
				output.WriteAttribute("title", "Click to expand/collapse");
				output.Write(HtmlTextWriter.SelfClosingTagEnd);
				output.Write(" ");
			}

			output.Write(title);
			output.WriteEndTag("span");

			if(titleControls != null && titleControls.Count > 0)
			{
				foreach(Control c in titleControls)
				{
					c.RenderControl(output);
				}
			}
		}
	}


	/// <summary>
	/// A panel that can expand and collapse when its title is clicked.  Supports Ajax loading.
	/// </summary>
	[DefaultProperty("Title"), 
	ToolboxData("<{0}:CollapsiblePanel Runat=\"server\" />")]
	public class CollapsiblePanel : System.Web.UI.WebControls.Panel
	{
		private string title = string.Empty;
		private bool collapsed = false;	
		private bool collapsible = true;
		private bool toggleOnly = false;
		private bool autoload = false;
		private bool forcedautoload = false;
		private bool showToggle = true;
		private string collapseType = "block";
		private string collapseImage = "../Images/collapse.gif";
		private string expandImage = "../Images/expand.gif";
		private string currentImage = "../Images/collapse.gif";
		private Hashtable arguments = null;
		//private ControlCollection controls = null;
		private ControlCollection buttons = null;
		private string controlName = string.Empty;

		/// <summary>
		/// Default constructor; sets the initial CSS class.
		/// </summary>
		public CollapsiblePanel()
		{
			this.CssClass = "itemdisplay";
		}

        public override bool HasControls()
        {
            return base.HasControls() || (buttons != null && buttons.Count > 0);
        }

		/// <summary>
		/// The title string to display.
		/// </summary>
		[Bindable(true), Category("Appearance"), DefaultValue(null),
			Description("The title string to display.")] 
		public string Title
		{
			get{ return title; }
			set{ title = value; }
		}

		/// <summary>
		/// Whether the panel is currently collapsed.
		/// </summary>
		[Bindable(true), Category("Appearance"), DefaultValue(false),
			Description("Whether the panel is currently collapsed.")] 
		public bool Collapsed
		{
			get{ return collapsed; }
			set
			{
				collapsed = value;
				if( collapsed ) currentImage = expandImage;
				else currentImage = collapseImage;
			}
		}

		/// <summary>
		/// Whether the panel can be collapsed.
		/// </summary>
		[Bindable(true), Category("Appearance"), DefaultValue(true),
		Description("Whether the panel can be collapsed.")] 
		public bool Collapsible
		{
			get{ return collapsible; }
			set{ collapsible = value; }
		}

		/// <summary>
		/// Whether the panel can be collapsed only by clicking the toggle button.
		/// </summary>
		[Bindable(true), Category("Appearance"), DefaultValue(true),
		Description("Whether the panel can be collapsed only by clicking the toggle button.")]
		public bool ToggleOnly
		{
			get { return toggleOnly; }
			set { toggleOnly = value; }
		}

		/// <summary>
		/// Whether this panel should automatically load after the page has loaded.
		/// </summary>
		[Bindable(true), Category("Settings"), DefaultValue(false),
		Description("Whether this panel should automatically load after the page has loaded.")]
		public bool AutoLoad
		{
			get{ return autoload; }
			set{ autoload = value; }
		}

		/// <summary>
		/// Whether this panel should automatically load immediately after its control is built.
		/// </summary>
		[Bindable(true), Category("Settings"), DefaultValue(false),
		Description("Whether this panel should automatically load immediately after its control is built.")]
		public bool ForcedAutoLoad
		{
			get{ return forcedautoload; }
			set{ forcedautoload = value; }
		}

		/// <summary>
		/// The CSS display type to use when re-expanding the panel.
		/// </summary>
		[Bindable(true), Category("Appearance"), DefaultValue("block"),
			Description("The CSS display type to use when re-expanding the panel.")] 
		public string CollapseType
		{
			get{ return collapseType; }
			set{ collapseType = value; }
		}

		/// <summary>
		/// The image to use when the panel can be expanded.
		/// </summary>
		[Bindable(true), Category("Appearance"), DefaultValue("../Images/expand.gif"),
			Description("The image to use when the panel can be expanded.")]
		public string ExpandImage
		{
			get{ return expandImage; }
			set{ expandImage = value; }
		}

		/// <summary>
		/// The image to use when the panel can be collapsed.
		/// </summary>
		[Bindable(true), Category("Appearance"), DefaultValue("../Images/collapse.gif"),
			Description("The image to use when the panel can be collapsed.")] 
		public string CollapseImage
		{
			get{ return collapseImage; }
			set{ collapseImage = value; }
		}

		/// <summary>
		/// A hashtable of additional arguments to pass when invoked.
		/// </summary>
		[Bindable(false), Category("Settings"), DefaultValue(null),
			Description("A hashtable of additional arguments to pass when invoked.")]
		public Hashtable Arguments
		{
			get{ return arguments; }
			set{ arguments = value; }
		}

		/// <summary>
		/// Whether or not to show the expand/collapse button.
		/// </summary>
		[Bindable(true), Category("Appearance"), DefaultValue(true),
			Description("Whether or not to show the expand/collapse button.")] 
		public bool ShowToggle
		{
			get{ return showToggle; }
			set{ showToggle = value; }
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
                //if (controls == null)
                //{
                //    controls = new ControlCollection(this);
                //}
                //return controls;
                return base.Controls;
			}
		}

		/// <summary>
		/// Controls this panel will render as "buttons" in the upper-right of the panel.
		/// </summary>
		[Bindable(false), Category("Settings"), DefaultValue(null),
			Description("Controls this panel will render as buttons in the upper-right of the panel.")]
		public ControlCollection Buttons
		{
			get
			{
				if( buttons == null ) 
                    buttons = new ControlCollection( this );
				return buttons;
			}
		}

		/// <summary>
		/// The name of the control to call via JavaScript when collapsing/expanding.
		/// </summary>
		[Bindable(true), Category("Settings"), DefaultValue(null),
			Description("The name of the control to call via JavaScript when collapsing/expanding.")]
		public string ControlName
		{
			get{ return controlName; }
			set{ controlName = value; }
		}

		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output">The HTML writer to write out to.</param>
		protected override void Render( HtmlTextWriter output )
		{
			output.WriteBeginTag( "div" );
			if( CssClass != string.Empty ) output.WriteAttribute( "class", CssClass );
            List<string> styleStrings = new List<string>();
            foreach(object key in Style.Keys)
            {
                string keyStr = (string) key;
                styleStrings.Add(String.Format("{0}: {1}", keyStr, Style[keyStr]));
            }
            if(styleStrings.Count > 0) output.WriteAttribute("style", String.Format("{0};", String.Join("; ", styleStrings.ToArray())));
			output.WriteAttribute( "id", this.ID );
			output.Write( HtmlTextWriter.TagRightChar );
			output.Write( "\n" );
			
			output.WriteBeginTag( "span" );
			output.WriteAttribute( "class", "collapseitemdetail" );
			if( collapsible && !toggleOnly )
			{
				output.WriteAttribute( "onclick", string.Format( "toggleRegion('{0}','{1}','{2}','{3}',true)",
                    this.ID, controlName, arguments, collapseType)); //NOTE: process args before setting value of CollapsiblePanel! PaginationHelper.ArgumentList( arguments )
				output.WriteAttribute( "title", "Click to expand/collapse" );
			}
			output.Write( HtmlTextWriter.TagRightChar );

			if( collapsible && showToggle )
			{
				output.WriteBeginTag( "img" );
				output.WriteAttribute( "src", currentImage );
				output.WriteAttribute( "id", this.ID + "Image" );
				output.WriteAttribute( "class", "collapsebutton" );
				output.WriteAttribute( "alt", "Click to expand/collapse" );
				output.WriteAttribute( "title", "Click to expand/collapse" );
				if( toggleOnly )
				{
					output.WriteAttribute( "onclick", string.Format( "toggleRegion('{0}','{1}','{2}','{3}',true)",
                        this.ID, controlName, arguments, collapseType)); //NOTE: process args before setting value of CollapsiblePanel! PaginationHelper.ArgumentList( arguments )
				}
				output.Write( HtmlTextWriter.SelfClosingTagEnd );
			}

			output.WriteBeginTag( "span" );
			output.WriteAttribute( "id", this.ID + "Title" );
			output.Write( HtmlTextWriter.TagRightChar );
			output.Write( title );
			output.WriteEndTag( "span" );

			output.WriteEndTag( "span" );
			output.Write( "\n" );

			if( buttons != null )
			{
				output.WriteBeginTag( "div" );
				output.WriteAttribute( "class", "panelaction" );
				output.Write( HtmlTextWriter.TagRightChar );

                foreach (Control b in buttons)
                {
                    b.RenderControl(output);
                }

				output.WriteEndTag( "div" );
				output.Write( "\n" );
			}

			output.WriteBeginTag( "div" );
			output.WriteAttribute( "id", this.ID + "Region" );
			if( collapsed ) output.WriteAttribute( "style", "display:none" );
			output.Write( HtmlTextWriter.TagRightChar );
			output.Write( "\n" );

			if( title.IndexOf( "notloaded" ) != -1 )
			{
				output.WriteBeginTag( "a" );
				output.WriteAttribute( "id", this.ID + "Loading" );
				output.Write( HtmlTextWriter.TagRightChar );
				output.WriteEndTag( "a" );
				output.Write( "\n" );
			}

            if (Controls != null)
            {
                foreach (Control c in Controls)
                {
                    c.RenderControl(output);
                }
            }

			output.WriteEndTag( "div" );
			output.Write( "\n" );

			if( autoload || forcedautoload )
			{
				output.WriteBeginTag( "img" );
				output.WriteAttribute( "src", "../Images/loading.gif" );
				output.WriteAttribute( "style", "display:none" );
				output.WriteAttribute( "onload", string.Format( "{4}('{0}','{1}','{2}','{3}')",
					this.ID, controlName, arguments, collapseType,
                    (forcedautoload ? "toggleRegion" : "addToQueue")));  //NOTE: process args before setting value of CollapsiblePanel! PaginationHelper.ArgumentList( arguments )
				output.WriteAttribute( "alt", "" );
				output.Write( HtmlTextWriter.SelfClosingTagEnd );
			}

			output.WriteEndTag( "div" );
			output.Write( "\n" );
		}
	}
}