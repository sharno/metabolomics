using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using PathwaysLib.QueryObjects;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.QueryObjects
{
	/// <summary>
	/// A query field for process names
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QFieldProcessName : QField
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="initValue">The initial value; use string.Empty for none</param>
		public QFieldProcessName( string initValue  ) : base( initValue ) {}

        public override string SelectSQL
        {
            get { return "PR{0}.name AS [Process # Name], PR{0}.generic_process_id AS [IDpc #_{0}]"; }
        }
		public override string[] FromTables
        {
            get { return new string[] { "processes PR{0}" }; }
        }
        public override string WhereValueClause(string val)
        {
            //return "PR{0}.name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + val + "%");
            return "PR{0}.name = " + DBWrapper.PreprocessSqlArgValue(val);
        }
        //public override string[] WhereSQL( string val )
        //{
        //    return new string[] { "(PR{0}.name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + val + "%") + ")" };
        //}

		public override int SearchCode { get { return 1; } }
        public override Dictionary<string, string> SearchFor(string search)
		{
            Dictionary<string, string> results = new Dictionary<string, string>();
            foreach (ServerProcess sp in ServerProcess.FindProcesses(search, SearchMethod.Contains, 30))
            {
                if (!results.ContainsValue(sp.Name.ToString()))
                    results.Add(sp.GenericProcessID.ToString(), sp.Name);
            }
            return results;
		}
	}
}

			