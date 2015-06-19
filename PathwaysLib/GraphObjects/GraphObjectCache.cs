using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//using PathwaysLib.ServerObjects;
using PathwaysLib.GraphObjects;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using PathwaysLib.Utilities;
using PathwaysLib.ServerObjects;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace PathwaysLib.GraphObjects
{
    public abstract class GraphObjectCache
    {
        public GraphObjectCache()
        {
            ClearCache();
        }
        protected bool explicitMoleculesAdded = false;        
        protected Dictionary<Guid, Guid> SpecificToGenericProcessIdTable = null;
        protected Dictionary<Guid, List<Guid>> GenericToSpecificProcessIdTable = null;
        protected Dictionary<Guid, List<IGraphProcessEntity>> GenericProcessEntities = null;
        //protected Dictionary<Guid, List<IGraphGeneProduct>> GenericProcessGeneProducts = null; // gene products are molecules!
        protected Dictionary<Guid, List<string>> GenericProcessECNumbers = null;
        protected Dictionary<Guid, List<Guid>> GenericProcessGeneProductMappings = null;
        protected Dictionary<Guid, List<Guid>> GenericProcessOrgGroups = null;
        protected Dictionary<Guid, IGraphGenericProcess> GenericProcesses = null;
        protected Dictionary<Guid, Guid> GenericProcessGraphNodes = null;
        protected Dictionary<Guid, List<Guid>> GenericProcessGraphNodePathways = null;

        protected Dictionary<Guid, IGraphMolecularEntity> Molecules = null;
        protected Dictionary<Guid, Guid> MoleculeGraphNodes = null;
        protected Dictionary<Guid, Guid> ExplicitMoleculeGraphNodes = null;
        protected Dictionary<Guid, List<Guid>> MoleculeGraphNodePathways = null;

        protected Dictionary<String, ServerMapSpeciesMolecularEntities> SpeciesMappings = null;
        protected Dictionary<String, ServerMapReactionsProcessEntities> ReactionMappings = null;
        //protected Dictionary<Guid, IReactionsMapping> ReactionMappings = null;
        protected List<Guid> MolIDsInPW = null;
        //protected Dictionary<Guid, List<Guid>> SpIDsInModel = null;
        protected Dictionary<Guid, List<Guid>> SpIDsInMapping = null;

        protected Dictionary<Guid, List<Guid>> PathwayOrgGroups = null;
        protected Dictionary<Guid, IGraphPathway> CollapsedPathways = null;
        protected Dictionary<Guid, IGraphPathway> ExpandedPathways = null;
        protected Dictionary<Guid, IGraphPathway> CollapsedLinkingPathways = null;
        protected Dictionary<Guid, List<Guid>> PathwaysToGenericProcesses = null;
        protected Dictionary<Guid, Dictionary<Guid, List<Guid>>> LinkingPathwaysAndMoleculeGraphNodes = null;
        protected Dictionary<Guid, Dictionary<Guid, List<Guid>>> LinkingInPathwaysAndMoleculeGraphNodes = null;
        protected bool includeTransportProcesses = false;
        protected List<Guid> additionalTransportProcesses = null;
        protected Dictionary<Guid, string> transportProcessTissue = null; //utility dictionary to indicate the tissue
        // added for load data function
        protected Dictionary<Guid, List<IGraphCatalyze>> GenericProcessCatalyzes = null;
        protected Dictionary<Guid, string> molIDTissNameMap = null;
        protected Dictionary<Guid, List<Guid>> tissueAwareMoleculeGraphNodes = null;
        protected Dictionary<Guid, List<Guid>> transportProcessGraphNodes = null;
        protected Dictionary<Guid, List<Guid>> tissueAwareProcessGraphNodes = null;
        //protected Dictionary<Guid, List<Guid>> tissueAwareMoleculeGraphNodes = null;
        protected Dictionary<Guid, IGraphCompartment> Compartments = null;
        protected Dictionary<Guid, IGraphModel> Models = null;
        protected Dictionary<Guid, IGraphPathway> MappingPathways = null;
        protected Dictionary<Guid, List<IGraphSpecies>> Species = null;
        protected Dictionary<Guid, IGraphReaction> Reactions = null;
        protected Dictionary<Guid, List<IGraphReactionSpecies>> ReactionSpecies = null;
        protected ArrayList commonSpecieslist =null;
        
        public void ClearCache()
        {
            SpecificToGenericProcessIdTable = new Dictionary<Guid, Guid>();
            GenericToSpecificProcessIdTable = new Dictionary<Guid, List<Guid>>();
            GenericProcessEntities = new Dictionary<Guid, List<IGraphProcessEntity>>();
            //GenericProcessGeneProducts = new Dictionary<Guid, List<IGraphGeneProduct>>();
            GenericProcessECNumbers = new Dictionary<Guid, List<string>>();
            GenericProcessGeneProductMappings = new Dictionary<Guid, List<Guid>>();
            GenericProcessOrgGroups = new Dictionary<Guid, List<Guid>>();
            GenericProcesses = new Dictionary<Guid, IGraphGenericProcess>();
            GenericProcessGraphNodes = new Dictionary<Guid, Guid>();
            GenericProcessGraphNodePathways = new Dictionary<Guid, List<Guid>>();

            Molecules = new Dictionary<Guid, IGraphMolecularEntity>();
            MoleculeGraphNodes = new Dictionary<Guid, Guid>();
            ExplicitMoleculeGraphNodes = new Dictionary<Guid, Guid>();
            MoleculeGraphNodePathways = new Dictionary<Guid, List<Guid>>();

            PathwayOrgGroups = new Dictionary<Guid, List<Guid>>();
            CollapsedPathways = new Dictionary<Guid, IGraphPathway>();
            ExpandedPathways = new Dictionary<Guid, IGraphPathway>();
            CollapsedLinkingPathways = new Dictionary<Guid, IGraphPathway>();
            PathwaysToGenericProcesses = new Dictionary<Guid, List<Guid>>();
            LinkingPathwaysAndMoleculeGraphNodes = new Dictionary<Guid, Dictionary<Guid, List<Guid>>>();
            LinkingInPathwaysAndMoleculeGraphNodes = new Dictionary<Guid, Dictionary<Guid, List<Guid>>>();

            SpeciesMappings = new Dictionary<String, ServerMapSpeciesMolecularEntities>();
            ReactionMappings = new Dictionary<string, ServerMapReactionsProcessEntities>();
            //ReactionMappings = new Dictionary<Guid, Guid>();
            MolIDsInPW = new List<Guid>();
            //SpIDsInModel = new Dictionary<Guid, List<Guid>>();
            SpIDsInMapping = new Dictionary<Guid, List<Guid>>();

            GenericProcessCatalyzes = new Dictionary<Guid, List<IGraphCatalyze>>();

            Compartments = new Dictionary<Guid, IGraphCompartment>();
            Models = new Dictionary<Guid, IGraphModel>();
            MappingPathways = new Dictionary<Guid, IGraphPathway>();
            Species = new Dictionary<Guid, List<IGraphSpecies>>();
            Reactions = new Dictionary<Guid, IGraphReaction>();
            ReactionSpecies = new Dictionary<Guid, List<IGraphReactionSpecies>>();
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }

        //BE: the following 4 methods are to be overridden
        public virtual Guid MoleculeIdToGraphNodeId(Guid pathwayId, Guid moleculeId)
        {
            return moleculeId;
        }

        public virtual Guid GenericProcessIdToGraphNodeId(Guid pathwayId, Guid genericProcessId)
        {
            return genericProcessId;
        }

        public virtual Guid GetPathwayForProcessGraphNode(Guid genericProcessGraphNodeId)
        {
            return Guid.Empty;
        }

        public virtual Guid GetPathwayForMoleculeGraphNode(Guid moleculeGraphNodeId)
        {
            return Guid.Empty;
        }

        #region XML generation

        /// <summary>
        /// Generate System Biology Model xml.
        /// By Xinjian Qi 03/09/09
        /// </summary>
        /// <returns></returns>
        public XmlDocument GenerateSBModelXml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("SBModel");
            doc.AppendChild(root);

            XmlElement mod = doc.CreateElement("Model");
            root.AppendChild(mod);
            if (Models.Count == 1)
            {
                foreach (Guid graphNodeId in Models.Keys)
                {
                    IGraphModel me = Models[graphNodeId];
                    //doc.AppendChild(GenerateModelXml(mod, graphNodeId, me));
                    XmlUtil.AddAttribute(mod, "ID", me.ID);
                    XmlUtil.AddAttribute(mod, "Name", me.Name);
                }            
            }

            XmlElement compsXML = doc.CreateElement("Compartments");
            root.AppendChild(compsXML);

            if (Compartments.Count > 0)
            {
                foreach (Guid graphNodeId in Compartments.Keys)
                    {
                        IGraphCompartment me = Compartments[graphNodeId];
                        XmlElement compXML = doc.CreateElement("Compartment");
                        compsXML.AppendChild(compXML);
                        GenerateCompartmentXml(compXML, graphNodeId, me);
                        if(Species.Count>0 && Species.ContainsKey(graphNodeId))
                        {
                            XmlElement specsXml = doc.CreateElement("SpeciesAll");
                            compXML.AppendChild(specsXml);
                            foreach (IGraphSpecies cat in Species[graphNodeId])
                            {
                                specsXml.AppendChild(GenerateSpeciesXml(doc, graphNodeId, cat));
                            }
                        }
                    }
            }

            XmlElement reacsXML = doc.CreateElement("Reactions");
            root.AppendChild(reacsXML);

            if (Reactions.Count > 0)
            {
                foreach (Guid graphNodeId in Reactions.Keys)
                {
                    IGraphReaction me = Reactions[graphNodeId];
                    XmlElement reacXML = doc.CreateElement("Reaction");
                    reacsXML.AppendChild(reacXML);
                    GenerateReactionXml(reacXML, graphNodeId, me);
                    if (ReactionSpecies.Count > 0 && ReactionSpecies.ContainsKey(graphNodeId))
                    {
                        XmlElement specsXml = doc.CreateElement("ReactionSpeciesAll");
                        reacXML.AppendChild(specsXml);
                        foreach (IGraphReactionSpecies cat in ReactionSpecies[graphNodeId])
                        {
                            specsXml.AppendChild(GenerateReactionSpeciesXml(doc, graphNodeId, cat));
                        }
                    }
                }
            }

            XmlElement pwsXML = doc.CreateElement("Pathways");
            root.AppendChild(pwsXML);

            if (MappingPathways.Count > 0)
            {
                foreach (Guid graphNodeId in MappingPathways.Keys)
                {
                    IGraphPathway me = MappingPathways[graphNodeId];
                    XmlElement molXml = doc.CreateElement("Pathway");
                    pwsXML.AppendChild(molXml);
                    XmlUtil.AddAttribute(molXml, "ID", me.ID);
                    XmlUtil.AddAttribute(molXml, "Name", me.Name);
                }
            }
            return doc;
        }

        public XmlDocument GenerateDataXml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("GraphData");
            doc.AppendChild(root);

            XmlElement mols = doc.CreateElement("Molecules");
            root.AppendChild(mols);

            XmlElement procs = doc.CreateElement("GenericProcesses");
            root.AppendChild(procs);

            XmlElement pathways = doc.CreateElement("Pathways");
            root.AppendChild(pathways);

            XmlElement mappings = doc.CreateElement("Mappings");
            root.AppendChild(mappings);

            // molecules
            if (Molecules.Count > 0)
            {
                //foreach (IGraphMolecularEntity me in Molecules.Values)
                //{
                //    mols.AppendChild(GenerateMoleculeXml(doc, me));
                //}

                if (ExplicitMoleculeGraphNodes.Count == 0)
                {
                    // add all implicit molecules
                    foreach (Guid graphNodeId in MoleculeGraphNodes.Keys)
                    {
                        IGraphMolecularEntity me = Molecules[MoleculeGraphNodes[graphNodeId]];
                        mols.AppendChild(GenerateMoleculeXml(doc, graphNodeId, me));
                    }
                }
                else
                {
                    // add only explicit molecules
                    foreach (Guid graphNodeId in MoleculeGraphNodes.Keys)
                    {
                        if (!ExplicitMoleculeGraphNodes.ContainsKey(graphNodeId))
                            continue;

                        IGraphMolecularEntity me = Molecules[MoleculeGraphNodes[graphNodeId]];
                        mols.AppendChild(GenerateMoleculeXml(doc, graphNodeId, me));
                    }
                }
            }

            // processes
            if (GenericProcesses.Count > 0)
            {
                foreach (Guid graphNodeId in GenericProcessGraphNodes.Keys)
                {
                    IGraphGenericProcess p = GenericProcesses[GenericProcessGraphNodes[graphNodeId]];
                    procs.AppendChild(GenerateGenericProcessXml(doc, graphNodeId, p));
                }
            }

            // pathways
            if (CollapsedPathways.Count + ExpandedPathways.Count + CollapsedLinkingPathways.Count > 0)
            {
                if (ExpandedPathways.Count > 0)
                {
                    foreach (IGraphPathway pw in ExpandedPathways.Values)
                    {
                        pathways.AppendChild(GeneratePathwayXml(doc, pw, mols, true, false));
                    }
                }                
                
                if (CollapsedPathways.Count > 0)
                {
                    foreach (IGraphPathway pw in CollapsedPathways.Values)
                    {
                        if (ExpandedPathways.ContainsKey(pw.ID))
                            continue; // already on the graph in expanded form
                        pathways.AppendChild(GeneratePathwayXml(doc, pw, mols, false, false));
                    }
                }

                if (CollapsedLinkingPathways.Count > 0)
                {
                    foreach (IGraphPathway pw in CollapsedLinkingPathways.Values)
                    {
                        if (ExpandedPathways.ContainsKey(pw.ID) || CollapsedPathways.ContainsKey(pw.ID))
                            continue; // already on the graph explicitly in expanded or collapsed form
                        pathways.AppendChild(GeneratePathwayXml(doc, pw, mols, false, true));
                    }
                }
            }

            if (SpeciesMappings.Count > 0)
            {
                XmlElement spemappingsXml = doc.CreateElement("SpeciesMappings");
                mappings.AppendChild(spemappingsXml);
                foreach (ServerMapSpeciesMolecularEntities spmp in SpeciesMappings.Values)
                {
                    spemappingsXml.AppendChild(GenerateSpecMolMappingXml(doc, spmp));
                }
            }

            if (ReactionMappings.Count > 0)
            {
                XmlElement reacmappingsXml = doc.CreateElement("ReactionsMappings");
                mappings.AppendChild(reacmappingsXml);
                foreach (ServerMapReactionsProcessEntities spmp in ReactionMappings.Values)
                {
                    reacmappingsXml.AppendChild(GenerateReacProcMappingXml(doc, spmp));
                }
            }           

            return doc;
        }

        //public XmlDocument GenerateDataXml(List<string> tissues)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    XmlElement root = doc.CreateElement("GraphData");
        //    doc.AppendChild(root);

        //    XmlElement mols = doc.CreateElement("Molecules");
        //    root.AppendChild(mols);

        //    XmlElement procs = doc.CreateElement("GenericProcesses");
        //    root.AppendChild(procs);

        //    XmlElement pathways = doc.CreateElement("Pathways");
        //    root.AppendChild(pathways);

        //    // processes
        //    if (GenericProcesses.Count > 0)
        //    {

        //        foreach (Guid graphNodeId in GenericProcessGraphNodes.Keys)
        //        {
        //            IGraphGenericProcess p = GenericProcesses[GenericProcessGraphNodes[graphNodeId]];
        //            List<XmlNode> processXmlList = GenerateMetabolomicsGenericProcessXml(doc, graphNodeId, p, tissues);
        //            foreach (XmlNode pXml in processXmlList)
        //            {
        //                procs.AppendChild(pXml);
        //            }

        //        }

        //    }


        //    // molecules
        //    //the molecules section is populated after the process section because some of the molecules are assigned
        //    //new graph node id's based on their tissue information obtained form their corresponding process entity entries
        //    if (Molecules.Count > 0)
        //    {

        //        foreach (Guid graphNodeId in MoleculeGraphNodes.Keys)
        //        {
        //            IGraphMolecularEntity me = Molecules[MoleculeGraphNodes[graphNodeId]];
        //            if (tissueAwareMoleculeGraphNodes.ContainsKey(graphNodeId))
        //            {
        //                foreach (Guid newMolGraphNodeId in tissueAwareMoleculeGraphNodes[graphNodeId])
        //                {
        //                    XmlNode mNode = GenerateMetabolomicsMoleculeXml(doc, newMolGraphNodeId, me, tissues);
        //                    if (mNode != null)
        //                    {
        //                        mols.AppendChild(mNode);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                XmlNode mNode = GenerateMetabolomicsMoleculeXml(doc, graphNodeId, me, tissues);
        //                if (mNode != null)
        //                {
        //                    mols.AppendChild(mNode);
        //                }
        //            }
        //        }
        //    }


        //    // pathways
        //    if (CollapsedPathways.Count + ExpandedPathways.Count + CollapsedLinkingPathways.Count > 0)
        //    {
        //        if (ExpandedPathways.Count > 0)
        //        {
        //            foreach (IGraphPathway pw in ExpandedPathways.Values)
        //            {
        //                pathways.AppendChild(GenerateMetabolomicsPathwayXml(doc, pw, mols, true, false, tissues));
        //            }
        //        }

        //        if (CollapsedPathways.Count > 0)
        //        {
        //            foreach (IGraphPathway pw in CollapsedPathways.Values)
        //            {
        //                if (ExpandedPathways.ContainsKey(pw.ID))
        //                    continue; // already on the graph in expanded form
        //                pathways.AppendChild(GenerateMetabolomicsPathwayXml(doc, pw, mols, false, false, tissues));
        //            }
        //        }

        //        if (CollapsedLinkingPathways.Count > 0)
        //        {
        //            foreach (IGraphPathway pw in CollapsedLinkingPathways.Values)
        //            {
        //                if (ExpandedPathways.ContainsKey(pw.ID) || CollapsedPathways.ContainsKey(pw.ID))
        //                    continue; // already on the graph explicitly in expanded or collapsed form
        //                pathways.AppendChild(GenerateMetabolomicsPathwayXml(doc, pw, mols, false, true, tissues));
        //            }
        //        }
        //    }

        //    return doc;
        //}


        /// <summary>
        /// Creates a xml representation of the data in the cache for visualization purposes, but only for the selected tissues
        /// this method is used in the metabolomics project
        /// </summary>
        /// <param name="tissues"></param>
        /// <returns></returns>
        public XmlDocument GenerateDataXml(List<string> tissues)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("GraphData");
            doc.AppendChild(root);

            XmlElement mols = doc.CreateElement("Molecules");
            root.AppendChild(mols);

            XmlElement procs = doc.CreateElement("GenericProcesses");
            root.AppendChild(procs);

            XmlElement pathways = doc.CreateElement("Pathways");
            root.AppendChild(pathways);

            // processes
            if (GenericProcesses.Count > 0)
            {

                foreach (Guid graphNodeId in GenericProcessGraphNodes.Keys)
                {
                    IGraphGenericProcess p = GenericProcesses[GenericProcessGraphNodes[graphNodeId]];
                    List<XmlNode> processXmlList = GenerateMetabolomicsGenericProcessXml(doc, graphNodeId, p, tissues);
                    foreach (XmlNode pXml in processXmlList)
                    {
                        procs.AppendChild(pXml);
                    }

                }

            }


            // molecules
            //the molecules section is populated after the process section because some of the molecules are assigned
            //new graph node id's based on their tissue information obtained form their corresponding process entity entries
            if (Molecules.Count > 0)
            {

                foreach (Guid graphNodeId in MoleculeGraphNodes.Keys)
                {
                    IGraphMolecularEntity me = Molecules[MoleculeGraphNodes[graphNodeId]];
                    if (tissueAwareMoleculeGraphNodes.ContainsKey(graphNodeId))
                    {
                        foreach (Guid newMolGraphNodeId in tissueAwareMoleculeGraphNodes[graphNodeId])
                        {
                            XmlNode mNode = GenerateMetabolomicsMoleculeXml(doc, newMolGraphNodeId, me, tissues);
                            if (mNode != null)
                            {
                                mols.AppendChild(mNode);
                            }
                        }
                    }
                    else
                    {
                        XmlNode mNode = GenerateMetabolomicsMoleculeXml(doc, graphNodeId, me, tissues);
                        if (mNode != null)
                        {
                            mols.AppendChild(mNode);
                        }
                    }
                }
            }


            // pathways
            if (CollapsedPathways.Count + ExpandedPathways.Count + CollapsedLinkingPathways.Count > 0)
            {
                if (ExpandedPathways.Count > 0)
                {
                    foreach (IGraphPathway pw in ExpandedPathways.Values)
                    {
                        pathways.AppendChild(GenerateMetabolomicsPathwayXml(doc, pw, mols, true, false, tissues));
                    }
                }

                if (CollapsedPathways.Count > 0)
                {
                    foreach (IGraphPathway pw in CollapsedPathways.Values)
                    {
                        if (ExpandedPathways.ContainsKey(pw.ID))
                            continue; // already on the graph in expanded form
                        pathways.AppendChild(GenerateMetabolomicsPathwayXml(doc, pw, mols, false, false, tissues));
                    }
                }

                if (CollapsedLinkingPathways.Count > 0)
                {
                    foreach (IGraphPathway pw in CollapsedLinkingPathways.Values)
                    {
                        if (ExpandedPathways.ContainsKey(pw.ID) || CollapsedPathways.ContainsKey(pw.ID))
                            continue; // already on the graph explicitly in expanded or collapsed form
                        pathways.AppendChild(GenerateMetabolomicsPathwayXml(doc, pw, mols, false, true, tissues));
                    }
                }
            }

            return doc;
        }

        private XmlNode GenerateMoleculeXml(XmlDocument doc, Guid graphNodeId, IGraphMolecularEntity me)
        {
            Guid defaultPathwayId = GetPathwayForMoleculeGraphNode(graphNodeId);

            XmlElement molXml = doc.CreateElement("Molecule");

            // data values
            XmlUtil.AddAttribute(molXml, "ID", graphNodeId);
            XmlUtil.AddAttribute(molXml, "EntityID", me.ID);
            XmlUtil.AddAttribute(molXml, "Name", me.Name);
            //XmlUtil.AddAttribute(molXml, "DefaultPathwayID", defaultPathwayId);

            // derived classes
            if (me is IGraphBasicMolecule)
            {
                IGraphBasicMolecule bm = (IGraphBasicMolecule)me;
                XmlUtil.AddAttribute(molXml, "IsCommon", bm.IsCommon.ToString());                
            }

            return molXml;
        }

        private XmlNode GenerateModelXml(XmlElement molXml, Guid graphNodeId, IGraphModel me)
        {
            //Guid defaultPathwayId = GetPathwayForMoleculeGraphNode(graphNodeId);

            //XmlElement molXml = doc.CreateElement("Model");

            // data values
            //XmlUtil.AddAttribute(molXml, "ID", graphNodeId);
            XmlUtil.AddAttribute(molXml, "ID", me.ID);
            XmlUtil.AddAttribute(molXml, "Name", me.Name);
           // XmlUtil.AddAttribute(molXml, "SbmlFile", me.SbmlFile);

            // derived classes
            //if (me is IGraphBasicMolecule)
            //{
            //    IGraphBasicMolecule bm = (IGraphBasicMolecule)me;
            //    XmlUtil.AddAttribute(molXml, "IsCommon", bm.IsCommon.ToString());
            //}

            return molXml;
        }

        private void GenerateCompartmentXml(XmlElement molXml, Guid graphNodeId, IGraphCompartment me)
        {
            //Guid defaultPathwayId = GetPathwayForMoleculeGraphNode(graphNodeId);

            //XmlElement molXml = doc.CreateElement("Compartment");

            // data values
            //XmlUtil.AddAttribute(molXml, "ID", graphNodeId);
            XmlUtil.AddAttribute(molXml, "ID", me.ID);
            XmlUtil.AddAttribute(molXml, "Name", me.Name);
            XmlUtil.AddAttribute(molXml, "sbmlID",me.sbmlID);
            XmlUtil.AddAttribute(molXml, "Size", me.Size);
            XmlUtil.AddAttribute(molXml, "SpatialDimensions", me.SpatialDimensions);
            XmlUtil.AddAttribute(molXml, "Constant", me.Constant);
            XmlUtil.AddAttribute(molXml, "CompartmentTypeId", me.CompartmentTypeId);
            XmlUtil.AddAttribute(molXml, "Outside", me.Outside);

            // derived classes
            //if (me is IGraphBasicMolecule)
            //{
            //    IGraphBasicMolecule bm = (IGraphBasicMolecule)me;
            //    XmlUtil.AddAttribute(molXml, "IsCommon", bm.IsCommon.ToString());
            //}

            //return molXml;
        }
        private void GenerateReactionXml(XmlElement molXml, Guid graphNodeId, IGraphReaction me)
        {
            //Guid defaultPathwayId = GetPathwayForMoleculeGraphNode(graphNodeId);

            //XmlElement molXml = doc.CreateElement("Compartment");

            // data values
            //XmlUtil.AddAttribute(molXml, "ID", graphNodeId);
            XmlUtil.AddAttribute(molXml, "ID", me.ID);
            XmlUtil.AddAttribute(molXml, "Name", me.Name);
            XmlUtil.AddAttribute(molXml, "sbmlId", me.SbmlId);
            XmlUtil.AddAttribute(molXml, "Reversible", me.Reversible);
            //XmlUtil.AddAttribute(molXml, "KineticLawId", Regex.Replace(ServerKineticLaw.LoadRowByID(me.KineticLawId).GetString("Math"), "\\", "--"));//"ServerKineticLaw.LoadRowByID(me.KineticLawId).GetString(id)");// Regex.Replace(ServerKineticLaw.LoadRowByID(me.KineticLawId).GetString("Math"),"&#x","-x#&-")); 
            XmlUtil.AddAttribute(molXml, "KineticLawId", ServerKineticLaw.LoadRowByID(me.KineticLawId).GetString("Math"));
            XmlUtil.AddAttribute(molXml, "Fast", me.Fast);
            
            // derived classes
            //if (me is IGraphBasicMolecule)
            //{
            //    IGraphBasicMolecule bm = (IGraphBasicMolecule)me;
            //    XmlUtil.AddAttribute(molXml, "IsCommon", bm.IsCommon.ToString());
            //}

            //return molXml;
        }
        private XmlNode GenerateSpeciesXml(XmlDocument doc, Guid graphNodeId, IGraphSpecies me)
        {
            //Guid defaultPathwayId = GetPathwayForMoleculeGraphNode(graphNodeId);
            if(commonSpecieslist==null)  {
                commonSpecieslist = new ArrayList();
                
                SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT * FROM common_species;");

                DataSet[] ds = new DataSet[0];
                DBWrapper.LoadMultiple(out ds, ref command);
                int iR=0;
                foreach (DataSet d in ds)
                {                    
                    commonSpecieslist.Add(d.Tables[0].Rows[0].ItemArray[0]);
                }
            }
                
            XmlElement molXml = doc.CreateElement("Species");

            // data values
            //XmlUtil.AddAttribute(molXml, "ID", graphNodeId);
            XmlUtil.AddAttribute(molXml, "ID", me.ID);
            XmlUtil.AddAttribute(molXml, "Name", me.Name);
            XmlUtil.AddAttribute(molXml, "sbmlID", me.SbmlId);
            XmlUtil.AddAttribute(molXml, "SpeciesTypeId", me.SpeciesTypeId);
            XmlUtil.AddAttribute(molXml, "InitialAmount", me.InitialAmount);
            XmlUtil.AddAttribute(molXml, "InitialConcentration", me.InitialConcentration);
            XmlUtil.AddAttribute(molXml, "SubstanceUnitsId", me.SubstanceUnitsId);
            XmlUtil.AddAttribute(molXml, "HasOnlySubstanceUnits", me.HasOnlySubstanceUnits);
            XmlUtil.AddAttribute(molXml, "BoundaryCondition", me.BoundaryCondition);
            XmlUtil.AddAttribute(molXml, "Charge", me.Charge);
            XmlUtil.AddAttribute(molXml, "Constant", me.Constant);
            XmlUtil.AddAttribute(molXml, "IsCommon", commonSpecieslist.Contains(me.Name));

            // derived classes
            //if (me is IGraphBasicMolecule)
            //{
            //    IGraphBasicMolecule bm = (IGraphBasicMolecule)me;
            //    XmlUtil.AddAttribute(molXml, "IsCommon", bm.IsCommon.ToString());
            //}

            return molXml;
        }

        private XmlNode GenerateReactionSpeciesXml(XmlDocument doc, Guid graphNodeId, IGraphReactionSpecies me)
        {
            //Guid defaultPathwayId = GetPathwayForMoleculeGraphNode(graphNodeId);

            XmlElement molXml = doc.CreateElement("ReactionSpecies");

            // data values
            //XmlUtil.AddAttribute(molXml, "ID", graphNodeId);
            XmlUtil.AddAttribute(molXml, "ID", me.ID);
            XmlUtil.AddAttribute(molXml, "Name", me.Name);
            XmlUtil.AddAttribute(molXml, "SpeciesId", me.SpeciesId);
            XmlUtil.AddAttribute(molXml, "RoleId",ReactionSpeciesRoleManager.GetRoleName(me.RoleId));
            XmlUtil.AddAttribute(molXml, "Stoichiometry", me.Stoichiometry);

            // derived classes
            //if (me is IGraphBasicMolecule)
            //{
            //    IGraphBasicMolecule bm = (IGraphBasicMolecule)me;
            //    XmlUtil.AddAttribute(molXml, "IsCommon", bm.IsCommon.ToString());
            //}

            return molXml;
        }
        
        /// <summary>
        /// Generates xml representation of a molecule for the metabolomics project 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="graphNodeId"></param>
        /// <param name="me"></param>
        /// <returns></returns>
        private XmlNode GenerateMetabolomicsMoleculeXml(XmlDocument doc, Guid graphNodeId, IGraphMolecularEntity me)
        {
            Guid defaultPathwayId = GetPathwayForMoleculeGraphNode(graphNodeId);

            XmlElement molXml = doc.CreateElement("Molecule");

            // data values
            XmlUtil.AddAttribute(molXml, "ID", graphNodeId);
            XmlUtil.AddAttribute(molXml, "EntityID", me.ID);
            XmlUtil.AddAttribute(molXml, "Name", me.Name);
            //XmlUtil.AddAttribute(molXml, "DefaultPathwayID", defaultPathwayId);
            if (molIDTissNameMap.ContainsKey(graphNodeId))
                XmlUtil.AddAttribute(molXml, "Tissue", molIDTissNameMap[graphNodeId]);

            // derived classes
            if (me is IGraphBasicMolecule)
            {
                IGraphBasicMolecule bm = (IGraphBasicMolecule)me;
                XmlUtil.AddAttribute(molXml, "IsCommon", bm.IsCommon.ToString());
            }

            return molXml;
        }

        /// <summary>
        /// Generates xml representation of a molecule for the metabolomics project
        /// xml  represetation is generated only if the molecule belong to the selected tissues
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="graphNodeId"></param>
        /// <param name="me"></param>
        /// <param name="tissues"></param>
        /// <returns></returns>
        private XmlNode GenerateMetabolomicsMoleculeXml(XmlDocument doc, Guid graphNodeId, IGraphMolecularEntity me, List<string> tissues)
        {

            if (molIDTissNameMap.ContainsKey(graphNodeId))
            {
                string tissue = molIDTissNameMap[graphNodeId];
                if (tissues.Contains(tissue) || tissue == "blood")
                {
                    Guid defaultPathwayId = GetPathwayForMoleculeGraphNode(graphNodeId);

                    XmlElement molXml = doc.CreateElement("Molecule");

                    // data values
                    XmlUtil.AddAttribute(molXml, "ID", graphNodeId);
                    XmlUtil.AddAttribute(molXml, "EntityID", me.ID);
                    XmlUtil.AddAttribute(molXml, "Name", me.Name);
                    //XmlUtil.AddAttribute(molXml, "DefaultPathwayID", defaultPathwayId);
                    if (molIDTissNameMap.ContainsKey(graphNodeId))
                        XmlUtil.AddAttribute(molXml, "Tissue", molIDTissNameMap[graphNodeId]);

                    // derived classes
                    if (me is IGraphBasicMolecule)
                    {
                        IGraphBasicMolecule bm = (IGraphBasicMolecule)me;
                        XmlUtil.AddAttribute(molXml, "IsCommon", bm.IsCommon.ToString());
                    }

                    return molXml;
                }
            }
            return null;
        }


        private XmlNode GenerateSpecMolMappingXml(XmlDocument doc, ServerMapSpeciesMolecularEntities spmp)
        {                    
                    XmlElement molXml = doc.CreateElement("Species");

                    // data values
                    XmlUtil.AddAttribute(molXml, "ID", spmp.SpeciesId);
                    XmlUtil.AddAttribute(molXml, "AnnotationQualifier", AnnotationQualifierManager.GetComplementQualifierName(spmp.QualifierId));
                    XmlUtil.AddAttribute(molXml, "MolecularEntityID", spmp.MolecularEntityId);
                    //XmlUtil.AddAttribute(molXml, "DefaultPathwayID", defaultPathwayId);
                    
                    return molXml;
        }


        private XmlNode GenerateReacProcMappingXml(XmlDocument doc, ServerMapReactionsProcessEntities spmp)
        {
            XmlElement molXml = doc.CreateElement("Reaction");

            // data values
            XmlUtil.AddAttribute(molXml, "ID", spmp.ReactionId);
            XmlUtil.AddAttribute(molXml, "AnnotationQualifier", AnnotationQualifierManager.GetComplementQualifierName(spmp.QualifierId));
            XmlUtil.AddAttribute(molXml, "ProcessID", spmp.ProcessId);
            //XmlUtil.AddAttribute(molXml, "DefaultPathwayID", defaultPathwayId);

            return molXml;
        }
   
        private XmlNode GenerateGenericProcessXml(XmlDocument doc, Guid graphNodeId, IGraphGenericProcess p)
        {
            Guid defaultPathwayId = GetPathwayForProcessGraphNode(graphNodeId);

            XmlElement procXml = doc.CreateElement("GenericProcess");

            // data values
            XmlUtil.AddAttribute(procXml, "ID", graphNodeId);
            XmlUtil.AddAttribute(procXml, "GenericProcessID", p.GenericProcessID);
            XmlUtil.AddAttribute(procXml, "Name", p.Name);
            XmlUtil.AddAttribute(procXml, "Reversible", p.Reversible);
            //XmlUtil.AddAttribute(procXml, "DefaultPathwayID", defaultPathwayId);

            // process entities
            if (GenericProcessEntities.Count > 0 && GenericProcessEntities.ContainsKey(p.GenericProcessID))
            {
                XmlElement molsXml = doc.CreateElement("Molecules");
                procXml.AppendChild(molsXml);

                foreach (IGraphProcessEntity pe in GenericProcessEntities[p.GenericProcessID])
                {
                    XmlElement peXml = doc.CreateElement("Molecule");
                    molsXml.AppendChild(peXml);

                    XmlUtil.AddAttribute(peXml, "ID", MoleculeIdToGraphNodeId(defaultPathwayId, pe.EntityID)); // convert to graphNode
                    //XmlUtil.AddAttribute(peXml, "RealMoleculeID", pe.EntityID);
                    XmlUtil.AddAttribute(peXml, "ProcessID", pe.ProcessID); //NOTE: SPECIFIC PROCESS ID!
                    XmlUtil.AddAttribute(peXml, "Role", pe.Role);
                }
            }

            // catalyzes
            if (GenericProcessCatalyzes.Count > 0 && GenericProcessCatalyzes.ContainsKey(p.GenericProcessID))
            {
                XmlElement catsXml = doc.CreateElement("Catalyzes");
                procXml.AppendChild(catsXml);

                foreach (IGraphCatalyze cat in GenericProcessCatalyzes[p.GenericProcessID])
                {
                    XmlElement catXml = doc.CreateElement("Catalyze");
                    catsXml.AppendChild(catXml);

                    XmlUtil.AddAttribute(catXml, "GeneProductMoleculeID", MoleculeIdToGraphNodeId(defaultPathwayId, cat.GeneProductID)); // convert to graphNode
                    XmlUtil.AddAttribute(catXml, "ProcessID", cat.ProcessID); //NOTE: SPECIFIC PROCESS ID!
                    XmlUtil.AddAttribute(catXml, "ECNumber", cat.ECNumber);
                    XmlUtil.AddAttribute(catXml, "OrganismGroupID", cat.OrganismGroupID);
                }
            }

            return procXml;
        }
        /// <summary>
        /// Generates XML representation of a process for the metabolomics project 
        /// This method is to be used if tissue restrictions are necessary
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="graphNodeId"></param>
        /// <param name="p"></param>
        /// <param name="tissues"></param>
        /// <returns></returns>
        private List<XmlNode> GenerateMetabolomicsGenericProcessXml(XmlDocument doc, Guid graphNodeId, IGraphGenericProcess p, List<string> tissues)
        {
            List<XmlNode> processXmlNode = new List<XmlNode>();

            //here I need to check if the process is a transport process
            //if (p.IsTransport)
            //{
            //    if (transportProcessTissue.ContainsKey(p.GenericProcessID) && tissues.Contains(transportProcessTissue[p.GenericProcessID]))
            //    {
            //        Guid defaultPathwayId = GetPathwayForProcessGraphNode(graphNodeId);

            //        XmlElement procXml = doc.CreateElement("GenericProcess");

            //        // data values
            //        XmlUtil.AddAttribute(procXml, "ID", graphNodeId);
            //        XmlUtil.AddAttribute(procXml, "GenericProcessID", p.GenericProcessID);
            //        XmlUtil.AddAttribute(procXml, "Name", p.Name);
            //        XmlUtil.AddAttribute(procXml, "Reversible", p.Reversible);
            //        XmlUtil.AddAttribute(procXml, "IsTransport", p.IsTransport);
            //        //XmlUtil.AddAttribute(procXml, "DefaultPathwayID", defaultPathwayId);

            //        // process entities
            //        if (GenericProcessEntities.Count > 0 && GenericProcessEntities.ContainsKey(p.GenericProcessID))
            //        {
            //            XmlElement molsXml = doc.CreateElement("Molecules");
            //            procXml.AppendChild(molsXml);

            //            foreach (IGraphProcessEntity pe in GenericProcessEntities[p.GenericProcessID])
            //            {
            //                if (pe.Tissue == "blood" || (!transportProcessGraphNodes.ContainsKey(p.ID)))
            //                {
            //                    XmlElement peXml = doc.CreateElement("Molecule");
            //                    molsXml.AppendChild(peXml);

            //                    Guid EntityGraphId = GraphNodeManager.GetEntityGraphNodeId(defaultPathwayId, pe.EntityID, pe.TissueID);
            //                    Guid oldGraphNodeId = MoleculeIdToGraphNodeId(defaultPathwayId, pe.EntityID);

            //                    if (!tissueAwareMoleculeGraphNodes.ContainsKey(oldGraphNodeId))
            //                    {
            //                        tissueAwareMoleculeGraphNodes[oldGraphNodeId] = new List<Guid>();
            //                    }
            //                    if (!tissueAwareMoleculeGraphNodes[oldGraphNodeId].Contains(EntityGraphId))
            //                    {
            //                        tissueAwareMoleculeGraphNodes[oldGraphNodeId].Add(EntityGraphId);
            //                    }
            //                    //sometimes only the entity id is used for transport process entities
            //                    oldGraphNodeId = pe.EntityID;
            //                    if (!tissueAwareMoleculeGraphNodes.ContainsKey(oldGraphNodeId))
            //                    {
            //                        tissueAwareMoleculeGraphNodes[oldGraphNodeId] = new List<Guid>();
            //                    }
            //                    if (!tissueAwareMoleculeGraphNodes[oldGraphNodeId].Contains(EntityGraphId))
            //                    {
            //                        tissueAwareMoleculeGraphNodes[oldGraphNodeId].Add(EntityGraphId);
            //                    }

            //                    XmlUtil.AddAttribute(peXml, "ID", EntityGraphId);  // convert to graphNode
            //                    //XmlUtil.AddAttribute(peXml, "RealMoleculeID", pe.EntityID);
            //                    XmlUtil.AddAttribute(peXml, "ProcessID", pe.ProcessID); //NOTE: SPECIFIC PROCESS ID!
            //                    XmlUtil.AddAttribute(peXml, "Role", pe.Role);
            //                    XmlUtil.AddAttribute(peXml, "Tissue", pe.Tissue);

            //                    if (!molIDTissNameMap.ContainsKey(EntityGraphId))
            //                        molIDTissNameMap.Add(EntityGraphId, pe.Tissue);
            //                }
            //                else
            //                {
            //                    foreach (Guid gNID in transportProcessGraphNodes[p.ID])
            //                    {
            //                        XmlElement peXml = doc.CreateElement("Molecule");
            //                        molsXml.AppendChild(peXml);

            //                        XmlUtil.AddAttribute(peXml, "ID", gNID);  // convert to graphNode
            //                        XmlUtil.AddAttribute(peXml, "ProcessID", pe.ProcessID); //NOTE: SPECIFIC PROCESS ID!
            //                        XmlUtil.AddAttribute(peXml, "Role", pe.Role);
            //                        XmlUtil.AddAttribute(peXml, "Tissue", pe.Tissue);

            //                        if (!molIDTissNameMap.ContainsKey(gNID))
            //                        {
            //                            molIDTissNameMap.Add(gNID, pe.Tissue);

            //                        }
            //                    }
            //                }
            //            }
            //        }

            //        // catalyzes
            //        if (GenericProcessCatalyzes.Count > 0 && GenericProcessCatalyzes.ContainsKey(p.GenericProcessID))
            //        {
            //            XmlElement catsXml = doc.CreateElement("Catalyzes");
            //            procXml.AppendChild(catsXml);

            //            foreach (IGraphCatalyze cat in GenericProcessCatalyzes[p.GenericProcessID])
            //            {
            //                XmlElement catXml = doc.CreateElement("Catalyze");
            //                catsXml.AppendChild(catXml);

            //                XmlUtil.AddAttribute(catXml, "GeneProductMoleculeID", MoleculeIdToGraphNodeId(defaultPathwayId, cat.GeneProductID)); // convert to graphNode
            //                XmlUtil.AddAttribute(catXml, "ProcessID", cat.ProcessID); //NOTE: SPECIFIC PROCESS ID!
            //                XmlUtil.AddAttribute(catXml, "ECNumber", cat.ECNumber);
            //                XmlUtil.AddAttribute(catXml, "OrganismGroupID", cat.OrganismGroupID);

            //                if (!molIDTissNameMap.ContainsKey(MoleculeIdToGraphNodeId(defaultPathwayId, cat.GeneProductID)))
            //                    molIDTissNameMap.Add(MoleculeIdToGraphNodeId(defaultPathwayId, cat.GeneProductID), "blood");
            //            }
            //        }

            //        processXmlNode.Add(procXml);
            //    }
            //    return processXmlNode;
            //}
            //else
            //{
            //    Dictionary<int, List<IGraphProcessEntity>> TissPEntityMap = new Dictionary<int, List<IGraphProcessEntity>>();
            //    //iterate through all the process entities to group all the process entities according to tissues
            //    if (GenericProcessEntities.ContainsKey(p.GenericProcessID))
            //    {
            //        foreach (IGraphProcessEntity pe in GenericProcessEntities[p.GenericProcessID])
            //        {
            //            if (!TissPEntityMap.ContainsKey(pe.TissueID))
            //            {
            //                TissPEntityMap[pe.TissueID] = new List<IGraphProcessEntity>();
            //            }
            //            TissPEntityMap[pe.TissueID].Add(pe);
            //        }

            //        if (TissPEntityMap.Count > 0)//Todo:have to handle the case of processes without process_entities
            //        {
            //            foreach (int tissueId in TissPEntityMap.Keys)
            //            {
            //                string tissueName = ProcessEntityTissueManager.GetTissueName(tissueId);
            //                if (tissues.Contains(tissueName) || tissueName == "blood")
            //                {

            //                    Guid defaultPathwayId = GetPathwayForProcessGraphNode(graphNodeId);

            //                    XmlElement procXml = doc.CreateElement("GenericProcess");
            //                    Guid ProcessGraphId = GraphNodeManager.GetProcessGraphNodeId(defaultPathwayId, p.GenericProcessID, tissueId);
            //                    if (!tissueAwareProcessGraphNodes.ContainsKey(graphNodeId))
            //                    {
            //                        tissueAwareProcessGraphNodes[graphNodeId] = new List<Guid>();
            //                    }
            //                    if (!tissueAwareProcessGraphNodes[graphNodeId].Contains(ProcessGraphId))
            //                    {

            //                        tissueAwareProcessGraphNodes[graphNodeId].Add(ProcessGraphId);
            //                    }
            //                    XmlUtil.AddAttribute(procXml, "ID", ProcessGraphId);
            //                    XmlUtil.AddAttribute(procXml, "GenericProcessID", p.GenericProcessID);
            //                    XmlUtil.AddAttribute(procXml, "Name", p.Name);
            //                    XmlUtil.AddAttribute(procXml, "Reversible", p.Reversible);
            //                    XmlUtil.AddAttribute(procXml, "IsTransport", p.IsTransport);
            //                    XmlUtil.AddAttribute(procXml, "Tissue", tissueName);

            //                    List<IGraphProcessEntity> pEntities = TissPEntityMap[tissueId];
            //                    //if (pEntities.Count > 0)
            //                    //{
            //                    //tissueName = pEntities[0].Tissue;

            //                    //}
            //                    XmlElement molsXml = doc.CreateElement("Molecules");
            //                    procXml.AppendChild(molsXml);

            //                    foreach (IGraphProcessEntity pEntity in pEntities)
            //                    {
            //                        XmlElement peXml = doc.CreateElement("Molecule");
            //                        molsXml.AppendChild(peXml);
            //                        Guid EntityGraphId = GraphNodeManager.GetEntityGraphNodeId(defaultPathwayId, pEntity.EntityID, pEntity.TissueID);
            //                        Guid oldGraphNodeId = MoleculeIdToGraphNodeId(defaultPathwayId, pEntity.EntityID);
            //                        if (!tissueAwareMoleculeGraphNodes.ContainsKey(oldGraphNodeId))
            //                        {
            //                            tissueAwareMoleculeGraphNodes[oldGraphNodeId] = new List<Guid>();
            //                        }
            //                        if (!tissueAwareMoleculeGraphNodes[oldGraphNodeId].Contains(EntityGraphId))
            //                        {
            //                            tissueAwareMoleculeGraphNodes[oldGraphNodeId].Add(EntityGraphId);
            //                        }
            //                        XmlUtil.AddAttribute(peXml, "ID", EntityGraphId);  // convert to graphNode
            //                        //XmlUtil.AddAttribute(peXml, "RealMoleculeID", pe.EntityID);
            //                        XmlUtil.AddAttribute(peXml, "ProcessID", pEntity.ProcessID); //NOTE: SPECIFIC PROCESS ID!
            //                        XmlUtil.AddAttribute(peXml, "Role", pEntity.Role);
            //                        XmlUtil.AddAttribute(peXml, "Tissue", pEntity.Tissue);


            //                        if (!molIDTissNameMap.ContainsKey(EntityGraphId))
            //                        {
            //                            molIDTissNameMap.Add(EntityGraphId, pEntity.Tissue);
            //                        }
            //                        if (includeTransportProcesses)
            //                        {
            //                            if (pEntity.Tissue != "blood")
            //                            {
            //                                ServerProcess[] transportProcesses = ServerProcessEntity.GetTransportProcessForEntity(pEntity.EntityID, pEntity.TissueID);
            //                                if (transportProcesses.Length > 0)
            //                                {
            //                                    foreach (ServerProcess transportProcess in transportProcesses)
            //                                    {
            //                                        if (!transportProcessGraphNodes.ContainsKey(transportProcess.ID))
            //                                        {
            //                                            transportProcessGraphNodes.Add(transportProcess.ID, new List<Guid>());

            //                                            if (!transportProcessGraphNodes[transportProcess.ID].Contains(EntityGraphId))
            //                                            {
            //                                                transportProcessGraphNodes[transportProcess.ID].Add(EntityGraphId);
            //                                            }
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }


            //                    }

            //                    if (GenericProcessCatalyzes.Count > 0 && GenericProcessCatalyzes.ContainsKey(p.GenericProcessID))
            //                    {
            //                        XmlElement catsXml = doc.CreateElement("Catalyzes");
            //                        procXml.AppendChild(catsXml);

            //                        foreach (IGraphCatalyze cat in GenericProcessCatalyzes[p.GenericProcessID])
            //                        {
            //                            XmlElement catXml = doc.CreateElement("Catalyze");
            //                            catsXml.AppendChild(catXml);
            //                            Guid oldGraphNodeId = MoleculeIdToGraphNodeId(defaultPathwayId, cat.GeneProductID);
            //                            Guid enzymeGraphId = GraphNodeManager.GetEntityGraphNodeId(defaultPathwayId, cat.GeneProductID, tissueId);
            //                            //XmlUtil.AddAttribute(catXml, "GeneProductMoleculeID", MoleculeIdToGraphNodeId(defaultPathwayId, cat.GeneProductID)); // convert to graphNode
            //                            XmlUtil.AddAttribute(catXml, "GeneProductMoleculeID", enzymeGraphId); // convert to graphNode
            //                            XmlUtil.AddAttribute(catXml, "ProcessID", cat.ProcessID); //NOTE: SPECIFIC PROCESS ID!
            //                            XmlUtil.AddAttribute(catXml, "ECNumber", cat.ECNumber);
            //                            XmlUtil.AddAttribute(catXml, "OrganismGroupID", cat.OrganismGroupID);

            //                            if (!tissueAwareMoleculeGraphNodes.ContainsKey(oldGraphNodeId))
            //                            {
            //                                tissueAwareMoleculeGraphNodes[oldGraphNodeId] = new List<Guid>();
            //                            }
            //                            if (!tissueAwareMoleculeGraphNodes[oldGraphNodeId].Contains(enzymeGraphId))
            //                            {
            //                                tissueAwareMoleculeGraphNodes[oldGraphNodeId].Add(enzymeGraphId);
            //                            }

            //                            if (!molIDTissNameMap.ContainsKey(enzymeGraphId))
            //                            {
            //                                molIDTissNameMap.Add(enzymeGraphId, tissueName);
            //                            }
            //                        }
            //                    }
            //                    processXmlNode.Add(procXml);
            //                }
            //            }
            //        }

            //    }
            //}

            return processXmlNode;
        }

        private XmlNode GeneratePathwayXml(XmlDocument doc, IGraphPathway pw, XmlElement mols, bool expanded, bool linkingPathway)
        {
            XmlElement pwXml = doc.CreateElement("Pathway");

            // data values
            XmlUtil.AddAttribute(pwXml, "ID", pw.ID);
            XmlUtil.AddAttribute(pwXml, "Name", pw.Name);
            XmlUtil.AddAttribute(pwXml, "Expanded", expanded.ToString());
            XmlUtil.AddAttribute(pwXml, "Linking", linkingPathway.ToString());

            // generic processes
            if (PathwaysToGenericProcesses.Count > 0 && PathwaysToGenericProcesses.ContainsKey(pw.ID))
            {
                XmlElement genProcsXml = doc.CreateElement("GenericProcesses");
                pwXml.AppendChild(genProcsXml);

                foreach (Guid genProcId in PathwaysToGenericProcesses[pw.ID])
                {
                    XmlElement genProcXml = doc.CreateElement("GenericProcess");
                    genProcsXml.AppendChild(genProcXml);

                    XmlUtil.AddAttribute(genProcXml, "ID", GenericProcessIdToGraphNodeId(pw.ID, genProcId)); // convert to graphNode
                }
            }

            // linking pathways and molecules
            if (!linkingPathway && 
                (LinkingPathwaysAndMoleculeGraphNodes[pw.ID].Count > 0 || 
               LinkingInPathwaysAndMoleculeGraphNodes[pw.ID].Count > 0 ))
            {
                XmlElement linksXml = doc.CreateElement("LinkingPathways");
                pwXml.AppendChild(linksXml);

                foreach (Guid toPwId in LinkingPathwaysAndMoleculeGraphNodes[pw.ID].Keys)
                {
                    XmlElement linkXml = doc.CreateElement("LinkingPathway");
                    linksXml.AppendChild(linkXml);

                    XmlUtil.AddAttribute(linkXml, "ID", toPwId);
                    XmlUtil.AddAttribute(linkXml, "Dir", "out");

                    foreach (Guid graphMolId in LinkingPathwaysAndMoleculeGraphNodes[pw.ID][toPwId])
                    {
                        XmlElement molXml = doc.CreateElement("LinkingMolecule");
                        linkXml.AppendChild(molXml);

                        XmlUtil.AddAttribute(molXml, "ID", graphMolId); // already using graph nodes
                    }
                }

                foreach (Guid fromPwId in LinkingInPathwaysAndMoleculeGraphNodes[pw.ID].Keys)
                {
                    XmlElement linkXml = doc.CreateElement("LinkingPathway");
                    linksXml.AppendChild(linkXml);

                    XmlUtil.AddAttribute(linkXml, "ID", fromPwId);
                    XmlUtil.AddAttribute(linkXml, "Dir", "in");

                    foreach (Guid graphMolId in LinkingInPathwaysAndMoleculeGraphNodes[pw.ID][fromPwId])
                    {
                        XmlElement molXml = doc.CreateElement("LinkingMolecule");
                        linkXml.AppendChild(molXml);

                        XmlUtil.AddAttribute(molXml, "ID", graphMolId); // already using graph nodes
                    }
                }
            }

            // organism groups (only for collapsed pathways); available at process level for expanded pathways
            if (!expanded)
            {
                List<Guid> orgs = PathwayOrgGroups[pw.ID];
                if (orgs.Count > 0)
                {
                    XmlElement orgsXml = doc.CreateElement("OrganismGroups");
                    pwXml.AppendChild(orgsXml);

                    foreach (Guid orgId in orgs)
                    {
                        XmlElement orgXml = doc.CreateElement("OrganismGroup");
                        orgsXml.AppendChild(orgXml);

                        XmlUtil.AddAttribute(orgXml, "ID", orgId);
                    }
                }
            }

            return pwXml;
        }

        /// <summary>
        /// Generates XML representation of a pathway for the metabolomics project
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="pw"></param>
        /// <param name="mols"></param>
        /// <param name="expanded"></param>
        /// <param name="linkingPathway"></param>
        /// <returns></returns>
        private XmlNode GenerateMetabolomicsPathwayXml(XmlDocument doc, IGraphPathway pw, XmlElement mols, bool expanded, bool linkingPathway)
        {
            XmlElement pwXml = doc.CreateElement("Pathway");

            // data values
            XmlUtil.AddAttribute(pwXml, "ID", pw.ID);
            XmlUtil.AddAttribute(pwXml, "Name", pw.Name);
            XmlUtil.AddAttribute(pwXml, "Expanded", expanded.ToString());
            XmlUtil.AddAttribute(pwXml, "Linking", linkingPathway.ToString());

            // generic processes
            if (PathwaysToGenericProcesses.Count > 0 && PathwaysToGenericProcesses.ContainsKey(pw.ID))
            {
                XmlElement genProcsXml = doc.CreateElement("GenericProcesses");
                pwXml.AppendChild(genProcsXml);

                foreach (Guid genProcId in PathwaysToGenericProcesses[pw.ID])
                {

                    if (tissueAwareProcessGraphNodes.ContainsKey(GenericProcessIdToGraphNodeId(pw.ID, genProcId)))
                    {
                        foreach (Guid tissueAwareProcessId in tissueAwareProcessGraphNodes[GenericProcessIdToGraphNodeId(pw.ID, genProcId)])
                        {
                            XmlElement genProcXml = doc.CreateElement("GenericProcess");
                            genProcsXml.AppendChild(genProcXml);
                            XmlUtil.AddAttribute(genProcXml, "ID", tissueAwareProcessId); // convert to graphNode
                        }
                    }
                    else
                    {
                        XmlElement genProcXml = doc.CreateElement("GenericProcess");
                        genProcsXml.AppendChild(genProcXml);
                        XmlUtil.AddAttribute(genProcXml, "ID", GenericProcessIdToGraphNodeId(pw.ID, genProcId)); // convert to graphNode
                    }
                }
            }


            // linking pathways and molecules
            if (!linkingPathway &&
                (LinkingPathwaysAndMoleculeGraphNodes[pw.ID].Count > 0 ||
               LinkingInPathwaysAndMoleculeGraphNodes[pw.ID].Count > 0))
            {
                XmlElement linksXml = doc.CreateElement("LinkingPathways");
                pwXml.AppendChild(linksXml);

                foreach (Guid toPwId in LinkingPathwaysAndMoleculeGraphNodes[pw.ID].Keys)
                {
                    XmlElement linkXml = doc.CreateElement("LinkingPathway");
                    linksXml.AppendChild(linkXml);

                    XmlUtil.AddAttribute(linkXml, "ID", toPwId);
                    XmlUtil.AddAttribute(linkXml, "Dir", "out");

                    foreach (Guid graphMolId in LinkingPathwaysAndMoleculeGraphNodes[pw.ID][toPwId])
                    {
                        XmlElement molXml = doc.CreateElement("LinkingMolecule");
                        linkXml.AppendChild(molXml);
                        XmlUtil.AddAttribute(molXml, "ID", graphMolId); // already using graph nodes
                    }
                }

                foreach (Guid fromPwId in LinkingInPathwaysAndMoleculeGraphNodes[pw.ID].Keys)
                {
                    XmlElement linkXml = doc.CreateElement("LinkingPathway");
                    linksXml.AppendChild(linkXml);

                    XmlUtil.AddAttribute(linkXml, "ID", fromPwId);
                    XmlUtil.AddAttribute(linkXml, "Dir", "in");

                    foreach (Guid graphMolId in LinkingInPathwaysAndMoleculeGraphNodes[pw.ID][fromPwId])
                    {
                        XmlElement molXml = doc.CreateElement("LinkingMolecule");
                        linkXml.AppendChild(molXml);

                        XmlUtil.AddAttribute(molXml, "ID", graphMolId); // already using graph nodes
                    }
                }
            }

            // organism groups (only for collapsed pathways); available at process level for expanded pathways
            if (!expanded)
            {
                List<Guid> orgs = PathwayOrgGroups[pw.ID];
                if (orgs.Count > 0)
                {
                    XmlElement orgsXml = doc.CreateElement("OrganismGroups");
                    pwXml.AppendChild(orgsXml);

                    foreach (Guid orgId in orgs)
                    {
                        XmlElement orgXml = doc.CreateElement("OrganismGroup");
                        orgsXml.AppendChild(orgXml);

                        XmlUtil.AddAttribute(orgXml, "ID", orgId);
                    }
                }
            }

            return pwXml;
        }

        /// <summary>
        /// Generates XML representation of a pathway for the metabolomics project
        /// This method is to be used if tissue restrictions are necessary
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="pw"></param>
        /// <param name="mols"></param>
        /// <param name="expanded"></param>
        /// <param name="linkingPathway"></param>
        /// <param name="tissues"></param>
        /// <returns></returns>
        private XmlNode GenerateMetabolomicsPathwayXml(XmlDocument doc, IGraphPathway pw, XmlElement mols, bool expanded, bool linkingPathway, List<string> tissues)
        {
            XmlElement pwXml = doc.CreateElement("Pathway");

            // data values
            XmlUtil.AddAttribute(pwXml, "ID", pw.ID);
            XmlUtil.AddAttribute(pwXml, "Name", pw.Name);
            XmlUtil.AddAttribute(pwXml, "Expanded", expanded.ToString());
            XmlUtil.AddAttribute(pwXml, "Linking", linkingPathway.ToString());

            // generic processes
            if (PathwaysToGenericProcesses.Count > 0 && PathwaysToGenericProcesses.ContainsKey(pw.ID))
            {
                XmlElement genProcsXml = doc.CreateElement("GenericProcesses");
                pwXml.AppendChild(genProcsXml);

                foreach (Guid genProcId in PathwaysToGenericProcesses[pw.ID])
                {

                    if (tissueAwareProcessGraphNodes.ContainsKey(GenericProcessIdToGraphNodeId(pw.ID, genProcId)))
                    {
                        foreach (Guid tissueAwareProcessId in tissueAwareProcessGraphNodes[GenericProcessIdToGraphNodeId(pw.ID, genProcId)])
                        {
                            XmlElement genProcXml = doc.CreateElement("GenericProcess");
                            genProcsXml.AppendChild(genProcXml);
                            XmlUtil.AddAttribute(genProcXml, "ID", tissueAwareProcessId); // convert to graphNode
                        }
                    }
                    else
                    {
                        XmlElement genProcXml = doc.CreateElement("GenericProcess");
                        genProcsXml.AppendChild(genProcXml);
                        XmlUtil.AddAttribute(genProcXml, "ID", GenericProcessIdToGraphNodeId(pw.ID, genProcId)); // convert to graphNode
                    }
                }
            }

            int validLinkingPathwaysCount = 0;
            // linking pathways and molecules
            if (!linkingPathway &&
                (LinkingPathwaysAndMoleculeGraphNodes[pw.ID].Count > 0 ||
               LinkingInPathwaysAndMoleculeGraphNodes[pw.ID].Count > 0))
            {
                XmlElement linksXml = doc.CreateElement("LinkingPathways");


                foreach (Guid toPwId in LinkingPathwaysAndMoleculeGraphNodes[pw.ID].Keys)
                {
                    int validLinkingMolecules = 0;
                    XmlElement linkXml = doc.CreateElement("LinkingPathway");


                    XmlUtil.AddAttribute(linkXml, "ID", toPwId);
                    XmlUtil.AddAttribute(linkXml, "Dir", "out");

                    foreach (Guid graphMolId in LinkingPathwaysAndMoleculeGraphNodes[pw.ID][toPwId])
                    {

                        if (tissues.Contains(molIDTissNameMap[graphMolId]) || molIDTissNameMap[graphMolId] == "blood")
                        {
                            XmlElement molXml = doc.CreateElement("LinkingMolecule");
                            linkXml.AppendChild(molXml);
                            XmlUtil.AddAttribute(molXml, "ID", graphMolId); // already using graph nodes
                            validLinkingMolecules++;
                        }
                    }
                    if (validLinkingMolecules > 0)
                    {
                        linksXml.AppendChild(linkXml);
                        validLinkingPathwaysCount++;
                    }
                }

                foreach (Guid fromPwId in LinkingInPathwaysAndMoleculeGraphNodes[pw.ID].Keys)
                {
                    int validLinkingMolecules = 0;
                    XmlElement linkXml = doc.CreateElement("LinkingPathway");
                    linksXml.AppendChild(linkXml);

                    XmlUtil.AddAttribute(linkXml, "ID", fromPwId);
                    XmlUtil.AddAttribute(linkXml, "Dir", "in");

                    foreach (Guid graphMolId in LinkingInPathwaysAndMoleculeGraphNodes[pw.ID][fromPwId])
                    {
                        if (tissues.Contains(molIDTissNameMap[graphMolId]) || molIDTissNameMap[graphMolId] == "blood")
                        {
                            XmlElement molXml = doc.CreateElement("LinkingMolecule");
                            linkXml.AppendChild(molXml);

                            XmlUtil.AddAttribute(molXml, "ID", graphMolId); // already using graph nodes
                            validLinkingMolecules++;
                        }
                    }
                    if (validLinkingMolecules > 0)
                    {
                        linksXml.AppendChild(linkXml);
                        validLinkingPathwaysCount++;
                    }
                }

                if (validLinkingPathwaysCount > 0)
                {
                    pwXml.AppendChild(linksXml);
                }
            }

            // organism groups (only for collapsed pathways); available at process level for expanded pathways
            if (!expanded)
            {
                List<Guid> orgs = PathwayOrgGroups[pw.ID];
                if (orgs.Count > 0)
                {
                    XmlElement orgsXml = doc.CreateElement("OrganismGroups");
                    pwXml.AppendChild(orgsXml);

                    foreach (Guid orgId in orgs)
                    {
                        XmlElement orgXml = doc.CreateElement("OrganismGroup");
                        orgsXml.AppendChild(orgXml);

                        XmlUtil.AddAttribute(orgXml, "ID", orgId);
                    }
                }
            }

            return pwXml;
        }


     

        #endregion

        #region Cache retrieval interface

        public List<IGraphProcessEntity> GetProcessEntities(Guid genericProcessId)
        {
            return GenericProcessEntities[genericProcessId];
        }

        public List<IGraphGeneProduct> GetGeneProducts(Guid genericProcessId)
        {
            if (!GenericProcessGeneProductMappings.ContainsKey(genericProcessId))
                return new List<IGraphGeneProduct>();

            List<IGraphGeneProduct> results = new List<IGraphGeneProduct>();
            foreach (Guid geneProductId in GenericProcessGeneProductMappings[genericProcessId])
            {
                results.Add((IGraphGeneProduct)Molecules[geneProductId]);
            }
            return results;
        }

        public List<string> GetECNumbers(Guid genericProcessId)
        {
            if (!GenericProcessECNumbers.ContainsKey(genericProcessId))
                return new List<string>();

            return GenericProcessECNumbers[genericProcessId];
        }

        public IGraphMolecularEntity GetMolecule(Guid molecularEntityId)
        {
            return Molecules[molecularEntityId];
        }

        public List<Guid> GetSpecificProcessIds(Guid genericProcessId)
        {
            return GenericToSpecificProcessIdTable[genericProcessId];
        }

        public Guid GetGenericProcessId(Guid specificProcessId)
        {
            return SpecificToGenericProcessIdTable[specificProcessId];
        }

        public List<Guid> GetOrganismGroupIds(Guid genericProcessId)
        {
            if (!GenericProcessOrgGroups.ContainsKey(genericProcessId))
                return new List<Guid>();

            return GenericProcessOrgGroups[genericProcessId];
        }

        public List<IGraphGenericProcess> GetGenericProcesses(Guid pathwayId)
        {
            List<IGraphGenericProcess> results = new List<IGraphGenericProcess>();

            foreach (Guid genericProcessId in PathwaysToGenericProcesses[pathwayId])
            {
                results.Add(GenericProcesses[genericProcessId]);
            }

            return results;
        }

        public IGraphPathway GetCollapsedPathway(Guid pathwayId)
        {
            return CollapsedPathways[pathwayId];
        }

        public IGraphPathway GetExpandedPathway(Guid pathwayId)
        {
            return ExpandedPathways[pathwayId];
        }

        public IGraphGenericProcess GetGenericProcess(Guid genericProcessId)
        {
            return GenericProcesses[genericProcessId];
        }

        public bool HasMolecule(Guid molecularEntityId)
        {
            return Molecules.ContainsKey(molecularEntityId);
        }

        public bool HasGenericProcess(Guid genericProcessId)
        {
            return GenericProcesses.ContainsKey(genericProcessId);
        }

        public bool HasCollapsedPathway(Guid pathwayId)
        {
            return CollapsedPathways.ContainsKey(pathwayId);
        }

        public bool HasExpandedPathway(Guid pathwayId)
        {
            return ExpandedPathways.ContainsKey(pathwayId);
        }

        public bool HasModel(Guid modelId)
        {
            return Models.ContainsKey(modelId);
        }

        public bool HasCompartment(Guid compartmentId)
        {
            return Compartments.ContainsKey(compartmentId);
        }

        public bool HasReacion(Guid reactionId)
        {
            return Reactions.ContainsKey(reactionId);
        }

        public bool HasMappingPathways(Guid pwId)
        {
            return MappingPathways.ContainsKey(pwId);
        }

        #endregion

        #region Cache fill interface (to be implemented by derived class)

        /// <summary>
        /// Loads all data required to draw a molecule.  This is typically just loading the molecule object.
        /// </summary>
        /// <param name="molecularEntityId"></param>
        public abstract void FillCacheForMolecularEntity(Guid molecularEntityId);

        public abstract void FillCacheForGenericProcess(Guid genericProcessId);

        public abstract void FillCacheForCompartment(Guid compartmentId);
        public abstract void FillCacheForModel(Guid modelId);
        public abstract void FillCacheForPathway(Guid pwId);
        public abstract void FillCacheForReaction(Guid reactionId);
        public abstract void FillCacheForSpeMappings(Guid mid,Guid pwid);
        public abstract void FillCacheForReactionMappings(Guid mid, Guid pwid);
        /// <summary>
        /// Loads all data required to draw a pathway in as few queries as possible.
        /// </summary>
        /// <param name="pathwayId"></param>
        public abstract void FillCacheForExpandedPathway(Guid pathwayId);

        public abstract void FillCacheForExpandedPathway(Guid pathwayId, bool includeTransportProcesses);

        public abstract void FillCacheForCollapsedPathway(Guid pathwayId);

        public abstract IGraphBasicMolecule[] GetAllCommonMolecules();

        #endregion

        protected static void AddValues<TKey, TValue>(Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> newValues)
        {
            foreach (TKey k in newValues.Keys)
            {
                if (!dic.ContainsKey(k))
                    dic.Add(k, newValues[k]);
            }
        }



    }
}
