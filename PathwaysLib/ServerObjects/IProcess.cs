#region Using Declarations
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using PathwaysLib.SoapObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
using PathwaysLib.GraphObjects;
#endregion

namespace PathwaysLib.ServerObjects
{
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/ServerObjects/ServerGenericProcess.cs</filepath>
    ///		<creation>2005/10/20</creation>
    ///		<author>
    ///				<name>Brendan Elliott</name>
    ///				<initials>BE</initials>
    ///				<email>bxe7@cwru.edu</email>
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
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/IProcess.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Common interface shared by both ServerProcess and ServerGenericProcess.
    /// </summary>
    #endregion		
    public interface IProcess : IGraphProcess
	{
        #region Methods

        /// <summary>
        /// Returns the EC Number(s) for these processes.
        /// </summary>
        /// <returns></returns>
        ServerECNumber[] GetECNumbers();

        /// <summary>
        /// Get all entries in the process_entities (catalyze) relation for this generic process
        /// </summary>
        /// <returns>
        /// an array of ServerProcessEntity objects
        /// </returns>
        ServerProcessEntity[] GetAllProcessEntities();

        /// <summary>
        /// Get all gene products
        /// </summary>
        /// <returns>
        /// Returns all of the gene products involved in the generic process
        /// </returns>
        ServerGeneProduct[] GetAllGeneProducts();


        /// <summary>
        /// Returns all organisms (including organism groups) that this process takes place in
        /// </summary>
        /// <returns></returns>
        ServerOrganismGroup[] GetAllOrganisms();

        /// <summary>
        /// Returns all organism groups that this process takes place in
        /// </summary>
        /// <returns></returns>
        ServerOrganismGroup[] GetAllOrganismGroups();
        #endregion
	}
}

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: IProcess.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: IProcess.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.2  2006/10/19 21:03:36  brendan
	New graph drawing code ... performs bulk-loading of server objects to reduce the number of queries and filling an object cache.  Also provides an interface for alternative data sources (i.e. XML biopax doc).  Other misc bug fixes.
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.3  2006/05/12 21:25:46  ali
	For processes no organisms were listed. This was due to the fact that all the functions returning organism lists contained queries that join organism and organism group tables which can technically work only for mouse, and human or real organisms. Now all such methods operate on organism group table as it contains all organisms and organism groups.
	
	Revision 1.2  2006/04/12 20:19:47  brian
	*** empty log message ***
	
	Revision 1.1.8.3  2006/03/22 21:25:21  brian
	Removed and renamed a few more functions for consistency
	
	Revision 1.1.8.2  2006/03/22 20:24:16  brian
	I forgot to list my changes over the last few commits, so here's a quick overview:
	1. IOrganismEntity interface removed
	2. All ServerObject queries that return IOrganismEntity now return either ServerOrganism or ServerOrganismGroup
	3. All OrganismGroup-Process relations have been commented out (pending removal)
	4. Several new functions have been added to ServerOrganism and ServerOrganismGroup too simplify the initialization of objects
	
	Revision 1.1.8.1  2006/03/22 19:48:06  brian
	*** empty log message ***
	
	Revision 1.1  2005/11/07 17:28:26  brendan
	New generic graph building code, exposed via a new web service call.
	
------------------------------------------------------------------------*/
#endregion