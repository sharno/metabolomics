#region Using Declarations
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

using PathwaysLib.ServerObjects;
using PathwaysLib.SoapObjects;
using PathwaysLib.Exceptions;
#endregion

namespace PathwaysLib.ServerObjects
{
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/Server/ServerObject.cs</filepath>
    ///		<creation>2005/06/09</creation>
    ///		<author>
    ///			<name>Brendan Elliott</name>
    ///			<initials>BE</initials>
    ///			<email>bxe7@cwru.edu</email>
    ///		</author>
    ///		<contributors>
    ///			<contributor>
    ///				<name>Michael F. Starke</name>
    ///				<initials>mfs</initials>
    ///				<email>michael.starke@case.edu</email>
    ///			</contributor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: ali $</cvs_author>
    ///			<cvs_date>$Date: 2009/03/25 17:36:58 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerObject.cs,v 1.2 2009/03/25 17:36:58 ali Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.2 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Base class for database persisted objects.
    /// </summary>
    #endregion	
    public abstract class ServerObject
	{
        #region Constructor & Destructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServerObject()
        {
			// (mfs)
			// initialize object
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ServerObject ( )
        {
			// Updates are not committed when object is destroyed.  This
			// allows us to implement an "undo changes" feature
        }
        #endregion


        #region Member Variables
		/// <summary>
		/// The Database row for this object.
		/// </summary>
		protected DBRow __DBRow;
        #endregion


        #region Properties
        #endregion


        #region Methods

        /// <summary>
        /// Derived classes must implement this method which is used to
        /// prepare the SOAP object for transmission.  
        /// </summary>
        /// <remarks>
        /// This function should handle the mapping for each individual
        /// class's DataRow to the corresponding SOAP object.  
        /// </remarks>
        /// <param name="derivedObject">If partially initialized derived object is passed, it's appropriate base class members will be filled.  If this parameter is null, a new object should be created.</param>
        public abstract SoapObject PrepareForSoap ( SoapObject derivedObject );

        /// <summary>
        /// Convienence verison of SoapObject PrepareForSoap ( SoapObject derivedObject ),
        /// that passes in null to ask the method to create a new object.
        /// </summary>
        /// <returns></returns>
        public SoapObject PrepareForSoap ( )
        {
            return PrepareForSoap(null);
        }

		/// <summary>
		/// Derived classes must implement this method which is used to
		/// retrieve from SOAP the object's properties.
		/// </summary>
		/// <remarks>
		/// This function should handle the mapping for a SOAP object
		/// to the corresponding DataRow indices.
		/// </remarks>
		/// <param name="o">
		/// The object returned from the SOAP call.
		/// </param>
		protected abstract void UpdateFromSoap( SoapObject o );

		/// <summary>
		/// Derived classes must implement this method which is used to
		/// set the SqlCommand paramteres used by ADO.NET.
		/// </summary>
		/// <remarks>
		/// This function should create and configure SqlCommands with
		/// the correct parameters and place them in a hash table.
		/// 
		/// Note that certain ID columns must be filled BEFORE the call
		/// to SetSqlCommandParameters().
		/// </remarks>
		protected abstract void SetSqlCommandParameters( );

		/// <summary>
		/// Update the database to reflect changes to the object.
		/// This will be overridden in deriveded classes to first 
		/// update their parent class's row, and then their own.
		/// </summary>
		public virtual void UpdateDatabase ( )
		{
            // (BE) ensure the parameters are filled incorrectly before an update occurs!
            SetSqlCommandParameters();

			__DBRow.UpdateDatabase();

            
//            if (oldStatus == ObjectStatus.Insert)
//            {
//                // insert just happened, fill in the SqlCommandParams to allow update, etc.
//                SetSqlCommandParameters(false);
//            }
		}

        /// <summary>
        /// Deletes the row(s) corresponding to an object from the database.
        /// </summary>
        public virtual void Delete ( )
        {
			if (__DBRow.Status != ObjectStatus.Invalid)
			{
                // (BE) ensure the parameters are filled incorrectly before an update occurs!
                SetSqlCommandParameters();

				__DBRow.Status = ObjectStatus.Delete;
				__DBRow.UpdateDatabase();
			}
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Returns an ArrayList of SoapObjects created from an array of ServerObjects.
        /// Useful to return lists to the rich client.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static ArrayList ToSoapArray(ServerObject[] array)
        {
            ArrayList results = new ArrayList(array.Length);
            for(int i = 0; i < array.Length; i++)
            {
                results.Add(array[i].PrepareForSoap());
            }
            return results;
        }
        /// <summary>
        /// Delelete all entries in this table
        /// </summary>
        public static void DeleteAll(string TableName)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "DELETE FROM " + TableName + ";");
            //              @" WHERE id NOT IN
            //                    (SELECT id FROM UnitDefinition)
            //                 AND id NOT IN
            ////                    (SELECT id FROM Unit);");

            DBWrapper.Instance.ExecuteNonQuery(ref command);
        }

