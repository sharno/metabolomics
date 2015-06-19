using AQILib;
//using PathwaysLib.ServerObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AQILib.Sql
{
    public class SqlDatasetQuerier : Querier
    {
        public delegate string SqlFieldSelectDelegate();
        public delegate string SqlFieldConditionDelegate(Dictionary<string, string> data);

        // {0} is the nodeID, {1} is the typeID, {2} is the field value (FieldConditions)
        // Relationships: {3} is the parent node ID, {4} is the child node ID
        private SqlTablesManager _tblMgr;
        private Dictionary<string, SqlTablesManager> _relationshipsTblMgr;

        private Dictionary<string, SqlFieldSelectDelegate> _sqlFieldSelectsDelegates; // <field_type, "Tbl.Property">
        private Dictionary<string, List<string>> _sqlFieldSelectsTables; // <field_type, {'tbl1alias', 'tbl2'}>
        private Dictionary<string, SqlFieldConditionDelegate> _sqlFieldConditionsDelegates; // <field_type, "Tbl.Property = '{2}'">
        private Dictionary<string, List<string>> _sqlFieldConditionsTables; // <field_type, {'tbl1alias', 'tbl2'}>

        private IAQIUtil _util;

        public SqlTablesManager TableManager
        {
            get { return _tblMgr; }
        }

        public Dictionary<string, SqlTablesManager> RelationshipsTableManager
        {
            get { return _relationshipsTblMgr; }
        }

        public SqlDatasetQuerier(IAQIUtil util)
            : base()
        {
            _tblMgr = new SqlTablesManager();
            _relationshipsTblMgr = new Dictionary<string, SqlTablesManager>();
            _sqlFieldSelectsDelegates = new Dictionary<string, SqlFieldSelectDelegate>();
            _sqlFieldSelectsTables = new Dictionary<string, List<string>>();
            _sqlFieldConditionsDelegates = new Dictionary<string, SqlFieldConditionDelegate>();
            _sqlFieldConditionsTables = new Dictionary<string, List<string>>();

            this._util = util;
        }

        public void AddSqlTable(string tableAlias, string tableName)
        {
            _tblMgr.AddSqlTable(tableAlias, tableName);
        }

        public void AddSqlTableCondition(string tableAlias, string tableCondition)
        {
            _tblMgr.AddSqlTableCondition(tableAlias, tableCondition);
        }

        public void AddSqlJoinCondition(string tableAlias1, string tableAlias2, string joinCondition)
        {
            _tblMgr.AddSqlJoinCondition(tableAlias1, tableAlias2, joinCondition);
        }

        public void AddSqlRelationshipTable(string childId, string tableAlias, string tableName)
        {
            if(!_relationshipsTblMgr.ContainsKey(childId))
                _relationshipsTblMgr[childId] = new SqlTablesManager();

            _relationshipsTblMgr[childId].AddSqlTable(tableAlias, tableName);
        }

        public void AddSqlRelationshipTableCondition(string childId, string tableAlias, string tableCondition)
        {
            if(!_relationshipsTblMgr.ContainsKey(childId))
                _relationshipsTblMgr[childId] = new SqlTablesManager();

            _relationshipsTblMgr[childId].AddSqlTableCondition(tableAlias, tableCondition);
        }

        public void AddSqlRelationshipJoinCondition(string childId, string tableAlias1, string tableAlias2, string joinCondition)
        {
            if(!_relationshipsTblMgr.ContainsKey(childId))
                _relationshipsTblMgr[childId] = new SqlTablesManager();

            _relationshipsTblMgr[childId].AddSqlJoinCondition(tableAlias1, tableAlias2, joinCondition);
        }

        public void AddSqlFieldSelect(string fieldTypeId, string fieldSelectString, string[] selectTables)
        {
            AddSqlFieldSelect(fieldTypeId,
                              delegate()
                              {
                                  return fieldSelectString;
                              },
                              selectTables);
        }

        public void AddSqlFieldSelect(string fieldTypeId, SqlFieldSelectDelegate fieldSelectDelegate, string[] selectTables)
        {
            _sqlFieldSelectsDelegates[fieldTypeId] = fieldSelectDelegate;
            _sqlFieldSelectsTables[fieldTypeId] = new List<string>(selectTables);
            _sqlFieldSelectsTables[fieldTypeId].AddRange(selectTables);
        }

        public void AddSqlFieldCondition(string fieldTypeId, string fieldInputKey, string fieldConditionString, string[] conditionTables)
        {
            AddSqlFieldCondition(fieldTypeId,
                                 delegate(Dictionary<string, string> data)
                                 {
                                     return String.Format(fieldConditionString, "{0}", "{1}", _util.PreprocessSqlArgValue(data[fieldInputKey]));
                                 },
                                 conditionTables);
        }

        public void AddSqlFieldCondition(string fieldTypeId, SqlFieldConditionDelegate fieldConditionDelegate, string[] conditionTables)
        {
            _sqlFieldConditionsDelegates[fieldTypeId] = fieldConditionDelegate;
            _sqlFieldConditionsTables[fieldTypeId] = new List<string>(conditionTables);
        }

        private string AddIds(string sql, int nodeId, int typeId)
        {
            return String.Format(sql, nodeId, typeId);
        }

        private string AddRelIds(string sql, int parentId, int childId)
        {
            return String.Format(sql, "{0}", "{1}", "{2}", parentId, childId);
        }

        private SqlDatasetBuilder GetSqlData(QNode node, ref QNodeIdCounter counter)
        {
            SqlDatasetBuilder builder = new SqlDatasetBuilder(counter.NextNodeId(), node.NodeTypeId, _tblMgr); //counter.NextTypeId(node.NodeType), _tblMgr);

            // Loop through fields and perform the following two tasks on each field:
            //   1) If the field is to be displayed, add the select statement to the builder.
            //   2) If the field is to be queried, add the where statement to the builder.
            foreach(KeyValuePair<string, QField> kvp in node.Fields)
            {
                QField f = kvp.Value;

                if(f.Displayed)
                    builder.AddSelect(_sqlFieldSelectsDelegates[f.FieldTypeId](), _sqlFieldSelectsTables[f.FieldTypeId]);

                if(f.Queried)
                    builder.AddWhere(f, _sqlFieldConditionsDelegates[f.FieldTypeId], _sqlFieldConditionsTables[f.FieldTypeId]);
            }

            // Process the children
            foreach(QNode child in node.Children)
            {
                SqlDatasetBuilder childBuilder = ((SqlDatasetQuerier) child.Querier).GetSqlData(child, ref counter);
                builder.Join(childBuilder, _relationshipsTblMgr[child.NodeTypeName]);
            }

            return builder;
        }

        public override IQueryResults Query(QNode node)
        {
            QNodeIdCounter counter = new QNodeIdCounter();

            SqlDatasetBuilder b = GetSqlData(node, ref counter);

            UniqueList<string> sqlSelects = b.Selects;
            UniqueList<string> sqlFroms = b.Froms;
            UniqueList<string> sqlWheres = b.Wheres;

            string sqlQuery;
            string sqlQueryNicelyFormatted;

            if(sqlWheres.Count > 0)
            {
                sqlQuery = String.Format("SELECT DISTINCT {0} FROM {1} WHERE {2}",
                                         String.Join(", ", sqlSelects.ToArray()),
                                         String.Join(", ", sqlFroms.ToArray()),
                                         String.Join(" AND ", sqlWheres.ToArray()));
                sqlQueryNicelyFormatted = String.Format("SELECT DISTINCT\n       {0}\nFROM\n     {1}\nWHERE\n  {2}",
                                                        String.Join(",\n       ", sqlSelects.ToArray()),
                                                        String.Join(",\n     ", sqlFroms.ToArray()),
                                                        String.Join("\n  AND ", sqlWheres.ToArray()));
            }
            else
            {
                sqlQuery = String.Format("SELECT DISTINCT {0} FROM {1}",
                                         String.Join(", ", sqlSelects.ToArray()),
                                         String.Join(", ", sqlFroms.ToArray()));
                sqlQueryNicelyFormatted = String.Format("SELECT DISTINCT\n       {0}\nFROM\n     {1}",
                                                        String.Join(",\n       ", sqlSelects.ToArray()),
                                                        String.Join(",\n     ", sqlFroms.ToArray()));
            }

            SqlDatasetQueryResults results = new SqlDatasetQueryResults();
            results.HasOutputs = (sqlSelects.Count > 0);
            results.SqlQuery = sqlQuery;
            results.SqlQueryNicelyFormatted = sqlQueryNicelyFormatted;

            if(!results.HasOutputs)
            {
                results.Dt = null;
                results.ResultsCount = 0;
                return results;
            }

            DataTable queryResults;
            //DataTable dataTable = new DataTable();

            results.ResultsCount = _util.ExecuteSqlQuery(out queryResults, sqlQuery); 
            if(results.ResultsCount > 0)
            {
                results.Dt = queryResults;

                //// Postprocessing for IDs
                //for (int c = 0; c < queryResults.Columns.Count - 1; c++)
                //{
                //    if (queryResults.Columns[c + 1].ColumnName.StartsWith("ID"))
                //    {
                //        for (int i = 0; i < queryResults.Rows.Count; i++)
                //        {
                //            queryResults.Rows[i][c] =
                //                String.Format(_util.AQIForwardLink, //"<a href=\"LinkForwarder.aspx?rid={0}&amp;rtype={1}\">{2}</a>",
                //                    queryResults.Rows[i][c + 1].ToString(),
                //                    _util.ColumnNameToTypeCode(queryResults.Columns[c + 1].ColumnName), //queryResults.Columns[c + 1].ColumnName.Substring(2, 3).TrimEnd(" ".ToCharArray()),
                //                    queryResults.Rows[i][c].ToString());
                //        }
                //    }
                //}

                //// Create the correct number of columns and add their headers
                //for (int r = 0; r < queryResults.Columns.Count; r++)
                //{
                //    if (!queryResults.Columns[r].ColumnName.StartsWith("ID"))
                //        dataTable.Columns.Add(queryResults.Columns[r].ColumnName, typeof(string));
                //}

                //// Fill the rest of the table
                //for (int i = 0; i < queryResults.Rows.Count; i++)
                //{
                //    DataRow row = dataTable.NewRow();
                //    int offset = 0;
                //    for (int s = 0; s < queryResults.Columns.Count; s++)
                //    {
                //        if (queryResults.Columns[s].ColumnName.StartsWith("ID"))
                //            offset++;
                //        else
                //            row[s - offset] = queryResults.Rows[i][s];
                //    }
                //    dataTable.Rows.Add(row);
                //}

                //results.Dt = dataTable;
            }
            else
            {
                results.Dt = null;
            }

            return results;
        }
    }
}