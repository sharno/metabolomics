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
	///		<filepath>PathwaysLib/Server/ConnectedPathway.cs</filepath>
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
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ConnectedPathwayAndCommonProcesses.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// ServerPathway ConnectedPathway- (to be listed as a connected pathway)
	/// ServerProcess[] SharedProcesses - (shared by two pathways)
	/// ServerMolecularEntity[] SharedExclusiveMolecules - (molecules shared
	/// by two pathways but are not included in any process in SharedProcesses)
	/// </summary>
	#endregion	
	public class ConnectedPathwayAndCommonProcesses
	{

		#region Constructor, Destructor, ToString

		/// <summary>
		/// Constructor
		/// </summary>
		private ConnectedPathwayAndCommonProcesses()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data"></param>
		public ConnectedPathwayAndCommonProcesses ( DBRow data )
		{
			// (mfs)
			// setup object
			__ConnectedPathRow = data;

			// (mfs)
			// required call to setup SqlCommands
			//SetSqlCommandParameters( ); // (BE) moved call to ServerObject.UpdateDatabase()
		}

		#endregion


		#region Member Variables
		private DBRow __ConnectedPathRow;
		#endregion


		#region Properties
		/// <summary>
		/// Get the connected pathway
		/// </summary>
		public ServerPathway ConnectedPathway
		{
			get
			{
				return ServerPathway.Load( __ConnectedPathRow.GetGuid("connectedPathwayId") );
			}
		}

		/// <summary>
		/// Get the processes shared by the original pathway and the connected pathway
		/// </summary>
		public ServerProcess[] SharedProcesses
		{
			get
			{
				return ServerPathway.GetProcessesInCommonForPathways( __ConnectedPathRow.GetGuid("originalPathwayId"), __ConnectedPathRow.GetGuid("connectedPathwayId") );
			}
		}

		/// <summary>
		/// Get the molecular entities that are shared by the pathways, but are not in
		/// the shared processes
		/// </summary>
		public ServerMolecularEntity[] SharedExclusiveMolecules
		{
			get
			{
				return ServerMolecularEntity.GetExclusiveEntitiesForPathways( __ConnectedPathRow.GetGuid("originalPathwayId"), __ConnectedPathRow.GetGuid("connectedPathwayId") );
			}
		}


		#endregion


	} // End class

} // End namespace

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ConnectedPathwayAndCommonProcesses.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: ConnectedPathwayAndCommonProcesses.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.2  2005/07/20 22:55:56  brandon
	I think I fixed the GetChromosome problem in ServerGene.cs, but maybe not
	
	Revision 1.1  2005/07/20 18:02:19  brandon
	added function to ServerPathway: GetConnectedPathways ( ), which returns an array of ConnectedPathwayAndCommonProcesses objects.  This new object has three properties:
	ServerPathway ConnectedPathway- (to be listed as a connected pathway)
	ServerProcess[] SharedProcesses - (shared by two pathways)
	ServerMolecularEntity[] SharedExclusiveMolecules - (molecules shared
	by two pathways but are not included in any process in SharedProcesses)
	
	Revision 1.1  2005/07/20 04:05:34  brandon
	Added the class EntityRoleProcessAndPathway which is used by ServerMolecularEntity.
	Fixed a bug in GetAllGenes() in ServerGeneProduct.cs
	Created ConnectedPathways.cs to help with pathway links, but it doesn't work yet.
	
	
	
------------------------------------------------------------------------*/
#endregion