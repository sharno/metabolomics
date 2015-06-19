#region Using Declarations

using System;
using System.Collections;
using System.Xml;
using System.Drawing;

#endregion

namespace PathwaysLib.Utilities
{
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>GraphLayoutLib/GraphBuilding/XmlUtil.cs</filepath>
    ///		<creation>2005/10/28</creation>
    ///		<author>
    ///				<name>Brendan Elliott</name>
    ///				<initials>BE</initials>
    ///				<email>bxe7@cwru.edu</email>
    ///		</author>
    ///		<contributors>
    ///			<contributor>
    ///				<name>none</name>
    ///				<initials>none</initials>
    ///				<email>none</email>
    ///			</contributor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: mustafa $</cvs_author>
    ///			<cvs_date>$Date: 2008/05/16 21:15:58 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/Utilities/XmlUtil.cs,v 1.1 2008/05/16 21:15:58 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Contains a set of static convience functions for Xml document building using DOM.
    /// </summary>
    #endregion
	public class XmlUtil
	{
		private XmlUtil()
		{
		}

        public static void AddAttribute(XmlElement element, string name, string value)
        {
            XmlAttribute attrib = element.OwnerDocument.CreateAttribute(name);
            attrib.Value = value;
            element.Attributes.Append(attrib);
        }

        public static void AddAttribute(XmlElement element, string name, object value)
        {
            AddAttribute(element, name, value.ToString());
        }

        public static void AddAttribute(XmlElement element, string name, Color value)
        {
            AddAttribute(element, name, value.R + "," + value.G + "," + value.B);
        }

        public static string EncodeXmlValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            return text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;<br />").Replace("\"", "&quot;").Replace("'", "&apos;");
        }
	}
}

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: XmlUtil.cs,v 1.1 2008/05/16 21:15:58 mustafa Exp $
	$Log: XmlUtil.cs,v $
	Revision 1.1  2008/05/16 21:15:58  mustafa
	*** empty log message ***
	
	Revision 1.2  2008/03/21 17:57:51  ann
	*** empty log message ***
	
	Revision 1.1  2007/10/11 16:53:25  brendan
	Removed TomSawyer dependencies from project by removing old web methods and moving key classes for generating the data XML into PathwaysWeb
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.1  2005/11/07 17:28:27  brendan
	New generic graph building code, exposed via a new web service call.
	
------------------------------------------------------------------------*/
#endregion