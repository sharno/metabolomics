<%@ Control Language="c#" Inherits="PathwaysWeb.Utilities.AQI" CodeFile="AQI.ascx.cs" %>
<%@ Register TagPrefix="aqi" Namespace="AQILib.Gui" Assembly="AQILib" %>
<script type="text/javascript" src="javascript/aqi.js"></script>
<!-- <script type="text/javascript" src="javascript/insearch.js"></script> -->

<form action="#" method="post" onsubmit="return false">
	<asp:PlaceHolder ID="AQIQuery" runat="server" />
</form>
<aqi:CollapsiblePanel ID="AQIResults" Runat="server" />
<aqi:CollapsiblePanel ID="AQITips" Runat="server" />