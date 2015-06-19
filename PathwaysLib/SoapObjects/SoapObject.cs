#region Using Declarations
using System;

using System.Xml.Serialization;
#endregion

namespace PathwaysLib.SoapObjects
{	
	/// <summary>
	/// The database status of the object.
	/// </summary>
    public enum ObjectStatus
    {
		/// <summary>
		/// Unchanged.
		/// </summary>
        NoChanges,
		/// <summary>
		/// Inserted item.
		/// </summary>
        Insert,
		/// <summary>
		/// Updated item.
		/// </summary>
        Update,
		/// <summary>
		/// Deleted item.
		/// </summary>
        Delete,

		/// <summary>
		/// Readonly item.
		/// </summary>
        ReadOnly,
		/// <summary>
		/// Invalid item.
		/// </summary>
        Invalid
    }
	
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/Data/SoapObject.cs</filepath>
    ///		<creation>2005/06/08</creation>
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
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapObject.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Base class for the data representation of an object to be sent to the client via SOAP.
    /// </summary>
    /// <remarks>
    /// This contains functions common to all SOAP-transmitted objects, such as 
    /// maintaining their 'dirty' state.
    /// </remarks>
    #endregion
    [Serializable(), SoapType("SoapObject")]
    public class SoapObject
    {


        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SoapObject()
        {
            this.__status = ObjectStatus.Insert;
        }

		/// <summary>
		/// Soap object constructor with status given
		/// </summary>
		/// <param name="status"></param>
        public SoapObject(ObjectStatus status)
        {
            this.__status = status;
        }
       
        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapObject()
        {
        }

        #endregion


        #region Member Variables

        ObjectStatus __status = ObjectStatus.NoChanges;

        #endregion


        #region Properties

		/// <summary>
		/// Status of the object
		/// Possible Status values:
		///	  Delete, Instert, Invalid, NoChanges, ReadOnly, Update
		/// These determine whether changes are written back to the
		/// database when UpdateDatabase is called
		/// </summary>
        public ObjectStatus Status
        {
            get {return __status;}
            set 
            {
                if (__status == ObjectStatus.Insert && value == ObjectStatus.Update)
                    return; // insert already implies update!

                __status = value;
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
	$Id: SoapObject.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapObject.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.6  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.5  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.4  2005/06/28 21:53:06  brendan
	ServerPathway now works properly for INSERT, SELECT, UPDATE and DELETE.  Changes have been to DBWrapper, DBRow and ServerObject to support this properly.
	
	Revision 1.3  2005/06/24 21:57:32  brendan
	Checkin of work on database object design.
	
	Revision 1.2  2005/06/22 22:06:31  brendan
	Progress on data model.  FieldLoader functions moved to DBRow.  Working on support for insert.
	
	Revision 1.1  2005/06/10 20:31:52  brendan
	Added ServerObject and SoapObject base classes and code in progress for ServerPathway and Pathway.
	
	Revision 1.2  2005/06/08 22:51:40  brendan
	Added default constructor for use with web service.
	
	Revision 1.1  2005/06/08 20:44:10  brendan
	Adding skeleton projects for refactoring into the 3.0 version.
		
------------------------------------------------------------------------*/
#endregion