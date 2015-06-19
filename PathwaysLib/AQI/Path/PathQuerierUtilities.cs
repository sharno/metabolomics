using AQILib;
using PathQueryLib;
using PathwaysLib.PathQuery;
using PathwaysLib.ServerObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    /// <summary>
    /// Utilities that are used in each path querier
    /// </summary>
    public class PathQuerierUtilities
    {
        /// <summary>
        /// Encapsulates the different kinds of entities
        /// </summary>
        public enum EntityType { Pathway, Process, Molecule, Organism }

        ///// <summary>
        ///// Grabs a nested dictionary/list of the data vluaes stored in the XML
        ///// </summary>
        ///// <param name="node"></param>
        ///// <param name="nodeTypeNames"></param>
        ///// <param name="childPtr"></param>
        ///// <returns></returns>
        //public static Dictionary<string, List<Dictionary<string, Dictionary<string, List<string>>>>> FetchChildNodesInOrderByType(QNode node, string[] nodeTypeNames, ref int childPtr)
        //{
        //    Dictionary<string, List<Dictionary<string, Dictionary<string, List<string>>>>> retVal = new Dictionary<string, List<Dictionary<string, Dictionary<string, List<string>>>>>();

        //    foreach(string nodeTypeName in nodeTypeNames)
        //        retVal.Add(nodeTypeName, new List<Dictionary<string, Dictionary<string, List<string>>>>());

        //    List<string> nodeTypesList = new List<string>(nodeTypeNames);
        //    while(childPtr < node.Children.Count)
        //    {
        //        if(!nodeTypesList.Contains(node.Children[childPtr].NodeTypeName))
        //            break;

        //        for(int i = 0; i < nodeTypeNames.Length; i++)
        //            if(node.Children[childPtr].NodeTypeName == nodeTypeNames[i])
        //                retVal[nodeTypeNames[i]].Add(FetchNode(node.Children[childPtr]));

        //        childPtr += 1;
        //    }

        //    return retVal;
        //}

        ///// <summary>
        ///// Helps FetchChildNodesInOrderByType by grabbing the values from a single node
        ///// </summary>
        ///// <param name="node"></param>
        ///// <returns></returns>
        //public static Dictionary<string, Dictionary<string, List<string>>> FetchNode(QNode node)
        //{
        //    Dictionary<string, Dictionary<string, List<string>>> retVal = new Dictionary<string, Dictionary<string, List<string>>>();

        //    foreach(KeyValuePair<string, QField> kvpField in node.Fields)
        //    {
        //        string fieldName = kvpField.Key;
        //        QField fieldObj = kvpField.Value;

        //        retVal.Add(fieldName, new Dictionary<string, List<string>>());

        //        foreach(KeyValuePair<string, List<QInput>> kvpInputList in fieldObj.Inputs)
        //        {
        //            string inputName = kvpInputList.Key;
        //            List<QInput> inputListObj = kvpInputList.Value;

        //            retVal[fieldName].Add(inputName, new List<string>());

        //            foreach(QInput i in inputListObj)
        //                retVal[fieldName][inputName].Add(i.Value);
        //        }
        //    }

        //    return retVal;
        //}

        /// <summary>
        /// Gets the node's ID from the database given the type and the name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="idMissingError"></param>
        /// <returns></returns>
        public static List<Guid> FetchIdsFromName(string name, EntityType type, Guid contextPathwayId, string idMissingError)
        {
            List<Guid> ids = new List<Guid>();
            switch(type)
            {
                case EntityType.Pathway:
                    // get first pathway with this name (should be only 1)
                    foreach (ServerPathway pathway in ServerPathway.FindPathways(name, SearchMethod.ExactMatch, 0, 1))
                    {
                        ids.Add(pathway.ID);
                    }
                    break;
                case EntityType.Process:
                    foreach (ServerProcess process in ServerProcess.FindProcesses(name, SearchMethod.ExactMatch))
                    {
                        //BE: converting to GraphNodeId's
                        if (contextPathwayId != Guid.Empty)
                        {
                            // we know the pathway context
                            Guid procGraphId = GraphNodeManager.GetProcessGraphNodeId(contextPathwayId, process.GenericProcessID);
                            if (procGraphId == process.GenericProcessID)
                            {
                                // process not in this pathway
                                continue;
                            }
                            if (!ids.Contains(procGraphId))
                                ids.Add(procGraphId); // found the process graph node for this pathway
                            break; //HACK: don't try any more (assuming only one process by this name in the pathway)
                        }
                        else
                        {
                            // no pathway context; get all graph nodes for this process!
                            List<Guid> pws = GraphNodeManager.GetProcessPathways(process.GenericProcessID);
                            foreach (Guid pw in pws)
                            {
                                Guid procGraphId = GraphNodeManager.GetProcessGraphNodeId(pw, process.GenericProcessID);
                                if (!ids.Contains(procGraphId))
                                    ids.Add(procGraphId);                
                            }
                        }
                    }
                    break;
                case EntityType.Molecule:
                    foreach (ServerMolecularEntity molecule in ServerMolecularEntity.FindMolecularEntities(name, SearchMethod.ExactMatch))
                    {
                        //BE: converting to GraphNodeId's
                        if (contextPathwayId != Guid.Empty)
                        {
                            // we know the pathway context
                            Guid molGraphId = GraphNodeManager.GetEntityGraphNodeId(contextPathwayId, molecule.ID);
                            if (molGraphId == molecule.ID)
                            {
                                // molecule not in this pathway
                                continue;
                            }
                            if (!ids.Contains(molGraphId))
                                ids.Add(molGraphId); // found the molecule graph node for this pathway
                            break; //HACK: don't try any more (assuming only one molecule by this name in the pathway)
                        }
                        else
                        {
                            // no pathway context; get all graph nodes for this molecule!
                            List<Guid> pws = GraphNodeManager.GetEntityPathways(molecule.ID);
                            foreach (Guid pw in pws)
                            {
                                Guid molGraphId = GraphNodeManager.GetEntityGraphNodeId(pw, molecule.ID);
                                if (!ids.Contains(molGraphId))
                                    ids.Add(molGraphId);
                            }
                        }

                    }
                    break;
                case EntityType.Organism:
                    // get first organism with this name (should be only 1)
                    if(name.Contains(" [") && name.Contains("]"))
                        name = name.Substring(0, name.LastIndexOf(" ["));
                    foreach (ServerOrganismGroup organism in ServerOrganismGroup.FindByName(name, SearchMethod.ExactMatch, 0, 1))
                    {
                        ids.Add(organism.ID);
                    }
                    break;
                default:
                    throw new PathQuerierException("Internal EntityType error.");
            }

            if(ids.Count <= 0)
                throw new PathQuerierException(String.Format(idMissingError, name));

            return ids;
        }

        ///// <summary>
        ///// Starts at a particular point in the XML document and continues until all of the desired child nodes are found and parsed
        ///// </summary>
        ///// <param name="parentNode">The parent node to start at</param>
        ///// <param name="nodeTypeNames">The node types to fetch</param>
        ///// <param name="nodeTypeTypes"></param>
        ///// <param name="nameMissingErrors"></param>
        ///// <param name="childPtr">The child # to start at</param>
        ///// <param name="ids">The IDs that are found</param>
        ///// <param name="names">The names that are found (corresponds with ids)</param>
        //public static void FetchNodes(QNode parentNode, string[] nodeTypeNames, EntityType[] nodeTypeTypes, string[] nameMissingErrors, ref int childPtr, out Dictionary<string, List<Guid>> ids, out Dictionary<string, List<string>> names)
        //{
        //    if(nodeTypeNames.Length != nodeTypeTypes.Length)
        //        throw new ArgumentOutOfRangeException("nodeTypeTypes", "FetchNodes: nodeTypeTypes is not the same length as nodeTypeNames!");
        //    if(nodeTypeNames.Length != nameMissingErrors.Length)
        //        throw new ArgumentOutOfRangeException("nameMissingErrors", "FetchNodes: nameMissingErrors is not the same length as nodeTypeNames!");

        //    Dictionary<EntityType, string> entityToTitle = new Dictionary<EntityType, string>();
        //    entityToTitle.Add(EntityType.Pathway, "Pathway");
        //    entityToTitle.Add(EntityType.Process, "Process");
        //    entityToTitle.Add(EntityType.Molecule, "Molecule");
        //    entityToTitle.Add(EntityType.Organism, "Organism");

        //    Dictionary<string, List<Dictionary<string, Dictionary<string, List<string>>>>> nodesData = FetchChildNodesInOrderByType(parentNode, nodeTypeNames, ref childPtr);
        //    ids = new Dictionary<string, List<Guid>>();
        //    names = new Dictionary<string, List<string>>();

        //    foreach(string nodeTypeName in nodeTypeNames)
        //    {
        //        ids.Add(nodeTypeName, new List<Guid>());
        //        names.Add(nodeTypeName, new List<string>());
        //    }

        //    Dictionary<string, EntityType> nodeTypeTypesDict = new Dictionary<string, EntityType>();
        //    Dictionary<string, string> nameMissingErrorsDict = new Dictionary<string, string>();
        //    Dictionary<string, string> paramNamesDict = new Dictionary<string, string>();

        //    for(int i = 0; i < nodeTypeNames.Length; i++)
        //    {
        //        nodeTypeTypesDict[nodeTypeNames[i]] = nodeTypeTypes[i];
        //        nameMissingErrorsDict[nodeTypeNames[i]] = nameMissingErrors[i];

        //        string paramName =
        //            nodeTypeTypes[i] == EntityType.Pathway ? "pathway_name" :
        //            nodeTypeTypes[i] == EntityType.Process ? "process_name" :
        //            nodeTypeTypes[i] == EntityType.Molecule ? "molecule_name" :
        //            nodeTypeTypes[i] == EntityType.Organism ? "organism_name" : "";

        //        if(paramName == "")
        //            throw new PathQuerierException("Internal paramName error.");

        //        paramNamesDict[nodeTypeNames[i]] = paramName;
        //    }

        //    foreach(KeyValuePair<string, List<Dictionary<string, Dictionary<string, List<string>>>>> kvp in nodesData)
        //    {
        //        string nodeTypeName = kvp.Key;
        //        List<Dictionary<string, Dictionary<string, List<string>>>> nodeDataList = kvp.Value;

        //        foreach(Dictionary<string, Dictionary<string, List<string>>> nodeData in nodeDataList)
        //        {
        //            if(nodeData[paramNamesDict[nodeTypeName]][paramNamesDict[nodeTypeName]].Count <= 0)
        //                throw new PathQuerierException(nameMissingErrorsDict[nodeTypeName]);

        //            string name = nodeData[paramNamesDict[nodeTypeName]][paramNamesDict[nodeTypeName]][0];

        //            //BE: changed this to give a list of IDs 
        //            List<Guid> ids = FetchIdsFromName(name, nodeTypeTypesDict[nodeTypeName], String.Format("{0} '{{0}}' not found in database.", entityToTitle[nodeTypeTypesDict[nodeTypeName]]));
        //            foreach (Guid id in ids)
        //            {
        //                ids[nodeTypeName].Add(id);
        //                names[nodeTypeName].Add(name);
        //            }
        //        }
        //    }

        //    return;
        //}

        ///// <summary>
        ///// Grabs a list of a INodes to go with the node types
        ///// </summary>
        ///// <param name="parentNode"></param>
        ///// <param name="nodeTypeNames"></param>
        ///// <param name="nodeTypeTypes"></param>
        ///// <param name="nodeTypes"></param>
        ///// <param name="nameMissingErrors"></param>
        ///// <param name="childPtr"></param>
        ///// <returns></returns>
        //public static Dictionary<string, List<INode>> FetchMultipleNodesINodes(QNode parentNode, string[] nodeTypeNames, EntityType[] nodeTypeTypes, NodeType[] nodeTypes, string[] nameMissingErrors, ref int childPtr)
        //{
        //    Dictionary<string, List<INode>> retVal = new Dictionary<string, List<INode>>();
        //    foreach(string nodeTypeName in nodeTypeNames)
        //        retVal.Add(nodeTypeName, new List<INode>());

        //    Dictionary<string, NodeType> nodeTypesDict = new Dictionary<string,NodeType>();
        //    for(int i = 0; i < nodeTypeNames.Length; i++)
        //        nodeTypesDict[nodeTypeNames[i]] = nodeTypes[i];

        //    Dictionary<string, List<Guid>> ids;
        //    Dictionary<string, List<string>> names;
        //    FetchNodes(parentNode, nodeTypeNames, nodeTypeTypes, nameMissingErrors, ref childPtr, out ids, out names);

        //    foreach(string nodeTypeName in nodeTypeNames)
        //        for(int i = 0; i < ids[nodeTypeName].Count; i++)
        //            retVal[nodeTypeName].Add(new Node(ids[nodeTypeName][i], nodeTypesDict[nodeTypeName], names[nodeTypeName][i]));

        //    return retVal;
        //}

        ///// <summary>
        ///// Grabs a list of INodes that are hops in the path query to go with the node types specified
        ///// </summary>
        ///// <param name="parentNode"></param>
        ///// <param name="nodeTypeNames"></param>
        ///// <param name="nodeTypeTypes"></param>
        ///// <param name="nodeTypes"></param>
        ///// <param name="nameMissingErrors"></param>
        ///// <param name="inclNodeTypeNames"></param>
        ///// <param name="inclNodeTypeTypes"></param>
        ///// <param name="inclNodeTypes"></param>
        ///// <param name="inclNameMissingErrors"></param>
        ///// <param name="exclNodeTypeNames"></param>
        ///// <param name="exclNodeTypeTypes"></param>
        ///// <param name="exclNodeTypes"></param>
        ///// <param name="exclNameMissingErrors"></param>
        ///// <param name="childPtr"></param>
        ///// <returns></returns>
        //public static List<QueryPathParametersToNode> FetchMultipleNodesToNodes(QNode parentNode, string[] nodeTypeNames, EntityType[] nodeTypeTypes, NodeType[] nodeTypes, string[] nameMissingErrors, string[] inclNodeTypeNames, EntityType[] inclNodeTypeTypes, NodeType[] inclNodeTypes, string[] inclNameMissingErrors, string[] exclNodeTypeNames, EntityType[] exclNodeTypeTypes, NodeType[] exclNodeTypes, string[] exclNameMissingErrors, ref int childPtr)
        //{
        //    List<QueryPathParametersToNode> retVal = new List<QueryPathParametersToNode>();

        //    int tempChildPtr = childPtr;
        //    Dictionary<string, List<INode>> iNodes = FetchMultipleNodesINodes(parentNode, nodeTypeNames, nodeTypeTypes, nodeTypes, nameMissingErrors, ref tempChildPtr);

        //    Dictionary<string, int> iNodePtrs = new Dictionary<string, int>();
        //    foreach(string nodeTypeName in nodeTypeNames)
        //        iNodePtrs.Add(nodeTypeName, 0);

        //    for(int i = childPtr; i < tempChildPtr; i++)
        //    {
        //        QNode childNode = parentNode.Children[i];
        //        string childNodeType = childNode.NodeTypeName;
        //        int childNodePtr = 0;
        //        int childINodePtr = iNodePtrs[childNodeType];
        //        iNodePtrs[childNodeType] += 1;

        //        INode toNode;
        //        int minLength;
        //        int maxLength;
        //        List<INode> includingRestrictions;
        //        List<INode> excludingRestrictions;

        //        toNode = iNodes[childNodeType][childINodePtr];

        //        FetchLengths(childNode, out minLength, out maxLength);

        //        string[] inclNameMissingErrorsFormatted = new string[inclNameMissingErrors.Length];
        //        for(int j = 0; j < inclNameMissingErrors.Length; j++)
        //            inclNameMissingErrorsFormatted[j] = String.Format(inclNameMissingErrors[j], toNode.Label);
        //        Dictionary<string, List<INode>> includingRestrictionsDict =
        //            PathQuerierUtilities.FetchMultipleNodesINodes(childNode, inclNodeTypeNames, inclNodeTypeTypes, inclNodeTypes, inclNameMissingErrorsFormatted, ref childNodePtr);
        //        includingRestrictions = new List<INode>();
        //        foreach(string key in includingRestrictionsDict.Keys)
        //            includingRestrictions.AddRange(includingRestrictionsDict[key]);

        //        string[] exclNameMissingErrorsFormatted = new string[exclNameMissingErrors.Length];
        //        for(int j = 0; j < exclNameMissingErrors.Length; j++)
        //            exclNameMissingErrorsFormatted[j] = String.Format(exclNameMissingErrors[j], toNode.Label);
        //        Dictionary<string, List<INode>> excludingRestrictionsDict =
        //            PathQuerierUtilities.FetchMultipleNodesINodes(childNode, exclNodeTypeNames, exclNodeTypeTypes, exclNodeTypes, exclNameMissingErrorsFormatted, ref childNodePtr);
        //        excludingRestrictions = new List<INode>();
        //        foreach(string key in excludingRestrictionsDict.Keys)
        //            excludingRestrictions.AddRange(excludingRestrictionsDict[key]);

        //        retVal.Add(new QueryPathParametersToNode(toNode, minLength, maxLength, includingRestrictions, excludingRestrictions));
        //    }
            
        //    childPtr = tempChildPtr;
        //    return retVal;
        //}

        /// <summary>
        /// Grab the boolean flag for common molecules from the XML
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool FetchUseCommonMolecules(QNode node)
        {
            string value = node.SelectSingleInputValue("@common_molecules:common_molecules");
            switch (value)
            {
                case "true":
                    return true;
                case "false":
                    return false;
                case null:
                    throw new PathQuerierException("Cannot find common molecules specification in Xml.");
                default:
                    throw new PathQuerierException("Invalid common molecules value in Xml.");
            }

            //Dictionary<string, Dictionary<string, List<string>>> nodeData = FetchNode(node);

            //if(   nodeData.ContainsKey("common_molecules")
            //   && nodeData["common_molecules"].ContainsKey("common_molecules")
            //   && nodeData["common_molecules"]["common_molecules"].Count > 0)
            //{
            //    if(nodeData["common_molecules"]["common_molecules"][0].Equals("true"))
            //        return true;
            //    else if(nodeData["common_molecules"]["common_molecules"][0].Equals("false"))
            //        return false;
            //    else
            //        throw new PathQuerierException("Invalid common molecules value in Xml.");
            //}
            //else
            //{
            //    throw new PathQuerierException("Cannot find common molecules specification in Xml.");
            //}
        }

        /// <summary>
        /// Grab the EdgeType flag for the search direction from the XML
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static EdgeType FetchSearchDirection(QNode node)
        {
            string value = node.SelectSingleInputValue("@search_direction:search_direction");
            switch (value)
            {
                case "Undirected":
                    return EdgeType.Undirected;
                case "Downstream":
                    return EdgeType.Directed;
                case "Upstream":
                    return EdgeType.DirectedReversed;
                case null:
                    throw new PathQuerierException("Cannot find search direction specification in Xml.");
                default:
                    throw new PathQuerierException("Invalid search direction value in Xml.");
            }

            //Dictionary<string, Dictionary<string, List<string>>> nodeData = FetchNode(node);

            //if(nodeData.ContainsKey("search_direction")
            //   && nodeData["search_direction"].ContainsKey("search_direction")
            //   && nodeData["search_direction"]["search_direction"].Count > 0)
            //{
            //    if(nodeData["search_direction"]["search_direction"][0].Equals("Undirected"))
            //        return EdgeType.Undirected;
            //    else if(nodeData["search_direction"]["search_direction"][0].Equals("Downstream"))
            //        return EdgeType.Directed;
            //    else if(nodeData["search_direction"]["search_direction"][0].Equals("Upstream"))
            //        return EdgeType.DirectedReversed;
            //    else
            //        throw new PathQuerierException("Invalid search direction value in Xml.");
            //}
            //else
            //{
            //    throw new PathQuerierException("Cannot find search direction specification in Xml.");
            //}

            // No need for return here
            // All code paths either return a value or throw an exception
        }

        /// <summary>
        /// Grab the min/max length from the XML
        /// </summary>
        /// <param name="node"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        public static void FetchLengths(QNode node, out int minLength, out int maxLength)
        {
            QField pathRes = node.SelectSingleField("@path_restrictions_length");
            if (pathRes == null)
                throw new PathQuerierException("Cannot find length specification in Xml.");

            string minVal = pathRes.SelectSingleInputValue("_min");
            if (minVal == null)
                minLength = 0;
            else
                minLength = int.Parse(minVal);

            string maxVal = pathRes.SelectSingleInputValue("_max");
            if (maxVal == null)
                maxLength = int.MaxValue;
            else
                maxLength = int.Parse(maxVal);

            //Dictionary<string, Dictionary<string, List<string>>> nodeData = FetchNode(node);

            //if(nodeData.ContainsKey("path_restrictions_length"))
            //{
            //    if(   nodeData["path_restrictions_length"].ContainsKey("_min")
            //       && nodeData["path_restrictions_length"]["_min"].Count > 0
            //       && nodeData["path_restrictions_length"]["_min"][0].Length > 0)
            //        minLength = int.Parse(nodeData["path_restrictions_length"]["_min"][0]);
            //    else
            //        minLength = 0;

            //    if(   nodeData["path_restrictions_length"].ContainsKey("_max")
            //       && nodeData["path_restrictions_length"]["_max"].Count > 0
            //       && nodeData["path_restrictions_length"]["_max"][0].Length > 0)
            //        maxLength = int.Parse(nodeData["path_restrictions_length"]["_max"][0]);
            //    else
            //        maxLength = int.MaxValue;
            //}
            //else
            //{
            //    throw new PathQuerierException("Cannot find length specification in Xml.");
            //}
        }

        public static NodeType FetchFindType(QNode node)
        {
            string value = node.SelectSingleInputValue("@find_type:find_type");
            switch (value)
            {
                case "":
                    return NodeType.Any;
                case "Molecules":
                    return NodeType.Node;
                case "Processes":
                    return NodeType.Edge;
                case null:
                    return NodeType.Any;
                default:
                    throw new PathQuerierException("Invalid find type value in Xml.");
            }

            //Dictionary<string, Dictionary<string, List<string>>> nodeData = FetchNode(node);

            //if (nodeData.ContainsKey("find_type")
            //   && nodeData["find_type"].ContainsKey("find_type")
            //   && nodeData["find_type"]["find_type"].Count > 0)
            //{
            //    if (nodeData["find_type"]["find_type"][0].Equals(string.Empty))
            //        return NodeType.Any;
            //    else if (nodeData["find_type"]["find_type"][0].Equals("Molecules"))
            //        return NodeType.Node;
            //    else if (nodeData["find_type"]["find_type"][0].Equals("Processes"))
            //        return NodeType.Edge;
            //    else
            //        throw new PathQuerierException("Invalid find type value in Xml.");
            //}
            //else
            //{
            //    //throw new PathQuerierException("Cannot locate find type specification in Xml.");
            //    return NodeType.Any;
            //}
        }

        public static Guid FetchFrom(QNode node, List<Guid> pathwayRestrictionsIds, bool pwLinks)
        {
            Guid fromNodeId;
            // from molecule
            List<string> fromMolNames = node.SelectInputValues("path_from_molecule@molecule_name:molecule_name");
            List<string> fromMolPathways = node.SelectInputValues("path_from_molecule@pathway_name:pathway_name");

            // from process
            List<string> fromProcNames = node.SelectInputValues("path_from_process@process_name:process_name");
            List<string> fromProcPathways = node.SelectInputValues("path_from_process@pathway_name:pathway_name");

            // from pathways
            List<string> fromPathwayNames = node.SelectInputValues("path_from_pathway@pathway_name:pathway_name");

            if (fromMolNames.Count == 1 && fromProcNames.Count == 0 && fromPathwayNames.Count == 0)
            {
                // starting from a molecule
                fromNodeId = FetchMoleculeID(pathwayRestrictionsIds, fromMolNames, fromMolPathways, pwLinks);
            }
            else if (fromMolNames.Count == 0 && fromProcNames.Count == 1 && fromPathwayNames.Count == 0)
            {
                // starting from a process
                fromNodeId = FetchProcessID(pathwayRestrictionsIds, fromProcNames, fromProcPathways);
            }
            else if (fromMolNames.Count == 0 && fromProcNames.Count == 0 && fromPathwayNames.Count == 1)
            {
                // starting from a pathway
                fromNodeId = PathQuerierUtilities.FetchIdsFromName(fromPathwayNames[0], PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database.")[0];
            }
            else
                throw new PathQuerierException(@"Please specify a single ""from"" entity.");
            return fromNodeId;
        }

        public static QueryPathParametersToNode FetchTo(QNode q, List<Guid> pathwayRestrictionsIds, bool pwLinks)
        {
            Guid toNodeId;
            int toMin, toMax;
            List<Guid> toInclude = new List<Guid>();
            List<Guid> toExclude = new List<Guid>();

            // get to ID
            switch (q.NodeTypeName)
            {
                case "path_to_process":
                    List<string> toProcNames = q.SelectInputValues("@molecule_name:molecule_name");
                    List<string> toProcPathways = q.SelectInputValues("@pathway_name:pathway_name");

                    toNodeId = PathQuerierUtilities.FetchProcessID(pathwayRestrictionsIds, toProcNames, toProcPathways);
                    break;

                case "path_to_molecule":
                    List<string> toMolNames = q.SelectInputValues("@molecule_name:molecule_name");
                    List<string> toMolPathways = q.SelectInputValues("@pathway_name:pathway_name");

                    toNodeId = PathQuerierUtilities.FetchMoleculeID(pathwayRestrictionsIds, toMolNames, toMolPathways, pwLinks);
                    break;

                case "path_to_pathway":
                    List<string> toPathways = q.SelectInputValues("@pathway_name:pathway_name");
                    toNodeId = PathQuerierUtilities.FetchIdsFromName(toPathways[0], PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database.")[0];
                    break;

                default:
                    throw new PathQuerierException("Unexcepted FetchTo Node Type: " + q.NodeTypeName);
            }

            // get length
            PathQuerierUtilities.FetchLengths(q, out toMin, out toMax);

            // including
            toInclude = FetchIncluding(q, pathwayRestrictionsIds);

            // not including
            toExclude = FetchNotIncluding(q);

            return new QueryPathParametersToNode(toNodeId, toMin, toMax, toInclude, toExclude);
        }

        public static List<Guid> FetchNotIncluding(QNode node)
        {
            List<Guid> notIncludingRestrictions = new List<Guid>();

            // not including molecule
            List<string> notIncludingMolNames = node.SelectInputValues("path_restriction_not_including_molecule@molecule_name:molecule_name");
            foreach (string molName in notIncludingMolNames)
            {
                if (string.IsNullOrEmpty(molName))
                    throw new PathQuerierException("Please either specify a value for the not including molecule graph restriction or remove that empty node from the query.");

                notIncludingRestrictions.AddRange(PathQuerierUtilities.FetchIdsFromName(molName, PathQuerierUtilities.EntityType.Molecule, Guid.Empty, "Molecule '{0}' not found in database."));
            }

            // not including process
            List<string> notIncludingProcNames = node.SelectInputValues("path_restriction_not_including_process@process_name:process_name");
            foreach (string procName in notIncludingProcNames)
            {
                if (string.IsNullOrEmpty(procName))
                    throw new PathQuerierException("Please either specify a value for the not including process graph restriction or remove that empty node from the query.");

                notIncludingRestrictions.AddRange(PathQuerierUtilities.FetchIdsFromName(procName, PathQuerierUtilities.EntityType.Process, Guid.Empty, "Process '{0}' not found in database."));
            }

            // not including pathways
            List<string> notIncludingPathwayNames = node.SelectInputValues("path_restriction_not_including_pathway@pathway_name:pathway_name");
            foreach (string pathwayName in notIncludingPathwayNames)
            {
                if (string.IsNullOrEmpty(pathwayName))
                    throw new PathQuerierException("Please either specify a value for a pathway restriction or remove that empty node from the query.");

                notIncludingRestrictions.AddRange(PathQuerierUtilities.FetchIdsFromName(pathwayName, PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database."));
            }

            return notIncludingRestrictions;
        }

        public static List<Guid> FetchIncluding(QNode node, List<Guid> pathwayRestrictionsIds)
        {
            List<Guid> includingRestrictions = new List<Guid>();

            // including molecule
            List<string> includingMolNames = node.SelectInputValues("path_restriction_including_molecule@molecule_name:molecule_name");
            List<string> includingMolPathways = node.SelectInputValues("path_restriction_including_molecule@pathway_name:pathway_name");
            foreach (string molName in includingMolNames)
            {
                if (string.IsNullOrEmpty(molName))
                    throw new PathQuerierException("Please either specify a value for the including molecule graph restriction or remove that empty node from the query.");

                Guid molPwId = Guid.Empty;
                if (includingMolPathways.Count != includingMolNames.Count)
                {
                    if (pathwayRestrictionsIds != null)
                    {
                        if (pathwayRestrictionsIds.Count == 1)
                            molPwId = pathwayRestrictionsIds[0];
                        else
                            throw new PathQuerierException(@"Please specify the pathway context for molecule '{0}' or give a single pathways restriction.", molName);
                    }
                    else
                        throw new PathQuerierException(@"Please specify the pathway context for molecule '{0}'", molName);
                }
                else
                    molPwId = PathQuerierUtilities.FetchIdsFromName(includingMolPathways[0], PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database.")[0];

                includingRestrictions.AddRange(PathQuerierUtilities.FetchIdsFromName(molName, PathQuerierUtilities.EntityType.Molecule, molPwId, "Molecule '{0}' not found in database."));
            }

            // include process
            List<string> includingProcNames = node.SelectInputValues("path_restriction_including_process@process_name:process_name");
            List<string> includingProcPathways = node.SelectInputValues("path_restriction_including_process@pathway_name:pathway_name");
            foreach (string procName in includingProcNames)
            {
                if (string.IsNullOrEmpty(procName))
                    throw new PathQuerierException("Please either specify a value for the including process graph restriction or remove that empty node from the query.");

                Guid procPwId = Guid.Empty;
                if (includingProcPathways.Count != includingProcNames.Count)
                {
                    if (pathwayRestrictionsIds != null)
                    {
                        if (pathwayRestrictionsIds.Count == 1)
                            procPwId = pathwayRestrictionsIds[0];
                        else
                            throw new PathQuerierException(@"Please specify the pathway context for process '{0}' or give a single pathways restriction.", procName);
                    }
                    else
                        throw new PathQuerierException(@"Please specify the pathway context for process '{0}'.", procName);
                }
                else
                    procPwId = PathQuerierUtilities.FetchIdsFromName(includingProcPathways[0], PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database.")[0];

                includingRestrictions.AddRange(PathQuerierUtilities.FetchIdsFromName(procName, PathQuerierUtilities.EntityType.Process, procPwId, "Process '{0}' not found in database."));
            }

            // include pathways
            List<string> includingPathwayNames = node.SelectInputValues("path_restriction_including_pathway@pathway_name:pathway_name");
            foreach (string pathwayName in includingPathwayNames)
            {
                if (string.IsNullOrEmpty(pathwayName))
                    throw new PathQuerierException("Please either specify a value for the including process graph restriction or remove that empty node from the query.");

                includingRestrictions.AddRange(PathQuerierUtilities.FetchIdsFromName(pathwayName, PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database."));
            }

            return includingRestrictions;
        }

        public static Guid FetchProcessID(List<Guid> pathwayRestrictionsIds, List<string> fromProcNames, List<string> fromProcPathways)
        {
            Guid fromNodeId;
            Guid procPwId = Guid.Empty;
            if (fromProcPathways.Count != fromProcNames.Count)
            {
                if (pathwayRestrictionsIds != null)
                {
                    if (pathwayRestrictionsIds.Count == 1)
                        procPwId = pathwayRestrictionsIds[0];
                    else
                        throw new PathQuerierException(@"Please specify the pathway context for process '{0}' or give a single pathways restriction.", fromProcNames[0]);
                }
                else
                    throw new PathQuerierException(@"Please specify the pathway context for process '{0}'.", fromProcNames[0]);

            }
            else
                procPwId = PathQuerierUtilities.FetchIdsFromName(fromProcPathways[0], PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database.")[0];

            fromNodeId = PathQuerierUtilities.FetchIdsFromName(fromProcNames[0], PathQuerierUtilities.EntityType.Process, procPwId, "Process '{0}' not found in database.")[0];
            return fromNodeId;
        }

        public static Guid FetchMoleculeID(List<Guid> pathwayRestrictionsIds, List<string> fromMolNames, List<string> fromMolPathways, bool pwLinks)
        {
            Guid fromNodeId;
            Guid molPwId = Guid.Empty;
            if (fromMolPathways.Count != fromMolNames.Count)
            {
                if (!pwLinks && pathwayRestrictionsIds != null)
                {
                    if (pathwayRestrictionsIds.Count == 1)
                        molPwId = pathwayRestrictionsIds[0];
                    else
                        throw new PathQuerierException(@"Please specify the pathway context for molecule '{0}' or give a single pathways restriction.", fromMolNames[0]);
                }
                else
                    throw new PathQuerierException(@"Please specify the pathway context for molecule '{0}'.", fromMolNames[0]);

            }
            else
                molPwId = PathQuerierUtilities.FetchIdsFromName(fromMolPathways[0], PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database.")[0];

            fromNodeId = PathQuerierUtilities.FetchIdsFromName(fromMolNames[0], PathQuerierUtilities.EntityType.Molecule, molPwId, "Molecule '{0}' not found in database.")[0];
            return fromNodeId;
        }

        /// <summary>
        /// Grab an integer configuration value from the .config file
        /// </summary>
        /// <param name="key"></param>
        /// <param name="nullValue"></param>
        /// <returns></returns>
        public static int GetIntConfigValue(string key, int nullValue)
        {
            if(ConfigurationManager.AppSettings.Get(key) != null)
                return int.Parse(ConfigurationManager.AppSettings.Get(key));
            else
                return nullValue;
        }

        /// <summary>
        /// Start an IPC remoting channel to connect to the GraphService
        /// </summary>
        /// <param name="manager"></param>
        public static void ConnectToGraphService(out IManager manager)
        {
            manager = StaticManager.Instance; // use in-memory singleton for graph caching
            return;


            //// Initialize a provider for the binary data transport
            //IChannel channel = ChannelServices.GetChannel("ipc_client_graphservice");
            //if(channel == null)
            //{
            //    BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            //    provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            //    // Setup the properties
            //    System.Collections.IDictionary channelProperties = new System.Collections.Hashtable();
            //    channelProperties["name"] = "ipc_client_graphservice";
            //    channelProperties["portName"] = "GraphProviderStoreIPCClient";
            //    channelProperties["authorizedGroup"] = "Everyone";
            //    channelProperties["exclusiveAddressUse"] = false;

            //    // Create and register a new channel
            //    channel = new IpcChannel(channelProperties, null, provider);
            //    ChannelServices.RegisterChannel(channel, false);
            //}

            //// Connect to the IManager class from the GraphService
            //manager = (IManager) RemotingServices.Connect(typeof(PathQueryLib.IManager), "ipc://GraphService/Manager");
        }


    }
}