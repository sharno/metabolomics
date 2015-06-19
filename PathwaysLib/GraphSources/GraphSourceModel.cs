using System;
using PathwaysLib.ServerObjects;
//using PathwaysLib.SBObjects;
using System.Collections;
using System.Drawing;
using System.Data;
using PathwaysLib.Utilities;
using System.Configuration;

namespace PathwaysLib.GraphSources
{
	/// <summary>
	/// IGraphSource implementation for processes
	/// </summary>
    //public class GraphSourceModel 
    //{
    public class GraphSourceModel : IGraphSource
    {
        private PathwaysLib.ServerObjects.ServerModel sModel;
        private Guid[] models;
        private ServerProcess[] sProcesses;
        private ServerProcess[] keggProcesses;
        private ServerProcess[] biomodelProcesses;
        private ServerProcess[] commonProcesses;
        //private Hashtable expandedPwIds = new Hashtable();
        //private Hashtable collapsedPwIds = new Hashtable();
        //private Hashtable genProcessIds = new Hashtable();
        //private Hashtable genBiomodelProcessIds = new Hashtable();
        //private Hashtable genKeggProcessIds = new Hashtable();
        //private Hashtable genCommonProcessIds = new Hashtable();
        //private Hashtable moleculeIds = new Hashtable();
        //private Hashtable keggMoleculeIds = new Hashtable();
        //private Hashtable biomodelMoleculeIds = new Hashtable();
        //private Hashtable commonMoleculeIds = new Hashtable();
        private string graphType = string.Empty;
        //private Hashtable networkRows = new Hashtable();
        private string sourceNode = "";
        private string sourceNodeName = "";
        private GraphContent sGraphContent = GraphContent.Model;
       
        //public LinkHelper LH;

        ///// <summary>
        ///// Default constructor
        ///// </summary>
        public GraphSourceModel(Guid modelid, string type,string contenttype)
        {
            if (contenttype != null && contenttype.Equals("usermodel"))
            {
                String strDB = ConfigurationManager.AppSettings.Get("dbUserUploadsConnectString");
                DBWrapper.Instance = new DBWrapper(strDB);
                sGraphContent = GraphContent.UserModel;
            }
            
            Model = ServerModel.Load(modelid);
            //sProcesses = Model.GetAllProcesses();
            //keggProcesses = Model.GetAllKeggProcesses();
            //biomodelProcesses = Model.GetAllBiomodelsProcesses();
            //commonProcesses = Model.GetCommonProcesses();
            graphType = type;
            models = new Guid[1];
            models[0] = sModel.ID;
        }

        public GraphSourceModel(Guid modelid, string type, Guid processid, string processname)
        {
            Model = ServerModel.Load(modelid);
            models = new Guid[1];
            models[0] = sModel.ID;
            
            sProcesses = Model.GetAllProcesses();
            commonProcesses = Model.GetCommonProcesses();
            keggProcesses = Model.GetAllKeggProcesses();
            biomodelProcesses = Model.GetAllBiomodelsProcesses();
            sourceNode = processid.ToString();
            sourceNodeName = processname;
            graphType = type;
            //Initialize();
        }

        //private void Initialize()
        //{
        //    //ServerModel sm = ServerModel.Load(this.Model.ID);
        //    //sm.GetAllMolecules();
        //    //LH.SetStandardParameters();

        //    ServerProcessEntity[] spes = PathwaysLib.Queries.ModelQueries.GetReactionNetworkData(this.Model.ID.ToString());

        //    #region
        //    //DataTable ReactionNetworkTable = new DataTable();
        //    //ReactionNetworkTable.Columns.Add("Reaction 1", typeof(string));
        //    //ReactionNetworkTable.Columns.Add("Molecular Entity", typeof(string));
        //    //ReactionNetworkTable.Columns.Add("Role in Reaction 1", typeof(string));

        //    //DataRow dr;
        //    //Guid tempId;
        //    //ServerProcess sp;
        //    #endregion

        //    foreach (ServerProcess sp in sProcesses)
        //    {
        //        genProcessIds.Add(sp.ID.ToString(), sp.ID.ToString());
        //    }
        //    foreach (ServerProcess sp in biomodelProcesses)
        //    {
        //        genBiomodelProcessIds.Add(sp.ID.ToString(), sp.ID.ToString());
        //    }
        //    foreach (ServerProcess sp in keggProcesses)
        //    {
        //        genKeggProcessIds.Add(sp.ID.ToString(), sp.ID.ToString());
        //    }
        //    foreach (ServerProcess sp in commonProcesses)
        //    {
        //        genCommonProcessIds.Add(sp.ID.ToString(), sp.ID.ToString());
        //    }
        //    foreach (ServerProcessEntity spe in spes)
        //    {
        //        string molid = spe.EntityID.ToString();

