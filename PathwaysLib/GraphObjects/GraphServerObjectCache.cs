using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.GraphObjects
{
    public class GraphServerObjectCache : GraphObjectCache
    {
        #region Cache fill interface

        public override void FillCacheForModel(Guid modelId)
        {
            if (!Models.ContainsKey(modelId))
            {
                Models.Add(modelId, ServerModel.Load(modelId));
            }
            else
            {
                return; // already in cache!
            }
        }
        
        public override void FillCacheForCompartment(Guid compartmentId)
        {            
            if (!Compartments.ContainsKey(compartmentId))
            {
                Compartments.Add(compartmentId, ServerCompartment.Load(compartmentId));

                ServerSpecies[] speciesincomp = ServerSpecies.LoadAllForCompartment(compartmentId);
                foreach (ServerSpecies spec in speciesincomp)
                {
                    if (!Species.ContainsKey(compartmentId))
                        Species.Add(compartmentId, new List<IGraphSpecies>());
                    Species[compartmentId].Add(spec);
                }            
            }
            else
            {
                return; // already in cache!
            }
        }

        public override void FillCacheForReaction(Guid reactionId) 
        {
            if (!Reactions.ContainsKey(reactionId))
            {
                Reactions.Add(reactionId, ServerReaction.Load(reactionId));

                ServerReactionSpecies[] speciesinreac = ServerReactionSpecies.LoadAllForReaction(reactionId);
                foreach (ServerReactionSpecies spec in speciesinreac)
                {
                    if (!ReactionSpecies.ContainsKey(reactionId))
                        ReactionSpecies.Add(reactionId, new List<IGraphReactionSpecies>());
                    ReactionSpecies[reactionId].Add(spec);
                }
            }
            else
            {
                return; // already in cache!
            }
        }


        public override void FillCacheForPathway(Guid pwId)
        {
            if (!MappingPathways.ContainsKey(pwId))
            {
                MappingPathways.Add(pwId, ServerPathway.Load(pwId));
            }
            else
            {
                return; // already in cache!
            }
        }
        
        public override void FillCacheForSpeMappings(Guid mid,Guid pwid)
        {
            //if (!MolIDsInPW.ContainsKey(pwid)) {
            //    MolIDsInPW.Add(pwid, GraphNodeManager.GetEntitiesByPW(pwid));
            //}

            //MolIDsInPW=GraphNodeManager.GetEntitiesByPW(pwid);
            List<Guid> entitiesInPW = GraphNodeManager.GetEntitiesByPW(pwid);

            //if(!SpIDsInModel.ContainsKey(mid))
            //    SpIDsInModel.Add(
            List<Guid> allSpecs = ServerSpecies.GetAllSpeciesIDForModel(mid);

            ServerMapSpeciesMolecularEntities[] spemappings=ServerMapSpeciesMolecularEntities.AllMapSpeciesMolecularEntities();

            if (entitiesInPW != null && allSpecs != null && spemappings!=null)
            {
                foreach(ServerMapSpeciesMolecularEntities spemapping in spemappings){
                    if (allSpecs.Contains(spemapping.SpeciesId) && entitiesInPW.Contains(spemapping.MolecularEntityId))
                    {
                        spemapping.MolecularEntityId=GraphNodeManager.GetEntityGraphNodeId(pwid, spemapping.MolecularEntityId); //mapping entityid to graphid, which is used in pathway graph visualization
                        SpeciesMappings.Add(spemapping.SpeciesId + "+" + spemapping.QualifierId + "+" + spemapping.MolecularEntityId, spemapping);
                    }
                }           
            }
        }

        public override void FillCacheForReactionMappings(Guid mid, Guid pwid)
        {
            List<Guid> entitiesInPW = ServerProcess.GetAllProcessesIDsForPathway(pwid);

            //if(!SpIDsInModel.ContainsKey(mid))
            //    SpIDsInModel.Add(
            List<Guid> allReacs = ServerReaction.GetAllReactionsIDsForModel(mid);

            ServerMapReactionsProcessEntities[] spemappings = ServerMapReactionsProcessEntities.AllMapReactionsProcessEntities();

            if (entitiesInPW != null && allReacs != null && spemappings != null)
            {
                foreach (ServerMapReactionsProcessEntities spemapping in spemappings)
                {
                    if (allReacs.Contains(spemapping.ReactionId) && entitiesInPW.Contains(spemapping.ProcessId))
                        spemapping.ProcessId = GraphNodeManager.GetProcessGraphNodeId(pwid, ServerProcess.Load(spemapping.ProcessId).GenericProcessID);//get graphid from genericprocessid from processid
                        ReactionMappings.Add(spemapping.ReactionId + "+" + spemapping.QualifierId + "+" + spemapping.ProcessId, spemapping);
                }
            }
        }

        /// <summary>
        /// Loads all data required to draw a molecule.  This is typically just loading the molecule object.
        /// </summary>
        /// <param name="molecularEntityId"></param>
        public override void FillCacheForMolecularEntity(Guid molecularEntityGraphNodeId)
        {
            // convert graphNodeId to molecule ID and pathway ID
            Guid molecularEntityId;
            Guid pathwayId;
            GraphNodeManager.GetPathwayAndEntityId(molecularEntityGraphNodeId, out pathwayId, out molecularEntityId);

            if (!ExplicitMoleculeGraphNodes.ContainsKey(molecularEntityGraphNodeId))
                ExplicitMoleculeGraphNodes.Add(molecularEntityGraphNodeId, molecularEntityGraphNodeId);

            if (!MoleculeGraphNodes.ContainsKey(molecularEntityGraphNodeId))
            {
                MoleculeGraphNodes.Add(molecularEntityGraphNodeId, molecularEntityId);
                MoleculeGraphNodePathways.Add(molecularEntityGraphNodeId, new List<Guid>());
                MoleculeGraphNodePathways[molecularEntityGraphNodeId].Add(pathwayId);
            }
            else
            {
                if (!MoleculeGraphNodePathways[molecularEntityGraphNodeId].Contains(pathwayId))
                    MoleculeGraphNodePathways[molecularEntityGraphNodeId].Add(pathwayId);

                return; // already in cache
            }

            if (!Molecules.ContainsKey(molecularEntityId))
            {
                Molecules.Add(molecularEntityId, ServerMolecularEntity.Load(molecularEntityId));
            }
            else
            {
                return; // already in cache!
            }
        }

        /// <summary>
        /// Loads all data required to draw a pathway in as few queries as possible.
        /// 
        /// (9-15 Queries)
        /// </summary>
        /// <param name="pathwayId"></param>
        public override void FillCacheForExpandedPathway(Guid pathwayId)
        {
            // pathway
            if (!ExpandedPathways.ContainsKey(pathwayId))
            {
                ExpandedPathways.Add(pathwayId, ServerPathway.Load(pathwayId));
            }
            else
            {
                return; // already in the cache!
            }

            ServerPathway pathway = (ServerPathway)ExpandedPathways[pathwayId];

            // load all generic processes
            ServerGenericProcess[] genProcs = pathway.GetAllGenericProcesses();
            PathwaysToGenericProcesses.Add(pathwayId, new List<Guid>());
            foreach (ServerGenericProcess genp in genProcs)
            {
                if (!GenericProcesses.ContainsKey(genp.GenericProcessID))
                {
                    GenericProcesses.Add(genp.GenericProcessID, genp);
                }

                PathwaysToGenericProcesses[pathwayId].Add(genp.GenericProcessID);
            }

            // specific process id -> generic process id mapping table (1 Query)
            Dictionary<Guid, Guid> specificToGeneric = ServerGenericProcess.GetProcessMappingTableInPathway(pathwayId);
            AddValues(SpecificToGenericProcessIdTable, specificToGeneric);
            // generic process -> specific process mapping
            AddToGenericToSpecificProcessTable(specificToGeneric);

            // load all process entities (1 Query)
            ServerProcessEntity[] entities = ServerProcessEntity.GetAllForPathway(pathwayId);
            foreach (ServerProcessEntity pe in entities)
            {
                Guid genericProcessId = SpecificToGenericProcessIdTable[pe.ProcessID];

                if (!GenericProcessEntities.ContainsKey(genericProcessId))
                {
                    GenericProcessEntities.Add(genericProcessId, new List<IGraphProcessEntity>());
                }

                GenericProcessEntities[genericProcessId].Add(pe);

                // graph node for process
                Guid processGraphNodeId = GraphNodeManager.GetProcessGraphNodeId(pathwayId, genericProcessId);
                if (!GenericProcessGraphNodes.ContainsKey(processGraphNodeId))
                {
                    GenericProcessGraphNodes.Add(processGraphNodeId, genericProcessId);
                    GenericProcessGraphNodePathways.Add(processGraphNodeId, new List<Guid>());
                }
                if (!GenericProcessGraphNodePathways[processGraphNodeId].Contains(pathwayId))
                    GenericProcessGraphNodePathways[processGraphNodeId].Add(pathwayId);
            }

            // load all process entity molecules (2-5 Queries)
            ServerMolecularEntity[] peMolecules = ServerProcessEntity.GetProcessEntitiesMoleculesForPathway(pathwayId);
            foreach (ServerMolecularEntity me in peMolecules)
            {
                //if (!Molecules.ContainsKey(me.ID))
                //    Molecules.Add(me.ID, me);

                // graph node for molecule
                FillMoleculeInPathway(pathwayId, me.ID, me, true);
                //Guid entityGraphNodeId = GraphNodeManager.GetEntityGraphNodeId(pathwayId, me.ID);
                //if (!MoleculeGraphNodes.ContainsKey(entityGraphNodeId))
                //{
                //    MoleculeGraphNodes.Add(entityGraphNodeId, me.ID);
                //    MoleculeGraphNodePathways.Add(entityGraphNodeId, new List<Guid>());
                //}
                //if (!MoleculeGraphNodePathways[entityGraphNodeId].Contains(pathwayId))
                //    MoleculeGraphNodePathways[entityGraphNodeId].Add(pathwayId);
            }

            // load all gene products (2-5 Queries)
            List<ServerGeneProduct> geneProducts = ServerCatalyze.GetAllGeneProductsForPathway(pathwayId);
            foreach (ServerGeneProduct gp in geneProducts)
            {
                //if (!Molecules.ContainsKey(gp.ID))
                //    Molecules.Add(gp.ID, gp);

                // graph node for molecule
                FillMoleculeInPathway(pathwayId, gp.ID, gp, true);
                //Guid entityGraphNodeId = GraphNodeManager.GetEntityGraphNodeId(pathwayId, gp.ID);
                //if (!MoleculeGraphNodes.ContainsKey(entityGraphNodeId))
                //{
                //    MoleculeGraphNodes.Add(entityGraphNodeId, gp.ID);
                //    MoleculeGraphNodePathways.Add(entityGraphNodeId, new List<Guid>());
                //}
                //if (!MoleculeGraphNodePathways[entityGraphNodeId].Contains(pathwayId))
                //    MoleculeGraphNodePathways[entityGraphNodeId].Add(pathwayId);
            }

            // load catalyzes [i.e. gene products to process mappings]  (1 Query)    
            AddValues(GenericProcessGeneProductMappings, ServerCatalyze.GetGeneProductsForGenericProcessesInPathway(pathwayId));

            // load all EC#'s for generic processes (1 Query)
            AddValues(GenericProcessECNumbers, ServerCatalyze.GetECNumbersForGenericProcessesInPathway(pathwayId));

            // load all organism/groups for generic processes (1 Query)
            AddValues(GenericProcessOrgGroups, ServerCatalyze.GetOrganismGroupsForGenericProcessesInPathway(pathwayId));

            // load all catalyzes tuples seperately
            //AddValues(GenericProcessCatalyzes, ServerCatalyze.LoadAllForGenericProcessesInPathway(pathwayId));
            ServerCatalyze[] catalyzes = ServerCatalyze.LoadAllInPathway(pathwayId);
            foreach (ServerCatalyze cat in catalyzes)
            {
                Guid genericProcId = GetGenericProcessId(cat.ProcessID);
                if (!GenericProcessCatalyzes.ContainsKey(genericProcId))
                    GenericProcessCatalyzes.Add(genericProcId, new List<IGraphCatalyze>());
                GenericProcessCatalyzes[genericProcId].Add(cat);
            }

            // linking pathways & molecules
            FillPathwayLinks(pathwayId, true);
        }

        public override void FillCacheForExpandedPathway(Guid pathwayId, bool includeTransportProcesses)
        {
            // pathway
            if (!ExpandedPathways.ContainsKey(pathwayId))
            {
                ExpandedPathways.Add(pathwayId, ServerPathway.Load(pathwayId));
            }
            else
            {
                return; // already in the cache!
            }

            ServerPathway pathway = (ServerPathway)ExpandedPathways[pathwayId];

            // load all generic processes
            ServerGenericProcess[] genProcs = pathway.GetAllGenericProcesses();
            PathwaysToGenericProcesses.Add(pathwayId, new List<Guid>());
            foreach (ServerGenericProcess genp in genProcs)
            {
                if (!GenericProcesses.ContainsKey(genp.GenericProcessID))
                {
                    GenericProcesses.Add(genp.GenericProcessID, genp);
                }

                PathwaysToGenericProcesses[pathwayId].Add(genp.GenericProcessID);
            }

            // specific process id -> generic process id mapping table (1 Query)
            Dictionary<Guid, Guid> specificToGeneric = ServerGenericProcess.GetProcessMappingTableInPathway(pathwayId);
            AddValues(SpecificToGenericProcessIdTable, specificToGeneric);
            // generic process -> specific process mapping
            AddToGenericToSpecificProcessTable(specificToGeneric);

            // load all process entities (1 Query)
            ServerProcessEntity[] entities = ServerProcessEntity.GetAllForPathway(pathwayId);
            foreach (ServerProcessEntity pe in entities)
            {
                Guid genericProcessId = SpecificToGenericProcessIdTable[pe.ProcessID];

                if (!GenericProcessEntities.ContainsKey(genericProcessId))
                {
                    GenericProcessEntities.Add(genericProcessId, new List<IGraphProcessEntity>());
                }

                GenericProcessEntities[genericProcessId].Add(pe);

                // graph node for process
                Guid processGraphNodeId = GraphNodeManager.GetProcessGraphNodeId(pathwayId, genericProcessId);
                if (!GenericProcessGraphNodes.ContainsKey(processGraphNodeId))
                {
                    GenericProcessGraphNodes.Add(processGraphNodeId, genericProcessId);
                    GenericProcessGraphNodePathways.Add(processGraphNodeId, new List<Guid>());
                }
                if (!GenericProcessGraphNodePathways[processGraphNodeId].Contains(pathwayId))
                    GenericProcessGraphNodePathways[processGraphNodeId].Add(pathwayId);


                //if (includeTransportProcesses)
                //{

                //    this.includeTransportProcesses = includeTransportProcesses;
                //    if (pe.Tissue != "blood" && (pe.Role == "substrate" || pe.Role == "product"))
                //    {
                //        ServerProcess[] transportProcesses = ServerProcessEntity.GetTransportProcessForEntity(pe.EntityID, pe.TissueID);
                //        if (transportProcesses.Length > 0)
                //        {

                //            foreach (ServerProcess p in transportProcesses)
                //            {
                //                if (!additionalTransportProcesses.Contains(p.GenericProcessID))
                //                {
                //                    additionalTransportProcesses.Add(p.GenericProcessID);
                //                    transportProcessTissue.Add(p.GenericProcessID, pe.Tissue);
                //                }
                //            }
                //        }
                //    }
                //}

            }

            // load all process entity molecules (2-5 Queries)
            ServerMolecularEntity[] peMolecules = ServerProcessEntity.GetProcessEntitiesMoleculesForPathway(pathwayId);
            foreach (ServerMolecularEntity me in peMolecules)
            {
                if (!Molecules.ContainsKey(me.ID))
                    Molecules.Add(me.ID, me);

                // graph node for molecule
                FillMoleculeInPathway(pathwayId, me.ID, me, true);
                //Guid entityGraphNodeId = GraphNodeManager.GetEntityGraphNodeId(pathwayId, me.ID);
                //if (!MoleculeGraphNodes.ContainsKey(entityGraphNodeId))
                //{
                //    MoleculeGraphNodes.Add(entityGraphNodeId, me.ID);
                //    MoleculeGraphNodePathways.Add(entityGraphNodeId, new List<Guid>());
                //}
                //if (!MoleculeGraphNodePathways[entityGraphNodeId].Contains(pathwayId))
                //    MoleculeGraphNodePathways[entityGraphNodeId].Add(pathwayId);
            }

            // load all gene products (2-5 Queries)
            List<ServerGeneProduct> geneProducts = ServerCatalyze.GetAllGeneProductsForPathway(pathwayId);
            foreach (ServerGeneProduct gp in geneProducts)
            {
                if (!Molecules.ContainsKey(gp.ID))
                    Molecules.Add(gp.ID, gp);

                // graph node for molecule
                FillMoleculeInPathway(pathwayId, gp.ID, gp, true);
                //Guid entityGraphNodeId = GraphNodeManager.GetEntityGraphNodeId(pathwayId, gp.ID);
                //if (!MoleculeGraphNodes.ContainsKey(entityGraphNodeId))
                //{
                //    MoleculeGraphNodes.Add(entityGraphNodeId, gp.ID);
                //    MoleculeGraphNodePathways.Add(entityGraphNodeId, new List<Guid>());
                //}
                //if (!MoleculeGraphNodePathways[entityGraphNodeId].Contains(pathwayId))
                //    MoleculeGraphNodePathways[entityGraphNodeId].Add(pathwayId);
            }

            // load catalyzes [i.e. gene products to process mappings]  (1 Query)    
            AddValues(GenericProcessGeneProductMappings, ServerCatalyze.GetGeneProductsForGenericProcessesInPathway(pathwayId));

            // load all EC#'s for generic processes (1 Query)
            AddValues(GenericProcessECNumbers, ServerCatalyze.GetECNumbersForGenericProcessesInPathway(pathwayId));

            // load all organism/groups for generic processes (1 Query)
            AddValues(GenericProcessOrgGroups, ServerCatalyze.GetOrganismGroupsForGenericProcessesInPathway(pathwayId));

            // load all catalyzes tuples seperately
            //AddValues(GenericProcessCatalyzes, ServerCatalyze.LoadAllForGenericProcessesInPathway(pathwayId));
            ServerCatalyze[] catalyzes = ServerCatalyze.LoadAllInPathway(pathwayId);
            foreach (ServerCatalyze cat in catalyzes)
            {
                Guid genericProcId = GetGenericProcessId(cat.ProcessID);
                if (!GenericProcessCatalyzes.ContainsKey(genericProcId))
                    GenericProcessCatalyzes.Add(genericProcId, new List<IGraphCatalyze>());
                GenericProcessCatalyzes[genericProcId].Add(cat);
            }

            // linking pathways & molecules
            FillPathwayLinks(pathwayId);

            if (includeTransportProcesses)
            {
                //additional transport processe
                foreach (Guid genPId in additionalTransportProcesses)
                {
                    FillCacheForGenericProcess(genPId);
                }
            }
        }

        private void AddToGenericToSpecificProcessTable(Dictionary<Guid, Guid> specificToGeneric)
        {
            foreach (Guid specificProcessId in specificToGeneric.Keys)
            {
                Guid genericProcessId = SpecificToGenericProcessIdTable[specificProcessId];
                if (!GenericToSpecificProcessIdTable.ContainsKey(genericProcessId))
                {
                    GenericToSpecificProcessIdTable.Add(genericProcessId, new List<Guid>());
                }
                if (!GenericToSpecificProcessIdTable[genericProcessId].Contains(specificProcessId))
                    GenericToSpecificProcessIdTable[genericProcessId].Add(specificProcessId);
            }
        }

        public override void FillCacheForCollapsedPathway(Guid pathwayId)
        {
            // pathway
            if (!CollapsedPathways.ContainsKey(pathwayId))
            {
                CollapsedPathways.Add(pathwayId, ServerPathway.Load(pathwayId));
            }
            else
            {
                return; // already in the cache!
            }

            // linking pathways & molecules
            FillPathwayLinks(pathwayId, true); // set to false to not include metadata on linking pathways

            // organism/groups for pathway node
            if (!PathwayOrgGroups.ContainsKey(pathwayId))
            {
                List<Guid> groupIds = ServerCatalyze.GetOrganismGroupsForPathway(pathwayId);
                PathwayOrgGroups.Add(pathwayId, groupIds);
            }
        }

        private void FillPathwayLinks(Guid pathwayId)
        {
            LinkingPathwaysAndMoleculeGraphNodes.Add(pathwayId, new Dictionary<Guid, List<Guid>>());
            LinkingInPathwaysAndMoleculeGraphNodes.Add(pathwayId, new Dictionary<Guid, List<Guid>>());

            // outgoing pathway links
            //Dictionary<Guid, List<Guid>> linkingPwAndMol = ServerPathway.GetConnectedPathwaysWithEntities(pathwayId);
            Dictionary<Guid, List<object[]>> linkingPwAndMol = ServerPathway.GetConnectedPathwaysWithTissueAwareEntities(pathwayId);
            ServerPathway[] linkedPathways = ServerPathway.GetPathwaysLinkedToPathway(pathwayId);
            foreach (ServerPathway toPw in linkedPathways) //(Guid toPwId in linkingPwAndMol.Keys)
            {
                if (!LinkingPathwaysAndMoleculeGraphNodes[pathwayId].ContainsKey(toPw.ID))
                    LinkingPathwaysAndMoleculeGraphNodes[pathwayId].Add(toPw.ID, new List<Guid>());

                if (!CollapsedLinkingPathways.ContainsKey(toPw.ID))
                {
                    CollapsedLinkingPathways.Add(toPw.ID, toPw);

                    // organism/groups for pathway node
                    if (!PathwayOrgGroups.ContainsKey(toPw.ID))
                    {
                        List<Guid> groupIds = ServerCatalyze.GetOrganismGroupsForPathway(toPw.ID);
                        PathwayOrgGroups.Add(toPw.ID, groupIds);
                    }
                }

                //foreach (object[] mol in linkingPwAndMol[toPw.ID])
                //{
                //    // add molecule if needed
                //    Guid entityGraphNodeId = FillMoleculeInPathway(pathwayId, (Guid)mol[1], null, false, Int32.Parse(mol[2].ToString()));

                //    LinkingPathwaysAndMoleculeGraphNodes[pathwayId][toPw.ID].Add(entityGraphNodeId);
                //    string linkingtissueName = ProcessEntityTissueManager.GetTissueName(Int32.Parse(mol[2].ToString()));
                //    if (!molIDTissNameMap.ContainsKey(entityGraphNodeId))
                //        molIDTissNameMap.Add(entityGraphNodeId, linkingtissueName);
                //}
            }

            // incoming pathway links
            Dictionary<Guid, List<object[]>> linkingInPwAndMol = ServerPathway.GetConnectedPathwaysWithTissueAwareEntitiesIncoming(pathwayId);
            ServerPathway[] linkedInPathways = ServerPathway.GetPathwaysLinkedToPathwayIncoming(pathwayId);
            foreach (ServerPathway fromPw in linkedInPathways)
            {
                if (!LinkingInPathwaysAndMoleculeGraphNodes[pathwayId].ContainsKey(fromPw.ID))
                    LinkingInPathwaysAndMoleculeGraphNodes[pathwayId].Add(fromPw.ID, new List<Guid>());

                if (!CollapsedLinkingPathways.ContainsKey(fromPw.ID))
                {
                    CollapsedLinkingPathways.Add(fromPw.ID, fromPw);

                    // organism/groups for pathway node
                    if (!PathwayOrgGroups.ContainsKey(fromPw.ID))
                    {
                        List<Guid> groupIds = ServerCatalyze.GetOrganismGroupsForPathway(fromPw.ID);
                        PathwayOrgGroups.Add(fromPw.ID, groupIds);
                    }
                }

                //foreach (object[] mol in linkingInPwAndMol[fromPw.ID])
                //{
                //    // add molecule if needed
                //    Guid entityGraphNodeId = FillMoleculeInPathway(fromPw.ID, (Guid)mol[1], null, false, Int32.Parse(mol[2].ToString()));

                //    LinkingInPathwaysAndMoleculeGraphNodes[pathwayId][fromPw.ID].Add(entityGraphNodeId);
                //    string linkingtissueName = ProcessEntityTissueManager.GetTissueName(Int32.Parse(mol[2].ToString()));
                //    if (!molIDTissNameMap.ContainsKey(entityGraphNodeId))
                //        molIDTissNameMap.Add(entityGraphNodeId, linkingtissueName);
                //}
            }
        }

  

        private void FillPathwayLinks(Guid pathwayId, bool addLinkingPathways)
        {
            LinkingPathwaysAndMoleculeGraphNodes.Add(pathwayId, new Dictionary<Guid, List<Guid>>());
            LinkingInPathwaysAndMoleculeGraphNodes.Add(pathwayId, new Dictionary<Guid, List<Guid>>());

            // outgoing pathway links
            Dictionary<Guid, List<Guid>> linkingPwAndMol = ServerPathway.GetConnectedPathwaysWithEntities(pathwayId);
            ServerPathway[] linkedPathways = ServerPathway.GetPathwaysLinkedToPathway(pathwayId);
            foreach (ServerPathway toPw in linkedPathways) //(Guid toPwId in linkingPwAndMol.Keys)
            {
                if (!LinkingPathwaysAndMoleculeGraphNodes[pathwayId].ContainsKey(toPw.ID))
                    LinkingPathwaysAndMoleculeGraphNodes[pathwayId].Add(toPw.ID, new List<Guid>());

                if (addLinkingPathways)
                {
                    if (!CollapsedLinkingPathways.ContainsKey(toPw.ID))
                    {
                        CollapsedLinkingPathways.Add(toPw.ID, toPw);

                        // organism/groups for pathway node
                        if (!PathwayOrgGroups.ContainsKey(toPw.ID))
                        {
                            List<Guid> groupIds = ServerCatalyze.GetOrganismGroupsForPathway(toPw.ID);
                            PathwayOrgGroups.Add(toPw.ID, groupIds);
                        }
                    }
                }

                foreach (Guid molId in linkingPwAndMol[toPw.ID])
                {
                    // add molecule if needed
                    Guid entityGraphNodeId = FillMoleculeInPathway(pathwayId, molId, null, false);

                    if (entityGraphNodeId != Guid.Empty)
                        LinkingPathwaysAndMoleculeGraphNodes[pathwayId][toPw.ID].Add(entityGraphNodeId);
                }
            }

            // incoming pathway links
            Dictionary<Guid, List<Guid>> linkingInPwAndMol = ServerPathway.GetConnectedPathwaysWithEntitiesIncoming(pathwayId);
            ServerPathway[] linkedInPathways = ServerPathway.GetPathwaysLinkedToPathwayIncoming(pathwayId);
            foreach (ServerPathway fromPw in linkedInPathways) 
            {
                if (!LinkingInPathwaysAndMoleculeGraphNodes[pathwayId].ContainsKey(fromPw.ID))
                    LinkingInPathwaysAndMoleculeGraphNodes[pathwayId].Add(fromPw.ID, new List<Guid>());

                if (addLinkingPathways)
                {
                    if (!CollapsedLinkingPathways.ContainsKey(fromPw.ID))
                    {
                        CollapsedLinkingPathways.Add(fromPw.ID, fromPw);

                        // organism/groups for pathway node
                        if (!PathwayOrgGroups.ContainsKey(fromPw.ID))
                        {
                            List<Guid> groupIds = ServerCatalyze.GetOrganismGroupsForPathway(fromPw.ID);
                            PathwayOrgGroups.Add(fromPw.ID, groupIds);
                        }
                    }
                }

                foreach (Guid molId in linkingInPwAndMol[fromPw.ID])
                {
                    // add molecule if needed
                    Guid entityGraphNodeId = FillMoleculeInPathway(fromPw.ID, molId, null, false);

                    if (entityGraphNodeId != Guid.Empty)
                        LinkingInPathwaysAndMoleculeGraphNodes[pathwayId][fromPw.ID].Add(entityGraphNodeId);
                }
            }
        }

        private Guid FillMoleculeInPathway(Guid pathwayId, Guid molId, IGraphMolecularEntity me, bool explicitAdd)
        {
            Guid entityGraphNodeId = GraphNodeManager.GetEntityGraphNodeId(pathwayId, molId);
            if (!MoleculeGraphNodes.ContainsKey(entityGraphNodeId))
            {
                if (explicitAdd && !ExplicitMoleculeGraphNodes.ContainsKey(entityGraphNodeId))
                    ExplicitMoleculeGraphNodes.Add(entityGraphNodeId, entityGraphNodeId);

                if (!explicitAdd && ExplicitMoleculeGraphNodes.Count > 0)
                    return Guid.Empty; // we're not asking for this molecule

                MoleculeGraphNodes.Add(entityGraphNodeId, molId);
                MoleculeGraphNodePathways.Add(entityGraphNodeId, new List<Guid>());
            }
            if (!MoleculeGraphNodePathways[entityGraphNodeId].Contains(pathwayId))
                MoleculeGraphNodePathways[entityGraphNodeId].Add(pathwayId);

            if (!Molecules.ContainsKey(molId))
            {
                if (me == null)
                    me = ServerMolecularEntity.Load(molId);
                Molecules.Add(molId, me);
            }
            else
            {
                // already in cache!
            }

            return entityGraphNodeId;
        }

        private Guid FillMoleculeInPathway(Guid pathwayId, Guid molId, IGraphMolecularEntity me, bool explicitAdd, int tissueId)
        {

            Guid entityGraphNodeId = GraphNodeManager.GetEntityGraphNodeId(pathwayId, molId, tissueId);
            if (!MoleculeGraphNodes.ContainsKey(entityGraphNodeId))
            {
                if (explicitAdd && !ExplicitMoleculeGraphNodes.ContainsKey(entityGraphNodeId))
                    ExplicitMoleculeGraphNodes.Add(entityGraphNodeId, entityGraphNodeId);

                if (!explicitAdd && explicitMoleculesAdded) //ExplicitMoleculeGraphNodes.Count > 0)
                    return Guid.Empty; // we're not asking for this molecule

                MoleculeGraphNodes.Add(entityGraphNodeId, molId);
                MoleculeGraphNodePathways.Add(entityGraphNodeId, new List<Guid>());
            }
            if (!MoleculeGraphNodePathways[entityGraphNodeId].Contains(pathwayId))
                MoleculeGraphNodePathways[entityGraphNodeId].Add(pathwayId);

            if (!Molecules.ContainsKey(molId))
            {
                if (me == null)
                    me = ServerMolecularEntity.Load(molId);
                Molecules.Add(molId, me);
            }
            else
            {
                // already in cache!
            }

            return entityGraphNodeId;
        }


        public override void FillCacheForGenericProcess(Guid genericProcessGraphNodeId)
        {
            // convert graphNodeId to generic process ID and pathway ID
            Guid genericProcessId;
            Guid pathwayId;
            GraphNodeManager.GetPathwayAndGenericProcessId(genericProcessGraphNodeId, out pathwayId, out genericProcessId);

            if (!GenericProcessGraphNodes.ContainsKey(genericProcessGraphNodeId))
            {
                GenericProcessGraphNodes.Add(genericProcessGraphNodeId, genericProcessId);
                GenericProcessGraphNodePathways.Add(genericProcessGraphNodeId, new List<Guid>());
                GenericProcessGraphNodePathways[genericProcessGraphNodeId].Add(pathwayId);
            }
            else
            {
                if (!GenericProcessGraphNodePathways[genericProcessGraphNodeId].Contains(pathwayId))
                    GenericProcessGraphNodePathways[genericProcessGraphNodeId].Add(pathwayId);
                return; // already in cache
            }

            // load generic process
            if (!GenericProcesses.ContainsKey(genericProcessId))
            {
                GenericProcesses.Add(genericProcessId, ServerGenericProcess.Load(genericProcessId));
            }
            else
            {
                return; // already in the cache!
            }

            // specific process id -> generic process id mapping table
            Dictionary<Guid, Guid> specificToGeneric = ServerGenericProcess.GetProcessMappingTableForGenericProcess(genericProcessId);
            AddValues(SpecificToGenericProcessIdTable, specificToGeneric);
            // generic process -> specific process mapping
            AddToGenericToSpecificProcessTable(specificToGeneric);


            // load all process entities
            if (!GenericProcessEntities.ContainsKey(genericProcessId))
            {
                GenericProcessEntities.Add(genericProcessId, new List<IGraphProcessEntity>());
            }
            GenericProcessEntities[genericProcessId].AddRange(ServerProcessEntity.GetAllForGenericProcess(genericProcessId));
            
            // load all process entity molecules
            ServerMolecularEntity[] peMolecules = ServerProcessEntity.GetProcessEntitiesMoleculesForGenericProcess(genericProcessId);
            foreach (ServerMolecularEntity me in peMolecules)
            {
                //if (!Molecules.ContainsKey(me.ID))
                //    Molecules.Add(me.ID, me);

                // graph node for molecule
                FillMoleculeInPathway(pathwayId, me.ID, me, true);

                //Guid entityGraphNodeId = GraphNodeManager.GetEntityGraphNodeId(pathwayId, me.ID);
                //if (!MoleculeGraphNodes.ContainsKey(entityGraphNodeId))
                //{
                //    MoleculeGraphNodes.Add(entityGraphNodeId, me.ID);
                //    MoleculeGraphNodePathways.Add(entityGraphNodeId, new List<Guid>());
                //}
                //if (!MoleculeGraphNodePathways[entityGraphNodeId].Contains(pathwayId))
                //    MoleculeGraphNodePathways[entityGraphNodeId].Add(pathwayId);
            }

            // load all gene products
            //if (!GenericProcessGeneProducts.ContainsKey(genericProcessId))
            //{
            //    GenericProcessGeneProducts.Add(genericProcessId, new List<ServerGeneProduct>());
            //}
            //GenericProcessGeneProducts[genericProcessId].AddRange(ServerCatalyze.GetAllGeneProductsForGenericProcess(genericProcessId));
            List<ServerGeneProduct> geneProducts = new List<ServerGeneProduct>(ServerCatalyze.GetAllGeneProductsForGenericProcess(genericProcessId));
            foreach (ServerGeneProduct gp in geneProducts)
            {
                //if (!Molecules.ContainsKey(gp.ID))
                //    Molecules.Add(gp.ID, gp);

                // graph node for molecule
                FillMoleculeInPathway(pathwayId, gp.ID, gp, true);
                //Guid entityGraphNodeId = GraphNodeManager.GetEntityGraphNodeId(pathwayId, gp.ID);
                //if (!MoleculeGraphNodes.ContainsKey(entityGraphNodeId))
                //{
                //    MoleculeGraphNodes.Add(entityGraphNodeId, gp.ID);
                //    MoleculeGraphNodePathways.Add(entityGraphNodeId, new List<Guid>());
                //}
                //if (!MoleculeGraphNodePathways[entityGraphNodeId].Contains(pathwayId))
                //    MoleculeGraphNodePathways[entityGraphNodeId].Add(pathwayId);
            }

            // load catalyzes [i.e. gene products to process mappings]     
            AddValues(GenericProcessGeneProductMappings, ServerCatalyze.GetGeneProductsForGenericProcesses(genericProcessId));

            // load all EC#'s for generic processes
            ServerECNumber[] ecNumbers = ServerCatalyze.GetECNumbersForGenericProcess(genericProcessId);
            List<string> ecList = new List<string>();
            foreach (ServerECNumber ecNum in ecNumbers)
            {
                ecList.Add(ecNum.ECNumber);
            }
            GenericProcessECNumbers.Add(genericProcessId, ecList);

            // load all organism/groups for generic processes
            AddValues(GenericProcessOrgGroups, ServerCatalyze.GetOrganismGroupsForGenericProcesses(genericProcessId));

            // load all catalyzes tuples seperately
            //AddValues(GenericProcessCatalyzes, ServerCatalyze.LoadAllForGenericProcesses(genericProcessId));
            ServerCatalyze[] catalyzes = ServerCatalyze.LoadAllForGenericProcesses(genericProcessId);
            foreach (ServerCatalyze cat in catalyzes)
            {
                if (!GenericProcessCatalyzes.ContainsKey(genericProcessId))
                    GenericProcessCatalyzes.Add(genericProcessId, new List<IGraphCatalyze>());
                GenericProcessCatalyzes[genericProcessId].Add(cat);
            }

        }

        public override IGraphBasicMolecule[] GetAllCommonMolecules()
        {
            return (IGraphBasicMolecule[])PathwaysLib.ServerObjects.ServerBasicMolecule.GetAllCommonMolecules();
        }

        #endregion

        public override Guid MoleculeIdToGraphNodeId(Guid pathwayId, Guid moleculeId)
        {
            return GraphNodeManager.GetEntityGraphNodeId(pathwayId, moleculeId);
        }

        public override Guid GenericProcessIdToGraphNodeId(Guid pathwayId, Guid genericProcessId)
        {
            return GraphNodeManager.GetProcessGraphNodeId(pathwayId, genericProcessId);
        }

        public override Guid GetPathwayForProcessGraphNode(Guid genericProcessGraphNodeId)
        {
            return GraphNodeManager.GetGenericProcessPathwayId(genericProcessGraphNodeId);
            //List<Guid> pw = GraphNodeManager.GetProcessPathways(genericProcessGraphNodeId);
            //if (pw.Count > 0)
            //    return pw[0];
            //return Guid.Empty;
        }

        public override Guid GetPathwayForMoleculeGraphNode(Guid moleculeGraphNodeId)
        {
            return GraphNodeManager.GetEntityPathwayId(moleculeGraphNodeId);
            //List<Guid> pw = GraphNodeManager.GetEntityPathways(moleculeGraphNodeId);
            //if (pw.Count > 0)
            //    return pw[0];
            //return Guid.Empty;
        }    
    
    }
}
