using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using AQILib.Sql;
using System.Data;

namespace AQILib
{
    /// <summary>
    /// A base class for queriers
    /// </summary>
    public abstract class Querier
    {
        public Querier() { }

        public abstract IQueryResults Query(QNode node);

        /// <summary>
        /// Generates a list of suggestions based on the entire state of the query and 
        /// showing only values that can return non-empty results.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="fieldId"></param>
        /// <param name="nodeType"></param>
        /// <param name="fieldTypeId"></param>
        /// <param name="inputType"></param>
        /// <param name="queryString"></param>
        /// <param name="treeXmlString"></param>
        /// <param name="topkMatches"></param>
        /// <returns></returns>
        public virtual Dictionary<string, string> GetInputValues(QNode node, string fieldId, string nodeType, string fieldTypeId, string inputType, string queryString, string treeXmlString, int topkMatches)
        {
            Dictionary<string, string> matches = new Dictionary<string, string>();

            // Transform Xml to Query for this particular node

            // Clear all of the "display" markers. The display marker (disp=1) for the value will be set later.
            treeXmlString = treeXmlString.Replace(@"display=""1""", @"display=""0""");

            // Split the fieldId into its subparts
            string fieldIdNode = fieldId.Split(';')[0];
            string fieldIdField = fieldId.Split(';')[1];

            // Get the last node's Type and TypeId. The last node in the id list is the node we are concerned with.
            string[] fieldIdNodes = fieldIdNode.Split(':');
            string fieldIdNodeLast = fieldIdNodes[fieldIdNodes.Length - 1];
            string fieldIdNodeLastType = fieldIdNodeLast.Split('-')[0];
            int fieldIdNodeLastTypeId = int.Parse(fieldIdNodeLast.Split('-')[1]);

            // Get the field type from the fieldId
            string fieldIdFieldType = fieldIdField.Split('-')[0];

            // Find the beginning of the node
            string fieldIdNodeLastXml = String.Format(@"type=""{0}"" typeId=""{1}"">", fieldIdNodeLastType, fieldIdNodeLastTypeId);
            int posNodeStart = treeXmlString.IndexOf(fieldIdNodeLastXml) + fieldIdNodeLastXml.Length;

            // Find the beginning of the field
            string fieldIdFieldTypeXml = String.Format(@"<field type=""{0}"" ", fieldIdFieldType);
            int posFieldStart = treeXmlString.IndexOf(fieldIdFieldTypeXml, posNodeStart);

            // Set the display marker and clear any valuesets for this field
            treeXmlString = String.Format(@"{0} display=""1"" {1}", treeXmlString.Substring(0, posFieldStart + fieldIdFieldTypeXml.Length - 1), treeXmlString.Substring(posFieldStart + fieldIdFieldTypeXml.Length + @" display=""0"" ".Length - 1));
            if(treeXmlString.Substring(treeXmlString.IndexOf('>', posFieldStart) + 1).StartsWith("<valueset>"))
            {
                int posValuesetStart = treeXmlString.IndexOf('>', posFieldStart) + 1;
                int posValuesetEnd = treeXmlString.IndexOf("</valueset></field>", posValuesetStart) + "</valueset>".Length;
                treeXmlString = String.Format(@"{0}{1}", treeXmlString.Substring(0, posValuesetStart), treeXmlString.Substring(posValuesetEnd));
            }

            //QNode.InitializeTypes(Assembly.GetAssembly(typeof(QNodePathway)));
            XmlDocument xmlDoc = new DataHandler().ParseAndValidate(treeXmlString);
            if(xmlDoc.DocumentElement.ChildNodes.Count > 0)
            {
                QNode rootNode = QNode.Parse(xmlDoc.DocumentElement, null);
                IQueryResults queryResults = rootNode.Query();
                SqlDatasetQueryResults queryResultsSql = (SqlDatasetQueryResults) queryResults;
                DataTable dt = queryResultsSql.Dt;

                if(dt != null)
                {
                    foreach(DataRow r in dt.Rows)
                    {
                        string item = r[0].ToString().Trim();

                        if(item.IndexOf(queryString, StringComparison.CurrentCultureIgnoreCase) > -1)
                            if(!matches.ContainsValue(item)) // filter out unique values with multiple ids
                                matches.Add(item, node[(fieldId.Split(';'))[0]].Fields[fieldTypeId].InputTemplateDictionary[inputType].GetPrettyValue(item));

                        if(matches.Count >= topkMatches) // limit number of choices
                            break;
                    }
                }
            }

            return matches;
        }
    }
}