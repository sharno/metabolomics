using System;
using System.Collections;
using System.Configuration;
using System.Web.UI.WebControls;
namespace PathwaysLib.WebControls
{
	/// <summary>
	/// A generic content panel from which other Ajax-aware controls derive from.
	/// </summary>
	public abstract class ContentPanel : System.Web.UI.UserControl
	{
		/// <summary>
		/// Default constructor... not really useful.
		/// </summary>
		public ContentPanel() {}

		/// <summary>
		/// What class to use when generating loading messages.
		/// </summary>
		protected string _loadStyle = "whitebg";
		
		/// <summary>
		/// Constructs all the stuff that will be placed in the content panel.
		/// </summary>
		/// <param name="args">A hashtable of arguments to process</param>
		/// <returns>A hashtable of additional data</returns>
		protected abstract Hashtable Build( Hashtable args );

		/// <summary>
		/// Wraps Build to allow for uniform error trapping.
		/// </summary>
		/// <param name="args">A hashtable of arguments to process</param>
		/// <returns>A hashtable of additional data</returns>
		public Hashtable BuildControl( Hashtable args )
		{
            try
            {
                return Build(args);
            }
            catch (Exception err)
            {
                Session["Error"] = err;
                Response.Redirect(ConfigurationManager.AppSettings["PathwaysWebBaseUrl"] + "/Web", true);
            }

            return null;
		}

		/// <summary>
		/// Determines what to display when there is nothing yet loaded.
		/// </summary>
		/// <param name="text">The string to display</param>
		/// <returns>A string of XHTML (that should be turned into a control)</returns>
		protected string NotLoadedBody( string text )
		{
			return "<div" + ( _loadStyle.Length > 0 ? " class=\"" + _loadStyle + "\"" : string.Empty ) + ">"
				+ text + " <img src=\"../Images/loading.gif\" alt=\"Loading...\" title=\"Loading...\" /></div>";
		}

		/// <summary>
		/// Determines what title to display when there is nothing yet loaded.
		/// </summary>
		/// <param name="text">The string to display</param>
		/// <returns>A string of XHTML</returns>
		protected string NotLoadedTitle( string text )
		{
			return "<span class=\"notloaded\">" + text + "</span>";
		}

        protected void datagrid_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            ListItemType itemType = e.Item.ItemType;
            if (itemType == ListItemType.Header)
            {
                e.Item.Cells[0].Width = Unit.Pixel(10);
                e.Item.Cells[0].Wrap = false;
            }
        } 
	}
}