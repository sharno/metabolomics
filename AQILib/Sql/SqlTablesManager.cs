using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib.Sql
{
    /// <summary>
    /// Track the tables for a particular AQI node
    /// </summary>
    public class SqlTablesManager
    {
        private Dictionary<string, string> _sqlTables; // <"Alias", "TableName">
        private Dictionary<string, string> _sqlTableConditions; // <"Tbl", "Tbl.Flag = 1">
        private Dictionary<string, Dictionary<string, List<string>>> _sqlJoinConditions; // <"Tbl1", <"Tbl2", { "Tbl1.ID = Tbl3.ID", "Tbl3.ID = Tbl2.ID" }>>
        private Dictionary<string, Dictionary<string, List<string>>> _sqlJoinConditionsTables; // <"Tbl1", <"Tbl2", { "Tbl1", "Tbl2", "Tbl3" }>>

        public Dictionary<string, string> SqlTables
        {
            get { return _sqlTables; }
        }

        public Dictionary<string, string> SqlTableConditions
        {
            get { return _sqlTableConditions; }
        }

        public Dictionary<string, Dictionary<string, List<string>>> SqlJoinConditions
        {
            get { return _sqlJoinConditions; }
        }

        public Dictionary<string, Dictionary<string, List<string>>> SqlJoinConditionsTables
        {
            get { return _sqlJoinConditionsTables; }
        }

        public SqlTablesManager()
        {
            _sqlTables = new Dictionary<string, string>();
            _sqlTableConditions = new Dictionary<string, string>();
            _sqlJoinConditions = new Dictionary<string, Dictionary<string, List<string>>>();
            _sqlJoinConditionsTables = new Dictionary<string, Dictionary<string, List<string>>>();
        }

        public void AddSqlTable(string tableAlias, string tableName)
        {
            _sqlTables[tableAlias] = tableName;
            _sqlJoinConditions[tableAlias] = new Dictionary<string, List<string>>();
            _sqlJoinConditionsTables[tableAlias] = new Dictionary<string, List<string>>();
        }

        public void AddSqlTableCondition(string tableAlias, string tableCondition)
        {
            _sqlTableConditions[tableAlias] = tableCondition;
        }

        public void AddSqlJoinCondition(string tableAlias1, string tableAlias2, string joinCondition)
        {
            List<string> joinConditions = new List<string>();
            joinConditions.Add(joinCondition);
            List<string> conditionTables = new List<string>(new string[] { tableAlias1, tableAlias2 });

            AddSqlJoinCondition(tableAlias1, tableAlias2, joinConditions, conditionTables);

            foreach(KeyValuePair<string, List<string>> kvp in _sqlJoinConditions[tableAlias1])
            {
                if(!_sqlJoinConditions[kvp.Key].ContainsKey(tableAlias2) && kvp.Key != tableAlias2)
                {
                    UniqueList<string> newJoinConditions = new UniqueList<string>();
                    UniqueList<string> newConditionTables = new UniqueList<string>();

                    foreach(string jCond in _sqlJoinConditions[kvp.Key][tableAlias1])
                        newJoinConditions.Add(jCond);
                    foreach(string jCond in joinConditions)
                        newJoinConditions.Add(jCond);

                    foreach(string cTbl in _sqlJoinConditionsTables[kvp.Key][tableAlias1])
                        newConditionTables.Add(cTbl);
                    foreach(string cTbl in conditionTables)
                        newConditionTables.Add(cTbl);

                    AddSqlJoinCondition(kvp.Key, tableAlias2, newJoinConditions, newConditionTables);
                }
            }

            foreach(KeyValuePair<string, List<string>> kvp in _sqlJoinConditions[tableAlias2])
            {
                if(!_sqlJoinConditions[tableAlias1].ContainsKey(kvp.Key) && tableAlias1 != kvp.Key)
                {
                    UniqueList<string> newJoinConditions = new UniqueList<string>();
                    UniqueList<string> newConditionTables = new UniqueList<string>();

                    foreach(string jCond in joinConditions)
                        newJoinConditions.Add(jCond);
                    foreach(string jCond in _sqlJoinConditions[tableAlias2][kvp.Key])
                        newJoinConditions.Add(jCond);

                    foreach(string cTbl in conditionTables)
                        newConditionTables.Add(cTbl);
                    foreach(string cTbl in _sqlJoinConditionsTables[tableAlias2][kvp.Key])
                        newConditionTables.Add(cTbl);

                    AddSqlJoinCondition(tableAlias1, kvp.Key, newJoinConditions, newConditionTables);
                }
            }
        }

        private void AddSqlJoinCondition(string tableAlias1, string tableAlias2, List<string> joinConditions, List<string> conditionTables)
        {
            _sqlJoinConditions[tableAlias1][tableAlias2] = joinConditions;
            _sqlJoinConditionsTables[tableAlias1][tableAlias2] = new List<string>(conditionTables);
            _sqlJoinConditions[tableAlias2][tableAlias1] = joinConditions;
            _sqlJoinConditionsTables[tableAlias2][tableAlias1] = new List<string>(conditionTables);
        }
    }
}