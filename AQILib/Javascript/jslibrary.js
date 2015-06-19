// These first two functions are for backwards-compatibility until built-in queries are updated
function toggleElementPic(id, imgid)
{
	toggleElementPic(id, imgid, "block");
}

function toggleElementPic(id, imgid, type)
{
	var	theImage = document.getElementById(imgid);
	var	element	= document.getElementById(id);
	if(element.style.display ==	"none")
	{
		element.style.display =	type;
		theImage.src = "../Images/collapse.gif";
	}
	else
	{
		element.style.display =	"none";
		theImage.src = "../Images/expand.gif";
	}
}


// Global variables
var ajaxURL = "AjaxHandler.aspx";  // URL that handles Ajax calls
var panelQueue = new Array();    // Stores panels that will be rendered after the page has loaded
var panelQueueLo = new Array();  // Stores lower-priority panels that will be rendered after high-priority ones

// Add a panel to the panel queue
function addToQueue(id, control, args, type)
{
	panelQueue.push(new Array(id, control, args, type));
}

// Add a panel to the low-priority panel queue
function addToLoQueue(id, control, args, type)
{
	panelQueueLo.push(new Array(id, control, args, type));
}

// Perform actions that must be done after the page has loaded
function startup()
{
	// Send screen information
	//var h = window.innerHeight ? window.innerHeight : document.body.offsetHeight ?
	//	document.body.offsetHeight : screen.availHeight;
	var h = screen.availHeight ? screen.availHeight : document.body.offsetHeight ? document.body.offsetHeight : window.innerHeight; 
	//	document.body.offsetHeight;
	xmlhttpGet("op=ClientInfo&w="+(document.getElementById("content").clientWidth)+"&h="+h+
		"&aw="+screen.availWidth);	
	
	// Load panels in the panel queue
	for(x in panelQueue)
	{
		toggleRegion(panelQueue[x][0], panelQueue[x][1], panelQueue[x][2], panelQueue[x][3], false);
	}
	
	for(x in panelQueueLo)
	{
		toggleRegion(panelQueueLo[x][0], panelQueueLo[x][1], panelQueueLo[x][2], panelQueueLo[x][3], false);
	}
}

function hideRegion(id)
{
		var	region = document.getElementById(id);
		if (region)
		{
		    region.style.display = "none";
		}
}

function showRegion(id)
{
		var	region = document.getElementById(id);
		if (region)
		{
		    region.style.display = "block";
		}
}

// Open/close a collapsible panel.  If the content is not loaded in the panel yet, load it up.
function toggleRegion(id, control, args, type, change)
{
	// Don't do anything if the region is already loading

	
	if(change)
	{
		var	img = document.getElementById(id + "Image");
		var	region = document.getElementById(id + "Region");
		if(region.style.display == "none")
		{
			region.style.display = type;
			img.src = "../Images/collapse.gif";
			// Fun stuff
			//region.style.opacity = 0.2;
			//var t = setTimeout("fadein('"+id+"Region', 0.2)", 25);
		}
		else
		{
			region.style.display = "none";
			img.src = "../Images/expand.gif";
		}
	}
		
	if(document.getElementById(id + "Region") && document.getElementById(id + "Region").innerHTML.match(id + "Loading"))
	{
		var params = "op=Collapse&control="+control+"&target="+id+"&arglist="+args;		
		xmlhttpGet(params, fillRegion, id, false);
	}
}

// Change the GO level currently being viewed
function changeGo(id, level, pwid, viewid)
{
	// TODO: Make this much nicer, and use the new CollapsiblePanel model
	document.getElementById(id + "Region").innerHTML = "<div class=\"whitebg\">Updating GO annotations for level "
	+level+"... <img src=\"../Images/loading.gif\" alt=\"Updating...\" title=\"Updating...\" /></div>";
	document.getElementById(id + "Title").innerHTML = "GO Pathway Annotations (PW-ANN)";
	
	var params = "op=ChangeGoLevel&id="+id+"&level="+level+"&pwid="+pwid+"&viewid="+viewid;
	xmlhttpGet(params, fillRegion, id, false);
}

// Fill the region of a collapsible panel with content.  The title and content data is split with
// the string "<contentstart>".
function fillRegion(xmlHttpReq, replaceControlId, userdata2)
{
	var title = document.getElementById(replaceControlId + "Title");
	var	region = document.getElementById(replaceControlId + "Region");
	if(title && region)
	{
		var req = xmlHttpReq.responseText.split( "<contentstart>", 2 );
		title.innerHTML = req[0];
		region.innerHTML = req[1];
	}
	else
	{
		alert("Region not found: " + replaceControlId);
	}
}