        //        string mol_source = ServerBasicMolecule.WhatIsTheSource(molid);

        //        if (mol_source.Contains("biomodel"))
        //        {
        //            if (!biomodelMoleculeIds.Contains(molid))
        //                biomodelMoleculeIds.Add(molid, molid);
        //        }
        //        else if (mol_source.Contains("kegg"))
        //        {
        //            if (!keggMoleculeIds.Contains(molid))
        //                keggMoleculeIds.Add(molid, molid);
        //        }
        //        else if (!commonMoleculeIds.Contains(molid)) // common
        //            commonMoleculeIds.Add(molid, molid);

        //        if (!moleculeIds.Contains(molid)) // all
        //            moleculeIds.Add(molid, molid);
        //    }
        //    #region unused... for now! dont remove
        //    //foreach (DataSet dsItem in ds)
        //    //{
        //        //DataRow tempDr = dsItem.Tables[0].Rows[0];
        //        //dr = ReactionNetworkTable.NewRow();
        //        //string key = PathwaysLib.Queries.ModelQueries.HashKeyProcuder(tempDr["gen_pro_id_1"].ToString(),
        //        //                                                                tempDr["connecting_mol_id"].ToString(),
        //        //                                                                tempDr["role_in_1"].ToString(),
        //        //                                                                tempDr["gen_pro_id_2"].ToString(),
        //        //                                                                tempDr["role_in_2"].ToString());

        //        //string molid = tempDr["connecting_mol_id"].ToString();

        //        //if (!moleculeIds.Contains(molid))
        //        //    moleculeIds.Add(molid, molid);

        //        //if (!networkRows.ContainsKey(key))
        //        //{
        //        //    networkRows.Add(key, key);
        //        //    dr["Reaction 1"] = HyperlinkBuilder.constructProcessHyperlink(LH, tempDr["gen_pro_id_1"].ToString(), tempDr["gen_pro_name_1"].ToString());
        //        //    dr["Molecular Entity"] = HyperlinkBuilder.constructMoleculeHyperlink(LH, tempDr["connecting_mol_id"].ToString(), tempDr["connecting_mol_name"].ToString());
        //        //    dr["Role in Reaction 1"] = tempDr["role_in_1"].ToString();
        //        //    dr["Reaction 2"] = HyperlinkBuilder.constructProcessHyperlink(LH, tempDr["gen_pro_id_2"].ToString(), tempDr["gen_pro_name_2"].ToString());
        //        //    dr["Role in Reaction 2"] = tempDr["role_in_2"].ToString();
        //        //    ReactionNetworkTable.Rows.Add(dr);
        //        //    string id1 = tempDr["gen_pro_id_1"].ToString(),
        //        //            id2 = tempDr["gen_pro_id_2"].ToString();

        //        //    if (!genProcessIds.Contains(id1))
        //        //        genProcessIds.Add(id1, id1);

        //        //    if (!genProcessIds.Contains(id2))
        //        //        genProcessIds.Add(id2, id2);

        //        //    string molid = tempDr["connecting_mol_id"].ToString();

        //        //    if (!moleculeIds.Contains(molid))
        //        //        moleculeIds.Add(molid, molid);
        //        //    if (sourceNode == null)
        //        //    {
        //        //        sourceNode = tempDr["gen_pro_id_1"].ToString();
        //        //        //sourceProcess = sourceNode;
        //        //    }
        //        //}
        //    //}
        //    #endregion
        //}

        public PathwaysLib.ServerObjects.ServerModel Model
        {
            get { return sModel; }
            set { sModel = value; }
        }

        //#region IGraphSource members
        /// <summary>
        /// A list of the IDs of collapsed models nodes to add to the graph.
        /// </summary>
        public Guid[] CollapsedModels
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// A list of the IDs of fully expanded models to add to the graph (i.e. add all the reactions in the model)
        /// </summary>
        public Guid[] ExpandedModels
        {
            get
            {
                return models;
            }
        }

        /// <summary>
        /// A list of the IDs of reactions to add to the graph.
        /// </summary>
        public Guid[] ReactionIDs
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// A lsit of the IDs of species to add to the graph.
        /// </summary>
        public Guid[] SpeciesIDs
        {
            get
            {
                return null;
            }
        }


