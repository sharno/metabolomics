using System;
using System.Collections.Generic;
using System.Text;

using AQILib;
using System.Data;
using PathwaysLib.ServerObjects;
using PathwaysLib.Utilities;

namespace PathwaysLib.AQI
{
    /// <summary>
    /// Implements pathways project specific settings & functions needed by the AQI library.
    /// </summary>
    public class PathwaysAQIUtil : IAQIUtil
    {
        private PathwaysAQIUtil()
        {
        }

        #region IAQIUtil Members

        /// <summary>
        /// Preprocess user input for inclusion in a generated SQL query
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public string PreprocessSqlArgValue(object o)
        {
            return DBWrapper.PreprocessSqlArgValue(o);
        }

        /// <summary>
        /// Execute a SQL query
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteSqlQuery(out DataTable dataTable, string sql)
        {
            DataSet results;
            int rc = DBWrapper.Instance.ExecuteQuery(out results, sql);
            dataTable = results.Tables[0];
            return rc;
        }

        /// <summary>
        /// The full URL for AQI autocomplete data service.
        /// </summary>
        public string AQIDataURL
        {
            //get { return LinkHelper.PathwaysWebBaseUrl + "/Web/JSONData.aspx?node_type={0}&field_type={1}&input_type={2}&query_str=%{{searchString}}&xml_flag=0&quote_flag=0"; }
            get { return LinkHelper.PathwaysWebBaseUrl + @"/Web/AjaxHandler.aspx?op=InSearch&fieldId={3}&nodeType={0}&fieldTypeId={1}&inputType={2}&queryString=%{{searchString}}&treeXmlString="" + encodeURIComponent(AQI.GetQueryXml(AQI.GetRootId(), null, null)) + ""&xmlFlag=0&quoteFlag=0"; }
        }

        public string AjaxHandlerURL
        {
            get { return "AjaxHandler.aspx"; }
        }


        /// <summary>
        /// Format string specifying an HTML link to an object using a typed object's ID
        /// </summary>
        public string AQIForwardLink
        {
            get { return "<a href=\"LinkForwarder.aspx?rid={0}&amp;rtype={1}\">{2}</a>"; }
        }

        public string ColumnNameToTypeCode(string columnName)
        {
            string code = columnName.Substring(2, 3).TrimEnd(" ".ToCharArray());

            if (code == "ogp") // treat orgs & org groups the same
                code = "org";

            return code;
        }

        #endregion


        static IAQIUtil instance = new PathwaysAQIUtil();

        /// <summary>
        /// Gets the singleton instance of the Pathways AQI util class.
        /// </summary>
        public static IAQIUtil Instance
        {
            get
            {
                return instance;
            }
        }

    }
}
