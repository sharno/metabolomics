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
	public class QFieldMoleculeQuantity : QField
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="initValue">The initial value; use string.Empty for none</param>
		public QFieldMoleculeQuantity( string initValue ) : base( initValue ) {}

		public override string SelectSQL { get { return "PE{0}.quantity AS [Molecule # Quantity]"; } }
		public override string[] FromTables { get { return new string[]
			{ "molecular_entities ME", "process_entities PE", "process_entity_roles PER" }; } }
		public override string[] WhereSQL( string val )
		{
			return new string[] { "PE{0}.quantity = " + val, "ME{0}.id = PE{0}.entity_id",
				"PE{0}.role_id = PER{0}.role_id" };
		}

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