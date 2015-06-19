using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using PathwaysLib.QueryObjects;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.QueryObjects
{
	/// <summary>
	/// A query field for molecular entity names
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QFieldMoleculeName : QField
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="initValue">The initial value; use string.Empty for none</param>
		public QFieldMoleculeName( string initValue ) : base( initValue ) {}

        public override string SelectSQL
        {
            get { return "ME{0}.name AS [Molecule # Name], ME{0}.id AS [IDme #_{0}]"; }
        }
        public override string[] FromTables
        {
            get { return new string[] { "molecular_entities ME{0}" }; }
        }
        public override string WhereValueClause(string val)
        {
            //return "ME{0}.name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + val + "%");
            return "ME{0}.name = " + DBWrapper.PreprocessSqlArgValue(val);
        }
        //public override string[] WhereAndClauses()
        //{
        //    return new string[] { "ME{0}.id = PE{0}.entity_id",
        //                          "PE{0}.role_id = PER{0}.role_id" };
        //}
        //public override string[] WhereSQL( string val )
        //{
        //    return new string[] { "(" + String.Join( " AND ",
        //                                             new string[] { "ME{0}.name LIKE " + DBWrapper.PreprocessSqlArgValue("%" + val + "%"),
        //                                                            "ME{0}.id = PE{0}.entity_id",
        //                                                            "PE{0}.role_id = PER{0}.role_id" } ) + ")" };
        //}

		public override int SearchCode { get { return 2; } }
        public override Dictionary<string, string> SearchFor(string search)
		{
            Dictionary<string, string> results = new Dictionary<string, string>();
            foreach (ServerMolecularEntity sme in ServerMolecularEntity.FindMolecularEntities(search, SearchMethod.Contains, 30))
            {
                if (!results.ContainsValue(sme.Name.ToString()))
                    results.Add(sme.ID.ToString(), sme.Name);
            }
            return results;
		}
	}
}

			