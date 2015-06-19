using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Xml;
using AQILib.Gui;
using System.Web;
using System.Data;
using AQILib.Sql;
using System.Web.UI.WebControls;
using System.Collections;

namespace AQILib
{
    /// <summary>
    /// A helper class to use the AQI with web-based interfaces
    /// </summary>
    public class AQIWebInterface
    {
        private AQIWebInterface()
        {
        }

        public static string AQIAdd(IAQIUtil util, string nodeId, string xml)
        {
            StringBuilder sb = new StringBuilder();

            //QNode.InitializeTypes(Assembly.GetAssembly(typeof(QNodePathway)));
            XmlDocument xmlDoc = new DataHandler().ParseAndValidate(xml);
            QNode rootNode = QNode.Parse(xmlDoc.DocumentElement, null);
            GuiAQIBasicData guiData = new GuiAQIBasicData(rootNode, util);
            GuiAQIBasic guiComponent = (GuiAQIBasic) rootNode.RenderNode(guiData);

            Queue<Control> qCtrl = new Queue<Control>();
            qCtrl.Enqueue(guiComponent.QueryContentPanel);
            while(qCtrl.Peek().ID != nodeId)
            {
                Control ctrl = qCtrl.Dequeue();
                foreach(Control c in ctrl.Controls)
                    qCtrl.Enqueue(c);
            }
            foreach(Control c in qCtrl.Peek().Controls)
                sb.Append(RenderControl(c));

            return sb.ToString();
        }

        public static string AQIGenerate(IAQIUtil util, string tree, IGuiData queryGuiData, out IGuiData queryGuiDataOut)
        {
            StringBuilder sb = new StringBuilder();

            // Attempt to query. If the querier returns a DataValidationException, return that.
            IGuiComponent data = null;
            try
            {
                data = new DataHandler().Query(tree, queryGuiData, out queryGuiDataOut, new NullQueryRenderer(util));
            }
            catch (DataValidationException e)
            {
                queryGuiDataOut = null;
                return e.Message;
            }

            // Output the debug data
            if(ConfigurationManager.AppSettings.Get("UseQueryLogger").Equals("true"))
                    sb.Append("<pre>" + tree.Replace("<", "&lt;").Replace(">", "&gt;<br />") + "</pre>");

            // Output the proper HTML codes depending on the output data renderer
            if(data is GuiDataTable)
            {
                GuiDataTable guiData = (GuiDataTable) data;
            

                if(ConfigurationManager.AppSettings.Get("UseQueryLogger").Equals("true"))
                    sb.Append("<br /><br />" + guiData.SqlQuery.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;<br />").Replace("\"", "&quot;").Replace("'", "&apos;") + "<br /><br />" + guiData.SqlQueryNicelyFormatted.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;<br />").Replace("\"", "&quot;").Replace("'", "&apos;").Replace(" ", "&nbsp;").Replace("\n", "<br />"));

                sb.Append(RenderControl(guiData.Table));
            }
            else if(data is GuiNullMessage)
            {
                GuiNullMessage guiData = (GuiNullMessage) data;
                sb.Append(RenderControl(CreateLiteral(guiData.Message)));
            }
            else if(data is GuiPath)
            {
                GuiPath guiData = (GuiPath) data;

                if(guiData.IsErrorResult)
                {
                    sb.Append(RenderControl(CreateLiteral(guiData.ErrorString)));
                }
                else
                {
                    sb.Append(RenderControl(guiData.GridPanel));
                }
            }
            else
            {
                sb.Append(RenderControl(CreateLiteral("Invalid data type on the returned data of the query renderer.")));
            }

