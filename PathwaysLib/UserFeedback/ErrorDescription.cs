using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using PathwaysLib.ServerObjects;
namespace PathwaysLib.UserFeedback
{
    /// <summary>
    /// 
    /// </summary>
    public class ErrorDescription
    {
        private int _id = 0;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private int _errorTypeId = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="errorTypeId"></param>
        public ErrorDescription(int id, string name, string description, int errorTypeId)
        {
            _id = id;
            _name = name;
            _description = description;
            _errorTypeId = errorTypeId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<ErrorDescription> GetErrorDescriptions(string connectionString)
        {
            DBWrapper dbWrapper = null;
            List<ErrorDescription> errorDescriptionList = null;
            SqlDataReader sqlDataReader = null;

            try
            {
                dbWrapper = new DBWrapper(connectionString);

                string sql = "SELECT * FROM error_description";
                sqlDataReader = dbWrapper.ExecuteQueryReader(connectionString, sql, new object[0]);

                errorDescriptionList = new List<ErrorDescription>();

                int tmpId = 0;
                int tmpErrorTypeId = 0;
                string tmpName = string.Empty;
                string tmpDescription = string.Empty;

                while (sqlDataReader.Read() == true)
                {
                    tmpId = (int) sqlDataReader["id"];
                    tmpErrorTypeId = (int)sqlDataReader["error_type_id"];
                    tmpName = (string) sqlDataReader["name"];
                    tmpDescription = (string) sqlDataReader["description"];
                    
                    ErrorDescription tmpErrorDescription = new ErrorDescription(tmpId, tmpName, tmpDescription, tmpErrorTypeId);
                    errorDescriptionList.Add(tmpErrorDescription);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dbWrapper != null)
                {
                    dbWrapper.Close();
                }

                if (sqlDataReader != null)
                {
                    dbWrapper.Close();
                }
            }

            return errorDescriptionList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="errorTypeName"></param>
        /// <returns></returns>
        public static List<ErrorDescription> GetErrorDescriptionsByErrorType(string connectionString, string errorTypeName)
        {
            List<ErrorDescription> errorDescriptions = null;
            DBWrapper dbWrapper = null;
            SqlDataReader sqlDataReader = null;

            try
            {
                dbWrapper = new DBWrapper(connectionString);
                string sql = "EXECUTE Get_ErrorDescriptions_by_ErrorType \'" + errorTypeName +"'";

                sqlDataReader = dbWrapper.ExecuteQueryReader(connectionString, sql, new object[0]);

                errorDescriptions = new List<ErrorDescription>();

                int tmpId = 0;
                int tmpErrorTypeId = 0;
                string tmpName = string.Empty;
                string tmpDescription = string.Empty;

                while (sqlDataReader.Read() == true)
                {
                    tmpId = (int)sqlDataReader["id"];
                    tmpErrorTypeId = (int)sqlDataReader["error_type_id"];
                    tmpName = (string)sqlDataReader["name"];
                    tmpDescription = (string)sqlDataReader["description"];

                    ErrorDescription tmpErrorDescription = new ErrorDescription(tmpId, tmpName, tmpDescription, tmpErrorTypeId);
                    errorDescriptions.Add(tmpErrorDescription);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dbWrapper != null)
                {
                    dbWrapper.Close();
                }

                if (sqlDataReader != null)
                {
                    sqlDataReader.Close();
                }
            }

            return errorDescriptions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<ErrorDescription> GetErrorDescriptionsByErrorType(string connectionString, int errorTypeId)
        { 
            List<ErrorDescription> errorDescriptions = null;
            DBWrapper dbWrapper = null;
            SqlDataReader sqlDataReader = null;

            try
            {
                dbWrapper = new DBWrapper(connectionString);
                string sql = "SELECT * FROM error_description WHERE error_type_id = " + errorTypeId.ToString();

                sqlDataReader = dbWrapper.ExecuteQueryReader(connectionString, sql, new object[0]);

                errorDescriptions = new List<ErrorDescription>();

                int tmpId = 0;
                int tmpErrorTypeId = 0;
                string tmpName = string.Empty;
                string tmpDescription = string.Empty;

                while (sqlDataReader.Read() == true)
                {
                    tmpId = (int)sqlDataReader["id"];
                    tmpErrorTypeId = (int)sqlDataReader["error_type_id"];
                    tmpName = (string)sqlDataReader["name"];
                    tmpDescription = (string)sqlDataReader["description"];

                    ErrorDescription tmpErrorDescription = new ErrorDescription(tmpId, tmpName, tmpDescription, tmpErrorTypeId);
                    errorDescriptions.Add(tmpErrorDescription);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dbWrapper != null)
                {
                    dbWrapper.Close();
                }

                if (sqlDataReader != null)
                {
                    sqlDataReader.Close();
                }
            }

            return errorDescriptions;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get { return _id; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ErrorTypeId
        {
            get { return _errorTypeId; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return _description; }
        }
    }
}
