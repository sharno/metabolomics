using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace AQILib
{
    /// <summary>
    /// A class used to help talk to the AQI nodes
    /// </summary>
    public class DataHandler
    {
        public DataHandler()
        { }

        /// <summary>
        /// Execute the given query xml document and render it
        /// </summary>
        /// <param name="query"></param>
        /// <param name="queryGuiData"></param>
        /// <param name="queryGuiDataOut"></param>
        /// <param name="nullQueryRenderer"></param>
        /// <returns></returns>
        public IGuiComponent Query(string query, IGuiData queryGuiData, out IGuiData queryGuiDataOut, IQueryRenderer nullQueryRenderer)
        {
            IGuiComponent queryGui;

            if(query.Length <= 0)
            {
                queryGui = nullQueryRenderer.Render(null, null, null, out queryGuiDataOut);
            }
            else
            {
                XmlDocument queryXml = ParseAndValidate(query);

                if(queryXml.DocumentElement.ChildNodes.Count == 0)
                {
                    queryGui = nullQueryRenderer.Render(null, null, null, out queryGuiDataOut);
                }
                else
                {
                    QNode rootNode = QNode.Parse(queryXml.DocumentElement, null);
                    IQueryResults queryResults = rootNode.Query();
                    queryGui = rootNode.RenderQuery(queryResults, queryGuiData, out queryGuiDataOut);
                }
            }

            return queryGui;
        }

        /// <summary>
        /// Parse and validate a query xml document
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public XmlDocument ParseAndValidate(string query)
        {
            XmlReader querySchema = XmlReader.Create(new System.IO.StringReader(AQILib.Properties.Resources.QueryXmlSchema));
            XmlDocument queryXml = new XmlDocument();
            queryXml.LoadXml(query);

            if(!queryXml.HasChildNodes)
                throw new DataValidationException("Validation Error: Empty AQI tree");

            queryXml.Schemas.Add("http://nashua.case.edu/aqiquery", querySchema);
            queryXml.Validate(new ValidationEventHandler(ValidationHandler));
            ValidateNodes(queryXml.FirstChild, null);

            return queryXml;
        }

        /// <summary>
        /// Validate a query ensuring that only the proper data exists
        /// </summary>
        /// <param name="query">The 'node' element in the XML where the parsing will begin</param>
        public void ValidateNodes(XmlNode query, QNode parent)
        {
            QNode n;
            n = QNode.CreateInstance(query.Attributes["type"].Value, parent);

            foreach(XmlNode child in query.ChildNodes)
            {
                if(child.Name.Equals("field"))
                {
                    QField f;

                    if(!n.Fields.ContainsKey(child.Attributes["type"].Value))
                        throw new DataValidationException("Validation Error: Illegal field type");
                    f = n.Fields[child.Attributes["type"].Value];

                    foreach(XmlNode valueset in child.ChildNodes)
                    {
                        List<string> valueNamesEncountered = new List<string>();

                        foreach(XmlNode value in valueset.ChildNodes)
                        {
                            string name = value.Attributes["name"].Value;
                            if(valueNamesEncountered.Contains(name))
                                throw new DataValidationException("Validation Error: Duplicated value names in a single valueset");
                            valueNamesEncountered.Add(name);

                            f.Validate(value.Attributes["name"].Value, value.Attributes["value"].Value);
                        }
                    }
                }
                else
                {
                    if(child.Name.Equals("node"))
                    {
                        if(n.IsValidRelationship(child.Attributes["type"].Value))
                            ValidateNodes(child, n);
                        else
                            throw new DataValidationException(String.Format("Validation Error: Invalid parent/child relationship detected between parent type {0} and child type {1}", n.NodeTypeName, int.Parse(child.Attributes["type"].Value)));
                    }
                    else
                    {
                        // This error should never happen due to the schema validation
                        throw new DataValidationException("Validation Error: Unexpected XML tag encountered");
                    }
                }
            }

            return;
        }

        public static void ValidationHandler(object sender, ValidationEventArgs e)
        {
            throw new DataValidationException("Validation " + e.Severity.ToString() + ": " + e.Message);
        }
    }
}