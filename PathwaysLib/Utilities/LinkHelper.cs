using System;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.Utilities
{
    /// <summary>
    /// Summary description for LinkHelper.
    /// </summary>
    public class LinkHelper
    {
        /// <summary>
        /// LinkHelper constructor.
        /// </summary>
        /// <param name="req">
        /// Current HttpRequest object.
        /// </param>
        public LinkHelper(HttpRequest req)
        {
            if (DBWrapper.IsInstanceNull)
            {
                DBWrapper.Instance = new DBWrapper();
            }

            __req = req;

            if (__req.QueryString[__parameterName] != null)
            {
                __ViewState = ServerViewState.Load(new Guid(__req.QueryString[__parameterName]));
            }
            else
            {
                __ViewState = ServerViewState.Load(ServerViewState.GetLinkID("Browser", "All", Guid.Empty, null, Guid.Empty, null, Guid.Empty, null, Guid.Empty, null, Tribool.False));
            }

            __Params = new ListDictionary();
            __AllowedParams = new ArrayList();

            // Add the parameters from the query string into the parameter list
            foreach (string q in req.QueryString)
            {
                if (q.Length > 0 && !q.Equals(__parameterName))
                    __Params.Add(q, req.QueryString[q]);
            }
        }

        /// <summary>
        /// LinkHelper constructor.
        /// </summary>
        /// <param name="viewStateID">
        /// The id of the view state to load.
        /// </param>
        public LinkHelper(Guid viewStateID)
        {
            //DBWrapper.Instance = new DBWrapper();
            __Params = new ListDictionary();
            __AllowedParams = new ArrayList();

            __ViewState = ServerViewState.Load(viewStateID);
        }

        /// <summary>
        /// Destructor.  Closes dangling db connections (BE: is this correct behavior?? I assume added to try to fix a connection leak... Anyway, ASP.NET does connection pooling, so it probably doesn't matter).
        /// </summary>
        ~LinkHelper()
        {
            if (!DBWrapper.IsInstanceNull)
            {
                DBWrapper.Instance.Close();
            }
        }

        private HttpRequest __req;
        private ServerViewState __ViewState;
        private static ListDictionary __Params;
        private static ArrayList __AllowedParams;
        private static readonly string __parameterName = "viewID";
        private static readonly string[] __standardParams = { "terms", "type", "page", "pwgid" };

        /// <summary>
        /// Get the parameter name.
        /// </summary>
        public string ParameterName
        {
            get
            {
                return __parameterName;
            }
        }

        /// <summary>
        /// Get the Section.
        /// </summary>
        public string Section
        {
            get
            {
                return __ViewState.OpenSection;
            }
        }

        /// <summary>
        /// Get/set the organism.
        /// </summary>
        public string Organism
        {
            get
            {
                return __ViewState.Organism;
            }
            set
            {
                __ViewState.Organism = value;
            }
        }

        /// <summary>
        /// Get the open node 1 id
        /// </summary>
        public Guid OpenNode1ID
        {
            get
            {
                return __ViewState.OpenNode1ID;
            }
        }

        /// <summary>
        /// Get the open node 1 type
        /// </summary>
        public string OpenNode1Type
        {
            get
            {
                return __ViewState.OpenNode1Type;
            }
        }

        /// <summary>
        /// Get the open node 2 id
        /// </summary>
        public Guid OpenNode2ID
        {
            get
            {
                return __ViewState.OpenNode2ID;
            }
        }

        /// <summary>
        /// Get the open node 2 type
        /// </summary>
        public string OpenNode2Type
        {
            get
            {
                return __ViewState.OpenNode2Type;
            }
        }

        /// <summary>
        /// Get the open node 3 id
        /// </summary>
        public Guid OpenNode3ID
        {
            get
            {
                return __ViewState.OpenNode3ID;
            }
        }

        /// <summary>
        /// Get the open node 3 type
        /// </summary>
        public string OpenNode3Type
        {
            get
            {
                return __ViewState.OpenNode3Type;
            }
        }

        /// <summary>
        /// Get the display item id
        /// </summary>
        public Guid DisplayItemID
        {
            get
            {
                return __ViewState.DisplayItemID;
            }
        }

        /// <summary>
        /// Get the display item type
        /// </summary>
        public string DisplayItemType
        {
            get
            {
                return __ViewState.DisplayItemType;
            }
        }

        /// <summary>
        /// Get the graph's visibility.
        /// </summary>
        public Tribool ViewGraph
        {
            get
            {
                return __ViewState.ViewGraph;
            }
        }
        /// <summary>
        /// Get the Open Nodes
        /// </summary>
        public Hashtable OpenNode
        {
            get
            {
                return __ViewState.OpenNode;
            }
        }
        /// <summary>
        /// Get the Open Node Types
        /// </summary>
        public Hashtable OpenNodeType
        {
            get
            {
                return __ViewState.OpenNodeType;
            }
        }

        /// <summary>
        /// Get a string that is used for page redirects; this includes the viewID.
        /// </summary>
        /// <remarks>
        /// Note that the string will begin with "&amp;".
        /// </remarks>
        public string RedirectParams
        {
            get
            {
                string s = "&amp;" + __parameterName + "=" + GetParameter(__parameterName);
                foreach (DictionaryEntry de in __Params)
                    if (__AllowedParams.Count == 0 || __AllowedParams.Contains(de.Key))
                        s += "&amp;" + de.Key + "=" + de.Value;
                return s;
            }
        }


        /// <summary>
        /// Prepare a query string from the current view state.
        /// </summary>
        /// <param name="openSection">
        /// The section of the browser that is open.
        /// </param>
        /// <param name="organism">
        /// The filtering organism in use.
        /// </param>
        /// <param name="openNode1ID">
        /// The id of the top level open node.
        /// </param>
        /// <param name="openNode1Type">
        /// The type of the top level open node.
        /// </param>
        /// <param name="openNode2ID">
        /// The id of the second level open node.
        /// </param>
        /// <param name="openNode2Type">
        /// The type of the second level open node.
        /// </param>
        /// <param name="openNode3ID">
        /// The id of the third level open node.
        /// </param>
        /// <param name="openNode3Type">
        /// The type of the third level open node.
        /// </param>
        /// <param name="displayItemID">
        /// The id of the currently displayed item.
        /// </param>
        /// <param name="displayItemType">
        /// The type of the currently displayed item.
        /// </param>
        /// <returns>
        /// A query string linking to the requested combination.
        /// </returns>
        public static string PrepareQueryString(string openSection, string organism, Guid openNode1ID, string openNode1Type, Guid openNode2ID, string openNode2Type, Guid openNode3ID, string openNode3Type, Guid displayItemID, string displayItemType)
        {
            return LinkHelper.PrepareQueryString(openSection, organism, openNode1ID, openNode1Type, openNode2ID, openNode2Type, openNode3ID, openNode3Type, displayItemID, displayItemType, Tribool.Null);
        }

        /// <summary>
        /// Prepare a query string fromt he current view state.
        /// </summary>
        /// <param name="openSection">
        /// The section of the browser that is open.
        /// </param>
        /// <param name="organism">
        /// The filtering organism in use.
        /// </param>
        /// <param name="openNode1ID">
        /// The id of the top level open node.
        /// </param>
        /// <param name="openNode1Type">
        /// The type of the top level open node.
        /// </param>
        /// <param name="openNode2ID">
        /// The id of the second level open node.
        /// </param>
        /// <param name="openNode2Type">
        /// The type of the second level open node.
        /// </param>
        /// <param name="openNode3ID">
        /// The id of the third level open node.
        /// </param>
        /// <param name="openNode3Type">
        /// The type of the third level open node.
        /// </param>
        /// <param name="displayItemID">
        /// The id of the currently displayed item.
        /// </param>
        /// <param name="displayItemType">
        /// The type of the currently displayed item.
        /// </param>
        /// <param name="viewGraph">
        /// Should the graph be viewed?
        /// </param>
        /// <returns>
        /// A query string linking to the requested combination.
        /// </returns>
        public static string PrepareQueryString(string openSection, string organism, Guid openNode1ID, string openNode1Type, Guid openNode2ID, string openNode2Type, Guid openNode3ID, string openNode3Type, Guid displayItemID, string displayItemType, Tribool viewGraph)
        {
            string s = PathwaysWebBaseUrl + "/Web/?" + __parameterName + "=" + ServerViewState.GetLinkID(openSection, organism, openNode1ID, openNode1Type, openNode2ID, openNode2Type, openNode3ID, openNode3Type, displayItemID, displayItemType, viewGraph);

            // Add in any extra parameters that should be passed through
            foreach (DictionaryEntry de in __Params)
                if (__AllowedParams.Count == 0 || (__AllowedParams.Contains(de.Key) && de.Key.ToString() != __parameterName))
                    s += "&" + de.Key + "=" + de.Value;

            return s;
        }
        /// <summary>
        /// the alternate method for the query string
        /// </summary>
        /// <param name="openSection"></param>
        /// <param name="organism"></param>
        /// <param name="OpenNode"></param>
        /// <param name="OpenNodeType"></param>
        /// <param name="displayItemID"></param>
        /// <param name="displayItemType"></param>
        /// <param name="viewGraph"></param>
        /// <returns></returns>
        public static string PrepareQueryString2(string openSection, string organism, Hashtable OpenNode, Hashtable OpenNodeType, Guid displayItemID, string displayItemType, Tribool viewGraph)
        {
            string s = PathwaysWebBaseUrl + "/Web/?" + __parameterName + "=" + ServerViewState.GetLinkID2(openSection, organism, OpenNode, OpenNodeType, displayItemID, displayItemType, viewGraph);

            // Add in any extra parameters that should be passed through
            foreach (DictionaryEntry de in __Params)
                if (__AllowedParams.Count == 0 || (__AllowedParams.Contains(de.Key) && de.Key.ToString() != __parameterName))
                    s += "&" + de.Key + "=" + de.Value;

            return s;
        }



        /// <summary>
        /// PathwaysWeb.Utilities.UtilityFunctions.PathwaysWebBaseUrl
        /// </summary>
        /// 

        public static string PathwaysWebBaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["PathwaysWebBaseUrl"];
            }
        }

        /// <summary>
        /// The current query string.
        /// </summary>
        /// <returns>
        /// The current query string.
        /// </returns>
        public string QueryString()
        {
            return PrepareQueryString(__ViewState.OpenSection, __ViewState.Organism, __ViewState.OpenNode1ID, __ViewState.OpenNode1Type, __ViewState.OpenNode2ID, __ViewState.OpenNode2Type, __ViewState.OpenNode3ID, __ViewState.OpenNode3Type, __ViewState.DisplayItemID, __ViewState.DisplayItemType, __ViewState.ViewGraph);
        }

        /// <summary>
        /// Change to the requested section.
        /// </summary>
        /// <param name="newSection">
        /// The section to change.
        /// </param>
        /// <returns>
        /// Query string with section altered.
        /// </returns>
        public string AlterSection(string newSection)
        {
            return PrepareQueryString(newSection, __ViewState.Organism, __ViewState.OpenNode1ID, __ViewState.OpenNode1Type, __ViewState.OpenNode2ID, __ViewState.OpenNode2Type, __ViewState.OpenNode3ID, __ViewState.OpenNode3Type, __ViewState.DisplayItemID, __ViewState.DisplayItemType, __ViewState.ViewGraph);
        }

        /// <summary>
        /// Change the opened browser path.
        /// </summary>
        /// <param name="id1">
        /// Id of the open level 1 node.
        /// </param>
        /// <param name="type1">
        /// Type of the open level 1 node.
        /// </param>
        /// <param name="id2">
        /// Id of the open level 2 node.
        /// </param>
        /// <param name="type2">
        /// Type of the open level 2 node.
        /// </param>
        /// <param name="id3">
        /// Id of the open level 3 node.
        /// </param>
        /// <param name="type3">
        /// Type of the open level 3 node.
        /// </param>
        /// <returns>
        /// Query string with path altered.
        /// </returns>
        public string AlterOpenPath(Guid id1, string type1, Guid id2, string type2, Guid id3, string type3)
        {
            return PrepareQueryString(__ViewState.OpenSection, __ViewState.Organism, id1, type1, id2, type2, id3, type3, __ViewState.DisplayItemID, __ViewState.DisplayItemType, __ViewState.ViewGraph);
        }

        /// <summary>
        /// Change the displayed item.
        /// </summary>
        /// <param name="id">
        /// Id of the new display item.
        /// </param>
        /// <param name="type">
        /// Type of the new display item.
        /// </param>
        /// <returns>
        /// Query string with display item altered.
        /// </returns>
        public string AlterDisplayItem(Guid id, string type)
        {
            return PrepareQueryString(__ViewState.OpenSection, __ViewState.Organism, __ViewState.OpenNode1ID, __ViewState.OpenNode1Type, __ViewState.OpenNode2ID, __ViewState.OpenNode2Type, __ViewState.OpenNode3ID, __ViewState.OpenNode3Type, id, type, Tribool.Null);
        }

        /// <summary>
        /// Change the displayed item.
        /// </summary>
        /// <param name="id">
        /// Id of the new display item.
        /// </param>
        /// <param name="type">
        /// Type of the new display item.
        /// </param>
        /// <param name="viewGraph">
        /// Is the graph visible?
        /// </param>
        /// <returns>
        /// Query string with display item altered.
        /// </returns>
        public string AlterDisplayItem(Guid id, string type, Tribool viewGraph)
        {
            return PrepareQueryString(__ViewState.OpenSection, __ViewState.Organism, __ViewState.OpenNode1ID, __ViewState.OpenNode1Type, __ViewState.OpenNode2ID, __ViewState.OpenNode2Type, __ViewState.OpenNode3ID, __ViewState.OpenNode3Type, id, type, viewGraph);
        }

        /// <summary>
        /// RJ: store more than level 3. got to 3rd base now lets start scoring
        /// </summary>
        /// <param name="OpenNode">
        /// All the open nodes at any level
        /// </param>    
        /// <param name="OpenNodeType">
        /// Corresponding Types
        /// </param>   
        /// <param name="id">
        /// displayitem/clicked item
        /// </param>
        /// <param name="type">
        /// clicked itemtype
        /// </param>
        /// <param name="viewGraph">
        /// RJ:this would be one of those things I do not wanna get into so I leave it as it was
        /// </param>
        /// <returns></returns>
        public string AlterOpenPathAndDisplayItem2(Hashtable OpenNode, Hashtable OpenNodeType, Guid id, string type, Tribool viewGraph)
        {
            return PrepareQueryString2(__ViewState.OpenSection, __ViewState.Organism, OpenNode, OpenNodeType, id, type, viewGraph);
            //return PrepareQueryString( __ViewState.OpenSection, __ViewState.Organism, id1, type1, id2, type2, id3, type3, id, type, viewGraph );
        }
        /// <summary>
        /// Alters the open path and the display item.
        /// </summary>
        /// <param name="id1">
        /// Id of the open level 1 node.
        /// </param>
        /// <param name="type1">
        /// Type of the open level 1 node.
        /// </param>
        /// <param name="id2">
        /// Id of the open level 2 node.
        /// </param>
        /// <param name="type2">
        /// Type of the open level 2 node.
        /// </param>
        /// <param name="id3">
        /// Id of the open level 3 node.
        /// </param>
        /// <param name="type3">
        /// Type of the open level 3 node.
        /// </param>
        /// <param name="id">
        /// Id of the new display item.
        /// </param>
        /// <param name="type">
        /// Tyope of the new display item.
        /// </param>
        /// <param name="viewGraph">
        /// Is the graph visible?
        /// </param>
        /// <returns>
        /// Query string with path and display item changed.
        /// </returns>
        public string AlterOpenPathAndDisplayItem(Guid id1, string type1, Guid id2, string type2, Guid id3, string type3, Guid id, string type, Tribool viewGraph)
        {
            return PrepareQueryString(__ViewState.OpenSection, __ViewState.Organism, id1, type1, id2, type2, id3, type3, id, type, viewGraph);
        }

        /// <summary>
        /// Change section and null the path.
        /// </summary>
        /// <param name="section">
        /// The new section.
        /// </param>
        /// <returns>
        /// The query string with path nulled.
        /// </returns>
        public string AlterSectionNullifyPathNullifyDisplayItem(string section)
        {
            RemoveParameters();
            return PrepareQueryString(section, __ViewState.Organism, Guid.Empty, null, Guid.Empty, null, Guid.Empty, null, Guid.Empty, null);
        }


        /// <summary>
        /// Change display item and null the query parameters.
        /// </summary>
        /// <param name="id">The item to change</param>
        /// <param name="type">The item type</param>
        /// <returns>A query string</returns>
        public string AlterDisplayItemNullifyQueryParams(Guid id, string type)
        {
            SetStandardParameters();
            string retString = PrepareQueryString(__ViewState.OpenSection, __ViewState.Organism, __ViewState.OpenNode1ID, __ViewState.OpenNode1Type, __ViewState.OpenNode2ID, __ViewState.OpenNode2Type, __ViewState.OpenNode3ID, __ViewState.OpenNode3Type, id, type, Tribool.Null);
            ClearAllowedParameters();
            return retString;
        }

        /// <summary>
        /// Change the selected organism.
        /// </summary>
        /// <param name="organism">
        /// The new organism.
        /// </param>
        /// <returns>
        /// The query string with organism changed.
        /// </returns>
        public string AlterOrganism(string organism)
        {
            return PrepareQueryString(__ViewState.OpenSection, organism, __ViewState.OpenNode1ID, __ViewState.OpenNode1Type, __ViewState.OpenNode2ID, __ViewState.OpenNode2Type, __ViewState.OpenNode3ID, __ViewState.OpenNode3Type, __ViewState.DisplayItemID, __ViewState.DisplayItemType, __ViewState.ViewGraph);
        }

        /// <summary>
        /// Manually set the value of a parameter in the query string.
        /// </summary>
        /// <param name="param">
        /// The parameter to set.
        /// </param>
        /// <param name="val">
        /// The value to give the parameter.
        /// </param>
        public void SetParameter(object param, object val)
        {
            if (param.ToString() != __parameterName) __Params[param.ToString()] = val.ToString();
            else __ViewState = ServerViewState.Load(new Guid(val.ToString()));
        }

        /// <summary>
        /// Acquire set the value of a parameter in the query string.
        /// </summary>
        /// <param name="param">
        /// The parameter to get.
        /// </param>
        public string GetParameter(string param)
        {
            if (param == __parameterName)
            {
                return __ViewState.ViewID.ToString();
            }
            else
            {
                return __Params.Contains(param) ? __Params[param].ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Completely remove a parameter from the query string.
        /// </summary>
        /// <param name="param">
        /// The parameter to nuke.
        /// </param>
        /// <remarks>
        /// Nuking the viewstate parameter does not necessarily guarantee it won't show up later.
        /// </remarks>
        public void RemoveParameter(string param)
        {
            if (__Params.Contains(param)) __Params.Remove(param);
        }

        /// <summary>
        /// Remove all parameters from the parameter list
        /// </summary>
        public void RemoveParameters()
        {
            __Params.Clear();
        }

        /// <summary>
        /// Set the parameters that will be allowed into the query string on future generations.
        /// Be sure to use ClearAllowedParameters() to remove the filter when finished!
        /// </summary>
        /// <param name="a">
        /// An array of strings representing parameters that should be allowed into the query string.
        /// </param>
        public void SetAllowedParameters(params object[] a)
        {
            // Clear out any old parameters
            ClearAllowedParameters();

            // If a is null, then no parameters should be allowed through
            if (a == null)
            {
                __AllowedParams.Add(__parameterName);
            }
            else
            {
                foreach (string i in a)
                    if (!__AllowedParams.Contains(i.ToString())) __AllowedParams.Add(i.ToString());
            }
        }

        /// <summary>
        /// Set the allowed parameters to be "standard" ones that are defined by a private variable.
        /// </summary>
        public void SetStandardParameters()
        {
            SetAllowedParameters(__standardParams);
        }

        /// <summary>
        /// Clears the allowed parameters filter so anything can once again go through.
        /// </summary>
        public void ClearAllowedParameters()
        {
            __AllowedParams.Clear();
        }

        /// <summary>
        /// Fills a hashtable with allowed query string parameters.
        /// </summary>
        /// <param name="args">A hashtable with entries for the string parameters</param>
        public void ReconstructParameters(Hashtable args)
        {
            foreach (DictionaryEntry de in args)
            {
                if (__AllowedParams.Count == 0 || __AllowedParams.Contains(de.Key))
                    SetParameter(de.Key, de.Value);
            }
        }
    }
}