        /// <summary>
        /// Specifies the type of the graph content. 
        /// </summary>
        public GraphContent ContentType
        {
            get
            {
                return sGraphContent;
            }
            set
            {
                sGraphContent = value;
            }
        }
        ///// <summary>
        ///// Get collapsed pathways
        ///// </summary>
        public Guid[] CollapsedPathways
        {
            get { return null; }
        }

        public String CompHValue
        {
            get { return "neutral"; }
        }

        ///// <summary>
        ///// Get expanded pathways
        ///// </summary>
        public Guid[] ExpandedPathways
        {
            get { return null; }
        }



        ///// <summary>
        ///// Get generic processes
        ///// </summary>
        public Guid[] GenericProcessGraphIDs
        {
            get 
            {
        //        if (genProcessIds.Count>0){
        //            IDictionaryEnumerator enu = genProcessIds.GetEnumerator();
        //            Guid[] ids = new Guid[genProcessIds.Count];
        //            int i = 0;
        //            while(enu.MoveNext()) {
        //                ids[i++] = GraphNodeManager.GetAnyProcessGraphNodeId(new Guid(enu.Entry.Value.ToString()));
        //            }
        //            return ids;
        //        }
        //        else if (sProcesses.Length > 0)
        //        {
        //            IEnumerator enu = sProcesses.GetEnumerator();
        //            Guid[] ids = new Guid[sProcesses.Length];
        //            int i = 0;
        //            while (enu.MoveNext())
        //            {
        //                ids[i++] = GraphNodeManager.GetAnyProcessGraphNodeId(((ServerProcess)enu.Current).ID);
        //            }
        //            return ids;
        //        }
        //        else
                return null;
            }
        }

        ///// <summary>
        ///// Get molecules
        ///// </summary>
        public Guid[] MoleculeGraphIDs
        {
            get
            {
        //        //return null;
        //        if (moleculeIds.Count > 0)
        //        {
        //            IDictionaryEnumerator enu = moleculeIds.GetEnumerator();
        //            Guid[] ids = new Guid[moleculeIds.Count];

        //            int i = 0;
        //            while (enu.MoveNext())
        //            {
        //                ids[i] = GraphNodeManager.GetAnyEntityGraphNodeId(new Guid(enu.Entry.Value.ToString()));
        //                i++;
        //            }
        //            return ids;
        //        }
                //else 
                return null;
            }
        }

        ///// <summary>
        ///// Whether this is a hierarchical layout or not
        ///// </summary>
        public GraphLayout Layout
        {
            // TODO: Add GraphSourceProcess.HierarchicalLayout implementation
            get { return GraphLayout.Organic; }
        }

        ///// <summary>
        ///// Get the initial organism
        ///// </summary>
        public string InitialOrganism
        {
            // TODO: Add GraphSourceProcess.InitialOrganism implementation
            get { return string.Empty; }
        }

        ///// <summary>
        ///// Get the graph title
        ///// </summary>
        public string GraphTitle
        {
            get {return "Interactive Model Graph";} 
            //get { return "Model Reaction Network"; }
        }

        ///// <summary>
        ///// Get the graph type
        ///// </summary>
        public string GraphType
        {
            get
            {
                return graphType;
            }
        }

