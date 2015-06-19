#region Using Declarations
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

using PathwaysLib.Exceptions;
using PathwaysLib.SoapObjects;
using PathwaysLib.Utilities;
#endregion

namespace PathwaysLib.ServerObjects
{	

    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/Server/DBRow.cs</filepath>
    ///		<creation>2005/06/22</creation>
    ///		<author>
    ///			<name>Michael F. Starke</name>
    ///			<initials>mfs</initials>
    ///			<email>michael.starke@case.edu</email>
    ///		</author>
    ///		<contributors>
    ///			<contributor>
    ///				<name>Brendan Elliott</name>
    ///				<initials>BE</initials>
    ///				<email>bxe7@cwru.edu</email>
    ///			</contributor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: mitali $</cvs_author>
    ///			<cvs_date>$Date: 2009/01/26 16:35:16 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/DBRow.cs,v 1.2 2009/01/26 16:35:16 mitali Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.2 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Encapsulates dataset and access to ADO.NET.
    /// </summary>
    #endregion
    public class DBRow
    {

        #region Constructor, Destructor, ToString
		private DBRow ( )
		{
		}

        /// <summary>
        /// Constructs a DBRow from an ADO DataSet object that contains exactly one table and one row.
        /// </summary>
        /// <param name="ds"></param>
		public DBRow ( DataSet ds )
		{
            if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count != 1)
                throw new DBWrapperException("DBRow cannot be created from an improperly formed DataSet.  It must be non-null and contain exactly one table and one row.");

			__ADOCommands = new Hashtable();
			__dataSet = ds;
            __row = ds.Tables[0].Rows[0];

            __status = ObjectStatus.NoChanges;
            __newRow = false;
		}

        /// <summary>
        /// Constructs an intially empty DBRow of the specified table to insert
        /// from a SOAP object.
        /// </summary>
        /// <param name="insertTableName"></param>
		public DBRow ( string insertTableName )
		{
			__ADOCommands = new Hashtable();
			__dataSet = DBWrapper.GetSchema(insertTableName);
            __row = __dataSet.Tables[0].NewRow();

            __status = ObjectStatus.Insert;
            __newRow = true;
		}

        /// <summary>
        /// Destructor.
        /// </summary>
        ~DBRow ( )
        {
            //TODO: ensure changes were saved!
        }
        #endregion


        #region Member Variables
		private Hashtable __ADOCommands;
		private DataSet __dataSet;
        private DataRow __row;
        private ObjectStatus __status;
        private bool __newRow;
        #endregion


        #region Properties
		/// <summary>
		/// Get/set the object's ADO commands
		/// </summary>
		public Hashtable ADOCommands
		{
			get
			{
				return __ADOCommands;
			}
			set
			{
				__ADOCommands = value;
			}
		}

		/// <summary>
		/// Get/set the server object status
		/// </summary>
        public ObjectStatus Status
        {
            get
            {
                return __status;
            }
            set
            {
                __status = value;
            }
        }
		
		/// <summary>
		/// Get set column values by name from the DataSet
		/// </summary>
		public object this[string columnName]
		{
			get
			{
                if (!HasColumn(columnName))
                    throw new InvalidColumnException("Table '{0}' does not contain column '{1}'!", __dataSet.Tables[0].TableName, columnName);

				object o = __row[columnName];

                if (o is DBNull)
                    return null;

                return o;
			}
			set
			{
                if (!HasColumn(columnName))
                    throw new InvalidColumnException("Table '{0}' does not contain column '{1}'!", __dataSet.Tables[0].TableName, columnName);

                if (value == null)
                    value = DBNull.Value;

                if (__row[columnName] == value)
                {
                    // no change to row
                    return;
                }
                try
                {
                    __row[columnName] = value;
                }
                catch (System.Exception ex)
                {
                    string s = ex.Message;
                }

                switch(__status)
                {
                    case ObjectStatus.NoChanges:
                        __status = ObjectStatus.Update;
                        break;

                    case ObjectStatus.Insert:
                    case ObjectStatus.Update:
                        // status doesn't change
                        break;

                    case ObjectStatus.ReadOnly:
                        throw new DBWrapperException("This row is read only!");
                    case ObjectStatus.Invalid:
                        throw new DBWrapperException("This row has already been deleted.");
                    case ObjectStatus.Delete:
                        throw new DBWrapperException("This row is being deleted!");
                }
			}
		}

        #endregion


