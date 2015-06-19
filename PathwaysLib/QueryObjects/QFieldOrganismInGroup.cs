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
    public class QFieldOrganismInGroup : QField
	{
        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="initValue">The initial value; use string.Empty for none</param>
        public QFieldOrganismInGroup(string initValue) : base(initValue) { }

        public override string SelectSQL
        {
            get { return "isnull(OGgrp{0}.scientific_name + isnull(nullif(' (' + OGgrp{0}.common_name + ')', ' ()'), ''), OGgrp{0}.common_name) AS [Organism # Group]"; }
        }
		public override string[] FromTables
        {
            get { return new string[] { "organism_groups OG{0}", "organism_groups OGgrp{0}" }; }
        }
        public override string WhereValueClause(string val)
        {
            if(val.Contains(" (") && val.Contains(")"))
            {
                string valScientific = val.Substring(0, val.LastIndexOf(" ("));
                string valCommon = val.Substring(val.LastIndexOf(" (")).TrimStart(' ', '(').TrimEnd(')');
                //return "(OGgrp{0}.common_name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + valCommon + "%") + " OR OGgrp{0}.scientific_name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + valScientific + "%") + ")";
                return "(OGgrp{0}.common_name = " + DBWrapper.PreprocessSqlArgValue(valCommon) + " OR OGgrp{0}.scientific_name = " + DBWrapper.PreprocessSqlArgValue(valScientific) + ")";
            }
            else
            {
                //return "(OGgrp{0}.common_name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + val + "%") + " OR OGgrp{0}.scientific_name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + val + "%") + ")";
                return "(OGgrp{0}.common_name = " + DBWrapper.PreprocessSqlArgValue(val) + " OR OGgrp{0}.scientific_name = " + DBWrapper.PreprocessSqlArgValue(val) + ")";
            }
        }
        public override string[] WhereAndClauses()
        {
            return new string[] { "OGgrp{0}.is_organism = '0'",
                                  "OG{0}.is_organism = '1'",
                                  "OG{0}.nodeLabel LIKE OGgrp{0}.nodeLabel + '%'" };
        }

		public override int SearchCode { get { return 3; } }
        public override Dictionary<string, string> SearchFor(string search)
		{
            Dictionary<string, string> results = new Dictionary<string, string>();
            /*foreach (ServerOrganismGroup sog in ServerOrganismGroup.FindByCommonName(search, SearchMethod.Contains))
            {
                if (!results.ContainsValue(sog.Name.ToString()))
                    results.Add(sog.ID.ToString(), sog.Name);
            }*/
            foreach(ServerOrganismGroup sog in ServerOrganismGroup.AllOrganismsGroup())
                if(sog.Name.Contains(search))
                    results.Add(sog.ID.ToString(), sog.Name);
            return results;
		}
	}
}