        ///// <summary>
        ///// Get coloring information
        ///// </summary>
        public GraphColoring[] Colorings
        {
            get
            {
        //        GraphColoring[] gc = null;
        //        int i = 0;
        //        if (sourceNodeName == "")
        //        {
        //            //return null;   
        //            gc = new GraphColoring[biomodelProcesses.Length + keggProcesses.Length + commonProcesses.Length + biomodelMoleculeIds.Count + keggMoleculeIds.Count + commonMoleculeIds.Count];
        //        }
        //        if (biomodelProcesses.Length > 0 || keggProcesses.Length > 0 || commonProcesses.Length > 0)
        //        {
        //            if (gc == null) // there is source
        //            {
        //                gc = new GraphColoring[biomodelProcesses.Length + keggProcesses.Length + commonProcesses.Length + biomodelMoleculeIds.Count + keggMoleculeIds.Count + commonMoleculeIds.Count + 1];
        //                gc[0] = new GraphColoring(new Guid(sourceNode), Color.OrangeRed, "Source Reaction (" + sourceNodeName.ToString() + ")");
        //                i = 1;
        //            }

        //            if (i != 1) i = 0;
        //            IDictionaryEnumerator enu = genBiomodelProcessIds.GetEnumerator();
        //            while (enu.MoveNext())
        //            {
        //                if (enu.Entry.Value.ToString() != sourceNode)
        //                {
        //                    gc[i++] = new GraphColoring(GraphNodeManager.GetAnyEntityGraphNodeId(new Guid(enu.Entry.Value.ToString())), Color.AliceBlue,
        //                        "BioModels reactions of the model");
        //                }
        //            }

        //            enu = genKeggProcessIds.GetEnumerator();
        //            while (enu.MoveNext())
        //            {
        //                if (enu.Entry.Value.ToString() != sourceNode)
        //                {
        //                    gc[i++] = new GraphColoring(GraphNodeManager.GetAnyEntityGraphNodeId(new Guid(enu.Entry.Value.ToString())), Color.Moccasin,
        //                        "Kegg reactions");
        //                }
        //            }
                    
        //            enu = genCommonProcessIds.GetEnumerator();
        //            while (enu.MoveNext())
        //            {
        //                if (enu.Entry.Value.ToString() != sourceNode)
        //                {
        //                    gc[i++] = new GraphColoring(GraphNodeManager.GetAnyEntityGraphNodeId(new Guid(enu.Entry.Value.ToString())), Color.Coral,
        //                        "Common reactions that exist in both Kegg and BioModels database");
        //                }
        //            }

        //            enu = biomodelMoleculeIds.GetEnumerator();
        //            while (enu.MoveNext())
        //            {
        //                gc[i++] = new GraphColoring(GraphNodeManager.GetAnyEntityGraphNodeId(new Guid(enu.Entry.Value.ToString())), Color.Yellow,
        //                    "BioModels molecular entities");
        //            }

        //            //if (biomodelMoleculeIds.Count > 0)
        //            //    gc[i++] = new GraphColoring(Guid.Empty, Color.Yellow, "Biomodels molecular entities");

        //            enu = keggMoleculeIds.GetEnumerator();
        //            while (enu.MoveNext())
        //            {
        //                gc[i++] = new GraphColoring(GraphNodeManager.GetAnyEntityGraphNodeId(new Guid(enu.Entry.Value.ToString())), Color.LightBlue,
        //                    "Kegg molecular entities");
        //            }

        //            //if (keggMoleculeIds.Count > 0)
        //            //    gc[i++] = new GraphColoring(Guid.Empty, Color.LightBlue, "Kegg molecular entities");

        //            enu = commonMoleculeIds.GetEnumerator();
        //            while (enu.MoveNext())
        //            {
        //                gc[i++] = new GraphColoring(GraphNodeManager.GetAnyEntityGraphNodeId(new Guid(enu.Entry.Value.ToString())), Color.Chartreuse,
        //                    "Common molecular entities that exist in both Kegg and BioModels database");
        //            }

        //            //if (commonMoleculeIds.Count > 0)
        //            //    gc[i++] = new GraphColoring(Guid.Empty, Color.LightGreen, "Common molecular entities");
        //            return gc;
        //        }
        //        #region
        //        //else if (genProcessIds.Count > 0)
        //        //{
        //        //    GraphColoring[] gc2 = new GraphColoring[genProcessIds.Count + moleculeIds.Count + 1];

        //        //    gc2[0] = new GraphColoring(new Guid(sourceNode), Color.OrangeRed, "Source Reaction (" + sourceNodeName.ToString() + ")");

        //        //    int i = 1;
        //        //    IDictionaryEnumerator enu = genProcessIds.GetEnumerator();
        //        //    while (enu.MoveNext())
        //        //    {
        //        //        if (enu.Entry.Value.ToString() != sourceNode)
        //        //        {
        //        //            gc2[i++] = new GraphColoring(GraphNodeManager.GetAnyEntityGraphNodeId(new Guid(enu.Entry.Value.ToString())), Color.Chartreuse,
        //        //                "Reactions of the model");
        //        //        }
        //        //    }

        //        //    enu = moleculeIds.GetEnumerator();
        //        //    while (enu.MoveNext())
        //        //    {
        //        //        gc2[i++] = new GraphColoring(GraphNodeManager.GetAnyEntityGraphNodeId(new Guid(enu.Entry.Value.ToString())), Color.Yellow,
        //        //            "Linking molecular entities");
        //        //    }
        //        //    return gc2;
        //        //}
        //        #endregion
        //        else
                return null;
            }
        }

        ///// <summary>
        ///// Display color legend at the bottom of the graph.
        ///// </summary>
        public bool LegendVisible
        {
            get { return true; }
        }

        ///// <summary>
        ///// The source node in the graph
        ///// </summary>
        public Guid SourceNode
        {
            get
            {
                if (sourceNode == "")
                    return Guid.Empty;
                else return new Guid(sourceNode);
            }
        }

        //#endregion
    }
}