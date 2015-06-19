using System;
using System.Collections;
using System.Web.UI;
using PathwaysLib.QueryObjects;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.QueryObjects
{
	/// <summary>
	/// A query field for molecule types
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QFieldMoleculeType : QField
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="initValue">The initial value; use string.Empty for none</param>
		public QFieldMoleculeType( string initValue ) : base( initValue ) {}

		public override string SelectSQL
        {
            get { return "MET{0}.name AS [Molecule # Type]"; }
        }
		public override string[] FromTables
        {
            get { return new string[] { "molecular_entities ME{0}",
                                        "molecular_entity_types MET{0}" }; }
        }
        public override string WhereValueClause(string val)
        {
            return "MET{0}.name = " + DBWrapper.PreprocessSqlArgValue(val);
        }
        public override string[] WhereAndClauses()
        {
            return new string[] { "ME{0}.type_id = MET{0}.type_id" };
        }
        //public override string[] WhereSQL( string val )
        //{
        //    return new string[] { "(" + String.Join( " AND ",
        //                                             new string[] { "MET{0}.name = " + DBWrapper.PreprocessSqlArgValue(val),
        //                                                            "ME{0}.type_id = MET{0}.type_id",
        //                                                            "ME{0}.id = PE{0}.entity_id",
        //                                                            "PE{0}.role_id = PER{0}.role_id" } ) + ")" };
        //}

		protected override ArrayList Populate()
		{
			ArrayList items = new ArrayList();
            items.Add( new DictionaryEntry( "amino_acids", "Amino Acids" ) );
			items.Add( new DictionaryEntry( "basic_molecules", "Basic molecules" ) );
			items.Add( new DictionaryEntry( "genes", "Genes" ) );
			items.Add( new DictionaryEntry( "proteins", "Proteins" ) );
            items.Add( new DictionaryEntry( "rnas", "RNAs" ) );
			return items;
		}
	}
}