using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using PathwaysLib.QueryObjects;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.QueryObjects
{
	/// <summary>
	/// A query field for organism scientific names
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QFieldOrganismScientificName : QField
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="initValue">The initial value; use string.Empty for none</param>
		public QFieldOrganismScientificName( string initValue ) : base( initValue) {}

		public override string SelectSQL
		{
            get { return "OG{0}.scientific_name AS [Organism # Scientific Name], OG{0}.id AS [IDorg#_{0}]"; }
        }
		public override string[] FromTables
        {
            get { return new string[] { "organism_groups OG{0}" }; }
        }
        public override string WhereValueClause(string val)
        {
            return "OG{0}.scientific_name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + val + "%");
        }
        //public override string[] WhereSQL( string val )
        //{
        //    return new string[] { "(OG{0}.scientific_name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + val + "%") + ")" };
        //}

		public override int SearchCode { get { return 3; } }
        public override Dictionary<string, string> SearchFor(string search)
		{
            Dictionary<string, string> results = new Dictionary<string, string>();
            foreach (ServerOrganismGroup sog in ServerOrganismGroup.FindByName(search, SearchMethod.Contains))
            {
                if (!results.ContainsValue(sog.Name.ToString()))
                    results.Add(sog.ID.ToString(), sog.Name);
            }
            return results;
		}
	}
}	