        #region Methods
		/// <summary>
		/// Update the object's status back to the database.
		/// </summary>
		public void UpdateDatabase ()
		{
            if ( __status == ObjectStatus.Insert || __status == ObjectStatus.Update )
            {
                if (__newRow == true)
                {
                    // we now add the row to the dataset to prepare for insert
                    __dataSet.Tables[0].Rows.Add(__row);
                }

                DBWrapper.Update( ref __dataSet, ADOCommands );
                __status = ObjectStatus.NoChanges;
            }
            else if ( __status == ObjectStatus.Delete )
            {
                // delete object
                __row.Delete();
                DBWrapper.Update( ref __dataSet, ADOCommands );
                __status = ObjectStatus.Invalid; // mark internal status as 'Invalid'
            }
		}

        /// <summary>
        /// Returns true if the named column occurs in this DBRow.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool HasColumn( string columnName )
        {
            return __dataSet.Tables[0].Columns.Contains(columnName);
        }

        /// <summary>
        /// Returns the value of an integer column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int GetInt ( string columnName )
        {
            object o = this[columnName];
            if ( o is int )
            {
                return ( int ) o;
            }
            else if ( o is Int16 )
            {
                return ( int ) ( ( Int16 ) o );
            }
            else if ( o is byte)
            {
                return (int) ( (byte) o );
            }
            else if ( o == null )
            {
                //TODO: how should this be handled?
                //return -1;
                throw new DataModelException("Null in an int field!  Use IsNull() first.");
            }

            throw new InvalidColumnException ( "Table '{0}', column '{1}' requested as type 'Int' is actually '{2}'.", __dataSet.Tables[0].TableName, columnName, o.GetType().ToString() ); 
        }

        /// <summary>
        /// Sets the value of an integer column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public void SetInt ( string columnName, int value )
        {
            this[columnName] = value;
        }

        public short GetShort(string columnName)
        {

            object o = this[columnName];
            if (o is short)
            {
                return (short)o;
            }
            else if (o is Int16)
            {
                return (short)((Int16)o);
            }
            else if (o is byte)
            {
                return (short)((byte)o);
            }
            else if (o == null)
            {
                //TODO: how should this be handled?
                //return -1;
                throw new DataModelException("Null in an int field!  Use IsNull() first.");
            }

            throw new InvalidColumnException("Table '{0}', column '{1}' requested as type 'short' is actually '{2}'.", __dataSet.Tables[0].TableName, columnName, o.GetType().ToString()); 

        }

        public void SetShort(string columnName, short value)
        {
            this[columnName] = value;
        }

        /// <summary>
        /// Returns the value of a long integer column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public Long GetLong ( string columnName )
        {
            object o = this[columnName];
            if ( o is long )
                return (long)o;
            else if (o is int)
                return (long)((int)o);
            else if (o is Int16)
                return (long)((Int16)o);
            else if (o is byte)
                return (long)((byte)o);
            else if (o == null)
                return null;
                //throw new DataModelException("Null in a long field!  Use IsNull() first.");

            throw new InvalidColumnException ( "Table '{0}', column '{1}' requested as type 'Long' is actually '{2}'.", __dataSet.Tables[0].TableName, columnName, o.GetType().ToString() ); 
        }

        /// <summary>
        /// Sets the value of a long integer column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public void SetLong ( string columnName, Long value)
        {
            if (value != null)
                this[columnName] = (long)value;
            else
                this[columnName] = null;
        }

        /// <summary>
        /// Returns the value of a decimal column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public decimal GetDecimal ( string columnName)
        {
            object o = this[columnName];
            if ( o is decimal)
                return (decimal)o;
            else if (o == null)
                throw new DataModelException("Null in a decimal field!  Use IsNull() first.");

            throw new InvalidColumnException ( "Table '{0}', column '{1}' requested as type 'Decimal' is actually '{2}'.", __dataSet.Tables[0].TableName, columnName, o.GetType().ToString() ); 
        }

        /// <summary>
        /// Sets the value of a decimal column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public void SetDecimal( string columnName, decimal value )
        {
            this[columnName] = value;
        }

        /// <summary>
        /// Returns the value of a double column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public double GetDouble( string columnName )
        {
            object o = this[columnName];
            if (o is double)
                return (double)o;
            else if (o is float)
                return (double)((float)o);
            else if (o == null)
                throw new DataModelException("Null in double field!  Use IsNull() first.");

            throw new InvalidColumnException ( "Table '{0}', column '{1}' requested as type 'Double' is actually '{2}'.", __dataSet.Tables[0].TableName, columnName, o.GetType().ToString() ); 
        }

        /// <summary>
        /// Sets the value of a double column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public void SetDouble( string columnName, double value )
        {
            this[columnName] = value;
        }

