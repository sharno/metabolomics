#region Using Declarations
using System;
using System.Threading;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using PathwaysLib.Exceptions;
using PathwaysLib.Utilities;
using System.Collections.Generic;
#endregion

namespace PathwaysLib.ServerObjects
{	
	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>/Pathways/PathwaysLib/ServerObjects/DBWrapper.cs</filepath>
	///		<creation>2005-06-13 17:29</creation>
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
	///			<cvs_author>$Author: xjqi $</cvs_author>
	///			<cvs_date>$Date: 2009/09/09 15:48:14 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/DBWrapper.cs,v 1.6 2009/09/09 15:48:14 xjqi Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.6 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// This class wraps common database functionality including
	/// connection and querying.
	/// </summary>
	/// <remarks>
	/// Database wrapper module including storage of the connection
	/// string, all pre-formated query strings and a convenient set of
	/// methods to request and update data in the database using
	/// arbitrary-length arguments.  Before making any queries, the
	/// connection must first be opened and the connection should be
	/// closed before the program exits.
	/// 
	/// This class was adapted from code provided by Brendan Elliott from
	/// the ToPPLabel project.
	/// </remarks>
	#endregion
	public class DBWrapper
	{
	
		#region Constructor & Destructor
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks>
		/// This version connects on the connection string provided in the
		/// global AppSettings.
		/// </remarks>
		public DBWrapper ( )
		{
            String strValue = String.Empty;

            strValue = ConfigurationManager.AppSettings.Get("dbConnectString");
			if ( strValue != null)
			{
				ConnectionString = ConfigurationManager.AppSettings.Get("dbConnectString");
			}
			else
			{
				throw new DBWrapperException("Configuration Error: Null database connection string.");
			}
            strValue = String.Empty;
            strValue = ConfigurationManager.AppSettings.Get("UseQueryLogger");
            if (strValue != null)
            {
                __Logging = (ConfigurationManager.AppSettings.Get("UseQueryLogger") == "true");
            }
			
			CommandTimeout = 300;
		}

		/// <summary>
		/// Alternate connection constructor.
		/// </summary>
		/// <remarks>
		/// This version connects on the connection string provided as a
		/// parameter to the function, unless connectionString is null
		/// in which case, the global AppSettings value is used.
		/// </remarks>
		/// <param name="connectionString">
		/// A connection string that is to be used to connect to the
		/// database.
		/// </param>
		public DBWrapper ( string connectionString )
		{
			if ( connectionString != null )
			{
				ConnectionString = connectionString;
			}
			else if ( ConfigurationManager.AppSettings.Get("dbConnectString") != null )
			{
				ConnectionString = ConfigurationManager.AppSettings.Get("dbConnectString");
			}
			else
			{
				throw new DBWrapperException("Configuration Error: Null database connection string.");
			}

			if ( ConfigurationManager.AppSettings.Get("UseQueryLogger") == "true" )
			{
				__Logging = true;
			}

			CommandTimeout = 300;
		}

		/// <summary>
		/// DBWrapper destructor
		/// </summary>
		/// <remarks>
		/// Unused.
		/// </remarks>
		~DBWrapper ( )
		{
			// (mfs)
			// There is currently no need for an object destructor for
			// this type, as there exist no vital resources used by this
			// object requiring explicit (deterministic) release
		}
		#endregion


		#region Member Variables
		private static string __ConnectionString = null;
		private SqlConnection __ActiveConnection;
		private int __CommandTimeout;
		//private static readonly string __DateFormat = "yyyy-MM-dd HH:mm:ss.fff";
		private static Hashtable __InstancesTable;
		private static bool __Logging = false;
		#endregion


		#region Properties
		/// <summary>
		/// Get the connection string.
		/// </summary>
		private string ConnectionString
		{
			get 
			{
				return __ConnectionString;
			}
			set
			{
				__ConnectionString = value;
			}
		}
		
		/// <summary>
		/// Get/set the active connection.
		/// </summary>
		private SqlConnection ActiveConnection
		{
			get
			{
				return __ActiveConnection;
			}
			set
			{
				__ActiveConnection = value;
			}
		}
		
		/// <summary>
		/// Get/set the command timeout.
		/// </summary>
		private int CommandTimeout
		{
			get
			{
				return __CommandTimeout;
			}
			set
			{
				__CommandTimeout = value;
			}
		}

		/// <summary>
		/// Get/set the hashtable instance.
		/// </summary>
		private static Hashtable InstancesTable
		{
			get
			{
				return __InstancesTable;
			}
			set
			{
				__InstancesTable = value;
			}
		}
		#endregion


		#region Methods
		/// <summary>
		/// Opens a connection to the database using the stored
		/// ConnectionString.
		/// </summary>
		public void Connect ( )
		{
			ActiveConnection = new SqlConnection( ConnectionString );
            ActiveConnection.Open();
		}

		/// <summary>
		/// Close the connection to the database.
		/// </summary>
		public void Close ( )
		{
			if ( ActiveConnection == null )
			{
				return;
			}

			ActiveConnection.Close();
			ActiveConnection = null;
		}

