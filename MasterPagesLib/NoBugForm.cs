using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace MasterPagesLib {

	/// <summary>
	/// A server form that does not cause a problem by being inside a master page or other naming container.
	/// </summary>
	/// <remarks>
	/// When using this version of masterpages with asp.net 1.1, use this form instead of the standard form element for server forms. This is only required because of a bug in asp.net 1.1's version of __doPostBack.
	/// </remarks>
	public class NoBugForm : System.Web.UI.HtmlControls.HtmlForm {

		/// <summary>
		/// Creates a new instance of the NoBugForm class.
		/// </summary>
		public NoBugForm() {
			System.Version runtimeVersion = System.Environment.Version;
			this.doesDoPostBackBugExist = runtimeVersion.Major == 1 && runtimeVersion.Minor == 1 && runtimeVersion.Build == 4322;
		}

		/// <summary>
		/// Overriden to fix the bug.
		/// </summary>
		public override string UniqueID {
			get {
				if ( this.doesDoPostBackBugExist && this.isNamingContainerBreakoutRequired && this.NamingContainer != this.Page ) {
					String fullUniqueID = base.UniqueID;
					return fullUniqueID.Substring(fullUniqueID.LastIndexOf(":")+1);
				} else {
					return base.UniqueID;
				}
			}
		}

		/// <summary>
		/// Overridden to fix the bug.
		/// </summary>
		/// <param name="writer"></param>
		protected override void RenderAttributes(HtmlTextWriter writer) {
			this.isNamingContainerBreakoutRequired = true;
			base.RenderAttributes (writer);
			this.isNamingContainerBreakoutRequired = false;
		}

		/// <summary>
		/// Overridden to fix the bug.
		/// </summary>
		/// <param name="writer"></param>
		protected override void RenderChildren(HtmlTextWriter writer) {
			this.isNamingContainerBreakoutRequired = true;
			base.RenderChildren (writer);
			this.isNamingContainerBreakoutRequired = false;
		}


		

		private Boolean doesDoPostBackBugExist = false;
		private Boolean isNamingContainerBreakoutRequired = false;
	}
}
