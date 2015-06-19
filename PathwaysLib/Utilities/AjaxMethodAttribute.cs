using System;

namespace PathwaysLib.Utilities
{
	/// <summary>
	/// An AjaxMethodAttribute container class.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class AjaxMethodAttribute : Attribute
	{
		/// <summary>
		/// A generic AjaxMethodAttribute constructor
		/// </summary>
		public AjaxMethodAttribute()
		{
		}
	}
}