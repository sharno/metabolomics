#region Using Declarations
using System;
using System.Xml.Serialization;
#endregion

namespace PathwaysLib.SoapObjects
{	
	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Data/ExternalDatabase.cs</filepath>
	///		<creation>2005/06/06</creation>
	///		<author>
	///			<name>Brendan Elliott</name>
	///			<initials>BE</initials>
	///			<email>bxe7@cwru.edu</email>
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
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapExternalDatabase.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents a biological pathway.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapExternalDatabase")]
	public class SoapExternalDatabase : SoapObject
	{


		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create an external database object with nothing initialized.
		/// </summary>
		public SoapExternalDatabase()
		{
		}

		/// <summary>
		/// Create a new external database object.
		/// </summary>
		/// <param name="nname"></param>
		/// <param name="nfullname"></param>
		/// <param name="nurl"></param>
		public SoapExternalDatabase(string nname, string nfullname, string nurl)
		{
			this.id = 0;
			this.name = nname;
			this.fullname = nfullname;
			this.url = nurl;
		}

		/// <summary>
		/// Create a new external database object
		/// </summary>
		/// <param name="status"></param>
		public SoapExternalDatabase(ObjectStatus status)
		{
			this.Status = status;
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapExternalDatabase()
		{
		}

		/// <summary>
		/// Returns the name of the external database.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string retval;
			//retval = base.ToString();
			retval = Name;

			return retval;
		}

		#endregion


		#region Member Variables

		int id = 0;
		private string name = null;
		private string fullname = null;
		private string url = null;

		#endregion


		#region Properties

		/// <summary>
		/// ID of the external database.
		/// </summary>
		public int ID
		{
			get {return id;}
			set 
			{
				if (id != value)
				{
					id = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// Name of external database
		/// </summary>
		public string Name
		{
			get {return name;}
			set 
			{
				if (name != value)
				{
					name = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// Fullname of the external database
		/// </summary>
		public string Fullname
		{
			get {return fullname;}
			set 
			{
				if (fullname != value)
				{
					fullname = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// URL of the external database
		/// </summary>
		public string URL
		{
			get {return url;}
			set 
			{
				if (url != value)
				{
					url = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		#endregion


		#region Methods


		#endregion


	}
}


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: SoapExternalDatabase.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapExternalDatabase.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.3  2006/06/09 02:24:08  greg
	These changes address a significant number of GO pathway viewer bugs, and also introduce substantial changes to a lot of pages.  I'm refactoring a lot of the HTML to not only move the site closer to XHTML compliance (so we can once again use those buttons that are commented out on the main page!), but also to make the site display properly in non-IE browsers as well, because currently it doesn't look right in non-IE browsers (and hopefully soon everyone will be using Firefox!!).
	
	Revision 1.2  2005/07/13 16:41:45  brandon
	Added a bunch of XML comments where they were missing.
	Changed ServerCatalyze so that it loads a single object from a given ec number instead of an array.
	
	Revision 1.1  2005/07/12 17:34:56  brandon
	Finished up (for the most part) the ProcessEntities relation (left out adding and removing stuff).  Created the ExternalDatabase objects.  fixed a bug in SoapRNA.
	
		
------------------------------------------------------------------------*/
#endregion