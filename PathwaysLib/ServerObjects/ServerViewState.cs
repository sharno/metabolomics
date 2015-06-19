#region Using Declarations
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web;

using PathwaysLib.SoapObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
using PathwaysLib.Utilities;
#endregion

namespace PathwaysLib.ServerObjects
{

    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/ServerObjects/ServerViewState.cs</filepath>
    ///		<creation>2005/07/18</creation>
    ///		<author>	
    ///			<name>Michael F. Starke</name>
    ///			<initials>mfs</initials>
    ///			<email>michael.starke@case.edu</email>
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
    ///			<cvs_date>$Date: 2010/05/13 20:43:49 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerViewState.cs,v 1.3 2010/05/13 20:43:49 sarp Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.3 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Encapsulates database access related to view states.
    /// </summary>
    #endregion
    public class ServerViewState : ServerObject
    {

        #region Constructor, Destructor, ToString
        private ServerViewState()
        {
        }

        /// <summary>
        /// ServerViewState constructor.
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
        public ServerViewState(string openSection, string organism, string openNode1ID, string openNode1Type, string openNode2ID, string openNode2Type, string openNode3ID, string openNode3Type, string displayItemID, string displayItemType, Tribool viewGraph)
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow(__TableName);

            OpenSection = openSection;
            Organism = organism;
            OpenNode1ID = new Guid(openNode1ID);
            OpenNode1Type = openNode1Type;
            OpenNode2ID = new Guid(openNode2ID);
            OpenNode2Type = openNode2Type;
            OpenNode3ID = new Guid(openNode3ID);
            OpenNode3Type = openNode3Type;
            DisplayItemID = new Guid(displayItemID);
            DisplayItemType = displayItemType;
            ViewGraph = viewGraph;
        }


        /// <summary>
        /// ServerViewState constructor.
        /// </summary>
        /// <param name="data">
        /// Soap item from which to copy data.
        /// </param>
        public ServerViewState(SoapViewState data)
        {
            // (mfs)
            // setup database row
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __DBRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __DBRow = LoadRow(data.ViewID);
                    break;
                default:
                    throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
            }

