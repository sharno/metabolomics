using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MasterPagesLib {

	/// <summary>
	/// The control marks a place holder for content in a master page.
	/// </summary>
	public class Region : PlaceHolder, INamingContainer {
	
		/// <summary>
		/// Overrides <see cref="Control.ID"/> to register regions as they are created.
		/// </summary>
		public override string ID {
			get {
				return base.ID;
			}
			set {
				base.ID = value;
				RegisterRegion();
			}
		}

		private static readonly String contextKey = "MetaBuilders.WebControls.MasterPages.Region ";

		private void RegisterRegion() {
			if ( HttpContext.Current != null ) {
				String myKey = contextKey + this.ID;
				if ( HttpContext.Current.Items.Contains(myKey) ) {
					throw new InvalidOperationException("Region IDs must be unique. '" + this.ID + "' is already in use.");
				} else {
					HttpContext.Current.Items[myKey] = this;
				}
			}
		}

		internal static Region FindRegion( String regionID ) {
			if ( HttpContext.Current == null ) {
				return null;
			}
			return HttpContext.Current.Items[contextKey + regionID] as Region;
		}
	
	}
}
