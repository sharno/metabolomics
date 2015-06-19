using System;

namespace PathwaysLib.ServerObjects
{
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/ServerObjects/ServerCatalyze.cs</filepath>
    ///		<creation>2005/07/15</creation>
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
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/IOrganismEntity.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Common interface used to represent either an Organism or an Organism Group.
    /// </summary>
    #endregion
    public interface IOrganismEntity
    {
		#region Properties
		/// <summary>
		/// The ID for the organism or organism group
		/// </summary>
        Guid OrganismEntityID
        {
            get;
        }
		
		/// <summary>
		/// The name of the organism or organism group
		/// </summary>
        string OrganismEntityName
        {
            get;
        }
		#endregion

    }
}

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: IOrganismEntity.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: IOrganismEntity.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.5  2006/04/12 20:02:14  brian
	*** empty log message ***
	
	Revision 1.2.4.1  2006/03/10 17:20:50  brendan
	Changes merged from main branch into release branch
	
	Revision 1.2  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.1  2005/07/15 21:29:11  brendan
	Adding IOrganismEntity.
	
------------------------------------------------------------------------*/
#endregion