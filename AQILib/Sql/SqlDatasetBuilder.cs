using AQILib;
using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib.Sql
{
    public class SqlDatasetBuilder
    {
        private int _nodeId;
        private int _typeId;

        private UniqueList<string> _selects;
        //private UniqueList<string> _froms;
        private Dictionary<int, UniqueList<string>> _fromTables;
        private Dictionary<int, SqlTablesManager> _tblMgrs;
        private Dictionary<int, UniqueList<string>> _joinFromTables;
        private Dictionary<int, SqlTablesManager> _joinTblMgrs;
        private UniqueList<string> _whereFields;
        private UniqueList<string> _whereTables;
        private UniqueList<string> _whereJoins;
        private UniqueList<string> _whereChildren;

        public int NodeId
        {
            get { return _nodeId; }
        }

        public int TypeId
        {
            get { return _typeId; }
        }

        public UniqueList<string> FromTables
        {
            get { return _fromTables[_nodeId]; }
            set { _fromTables[_nodeId] = value; }
        }

        public UniqueList<string> Selects
        {
            get
            {
                return _selects;
            }
        }

        public UniqueList<string> Froms
        {
            get
            {
                UniqueList<string> retVal = new UniqueList<string>();

                foreach(KeyValuePair<int, UniqueList<string>> kvp in _fromTables)
                {
                    int nodeId = kvp.Key;
                    UniqueList<string> fromTbls = kvp.Value;

                    if(nodeId == _nodeId)
                    {
                        foreach(string tbl in fromTbls)
                            retVal.Add(AddIds(_tblMgrs[nodeId].SqlTables[tbl], nodeId));
                    }
                    else
                    {
                        UniqueList<string> joinFromTbls = new UniqueList<string>();
                        if(_joinFromTables.ContainsKey(nodeId))
                            joinFromTbls = _joinFromTables[nodeId];

                        foreach(string tbl in joinFromTbls)
                            retVal.Add(AddIds(_joinTblMgrs[nodeId].SqlTables[tbl], nodeId));

                        foreach(string tbl in fromTbls)
                            retVal.Add(tbl);
                    }
                }

                return retVal;
            }
        }

        public UniqueList<string> Wheres
        {
            get
            {
                UniqueList<string> retVal = new UniqueList<string>();

                foreach(string cond in _whereFields)
                    retVal.Add(cond);
                foreach(string cond in _whereTables)
                    retVal.Add(cond);
                foreach(string cond in _whereJoins)
                    retVal.Add(cond);
                foreach(string cond in _whereChildren)
                    retVal.Add(cond);

                return retVal;
            }
        }

        private SqlDatasetBuilder() { }

        public SqlDatasetBuilder(int nodeId, int typeId, SqlTablesManager tblMgr)
        {
            _nodeId = nodeId;
            _typeId = typeId;

            _selects = new UniqueList<string>();
            _fromTables = new Dictionary<int, UniqueList<string>>();
            _tblMgrs = new Dictionary<int, SqlTablesManager>();
            _joinFromTables = new Dictionary<int, UniqueList<string>>();
            _joinTblMgrs = new Dictionary<int, SqlTablesManager>();
            _whereFields = new UniqueList<string>();
            _whereTables = new UniqueList<string>();
            _whereJoins = new UniqueList<string>();
            _whereChildren = new UniqueList<string>();

            _fromTables[_nodeId] = new UniqueList<string>();
            SetTblMgr(_nodeId, tblMgr);
        }

        private string AddIds(string sql)
        {
            return String.Format(sql, _nodeId, _typeId);
        }

        private string AddIds(string sql, int childId)
        {
            return String.Format(sql, childId, "{1}", "{2}", _nodeId, childId);
        }

        public void AddSelect(string sql, List<string> tables)
        {
            _selects.Add(AddIds(sql));
            AddFromRange(tables);
        }

        private void AddFrom(string tbl)
        {
            AddFrom(tbl, _nodeId);
        }

        public void AddFrom(string tbl, int nodeId)
        {
            if(!_fromTables.ContainsKey(nodeId))
                _fromTables[nodeId] = new UniqueList<string>();

            _fromTables[nodeId].Add(tbl);

            if(_tblMgrs[nodeId].SqlTableConditions.ContainsKey(tbl))
                AddWhere(_tblMgrs[nodeId].SqlTableConditions[tbl]);

            if(nodeId == _nodeId && FromTables.Count > 1)
            {
                foreach(string joinCondition in _tblMgrs[_nodeId].SqlJoinConditions[FromTables[FromTables.Count - 2]][FromTables[FromTables.Count - 1]])
                    _whereTables.Add(AddIds(joinCondition));
                foreach(string joinTbl in _tblMgrs[_nodeId].SqlJoinConditionsTables[FromTables[FromTables.Count - 2]][FromTables[FromTables.Count - 1]])
                    if(!FromTables.Contains(joinTbl)) // Prevents an infinite loop!
                        AddFrom(joinTbl, nodeId);
            }
        }

        private void AddFromRange(IEnumerable<string> tables)
        {
            AddFromRange(tables, _nodeId);
        }

        private void AddFromRange(IEnumerable<string> tables, int nodeId)
        {
            foreach(string tbl in tables)
                AddFrom(tbl, nodeId);
        }

        public void AddWhere(QField fld, SqlDatasetQuerier.SqlFieldConditionDelegate del, List<string> tables)
        {
            List<Dictionary<string, string>> groupedValues = new List<Dictionary<string, string>>();

            // Set up empty list/dictionary
            for(int i = 0; i < fld.ValuesetCount; i++)
                groupedValues.Add(new Dictionary<string, string>());

            // Populate the list/dictionary with values
            foreach(List<QInput> l in fld.Inputs.Values)
                for(int i = 0; i < l.Count; i++)
                    groupedValues[i].Add(l[i].Name, l[i].Value);

            // Now that the values are grouped properly, find the connector and perform the proper connections
            if(fld.Connector.Equals("or"))
            {
                UniqueList<string> whereFieldsNotORed = new UniqueList<string>();
                foreach(Dictionary<string, string> data in groupedValues)
                {
                    // Bug Fix: If the value passed in to this QInput, stored in data, contains one or more braces
                    //          (either { or }), then the String.Format function (in AddIds) can get confused when
                    //          called. Therefore, the way around this is to format a single brace as a double brace
                    //          so that when it is passed into the String.Format function, it is turned back into a
                    //          single brace. --- Steve Mayes, 7/3/2007
                    //
                    // Old Code: whereFieldsNotORed.Add(AddIds(del(data)));
                    // New Code below
                    Dictionary<string, string> fixedData = new Dictionary<string, string>();
                    foreach(KeyValuePair<string, string> kvp in data)
                        fixedData[kvp.Key] = kvp.Value.Replace("{", "{{").Replace("}", "}}");
                    whereFieldsNotORed.Add(AddIds(del(fixedData)));
                }
                _whereFields.Add(String.Format("({0})", String.Join(" OR ", whereFieldsNotORed.ToArray())));

                AddFromRange(tables);
            }
            else
            {
                throw new DataValidationException("Invalid connector (" + fld.Connector + ") found while querying");
            }
        }

        public void AddWhere(string sql)
        {
            _whereTables.Add(AddIds(sql));
        }

        public void AddWhere(string sql, int childId)
        {
            _whereTables.Add(AddIds(sql, childId));
        }

        public void SetTblMgr(int nodeId, SqlTablesManager tblMgr)
        {
            _tblMgrs[nodeId] = tblMgr;
        }

        public void Join(SqlDatasetBuilder childBuilder, SqlTablesManager tblMgr)
        {
            // Find the 'source' table in the parent and the 'destination' table in the child
            // Join between these two tables!
            string sourceTable = "";
            string destinationTable = "";
            UniqueList<string> joinTables = new UniqueList<string>(tblMgr.SqlTables.Keys);
            foreach(string tbl in joinTables)
            {
                if(this.FromTables.Contains(tbl))
                    sourceTable = tbl;
                if(childBuilder.FromTables.Contains(tbl) && destinationTable.Equals(""))
                    destinationTable = tbl;
            }
            if(sourceTable.Equals(""))
            {
                sourceTable = joinTables[0];
                this.FromTables.Add(sourceTable);
            }
            if(destinationTable.Equals(""))
            {
                destinationTable = joinTables[joinTables.Count - 1];
                childBuilder.FromTables.Add(destinationTable);
            }

            _joinTblMgrs[childBuilder.NodeId] = tblMgr;
            _joinFromTables[childBuilder.NodeId] = new UniqueList<string>();
            foreach(string tbl in tblMgr.SqlJoinConditionsTables[sourceTable][destinationTable])
            {
                _joinFromTables[childBuilder.NodeId].Add(tbl);
                if(tblMgr.SqlTableConditions.ContainsKey(tbl))
                    AddWhere(tblMgr.SqlTableConditions[tbl], childBuilder.NodeId);
            }
            foreach(string cond in tblMgr.SqlJoinConditions[sourceTable][destinationTable])
                _whereJoins.Add(AddIds(cond, childBuilder.NodeId));

            // Tell this builder about the child's select, from, and where clauses
            _selects.AddRange(childBuilder.Selects);
            _fromTables[childBuilder.NodeId] = childBuilder.Froms;
            _tblMgrs[childBuilder.NodeId] = tblMgr;
            _whereChildren.AddRange(childBuilder.Wheres);
        }
    }
}