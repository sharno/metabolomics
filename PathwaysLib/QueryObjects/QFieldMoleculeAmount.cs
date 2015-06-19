using System;
using System.Collections;
using System.Web.UI;
using PathwaysLib.QueryObjects;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.QueryObjects
{
	/// <summary>
	/// A query field for molecule quantities
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QFieldMoleculeAmount : QField
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="initValue">The initial value; use string.Empty for none</param>
		public QFieldMoleculeAmount( string initValue ) : base( initValue ) {}

		public override string SelectSQL
        {
            get { return "PE{0}.quantity AS [Molecule # Quantity]"; }
        }
        public override string[] FromTables
        {
            get { return new string[] { "molecular_entities ME{0}",
                                        "process_entities PE{0}" }; }
        }
        //public override string[] FromTables { get { return new string[]
        //{ "molecular_entities ME", "process_entities PE", "process_entity_roles PER" }; } }
        public override string WhereValueClause(string val)
        {
            return "PE{0}.quantity = " + DBWrapper.PreprocessSqlArgValue(val);
        }
        public override string[] WhereAndClauses()
        {
            return new string[] { "ME{0}.id = PE{0}.entity_id" };
        }
        //public override string[] WhereSQL( string val )
        //{
        //    return new string[] { "(" + String.Join( " AND ",
        //                                             new string[] { "PE{0}.quantity = " + DBWrapper.PreprocessSqlArgValue(Int32.Parse(val)),
        //                                                            "ME{0}.id = PE{0}.entity_id",
        //                                                            "PE{0}.role_id = PER{0}.role_id" } ) + ")" };
        //}

		protected override ArrayList Populate()
		{
			ArrayList items = new ArrayList();
			items.Add( new DictionaryEntry( "1", "1" ) );
			items.Add( new DictionaryEntry( "2", "2" ) );
			items.Add( new DictionaryEntry( "3", "3" ) );
			items.Add( new DictionaryEntry( "6", "6" ) );
			return items;
		}
	}
}