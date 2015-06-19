#region Using Declarations
using System;
using System.Xml.Serialization;
#endregion

namespace PathwaysLib.SoapObjects
{	
	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Data/SoapOrganismGroup.cs</filepath>
	///		<creation>2005/07/01</creation>
	///		<author>
	///			<name>Brandon Evans</name>
	///			<initials>bse</initials>
	///			<email>brandon.evans@case.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name>Brendan Elliott</name>
	///				<initials>BE</initials>
	///				<email>bxe7@cwru.edu</email>
	///			</contributor>
	///			<contributor>
	///				<name>Suleyman Fatih Akgul</name>
	///				<initials>sfa</initials>
	///				<email>fatih@case.edu</email>
	///			</contributor>
	///			<contributor>
	///				<name>Gokhan Yavas</name>
	///				<initials>gy</initials>
	///				<email>gokhan.yavas@case.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapOrganismGroup.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents an organism group.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapOrganismGroup")]
	public class SoapOrganismGroup : SoapObject
	{


		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create a organism group with nothing initialized.
		/// </summary>
		public SoapOrganismGroup()
		{
		}

		/// <summary>
		/// Create a new organism group.
		/// </summary>
		/// <param name="scientific_name"></param>
		/// <param name="parent_id"></param>
		/// <param name="notes"></param>
		public SoapOrganismGroup(string scientific_name, Guid parent_id, string notes)
		{
			this.ID              = Guid.Empty; // created on insert into the DB
		    this.ScientificName  = scientific_name;
			this.CommonName      = null;       // not really necessary
			this.ParentID        = parent_id;
		    this.Notes           = notes;

			this.Status          = ObjectStatus.Insert;
		}


		/// <summary>
		/// Like the above, but also includes the common name
		/// </summary>
		/// <param name="scientific_name"></param>
		/// <param name="common_name"></param>
		/// <param name="parent_id"></param>
		/// <param name="notes"></param>
		public SoapOrganismGroup(string scientific_name, string common_name, Guid parent_id, string notes)
		{
			this.ID              = Guid.Empty;
			this.ScientificName  = scientific_name;
			this.CommonName      = common_name;
			this.ParentID        = parent_id;
			this.Notes           = notes;

			this.Status          = ObjectStatus.Insert;
		}

		/// <summary>
		/// Constructor with status given
		/// </summary>
		/// <param name="status"></param>
		public SoapOrganismGroup(ObjectStatus status)
		{
			this.Status = status;
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapOrganismGroup()
		{
		}

		/// <summary>
		/// Returns the name of the organism group.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string retval = this.Name;

			return retval;
		}

		#endregion


		#region Member Variables

		private Guid   id              = Guid.Empty;
		private string scientific_name = null;
		private string common_name     = null;
		private Guid   parent_id       = Guid.Empty;
		private string notes           = null;

		#endregion


		#region Properties

		/// <summary>
		/// OrganismGroup database ID.
		/// </summary>
		public Guid ID
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
		/// Formats the name depending on upon whether scientific name, common name, or both are available
		/// </summary>
		public string Name
		{
			get
			{
				if(this.ScientificName == null)
					return this.CommonName;
				else if(this.CommonName == null)
					return this.ScientificName;
				else
					return (this.ScientificName + " (" + this.CommonName + ")"  );
			}
		}

		/// <summary>
		/// Scientific Name of organism group
		/// </summary>
		public string ScientificName
		{
			get {return scientific_name;}
			set 
			{
				if (scientific_name != value)
				{
					scientific_name = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// Get/Set the common name for the organism group
		/// </summary>
		public string CommonName
		{
			get{return common_name;}
			set
			{
				if(common_name != value)
				{
					common_name = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// OrganismGroup parent ID.
		/// </summary>
		public Guid ParentID
		{
			get {return parent_id;}
			set 
			{
				if (parent_id != value)
				{
					parent_id = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// Notes for the Organism Group
		/// </summary>
		public string Notes
		{
			get {return notes;}
			set 
			{
				if (notes != value)
				{
					notes = value;
					Status = ObjectStatus.Update;
				}
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
	$Id: SoapOrganismGroup.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapOrganismGroup.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.6  2006/04/12 20:27:19  brian
	*** empty log message ***
	
	Revision 1.5.8.2  2006/03/20 19:26:32  brian
	*** empty log message ***
	
	Revision 1.5.8.1  2006/03/15 03:44:57  brian
	Reorganized ServerOrganismGroup and SoapOrganismGroup to allow for ServerOrganism to be a derived type
	
	Revision 1.5  2005/10/28 10:48:29  fatih
	Pathway, process, organism group tested and corrected
	
	Revision 1.4  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.3  2005/07/13 16:41:45  brandon
	Added a bunch of XML comments where they were missing.
	Changed ServerCatalyze so that it loads a single object from a given ec number instead of an array.
	
	Revision 1.2  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.1  2005/07/01 20:47:39  brendan
	Work on inheritance & the object wrapper.
	
	Revision 1.2  2005/07/01 16:45:21  brandon
	not sure if it worked when I tried to commit OrganismGroup classes, so I'm trying again
	
	Revision 1.1  2005/07/01 16:40:12  brandon
	Added OrganismGroup objects
	

------------------------------------------------------------------------*/
#endregion