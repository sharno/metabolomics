#region Using Declarations
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using PathwaysLib.SoapObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
#endregion

namespace PathwaysLib.ServerObjects
{
    public class ServerNodeCode : ServerObject
    {

        #region Constructor, Destructor, ToString        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nodeCode"></param>
        public ServerNodeCode(string id, string nodeCode)
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow(__TableName);

            this.ID = id;
            this.NodeCode = nodeCode;            
        }

        public ServerNodeCode()
        {
         
        }
       

        /// <summary>
        /// Constructor for server node code
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerNodeCode object from a
        /// DBRow.
        /// </remarks>
        /// <param name="data">
        /// DBRow to load into the object.
        /// </param>
        public ServerNodeCode(DBRow data)
        {
            // (mfs)
            // setup object
            __DBRow = data;

        }

        /// <summary>
        /// Destructor for the ServerECNumber class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerNodeCode()
        {
        }
        #endregion


        #region Member Variables
        protected string __TableName ="GONodeCodes" ;
        protected string __IdColumnName = "goid" ;
        protected string __NodeCodeColumnName = "nodeCode" ;
        #endregion

        #region Properties
        /// <summary>
        /// Get/set the Id.
        /// </summary>
        public string ID
        {
            get
            {
                return __DBRow.GetString(__IdColumnName);
            }
            set
            {
                __DBRow.SetString(__IdColumnName, value);
            }
        }

        /// <summary>
        /// Get/set Node Code.
        /// </summary>
        public string NodeCode
        {
            get
            {
                return __DBRow.GetString(__NodeCodeColumnName);
            }
            set
            {
                __DBRow.SetString(__NodeCodeColumnName, value);                
            }
        }
        
        #endregion


        #region Methods
        //implement later if needed
        public override SoapObject PrepareForSoap(SoapObject derived)
        {
            return null;
        }

