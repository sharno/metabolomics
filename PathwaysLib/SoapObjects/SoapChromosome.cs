#region Using Declarations
using System;
using System.Xml.Serialization;
#endregion

namespace PathwaysLib.SoapObjects
{	
	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Data/SoapChromosome.cs</filepath>
	///		<creation>2005/06/30</creation>
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
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapChromosome.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents a chromosome.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapChromosome")]
	public class SoapChromosome : SoapObject
	{


		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create a chromosome with nothing initialized.
		/// </summary>
		public SoapChromosome()
		{
		}

		/// <summary>
		/// Create a new chromosome.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="length"></param>
		/// <param name="notes"></param>
		public SoapChromosome(string name, Guid organism_group_id, long length, int centromere_location, string notes)
		{
			this.id = Guid.Empty; // created on insert into the DB
            this.organism_group_id = organism_group_id;
            this.centromere_location = centromere_location;
			this.name = name;
			this.length = length;
			this.notes = notes;
		}

		/// <summary>
		/// Constructor, sets the object status to the given parameter
		/// </summary>
		/// <param name="status"></param>
		public SoapChromosome(ObjectStatus status)
		{
			this.Status = status;
		}

        public SoapChromosome(string id, string name, string length)
        {
            this.id = new Guid(id);
            this.name = name;
            this.length = long.Parse(length);
        }

        public SoapChromosome(string id, string name, string length, object centromere_location)
        {
            this.id = new Guid(id);
            this.name = name;
            this.length = long.Parse(length);
            if (centromere_location != null)
                this.centromere_location = int.Parse(centromere_location.ToString());
            else
                this.centromere_location = 1;
        }

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapChromosome()
		{
		}

		/// <summary>
		/// Returns the name of the chromosome.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string retval;
			//retval = base.ToString();
			retval = name;

			return retval;
		}

		#endregion


		#region Member Variables

		Guid id = Guid.Empty;
		private string name = null;
        private Guid organism_group_id;
		private long length = 1;
		private string notes = null;
        private int centromere_location = 1;

		#endregion


		#region Properties

		/// <summary>
		/// Chromosome database ID.
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
		/// Name of chromosome
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
		/// Length of chromosome
		/// </summary>
		public long Length
		{
			get {return length;}
			set 
			{
				if (length != value)
				{
					length = value;
					Status = ObjectStatus.Update;
				}
			}
		}

        /// <summary>
        /// Location of the centromere
        /// </summary>
        public int CentromereLocation
        {
            get
            {
                return centromere_location;
            }
            set
            {
                centromere_location = value;
            }
        }

        /// <summary>
        /// Organism Group Id
        /// </summary>
        public Guid OrganismGroupId
        {
            get
            {
                return organism_group_id;
            }
            set
            {
                organism_group_id = value;
            }
        }

		/// <summary>
		/// Notes on the chromosome
		/// </summary>
		public string ChromosomeNotes
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
	$Id: SoapChromosome.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapChromosome.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.4  2006/10/19 01:24:43  ali
	*** empty log message ***
	
	Revision 1.3  2006/09/06 14:41:40  pathwaysdeploy
	A new web method for gene viewer has been added.
	
	Revision 1.2  2006/08/17 15:04:43  ali
	A new web method "GetGeneMappingForPathway" is added for the gene viewer.
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.5  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.4  2006/04/21 17:37:29  michael
	*** empty log message ***
	
	Revision 1.3  2005/07/13 16:41:45  brandon
	Added a bunch of XML comments where they were missing.
	Changed ServerCatalyze so that it loads a single object from a given ec number instead of an array.
	
	Revision 1.2  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.1  2005/06/30 19:46:32  brandon
	added a bunch of new classes for all the boxed on the ER diagram, even the ones that only have an id attribute
	
		
------------------------------------------------------------------------*/
#endregion