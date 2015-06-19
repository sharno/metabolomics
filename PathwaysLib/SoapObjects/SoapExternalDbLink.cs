#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/SoapObjects/SoapExternalDbLink.cs</filepath>
	///		<creation>2005/07/07</creation>
	///		<author>
	///			<name>Brandon S. Evans</name>
	///			<initials>bse</initials>
	///			<email>brandon.evans@case.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name>Brendan Elliott</name>
	///				<initials>BE</initials>
	///				<email>bxe7@case.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapExternalDbLink.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents one of possibly many typed names for a molecular entity.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapExternalDbLink")]	
	public class SoapExternalDbLink : SoapObject
	{
		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create a catalyze relation with nothing initialized.
		/// </summary>
		public SoapExternalDbLink()
		{
		}	

		/// <summary>
		/// Constructor, create a new external database link with all fields initialized.
		/// </summary>
		/// <param name="local_id"></param>
		/// <param name="external_database_id"></param>
		/// <param name="id_in_external_database"></param>
		public SoapExternalDbLink( Guid local_id, int external_database_id, string id_in_external_database )
		{
			this.local_id = local_id;
			this.external_database_id = external_database_id;
			this.id_in_external_database = id_in_external_database;
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapExternalDbLink()
		{
		}

		#endregion

		#region Member Variables

		Guid local_id = Guid.Empty;
		int external_database_id = 0;
		string id_in_external_database = null;

		#endregion

		#region Properties

		/// <summary>
		/// ID of the object (process, molecular entity, etc...)
		///  in our database
		/// </summary>
		public Guid LocalID
		{
			get {return local_id;}
			set 
			{
				if (local_id != value)
				{
					local_id = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// Id of the external database
		/// </summary>
		public int ExternalDatabaseID
		{
			get {return external_database_id;}
			set 
			{
				if (external_database_id != value)
				{
					external_database_id = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// The id of the object (process, molecular entity, etc...)
		///  in an external database
		/// </summary>
		public string IdInExternalDatabase
		{
			get {return id_in_external_database;}
			set 
			{
				if (id_in_external_database != value)
				{
					id_in_external_database = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		#endregion

	} // end class

} // end namespace

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: SoapExternalDbLink.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapExternalDbLink.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.4  2006/06/09 02:24:08  greg
	These changes address a significant number of GO pathway viewer bugs, and also introduce substantial changes to a lot of pages.  I'm refactoring a lot of the HTML to not only move the site closer to XHTML compliance (so we can once again use those buttons that are commented out on the main page!), but also to make the site display properly in non-IE browsers as well, because currently it doesn't look right in non-IE browsers (and hopefully soon everyone will be using Firefox!!).
	
	Revision 1.3  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.2  2005/07/13 16:41:45  brandon
	Added a bunch of XML comments where they were missing.
	Changed ServerCatalyze so that it loads a single object from a given ec number instead of an array.
	
	Revision 1.1  2005/07/12 22:14:04  brandon
	Bug fixes.      Also added external_database_links objects
	
	
------------------------------------------------------------------------*/
#endregion