        ///<summary>
        ///Gets the value of a column of a value type that can be NULL in the database        
        ///</summary>
        public Nullable<T> GetNullable<T>(string columnName) where T : struct
        {
            object o = this[columnName];

            if (this[columnName] is DBNull)
                return new Nullable<T>();
            else if (this[columnName] is T)
                return new Nullable<T>((T)this[columnName]);

            throw new InvalidColumnException("Table '{0}', column '{1}' requested as type '{3}' is actually '{2}'.", __dataSet.Tables[0].TableName, columnName, o.GetType().ToString(), typeof(T).ToString()); 
        }

        public void SetNullable<T>(string columnName, Nullable<T> value) where T : struct
        {
            if (!value.HasValue)
                this[columnName] = DBNull.Value; //check if this is right
            else
                this[columnName] = value;
        }

        /// <summary>
        /// Gets the value of a GUID column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public Guid GetGuid ( string columnName )
        {
            object o = this[columnName];
            if ( o is Guid )
            {
                return ( Guid ) o;
            }
            else if ( o == null )
            {
                // null Guid???
                return Guid.Empty;
            }

            throw new InvalidColumnException ( "Table '{0}', column '{1}' requested as type 'Guid' is actually '{2}'.", __dataSet.Tables[0].TableName, columnName, o.GetType().ToString() ); 
        }

        /// <summary>
        /// Sets the value of a GUID column
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public void SetGuid ( string columnName, Guid value)
        {
            if (value == Guid.Empty)
            {
                this[columnName] = null;
                return;
            }

            this[columnName] = value;
        }


        public DateTime GetDateTime(string columnName)
        {
            object o = this[columnName];
            if (o is DateTime)
            {
                return (DateTime)o;
            }

            else if (o == null)
            {
                return DateTime.MinValue ;
            }
            throw new InvalidColumnException("Table '{0}', column '{1}' requested as type 'DateTime' is actually '{2}' .", __dataSet.Tables[0].TableName, columnName, o.GetType().ToString());
        }

        public void SetDateTime(string columnName, DateTime value)
        {
            if (value == null)
            {
                this[columnName] = null;
                return;
            }
            this[columnName] = value;
        }
        /// <summary>
        /// Gets the value of a string column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string GetString ( string columnName )
        {
            object o = this[columnName];
            if ( o is string )
            {
                return (( string ) o).Trim();
            }
            else if ( o == null )
            {
                return null;
            }

            throw new InvalidColumnException ( "Table '{0}', column '{1}' requested as type 'String' is actually '{2}'.", __dataSet.Tables[0].TableName, columnName, o.GetType().ToString() ); 
        }

        /// <summary>
        /// Sets the value of a string column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public void SetString ( string columnName, string value )
        {
            this[columnName] = value;
        }

        /// <summary>
        /// Gets the value of a bool column that may also be null.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public Tribool GetTribool ( string columnName )
        {
            object o = this[columnName];
			if ( o is bool )
			{
				return ( ( bool ) o ) == true ? Tribool.True : Tribool.False;
			}
			else if( o is Byte )
			{
				Byte b = ( Byte ) o;
				return b == 0 ? Tribool.False : Tribool.True;
			}
			else if ( o == null )
			{
				return Tribool.Null;
			}

            throw new InvalidColumnException ( "Table '{0}', column '{1}' requested as type 'Tribool' is actually '{2}'.", __dataSet.Tables[0].TableName, columnName, o.GetType().ToString() ); 
        }

        /// <summary>
        /// Sets the value of a bool column that may also be null.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public void SetTribool ( string columnName, Tribool value )
        {
            if (value == Tribool.Null)
            {
                this[columnName] = null;
                return;
            }

            this[columnName] = (value == Tribool.True ? true : false);
        }

        /// <summary>
        /// Gets the value of a bool column that may not be null.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool GetBool ( string columnName )
        {
            object o = this[columnName];
            if ( o is bool )
            {
                return ( bool ) o;
            }

            throw new InvalidColumnException ( "Table '{0}', column '{1}' requested as type 'Bool' is actually '{2}'.", __dataSet.Tables[0].TableName, columnName, o.GetType().ToString() ); 
        }

        /// <summary>
        /// Sets the value of a bool column that may not be null.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public void SetBool ( string columnName, bool value )
        {
            this[columnName] = value;
        }	

		/// <summary>
		/// Sets the entry in the given column to null
		/// </summary>
		/// <param name="columnName"></param>
        public void SetNull ( string columnName )
        {
            this[columnName] = null;
        }

		/// <summary>
		/// Returns true if the entry in the given column is null,
		///   returns false if it isn't null.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
        public bool IsNull ( string columnName )
        {
            return this[columnName] == null;
        }