        #endregion	

    } // End class
} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerObject.cs,v 1.2 2009/03/25 17:36:58 ali Exp $
	$Log: ServerObject.cs,v $
	Revision 1.2  2009/03/25 17:36:58  ali
	*** empty log message ***
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.2  2008/02/29 21:12:22  divya
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.24  2006/04/12 21:01:13  brian
	*** empty log message ***
	
	Revision 1.23  2006/04/12 20:02:14  brian
	*** empty log message ***
	
	Revision 1.20.4.1  2006/03/10 17:20:50  brendan
	Changes merged from main branch into release branch
	
	Revision 1.20  2005/11/28 20:57:11  brendan
	Recent fixes to graph layout, added IGraphSource interface for passing data from querying web components to the graph drawing component.
	
	Revision 1.19  2005/11/02 20:35:16  fatih
	Test functions are working for pathwayslib objects
	
	Revision 1.18  2005/10/31 00:39:36  fatih
	*** empty log message ***
	
	Revision 1.17  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.16  2005/07/26 21:13:05  brendan
	Fixed bug in ServerObject that was breaking AllPathways in the web service.
	
	Revision 1.15  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.14  2005/07/11 18:48:56  brendan
	Moved SetSqlCommandParameters() call from constructors to ServerObject.UpdateDatabase().  Added object creation constructor to ServerPathway/SoapPathway.
	
	Revision 1.13  2005/07/01 20:47:39  brendan
	Work on inheritance & the object wrapper.
	
	Revision 1.12  2005/06/29 22:06:10  brendan
	Working on adding support for inheritance between MolecularEntity and Gene.
	
	Revision 1.11  2005/06/28 21:53:06  brendan
	ServerPathway now works properly for INSERT, SELECT, UPDATE and DELETE.  Changes have been to DBWrapper, DBRow and ServerObject to support this properly.
	
	Revision 1.10  2005/06/24 21:57:32  brendan
	Checkin of work on database object design.
	
	Revision 1.9  2005/06/22 22:06:31  brendan
	Progress on data model.  FieldLoader functions moved to DBRow.  Working on support for insert.
	
	Revision 1.8  2005/06/22 18:39:11  michael
	Changing data model again to encapsulate the ADO.NET funcationality further.
	Updating the classes that used the old functionality to use the new DBRow class.
	
	Revision 1.7  2005/06/20 19:39:31  michael
	debugging ADO updating of database.
	
	Revision 1.6  2005/06/16 21:14:11  michael
	testing data model
	
	Revision 1.5  2005/06/16 19:09:16  michael
	Demo of ServerPathway.
	
	Revision 1.4  2005/06/16 17:14:21  michael
	further work on the new object data model
	
	Revision 1.3  2005/06/16 16:10:50  michael
	finishing up DBWrapper class and beginning work on creating the object model.
	
	Revision 1.2  2005/06/13 22:53:08  michael
	Data model changes implementation in progress.
	
	Revision 1.1  2005/06/10 20:31:52  brendan
	Added ServerObject and SoapObject base classes and code in progress for ServerPathway and Pathway.
	
------------------------------------------------------------------------*/
#endregion