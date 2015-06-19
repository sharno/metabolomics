using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MasterPagesLib {

	/// <summary>
	/// This control contains the content for a particular region
	/// </summary>
	[ToolboxData("<{0}:Content Runat=\"Server\"></{0}:Content>")]
	public class Content : PlaceHolder {

		internal string _templateSourceDirectory;

		/// <summary>
		/// Overrides <see cref="Control.TemplateSourceDirectory"/>.
		/// </summary>
		public override string TemplateSourceDirectory {
			get {
				return _templateSourceDirectory;
			}
		}
	}
}
