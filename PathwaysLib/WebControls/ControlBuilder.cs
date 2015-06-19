using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace PathwaysLib.WebControls
{
	/// <summary>
	/// Builds simple controls for things like Literals and the like.
	/// </summary>
	public class ControlBuilder
	{
		/// <summary>
		/// Default constructor... not used.
		/// </summary>
		public ControlBuilder() {}

		/// <summary>
		/// Turns a string of text into a literal control.
		/// </summary>
		/// <param name="text">The HTML for the control.</param>
		/// <returns>A literal control.</returns>
		public static Literal BuildLiteral( string text )
		{
			Literal lit = new Literal();
			lit.Text = text;
			return lit;
		}
	}
}