		/// <summary>
		/// Execute a SELECT-type query and store the results in a pre-
		/// allocated DataTable.
		/// </summary>
		/// <remarks>
		/// The query is passed as a SqlCommand object.
		/// </remarks>
		/// <param name="results">
		/// Pre-allocated DataSet
		/// </param>
		/// <param name="command">
		/// SQL command to be performed.
		/// </param>
		/// <returns>
		/// Number of rows in the results DataTable.
		/// </returns>
		public int ExecuteQuery( out DataSet results, ref SqlCommand command )
		{
			int r;
			try
			{
				TestConnection();
				SetCommandProperties( ref command );

				SqlDataAdapter sda = new SqlDataAdapter( command );
				results = new DataSet();

				if( __Logging ) QueryLogger.StartTimer();

				r = sda.Fill( results );

				if( __Logging ) QueryLogger.UpdateLog( "ExecuteQuery", command.CommandText );
			}
			catch ( SqlException se )
			{
				if ( se.Message.StartsWith( "Timeout expired." ) )
				{
					throw new QueryTimeoutException("Timeout expired.");
				}
				else
				{
					throw new DBWrapperException( "Error in query: " + se.ToString() + " - " + command.CommandText, se );
				}
			}
			finally
			{
				// This should not actually be closing the connection; rather, it is returning
				// it to the connection pool (handled by the magic that is .NET)
				Close();
			}
			
			return r;
		}

        public SqlDataReader ExecuteQueryReader(ref SqlCommand command)
        {
            SqlDataReader reader = null;
            try
            {
                TestConnection();
                SetCommandProperties(ref command);

                //SqlDataAdapter sda = new SqlDataAdapter(command);
                //results = new DataSet();

                if (__Logging) QueryLogger.StartTimer();

                //r = sda.Fill(results);
                reader = command.ExecuteReader();

                if (__Logging) QueryLogger.UpdateLog("ExecuteQuery", command.CommandText);
            }
            catch (SqlException se)
            {
                if (se.Message.StartsWith("Timeout expired."))
                {
                    throw new QueryTimeoutException("Timeout expired.");
                }
                else
                {
                    throw new DBWrapperException("Error in query: " + se.ToString() + " - " + command.CommandText, se);
                }
            }
            finally
            {
                // This should not actually be closing the connection; rather, it is returning
                // it to the connection pool (handled by the magic that is .NET)
                Close();
            }

            return reader;
        }

        public SqlDataReader ExecuteQueryReader(string sql, params object[] paramNameTypeValueList)
        {
            SqlCommand command = BuildCommand(sql, paramNameTypeValueList);
            return ExecuteQueryReader(ref command);
        }

        public SqlDataReader ExecuteQueryReader(string connectionString, string sql, params object[] paramNameTypeValueList)
        {
            SqlCommand command = BuildCommand(sql, paramNameTypeValueList);
            return ExecuteQueryReader(ref command, connectionString);
        }

        public SqlDataReader ExecuteQueryReader(ref SqlCommand command, string connectionString)
        {
            SqlDataReader reader = null;
            try
            {
                //TestConnection();
                TestConnection(connectionString);
                SetCommandProperties(ref command);

                //SqlDataAdapter sda = new SqlDataAdapter(command);
                //results = new DataSet();

                if (__Logging) QueryLogger.StartTimer();

                //r = sda.Fill(results);
                reader = command.ExecuteReader();

                if (__Logging) QueryLogger.UpdateLog("ExecuteQuery", command.CommandText);
            }
            catch (SqlException se)
            {
                if (se.Message.StartsWith("Timeout expired."))
                {
                    throw new QueryTimeoutException("Timeout expired.");
                }
                else
                {
                    throw new DBWrapperException("Error in query: " + se.ToString() + " - " + command.CommandText, se);
                }
            }
            finally
            {
                // This should not actually be closing the connection; rather, it is returning
                // it to the connection pool (handled by the magic that is .NET)
                //Close();
            }

            return reader;
        }


        /// <summary>
        /// Tests a connection for openness, and if closed, opens it (expects a connection string parameter).
        /// </summary>
        public void TestConnection(string connectionString)
        {
            if (ActiveConnection == null || ActiveConnection.State == ConnectionState.Closed)
            {
                Connect(connectionString);
            }
        }
        public void Connect(string connectionString)
        {
            ActiveConnection = new SqlConnection(connectionString);
            ActiveConnection.Open();
        }

		/// <summary>
		/// Execute a SELECT-type query and store the results in a pre-
		/// allocated DataTable.
		/// </summary>
		/// <remarks>
		/// The query is passed as a SqlCommand object.
		/// </remarks>
		/// <param name="results">
		/// Pre-allocated DataSet
		/// </param>
		/// <param name="sql">
		/// SQL command string to be performed.
		/// </param>
		/// <param name="paramNameTypeValueList">
		/// parameters used in the SQL command string
		/// </param>
		/// <returns>
		/// Number of rows in the results DataTable.
		/// </returns>
        public int ExecuteQuery(out DataSet results, string sql, params object[] paramNameTypeValueList)
        {
            SqlCommand command = BuildCommand(sql, paramNameTypeValueList);
            return ExecuteQuery(out results, ref command);
        }


        

