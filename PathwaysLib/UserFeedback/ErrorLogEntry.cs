using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

//using PathwaysLib.ServerObjects;

namespace PathwaysLib.UserFeedback
{
    /// <summary>
    /// 
    /// </summary>
    public class ErrorLogEntry
    {
        private int _id = 0;
        private string _errorText = string.Empty;
        private DateTime _date;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errorText"></param>
        /// <param name="date"></param>
        public ErrorLogEntry(int id, string errorText, DateTime date)
        {
            _id = id;
            _errorText = errorText;
            _date = date;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<ErrorLogEntry> GetErrors(string connectionString)
        {
            List<ErrorLogEntry> errorLogEntries = null;

            SqlCommand sqlCommand = null;
            SqlConnection sqlConnection = null;
            SqlDataReader dataReader = null;

            try
            {
                string sqlText = "SELECT * FROM dbo.error";
                sqlConnection = new SqlConnection(connectionString);
                sqlCommand = new SqlCommand(sqlText, sqlConnection);

                dataReader = sqlCommand.ExecuteReader();

                errorLogEntries = new List<ErrorLogEntry>();

                while (dataReader.Read() == true)
                {
                    errorLogEntries.Add(new ErrorLogEntry( (int) dataReader["id"], (string) dataReader["errorText"], 
                        (DateTime) dataReader["date"]));    
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                }

                if (dataReader != null)
                {
                    dataReader.Close();
                }
            }

            return errorLogEntries;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="errorText"></param>
        public static void InsertError(string connectionString, string errorText)
        {
            SqlCommand sqlCommand = null;
            SqlConnection sqlConnection = null;

            try
            {
                errorText = errorText.Replace("'", "''");
                string sqlText = "EXECUTE dbo.Insert_Into_Error '" + errorText + "'";

                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                sqlCommand = new SqlCommand(sqlText, sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }
            catch
            {
                // swallow exception.  This code is used in catch blocks, and should not throw an exceptions
            }
            finally
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                }
            }
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
        public string ErrorText
        {
            get { return _errorText; }
        }
    }
}
