// System tools and utilities; only one instance exists

var System = new Object;

System.PositionOf = function(obj)
{
	var curleft = curtop = 0;
	if(obj.offsetParent)
	{
		curleft = obj.offsetLeft;
		curtop = obj.offsetTop;
		while(obj = obj.offsetParent)
		{
			curleft += obj.offsetLeft
			curtop += obj.offsetTop
		}
	}
	
	return [curleft,curtop];
}

System.AddListener = function(obj, name, func)
{
	// Eases cross-browser problems for adding event listeners
	if(obj.addEventListener) obj.addEventListener(name, func, false);
	else obj.attachEvent("on"+name, func);
}

System.RemoveListener = function(obj, name, func)
{
	// Eases cross-browser problems for removing event listeners
	if(obj.removeEventListener) obj.removeEventListener(name, func, false);
	else obj.detachEvent("on"+name, func);
}

System.AttachToolTip = function(obj, msg)
{
	// Attach a tooltip to the specified element that follows the mouse around
	obj.tip = document.createElement("div");
	obj.tip.className = "systemtooltip";
	obj.tip.innerHTML = msg;
	obj.tip.opacity = obj.tip.style.opacity = 0;
	obj.tip.fading = 0.1;
	
	function _fade()
	{
		if(obj.tip && obj.tip.fading)
		{
			if(obj.tip.opacity > 1.0 || obj.tip.opacity < 0.0) obj.tip.fading = 0;
			else
			{
				obj.tip.opacity += obj.tip.fading;
				obj.tip.style.opacity = obj.tip.opacity;
				setTimeout(_fade, 25);
			}
		}
	}
	
	function _move(e)
	{
		if(e != -1)
		{
			if(!e) e = window.event;
			if(obj.tip)
			{
				obj.tip.style.top = e.clientY - 20 + "px";
				obj.tip.style.left = e.clientX + 20 + "px";
				if(!obj.tip.opacity)
				{
					obj.tip.style.display = "block";
					setTimeout(_fade, 25);
				}
			}
		}
	}
	
	function _out()
	{
	    if(obj.tip)
		{
			bodyElement.removeChild(obj.tip);
			obj.tip = null;
			System.RemoveListener(obj, "mousemove", _move);
			System.RemoveListener(obj, "mouseout", _out);
		}
	}
	
	System.AddListener(obj, "mousemove", _move);
	System.AddListener(obj, "mouseout", _out);
	obj.tipMoveFunc = _move;
	obj.tipOutFunc = _out;
	var bodyElement = document.getElementsByTagName("body")[0];
	_move(-1);
	bodyElement.appendChild(obj.tip);
}

System.StripToolTips = function(obj)
{
    if(obj.tip)
    {
        var bodyElement = document.getElementsByTagName("body")[0];
	    bodyElement.removeChild(obj.tip);
	    obj.tip = null;
	    System.RemoveListener(obj, "mousemove", obj.tipMoveFunc);
	    System.RemoveListener(obj, "mouseout", obj.tipOutFunc);
    }
    
    var i = 0;
    for(i = 0; i < obj.childNodes.length; i++)
        System.StripToolTips(obj.childNodes[i]);
}