		#endregion

        #region Static Methods
        #endregion

        // for servermodel.sourcelink -ahmet
        internal string GetSourceLink(string columnName)
        {
            object o = this[columnName];
            if (o != null)
            {
                return (string)o;
            }

            else if (o == null || o.ToString() == string.Empty)
            {
                return "To be displayed";
            }
            throw new InvalidColumnException("Table '{0}', column '{1}' requested as type 'source link in string format' is actually '{2}' .", __dataSet.Tables[0].TableName, columnName, o.GetType().ToString());
        }

        // for servermodel.sourcelink -ahmet
        internal void SetSourceLink(string columnName, string value)
        {
            if (value == null)
            {
                this[columnName] = null;
                return;
            }
            this[columnName] = value;
        }
    } // End class

} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: DBRow.cs,v 1.2 2009/01/26 16:35:16 mitali Exp $
	$Log: DBRow.cs,v $
	Revision 1.2  2009/01/26 16:35:16  mitali
	*** empty log message ***
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.6  2008/04/08 20:09:59  akaraca
	left pane changed, biomodels grouped into 2, necessary user controls added, content messages files added, 2 db connection issue needs to be clarified, model content improved by source link, page title setting is changed in mayny places [testing whether we are in models mode], display of modellist changed, etc...
	
	Revision 1.5  2008/03/13 19:42:52  divya
	*** empty log message ***
	
	Revision 1.4  2008/02/21 22:46:44  divya
	*** empty log message ***
	
	Revision 1.3  2008/02/20 22:46:59  divya
	*** empty log message ***
	
	Revision 1.2  2006/10/19 21:03:36  brendan
	New graph drawing code ... performs bulk-loading of server objects to reduce the number of queries and filling an object cache.  Also provides an interface for alternative data sources (i.e. XML biopax doc).  Other misc bug fixes.
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.13  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.12  2006/04/12 20:18:54  brian
	*** empty log message ***
	
	Revision 1.11.8.1  2006/03/23 18:42:11  brian
	ServerOrganism should work correctly now
	
	Revision 1.11  2005/07/28 18:46:51  michael
	fiking Tribool casting problem
	
	Revision 1.10  2005/07/26 18:24:47  michael
	Injecting ali's pathways content manager into the system
	
	Revision 1.9  2005/07/25 16:50:33  brandon
	added some XML comments
	
	Revision 1.8  2005/07/06 21:24:17  brandon
	Fixed Tribool, and got AddProcessToOrganismGroup to work!
	
	Revision 1.7  2005/07/01 20:47:39  brendan
	Work on inheritance & the object wrapper.
	
	Revision 1.6  2005/06/28 23:20:25  brendan
	Fixed Gene, MolecularEntity, Organism, and Process to reflect recent wrapper object design changes.
	
	Revision 1.5  2005/06/28 21:53:06  brendan
	ServerPathway now works properly for INSERT, SELECT, UPDATE and DELETE.  Changes have been to DBWrapper, DBRow and ServerObject to support this properly.
	
	Revision 1.4  2005/06/24 21:57:32  brendan
	Checkin of work on database object design.
	
	Revision 1.3  2005/06/24 20:57:34  brandon
	added LoadDecimal, LoadLong, SetDecimal, and SetLong to DBRow.cs
	
	Revision 1.2  2005/06/22 22:06:31  brendan
	Progress on data model.  FieldLoader functions moved to DBRow.  Working on support for insert.
	
	Revision 1.1  2005/06/22 19:59:10  michael
	The new data model requires an new class...
	That class is DBRow!
	
	Revision 1.9  2005/06/20 19:39:31  michael
	debugging ADO updating of database.
	
	Revision 1.8  2005/06/20 17:53:15  michael
	Bug fixes
	
	Revision 1.7  2005/06/16 21:14:11  michael
	testing data model
	
	Revision 1.6  2005/06/16 19:09:16  michael
	Demo of ServerPathway.
	
	Revision 1.5  2005/06/16 17:14:21  michael
	further work on the new object data model
	
	Revision 1.4  2005/06/16 16:10:50  michael
	finishing up DBWrapper class and beginning work on creating the object model.
	
	Revision 1.3  2005/06/14 20:50:38  michael
	Finishing refactoring of DBWrapper and begin implementation of ServerPathway
	
	Revision 1.2  2005/06/10 20:31:52  brendan
	Added ServerObject and SoapObject base classes and code in progress for ServerPathway and Pathway.
	
	Revision 1.1  2005/06/08 20:44:10  brendan
	Adding skeleton projects for refactoring into the 3.0 version.
		
------------------------------------------------------------------------*/
#endregion