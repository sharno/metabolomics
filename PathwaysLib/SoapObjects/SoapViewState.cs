#region Using Declarations
	using System;
	using System.Xml.Serialization;

	using PathwaysLib.ServerObjects;
using PathwaysLib.Utilities;
#endregion

namespace PathwaysLib.SoapObjects
{	
	
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/SoapObjects/SoapViewState.cs</filepath>
    ///		<creation>2005/07/18</creation>
    ///		<author>
    ///			<name>Michael F. Starke</name>
    ///			<initials>mfs</initials>
    ///			<email>michael.starke@cwru.edu</email>
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
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapViewState.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Represents a biological pathway.
    /// </summary>
    #endregion
    [Serializable(), SoapType("SoapViewState")]
    public class SoapViewState : SoapObject
    {

        #region Constructor, Destructor, ToString
		/// <summary>
		/// SoapViewState constructor.
		/// </summary>
        public SoapViewState()
        {
        }

		/// <summary>
		/// SoapViewState Constructor.
		/// </summary>
		/// <param name="openSection">
		/// The section of the browser that is open.
		/// </param>
		/// <param name="openNode1ID">
		/// The id of the top level open node.
		/// </param>
		/// <param name="openNode1Type">
		/// The type of the top level open node.
		/// </param>
		/// <param name="openNode2ID">
		/// The id of the second level open node.
		/// </param>
		/// <param name="openNode2Type">
		/// The type of the second level open node.
		/// </param>
		/// <param name="openNode3ID">
		/// The id of the third level open node.
		/// </param>
		/// <param name="openNode3Type">
		/// The type of the third level open node.
		/// </param>
		/// <param name="displayItemID">
		/// The id of the currently displayed item.
		/// </param>
		/// <param name="displayItemType">
		/// The type of the currently displayed item.
		/// </param>
		/// <param name="viewGraph">
		/// Should the graph be viewed?
		/// </param>
        public SoapViewState( string openSection, Guid openNode1ID, string openNode1Type, Guid openNode2ID, string openNode2Type, Guid openNode3ID, string openNode3Type, Guid displayItemID, string displayItemType, Tribool viewGraph )
        {
			OpenSection = openSection;
			OpenNode1ID = openNode1ID;
			OpenNode1Type = openNode1Type;
			OpenNode2ID = openNode2ID;
			OpenNode2Type = openNode2Type;
			OpenNode3ID = openNode3ID;
			OpenNode3Type = openNode3Type;
			DisplayItemID = displayItemID;
			DisplayItemType = displayItemType;
			ViewGraph = viewGraph;

			this.Status = ObjectStatus.Insert;
        }

		/// <summary>
		/// SoapViewState constructor.
		/// </summary>
		/// <param name="status">
		/// The object's database status.
		/// </param>
        public SoapViewState( ObjectStatus status )
        {
            this.Status = status;
        }

		/// <summary>
		/// SoapViewState destructor.
		/// </summary>
        ~SoapViewState()
        {
        }
        #endregion


        #region Member Variables
		private Guid __ViewID;
        private string __OpenSection;
		private Guid __OpenNode1ID;
		private string __OpenNode1Type;
		private Guid __OpenNode2ID;
		private string __OpenNode2Type;
		private Guid __OpenNode3ID;
		private string __OpenNode3Type;
		private Guid __DisplayItemID;
		private string __DisplayItemType;
		private Tribool __ViewGraph;

		private static readonly char[] __Slash = {'/'};
		private static readonly char[] __Bar = {'|'};
        #endregion


        #region Properties
		/// <summary>
		/// Get/set the viewid.
		/// </summary>
		public Guid ViewID
		{
			get
			{
				return __ViewID;
			}
			set
			{
				__ViewID = value;
			}
		}

		/// <summary>
		/// Get/set the open section.
		/// </summary>
		public string OpenSection
		{
			get
			{
				return __OpenSection;
			}
			set
			{
				__OpenSection = value;
			}
		}

