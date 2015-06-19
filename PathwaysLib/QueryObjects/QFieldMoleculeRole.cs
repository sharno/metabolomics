using System;
using System.Collections;
using System.Web.UI;
using PathwaysLib.QueryObjects;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.QueryObjects
{
	/// <summary>
	/// A query field for molecule roles
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QFieldMoleculeRole : QField
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="initValue">The initial value; use string.Empty for none</param>
		public QFieldMoleculeRole( string initValue ) : base( initValue ) {}

		public override string SelectSQL
        {
            get { return "PER{0}.name AS [Molecule # Role]"; }
        }
        public override string[] FromTables
        {
            get { return new string[] { "molecular_entities ME{0}",
                                        "process_entities PE{0}",
                                        "process_entity_roles PER{0}" }; }
        }
        //public override string[] FromTables { get { return new string[]
        //    { "molecular_entities ME{0}", "process_entities PE{0}", "process_entity_roles PER{0}" }; } }
        public override string WhereValueClause(string val)
        {
            return "PER{0}.name = " + DBWrapper.PreprocessSqlArgValue(val);
        }
        public override string[] WhereAndClauses()
        {
            return new string[] { "ME{0}.id = PE{0}.entity_id",
                                  "PE{0}.role_id = PER{0}.role_id" };
        }
        //public override string[] WhereSQL( string val )
        //{
        //    return new string[] { "(" + String.Join( " AND ",
        //                                             new string[] { "ME{0}.name = " + DBWrapper.PreprocessSqlArgValue(val),
        //                                                            "ME{0}.id = PE{0}.entity_id",
        //                                                            "PE{0}.role_id = PER{0}.role_id" } ) + ")" };
        //}

		protected override ArrayList Populate()
		{
			ArrayList items = new ArrayList();
			items.Add( new DictionaryEntry( "activator", "Activator" ) );
			items.Add( new DictionaryEntry( "cofactor in", "Cofactor in" ) );
			items.Add( new DictionaryEntry( "cofactor out", "Cofactor out" ) );
			items.Add( new DictionaryEntry( "inhibitor", "Inhibitor" ) );
			items.Add( new DictionaryEntry( "product", "Product" ) );
			items.Add( new DictionaryEntry( "regulator", "Regulator" ) );
			items.Add( new DictionaryEntry( "substrate", "Substrate" ) );
			return items;
		}
	}
}