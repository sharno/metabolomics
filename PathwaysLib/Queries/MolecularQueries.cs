#region Using Declarations
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;

using PathwaysLib.ServerObjects;
using PathwaysLib.ServerObjects.Utilities;
using PathwaysLib.Exceptions;
#endregion

namespace PathwaysLib.Queries
{
	/// <summary>
	/// This class was designed as a quick fix for some of the PathwaysLib2 functions.
	/// As far as I can tell, this should just be temporary.
	/// </summary>
	public class MolecularQueries
	{
		private MolecularQueries() {}


		/// <summary>
		/// Grab a list of all common molecule id's
		/// </summary>
		/// <returns></returns>
		static public Guid[] GetCommonMoleculeIds()
		{
			// TODO: Replace this function with one that returns a polymorphic list of ServerMolecularEntity's

			SqlCommand command = DBWrapper.BuildCommand("SELECT * FROM common_molecules;");
			DataSet rows;
			DBWrapper.Instance.ExecuteQuery(out rows, ref command);
			ArrayList results = new ArrayList();
			foreach(DataRow r in rows.Tables[0].Rows)
			{
				results.Add((Guid)r["id"]);
			}
			return (Guid[])results.ToArray(typeof(Guid));
		}
	}
}
