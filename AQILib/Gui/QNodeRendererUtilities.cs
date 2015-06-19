using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Configuration;
//using PathwaysLib.Utilities;

namespace AQILib.Gui
{
    public class QNodeRendererUtilities
    {
        public static IDLabel InputCheckboxLabel(QInput input, string fieldId, IAQIUtil util)
        {
            IDLabel l = new IDLabel();
            {
                l.ID = String.Format("{0}", fieldId);

                IDHtmlInputCheckBox checkbox = new IDHtmlInputCheckBox();
                {
                    checkbox.ID = String.Format("{0};Val", fieldId);
                    checkbox.Attributes.Add("name", String.Format("{0};Val", fieldId));
                    checkbox.Attributes.Add("class", "aqiselect");
                    checkbox.Checked = input.Value.Equals("true") ? true : false;
                }

                l.Controls.Add(checkbox);
            }

            return l;
        }

        public static IDLabel InputDropDownLabel(QInput input, string fieldId, IAQIUtil util)
        {
            IDLabel l = new IDLabel();
            {
                l.ID = String.Format("{0}", fieldId);

                IDHtmlSelect dropdown = new IDHtmlSelect();
                {
                    dropdown.ID = String.Format("{0};Val", fieldId);
                    dropdown.Attributes.Add("name", String.Format("{0};Val", fieldId));
                    dropdown.Attributes.Add("class", "aqiselect");
                    dropdown.Size = 1;

                    foreach(KeyValuePair<string, string> kvp in input.GetInitialValues())
                    {
                        string value = kvp.Key;
                        string text = kvp.Value;
                        dropdown.Items.Add(new ListItem(text, value));
                    }

                    dropdown.Value = input.Value;
                }

                l.Controls.Add(dropdown);
            }

            return l;
        }

        public static IDLabel InputAutoCompleteLabel(QInput input, string fieldId, IAQIUtil util)
        {
            return InputComboBoxLabel(input, fieldId, false, util);
        }

        public static IDLabel InputSelectLabel(QInput input, string fieldId, IAQIUtil util)
        {
            return InputComboBoxLabel(input, fieldId, true, util);
        }

