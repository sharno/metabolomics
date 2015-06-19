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
    ///		<filepath>PathwaysLib/SoapObjects/SoapProcess.cs</filepath>
    ///		<creation>2005/06/17</creation>
    ///		<author>
    ///			<name>Brandon S. Evans</name>
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
	///				<name>Michael F. Starke</name>
	///				<initials>mfs</initials>
	///				<email>michael.starke@case.edu</email>
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
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapProcess.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Represents a biological process.
    /// </summary>
    #endregion
    [Serializable(), SoapType("SoapProcess")]
    public class SoapProcess : SoapObject
    {


        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a process with nothing initialized.
        /// </summary>
        public SoapProcess()
        {
        }

        /// <summary>
        /// Create a new process.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="reversible"></param>
        /// <param name="location"></param>
        /// <param name="notes"></param>
        /// <param name="generic_process_id"></param>
        public SoapProcess( string name, Tribool reversible, string location, string notes, Guid generic_process_id)
        {
            this.id = Guid.Empty; // created on insert into the DB
            this.name = name;
			this.reversible = reversible;
			this.location = location;
			this.notes = notes;
			this.generic_process_id = generic_process_id;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapProcess()
        {
        }

        /// <summary>
        /// Returns the name of the process.
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
		private Tribool reversible = Tribool.Null;
		private string location = null;
	
		private string notes = null;
		Guid generic_process_id = Guid.Empty;

        #endregion


        #region Properties

        /// <summary>
        /// Process database ID.
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
        /// Name of process
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
        /// Whether process is reversible or not
        /// </summary>
        public Tribool Reversible
        {
            get {return reversible;}
			set            
			{
				if (reversible != value)
				{
					reversible = value;
					Status = ObjectStatus.Update;
				}
			}
        }

		/// <summary>
		/// Location of process
		/// </summary>
		public string Location
		{
			get {return location;}
			set            
			{
				if (location != value)
				{
					location = value;
					Status = ObjectStatus.Update;
				}
			}
		}

	

		/// <summary>
		/// Notes on process
		/// </summary>
		public string ProcessNotes
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

		/// <summary>
		/// Type of process
		/// </summary>
		public Guid GenericProcessID
		{
			get {return generic_process_id;}
			set           
			{
				if (generic_process_id != value)
				{
					generic_process_id = value;
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
	$Id: SoapProcess.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapProcess.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.2  2006/10/19 21:03:36  brendan
	New graph drawing code ... performs bulk-loading of server objects to reduce the number of queries and filling an object cache.  Also provides an interface for alternative data sources (i.e. XML biopax doc).  Other misc bug fixes.
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.11  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.10  2006/05/10 02:40:17  ali
	Server Objects have been modified to make them compliant with the recent schema changes concerning the addition of several entity_type tables into the database.
	
	Revision 1.9  2005/10/28 10:48:29  fatih
	Pathway, process, organism group tested and corrected
	
	Revision 1.8  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.7  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.6  2005/06/29 22:06:10  brendan
	Working on adding support for inheritance between MolecularEntity and Gene.
	
	Revision 1.5  2005/06/28 23:20:25  brendan
	Fixed Gene, MolecularEntity, Organism, and Process to reflect recent wrapper object design changes.
	
	Revision 1.4  2005/06/24 21:57:32  brendan
	Checkin of work on database object design.
	
	Revision 1.3  2005/06/22 22:06:31  brendan
	Progress on data model.  FieldLoader functions moved to DBRow.  Working on support for insert.
	
	Revision 1.2  2005/06/20 21:56:36  brandon
	it builds
	

		
------------------------------------------------------------------------*/
#endregion