using AQILib;
using PathQueryLib;
using PathwaysLib.PathQuery;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    /// <summary>
    /// A querier for the path query on the metabolic network graph
    /// </summary>
    public class PathMetabolicNWQuerier : PathQuerierMetabolicNW
    {
        private PathMetabolicNWQuerier()
            : base()
        { }

        public PathMetabolicNWQuerier(IAQIUtil util)
            : base(util)
        { }

        public override AQILib.IQueryResults Query(QNode node)
        {
            // Start the IPC connection between this thread and GraphService
            IManager manager;
            PathQuerierUtilities.ConnectToGraphService(out manager);

            /* ----- Parse Data ----- */

            bool useCommonMolecules;
            EdgeType searchDirection;
            List<Guid> pathwayRestrictionsIds = new List<Guid>();
            List<Guid> organismRestrictionsIds = new List<Guid>();
            Guid fromNodeId;
            List<QueryPathParametersToNode> toNodes = new List<QueryPathParametersToNode>();
            int minLength;
            int maxLength;
            List<Guid> includingRestrictions = new List<Guid>();
            List<Guid> excludingRestrictions = new List<Guid>();
            int maxResultLimit;
            int maxGraphLimit;
            int timeoutLimit;
            try
            {
                // Parse common molecule checkbox
                useCommonMolecules = PathQuerierUtilities.FetchUseCommonMolecules(node);

                // Parse search direction dropdown
                searchDirection = PathQuerierUtilities.FetchSearchDirection(node);

                // pathways restrictions
                List<string> pwResNames = node.SelectInputValues("path_graph_restriction_pathway@pathway_name:pathway_name");
                foreach (string pwName in pwResNames)
                {
                    if (string.IsNullOrEmpty(pwName))
                        throw new PathQuerierException("Please either specify a value for the pathway graph restriction or remove that empty node from the query.");
                    pathwayRestrictionsIds.AddRange(PathQuerierUtilities.FetchIdsFromName(pwName, PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database."));
                }

                // organism restrictions
                List<string> orgResNames = node.SelectInputValues("path_graph_restriction_organism@organism_name:organism_name");
                foreach (string orgName in orgResNames)
                {
                    if (string.IsNullOrEmpty(orgName))
                        throw new PathQuerierException("Please either specify a value for the organism graph restriction or remove that empty node from the query.");
                    organismRestrictionsIds.AddRange(PathQuerierUtilities.FetchIdsFromName(orgName, PathQuerierUtilities.EntityType.Organism, Guid.Empty, "Organism '{0}' not found in database."));
                }

                // Parse From
                fromNodeId = PathQuerierUtilities.FetchFrom(node, pathwayRestrictionsIds, false);

                // Parse To
                List<QNode> toQNodes = node.GetChildren("path_to_process", "path_to_molecule");
                foreach (QNode q in toQNodes)
                {
                    toNodes.Add(PathQuerierUtilities.FetchTo(q, pathwayRestrictionsIds, false));
                }

                if (toNodes.Count <= 0)
                    throw new PathQuerierException(@"Please specify one or more ""to"" entities.");


                // Parse Length
                PathQuerierUtilities.FetchLengths(node, out minLength, out maxLength);

                // not including
                excludingRestrictions = PathQuerierUtilities.FetchNotIncluding(node);

                // including
                includingRestrictions = PathQuerierUtilities.FetchIncluding(node, pathwayRestrictionsIds);

                //// including molecule
                //List<string> includingMolNames = node.SelectInputValues("path_restriction_including_molecule@molecule_name:molecule_name");
                //List<string> includingMolPathways = node.SelectInputValues("path_restriction_including_molecule@pathway_name:pathway_name");
                //foreach (string molName in includingMolNames)
                //{
                //    if (string.IsNullOrEmpty(molName))
                //        throw new PathQuerierException("Please either specify a value for the including molecule graph restriction or remove that empty node from the query.");

                //    Guid molPwId = Guid.Empty;
                //    if (includingMolPathways.Count != includingMolNames.Count)
                //    {
                //        if (pathwayRestrictionsIds.Count == 1)
                //            molPwId = pathwayRestrictionsIds[0];
                //        else
                //            throw new PathQuerierException(@"Please specify the pathway context for molecule '{0}' or give a single pathways restriction.", fromMolNames[0]);
                //    }
                //    else
                //        molPwId = PathQuerierUtilities.FetchIdsFromName(includingMolPathways[0], PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database.")[0];

                //    includingRestrictions.AddRange(PathQuerierUtilities.FetchIdsFromName(molName, PathQuerierUtilities.EntityType.Molecule, molPwId, "Molecule '{0}' not found in database."));
                //}

                //// include process
                //List<string>includingProcNames = node.SelectInputValues("path_restriction_including_process@process_name:process_name");
                //List<string> includingProcPathways = node.SelectInputValues("path_restriction_including_process@pathway_name:pathway_name");
                //foreach (string procName in includingProcNames)
                //{
                //    if (string.IsNullOrEmpty(procName))
                //        throw new PathQuerierException("Please either specify a value for the including process graph restriction or remove that empty node from the query.");

                //    Guid procPwId = Guid.Empty;
                //    if (includingProcPathways.Count != includingProcNames.Count)
                //    {
                //        if (pathwayRestrictionsIds.Count == 1)
                //            procPwId = pathwayRestrictionsIds[0];
                //        else
                //            throw new PathQuerierException(@"Please specify the pathway context for process '{0}' or give a single pathways restriction.", fromProcNames[0]);
                //    }
                //    else
                //        procPwId = PathQuerierUtilities.FetchIdsFromName(includingProcPathways[0], PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database.")[0];

                //    includingRestrictions.AddRange(PathQuerierUtilities.FetchIdsFromName(procName, PathQuerierUtilities.EntityType.Process, procPwId, "Process '{0}' not found in database."));
                //}

                // Grab the configuration settings for the max result limit, max graph limit, and timeout limit.
                maxResultLimit = PathQuerierUtilities.GetIntConfigValue("PathQueryMaxResultLimit", int.MaxValue);
                maxGraphLimit = PathQuerierUtilities.GetIntConfigValue("PathQueryMaxGraphLimit", int.MaxValue);
                timeoutLimit = PathQuerierUtilities.GetIntConfigValue("PathQueryTimeoutLimit", int.MaxValue);

                #region Old version

                //int childPtr = 0;

                //// Parse common molecule checkbox
                //useCommonMolecules = PathQuerierUtilities.FetchUseCommonMolecules(node);

                //// Parse search direction dropdown
                //searchDirection = PathQuerierUtilities.FetchSearchDirection(node);

                //// Parse restrictions and graph
                //Dictionary<string, List<Guid>> restrictionIds;
                //Dictionary<string, List<string>> restrictionNames;
                //PathQuerierUtilities.FetchNodes(node,
                //                                new string[] { "path_graph_restriction_pathway", "path_graph_restriction_organism" },
                //                                new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Pathway, PathQuerierUtilities.EntityType.Organism },
                //                                new string[] { "Please either specify a value for the pathway graph restriction or remove that empty node from the query.", "Please either specify a value for the organism graph restriction or remove that empty node from the query." },
                //                                ref childPtr, out restrictionIds, out restrictionNames);
                //pathwayRestrictionsIds = restrictionIds["path_graph_restriction_pathway"];
                //pathwayRestrictionsNames = restrictionNames["path_graph_restriction_pathway"];
                //organismRestrictionsIds = restrictionIds["path_graph_restriction_organism"];
                //organismRestrictionsNames = restrictionNames["path_graph_restriction_organism"];

                //// Parse From
                //Dictionary<string, List<INode>> fromNodes =
                //    PathQuerierUtilities.FetchMultipleNodesINodes(node,
                //                                                  new string[] { "path_from_molecule", "path_from_process" },
                //                                                  new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Molecule, PathQuerierUtilities.EntityType.Process },
                //                                                  new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                  new string[] { @"Please specify a value for the ""from molecule"" entity.", @"Please specify a value for the ""from process"" entity." },
                //                                                  ref childPtr);
                //List<INode> fromMoleculeNodes = fromNodes["path_from_molecule"];
                //List<INode> fromProcessNodes = fromNodes["path_from_process"];

                //if(fromMoleculeNodes.Count == 1 && fromProcessNodes.Count == 0)
                //    fromNode = fromMoleculeNodes[0];
                //else if(fromMoleculeNodes.Count == 0 && fromProcessNodes.Count == 1)
                //    fromNode = fromProcessNodes[0];
                //else
                //    throw new PathQuerierException(@"Please specify a single ""from"" entity.");

                //// Parse To
                //toNodes = PathQuerierUtilities.FetchMultipleNodesToNodes(node,

                //                                                         new string[] { "path_to_molecule", "path_to_process" },
                //                                                         new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Molecule, PathQuerierUtilities.EntityType.Process },
                //                                                         new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                         new string[] { @"Please specify a value for the ""to molecule"" entity.", @"Please specify a value for the ""to process"" entity." },

                //                                                         new string[] { "path_restriction_including_molecule", "path_restriction_including_process" },
                //                                                         new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Molecule, PathQuerierUtilities.EntityType.Process },
                //                                                         new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                         new string[] { "Please either specify a value for the including molecule graph restriction or remove that empty node from the segment ending at <i>{0}</i>.", "Please either specify a value for the including process graph restriction or remove that empty node from the segment ending at <i>{0}</i>." },

                //                                                         new string[] { "path_restriction_not_including_molecule", "path_restriction_not_including_process" },
                //                                                         new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Molecule, PathQuerierUtilities.EntityType.Process },
                //                                                         new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                         new string[] { "Please either specify a value for the not including molecule graph restriction or remove that empty node from the segment ending at <i>{0}</i>.", "Please either specify a value for the not including process graph restriction or remove that empty node from the segment ending at <i>{0}</i>." },

                //                                                         ref childPtr);
                //if(toNodes.Count <= 0)
                //    throw new PathQuerierException(@"Please specify one or more ""to"" entities.");

                //// Parse Length
                //PathQuerierUtilities.FetchLengths(node, out minLength, out maxLength);

                //// Parse Incl/NotIncl
                //Dictionary<string, List<INode>> includingRestrictionsDict =
                //    PathQuerierUtilities.FetchMultipleNodesINodes(node,
                //                                                  new string[] { "path_restriction_including_molecule", "path_restriction_including_process" },
                //                                                  new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Molecule, PathQuerierUtilities.EntityType.Process },
                //                                                  new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                  new string[] { "Please either specify a value for the including molecule graph restriction or remove that empty node from the query.", "Please either specify a value for the including process graph restriction or remove that empty node from the query." },
                //                                                  ref childPtr);
                //includingRestrictions = new List<INode>();
                //includingRestrictions.AddRange(includingRestrictionsDict["path_restriction_including_molecule"]);
                //includingRestrictions.AddRange(includingRestrictionsDict["path_restriction_including_process"]);

                //Dictionary<string, List<INode>> excludingRestrictionsDict =
                //    PathQuerierUtilities.FetchMultipleNodesINodes(node,
                //                                                  new string[] { "path_restriction_not_including_molecule", "path_restriction_not_including_process" },
                //                                                  new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Molecule, PathQuerierUtilities.EntityType.Process },
                //                                                  new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                  new string[] { "Please either specify a value for the not including molecule graph restriction or remove that empty node from the query.", "Please either specify a value for the not including process graph restriction or remove that empty node from the query." },
                //                                                  ref childPtr);
                //excludingRestrictions = new List<INode>();
                //excludingRestrictions.AddRange(excludingRestrictionsDict["path_restriction_not_including_molecule"]);
                //excludingRestrictions.AddRange(excludingRestrictionsDict["path_restriction_not_including_process"]);

                //// Grab the configuration settings for the max result limit, max graph limit, and timeout limit.
                //maxResultLimit = PathQuerierUtilities.GetIntConfigValue("PathQueryMaxResultLimit", int.MaxValue);
                //maxGraphLimit = PathQuerierUtilities.GetIntConfigValue("PathQueryMaxGraphLimit", int.MaxValue);
                //timeoutLimit = PathQuerierUtilities.GetIntConfigValue("PathQueryTimeoutLimit", int.MaxValue);

                #endregion
            }
            catch (PathQuerierException e)
            {
                return new PathQueryResults(e.Message);
            }

            /* ----- Execute Query ----- */

            // Load and register the requested graph (if necessary)
            GraphLoader loader = GraphLoader.Instance;
            string graphName;
            loader.LoadMetabolicNetworkGraph(manager, false, !useCommonMolecules, pathwayRestrictionsIds, organismRestrictionsIds, out graphName);

            // Setup query parameters
            IQueryParameters parameters = new QueryPathParameters(fromNodeId, toNodes, minLength, maxLength, includingRestrictions, excludingRestrictions, maxResultLimit, maxGraphLimit, timeoutLimit);

            // Register the query on the server (if necessary)
            if (!manager.ContainsQuery("path_simple"))
                manager.RegisterQuery("path_simple", new QueryPathSimple());

            // Execute the query
            PathQueryLib.IQueryResults qResults = manager.Execute("path_simple", graphName, searchDirection, parameters);

            /* ----- Return the Results ----- */

            return new PathQueryResults(qResults);
        }
    }
}