		/// <summary>
		/// Execute a query that does not return a result set (INSERT,
		/// UPDATE, DELETE).
		/// </summary>
		/// <remarks>
		/// The query is passed as a SqlCommand object.
		/// </remarks>
		/// <param name="command"></param>
		/// <returns>
		/// Number of affected rows.
		/// </returns>
		public int ExecuteNonQuery( ref SqlCommand command )
		{
			int r;
			try
			{
				TestConnection();
				SetCommandProperties( ref command );

				if( __Logging ) QueryLogger.StartTimer();

				r = command.ExecuteNonQuery();

				if( __Logging ) QueryLogger.UpdateLog( "ExecuteNonQuery", command.CommandText );				
			}
			catch ( SqlException se )
			{
				if ( se.Message.StartsWith( "Timeout expired." ) )
				{
					throw new QueryTimeoutException("Timeout expired.");
				}
				else
				{
					throw new DBWrapperException( "Error in query: " + se.ToString() + " - " + command.CommandText, se );
				}
			}
			finally
			{
				Close();
			}
			return r;
		}
		
        /// <summary>
        /// Execute a query that does not return a result set (INSERT,
        /// UPDATE, DELETE), using the argument syntax of the BuildCommand() function.  
        /// See BuildCommand() for more details.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramNameTypeValueList"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, params object[] paramNameTypeValueList)
        {
            SqlCommand command = BuildCommand(sql, paramNameTypeValueList);
            return ExecuteNonQuery(ref command);
        }

		/// <summary>
		/// Execute a query that returns a single scalar value (i.e. the
		/// result of an aggregate function).
		/// </summary>
		/// <remarks>
		/// The query is passed as a SqlCommand object.
		/// </remarks>
		/// <param name="command"></param>
		/// <returns>
		/// Object containing the scalar value.
		/// </returns>
		public object ExecuteScalar ( ref SqlCommand command )
		{
			object o;
			try
			{
				TestConnection();
				SetCommandProperties( ref command );
				
				if( __Logging ) QueryLogger.StartTimer();

				o = command.ExecuteScalar();

				if( __Logging ) QueryLogger.UpdateLog( "ExecuteScalar", command.CommandText );
			}
			catch ( SqlException se )
			{
				if ( se.Message.StartsWith( "Timeout expired." ) )
				{
					throw new QueryTimeoutException( "Timeout expired." );
				}
				else
				{
					throw new DBWrapperException( "Error in query: " + se.ToString() + " - " + command.CommandText, se );
				}
			}
			finally
			{
				Close();
			}
			return o;
		}

        /// <summary>
        /// EExecute a query that returns a single scalar value (i.e. the
        /// result of an aggregate function), using the argument syntax of the BuildCommand() function.  
        /// See BuildCommand() for more details.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramNameTypeValueList"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, params object[] paramNameTypeValueList)
        {
            SqlCommand command = BuildCommand(sql, paramNameTypeValueList);
            return ExecuteScalar(ref command);
        }

        /// <summary>
        /// Execute a query that returns a single scalar value (i.e. the
        /// result of an aggregate function).
        /// </summary>
        /// <remarks>If the query requires parameters, the SqlCommand version should be used instead.</remarks>
        /// <param name="parameterlessCommand"></param>
        /// <returns></returns>
        public object ExecuteScalar ( string parameterlessCommand )
        {
            SqlCommand cmd = new SqlCommand(parameterlessCommand);
            return ExecuteScalar(ref cmd);
        }

		/// <summary>
		/// Return a DataSet to load a single object.
		/// </summary>
		/// <param name="results">
		/// DataSet containing the command's query results.
		/// </param>
		/// <param name="command">
		/// The SQL command to issue to the database.
		/// </param>
		public static int LoadSingle( out DataSet results, ref SqlCommand command )
		{
			DBWrapper.Instance.TestConnection();
			int n = DBWrapper.Instance.ExecuteQuery( out results, ref command );
			DBWrapper.Instance.Close();

			if ( n != 1 )
			{
				throw new DBWrapperException( "Error: Query command passed to LoadSingle returned " + n + " rows (expecting n = 1). " + command.CommandText ); 
			}
            return n;
		}

		/// <summary>
		/// Return a DataSet to load a single object.
		/// </summary>
		/// <param name="results">
		/// DataSet containing the command's query results.
		/// </param>
		/// <param name="command">
		/// The SQL command to issue to the database.
		/// </param>
		/// <param name="allowZero">
		/// If false, an exception is thrown if no rows are returned when one is expected. 
		/// </param>
		public static int LoadSingle( out DataSet results, ref SqlCommand command, bool allowZero )
		{
			if ( !allowZero )
			{
				return LoadSingle( out results, ref command );
			}
			return DBWrapper.Instance.ExecuteQuery( out results, ref command );
		}

