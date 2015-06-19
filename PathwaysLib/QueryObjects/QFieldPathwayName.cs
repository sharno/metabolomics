using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using PathwaysLib.QueryObjects;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.QueryObjects
{
	/// <summary>
	/// A query field for pathway names
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QFieldPathwayName : QField
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="initValue">The initial value; use string.Empty for none</param>
		public QFieldPathwayName( string initValue ) : base( initValue ) {}

        public override string SelectSQL
        {
            get { return "PW{0}.name AS [Pathway # Name], PW{0}.id AS [IDpw #_{0}]"; }
        }
		public override string[] FromTables
        {
            get { return new string[] { "pathways PW{0}" }; }
        }
        public override string WhereValueClause(string val)
        {
            //return "PW{0}.name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + val + "%");
            return "PW{0}.name = " + DBWrapper.PreprocessSqlArgValue(val);
        }
        //public override string[] WhereSQL( string val )
        //{
        //    return new string[] { "(PW{0}.name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + val + "%") + ")" };
        //}

		public override int SearchCode { get { return 0; } }
        public override Dictionary<string, string> SearchFor(string search)
		{
            Dictionary<string, string> results = new Dictionary<string, string>();
            foreach (ServerPathway spw in ServerPathway.FindPathways(search, SearchMethod.Contains))
            {
                if (!results.ContainsValue(spw.Name.ToString()))
                    results.Add(spw.ID.ToString(), spw.Name);
            }
            return results;
		}
	}
}