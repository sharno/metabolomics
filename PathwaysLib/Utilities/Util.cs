#region Using Declarations
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;
using System.Configuration;
#endregion

namespace PathwaysLib.Utilities
{
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/Utilities/Util.cs</filepath>
    ///		<creation>2006/08/09</creation>
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
    ///			<cvs_author>$Author: sarp $</cvs_author>
    ///			<cvs_date>$Date: 2010/11/15 20:40:57 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/Utilities/Util.cs,v 1.6 2010/11/15 20:40:57 sarp Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.6 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Static class containing assorted utility functions.  
    /// </summary>
    #endregion
    public class Util
    {
        public static readonly string NullGuid = "00000000-0000-0000-0000-000000000000";
        public static readonly int NullPathwayTypeId = 99;
        private Util()
        {
        }


        //public static string ModelRelatedSQLString(string entityName, string cond, string name)
        //{
        //    string SQLString = " SELECT " + entityName +
        //                       " FROM organism_groups og, Catalyzes c, Pathway_Processes pp, Pathways pa, Processes p " +
        //                       " WHERE " + cond +
        //                       " og.is_organism = 1 AND " +
        //                       " og.id = c.organism_group_id AND " +
        //                       " c.process_id = p.id AND " +
        //                       " pp.pathway_id = pa.id AND " +
        //                       " pp.process_id = p.id AND p.id IN " +
        //                       " ( SELECT DISTINCT pe.processId  " +
        //                         " FROM MapReactionsProcessEntities pe ) " +
        //                       " ORDER BY " + name;

        //    return SQLString;
        //}


        /// <summary>
        /// Converts an SBO number into a full SBO link.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string ConvertToSBOLink(string p)
        {
            return "http://www.ebi.ac.uk/sbo/main/" + ConvertToSBO(p);
        }

        /// <summary>
        /// Converts an SBO number into SBO format
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string ConvertToSBO(string p)
        {
            string fullSBO = "0000000";
            int zeros = fullSBO.Length - p.Length;
            fullSBO = fullSBO.Remove(zeros);
            return "SBO:" + fullSBO + p;
        }


        public static string TrimString(string s, int maxChars)
        {
            if (s == null)
                return null;
            if (maxChars < 4)
                return "...";
            if (s.Length <= maxChars)
                return s;
            return s.Substring(0, maxChars - 3) + "...";
        }

        public static string XmlDocumentToString(XmlDocument doc)
        {
            StringWriter sw = new StringWriter();
            doc.Save(sw);
            return sw.ToString();
        }

        /// <summary>
        /// Convert a string-encoded list of GUIDs deliminated by semi-colon (;) 
        /// into an array of type Guid.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Guid[] GuidListFromString(string text)
        {
            if (text == null || text.Trim() == "")
                return new Guid[0];

            string[] guidStrings = text.Split(new char[] { ';' });

            if (guidStrings.Length < 1)
                return new Guid[0];

            ArrayList results = new ArrayList();

            foreach (string s in guidStrings)
            {
                if (s.Trim() == "")
                    continue;

                results.Add(new Guid(s));
            }

            return (Guid[])results.ToArray(typeof(Guid));
        }

        public static string[] GetStringListFromString(string text)
        {
            if (text == null || text.Trim() == "")
                return new string[0];

            string[] strList = text.Split(new char[] { ';' });

            if (strList.Length < 1)
                return new string[0];

            ArrayList results = new ArrayList();

            foreach (string s in strList)
            {
                if (s.Trim() == "")
                    continue;

                results.Add(s);
            }

            return (string[])results.ToArray(typeof(string));
        }
        public static object StringToEnum(Type enumType, string value, object defaultValue)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            try
            {
                return Enum.Parse(enumType, value, true);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool GetConfigBoolValue(string key, bool defaultValue)
        {
            if (ConfigurationManager.AppSettings.Get(key) != null)
            {
                try
                {
                    return bool.Parse(ConfigurationManager.AppSettings.Get(key));
                }
                catch
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }

        public static string StringListMerge(List<string> items, string itemPrefix, string itemPostfix, string betweenItems)
        {
            if (items == null || items.Count < 1)
                return "";

            StringBuilder sb = new StringBuilder();

            sb.Append(itemPrefix);
            sb.Append(items[0]);
            sb.Append(itemPostfix);
            for (int i = 1; i < items.Count; i++)
            {
                sb.Append(betweenItems);

                sb.Append(itemPrefix);
                sb.Append(items[i]);
                sb.Append(itemPostfix);
            }

            return sb.ToString();
        }
    }
}

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: Util.cs,v 1.6 2010/11/15 20:40:57 sarp Exp $
	$Log: Util.cs,v $
	Revision 1.6  2010/11/15 20:40:57  sarp
	SBO Term is shown in full format.
	
	Revision 1.5  2010/11/11 21:41:44  sarp
	SBOTerms are added for Compartment, Species, and Reactions.
	
	Revision 1.4  2009/07/14 15:33:22  ann
	*** empty log message ***
	
	Revision 1.3  2009/05/27 14:03:36  ann
	*** empty log message ***
	
	Revision 1.2  2009/04/03 21:16:03  xjqi
	Visualization Part: Given a model id, visualize compartments in hierarchy with species,reactions.
	Not integrating with web interface.
	
	Revision 1.1  2008/05/16 21:15:58  mustafa
	*** empty log message ***
	
	Revision 1.4  2008/03/07 19:53:13  brendan
	AQI refactoring
	
	Revision 1.3  2007/04/09 17:14:31  ali
	*** empty log message ***
	
	Revision 1.2  2007/01/22 19:19:22  brendan
	added top menu items, created data model placeholder page, added config key to disable AQI, set kegg to default to symmetric layout
	
	Revision 1.1  2006/08/10 20:54:25  brendan
	Updated graph drawing interface to provide more layouts and ability to hide/show common molecules, regulator details, and cofactors.
	
------------------------------------------------------------------------*/
#endregion