		/// <summary>
		/// Return an array of DataSets to load multiple objects.
		/// </summary>
		/// <param name="results">
		/// Array of DataSets containing the query results segregated into
		/// their own DataSet.
		/// </param>
		/// <param name="command">
		/// The SQL command to issue to the database.
		/// </param>
		public static int LoadMultiple( out DataSet[] results, ref SqlCommand command )
		{
			DBWrapper.Instance.TestConnection();
			DataSet ds = new DataSet();
			int r = DBWrapper.Instance.ExecuteQuery( out ds, ref command );
			DBWrapper.Instance.Close();

			results = new DataSet[r];
			if ( r == 0 )
				return r;

			if ( r == 1 )
			{
				results[0] = ds;
				return r;
			}
			int index = 0;
			foreach ( DataRow row in ds.Tables[0].Rows )
			{

				//Changed: mrr 2006-02-15
				DataSet temp = new DataSet();

				DataTable table = ds.Tables[0].Clone();

				temp.Tables.Add(table);

				temp.Tables[0].ImportRow( row );
				
				results[index++] = temp;

				//old version
//				DataSet temp = new DataSet();
//				temp = ds.Copy();
//
//				temp.Tables[0].Rows.Clear();
//				temp.Tables[0].ImportRow( row );
//				
//				results[index++] = temp;
			}
			return r;
		}

        //public static List<T> LoadMultiple<T>(SqlCommand command) where T : new()
        //{
        //    DataSet[] ds;
        //    DBWrapper.LoadMultiple(out ds, ref command);

        //    List<T> results = new List<T>();
        //    foreach (DataSet r in ds)
        //        results.Add(new T(new DBRow(r)));

        //    return results;
        //}

        //internal static T LoadSingle<T>(SqlCommand command) where T : new()
        //{
        //    DataSet ds;
        //    DBWrapper.LoadSingle(out ds, ref command);
        //    return new T(new DBRow(ds));
        //}

        /// <summary>
        /// Returns an empty dataset with the columns of the specified table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataSet GetSchema( string tableName )
        {
            DBWrapper.Instance.TestConnection();
            DataSet ds = new DataSet();
            // run select with impossible condition to get schema
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM " + tableName + " WHERE 0=1", DBWrapper.Instance.ActiveConnection);
            da.FillSchema(ds, SchemaType.Source, tableName);
			DBWrapper.Instance.Close();
            
            return ds;
        }

		#region Utility Methods
		/// <summary>
		/// Tests a connection for openness, and if closed, opens it.
		/// </summary>
		public void TestConnection ( )
		{
			if ( ActiveConnection == null || ActiveConnection.State == ConnectionState.Closed) 
			{				
				Connect ( );
			}
		}

		/// <summary>
		/// Set the connection-specific properties of a command object.
		/// </summary>
		/// <param name="command">
		/// A SqlCommand object needing a connection parameter, and a
		/// timeout parameter.
		/// </param>
		private void SetCommandProperties( ref SqlCommand command )
		{
			command.Connection = ActiveConnection;
			command.CommandTimeout = CommandTimeout;
		}
		#endregion
		#endregion


		#region Static Methods
		/// <summary>
		/// Static property to get/set a database instance
		/// </summary>
		public static DBWrapper Instance
		{
			get
			{
				if ( InstancesTable == null )
				{
					InstancesTable = new Hashtable();
				}
				if ( !( InstancesTable.ContainsKey( Thread.CurrentThread ) ) )
				{
				//	throw new DBWrapperException( "DBWrapper instance not initialize correctly on this thread (instance not initialized on this thread or a possible configuration error)" );
				    Instance = new DBWrapper();
				}

				return ( DBWrapper ) InstancesTable[ Thread.CurrentThread ];
			}
			set
			{
				if ( InstancesTable == null )
				{
					InstancesTable = new Hashtable();
				}
				InstancesTable[ Thread.CurrentThread ] = value;
			}
		}

		/// <summary>
		/// Static property to check if the instance is null
		/// </summary>
		public static bool IsInstanceNull
		{
			get
			{
				if ( InstancesTable == null )
				{
					return true;
				}
                return !InstancesTable.ContainsKey(Thread.CurrentThread) || InstancesTable[Thread.CurrentThread] == null;
			}
		}

		/// <summary>
		/// Update the supplied dataset using the supplied commands.
		/// </summary>
		/// <param name="ds">
		/// The dataset to update to/from the database.
		/// </param>
		/// <param name="commands">
		/// The commands passed to the DataAdapter to update the database.
		/// </param>
		public static void Update( ref DataSet ds, Hashtable commands )
		{
			// (mfs)
			// get a DB instance.
			DBWrapper.Instance.TestConnection();

			// (mfs)
			// set the connection for the commands
			SqlCommand Select = ( SqlCommand ) commands["select"];
			if (Select != null)
				Select.Connection = DBWrapper.Instance.ActiveConnection;
			SqlCommand Update = ( SqlCommand ) commands["update"];
			if (Update != null)
				Update.Connection = DBWrapper.Instance.ActiveConnection;
			SqlCommand Insert = ( SqlCommand ) commands["insert"];
			if (Insert != null)
				Insert.Connection = DBWrapper.Instance.ActiveConnection;
			SqlCommand Delete = ( SqlCommand ) commands["delete"];
			if (Delete != null)
				Delete.Connection = DBWrapper.Instance.ActiveConnection;			

			// (mfs)
			// setup the data adapter
			SqlDataAdapter adapter = new SqlDataAdapter();
			adapter.SelectCommand = Select;
			adapter.UpdateCommand = Update;
			adapter.InsertCommand = Insert;
			adapter.DeleteCommand = Delete;
			
			// (mfs)
			// write changes back, then just to be safe, pull the data
			// back again

            if (ds.Tables[0].Rows[0].RowState == DataRowState.Added || ds.Tables[0].Rows[0].RowState == DataRowState.Deleted)
            {
                // (BE) this table mapping is required for insert, but causes an error during update
                adapter.TableMappings.Add("Table", ds.Tables[0].TableName);
            }

            adapter.Update( ds );

//			ds.Tables[0].Rows.Clear();
//
//			//PrintAllErrs( ds );
//
//			//string row = ds.Tables[0].Rows[0];
//
//			try
//			{
//				//ds.EnforceConstraints = false;
//				adapter.Fill( ds );
//				//ds.EnforceConstraints = true;
//			}
//			catch(ConstraintException ce)
//			{
//				PrintAllErrs( ds );
//				throw ce;
//			}

			DBWrapper.Instance.Close();
		}

