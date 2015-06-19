using System;
using System.Collections;
using System.Web.UI;
using PathwaysLib.QueryObjects;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.QueryObjects
{
	/// <summary>
	/// A query field for molecule name types
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QFieldMoleculeNameType : QField
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="initValue">The initial value; use string.Empty for none</param>
		public QFieldMoleculeNameType( string initValue ) : base( initValue ) {}

		public override string SelectSQL
        {
            get { return "NT{0}.name AS [Molecule # Name Type]"; }
        }
        public override string[] FromTables
        {
            get { return new string[] { "molecular_entities ME{0}",
                                        "entity_name_lookups ENL{0}",
                                        "name_types NT{0}" }; }
        }
		public override string WhereValueClause( string val )
		{
            return "NT{0}.name = " + DBWrapper.PreprocessSqlArgValue(val);
            //return new string[] { "(" + String.Join( " AND ",
            //                                         new string[] { "NT{0}.name = " + DBWrapper.PreprocessSqlArgValue(val),
            //                                                        "ME{0}.id = ENL{0}.entity_id",
            //                                                        "ENL{0}.name_type_id = NT{0}.name_type_id",
            //                                                        "ME{0}.id = PE{0}.entity_id",
            //                                                        "PE{0}.role_id = PER{0}.role_id" } ) + ")" };
		}
        public override string[] WhereAndClauses()
        {
            return new string[] { "ME{0}.id = ENL{0}.entity_id",
                                  "ENL{0}.name_type_id = NT{0}.name_type_id" };
        }
        //public override string[] WhereAndClauses()
        //{
        //    return new string[] { "ME{0}.id = ENL{0}.entity_id",
        //                          "ENL{0}.name_type_id = NT{0}.name_type_id",
        //                          "ME{0}.id = PE{0}.entity_id",
        //                          "PE{0}.role_id = PER{0}.role_id" };
        //}

		protected override ArrayList Populate()
		{
			ArrayList items = new ArrayList();
			items.Add( new DictionaryEntry( "primary name", "Primary name" ) );
            items.Add( new DictionaryEntry( "common name", "Common name" ) );
			items.Add( new DictionaryEntry( "other name", "Other name" ) );
			items.Add( new DictionaryEntry( "gene symbol", "Gene symbol" ) );
			return items;
		}
	}
}