        //implement later if needed
        protected override void UpdateFromSoap(SoapObject o)
        {
        
        }
        
        
        #region ADO.NET SqlCommands


        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {            
            __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (" + __IdColumnName + ", " + __NodeCodeColumnName + ") VALUES (@id, @code);",
                "@id", SqlDbType.VarChar, ID,
                "@code", SqlDbType.VarChar, NodeCode);

            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE " + __IdColumnName + " = @id;",
                "@id", SqlDbType.VarChar, ID);

            __DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET " + __NodeCodeColumnName + " = @code WHERE " + __IdColumnName + " = @id;",
                "@id", SqlDbType.VarChar, ID,
                "@code", SqlDbType.VarChar, NodeCode);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE " + __IdColumnName + " = @id;",
                 "@id", SqlDbType.VarChar, ID);
        }
        #endregion
        #endregion


        #region Static Methods
        /// <summary>
        /// This method should be called first before creating any instance of the ServerNodeCode class. It sets the table name, id and nodeCode column names.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="idColumnName"></param>
        /// <param name="nodeCodeColumnName"></param>
        public void SetSchemaParameters(string tableName, string idColumnName, string nodeCodeColumnName)
        {
            __TableName = tableName;
            __IdColumnName = idColumnName;
            __NodeCodeColumnName = nodeCodeColumnName;
        }

        public bool IsEmpty()
        {
            string query;
            SqlCommand command;
            DataSet ds;

            query = "SELECT top 1 * FROM " + __TableName;
                    
            command = DBWrapper.BuildCommand(query);
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            if (ds.Tables[0].Rows.Count > 0)
                return false;
            else
                return true;
            
        }
        public string[] GetNodeCodes()
        {
            return GetNodeCodes(this.NodeCode);
        }
        public string GetFirstNodeCode()
        {
            return GetFirstNodeCode(this.NodeCode);
        }
        /// <summary>
        /// Returns the node codes for a given term.
        /// </summary>
        /// <param name="termId"></param>
        /// <returns></returns>
        public string[] GetNodeCodes(string termId)
        { 
           
            string query;
            SqlCommand command;
            DataSet ds;
            
            query = "SELECT " + __NodeCodeColumnName + " FROM " + __TableName
                    + " WHERE " + __IdColumnName + " = \'" + termId + "\'";
            command = DBWrapper.BuildCommand(query);
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);

            string[] nodeCodes = new string[ds.Tables[0].Rows.Count];
            int i = 0;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                nodeCodes[i] = dr[__NodeCodeColumnName].ToString();
                i++;
            }
            return nodeCodes;
        }
         /// <summary>
        /// Returns the first node code for a given term.
        /// </summary>
        /// <param name="termId"></param>
        /// <returns></returns>
        public string GetFirstNodeCode(string termId)
        {

            string query;
            SqlCommand command;
            DataSet ds;

            query = "SELECT top 1 " + __NodeCodeColumnName + " FROM " + __TableName
                    + " WHERE " + __IdColumnName + " = \'" + termId + "\'";
            command = DBWrapper.BuildCommand(query);
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);

            string nodeCode = ds.Tables[0].Rows[0][0].ToString();
                
            return nodeCode;
        }
        /// <summary>
        /// Returns descendants of a given ontology term as a comma-separated list of ids (including the id of the original term)
        /// </summary>
        /// <param name="termId"></param>
        /// <returns></returns>
        public string GetDescendantTerms(string termId)
        {
            string[] nodeCodes = GetNodeCodes(termId);

            string likeStatement = "(";
            foreach (string nc in nodeCodes)
            {
                likeStatement += __NodeCodeColumnName + @" like '" + nc + @"%' OR ";                
            }
            likeStatement = likeStatement.Substring(0, likeStatement.Length - 4);
            likeStatement += ")";

            string descendantTerms = "(";
            string query;
            SqlCommand command;
            DataSet ds;
                              
            query = "SELECT " + __IdColumnName + " FROM " + __TableName
                    + " WHERE " + likeStatement;
            command = DBWrapper.BuildCommand(query);
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                descendantTerms += "'" + dr[__IdColumnName].ToString() + "', ";
            }
            descendantTerms = descendantTerms.Substring(0, descendantTerms.Length - 2);
            descendantTerms += ")";
            return descendantTerms;
        }

        /// <summary>
        /// Returns children of a given ontology term as a comma-separated list of ids (including the id of the original term)
        /// </summary>
        /// <param name="termId"></param>
        /// <returns></returns>
        public string GetChildTerms(string termId)
        {
            string nc = GetFirstNodeCode(termId);

            string likeStatement = "(";
            likeStatement += __NodeCodeColumnName + @" like '" + nc + @"%' AND 
                            charindex('.', " + __NodeCodeColumnName + @", " + (nc.Length + 1) + @")-len(" + __NodeCodeColumnName + @") = 0";
                            //charindex('.', " + __NodeCodeColumnName + @", charindex('.', " + __NodeCodeColumnName + @", " + (nc.Length + 1) + @"))-len(" + __NodeCodeColumnName + @") = 0";
                //where nodeCode like '1.1.%'
                //and charindex('.', nodeCode, charindex('.', nodeCode, 5))-len(nodecode) = 0                       
            likeStatement += ")";

            string childTerms = "(";
            string query;
            SqlCommand command;
            DataSet ds;

            query = "SELECT " + __IdColumnName + " FROM " + __TableName
                    + " WHERE " + likeStatement;
            command = DBWrapper.BuildCommand(query);
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            if (ds.Tables[0].Rows.Count == 0)
                return "";
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                childTerms += "'" + dr[__IdColumnName].ToString() + "', ";
            }
            childTerms = childTerms.Substring(0, childTerms.Length - 2);
            childTerms += ")";
            return childTerms;
        }

        public int GetChildCount(string termId)
        {
            string nc = GetFirstNodeCode(termId);

            string likeStatement = "(";
            likeStatement += __NodeCodeColumnName + @" like '" + nc + @"%' AND 
                            charindex('.', " + __NodeCodeColumnName + @", charindex('.', " + __NodeCodeColumnName + @", " + (nc.Length + 1) + @"))-len(" + __NodeCodeColumnName + @") = 0";
            //where nodeCode like '1.1.%'
            //and charindex('.', nodeCode, charindex('.', nodeCode, 5))-len(nodecode) = 0                       
            likeStatement += ")";

            string childTerms = "(";
            string query;
            SqlCommand command;
            DataSet ds;

            query = "SELECT COUNT (DISTINCT " + __IdColumnName + ") FROM " + __TableName
                    + " WHERE " + likeStatement;
            command = DBWrapper.BuildCommand(query);
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            return int.Parse(ds.Tables[0].Rows[0][0].ToString());            
        }


        /// <summary>
        /// Populates the coresponding node code table based on the information in the provided hierarchy table
        /// </summary>
        /// <param name="conceptHierarchyTableName"></param>
        /// <param name="childIdColumnName"></param>
        /// <param name="parentIdColumnName"></param>
        /// <param name="emptyCurrentNodeCodesTable"></param>
        public void PopulateNodeCodes(string conceptHierarchyTableName, string childIdColumnName, string parentIdColumnName, bool emptyCurrentNodeCodesTable)
        {
            string query;
            SqlCommand command;

            if (emptyCurrentNodeCodesTable)
            {
                //empty the curent node codes table
                query = "DELETE FROM " + __TableName;
                command = DBWrapper.BuildCommand(query);
                DBWrapper.Instance.ExecuteNonQuery(ref command);
            }

            //start with the root terms
            query = @"SELECT DISTINCT " + childIdColumnName
                       + " FROM " + conceptHierarchyTableName
                       + " WHERE " + parentIdColumnName + " = \'all\'"
                       + "    OR " + parentIdColumnName + " is null";
                    
            command = DBWrapper.BuildCommand(query);  
            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);

            List<string> termIds = new List<string>(1000);
            List<string> nodeCodes = new List<string>(1000);
            ServerNodeCode nc;

            int siblingIndex = 1;
            string termId, parentId, parentNodeCode = "", nodeCode = "";
            
            foreach (DataRow dr in ds.Tables[0].Rows)
            {                
                termId = dr[childIdColumnName].ToString();      
                nodeCode = parentNodeCode + siblingIndex + ".";
                nc = new ServerNodeCode(termId, nodeCode);
                nc.UpdateDatabase();
                termIds.Add(termId);    
                nodeCodes.Add(nodeCode);

                siblingIndex++;
            }
            
            for(int i = 0; i < termIds.Count; i++)
            {
                parentId = termIds[i];
                parentNodeCode = nodeCodes[i];

                //get children
                query = @"SELECT DISTINCT " + childIdColumnName
                           + " FROM " + conceptHierarchyTableName
                           + " WHERE " + parentIdColumnName + " = @parentId";                                       
                
                command = DBWrapper.BuildCommand(query, "@parentId", SqlDbType.VarChar, parentId);                
                DBWrapper.Instance.ExecuteQuery(out ds, ref command);
                siblingIndex = 1;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {                
                    termId = dr[childIdColumnName].ToString();        
                    nodeCode = parentNodeCode + siblingIndex + ".";
                    nc = new ServerNodeCode(termId, nodeCode);    
                    nc.UpdateDatabase();
                    termIds.Add(termId);
                    nodeCodes.Add(nodeCode);
                    siblingIndex++;
                }
            }
        }
        
        #endregion

    } // End class

}