		/// <summary>
		/// Print the errors in the dataset
		/// </summary>
		/// <param name="myDataSet"></param>
		private static void PrintAllErrs(DataSet myDataSet)
		{
			DataRow[] rowsInError; 

			Console.WriteLine("Printing errors...\n");
     
			foreach(DataTable myTable in myDataSet.Tables)
			{
				// Test if the table has errors. If not, skip it.
				if(myTable.HasErrors)
				{
					// Get an array of all rows with errors.
					rowsInError = myTable.GetErrors();
					// Print the error of each column in each row.
					for(int i = 0; i < rowsInError.Length; i++)
					{
						foreach(DataColumn myCol in myTable.Columns)
						{
							Console.WriteLine(myCol.ColumnName + " " + 
								rowsInError[i].GetColumnError(myCol));
						}
						// Clear the row errors
						rowsInError[i].ClearErrors();
					}
				}
			}
		}


        /// <summary>
        /// Returns a new Guid from the database, which is needed for inserting
        /// a new run into a uniqueidentifier primary key.
        /// </summary>
        /// <returns></returns>
        public static Guid NewID()
        {
            DBWrapper.Instance.TestConnection();
			Guid r = (Guid)DBWrapper.Instance.ExecuteScalar("SELECT newid()");
			DBWrapper.Instance.Close();

            return r;
        }


        /// <summary>
        /// Added by En
        /// Returns a new Guid from the database, which is needed for inserting
        /// a new run into a uniqueidentifier primary key.
        /// </summary>
        /// <returns></returns>
        //public static short NewIntID()
        //{
        //    /// create table newIds([id] smallint identity(1,1), created dateTime default getdate(), [desc] varchar(64) default ''); 
        //    string strSQL = "begin\r\ndeclare @id smallint; set @Id=0;";
        //    strSQL = strSQL + "insert into [newIds] ([desc]) values ('creating new Id test..');  \r\n";
        //    strSQL = strSQL + "select @Id=ident_current('newIds'); \r\n";
        //    strSQL = strSQL + "delete from [newIds] where [Id] < @Id; \r\n";
        //    strSQL = strSQL + "select @Id; \r\n";
        //    strSQL = strSQL + "end \r\n";
            
        //    DBWrapper.Instance.TestConnection();
        //    short r = (short)DBWrapper.Instance.ExecuteScalar(strSQL);// ("SELECT newid()");           
        //    DBWrapper.Instance.Close();

        //    return r;
        //}

         /// <summary>        
        /// Returns a new int id from the database from the corresponding table.
        /// </summary>
        /// <returns></returns>
        public static short NewIntID(string tableName)
        {   
            return NewIntID(tableName, "id");
        }
        /// <summary>        
        /// Returns a new int id from the database from the corresponding table.
        /// </summary>
        /// <returns></returns>
        public static short NewIntID(string tableName, string idField)
        {            
            DBWrapper.Instance.TestConnection();
            string strSQL = "select count(*) from " + tableName;
            short id = short.Parse(DBWrapper.Instance.ExecuteScalar(strSQL).ToString());
            if (id > 0)
            {
                strSQL = "select max(" + idField + ") from " + tableName;
                id = short.Parse(DBWrapper.Instance.ExecuteScalar(strSQL).ToString());
            }

            id++;
            return (short)id;
        }         
            



