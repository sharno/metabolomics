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
    public class Module
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
        public Module(int id, string name, string description)
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
        public static List<Module> GetModules(string connectionString)
        {
            DBWrapper dbWrapper = null;
            List<Module> moduleList = null;

            try
            {
                dbWrapper = new DBWrapper(connectionString);

                string sql = "SELECT * FROM module";
                SqlDataReader sqlDataReader = dbWrapper.ExecuteQueryReader(connectionString, sql, new object[0]);

                moduleList = new List<Module>();

                int tmpId = 0;
                string tmpName = string.Empty;
                string tmpDescription = string.Empty;

                while (sqlDataReader.Read() == true)
                {
                    tmpId = (int) sqlDataReader["id"];
                    tmpName = (string) sqlDataReader["name"];
                    tmpDescription = (string) sqlDataReader["description"];
                    
                    Module tmpModule = new Module(tmpId, tmpName, tmpDescription);
                    moduleList.Add(tmpModule);
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

            return moduleList;
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