            // (mfs)
            // get potential updates from Soap object, unless it's
            // supposed to be read only
            if (data.Status != ObjectStatus.ReadOnly)
            {
                UpdateFromSoap(data);
            }
        }

        /// <summary>
        /// ServerViewState constructor.
        /// </summary>
        /// <param name="data">
        /// The database row that is the source of data.
        /// </param>
        public ServerViewState(DBRow data)
        {
            // (mfs)
            // setup object
            __DBRow = data;
            __OpenNode = new Hashtable();
            __OpenNodeType = new Hashtable();
            getViewstate_Nodes(__DBRow.GetGuid("viewid"));
        }

        /// <summary>
        /// ServerViewState destructor.
        /// </summary>
        ~ServerViewState()
        {
        }
        #endregion


        #region Member Variables
        /// <summary>
        /// The table name for this object.
        /// </summary>
        private static readonly string __TableName = "viewState";
        private static readonly string __TablenameDenormal = "viewstate_nodes";
        private static Hashtable __OpenNode;
        private static Hashtable __OpenNodeType;
        #endregion


        #region Properties
        /// <summary>
        /// Get/set the view state's id.
        /// </summary>
        public Guid ViewID
        {
            get
            {
                return __DBRow.GetGuid("viewID");
            }
            set
            {
                __DBRow.SetGuid("viewID", value);
            }
        }

        /// <summary>
        /// Get/set the view state's open section.
        /// </summary>
        public string OpenSection
        {
            get
            {
                return __DBRow.GetString("openSection");
            }
            set
            {
                __DBRow.SetString("openSection", value);
            }
        }

        /// <summary>
        /// Get/set the view state's filtering organism.
        /// </summary>
        public string Organism
        {
            get
            {
                return __DBRow.GetString("organism");
            }
            set
            {
                __DBRow.SetString("organism", value);
            }
        }

        /// <summary>
        /// Get/set the view state's open node 1 id.
        /// </summary>
        public Guid OpenNode1ID
        {
            get
            {
                return __DBRow.GetGuid("openNode1ID");
                // TRY TO GET THESE FROM THE HASHTABLE
               // return __OpenNode[1];
            }
            set
            {
                __DBRow.SetGuid("openNode1ID", value);
            }
        }

        /// <summary>
        /// Get/set the view state's open node 1 type.
        /// </summary>
        public string OpenNode1Type
        {
            get
            {
                return __DBRow.GetString("openNode1Type");
                //
                //return __OpenNodeType[1];
            }
            set
            {
                __DBRow.SetString("openNode1Type", value);
            }
        }

        /// <summary>
        /// Get/set the view state's open node 2 id.
        /// </summary>
        public Guid OpenNode2ID
        {
            get
            {
                return __DBRow.GetGuid("openNode2ID");
                // return __OpenNode[1];
            }
            set
            {
                __DBRow.SetGuid("openNode2ID", value);
            }
        }

        /// <summary>
        /// Get/set the view state's open node 2 type.
        /// </summary>
        public string OpenNode2Type
        {
            get
            {
                return __DBRow.GetString("openNode2Type");
                //return __OpenNodeType[1];
            }
            set
            {
                __DBRow.SetString("openNode2Type", value);
            }
        }

        /// <summary>
        /// Get/set the view state's open node 3 id.
        /// </summary>
        public Guid OpenNode3ID
        {
            get
            {
                return __DBRow.GetGuid("openNode3ID");
                // return __OpenNode[2];
            }
            set
            {
                __DBRow.SetGuid("openNode3ID", value);
            }
        }

        /// <summary>
        /// Get/set the view state's open node 3 type.
        /// </summary>
        public string OpenNode3Type
        {
            get
            {
                return __DBRow.GetString("openNode3Type");
                //return __OpenNodeType[3];
            }
            set
            {
                __DBRow.SetString("openNode3Type", value);
            }
        }

        /// <summary>
        /// Get/set the view state's display item id.
        /// </summary>
        public Guid DisplayItemID
        {
            get
            {
                return __DBRow.GetGuid("displayItemID");
            }
            set
            {
                __DBRow.SetGuid("displayItemID", value);
            }
        }

        /// <summary>
        /// Get/set the view state's display item type.
        /// </summary>
        public string DisplayItemType
        {
            get
            {
                return __DBRow.GetString("displayItemType");
            }
            set
            {
                __DBRow.SetString("displayITemType", value);

            }
        }

        /// <summary>
        /// Get/set the graph's visibility.
        /// </summary>
        public Tribool ViewGraph
        {
            get
            {
                return __DBRow.GetTribool("viewGraph");
            }
            set
            {
                __DBRow.SetTribool("viewGraph", value);
            }
        }
        /// <summary>
        /// Get the Open Nodes
        /// </summary>
        public Hashtable OpenNode
        {
            get
            {
                return __OpenNode;
            }
            set
            {
                __OpenNode = value;
            }

        }
        /// <summary>
        /// Get the Open Node Types
        /// </summary>
        public Hashtable OpenNodeType
        {
            get
            {
                return __OpenNodeType;
            }
            set
            {
                __OpenNodeType = value;
            }

        }
        #endregion


        #region Methods
        /// <summary>
        /// Returns a representation of this object suitable for being
        /// sent to a client via SOAP.
        /// </summary>
        /// <returns>
        /// A SoapObject object capable of being passed via SOAP.
        /// </returns>
        public override SoapObject PrepareForSoap(SoapObject derived)
        {
            SoapViewState retval = (derived == null) ? retval = new SoapViewState() : retval = (SoapViewState)derived;

            retval.ViewID = this.ViewID;
            retval.OpenNode1ID = this.OpenNode1ID;
            retval.OpenNode1Type = this.OpenNode1Type;
            retval.OpenNode2ID = this.OpenNode2ID;
            retval.OpenNode2Type = this.OpenNode2Type;
            retval.OpenNode3ID = this.OpenNode3ID;
            retval.OpenNode3Type = this.OpenNode3Type;
            retval.DisplayItemID = this.DisplayItemID;
            retval.DisplayItemType = this.DisplayItemType;
            retval.ViewGraph = this.ViewGraph;

            retval.Status = ObjectStatus.NoChanges;

            return retval;
        }

        /// <summary>
        /// Consumes a SoapObject object and updates the object
        /// from it.
        /// </summary>
        /// <param name="o">
        /// The SoapObject object to update from, potentially containing
        /// changes to the object.
        /// </param>
        protected override void UpdateFromSoap(SoapObject o)
        {
            SoapViewState v = o as SoapViewState;

            this.ViewID = v.ViewID;
            this.OpenNode1ID = v.OpenNode1ID;
            this.OpenNode1Type = v.OpenNode1Type;
            this.OpenNode2ID = v.OpenNode2ID;
            this.OpenNode2Type = v.OpenNode2Type;
            this.OpenNode3ID = v.OpenNode3ID;
            this.OpenNode3Type = v.OpenNode3Type;
            this.DisplayItemID = v.DisplayItemID;
            this.DisplayItemType = v.DisplayItemType;
            this.ViewGraph = v.ViewGraph;
        }

        #region ADO.NET SqlCommands
        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            // (BE) rewrote using BuildCommand()

            Guid newID = DBWrapper.NewID();
            __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (viewID, openSection, openNode1ID, openNode1Type, openNode2ID, openNode2Type, openNode3ID, openNode3Type, displayItemID, displayItemType, viewGraph ) VALUES (@viewid, @opensection, @opennode1id, @opennode1type, @opennode2id, @opennode2type, @opennode3id, @opennode3type, @displayitemiD, @displayitemtype, @viewgraph);",
                    "@viewid", SqlDbType.UniqueIdentifier, newID,
                    "@opensection", SqlDbType.VarChar, OpenSection,
                    "@opennode1id", SqlDbType.UniqueIdentifier, OpenNode1ID,
                    "@opennode1type", SqlDbType.VarChar, OpenNode1Type,
                    "@opennode2id", SqlDbType.UniqueIdentifier, OpenNode2ID,
                    "@opennode2type", SqlDbType.VarChar, OpenNode2Type,
                    "@opennode3id", SqlDbType.UniqueIdentifier, OpenNode3ID,
                    "@opennode3type", SqlDbType.VarChar, OpenNode3Type,
                    "@displayitemid", SqlDbType.UniqueIdentifier, DisplayItemID,
                    "@displayitemtype", SqlDbType.VarChar, DisplayItemType,
                    "@viewgraph", SqlDbType.Bit, ViewGraph
            );

            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE viewID = @viewID;",
                    "@viewID", SqlDbType.UniqueIdentifier, ViewID
            );

            __DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET openSection = @section, openNode1ID = @opennode1id, openNode1Type = @opennode1type, openNode2ID = @opennode2id, openNode2Type = @opennode2type, openNode3ID = @opennode3id, openNode3Type = @opennode3type, displayItemID = @displayitemid, displayItemType = @displayitemtype, viewGraph = @viewgraph WHERE viewID = @viewID;",
                    "@opensection", SqlDbType.VarChar, OpenSection,
                    "@opennode1id", SqlDbType.UniqueIdentifier, OpenNode1ID,
                    "@opennode1type", SqlDbType.VarChar, OpenNode1Type,
                    "@opennode2id", SqlDbType.UniqueIdentifier, OpenNode2ID,
                    "@opennode2type", SqlDbType.VarChar, OpenNode2Type,
                    "@opennode3id", SqlDbType.UniqueIdentifier, OpenNode3ID,
                    "@opennode3type", SqlDbType.VarChar, OpenNode3Type,
                    "@displayitemid", SqlDbType.UniqueIdentifier, DisplayItemID,
                    "@displayitemtype", SqlDbType.VarChar, DisplayItemType,
                    "@viewgraph", SqlDbType.Bit, ViewGraph,
                    "@viewid", SqlDbType.UniqueIdentifier, newID
            );

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE viewID = @viewID;",
                    "@viewID", SqlDbType.UniqueIdentifier, ViewID
            );
        }
        #endregion
        #endregion


        #region Static Methods
        private static DBRow LoadRow(Guid id)
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + " WHERE viewID = @id;");
            SqlParameter vid = new SqlParameter("@id", SqlDbType.UniqueIdentifier);
            vid.SourceVersion = DataRowVersion.Original;
            vid.Value = id;
            command.Parameters.Add(vid);

            DataSet ds = new DataSet();
            DBWrapper db = DBWrapper.Instance;
            int rc = db.ExecuteQuery(out ds, ref command);
            //DBWrapper.LoadSingle( out ds, ref command );
            if (rc != 1)
            {
                throw new LinkException("Error: Invalid view state. " + command.CommandText);
            }
            return new DBRow(ds);
        }

        /// <summary>
        /// Load a ServerViewState from its id.
        /// </summary>
        /// <param name="id">
        /// The id of the requested item.
        /// </param>
        /// <returns>
        /// The requested ServerViewState.
        /// </returns>
        public static ServerViewState Load(Guid id)
        {
            return new ServerViewState(LoadRow(id));
        }

        /// <summary>
        /// Get the link id based on the input criteria.
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
        /// The guid of the row to link to.
        /// </returns>
        public static Guid GetLinkID(string openSection, string organism, Guid openNode1ID, string openNode1Type, Guid openNode2ID, string openNode2Type, Guid openNode3ID, string openNode3Type, Guid displayItemID, string displayItemType, Tribool viewGraph)
        {
            Guid viewID = Guid.Empty;

            object[] argArray = {
				"@opensection", SqlDbType.VarChar, openSection,
				"@organism", SqlDbType.VarChar, organism,
				"@opennode1id", SqlDbType.UniqueIdentifier, openNode1ID,
				"@opennode1type", SqlDbType.VarChar, openNode1Type,
				"@opennode2id", SqlDbType.UniqueIdentifier, openNode2ID,
				"@opennode2type", SqlDbType.VarChar, openNode2Type,
				"@opennode3id", SqlDbType.UniqueIdentifier, openNode3ID,
				"@opennode3type", SqlDbType.VarChar, openNode3Type,
				"@displayitemid", SqlDbType.UniqueIdentifier, displayItemID,
				"@displayitemtype", SqlDbType.VarChar, displayItemType,
				"@viewgraph", SqlDbType.Bit, viewGraph == Tribool.True ? 1 : 0
			};
            // modify the command such that it searches both the tables 
            SqlCommand command = DBWrapper.BuildCommand(string.Format(
                @"SELECT viewID
					FROM " + __TableName + @"
					WHERE openSection {0} AND organism {1}
						AND openNode1ID {2} AND openNode1Type {3}
						AND openNode2ID {4} AND openNode2Type {5}
						AND openNode3ID {6} AND openNode3Type {7}
						AND displayItemID {8} AND displayItemType {9}
						AND viewGraph = @viewgraph",
                    (openSection == null ? "IS NULL" : "= @opensection"),
                    (organism == null ? "IS NULL" : "= @organism"),
                    (openNode1ID == Guid.Empty ? "IS NULL" : "= @opennode1id"),
                    (openNode1Type == null ? "IS NULL" : "= @opennode1type"),
                    (openNode2ID == Guid.Empty ? "IS NULL" : "= @opennode2id"),
                    (openNode2Type == null ? "IS NULL" : "= @opennode2type"),
                    (openNode3ID == Guid.Empty ? "IS NULL" : "= @opennode3id"),
                    (openNode3Type == null ? "IS NULL" : "= @opennode3type"),
                    (displayItemID == Guid.Empty ? "IS NULL" : "= @displayitemid"),
                    (displayItemType == null ? "IS NULL" : "= @displayitemtype")),
                    argArray);
            DBWrapper.Instance.TestConnection();
            object result = DBWrapper.Instance.ExecuteScalar(ref command);
            DBWrapper.Instance.Close();

            // (mfs)
            // if exists, return
            // else insert
            if (result != null)
            {
                return (Guid)result;
            }
            else
            {
                //Insert into the new table as well
                Guid newID = DBWrapper.NewID();
                object[] argArray2 = {
					"@viewid", SqlDbType.UniqueIdentifier, newID,
					"@opensection", SqlDbType.VarChar, openSection,
					"@organism", SqlDbType.VarChar, organism,
					"@opennode1id", SqlDbType.UniqueIdentifier, openNode1ID,
					"@opennode1type", SqlDbType.VarChar, openNode1Type,
					"@opennode2id", SqlDbType.UniqueIdentifier, openNode2ID,
					"@opennode2type", SqlDbType.VarChar, openNode2Type,
					"@opennode3id", SqlDbType.UniqueIdentifier, openNode3ID,
					"@opennode3type", SqlDbType.VarChar, openNode3Type,
					"@displayitemid", SqlDbType.UniqueIdentifier, displayItemID,
					"@displayitemtype", SqlDbType.VarChar, displayItemType,
					"@viewgraph", SqlDbType.Bit, viewGraph == Tribool.True ? 1 : 0
				};
                SqlCommand insert = DBWrapper.BuildCommand(
                    "INSERT INTO " + __TableName + @"
						(viewID, openSection, organism, openNode1ID, openNode1Type, openNode2ID, openNode2Type, openNode3ID, openNode3Type, displayItemID, displayItemType, viewGraph )
						VALUES (@viewid, @opensection, @organism, @opennode1id, @opennode1type, @opennode2id, @opennode2type, @opennode3id, @opennode3type, @displayitemiD, @displayitemtype, @viewgraph);",
                        argArray2);

                // this is where you insert in the alternate table


                DBWrapper.Instance.TestConnection();
                DBWrapper.Instance.ExecuteNonQuery(ref insert);
                DBWrapper.Instance.Close();
                return newID;
            }
        }

        public static ServerViewState GetViewStateOfModel(Guid modelId)
        {
            ArrayList results = new ArrayList();
            SqlCommand command = DBWrapper.BuildCommand(
            @"SELECT vs.* from
			    ViewState vs
				WHERE vs.[displayItemId] = @modelId AND openNode1Type='Model';",
            "@modelId", SqlDbType.UniqueIdentifier, modelId);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            foreach (DataSet d in ds)
            {
                results.Add(new ServerViewState(new DBRow(d)));
            }

            var svList = ((ServerViewState[])(results.ToArray(typeof(ServerViewState))));
            if (svList.Length > 0)
            {
                return svList[0];
            }
            else
            {
                return null;
            }
        }


        public static Guid GetLinkID2(string openSection, string organism, Hashtable OpenNode, Hashtable OpenNodeType, Guid displayItemID, string displayItemType, Tribool viewGraph)
        {
            Guid viewID = Guid.Empty;

            // need to add stuff here that checks and inserts in both tables etc

            object[] argArray = {
				"@opensection", SqlDbType.VarChar, openSection,
				"@organism", SqlDbType.VarChar, organism,				
				"@displayitemid", SqlDbType.UniqueIdentifier, displayItemID,
				"@displayitemtype", SqlDbType.VarChar, displayItemType,
				"@viewgraph", SqlDbType.Bit, viewGraph == Tribool.True ? 1 : 0
			};

            string qrystr = @"select viewstate.viewid from viewstate,  
                (
	                select viewid from 
	                (
		                select viewid ";
            /////////////////////////////
            foreach (DictionaryEntry de in OpenNode)
            {
                qrystr = qrystr +
                   ",MAX(CASE WHEN level = " + de.Key.ToString() + " THEN opennodeID ELSE NULL END)node" + de.Key.ToString()
                    + ",MAX(CASE WHEN level = " + de.Key.ToString() + " THEN opennodetype ELSE NULL END)node" + de.Key.ToString() + "type";

            }
            ///////////////////////////////////////////
            qrystr = qrystr + @" from viewstate_nodes
		                group by viewid Having ( max(level) = " + OpenNode.Count + @" ) 
		                ) normalviewids 
	                  ";
            ////////////////////////////////////////////////////
            bool first = true;
            foreach (DictionaryEntry de in OpenNode)
            {
                if (first)
                {
                    qrystr = qrystr + " Where node" + de.Key.ToString() + " = '" + OpenNode[de.Key] +
                "' AND node" + de.Key.ToString() + "type = '" + OpenNodeType[de.Key] + "'";
                    first = false;
                }
                else
                {
                    qrystr = qrystr + " AND node" + de.Key.ToString() + " = '" + OpenNode[de.Key] +
                    "' AND node" + de.Key.ToString() + "type = '" + OpenNodeType[de.Key] + "'";
                }
            }
            //////////////////////////////////////////////////////
            qrystr = qrystr + string.Format(@" )viewid
                where 
                viewid.viewid = viewstate.viewid
                AND opensection {0}
                AND organism {1}
                AND displayitemid {2}
                AND displayitemtype {3}
                AND viewGraph = @viewgraph ",
                (openSection == null ? "IS NULL" : "= @opensection"),
                (organism == null ? "IS NULL" : "= @organism"),
                (displayItemID == Guid.Empty ? "IS NULL" : "= @displayitemid"),
                (displayItemType == null ? "IS NULL" : "= @displayitemtype"));

            SqlCommand command = DBWrapper.BuildCommand(qrystr, argArray);

            // modify the command such that it searches bopthe the tables 
            //            SqlCommand command = DBWrapper.BuildCommand(string.Format(
            //                @"SELECT viewID
            //					FROM " + __TableName + @"
            //					WHERE openSection {0} AND organism {1}						
            //						AND displayItemID {8} AND displayItemType {9}
            //						AND viewGraph = @viewgraph",
            //                    (openSection == null ? "IS NULL" : "= @opensection"),
            //                    (organism == null ? "IS NULL" : "= @organism"),                  
            //                    (displayItemID == Guid.Empty ? "IS NULL" : "= @displayitemid"),
            //                    (displayItemType == null ? "IS NULL" : "= @displayitemtype")),
            //                    argArray);
            DBWrapper.Instance.TestConnection();
            object result = DBWrapper.Instance.ExecuteScalar(ref command);
            DBWrapper.Instance.Close();

            // (mfs)
            // if exists, return
            // else insert
            if (result != null)
            {
                return (Guid)result;
            }
            else
            {
                Guid newID = DBWrapper.NewID();
                object[] argArray2 = {
					"@viewid", SqlDbType.UniqueIdentifier, newID,
					"@opensection", SqlDbType.VarChar, openSection,
					"@organism", SqlDbType.VarChar, organism,
					"@opennode1id", SqlDbType.UniqueIdentifier, OpenNode.ContainsKey(1)? OpenNode[1]: Guid.Empty,
					"@opennode1type", SqlDbType.VarChar, OpenNodeType.ContainsKey(1)? OpenNodeType[1]: string.Empty ,
					"@opennode2id", SqlDbType.UniqueIdentifier,  OpenNode.ContainsKey(2)? OpenNode[2]: Guid.Empty,
					"@opennode2type", SqlDbType.VarChar,OpenNodeType.ContainsKey(2)? OpenNodeType[2]: string.Empty,
					"@opennode3id", SqlDbType.UniqueIdentifier,  OpenNode.ContainsKey(3)? OpenNode[3]: Guid.Empty,
					"@opennode3type", SqlDbType.VarChar,OpenNodeType.ContainsKey(3)? OpenNodeType[3]: string.Empty,
					"@displayitemid", SqlDbType.UniqueIdentifier, displayItemID,
					"@displayitemtype", SqlDbType.VarChar, displayItemType,
					"@viewgraph", SqlDbType.Bit, viewGraph == Tribool.True ? 1 : 0
				};
                SqlCommand insert = DBWrapper.BuildCommand(
                    "INSERT INTO " + __TableName + @"
						(viewID, openSection, organism, openNode1ID, openNode1Type, openNode2ID, openNode2Type, openNode3ID, openNode3Type, displayItemID, displayItemType, viewGraph )
						VALUES (@viewid, @opensection, @organism, @opennode1id, @opennode1type, @opennode2id, @opennode2type, @opennode3id, @opennode3type, @displayitemiD, @displayitemtype, @viewgraph);",
                        argArray2);
                DBWrapper.Instance.TestConnection();
                DBWrapper.Instance.ExecuteNonQuery(ref insert);
                foreach (DictionaryEntry de in OpenNode)
                {
                    int level = Convert.ToInt32(de.Key);
                    Guid node = (Guid)de.Value;
                    string nodetype = OpenNodeType[de.Key].ToString();
                    object[] argArray3 = {
					"@viewid", SqlDbType.UniqueIdentifier, newID,
					"@level",SqlDbType.Int,level,
					"@node", SqlDbType.UniqueIdentifier,node,
                        "@nodetype",SqlDbType.VarChar,nodetype};
                    SqlCommand insert2 = DBWrapper.BuildCommand(
                    "INSERT INTO " + __TablenameDenormal + @"
						(viewID,level,openNodeID,openNodeType)
						VALUES (@viewid,@level,@node,@nodetype );",
                        argArray3);

                    DBWrapper.Instance.ExecuteNonQuery(ref insert2);
                }
                DBWrapper.Instance.Close();
                return newID;
            }
        }
        #endregion

        # region Hashtable Methods


        private static void getViewstate_Nodes(Guid ID)
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TablenameDenormal + " WHERE viewID = @id;");
            SqlParameter vid = new SqlParameter("@id", SqlDbType.UniqueIdentifier);
            vid.SourceVersion = DataRowVersion.Original;
            vid.Value = ID;
            command.Parameters.Add(vid);

            DataSet ds = new DataSet();
            DBWrapper db = DBWrapper.Instance;
            int rc = db.ExecuteQuery(out ds, ref command);
            //DBWrapper.LoadSingle( out ds, ref command );
            if (rc >= 1) // otherwise Tables[0] would not exist.
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    __OpenNode[Convert.ToInt32(dr[1])] = dr[2];
                    __OpenNodeType[Convert.ToInt32(dr[1])] = dr[3];
                    //cant do this until we start inserting the viewstates
                    // throw new LinkException("Error: Invalid view state. " + command.CommandText);
                }
            }

        }


        #endregion

    } // End class

} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerViewState.cs,v 1.3 2010/05/13 20:43:49 sarp Exp $
	$Log: ServerViewState.cs,v $
	Revision 1.3  2010/05/13 20:43:49  sarp
	Rishi's nashuatest2 code.
	
	Revision 1.2  2010/04/17 02:05:11  rishi
	trying to keep the tree open
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.2  2006/10/19 21:03:36  brendan
	New graph drawing code ... performs bulk-loading of server objects to reduce the number of queries and filling an object cache.  Also provides an interface for alternative data sources (i.e. XML biopax doc).  Other misc bug fixes.
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.11  2006/05/18 19:01:47  greg
	 - Pathway menu expansion
	When going through the pathways menu in the control bar, the menu that should be open isn't.  Now it is.
	
	 - SQL injection stuff
	More queries were rewritten to prevent SQL injection, and some were also rewritten to be a little more aesthetically pleasing.
	
	 - Finding SQL bugs
	SQL bugs that have arisen as a result of schema changes are still being checked for.  Any that I find are being sent to Ali for rewriting and reintegration.
	
	Revision 1.10  2006/05/15 19:58:27  greg
	Handled exceptions thrown on invalid GUIDs and fixed some pagination issues (which also fixed some search issues as well).
	
	Revision 1.9  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.8  2005/08/24 22:27:52  michael
	Fixing SQL Connection Overflow bug
	
	Revision 1.7  2005/08/09 00:28:34  michael
	Adding organism selection to browser
	
	Revision 1.6  2005/08/08 20:13:38  michael
	Website content updates
	
	Revision 1.5  2005/07/25 23:23:17  michael
	stuff
	
	Revision 1.4  2005/07/25 20:56:20  michael
	Fixing/debugging:
	  ServerViewState
	  SoapViewState
	  LinkHelper
	
	Revision 1.3  2005/07/21 22:34:53  michael
	finalizing serverviewstate and link helper
	
	Revision 1.2  2005/07/21 19:14:42  michael
	fixing project files/compilation error
	
	Revision 1.1  2005/07/21 18:04:58  michael
	new viewstate object
------------------------------------------------------------------------*/
#endregion