		/// <summary>
		/// Helpful function for constructing a SqlCommand object from a sql string with parameters
		/// and a variable-length list of triples (param name, param SqlDbType, and the value for the param).
		/// 
		/// For readablity, each parameter triple should be on its own line. 
		/// </summary>
		/// <remarks>
		/// Arbitrary parameter variant.
		/// 
		/// An exception will be thrown if:
		///   1) The number of arguments passed to paramNameTypeValueList is not a multiple of three.
		///   2) In each triple (i.e. one SqlParameter), the first value is not a string.
		///   3) In each triple, the second value is not a SqlDbType.
		/// <example>
		/// SqlCommand command = DBWrapper.BuildCommand(
		///     "INSERT INTO " + __TableName + " (id, name, type) VALUES (@id, @name, @type);",
		///         "@id", SqlDbType.UniqueIdentifier, ID,
		///         "@name", SqlDbType.VarChar, Name,
		///         "@type", SqlDbType.VarChar, Type);
		/// </example>
		/// </remarks>
		/// <param name="sql">SQL command string containing zero or more parameters (@param)</param>
		/// <param name="paramNameTypeValueList"></param>
		/// <returns></returns>
		public static SqlCommand BuildCommand(string sql, params object[] paramNameTypeValueList)
		{
			if (paramNameTypeValueList.Length % 3 != 0)
				throw new DBWrapperException("Must have an even number of params, alternating between string param names, their types and their values.");

			SqlCommand command = new SqlCommand(sql);

			if (paramNameTypeValueList.Length > 0)
			{
				for(int i = 0; i < paramNameTypeValueList.Length; i += 3)
				{
					if (!(paramNameTypeValueList[i] is string))
						throw new DBWrapperException("Must have an even number of params, alternating between string param names, their types and their values.");

					if (!(paramNameTypeValueList[i + 1] is SqlDbType))
						throw new DBWrapperException("Must have an even number of params, alternating between string param names, their SqlDbType and their object value.");


					object val = paramNameTypeValueList[i + 2];
					if (val == null)
						val = DBNull.Value;
					if (val is Guid && (Guid)val == Guid.Empty)
						val = DBNull.Value;

					SqlParameter p = new SqlParameter((string)paramNameTypeValueList[i], (SqlDbType)paramNameTypeValueList[i + 1]);
					p.SourceVersion = DataRowVersion.Original;
					p.Value = val;
					command.Parameters.Add(p);
				}
			}

			return command;
		}

		/// <summary>
		/// Helpful function for constructing a SqlCommand object from a sql string with parameters
		/// and a variable-length list of triples (param name, param SqlDbType, and the value for the param).
		/// </summary>
		/// <remarks>1 parameter variant.</remarks>
		/// <param name="sql"></param>
		/// <param name="paramName"></param>
		/// <param name="paramType"></param>
		/// <param name="paramValue"></param>
		/// <returns></returns>
		public static SqlCommand BuildCommand(string sql, string paramName, SqlDbType paramType, object paramValue)
		{
			return BuildCommand(sql, new object[]{paramName, paramType, paramValue});
		}

		/// <summary>
		/// Helpful function for constructing a SqlCommand object from a sql string with parameters
		/// and a variable-length list of triples (param name, param SqlDbType, and the value for the param).
		/// </summary>
		/// <remarks>2 parameter variant.</remarks>
		/// <param name="sql"></param>
		/// <param name="param1Name"></param>
		/// <param name="param1Type"></param>
		/// <param name="param1Value"></param>
		/// <param name="param2Name"></param>
		/// <param name="param2Type"></param>
		/// <param name="param2Value"></param>
		/// <returns></returns>
		public static SqlCommand BuildCommand(string sql, string param1Name, SqlDbType param1Type, object param1Value, string param2Name, SqlDbType param2Type, object param2Value)
		{
			return BuildCommand(sql, new object[]{param1Name, param1Type, param1Value, param2Name, param2Type, param2Value});
		}

		/// <summary>
		/// Helpful function for constructing a SqlCommand object from a sql string with parameters
		/// and a variable-length list of triples (param name, param SqlDbType, and the value for the param).
		/// </summary>
		/// <remarks>3 parameter variant.</remarks>
		/// <param name="sql"></param>
		/// <param name="param1Name"></param>
		/// <param name="param1Type"></param>
		/// <param name="param1Value"></param>
		/// <param name="param2Name"></param>
		/// <param name="param2Type"></param>
		/// <param name="param2Value"></param>
		/// <param name="param3Name"></param>
		/// <param name="param3Type"></param>
		/// <param name="param3Value"></param>
		/// <returns></returns>
		public static SqlCommand BuildCommand(string sql, string param1Name, SqlDbType param1Type, object param1Value, string param2Name, SqlDbType param2Type, object param2Value, string param3Name, SqlDbType param3Type, object param3Value)
		{
			return BuildCommand(sql, new object[]{param1Name, param1Type, param1Value, param2Name, param2Type, param2Value, param3Name, param3Type, param3Value});
		}

		/// <summary>
		/// Helpful function for constructing a SqlCommand object from a sql string with parameters
		/// and a variable-length list of triples (param name, param SqlDbType, and the value for the param).
		/// </summary>
		/// <remarks>4 parameter variant.</remarks>
		/// <param name="sql"></param>
		/// <param name="param1Name"></param>
		/// <param name="param1Type"></param>
		/// <param name="param1Value"></param>
		/// <param name="param2Name"></param>
		/// <param name="param2Type"></param>
		/// <param name="param2Value"></param>
		/// <param name="param3Name"></param>
		/// <param name="param3Type"></param>
		/// <param name="param3Value"></param>
		/// <param name="param4Name"></param>
		/// <param name="param4Type"></param>
		/// <param name="param4Value"></param>
		/// <returns></returns>
		public static SqlCommand BuildCommand(string sql, string param1Name, SqlDbType param1Type, object param1Value, string param2Name, SqlDbType param2Type, object param2Value, string param3Name, SqlDbType param3Type, object param3Value, string param4Name, SqlDbType param4Type, object param4Value)
		{
			return BuildCommand(sql, new object[]{param1Name, param1Type, param1Value, param2Name, param2Type, param2Value, param3Name, param3Type, param3Value, param4Name, param4Type, param4Value});
		}

