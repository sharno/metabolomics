#region Using Declarations
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using PathwaysLib.SoapObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
#endregion


namespace PathwaysLib.Utilities
{
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Utilities/OrganismMeta.cs</filepath>
	///		<creation>2006/02/22</creation>
	///		<author>
	///			<name>Brian Lauber</name>
	///			<initials>bml</initials>
	///			<email>bml8@case.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name></name>
	///				<initials></initials>
	///				<email></email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:58 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/Utilities/OrganismMeta.cs,v 1.1 2008/05/16 21:15:58 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// These functions unify the management of ServerOrganism and ServerOrganismGroup
	/// </summary>
	#endregion
	public class OrganismMeta
	{

		#region Constructor, Destructor, ToString
		private OrganismMeta() {}
		#endregion


		#region Static Methods

		/// <summary>
		/// Return all organisms and organism groups as IOrganismEntity Objects from the system.
		/// </summary>
		/// <returns>
		/// Array of SoapOrganism objects ready to be sent via SOAP.
		/// </returns>
		public static IOrganismEntity[] AllOrganismEntities( )
		{

			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT og.[id], og.[name] FROM organism_groups og UNION  SELECT o.[id], o.common_name AS [name] FROM organisms o ORDER BY [name];" );
			
			DataSet ds;
			DBWrapper.Instance.ExecuteQuery(out ds, ref command);

			ArrayList results = new ArrayList();
			foreach(DataRow r in ds.Tables[0].Rows)
			{
				Guid id = (Guid)r["id"];

				if (ServerOrganism.Exists(id))
					results.Add(ServerOrganism.Load(id));
				else if (ServerOrganismGroup.Exists(id))
					results.Add(ServerOrganismGroup.Load(id));
				else
				{
					//not a valid organism or organism group!

					//TODO: ignoring . . . should this throw an exception?
				}
			}

			return (IOrganismEntity[])results.ToArray(typeof(IOrganismEntity));

		}




		#endregion
	}
}

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: OrganismMeta.cs,v 1.1 2008/05/16 21:15:58 mustafa Exp $
	$Log: OrganismMeta.cs,v $
	Revision 1.1  2008/05/16 21:15:58  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.3  2006/03/03 22:33:21  brian
	This should clear all the LegacyBlaster code out of the head.  Let me know if something's still broken.
	
	Revision 1.2  2006/02/27 21:47:42  brian
	*** empty log message ***
	
	Revision 1.1.2.3  2006/02/24 04:47:18  brian
	The new functions appear to be working.  Right now, there is only a demonstration of the new pathway selection routine (we should modify our interface so that it's passing id's, not names of organisms).
	
		
------------------------------------------------------------------------*/
#endregion