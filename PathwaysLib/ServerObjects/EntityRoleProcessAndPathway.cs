#region Using Declarations
using System;
using System.Data;

using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
#endregion

namespace PathwaysLib.ServerObjects
{
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Server/EntityRoleProcessAndPathway.cs</filepath>
	///		<creation>2005/07/05</creation>
	///		<author>
	///			<name>Brandon S. Evans</name>
	///			<initials>bse</initials>
	///			<email>brandon.evans@case.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name>none</name>
	///				<initials>none</initials>
	///				<email>none</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/EntityRoleProcessAndPathway.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Manages access to the process_entities and pathway_processes
	/// tables for Molecular Entities.  Contains members to return all
	/// pathway with the specific process that the entity takes role in
	/// that pathway, including the role of the molecule in	that process.
	/// </summary>
	#endregion	
	public class EntityRoleProcessAndPathway
	{

		#region Constructor, Destructor, ToString

		/// <summary>
		/// Constructor
		/// </summary>
		private EntityRoleProcessAndPathway()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data"></param>
		public EntityRoleProcessAndPathway ( DBRow data )
		{
			// (mfs)
			// setup object
			__RPaPRow = data;

			// (mfs)
			// required call to setup SqlCommands
			//SetSqlCommandParameters( ); // (BE) moved call to ServerObject.UpdateDatabase()
		}

		#endregion


		#region Member Variables
		private DBRow __RPaPRow;
        private ServerPathway pathway;
		#endregion


		#region Properties
		/// <summary>
		/// Get/set the Process name.
		/// </summary>
		public ServerPathway Pathway
		{
			get
			{
                if (pathway == null)
                {
                    if (__RPaPRow.GetGuid("pathwayId").ToString() == Utilities.Util.NullGuid)
                        pathway = new ServerPathway("N/A", "Unknown", "--", "--");
                    else
                        pathway = ServerPathway.Load(__RPaPRow.GetGuid("pathwayId"));
                }
                return pathway;
			}
		}

		/// <summary>
		/// Get/set the Process name.
		/// </summary>
		public ServerProcess Process
		{
			get
			{
				return ServerProcess.Load( __RPaPRow.GetGuid("processId") );
			}
		}

		/// <summary>
		/// Get/set the entity role as a string.
		/// </summary>
		public string Role
		{
			get
			{
				//return ProcessEntityRoleManager.GetRoleName(RoleId);
                return __RPaPRow.GetString("role");
            }
//			set
//			{
//				RoleId = ProcessEntityRoleManager.GetRoleId(value); // (ac) will fail if string is not in db!
//			}
		}

//        /// <summary>
//        /// Get/set entity role ID.
//        /// </summary>
//		public int RoleId
//		{
//			get
//			{
//				return __RPaPRow.GetInt("role_id");
//			}
//			set
//			{
//				__RPaPRow.SetInt("role_id", value);
//			}
//		}

	//Having the pathwayName in the DBRow is necessary so that the
		// entries can be ordered by pathway name in array.
		// This can be uncommented if it is found useful.
		// For now, use 'Pathway.Name' to get the pathway name.
//		/// <summary>
//		/// Get/set the pathway name.
//		/// </summary>
//		public string PathwayName
//		{
//			get
//			{
//				return __RPaPRow.GetString("pathwayName");
//			}
//			set
//			{
//				__RPaPRow.SetString( "pathwayName", value );
//			}
//		}



		#endregion


	} // End class

} // End namespace

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: EntityRoleProcessAndPathway.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: EntityRoleProcessAndPathway.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.2  2007/04/09 17:14:31  ali
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.5  2006/05/11 21:18:33  brendan
	Fixed numerous bugs, basic browsing starting to work again
	
	Revision 1.4  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.3  2006/05/10 02:40:17  ali
	Server Objects have been modified to make them compliant with the recent schema changes concerning the addition of several entity_type tables into the database.
	
	Revision 1.2  2005/07/20 04:05:34  brandon
	Added the class EntityRoleProcessAndPathway which is used by ServerMolecularEntity.
	Fixed a bug in GetAllGenes() in ServerGeneProduct.cs
	Created ConnectedPathways.cs to help with pathway links, but it doesn't work yet.
	
	Revision 1.1  2005/07/20 00:29:51  brandon
	*** empty log message ***
	
	
------------------------------------------------------------------------*/
#endregion