        /// <summary>
        /// Preprocesses a data value for direct use in a SQL expression string.
        /// This includes removing any dangerous single quote characters, using a 
        /// valid date format and not adding single quotes if the value is NULL.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string PreprocessSqlArgValue(object o)
        {
            if (o != null)
            {
                string s = o as string;
                if (s != null)
                {
                    s = s.Trim();
                    if ("".Equals(s))
                    {
                        // store NULL instead of an empty/blank string
                        o = null;
                    }
                    else
                    {
                        o = s;
                    }

                    // remove troublesome single quotes
                    o = s.Replace("'", "' + CHAR(39) + '");
                }
            }

            if (o == null)
            {
                return "NULL";
            }
            else if (o is DateTime)
            {
                return "'" + ((DateTime)o).ToString(DBWrapper.DateFormat) + "'";
            }
            else if (o is bool)
            {
                if ((bool)o)
                    return "'1'";
                else
                    return "'0'";
            }
            else
            {
                return "'" + o.ToString() + "'";
            }
        }

        /// <summary>
        /// SQLServer's prefered date format string.
        /// </summary>
        public static readonly string DateFormat = "yyyy-MM-dd HH:mm:ss.fff";	   

		#endregion

        /// <summary>
        /// Execute a SQL query for a many-to-many relationship as a Dictionary with the first column as the key 
        /// and the second column as the values associated with that key.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="L"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramNameTypeValueList"></param>
        /// <returns></returns>
        public Dictionary<K, List<L>> ExecuteQueryDictionaryList<K, L>(string sql, params object[] paramNameTypeValueList)
        {
            Dictionary<K, List<L>> results = new Dictionary<K, List<L>>();
            DBWrapper db = DBWrapper.Instance;

            DataSet t = new DataSet();
            db.ExecuteQuery(out t, sql, paramNameTypeValueList);

            foreach (DataRow r in t.Tables[0].Rows)
            {
                K key = (K)r[0];
                if (!results.ContainsKey(key))
                    results.Add(key, new List<L>());
                results[key].Add((L)r[1]);
            }
            return results;
        }

        public Dictionary<K, List<object[]>> ExecuteQueryDictionaryObjectList<K, L>(string sql, params object[] paramNameTypeValueList)
        {
            Dictionary<K, List<object[]>> results = new Dictionary<K, List<object[]>>();

            DataSet t = new DataSet();
            ExecuteQuery(out t, sql, paramNameTypeValueList);

            foreach (DataRow r in t.Tables[0].Rows)
            {
                K key = (K)r[0];

                if (!results.ContainsKey(key))
                {
                    results.Add(key, new List<object[]>());
                }
                results[key].Add(r.ItemArray);
            }
            return results;
        }

        /// <summary>
        /// Execute a SQL query and return the results in the first column as a List<>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<T> ExecuteQueryList<T>(string sql, params object[] paramNameTypeValueList)
        {
            List<T> results = new List<T>();
            DBWrapper db = DBWrapper.Instance;

            DataSet t = new DataSet();
            db.ExecuteQuery(out t, sql, paramNameTypeValueList);

            foreach (DataRow r in t.Tables[0].Rows)
            {
                results.Add((T)r[0]);
            }
            return results;
        }

