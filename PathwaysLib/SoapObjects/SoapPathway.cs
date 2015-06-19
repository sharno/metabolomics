#region Using Declarations
using System;
using System.Xml.Serialization;
#endregion

namespace PathwaysLib.SoapObjects
{	
	
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/Data/Pathway.cs</filepath>
    ///		<creation>2005/06/06</creation>
    ///		<author>
    ///			<name>Brendan Elliott</name>
    ///			<initials>BE</initials>
    ///			<email>bxe7@cwru.edu</email>
    ///		</author>
    ///		<contributors>
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
    ///         <contributor>
    ///         <name>Divya</name>
    ///         <initials>DB</initials>
    ///         <email>dvya.babuji@case.edu</email>
    ///         </contributor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: mustafa $</cvs_author>
    ///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapPathway.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Represents a biological pathway.
    /// </summary>
    #endregion
    [Serializable(), SoapType("SoapPathway")]
    public class SoapPathway : SoapObject
    {


        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a pathway with nothing initialized.
        /// </summary>
        public SoapPathway()
        {
        }

        /// <summary>
        /// Create a new pathway.
        /// </summary>
        /// <param name="name">
        /// Name of the pathway
        /// </param>
        /// <param name="type"></param>
        /// <param name="pathwayStatus"></param>
        /// <param name="pathwayNotes"></param>
        public SoapPathway(string name, string type, string pathwayStatus, string pathwayNotes)
        {
            this.id = Guid.Empty; // created on insert into the DB
            this.name = name;
            this.type = type;
			this.pathwayStatus = pathwayStatus;
			this.pathwayNotes = pathwayNotes;
        }

		/// <summary>
		/// Constructor with status given
		/// </summary>
		/// <param name="status"></param>
        public SoapPathway(ObjectStatus status)
        {
            this.Status = status;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapPathway()
        {
        }

        /// <summary>
        /// Returns the name of the pathway.
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
        private string type = null;
		private string pathwayNotes = null;
		private string pathwayStatus = null;

        #endregion


        #region Properties

        /// <summary>
        /// Pathway database ID.
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
        /// Name of pathway
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
        /// Type of pathway
        /// </summary>
        public string Type
        {
            get {return type;}
            set 
            {
                if (type != value)
                {
                    type = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

		/// <summary>
		/// Status of pathway
		/// </summary>
		public string PathwayStatus
		{
			get {return pathwayStatus;}
			set 
			{
				if (pathwayStatus != value)
				{
					pathwayStatus = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// Notes about pathway
		/// </summary>
		public string PathwayNotes
		{
			get {return pathwayNotes;}
			set 
			{
				if (pathwayNotes != value)
				{
					pathwayNotes = value;
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
	$Id: SoapPathway.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapPathway.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.2  2008/05/09 18:33:25  divya
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.9  2005/11/07 17:28:26  brendan
	New generic graph building code, exposed via a new web service call.
	
	Revision 1.8  2005/10/28 10:48:29  fatih
	Pathway, process, organism group tested and corrected
	
	Revision 1.7  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
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