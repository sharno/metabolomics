using System;
using System.Collections;
using System.Web.UI;
using PathwaysLib.QueryObjects;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.QueryObjects
{
	/// <summary>
	/// A query field for reversible processes
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QFieldProcessReversible : QField
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="initValue">The initial value; use string.Empty for none</param>
		public QFieldProcessReversible( string initValue ) : base( initValue ) {}

		public override string SelectSQL
        {
            get { return "PR{0}.reversible AS [Process # Reversible]"; }
        }
		public override string[] FromTables
        {
            get { return new string[] { "processes PR{0}" }; }
        }
        public override string WhereValueClause(string val)
        {
            return "PR{0}.reversible = " + DBWrapper.PreprocessSqlArgValue(val);
        }
        //public override string[] WhereSQL( string val )
        //{
        //    return new string[] { "(PR{0}.reversible = " + DBWrapper.PreprocessSqlArgValue(Int32.Parse(val)) + ")" };
        //}

		protected override ArrayList Populate()
		{
			ArrayList items = new ArrayList();
			items.Add( new DictionaryEntry( "1", "Yes" ) );
			items.Add( new DictionaryEntry( "0", "No" ) );
			return items;
		}
	}
}

			