using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using AQILib.Sql;
using System.Data;

namespace AQILib.Gui
{
    public class SqlDatasetRenderer : IQueryRenderer
    {
        private IAQIUtil _util;

        private SqlDatasetRenderer()
        { }

        public SqlDatasetRenderer(IAQIUtil util)
        {
            this._util = util;
        }

        public IGuiComponent Render(QNode node, IQueryResults results, IGuiData data, out IGuiData dataOut)
        {
            SqlDatasetQueryResults sqlResults = (SqlDatasetQueryResults) results;
            dataOut = null;

            // Does the result have output values (columns selected for output)?
            if(!sqlResults.HasOutputs)
                return new GuiDataTable(CreateLiteral("Please select at least one output value."), sqlResults.SqlQuery, sqlResults.SqlQueryNicelyFormatted);

            // Did the query return any results?
            if(sqlResults.ResultsCount <= 0)
                return new GuiDataTable(CreateLiteral("The query did not return any results."), sqlResults.SqlQuery, sqlResults.SqlQueryNicelyFormatted);

            // Postprocessing for IDs
            DataTable queryResults = sqlResults.Dt;
            DataTable dataTable = new DataTable();
            for(int c = 0; c < queryResults.Columns.Count - 1; c++)
            {
                if(queryResults.Columns[c + 1].ColumnName.StartsWith("ID"))
                {
                    for(int i = 0; i < queryResults.Rows.Count; i++)
                    {
                        queryResults.Rows[i][c] =
                            String.Format(_util.AQIForwardLink, //"<a href=\"LinkForwarder.aspx?rid={0}&amp;rtype={1}\">{2}</a>",
                                queryResults.Rows[i][c + 1].ToString(),
                                _util.ColumnNameToTypeCode(queryResults.Columns[c + 1].ColumnName), //queryResults.Columns[c + 1].ColumnName.Substring(2, 3).TrimEnd(" ".ToCharArray()),
                                queryResults.Rows[i][c].ToString());
                    }
                }
            }

            // Create the correct number of columns and add their headers
            for(int r = 0; r < queryResults.Columns.Count; r++)
            {
                if(!queryResults.Columns[r].ColumnName.StartsWith("ID"))
                    dataTable.Columns.Add(queryResults.Columns[r].ColumnName, typeof(string));
            }

            // Fill the rest of the table
            for(int i = 0; i < queryResults.Rows.Count; i++)
            {
                DataRow row = dataTable.NewRow();
                int offset = 0;
                for(int s = 0; s < queryResults.Columns.Count; s++)
                {
                    if(queryResults.Columns[s].ColumnName.StartsWith("ID"))
                        offset++;
                    else
                        row[s - offset] = queryResults.Rows[i][s];
                }
                dataTable.Rows.Add(row);
            }

            // Bind to an HTML control and send it back
            DataGrid resGrid = new DataGrid();
            resGrid.DataSource = dataTable;
            resGrid.DataBind();
            resGrid.CssClass = "datagridbase";
            resGrid.CellPadding = 4;
            resGrid.HeaderStyle.CssClass = "datagridheader";
            resGrid.ItemStyle.CssClass = "datagriditem";

            return new GuiDataTable(resGrid, sqlResults.SqlQuery, sqlResults.SqlQueryNicelyFormatted);
        }

        private Panel CreateLiteral(string message)
        {
            Panel res = new Panel();
            res.CssClass = "whitebg";
            Literal msg = new Literal();
            msg.Text = message;
            res.Controls.Add(msg);
            return res;
        }
    }
}