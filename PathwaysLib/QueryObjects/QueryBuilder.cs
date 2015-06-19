using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using PathwaysLib.QueryObjects;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.Utilities
{
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
	public class QueryBuilder
	{
		//private static int counter = 0;
        private QIdCounter counter;
		private int id = -1;
		private string sql = string.Empty;
		private ArrayList selects = new ArrayList();
		private ArrayList froms = new ArrayList();
		private ArrayList wheres = new ArrayList();
		private ArrayList keys = new ArrayList();

		//public static int UniqueCounter { get { return counter.increment().Value; } }

		public string NodeID { get { if( id == -1 ) id = counter.increment().Value; return id.ToString(); } }
		public string SQL { get { return sql == string.Empty ? GenerateSQL() : sql; } }

		public ArrayList Selects { get { return selects; } }
		public ArrayList Froms { get { return froms; } }
		public ArrayList Wheres { get { return wheres; } }
		public ArrayList Keys { get { return keys; } }

		private void addGeneric( string val, ref ArrayList list )
		{
			val = string.Format( val, NodeID );
			if( !list.Contains( val ) ) list.Add( val );
		}

		private void addGeneric( string[] vals, ref ArrayList list )
		{
			foreach( string val in vals ) addGeneric( val, ref list );
		}

		public void AddSelect( string val ) { addGeneric( val, ref selects ); }
		public void AddFrom( string[] vals ) { addGeneric( vals, ref froms ); }
		public void AddWhere( string val ) { addGeneric( val, ref wheres ); }
		
		/// <summary>
		/// Default constructor; not all that useful
		/// </summary>
		public QueryBuilder() {}

        public QueryBuilder( QIdCounter counter ) { this.counter = counter; }

		/// <summary>
		/// Generates the SQL for this QueryBuilder object
		/// </summary>
		/// <returns>The SQL this QueryBuilder object represents</returns>
		public string GenerateSQL()
		{
			string s = string.Empty;

			if( selects.Count == 0 ) return s;
			s += "SELECT DISTINCT " + string.Join( ", ", (string[])selects.ToArray( typeof( string ) ) ) +
				" FROM " + string.Join( ", ", (string[])froms.ToArray( typeof( string ) ) );
			if( wheres.Count > 0 ) s +=
				" WHERE " + string.Join( " AND ", (string[])wheres.ToArray( typeof( string ) ) );
			sql = s;

			return s;
		}

		/// <summary>
		/// Merges this QueryBuilder object with another
		/// </summary>
		/// <param name="foreignQuery">The QueryBuilder object with which to merge</param>
		public void MergeWith( QueryBuilder foreignQuery )
		{
			foreach( string select in foreignQuery.Selects ) AddSelect( select );
			AddFrom( (string[])foreignQuery.Froms.ToArray( typeof( string ) ) );
			foreach( string where in foreignQuery.Wheres ) AddWhere( where );
		}

		/// <summary>
		/// Generate an HTML-ready display control of this QueryBuilder's SQL results
		/// </summary>
		/// <returns>A web control with the query results</returns>
		public Control GenerateTable()
		{
			if( SQL == string.Empty )
			{
				Panel res = new Panel();
				res.CssClass = "whitebg";
				Literal msg = new Literal();
				msg.Text = "Please select at least one output value.";
				res.Controls.Add( msg );
				return res;
			}

			SqlCommand command = DBWrapper.BuildCommand( SQL );
			DataSet[] ds = new DataSet[0];
			DataTable resTable = new DataTable();
			DataGrid resGrid = new DataGrid();

			if( DBWrapper.LoadMultiple( out ds, ref command ) > 0 )
			{
				// TODO: Link the output results (XML client-side?)

                // Postprocessing for IDs
                for (int c = 0; c < ds[0].Tables[0].Columns.Count - 1; c++)
                {
                    if (ds[0].Tables[0].Columns[c + 1].ColumnName.StartsWith("ID"))
                    {
                        for (int i = 0; i < ds.Length; i++)
                        {
                            ds[i].Tables[0].Rows[0][c] =
                                String.Format("<a href=\"LinkForwarder.aspx?rid={0}&amp;rtype={1}\">{2}</a>",
                                    ds[i].Tables[0].Rows[0][c + 1].ToString(),
                                    ds[0].Tables[0].Columns[c + 1].ColumnName.Substring(2, 3).TrimEnd(" ".ToCharArray()),
                                    ds[i].Tables[0].Rows[0][c].ToString());
                            //ds[i].Tables[0].Columns.RemoveAt(c + 1);
                        }
                        //ds[0].Tables[0].Columns.RemoveAt(c + 1);
                    }
                }

                // Create the correct number of columns and add their headers
                for (int r = 0; r < ds[0].Tables[0].Rows[0].ItemArray.Length; r++)
                    if(!ds[0].Tables[0].Columns[r].ColumnName.StartsWith("ID"))
                        resTable.Columns.Add(ds[0].Tables[0].Columns[r].ColumnName, typeof(string));
				
				// Fill the rest of the table
				for( int i = 0; i < ds.Length; i++ )
				{
					DataRow row = resTable.NewRow();
                    int offset = 0;
                    //for (int s = 0; s < resTable.Columns.Count; s++)
                    for(int s = 0; s < ds[0].Tables[0].Rows[0].ItemArray.Length; s++)
                    {
                        if (ds[0].Tables[0].Columns[s].ColumnName.StartsWith("ID"))
                        {
                            offset++;
                        }
                        else
                        {
                            row[s - offset] = ds[i].Tables[0].Rows[0][s];
                        }
                    }
                    //for (int s = 0; s < resTable.Columns.Count; s++) row[s] = "<a href='www.google.com'>" + ds[i].Tables[0].Rows[0][s].ToString() + "</a>";
					resTable.Rows.Add( row );
				}

				// Bind to an HTML control and send it back
				resGrid.DataSource = resTable;
				resGrid.DataBind();
				resGrid.CssClass = "datagridbase";
				resGrid.CellPadding = 4;
				resGrid.HeaderStyle.CssClass = "datagridheader";
				resGrid.ItemStyle.CssClass = "datagriditem";
				return resGrid;
			}
			else
			{
				Panel res = new Panel();
				res.CssClass = "whitebg";
				Literal msg = new Literal();
				msg.Text = "The query did not return any results.";
				res.Controls.Add( msg );
				return res;
			}
		}
	}
}