            return sb.ToString();
        }

        private static Panel CreateLiteral(string message)
        {
            Panel res = new Panel();
            res.CssClass = "whitebg";
            Literal msg = new Literal();
            msg.Text = message;
            res.Controls.Add(msg);
            return res;
        }

        public static string InSearch(IAQIUtil util, HttpResponse Response, string fieldId, string nodeType, string fieldTypeId, string inputType, string queryString, string treeXmlString, bool xmlFlag, bool quoteFlag, int topkMatches)
        {
            StringBuilder sb = new StringBuilder();

            //TODO: implement TOP-K search! (should be in SQL?)

            //QNode node = null;
            //try
            //{
            //    //BUG: only creating the single node, not parsing the entire graph; breaks simple field dependancies needed for getting molecule/pathway pairs!
            //    //TODO: parse the actual data & call appropriate class's GetValues() function!
            //    node = QNode.CreateInstance(nodeType, null);
            //}
            //catch
            //{
            //    return String.Format("ERROR: Type {0} not recognized!", nodeType);
            //}

            QNode root = null;
            QField field = null;
            Dictionary<string, string> matches = null;
            try
            {
                XmlDocument nodeXml = (new DataHandler()).ParseAndValidate(treeXmlString);
                root = QNode.Parse(nodeXml.DocumentElement, null);
            }
            catch
            {
                return String.Format("ERROR: Invalid query XML!");
            }

            // try to get values using overloaded QInput GetValues() function first
            field = root.GetField(fieldId);
            if (field != null)
            {
                //matches = node.Fields[fieldTypeId].InputTemplateDictionary[inputType].GetValues(queryString, util);
                matches = field.InputTemplateDictionary[inputType].GetValues(queryString, util);
            }

            if(matches == null)
            {
                //XmlDocument nodeXml = (new DataHandler()).ParseAndValidate(treeXmlString);
                //node = QNode.Parse(nodeXml.DocumentElement, null);

                // use query transformation to get only values that can possible return results
                matches = root.GetInputValues(fieldId, nodeType, fieldTypeId, inputType, queryString, treeXmlString, topkMatches);
            }

            if(matches == null)
            {
                return "Search requested for a non-searchable field.";
            }
            else
            {
                if (xmlFlag) // XML Version
                {
                    Response.ContentType = "text/xml";

                    sb.Append("<?xml version=\"1.0\"?><Suggestions>");

                    //Write("<suggestion>{1}{0}{1}</suggestion>\n", "", quoteFlag ? "\"" : ""); // empty entry

                    // Sort items before going out
                    SortedDictionary<string, string> sortedMatches = new SortedDictionary<string, string>();
                    foreach(string item in matches.Values)
                        sortedMatches.Add(item, item);

                    foreach(string item in sortedMatches.Values)
                    {
                        sb.AppendFormat("<suggestion>{1}{0}{1}</suggestion>\n", item, quoteFlag ? "\"" : "");
                    }

                    sb.Append("</Suggestions>");
                }
                else // JSON version
                {
                    Response.ContentType = "text/json";

                    sb.Append("[");

                    List<string> formattedItems = new List<string>(matches.Count);

                    //BE: we really want a blank item as the first in the list... but Dojo gets wacky if you try adding a space here
                    //formattedItems.Add(String.Format("[\"{2}{0}{2}\",\"{1}\"]", "", "", quoteFlag ? "\\\"" : ""));

                    // Sort items before going out
                    SortedDictionary<string, string> sortedMatches = new SortedDictionary<string, string>();
                    foreach(KeyValuePair<string, string> kvp in matches)
                        sortedMatches.Add(kvp.Value, kvp.Key);

                    foreach(KeyValuePair<string, string> item in sortedMatches)
                    {
                        // Note that when concerning the "matches" Dictionary, sortedMatches.item.Key == matches.item.Value and vice versa
                        formattedItems.Add(String.Format("[\"{2}{0}{2}\",\"{1}\"]", item.Key, item.Value, quoteFlag ? "\\\"" : ""));
                    }
                    sb.Append(String.Join(", \n", formattedItems.ToArray()));

                    sb.Append("]");
                }
            }

            return sb.ToString();
        }

        public static void CreateAQIOnPage(PlaceHolder AQIQuery, CollapsiblePanel AQIResults, CollapsiblePanel AQITips, string xml, IAQIUtil util)
        {
            // Render a root node (which consists of the potential root node's Add New links)
            QNode rootNode = QNode.CreateInstance("root", null);
            GuiAQIRoot g = (GuiAQIRoot) rootNode.RenderNode(new GuiAQIRootData(AQIQuery, AQIResults, AQITips, new QNodeIdCounter(), util));

            AQIQuery.Controls.Add(g.QueryPanel);
            AQIResults.Controls.Add(g.ResultsPanel);
            AQITips.Controls.Add(g.TipsPanel);

            // If an Xml parameter is specified, load those node(s) to replace the root add links panel
            if(xml != null)
            {
                XmlDocument xmlDoc = new DataHandler().ParseAndValidate(xml);
                rootNode = QNode.Parse(xmlDoc.DocumentElement, null);
                GuiAQIBasicData guiData = new GuiAQIBasicData(rootNode, util);
                GuiAQIBasic guiComponent = (GuiAQIBasic) rootNode.RenderNode(guiData);

                Control rootContentPanel = AQIQuery.Controls[0].Controls[0];
                rootContentPanel.Controls.AddAt(1, guiComponent.QueryContentPanel);

                // Add each script as a new function for the window onload event!
                Queue<Control> ctrls = new Queue<Control>();
                ctrls.Enqueue(rootContentPanel.Controls[0]);
                ctrls.Enqueue(rootContentPanel.Controls[1]);
                while(ctrls.Count > 0)
                {
                    Control ctrl = ctrls.Dequeue();

                    if(ctrl.GetType().FullName == "System.Web.UI.WebControls.Literal")
                    {
                        Literal ctrlLiteral = (System.Web.UI.WebControls.Literal) ctrl;
                        if(ctrlLiteral.Text.Contains("</script>"))
                        {
                            string script = ctrlLiteral.Text;
                            int scriptStartPos1 = script.IndexOf("<script");
                            int scriptStartPos2 = script.IndexOf(">", scriptStartPos1);
                            int scriptEndPos = script.IndexOf("</script>");
                            ctrlLiteral.Text = String.Format(@"<script>System.AddListener(window, ""load"", function() {{ {0} }});</script>", script.Substring(scriptStartPos2 + 1, scriptEndPos - scriptStartPos2 - 1));
                        }
                    }

                    foreach(Control c in ctrl.Controls)
                        ctrls.Enqueue(c);
                }

                // Add one last script to update the cardinalities!
                rootContentPanel.Controls.AddAt(2,
                                                AQILib.Gui.ControlBuilder.BuildLiteral(String.Format(@"
                                                    <script>
                                                        System.AddListener(window,
                                                                           ""load"",
                                                                           function()
                                                                           {{
                                                                               AQI.UpdateLinkCardinalities('root;Content');
                                                                           }});
                                                    </script>")));
            }
        }

        public static string RenderControl(Control control)
        {
            StringWriter sw = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(sw);
            control.RenderControl(writer);

            return sw.ToString();
        }
    }
}
