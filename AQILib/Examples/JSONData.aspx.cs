using AQILib;
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using PathwaysLib.Exceptions;
using PathwaysLib.ServerObjects;
using PathwaysLib.Utilities;
using PathwaysLib.WebControls;
using System.Collections.Generic;
using PathwaysLib.AQI;

public partial class Web_JSONData : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        EventLogger.ComponentName = "JSONData.aspx";
        EventLogger.Url = Request.Url.ToString();
        DBWrapper.Instance = new DBWrapper();

        Session.Timeout = 15;

        //Response.ContentType = "text/json";
        //Response.ContentType = "text/html"; //for debugging
        Response.ContentType = "text/xml";
        Response.Cache.SetCacheability(HttpCacheability.NoCache); // using caching since data rarely changes???

        if(string.IsNullOrEmpty(Request.Params["node_type"]) && string.IsNullOrEmpty(Request.Params["query_str"]))
        {
            Response.Write("Access denied.");
            return;
        }

        try
        {
            string nodeType = Request.Params["node_type"];
            string fieldTypeId = Request.Params["field_type"];
            string inputType = Request.Params["input_type"];
            string queryString = Request.Params["query_str"];
            bool xmlFlag = (Request.Params["xml_flag"] == "1");
            bool quoteFlag = (Request.Params["quote_flag"] == "1");

            //int topk = 10;
            //if (!string.IsNullOrEmpty(Request.Params["k"]))
            //{
            //    topk = int.Parse(Request.Params["k"]);
            //    if (topk < 1)
            //        topk = 1;
            //}

            InSearch(nodeType, fieldTypeId, inputType, queryString, xmlFlag, quoteFlag);
        }
        catch (Exception ex)
        {
            EventLogger.SystemEventLog("Unhandled exception: " + ex.ToString());
            Error(ex.ToString());
        }
        finally
        {
            EventLogger.ComponentName = null;
            if (!DBWrapper.IsInstanceNull)
            {
                DBWrapper.Instance.Close();
                DBWrapper.Instance = null;
            }
        }
    }

    public void InSearch(string nodeType, string fieldTypeId, string inputType, string queryString, bool xmlFlag, bool quoteFlag)
    {
        //TODO: implement TOP-K search!

        // Create a node and tell one of its fields to process this request
        QNode node = null;
        try
        {
            QNode.InitializeTypes(Assembly.GetAssembly(typeof(QNodePathway)));
            node = QNode.CreateInstance(nodeType, null);
        }
        catch
        {
            Error(String.Format("Type {0} not recognized!", nodeType));
            return;
        }

        Dictionary<string, string> matches =
            queryString.Trim().Length >= 0 ?
                //((QParam)newNode.Parameters[paramId]).Fields[fieldId].SearchFor(str.Trim()) :
                node.Fields[fieldTypeId].InputTemplateDictionary[inputType].GetValues(queryString) :
                new Dictionary<string, string>();

        if(matches == null)
        {
            Error("Search requested for a non-searchable field.");
        }
        else
        {
            if(xmlFlag) // XML Version
            {
                Write("<?xml version=\"1.0\"?><Suggestions>");

                //Write("<suggestion>{1}{0}{1}</suggestion>\n", "", quoteFlag ? "\"" : ""); // empty entry

                foreach (string item in matches.Values)
                {
                    Write("<suggestion>{1}{0}{1}</suggestion>\n", item, quoteFlag ? "\"" : "");
                }

                Write("</Suggestions>");
            }
            else // JSON version
            {
                Write("[");

                List<string> formattedItems = new List<string>(matches.Count);

                //BE: we really want a blank item as the first in the list... but Dojo gets wacky if you try adding a space here
                //formattedItems.Add(String.Format("[\"{2}{0}{2}\",\"{1}\"]", "", "", quoteFlag ? "\\\"" : ""));

                foreach (KeyValuePair<string, string> item in matches)
                {
                    formattedItems.Add(String.Format("[\"{2}{0}{2}\",\"{1}\"]", item.Value, item.Key, quoteFlag ? "\\\"" : ""));
                }
                Write(String.Join(", \n", formattedItems.ToArray()));

                Write("]");
            }
        }
    }

    #region Util functions

    /// <summary>
    /// Writes text to the response output.
    /// </summary>
    /// <param name="msg">The string to write</param>
    protected void Write(string msg)
    {
        Response.Write(msg);
    }

    /// <summary>
    /// Writes formatted text to the response output.
    /// </summary>
    /// <param name="msg">The formatted string to write</param>
    /// <param name="args">Parameters for msg</param>
    protected void Write(string msg, params object[] args)
    {
        Response.Write(string.Format(msg, args));
    }

    /// <summary>
    /// Writes a line of text (separated by an XHTML line break) to the output.
    /// </summary>
    /// <param name="msg">The string to write</param>
    protected void WriteLine(string msg)
    {
        Response.Write(msg + "<br />\n");
    }

    /// <summary>
    /// Writes a formatted line of text to the output.
    /// </summary>
    /// <param name="msg">The formatted string to write</param>
    /// <param name="args">Parameters for msg</param>
    protected void WriteLine(string msg, params object[] args)
    {
        Response.Write(string.Format(msg, args) + "<br />\n");
    }

    /// <summary>
    /// Writes a formatted error message to the output.
    /// </summary>
    /// <param name="msg">The formatted error message to write</param>
    /// <param name="args">Parameters for msg</param>
    protected new void Error(string msg, params object[] args)
    {
        Response.Write("ERROR: " + string.Format(msg, args) + "<br />\n");
    }

    #endregion
}