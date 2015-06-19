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
    public class ErrorType
    {
        private int _id = 0;
        private string _name = string.Empty;
        private string _description = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public ErrorType(int id, string name, string description)
        {
            _id = id;
            _name = name;
            _description = description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<ErrorType> GetErrorTypes(string connectionString)
        {
            DBWrapper dbWrapper = null;
            List<ErrorType> errorTypeList = null;

            try
            {
                dbWrapper = new DBWrapper(connectionString);

                string sql = "SELECT * FROM error_type";
                SqlDataReader sqlDataReader = dbWrapper.ExecuteQueryReader(connectionString, sql, new object[0]);

                errorTypeList = new List<ErrorType>();

                int tmpId = 0;
                string tmpName = string.Empty;
                string tmpDescription = string.Empty;

                while (sqlDataReader.Read() == true)
                {
                    tmpId = (int) sqlDataReader["id"];
                    tmpName = (string) sqlDataReader["name"];
                    tmpDescription = (string) sqlDataReader["description"];
                    
                    ErrorType tmpErrorType = new ErrorType(tmpId, tmpName, tmpDescription);
                    errorTypeList.Add(tmpErrorType);
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
            }

            return errorTypeList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public static List<ErrorType> GetErrorTypesByModule(string connectionString, string module)
        {
            DBWrapper dbWrapper = null;
            SqlCommand sqlCommand = null;
            List<ErrorType> errorTypeList = null;
            SqlDataReader dataReader = null;

            try
            {
                string commandText = "EXECUTE dbo.Get_ErrorTypes_For_Module '" + module + "'";
                sqlCommand = new SqlCommand(commandText);
                dbWrapper = new DBWrapper(connectionString);
                
                dataReader = dbWrapper.ExecuteQueryReader(ref sqlCommand);

                errorTypeList = new List<ErrorType>();
                ErrorType errorTypeTmp = null;
                while (dataReader.Read() == true)
                {
                    errorTypeTmp = new ErrorType( (int) dataReader["id"], (string) dataReader["name"], (string) dataReader["description"] );
                    errorTypeList.Add(errorTypeTmp);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                dbWrapper.Close();
                dataReader.Close();
            }

            return errorTypeList;
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
        public string Description
        {
            get { return _description; }
        }
    }
}
