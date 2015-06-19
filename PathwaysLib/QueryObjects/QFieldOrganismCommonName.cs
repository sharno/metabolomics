using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using PathwaysLib.QueryObjects;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.QueryObjects
{
	/// <summary>
	/// A query field for organism common names
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QFieldOrganismCommonName : QField
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="initValue">The initial value; use string.Empty for none</param>
		public QFieldOrganismCommonName( string initValue ) : base( initValue ) {}

        public override string SelectSQL
        {
            get { return "OG{0}.common_name AS [Organism # Common Name], OG{0}.id AS [IDorg#_{0}]"; }
        }
		public override string[] FromTables
        {
            get { return new string[] { "organism_groups OG{0}" }; }
        }
        public override string WhereValueClause(string val)
        {
            return "OG{0}.common_name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + val + "%");
        }
        //public override string[] WhereSQL( string val )
        //{
        //    return new string[] { "(OG{0}.common_name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + val + "%") + ")" };
        //}

		public override int SearchCode { get { return 3; } }
        public override Dictionary<string, string> SearchFor(string search)
		{
            Dictionary<string, string> results = new Dictionary<string, string>();
            foreach (ServerOrganismGroup sog in ServerOrganismGroup.FindByCommonName(search, SearchMethod.Contains))
            {
                if (!results.ContainsValue(sog.Name.ToString()))
                    results.Add(sog.ID.ToString(), sog.Name);
            }
            return results;
		}
	}
}