        private static IDLabel InputComboBoxLabel(QInput input, string fieldId, bool forceValidOptionFlag, IAQIUtil util)
        {
            IDLabel l = new IDLabel();
            {
                l.ID = fieldId;

                IDPanel comboBoxPlaceHolder = new IDPanel();
                {
                    comboBoxPlaceHolder.ID = String.Format("{0};ValPlaceHolder", fieldId);
                    comboBoxPlaceHolder.Style.Add(System.Web.UI.HtmlTextWriterStyle.Display, "none");
                }

                Literal comboBoxScript = ControlBuilder.BuildLiteral(String.Format(@"
                    <script type=""text/javascript"">
	                    var autocompleteBox =
		                    dojo.widget.createWidget(""ComboBox"",
		                                             {{ dataUrl: """ + util.AQIDataURL + @""",
		                                               maxListLength: ""10"",
		                                               mode: ""remote"",
		                                               searchType: ""SUBSTRING"",
		                                               autoComplete: false,
                                                       forceValidOption: {5} }});

                        autocompleteBox.setValue(""{4}"");

                        autocompleteBox._onFocusInputOld = autocompleteBox._onFocusInput;
                        autocompleteBox._onFocusInput = function()
                        {{
                            this._onFocusInputOld();
                            this.dataUrl = """ + util.AQIDataURL + @""";
                            this.dataProvider.searchUrl = """ + util.AQIDataURL + @""";
                        }}

                        autocompleteBox.domNode.id = ""{3};Val"";
	                    autocompleteBox.domNode.isDojo = true;
	                    autocompleteBox.domNode.dojoObj = autocompleteBox;

	                    var placeholder = document.getElementById(""{3};ValPlaceHolder"");
                        document.getElementById(""{3}"").insertBefore(autocompleteBox.domNode, placeholder);
                        document.getElementById(""{3}"").removeChild(placeholder);

	                    document.getElementById(""{3};Val"").cloneField = function(newId)
	                    {{
		                    var newCombo =
			                    dojo.widget.createWidget(""ComboBox"",
			                                             {{ dataUrl: """ + util.AQIDataURL + @""",
			                                               maxListLength: ""10"",
			                                               mode: ""remote"",
			                                               searchType: ""SUBSTRING"",
			                                               autoComplete: false,
                                                           forceValidOption: {5} }});

                            newCombo._onFocusInputOld = newCombo._onFocusInput;
                            newCombo._onFocusInput = function()
                            {{
                                this._onFocusInputOld();
                                this.dataUrl = """ + util.AQIDataURL + @""";
                                this.dataProvider.searchUrl = """ + util.AQIDataURL + @""";
                            }}

                            newCombo.domNode.id = newId;
		                    newCombo.domNode.isDojo = true;
		                    newCombo.domNode.dojoObj = newCombo;

		                    return newCombo.domNode;
	                    }}
                    </script>
                    ", input.Parent.Parent.NodeTypeName, input.Parent.FieldTypeId, input.Name, fieldId, input.Value, forceValidOptionFlag ? "true" : "false"));
                     //input.Parent.Parent.NodeTypeName, input.Parent.FieldTypeId, input.Name, fieldId, util.AQIDataURL, input.Value));
                { }

                l.Controls.Add(comboBoxPlaceHolder);
                l.Controls.Add(comboBoxScript);
            }

            return l;
        }

        public static IDPanel FieldPanel(QField field, string parentNodeID)
        {
            string fieldId = String.Format("{0};{1}", parentNodeID, field.FieldTypeId);

            IDPanel fieldPanel = new IDPanel();
            {
                fieldPanel.ID = fieldId;
                fieldPanel.CssClass = "row aqirow";
                if(!field.Visible)
                    fieldPanel.Style.Add(System.Web.UI.HtmlTextWriterStyle.Display, "none");
                fieldPanel.Attributes.Add("domNodeType", "field");
                fieldPanel.Attributes.Add("connector", field.Connectors.ContainsKey("or") ? "or" : "");

                Literal fieldPanelIncScript = ControlBuilder.BuildLiteral(String.Format(@"
                    <script type=""text/javascript"">
                        AQI.SetParamInc('{0}', {1});
                    </script>
                    ", fieldId, field.Displayed ? "true" : "false"));
                { }

                IDPanel labelPanel = new IDPanel();
                {
                    labelPanel.ID = String.Format("{0};Label", fieldId);
                    labelPanel.CssClass = "label aqilabel";
                    if(field.DisplayedActive)
                    {
                        labelPanel.Attributes.Add("title", "Click to include/exclude parameter");
                        labelPanel.Attributes.Add("onclick", String.Format("AQI.ToggleParamInc('{0}')", fieldId));
                    }
                    else
                    {
                        labelPanel.Style.Add(System.Web.UI.HtmlTextWriterStyle.Cursor, "default");
                    }

                    Literal labelTitle = ControlBuilder.BuildLiteral(String.Format("{0}:", field.Title));
                    { }

                    labelPanel.Controls.Add(labelTitle);
                }

                IDPanel contentPanel = new IDPanel();
                {
                    contentPanel.ID = String.Format("{0};Content", fieldId);
                    contentPanel.CssClass = "content";

                    List<List<Label>> inputLabelsList = new List<List<Label>>();
                    {
                        if(field.Queried)
                        {
                            for(int i = 0; i < field.ValuesetCount; i++)
                            {
                                List<Label> inputLabels = new List<Label>();

                                foreach(List<QInput> inputs in field.Inputs.Values)
                                {
                                    string inputId = field.FieldTypeId == inputs[i].Name ? fieldId : String.Format("{0};{1}{2}", parentNodeID, field.FieldTypeId, inputs[i].Name);
                                    inputLabels.Add(((GuiAQIBasicInput) inputs[i].Renderer.Render(field.Parent, new GuiAQIBasicInputData(inputs[i], String.Format("{0}-{1}", inputId, i + 1)))).InputLabel);
                                }

                                inputLabelsList.Add(inputLabels);
                            }
                        }
                        else
                        {
                            List<Label> inputLabels = new List<Label>();

                            foreach(QInput input in field.InputTemplate)
                            {
                                string inputId = field.FieldTypeId == input.Name ? fieldId : String.Format("{0};{1}{2}", parentNodeID, field.FieldTypeId, input.Name);
                                inputLabels.Add(((GuiAQIBasicInput) input.Renderer.Render(field.Parent, new GuiAQIBasicInputData(input, String.Format("{0}-1", inputId)))).InputLabel);
                            }

                            inputLabelsList.Add(inputLabels);
                        }
                    }

                    IDLabel connectorSpan = new IDLabel();
                    {
                        HyperLink connectorLink = new HyperLink();
                        {
                            connectorLink.NavigateUrl = "#";
                            connectorLink.Attributes.Add("onclick", String.Format("AQI.AddField('{0}');return false", fieldId));
                            connectorLink.Text = "&nbsp;or...";
                            connectorLink.ToolTip = "Add a new value";
                        }

                        connectorSpan.Controls.Add(connectorLink);
                    }

                    Image deleteImage = new Image();
                    {
                        deleteImage.AlternateText = "Delete this field";
                        deleteImage.Attributes.Add("title", deleteImage.AlternateText);
                        deleteImage.CssClass = "collapsebutton";
                        // TODO: Change this to work with the settings!
                        deleteImage.ImageUrl = "../Images/delete.gif"; // AQIConfig["imageDirectory"] + "delete.gif";
                    }

                    for(int i = 0; i < inputLabelsList.Count; i++)
                    {
                        List<Label> inputLabels = inputLabelsList[i];

                        for(int j = 0; j < inputLabels.Count; j++)
                        {
                            contentPanel.Controls.Add(ControlBuilder.BuildLiteral(field.Descriptions[j]));

                            if(i > 0)
                            {
                                inputLabels[j].Controls.AddAt(0, ControlBuilder.BuildLiteral(" or"));
                                inputLabels[j].Controls.AddAt(1, ControlBuilder.BuildLiteral("<br/>"));

                                Image deleteImageClone = new Image();
                                deleteImageClone.AlternateText = deleteImage.AlternateText;
                                deleteImageClone.Attributes.Add("title", deleteImage.AlternateText);
                                deleteImageClone.CssClass = deleteImage.CssClass;
                                deleteImageClone.ImageUrl = deleteImage.ImageUrl;
                                deleteImageClone.Attributes.Add("onclick", String.Format("AQI.DeleteField('{0}-{1}');", fieldId, i + 1));
                                inputLabels[j].Controls.Add(deleteImageClone);
                            }

                            contentPanel.Controls.Add(inputLabels[j]);
                        }
                        contentPanel.Controls.Add(ControlBuilder.BuildLiteral(field.Descriptions[field.Descriptions.Count - 1]));

                        //if(i > 0)
                        //{
                        //    Image deleteImageClone = new Image();
                        //    deleteImageClone.AlternateText = deleteImage.AlternateText;
                        //    deleteImageClone.Attributes.Add("title", deleteImage.AlternateText);
                        //    deleteImageClone.CssClass = deleteImage.CssClass;
                        //    deleteImageClone.ImageUrl = deleteImage.ImageUrl;
                        //    deleteImageClone.Attributes.Add("onclick", String.Format("AQI.DeleteField('{0}-{1}');", fieldId, i + 1));
                        //    contentPanel.Controls.Add(deleteImageClone);
                        //}

                        if(i == inputLabelsList.Count - 1 && field.Connectors.ContainsKey("or"))
                        {
                            contentPanel.Controls.Add(connectorSpan);
                        }
                        //else
                        //{
                        //    contentPanel.Controls.Add(ControlBuilder.BuildLiteral(" or"));
                        //    contentPanel.Controls.Add(ControlBuilder.BuildLiteral("<br/>"));
                        //}
                    }
                }

                if(field.DisplayedActive)
                    fieldPanel.Controls.Add(fieldPanelIncScript);
                fieldPanel.Controls.Add(labelPanel);
                fieldPanel.Controls.Add(contentPanel);
            }

            return fieldPanel;
        }

        public static IDPanel LinkPanel(QNode node, QLink link, string parentNodeID, IAQIUtil util)
        {
            string linkId = String.Format("{0};{1}", parentNodeID, link.LinkTypeId);

            IDPanel linkPanel = new IDPanel();
            {
                linkPanel.ID = linkId;
                linkPanel.CssClass = "row aqirow";
                linkPanel.Attributes.Add("domNodeType", "link");
                linkPanel.Attributes.Add("maxCardinality", link.MaxCardinality.ToString());
                linkPanel.Attributes.Add("cardinality", "0");
                if(!node.Editable && link.Cardinality(node) <= 0)
                    linkPanel.Style.Add(System.Web.UI.HtmlTextWriterStyle.Display, "none");

                IDPanel labelPanel = new IDPanel();
                {
                    labelPanel.ID = String.Format("{0};Label", linkId);
                    labelPanel.CssClass = "label aqilabel";
                    if(node.Editable || link.Cardinality(node) > 0)
                        labelPanel.Style.Add(System.Web.UI.HtmlTextWriterStyle.Display, "block");
                    else
                        labelPanel.Style.Add(System.Web.UI.HtmlTextWriterStyle.Display, "none");
                    labelPanel.Style.Add(System.Web.UI.HtmlTextWriterStyle.Cursor, "default");
                    //labelPanel.Attributes.Add("title", "Click to include/exclude parameter");
                    //labelPanel.Attributes.Add("onclick", String.Format("AQI.ToggleParamInc('{0}')", fieldId));

                    Literal labelTitle = ControlBuilder.BuildLiteral(String.Format("{0}:", link.Title));
                    { }

                    labelPanel.Controls.Add(labelTitle);
                }

                IDPanel contentPanel = new IDPanel();
                {
                    contentPanel.ID = String.Format("{0};Content", linkId);
                    contentPanel.CssClass = "content";
                    if(node.Editable)
                        contentPanel.Style.Add(System.Web.UI.HtmlTextWriterStyle.Display, "block");
                    else
                        contentPanel.Style.Add(System.Web.UI.HtmlTextWriterStyle.Display, "none");
                    contentPanel.Style.Add(System.Web.UI.HtmlTextWriterStyle.Cursor, "default");

                    for(int i = 0; i < link.LinkedNodeTypes.Count; i++)
                    {
                        contentPanel.Controls.Add(ControlBuilder.BuildLiteral(link.Descriptions[i]));
                        contentPanel.Controls.Add(LinkPanelLink(link.LinkedNodeTypes[i], parentNodeID, link.LinkTypeId, util, link.LinkMouseOverTexts[i], link.LinkTexts[i], node.IsValidRelationship(link.LinkedNodeTypes[i])));
                    }
                    contentPanel.Controls.Add(ControlBuilder.BuildLiteral(link.Descriptions[link.Descriptions.Count - 1]));
                }

                IDPanel contentPanelBlank = new IDPanel();
                {
                    contentPanelBlank.ID = String.Format("{0};Content;Blank", linkId);
                    contentPanelBlank.CssClass = "content";
                    if(!node.Editable)
                        contentPanelBlank.Style.Add(System.Web.UI.HtmlTextWriterStyle.Display, "block");
                    else
                        contentPanelBlank.Style.Add(System.Web.UI.HtmlTextWriterStyle.Display, "none");
                    contentPanelBlank.Style.Add(System.Web.UI.HtmlTextWriterStyle.Cursor, "default");
                    contentPanelBlank.Controls.Add(ControlBuilder.BuildLiteral("&nbsp;"));
                }

                linkPanel.Controls.Add(labelPanel);
                linkPanel.Controls.Add(contentPanel);
                linkPanel.Controls.Add(contentPanelBlank);
            }

            return linkPanel;
        }

        public static HyperLink LinkPanelLink(string nodeType, string parentID, string linkID, IAQIUtil util, string linkToolTip, string linkText, bool isValidRelationship)
        {
            HyperLink retVal = new HyperLink();

            //retVal.NavigateUrl = "#";
            retVal.ForeColor = isValidRelationship ? System.Drawing.Color.FromArgb(0x0, 0x0, 0x80) : System.Drawing.Color.FromArgb(0x75, 0x75, 0x75);
            retVal.Style.Add(System.Web.UI.HtmlTextWriterStyle.Cursor, isValidRelationship ? "pointer" : "default");
            retVal.Attributes.Add("onmouseover", String.Format("System.AttachToolTip(this,'{0}')", (isValidRelationship ? "" : "(Not Allowed In This Context) ") + linkToolTip));
            if(isValidRelationship)
                retVal.Attributes.Add("onclick", String.Format("AQI.AddNode('{0}','{1}','{2}','{3}');return false", nodeType, parentID, linkID, util.AjaxHandlerURL));
            retVal.Text = linkText;

            return retVal;
        }

        //public static bool AddLinksPanel(QNode parentNode, string parentID, out IDPanel addLinksPanel, IAQIUtil util)
        //{
        //    addLinksPanel = new IDPanel();
        //    addLinksPanel.CssClass = "aqiaddnode";
        //    addLinksPanel.Controls.Add(ControlBuilder.BuildLiteral("Add new: "));

        //    SortedDictionary<int, SortedDictionary<string, HyperLink>> addLinks = new SortedDictionary<int, SortedDictionary<string, HyperLink>>();
            
        //    foreach(KeyValuePair<string, object[]> kvp in QRelationship.GetAddLinks(parentNode))
        //    {
        //        string nodeType = kvp.Key;
        //        object[] addLinkInfo = kvp.Value;

        //        int sortOrder = (int) addLinkInfo[0];
        //        string linkDesc = (string) addLinkInfo[1];
        //        string linkTip = (string) addLinkInfo[2];

        //        HyperLink addLink = new HyperLink();
        //        addLink.NavigateUrl = "#";
        //        addLink.Attributes.Add("onclick", String.Format("AQI.AddNode('{0}','{1}','{2}');return false", nodeType, parentID, util.AjaxHandlerURL));
        //        addLink.Attributes.Add("onmouseover", String.Format("System.AttachToolTip(this,'{0}')", linkTip));
        //        addLink.Text = linkDesc;

        //        if(!addLinks.ContainsKey(sortOrder))
        //            addLinks.Add(sortOrder, new SortedDictionary<string, HyperLink>());
        //        addLinks[sortOrder][linkDesc] = addLink;
        //    }
            
        //    foreach(SortedDictionary<string, HyperLink> AddLinksGroup in addLinks.Values)
        //    {
        //        foreach(HyperLink l in AddLinksGroup.Values)
        //        {
        //            addLinksPanel.Controls.Add(l);
        //            addLinksPanel.Controls.Add(ControlBuilder.BuildLiteral(", "));
        //        }
        //    }

        //    addLinksPanel.Controls.RemoveAt(addLinksPanel.Controls.Count - 1); // Remove the last ", " or the "Add New: " (if no links exist)

        //    return (addLinksPanel.Controls.Count != 0);
        //}

        public static void QueryContentPanel(QNode rootNode, QNodeIdCounter idCounter, string parentNodeID, bool showNumberInTitle, out string nodeId, out Literal nodeCollapsedScript, out IDPanel titlePanel, out IDPanel contentPanel, IAQIUtil util)
        {
            nodeId = rootNode.Id; //nodeId = String.Format("{0}{1}{2}-{3}", parentNodeID, parentNodeID.Length == 0 ? "" : ":", rootNode.NodeTypeName, rootNode.NodeTypeId); //idCounter.NextTypeId(rootNode.NodeType));

            nodeCollapsedScript = ControlBuilder.BuildLiteral(String.Format(@"
                                     <script type=""text/javascript"">
                                         AQI.SetNodeCollapsed('{0}', false);
                                     </script>
                                     ", nodeId));
            { }

            titlePanel = new IDPanel();
            {
                titlePanel.ID = String.Format("{0};Title", nodeId);
                titlePanel.CssClass = "aqititle";

                IDPanel titleLeftPanel = new IDPanel();
                {
                    titleLeftPanel.CssClass = "aqititleleft";
                    titleLeftPanel.Attributes.Add("onclick", String.Format("AQI.ToggleNodeCollapsed('{0}')", nodeId));
                    titleLeftPanel.ToolTip = "Click to collapse/expand this node";

                    IDImage titleLeftCollapseButton = new IDImage();
                    {
                        titleLeftCollapseButton.ID = String.Format("{0};Collapse", nodeId);
                        titleLeftCollapseButton.CssClass = "collapsebutton";
                        titleLeftCollapseButton.AlternateText = "Click to collapse/expand this node";
                        titleLeftCollapseButton.ToolTip = "Click to collapse/expand this node";
                        titleLeftCollapseButton.ImageUrl = "../Images/collapse.gif";
                    }

                    Label titleLeftLabel = new Label();
                    {
                        titleLeftLabel.Text = String.Format("{0}{1}", rootNode.Title, showNumberInTitle ? String.Format(" {0}", rootNode.NodeTypeId) : ""); //idCounter.CurrentTypeId(rootNode.NodeType));
                        titleLeftLabel.CssClass = "aqinodetitle";
                    }

                    titleLeftPanel.Controls.Add(titleLeftCollapseButton);
                    titleLeftPanel.Controls.Add(titleLeftLabel);
                }

                Image titleDeleteButton = new Image();
                {
                    if(rootNode.Editable)
                    {
                        titleDeleteButton.CssClass = "aqideletebutton";
                        titleDeleteButton.AlternateText = "Delete this node";
                        titleDeleteButton.ToolTip = "Delete this node";
                        titleDeleteButton.ImageUrl = "../Images/delete.gif";
                        titleDeleteButton.Attributes.Add("onclick", String.Format("AQI.DeleteNode('{0}')", nodeId));
                    }
                }

                titlePanel.Controls.Add(titleLeftPanel);
                if(rootNode.Editable)
                    titlePanel.Controls.Add(titleDeleteButton);
            }

            contentPanel = new IDPanel();
            {
                contentPanel.ID = String.Format("{0};Content", nodeId);
                contentPanel.CssClass = "aqicontent";

                IDPanel paramPanel = new IDPanel();
                {
                    paramPanel.ID = String.Format("{0};Params", nodeId);
                    paramPanel.CssClass = "aqiparams";

                    Dictionary<string, IDPanel> fieldPanels = new Dictionary<string, IDPanel>();
                    {
                        foreach(QField f in rootNode.Fields.Values)
                            fieldPanels.Add(f.FieldTypeId, ((GuiAQIBasicField) rootNode.RenderField(new GuiAQIBasicFieldData(f, nodeId))).FieldPanel);
                    }

                    Dictionary<string, IDPanel> linkPanels = new Dictionary<string, IDPanel>();
                    {
                        foreach(KeyValuePair<string, QLink> kvp in rootNode.Links)
                        {
                            bool displayThisLinkPanel = false;
                            foreach(string linkedNodeType in kvp.Value.LinkedNodeTypes)
                            {
                                if(rootNode.IsValidRelationship(linkedNodeType))
                                {
                                    displayThisLinkPanel = true;
                                    break;
                                }
                            }

                            if(displayThisLinkPanel)
                                linkPanels.Add(kvp.Key, LinkPanel(rootNode, kvp.Value, nodeId, util));
                            else
                                linkPanels.Add(kvp.Key, null);
                        }
                    }

                    Dictionary<string, List<IDPanel>> childPanels = new Dictionary<string, List<IDPanel>>();
                    {
                        foreach(QNode child in rootNode.Children)
                        {
                            IDPanel childPanel = ((GuiAQIBasic) child.RenderNode(new GuiAQIBasicData(child, idCounter, nodeId, util))).QueryContentPanel;
                            if(!childPanels.ContainsKey(child.NodeTypeName))
                                childPanels.Add(child.NodeTypeName, new List<IDPanel>());
                            childPanels[child.NodeTypeName].Add(childPanel);
                        }
                    }

                    string idPtr = String.Empty;
                    while(linkPanels.ContainsKey(idPtr))
                    {
                        if(linkPanels[idPtr] != null)
                            paramPanel.Controls.Add(linkPanels[idPtr]);

                        foreach(string linkedNodeType in rootNode.Links[idPtr].LinkedNodeTypes)
                            if(childPanels.ContainsKey(linkedNodeType))
                                foreach(IDPanel childPanel in childPanels[linkedNodeType])
                                    paramPanel.Controls.Add(childPanel);

                        idPtr = rootNode.Links[idPtr].LinkTypeId;
                    }

                    foreach(KeyValuePair<string, IDPanel> kvp in fieldPanels)
                    {
                        string fieldTypeId = kvp.Key;
                        IDPanel fieldPanel = kvp.Value;

                        paramPanel.Controls.Add(fieldPanel);

                        idPtr = fieldTypeId;
                        while(linkPanels.ContainsKey(idPtr))
                        {
                            if(linkPanels[idPtr] != null)
                                paramPanel.Controls.Add(linkPanels[idPtr]);

                            foreach(string linkedNodeType in rootNode.Links[idPtr].LinkedNodeTypes)
                                if(childPanels.ContainsKey(linkedNodeType))
                                    foreach(IDPanel childPanel in childPanels[linkedNodeType])
                                        paramPanel.Controls.Add(childPanel);

                            idPtr = rootNode.Links[idPtr].LinkTypeId;
                        }
                    }
                }

                //IDPanel addPanel;
                //bool useAddPanel;
                //{
                //    useAddPanel = AddLinksPanel(rootNode, nodeId, out addPanel, util);
                //    if(useAddPanel)
                //        addPanel.ID = String.Format("{0};Add", nodeId);
                //}

                contentPanel.Controls.Add(paramPanel);
                //foreach(IDPanel childPanel in childPanels)
                //    contentPanel.Controls.Add(childPanel);
                //if(useAddPanel)
                //    contentPanel.Controls.Add(addPanel);
            }

            return;
        }
    }
}