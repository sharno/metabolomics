using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace AQILib.Gui
{
    public class GuiDataTable : IGuiComponent
    {
        private Control _table;
        private string _sqlQuery;
        private string _sqlQueryNicelyFormatted;

        public Control Table
        {
            get { return _table; }
        }

        public string SqlQuery
        {
            get { return _sqlQuery; }
        }

        public string SqlQueryNicelyFormatted
        {
            get { return _sqlQueryNicelyFormatted; }
        }

        public GuiDataTable()
        { }

        public GuiDataTable(Control table, string sqlQuery, string sqlQueryNicelyFormatted)
        {
            _table = table;
            _sqlQuery = sqlQuery;
            _sqlQueryNicelyFormatted = sqlQueryNicelyFormatted;
        }
    }
}