// Open up a new window in the upper-left corner of the screen
function winopen(theURL)
{
	var newWin = window.open(theURL, "", "resize=1,scroll");
	newWin.moveTo(0,0);
	newWin.focus();
}

// Open a link in a new window with no particularly special properties
function newWin(obj)
{
	window.open(obj.href);
	return false;
}

// Redirect users via the dropdown menu of the content browser
function dropRedirect(val)
{
	window.location	= "LinkForwarder.aspx?rid=" + val + "&rtype=br";
}

// Change the list of pages available for browsing in the content browser
function changePageList(start, total, atonce, current, control, arglist)
{
	// TODO: Update to use DOM instead
	var newpages = "", i
	
	// First, update the page list
	for(i=0; i<atonce; i++)
	{
		var j = i+start;
		if(j > total) break;
		newpages += "<a href=\"javascript:void(0)\""+(j==current?" class=\"bold\"":"")
			+" id=\"SPage"+j+"\" title=\"Page "+j+"\" onclick=\"changeSearchPage("+j+","+start+","+atonce+","
			+total+",'"+control+"','"+arglist+"')\">"+j+"</a>" +(i==atonce?"":" ");
	}
	
	document.getElementById("SearchPageList").innerHTML = newpages;
	
	// Now, update the paging buttons
	var prevpages = "", nextpages = "";
	if(start > 1)
	{
		prevpages = "<span class=\"pagingitem\" onclick=\"changePageList("+(start-atonce<1?1:start-atonce)
			+","+total+","+atonce+","+current+",'"+control+"','"+arglist
			+"')\" title=\"Previous pages\">&#0171;</span> ";
	}
	else prevpages = "<span class=\"inactivepagingitem\">&#0171;</span> ";
		
	if(current > 1)
	{
		prevpages += "<span class=\"pagingitem\" onclick=\"changeSearchPage("+(current-1)+","+start+","
			+atonce+","+total+",'"+control+"','"+arglist+"')\" title=\"Previous page\">&lt;</span>";
	}
	else prevpages += "<span class=\"inactivepagingitem\">&lt;</span>";
	
	if(current < total)
	{
		nextpages = "<span class=\"pagingitem\" onclick=\"changeSearchPage("+(current+1)+","+start+","
			+atonce+","+total+",'"+control+"','"+arglist+"')\" title=\"Next page\">&gt;</span> ";
	}
	else nextpages = "<span class=\"inactivepagingitem\">&gt;</span> ";
	
	if(start+atonce <= total)
	{
		nextpages += "<span class=\"pagingitem\" onclick=\"changePageList("
			+(start+(atonce*2)>total?total-atonce+1:start+atonce)+","+total+","+atonce+","+current
			+",'"+control+"','"+arglist+"')\" title=\"Next pages\">&#0187;</span>";
	}
	else nextpages += "<span class=\"inactivepagingitem\">&#0187;</span>";
	
	document.getElementById("SearchPrevPage").innerHTML = prevpages;
	document.getElementById("SearchNextPage").innerHTML = nextpages;
}

// Change which page of information is currently showing in the content browser
function changeSearchPage(page, start, atonce, total, control, arglist)
{
	// TODO: Update to use DOM
	// Don't do anything if the area is already loading
	if(!document.getElementById("BrowserListItems").innerHTML.match("Updating browser list..."))
	{
		document.getElementById("BrowserListItems").innerHTML =
			"<div class=\"bold\">Updating browser list... <img src=\"../Images/loading.gif\" alt=\"Loading...\" title=\"Loading...\" /></div>";
		
		var st = page-(atonce/2)<1?1:(total-page<(atonce/2)?total-atonce+1:page-(atonce/2));
		if(st<1) st = 1;
		changePageList(st, total, atonce, page, control, arglist);
		
		var params = "op=LoadBrowserList&control="+control+"&page="+page+"&arglist="+arglist;
		xmlhttpGet(params, fillBrowserList, "BrowserListItems", false);
	}
}

// Fill up the content browser with juicy information
function fillBrowserList(xmlHttpReq, replaceControlId, userdata2)
{
	document.getElementById(replaceControlId).innerHTML = xmlHttpReq.responseText;
}

// Switch between single and neighborhood pathway graphs
function changeGraphType(control, id, type, viewid, screenw, screenh)
{
	// TODO: Update to use DOM
	document.getElementById(control + "Region").innerHTML = "<div class=\"whitebg\">Updating graph... <img src=\"../Images/loading.gif\" alt=\"Updating...\" title=\"Updating...\" /></div>";
	
	var params = "op=ChangeGraphType&control="+control+"&id="+id+"&type="+type+"&viewid="+viewid+"&screenw="+screenw+"&screenh="+screenh;
	xmlhttpGet(params, fillRegion, control, false);
}