        /// <summary>
        /// Execute a SQL query of key-value pairs in the first two columns and return as a Dictionary<,>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Dictionary<K, V> ExecuteQueryDictionary<K, V>(string sql, params object[] paramNameTypeValueList)
        {
            Dictionary<K, V> results = new Dictionary<K, V>();
            DBWrapper db = DBWrapper.Instance;

            DataSet t = new DataSet();
            db.ExecuteQuery(out t, sql, paramNameTypeValueList);

            foreach (DataRow r in t.Tables[0].Rows)
            {
                results.Add((K)r[0], (V)r[1]);
            }
            return results;
        }

    } // End class ChangeMeClassName

} // End namespace ChangeMeNamespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: DBWrapper.cs,v 1.6 2009/09/09 15:48:14 xjqi Exp $
	$Log: DBWrapper.cs,v $
	Revision 1.6  2009/09/09 15:48:14  xjqi
	*** empty log message ***
	
	Revision 1.5  2009/04/03 21:16:03  xjqi
	Visualization Part: Given a model id, visualize compartments in hierarchy with species,reactions.
	Not integrating with web interface.
	
	Revision 1.4  2009/02/06 22:33:25  mitali
	*** empty log message ***
	
	Revision 1.3  2009/01/26 16:35:16  mitali
	*** empty log message ***
	
	Revision 1.2  2009/01/08 20:47:59  ann
	*** empty log message ***
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.6  2008/05/02 21:35:29  brendan
	*** empty log message ***
	
	Revision 1.5  2008/02/20 18:51:54  brendan
	*** empty log message ***
	
	Revision 1.4  2007/12/30 22:22:06  divya
	*** empty log message ***
	
	Revision 1.3  2007/03/26 16:38:30  brendan
	changed AQI's main load node function to use prototype.js's Ajax.Updater() function to get HTML from the server and also allow for javascript blocks in this HTML to be run.  Bugfix in DBWrapper.
	
	Revision 1.2  2007/02/07 23:55:09  brendan
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.31  2006/06/30 20:02:38  greg
	There have been some very big changes here lately...
	
	 - Query logger
	The DBWrapper class now has support for logging queries in a format that you can import into Excel, etc. for analysis.  There are some Web.config lines you'll have to add, though.
	
	 - JavaScript redirects
	The dropdown list on the main browser bar uses JavaScript for redirects.  Yay.
	
	 - Visual issues
	There were several unresolved visual issues (mostly stemming from the way IE and Firefox render pages differently), but most of them should now be resolved.
	
	 - Ajax browsing
	The biggest part of this update involves Ajax.  All pages load significantly faster now, and data requests are made asynchronously.  The fine details about which panels will start open by default and everything can be worked out later... but for now it appears that everything is working nicely.
	
	Revision 1.30  2006/05/16 19:47:27  gokhan
	*** empty log message ***
	
	Revision 1.29  2006/05/11 16:18:39  marc
	Woah, Merge from GeneOntologyFeatures
	

	Revision 1.28  2006/04/17 18:59:53  brian
	1. Sections of code that are dependent on PathwayLib2 will throw an exception
	2. Updated a few queries to reflect new database layout
	
	Revision 1.27  2006/02/27 22:07:16  brian
	*** empty log message ***
	
	Revision 1.26.2.2  2006/02/22 23:41:42  brian
	1.  Unifying organism and organism_group tables
	2.  Operations to get pathways by organism or group are now handled by polymorphic functions
	
	Revision 1.26.2.1  2006/02/14 20:57:35  brian
	1. Broken the code to reveal (all?) references to PathwaysService2
	2. Fixed several warnings due to outdated params tags
	
	Revision 1.26.6.1  2006/02/21 22:09:00  marc
	Entire DataSet is not copied for each row in LoadMultiple, just the schema
	
	Revision 1.26  2005/11/02 20:35:16  fatih
	Test functions are working for pathwayslib objects
	
	Revision 1.25  2005/10/31 19:25:11  fatih
	*** empty log message ***
	
	Revision 1.24  2005/10/05 23:57:18  brendan
	Tweaked the null instance error message to be more helpful and added some xml comments.
	
	Revision 1.23  2005/08/24 22:27:52  michael
	Fixing SQL Connection Overflow bug
	
	Revision 1.22  2005/08/08 20:13:38  michael
	Website content updates
	
	Revision 1.21  2005/08/04 01:29:59  michael
	Debugging search and pagination
	
	Revision 1.20  2005/07/28 21:00:52  michael
	Updating display window to accept simple query controls
	
	Revision 1.19  2005/07/25 16:50:33  brandon
	added some XML comments
	
	Revision 1.18  2005/07/21 20:07:34  brandon
	Added a 4 parameter variant to the DBWrapper BuildCommand( ) function
	
	Revision 1.17  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.16  2005/07/18 19:18:12  brandon
	Added another test file, Brendan fixed his Protein table fix thing, and added a query to geneProducts to override the GetAllPathways in ServerMolecularEntity
	
	Revision 1.15  2005/07/05 21:08:15  brendan
	Added a CommandBuild convience function to DBWrapper to simplify the code for calling SQL commands with @ params.  Modified ServerPathway to use this new function and tested it.
	
	Revision 1.14  2005/07/01 20:47:39  brendan
	Work on inheritance & the object wrapper.
	
	Revision 1.13  2005/06/28 21:53:06  brendan
	ServerPathway now works properly for INSERT, SELECT, UPDATE and DELETE.  Changes have been to DBWrapper, DBRow and ServerObject to support this properly.
	
	Revision 1.12  2005/06/24 21:57:32  brendan
	Checkin of work on database object design.
	
	Revision 1.11  2005/06/22 22:06:31  brendan
	Progress on data model.  FieldLoader functions moved to DBRow.  Working on support for insert.
	
	Revision 1.10  2005/06/21 02:26:30  michael
	ADO tests and debugging
	
	Revision 1.9  2005/06/20 19:39:31  michael
	debugging ADO updating of database.
	
	Revision 1.8  2005/06/20 17:53:15  michael
	Bug fixes
	
	Revision 1.7  2005/06/16 21:14:11  michael
	testing data model
	
	Revision 1.6  2005/06/16 16:10:50  michael
	finishing up DBWrapper class and beginning work on creating the object model.
	
	Revision 1.5  2005/06/14 20:40:51  michael
	new version of DBWrapper
	
	Revision 1.4  2005/06/13 22:53:08  michael
	Data model changes implementation in progress.
	
	Revision 1.3  2005/06/13 19:17:59  michael
	work in progress
	
	Revision 1.2  2005/06/10 21:45:43  brendan
	Refactored ProcessArg function for use in FieldLoader.
	
	Revision 1.1  2005/06/08 22:46:08  michael
	DBWrapper class first commit.
	
------------------------------------------------------------------------*/
#endregion