// AQI object - provides an interface for using the AQI tool //////////

// Runtime variables that should be set appropriately
var AQIConfig = new Array();
AQIConfig["nodeLimit"]         = 6;
AQIConfig["paramBGOnColor"]    = "#8B0000";
AQIConfig["paramBGOffColor"]   = "";
AQIConfig["paramTextOnColor"]  = "#FFF";
AQIConfig["paramTextOffColor"] = "#8B0000";
AQIConfig["imageDirectory"]    = "../Images/";

function AQISystem()
{	
	// Private variables and methods
	var numNodes = 0;
	var limitOverride = false;
	
	function checkNodeLimit()
	{
		if(limitOverride || numNodes < AQIConfig["nodeLimit"])
		{
		    return true;
		}
		else
		{
		    if(confirm("Adding more nodes may make your query take a very long time to process. Are you sure you want to continue adding nodes?"))
		    {
		        limitOverride = true;
		        return true;
		    }
		    else
		    {
		        return false;
		    }
		}
	}
		
	// Public methods
	this.SubmitQuery = function()
	{
	    // This function is run when the Ajax call returns successfully, display the results in their region
		function _showResults(res, region)
		{
		    if(region)
		        region.innerHTML = res.responseText;
		}
		
		//if(numNodes > AQIConfig["nodeLimit"] && !confirm("A query with this many nodes may take a very long "+
		//	"time to process.  Are you sure you want to proceed?")) return false;
		
		// Generate the Xml to send to the server
		//var xml = "<aqi version=\"1.0\">" + root.generateXML() + "</aqi>"; // NOW BROKEN --- REFACTOR
		var rootId = AQI.GetRootId();
		var xml = AQI.GetQueryXml(rootId, null, null);
		
		// Change the "results region" to display the "One moment please" text
		var resultsRegion = document.getElementById("AQIResultsRegion");
		if(resultsRegion)
		    resultsRegion.innerHTML = "<div class=\"whitebg\">One moment, please. <img src=\"../Images/loading.gif\"/></div>";
		
		// Ajax loading call to get the results from the server    
		xmlhttpGet("op=AQIGenerate&tree="+encodeURIComponent(xml), _showResults, resultsRegion);
		
		// Return the generated Xml
		return xml;
	}
	
	this.UpdateLinkCardinalities = function(id)
	{
	    var currentLinkPtr = null;
	    
	    for(var i = 0; i < document.getElementById(id).childNodes.length; i++)
	    {
	        var childNode = document.getElementById(id).childNodes[i];
	        
	        if(!childNode.id)
	            continue;
	        
	        if(childNode.getAttribute("domNodeType") == "link")
	            currentLinkPtr = childNode;
	        if(childNode.getAttribute("domNodeType") == "node")
	        {
	            numNodes += 1;
	            
	            currentLinkPtr.setAttribute("cardinality", parseInt(currentLinkPtr.getAttribute("cardinality")) + 1);
	            if(parseInt(currentLinkPtr.getAttribute("maxCardinality")) > 0 && parseInt(currentLinkPtr.getAttribute("cardinality")) >= parseInt(currentLinkPtr.getAttribute("maxCardinality")))
                {
                    hideRegion(currentLinkPtr.id + ";Content");
                    showRegion(currentLinkPtr.id + ";Content;Blank");
                }
                
                AQI.UpdateLinkCardinalities(childNode.id + ";Params");
	        }
	    }
	}
	
	this.GetRootId = function()
	{
        var rootId = "";
        
        for(var i = 0; i < document.getElementById("root;Content").childNodes.length; i++)
        {
            if(document.getElementById("root;Content").childNodes[i].id && document.getElementById("root;Content").childNodes[i].className == "aqinode")
            {
                rootId = document.getElementById("root;Content").childNodes[i].id;
                break;
            }
        }
        
//        if(rootId == "root;Add")
//            rootId = "";
	    
	    return rootId;
	}
	
	this.AddNode = function(type, parentID, linkID, ajaxHandlerURL) // linkID = the ID of the link panel
	{
		//var newNode;
	    //var newNodeDOM;
	    //var isDojo = false;
	    
		function _nodeAdded()
		{
		    newNode.className = "aqinode";
		    
		    // The next two lines need to be fixed!
		    // This is a quick fix to a IE bug.
		    
		    //Dectect IE from: http://snipplr.com/view/132/detect-ie/
            if (/msie/i.test(navigator.userAgent) && !/opera/i.test(navigator.userAgent))
            {
		        newNode.style.display = "none";
		        setTimeout(function() { newNode.style.display = "block"; }, 25);
		    }
		}
			
		function reportError(request)
	    {
		    alert('Sorry. There was an error:' + request);
	    }
	    
	    numNodes += 1;
	    if(!checkNodeLimit())
	        return false;
	    
	    var rootId = AQI.GetRootId();
		var queryXml = AQI.GetQueryXml(rootId, parentID, type);
	    
//	    var queryAddLinksPanel = document.getElementById("root;Add");
//	    if(!queryAddLinksPanel)
//	        throw("Adding a new node failed because the query's root's add links panel cannot be found.");
//	    queryAddLinksPanel.style.display = "none";
	    
		var queryParentContent = document.getElementById((parentID == "" ? "root" : parentID) + ";Content");
		if(!queryParentContent)
		    throw("Adding a new node failed because the query's parent content node cannot be found.");
		    
		var queryParentAddLinksPanel = document.getElementById((parentID == "" ? "root" : parentID) + ";" + linkID);
		if(!queryParentAddLinksPanel)
		    throw("Adding a new node failed because the query's parent's add links panel for this node type cannot be found.");
		   
	    queryParentAddLinksPanel.setAttribute("cardinality", parseInt(queryParentAddLinksPanel.getAttribute("cardinality")) + 1); 
		if(parseInt(queryParentAddLinksPanel.getAttribute("maxCardinality")) > 0 && parseInt(queryParentAddLinksPanel.getAttribute("cardinality")) >= parseInt(queryParentAddLinksPanel.getAttribute("maxCardinality")))
		{
		    hideRegion(queryParentAddLinksPanel.id + ";Content");
		    showRegion(queryParentAddLinksPanel.id + ";Content;Blank");
		}
		
		//System.StripToolTips(queryRootContent);
		//while(queryRootContent.childNodes.length > 0)
		    //queryRootContent.removeChild(queryRootContent.childNodes[0]);
		
		var newTypeId = AQI.GetQueryXmlNextNodeTypeId(type);
		
		newNode = document.createElement("div");
		newNode.id = (parentID == "" || parentID == "root" ? "" : parentID + ":") + type + "-" + newTypeId;
		newNode.className = "aqinode aqinodeload";
		newNode.setAttribute("style", parentID == "" || parentID == "root" ? "" : "margin-left: 20px;");
		newNode.setAttribute("domNodeType", "node");
		newNode.setAttribute("parentLinkID", linkID);
		newNode.typeId = newTypeId;
		newNode.appendChild(loadingImage());
		
		var ptr = queryParentAddLinksPanel.nextSibling;
		while(ptr)
		{
		    if(ptr.attributes && (ptr.getAttribute("domNodeType") != "node"))
		        break;
		    ptr = ptr.nextSibling
		}
		queryParentAddLinksPanel.parentNode.insertBefore(newNode, ptr);
		
		// Grab the node(s) from the server		
		var ajaxParams = $H();
		ajaxParams['op'] = "AQIAdd";
		ajaxParams['nodeId'] = newNode.id;
		ajaxParams['xml'] = queryXml;
		
		var myAjax = new Ajax.Updater(
				{ success: newNode }, 
				ajaxHandlerURL, 
				{
					method: 'post', 
					parameters: ajaxParams, 
					evalScripts: true, // TODO: Change this back to true! If you are reading this, do it now. Things will be forever broken if you don't! Thanks!
					onComplete: _nodeAdded,
					onFailure: reportError
				});
				
		return true;
	}
	
	this.GetQueryXmlNextNodeTypeId = function(nodeType)
	{	    
        var queue = new Array();
        var retVal = 1;

        queue.push(document.getElementById("root;Content"));
        while(queue.length > 0)
        {
            var element = queue.shift();
            if(element.className == "aqinode")
            {
                var elementIdSplit = element.id.split(":");
                var elementIdLast = elementIdSplit[elementIdSplit.length - 1]
                var elementIdLastType = elementIdLast.split("-")[0];
                var elementIdLastTypeId = parseInt(elementIdLast.split("-")[1]);
                
                if(elementIdLastType == nodeType)
                    if(elementIdLastTypeId >= retVal)
                        retVal = elementIdLastTypeId + 1;
            }

            var elementChildren = element.childNodes;
            for(var i = 0; i < elementChildren.length; i++)
                queue.push(elementChildren[i]);
        }
        
        return retVal;
	}
	
	this.GetQueryXml = function(id, parentID, newType) // id = "pathway-1"
	{
	    if(id == "")
	        return newType ? "<node negate=\"0\" type=\"" + newType + "\" typeId=\"1\"/>" : "";
	    
	    var idArray = id.split(":");
	    var idType = idArray[idArray.length - 1].split("-")[0];
	    var idTypeId = idArray[idArray.length - 1].split("-")[1];
	    var xml = "<node negate=\"0\" type=\"" + idType + "\" typeId=\"" + idTypeId + "\">";
	    
	    var paramRows = document.getElementById(id + ";Params").childNodes;
	    for(var i = 0; i < paramRows.length; i++)
	    {
	        var paramId = paramRows[i].id;
	        
	        if(paramId && paramId.indexOf(id) == 0)
	        {
	            if(paramRows[i].getAttribute("domNodeType") == "node") // This is a child node
	            {
	                xml += AQI.GetQueryXml(paramId, parentID, newType);
	            }
	            else if(paramRows[i].getAttribute("domNodeType") == "field") // This is a field
	            {
	                var paramInc = document.getElementById(paramId).inc;
	                var paramConnector = document.getElementById(paramId).getAttribute("connector");
	                var paramValuesets = AQI.GetQueryXmlValuesets(paramId);
	                
	                xml += "<field type=\"" + paramId.split(";")[1] + "\" ";
	                if(paramInc != null)
	                    xml += "display=\"" + (paramInc ? "1" : "0") + "\" ";
	                xml += "negate=\"0\" connector=\"" + paramConnector + "\"";
	                
	                if(paramValuesets == "")
	                    xml += "/>";
	                else
	                    xml += ">" + paramValuesets + "</field>";
	            }
	        }
	        
//	        if(paramId && paramId.indexOf(id) == 0 && paramRows[i].getAttribute("domNodeType") != "link")
//	        {
//	            // Is this a child node or a field?
//	            if(paramId && paramId.indexOf(id + ":") == 0) // It's a child
//	            {
//	                xml += AQI.GetQueryXml(paramId, parentID, newType);
//	            }
//	            else // It's a field
//	            {
//	                var paramInc = document.getElementById(paramId).inc;
//	                var paramValuesets = AQI.GetQueryXmlValuesets(paramId);
//    	            
////    	            if(paramInc || paramValuesets != "") //BE: bug with display info not sent when not displayed or queried
////    	            {
//	                    xml += "<field type=\"" + paramId.split(";")[1] + "\" display=\"" + (paramInc ? "1" : "0") + "\" negate=\"0\" connector=\"or\"";
//	                    xml += paramValuesets == "" ? "/>" : ">" + paramValuesets + "</field>"
////    	            }
//                }
//	        }
	    }
	    
//	    var childNodes = document.getElementById(id + ";Content").childNodes;
//	    for(var i = 0; i < childNodes.length; i++)
//	    {
//	        var childId = childNodes[i].id;
//	        
//	        if(childId && childId.indexOf(id + ":") == 0)
//	            xml += AQI.GetQueryXml(childId, parentID, newType);
//	    }
	    
	    if(id == parentID && newType)
	        xml += "<node negate=\"0\" type=\"" + newType + "\" typeId=\"" + AQI.GetQueryXmlNextNodeTypeId(newType) + "\"/>";
	    
	    xml += "</node>";
	    
	    return xml;
	}
	
	this.GetQueryXmlValuesets = function(id) // id  = "pathway-1;name"
	{
	    var xml = "";
	    var valueSpans = document.getElementById(id + ";Content").childNodes;
	    
	    var valueSpanCurPtr = 1;
	    xml += "<valueset>";
	    
	    for(var i = 0; i < valueSpans.length; i++)
	    {
	       var valueSpanId = valueSpans[i].id;
	       
	       if(valueSpanId && valueSpanId.indexOf(id) == 0)
	       {
	    	   var valueSpanName = (valueSpanId.split(";")[1]).split("-")[0]; // id.split(";")[1]
	           var valueSpanSetPtr = (valueSpanId.split(";")[1]).split("-")[1];
    	       
	           if(valueSpanSetPtr > valueSpanCurPtr)
	           {
	               valueSpanCurPtr = valueSpanSetPtr;
	               xml += "</valueset><valueset>";
	           }
	       
	           var paramValue = AQI.GetQueryXmlValuesetsValue(valueSpanId);
	           
	           if(paramValue != "")
	               xml += "<value name=\"" + valueSpanName + "\" value=\"" + paramValue + "\"/>"
	       }
	    }
	    
	    xml += "</valueset>";
	    
	    if(xml == "<valueset></valueset>")
	        xml = "";
	    
	    return xml;
	}
	
	this.GetQueryXmlValuesetsValue = function(id) // id = "pathway-1;name-1"
	{
	    var value;
	    
	    if(document.getElementById(id + ";Val")) // Is there a value for this input?
	    {
	        // If so...
	        if(document.getElementById(id + ";Val").dojoObj) // Is there a dojo box in this input?
	        {
//	            // If so...
//	            if(document.getElementById(id + ";Val").dojoObj.comboBoxSelectionValue.getValue()) // Is there a value given for the currently selected value (there will not be if it was typed in manually and not selected)
//	            {
//	                // If so, use the given value for the selected item
//	                value = document.getElementById(id + ";Val").dojoObj.comboBoxSelectionValue.getValue();
//	            }
//	            else
//	            {
	                // If not, use what is typed in
	                value = document.getElementById(id + ";Val").dojoObj.getValue();
//	            }
	        }
	        else if(typeof document.getElementById(id + ";Val").checked == "boolean") // This is not a dojo box, but a checkbox
	        {
	            value = document.getElementById(id + ";Val").checked ? "true" : "false";
	        }
	        else // If there is no dojo box or checkbox, just use the value
	        {
	            value = document.getElementById(id + ";Val").value;
	        }
	    }
	    else // There is no value for this input
	    {
	        value = "";
	    }
	    
//	    var value = document.getElementById(id + ";Val") ?
//	                  document.getElementById(id + ";Val").dojoObj ?
//	                    document.getElementById(id + ";Val").dojoObj.comboBoxSelectionValue.getValue() ?
//	                      document.getElementById(id + ";Val").dojoObj.comboBoxSelectionValue.getValue()
//	                    : document.getElementById(id + ";Val").dojoObj.getValue()
//	                  : document.getElementById(id + ";Val").value
//	                : "";
	    
	    value = value.replace(/&/g, "&amp;");
	    value = value.replace(/</g, "&lt;");
	    value = value.replace(/>/g, "&gt;");
	    value = value.replace(/"/g, "&quot;");
	    value = value.replace(/'/g, "&apos;");
	    
	    return value;
	}
	
	this.DeleteNode = function(id)
	{
		numNodesDeleted = 0;
		
		var queue = new Array();
		queue.push(document.getElementById(id));
		while(queue.length > 0)
		{
		    var element = queue.shift();
		    
            if(element.className == "aqinode")
                numNodesDeleted += 1;

            var elementChildren = element.childNodes;
            for(var i = 0; i < elementChildren.length; i++)
                queue.push(elementChildren[i]);
		}
		
		numNodes -= numNodesDeleted;
		
		if(numNodes < AQIConfig["nodeLimit"])
		    limitOverride = false;
		    
		var ptr = document.getElementById(id).parentNode.firstChild;
		while(ptr.id != id)
		    ptr = ptr.nextSibling;
		    
		while(ptr)
		{
		    if(ptr.attributes && ptr.getAttribute("domNodeType") == "link")
		        break;
		    ptr = ptr.previousSibling
		}
		
		var addLinksPanel = ptr;
		
		addLinksPanel.setAttribute("cardinality", parseInt(addLinksPanel.getAttribute("cardinality")) - 1); 
		if(parseInt(addLinksPanel.getAttribute("cardinality")) < parseInt(addLinksPanel.getAttribute("maxCardinality")))
		{
		    showRegion(addLinksPanel.id + ";Content");
		    hideRegion(addLinksPanel.id + ";Content;Blank");
		}
		
		document.getElementById(id).parentNode.removeChild(document.getElementById(id));
	}
	
	this.SetNodeCollapsed = function(id, collapsed)
	{
	    var nodeDOM = document.getElementById(id);
	    if(!nodeDOM)
	        throw("Collapsing the node'" + id + "' failed because it cannot be found in the DOM.");
	        
	    var contentDOM = document.getElementById(id+";Content");
	    if(!contentDOM)
	        throw("Collapsing the node'" + id + "' failed because it cannot be found in the DOM.");
	        
	    var imgDOM = document.getElementById(id+";Collapse");
	    if(!imgDOM)
	        throw("Collapsing the node'" + id + "' failed because it cannot be found in the DOM.");
	        
	    nodeDOM.collapsed = collapsed;
	    contentDOM.style.display = collapsed ? "none" : "";
	    imgDOM.src = AQIConfig["imageDirectory"] + (collapsed ? "expand" : "collapse") + ".gif";
	    
	    return;
	}
	
	this.ToggleNodeCollapsed = function(id)
	{
	    var nodeDOM = document.getElementById(id);
		if(!nodeDOM)
		    throw("Collapsing the node '" + id + "' failed because it cannot be found in the DOM.");
		    
		AQI.SetNodeCollapsed(id, !nodeDOM.collapsed);
		
		return;
	}
	
	this.SetParamInc = function(id, inc)
	{
		var fieldDOM = document.getElementById(id);
		if(!fieldDOM)
		    throw("Toggling the parameter '" + id + "' failed because it cannot be found in the DOM.");
		    
		var fieldLabelDOM = document.getElementById(id+";Label");
		if(!fieldLabelDOM)
		    throw("Toggling the parameter '" + id + "' failed because it cannot be found in the DOM.");
		
		fieldDOM.inc = inc;
		
		// Update the parameter visually
		fieldLabelDOM.style.backgroundColor = fieldDOM.inc ? AQIConfig["paramBGOnColor"] : AQIConfig["paramBGOffColor"];
		fieldLabelDOM.style.color = fieldDOM.inc ? AQIConfig["paramTextOnColor"] : AQIConfig["paramTextOffColor"];
		
		return;
	}
	
	this.ToggleParamInc = function(id)
	{
		var fieldDOM = document.getElementById(id);
		if(!fieldDOM)
		    throw("Toggling the parameter '" + id + "' failed because it cannot be found in the DOM.");
		    
		AQI.SetParamInc(id, !fieldDOM.inc);
		
		return;
	}
	
	this.AddField = function(parentId)
	{
	    var newFieldSpan = document.createElement("span");
	    newFieldSpan.id = parentId + "-" + (document.getElementById(parentId + ";Content").childNodes.length - 2);
	    newFieldSpan.appendChild(document.createTextNode(" or"));
	    newFieldSpan.appendChild(document.createElement("br"));
	    
	    var newField;
	    if(document.getElementById(parentId + "-1;Val").cloneField)
	        newField = document.getElementById(parentId + "-1;Val").cloneField(newFieldSpan.id + ";Val");
	    else
	        newField = document.getElementById(parentId + "-1;Val").cloneNode(true);
	    newField.id = newFieldSpan.id + ";Val";
	    newField.name = newField.id;
	    if(newField.dojoObj)
	        newField.dojoObj.setValue("");
	    else
	        newField.selectedIndex = 0;
	    
	    var deleteImg = document.createElement("img");
	    deleteImg.alt = "Delete this field";
	    deleteImg.title = deleteImg.alt;
	    deleteImg.className = "collapsebutton";
	    deleteImg.src = AQIConfig["imageDirectory"] + "delete.gif";
	    deleteImg.onclick = new Function("AQI.DeleteField('" + newFieldSpan.id + "');");
	    
	    newFieldSpan.appendChild(newField);
	    newFieldSpan.appendChild(deleteImg);
	    
	    document.getElementById(parentId + ";Content").insertBefore(newFieldSpan, document.getElementById(parentId + ";Content").lastChild.previousSibling);
	    
	    return;
	}
	
	this.DeleteField = function(id)
	{
	    document.getElementById(id).parentNode.removeChild(document.getElementById(id));
	    return true;
	}
}


function loadingImage()
{
	// Create and return an image to let people know we're thinking
	// TODO: Make this a global object
	var img = document.createElement("img");
	img.src = "../Images/loading.gif";
	img.alt = "Loading...";
	img.title = img.alt;
	img.style.marginLeft = "10px";
	return img;
}

var AQI = new AQISystem();