// Generic Ajax function... complete with custom data fields of awesomeness!
function xmlhttpGet(params, responseCallback, userdata1, userdata2)	
{
	// This code below may look like a big comment block, but it's not...
	var xmlHttpReq = false;
	/*@cc_on @*/
	/*@if (@_jscript_version >= 5)
	try
	{
		xmlHttpReq = new ActiveXObject("Msxml2.XMLHTTP");
	}
	catch(e)
	{
		try
		{
			xmlHttpReq = new ActiveXObject("Microsoft.XMLHTTP");
		}
		catch(e2)
		{
			xmlHttpReq = false;
		}
	}
	@end @*/

	if(!xmlHttpReq && typeof XMLHttpRequest != "undefined")
	{
		xmlHttpReq = new XMLHttpRequest();
	}
	
	if(xmlHttpReq)
	{
		if(xmlHttpReq.overrideMimeType) xmlHttpReq.overrideMimeType("text/xml");
		xmlHttpReq.open("POST", ajaxURL, true);
		xmlHttpReq.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
		xmlHttpReq.setRequestHeader("Content-Length", params.length);
		xmlHttpReq.setRequestHeader("Cache-Control", "no-cache");
		xmlHttpReq.setRequestHeader("Connection", "close");
		
		//xmlHttpReq.open("GET", strURL, true);

		xmlHttpReq.onreadystatechange =	function() 
		{
			if(xmlHttpReq.readyState == 4)	
			{
				// Sometimes interrupted requests may cause problems
				try
				{
					if(xmlHttpReq.status == 200)
					{
						if(responseCallback)
						{
							responseCallback(xmlHttpReq, userdata1,	userdata2);
						}
					}
					else
					{
						throw("Unsuccessful Ajax call (code "+xmlHttpReq.status+").  Status: "+xmlHttpReq.statusText);
					}
				}
				catch(error)
				{
					// Comment this line out for release builds
					//alert("Ajax exception caught.  Parameters: "+params.replace(/&/g,", ")+"  Message: "+error);
				}
			}
		}
		xmlHttpReq.send(params);
	}
}

function fillInnerHtml(xmlHttpReq, replaceControlId, userdata2)
{
	var	theControl = document.getElementById(replaceControlId);
	if(theControl)
	{
		theControl.innerHTML = xmlHttpReq.responseText;
	}
	else
	{
		alert("Not found: "	+ replaceControlId);
	}
}

function xmlhttpGetSetInnerHtml(strURL,	replaceControlId) 
{
	xmlhttpGet(strURL, fillInnerHtml, replaceControlId, false);
}

var fadeQueue = new Array();  // Array for fading effects

function startHighlight(id)
{
	if( fadeQueue[id] == null )
	{
		fadeQueue[id] = 150;
		document.getElementById(id).style.border = "solid 2px rgb(225,250,250)";
		document.getElementById(id+"_title").style.color = "rgb(150,150,250)";
		var t = setTimeout("fadeSelect('"+id+"')", 25);
	}
}

function endHighlight(id)
{
	fadeQueue[id] = null;
	document.getElementById(id).style.border = "none";
	document.getElementById(id+"_title").style.color = "#006600";
}

function fadeSelect(id)
{
	if( fadeQueue[id] != null )
	{
		var color = fadeQueue[id];
		var current = Math.abs(color);
		document.getElementById(id).style.borderColor = "rgb("+(75+current)+","+(100+current)+",250)";
		document.getElementById(id+"_title").style.color = "rgb("+current+","+current+",250)";
		
		color -= 5;
		if(color < -150) color = 145;
		
		fadeQueue[id] = color;
		
		var t = setTimeout("fadeSelect('"+id+"')", 25);
	}
}

function getEventSource(e)
{
    var targ;
	if (!e) var e = window.event;
	if (e.target) targ = e.target;
	else if (e.srcElement) targ = e.srcElement;
	if (targ.nodeType == 3) // defeat Safari bug
		targ = targ.parentNode;
	return targ;
}


function SetCheck(id, value)
{
	var element = document.getElementById(id);
	if (element)
    {
        element.checked = value;
    }
}

function SetCheckFromOther(id, valueId)
{
	var element = document.getElementById(id);
	var valueElement = document.getElementById(valueId);
	if (element && valueElement)
    {
        element.checked = valueElement.checked;
    }
}