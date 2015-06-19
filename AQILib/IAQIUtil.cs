using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace AQILib
{
    /// <summary>
    /// An interface constructed to expose many project-specific methods to the library
    /// </summary>
    public interface IAQIUtil
    {
        string PreprocessSqlArgValue(object o);
        int ExecuteSqlQuery(out DataTable dataTable, string sql);

        string AQIDataURL
        {
            get;
        }

        string AQIForwardLink
        {
            get;
        }

        string AjaxHandlerURL
        {
            get;
        }

        string ColumnNameToTypeCode(string columnName);
    }
}