		/// <summary>
		/// Get/set the open node 1 id.
		/// </summary>
		public Guid OpenNode1ID
		{
			get
			{
				return __OpenNode1ID;
			}
			set
			{
				__OpenNode1ID = value;
			}
		}

		/// <summary>
		/// Get/set the open node 1 type
		/// </summary>
		public string OpenNode1Type
		{
			get
			{
				return __OpenNode1Type;
			}
			set
			{
				__OpenNode1Type = value;
			}
		}

		/// <summary>
		/// Get/set the open node 2 id.
		/// </summary>
		public Guid OpenNode2ID
		{
			get
			{
				return __OpenNode2ID;
			}
			set
			{
				__OpenNode2ID = value;
			}
		}

		/// <summary>
		/// Get/set the open node 2 type
		/// </summary>
		public string OpenNode2Type
		{
			get
			{
				return __OpenNode2Type;
			}
			set
			{
				__OpenNode2Type = value;
			}
		}

		/// <summary>
		/// Get/set the open node 3 id.
		/// </summary>
		public Guid OpenNode3ID
		{
			get
			{
				return __OpenNode3ID;
			}
			set
			{
				__OpenNode3ID = value;
			}
		}

		/// <summary>
		/// Get/set the open node 3 type
		/// </summary>
		public string OpenNode3Type
		{
			get
			{
				return __OpenNode3Type;
			}
			set
			{
				__OpenNode3Type = value;
			}
		}

		/// <summary>
		/// Get/set the display item id.
		/// </summary>
		public Guid DisplayItemID
		{
			get
			{
				return __DisplayItemID;
			}
			set
			{
				__DisplayItemID = value;
			}
		}

		/// <summary>
		/// Get/set the display item type
		/// </summary>
		public string DisplayItemType
		{
			get
			{
				return __DisplayItemType;
			}
			set
			{
				__DisplayItemType = value;
			}
		}

		/// <summary>
		/// Get/set the graph's visibility.
		/// </summary>
		public Tribool ViewGraph
		{
			get
			{
				return __ViewGraph;
			}
			set
			{
				__ViewGraph = value;
			}
		}
		#endregion


        #region Methods
        #endregion

    } // End class

} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: SoapViewState.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapViewState.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.2  2006/10/19 21:03:36  brendan
	New graph drawing code ... performs bulk-loading of server objects to reduce the number of queries and filling an object cache.  Also provides an interface for alternative data sources (i.e. XML biopax doc).  Other misc bug fixes.
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.4  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.3  2005/07/25 20:56:20  michael
	Fixing/debugging:
	  ServerViewState
	  SoapViewState
	  LinkHelper
	
	Revision 1.2  2005/07/21 19:14:42  michael
	fixing project files/compilation error
	
	Revision 1.1  2005/07/21 18:05:25  michael
	new viewstate object
	
	Revision 1.6  2005/07/11 19:49:59  brendan
	Added explicit line setting ID to Guid.Empty before the object has been inserted.
	
	Revision 1.5  2005/07/11 18:48:56  brendan
	Moved SetSqlCommandParameters() call from constructors to ServerObject.UpdateDatabase().  Added object creation constructor to ServerPathway/SoapPathway.
	
	Revision 1.4  2005/06/28 21:53:06  brendan
	ServerPathway now works properly for INSERT, SELECT, UPDATE and DELETE.  Changes have been to DBWrapper, DBRow and ServerObject to support this properly.
	
	Revision 1.3  2005/06/24 21:57:32  brendan
	Checkin of work on database object design.
	
	Revision 1.2  2005/06/20 17:53:15  michael
	Bug fixes
	
	Revision 1.1  2005/06/16 20:23:28  michael
	renaming Pathway.cs to SoapPathway.cs
	
	Revision 1.3  2005/06/10 20:31:52  brendan
	Added ServerObject and SoapObject base classes and code in progress for ServerPathway and Pathway.
	
	Revision 1.2  2005/06/08 22:51:40  brendan
	Added default constructor for use with web service.
	
	Revision 1.1  2005/06/08 20:44:10  brendan
	Adding skeleton projects for refactoring into the 3.0 version.
		
------------------------------------------------------------------------*/
#endregion