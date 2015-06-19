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
    /// A querier for the neighborhood query on the metabolic network graph
    /// </summary>
    public class PathNeighborhoodMetabolicNWQuerier : PathQuerierMetabolicNW
    {
        private PathNeighborhoodMetabolicNWQuerier()
            : base()
        { }

        public PathNeighborhoodMetabolicNWQuerier(IAQIUtil util)
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
            NodeType findType;
            //NodeType fromType;
            Guid fromNodeId;
            int minLength;
            int maxLength;
            List<Guid> includingRestrictions = new List<Guid>();
            List<Guid> notIncludingRestrictions = new List<Guid>();
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

                fromNodeId = PathQuerierUtilities.FetchFrom(node, pathwayRestrictionsIds, false);

                // Parse Length
                PathQuerierUtilities.FetchLengths(node, out minLength, out maxLength);

                // Parse query find type
                findType = PathQuerierUtilities.FetchFindType(node);

                // not including
                notIncludingRestrictions = PathQuerierUtilities.FetchNotIncluding(node);

                //// not including molecule
                //List<string> notIncludingMolNames = node.SelectInputValues("path_restriction_not_including_molecule@molecule_name:molecule_name");
                //foreach (string molName in notIncludingMolNames)
                //{
                //    if (string.IsNullOrEmpty(molName))
                //        throw new PathQuerierException("Please either specify a value for the not including molecule graph restriction or remove that empty node from the query.");

                //    notIncludingRestrictions.AddRange(PathQuerierUtilities.FetchIdsFromName(molName, PathQuerierUtilities.EntityType.Molecule, Guid.Empty, "Molecule '{0}' not found in database."));
                //}

                //// not including process
                //List<string> notIncludingProcNames = node.SelectInputValues("path_restriction_not_including_process@process_name:process_name");
                //foreach (string procName in notIncludingProcNames)
                //{
                //    if (string.IsNullOrEmpty(procName))
                //        throw new PathQuerierException("Please either specify a value for the not including process graph restriction or remove that empty node from the query.");

                //    notIncludingRestrictions.AddRange(PathQuerierUtilities.FetchIdsFromName(procName, PathQuerierUtilities.EntityType.Process, Guid.Empty, "Process '{0}' not found in database."));
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

                //// Parse Length
                //PathQuerierUtilities.FetchLengths(node, out minLength, out maxLength);

                //// Parse query find type
                //findType = PathQuerierUtilities.FetchFindType(node);

                //// Parse Incl/NotIncl
                ///*Dictionary<string, List<INode>> includingRestrictionsDict =
                //    PathQuerierUtilities.FetchMultipleNodesINodes(node,
                //                                                  new string[] { "path_restriction_including_molecule", "path_restriction_including_process" },
                //                                                  new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Molecule, PathQuerierUtilities.EntityType.Process },
                //                                                  new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                  new string[] { "Please either specify a value for the including molecule graph restriction or remove that empty node from the query.", "Please either specify a value for the including process graph restriction or remove that empty node from the query." },
                //                                                  ref childPtr);*/
                //includingRestrictions = new List<INode>();
                ////includingRestrictions.AddRange(includingRestrictionsDict["path_restriction_including_molecule"]);
                ////includingRestrictions.AddRange(includingRestrictionsDict["path_restriction_including_process"]);

                //Dictionary<string, List<INode>> notIncludingRestrictionsDict =
                //    PathQuerierUtilities.FetchMultipleNodesINodes(node,
                //                                                  new string[] { "path_restriction_not_including_molecule", "path_restriction_not_including_process" },
                //                                                  new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Molecule, PathQuerierUtilities.EntityType.Process },
                //                                                  new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                  new string[] { "Please either specify a value for the not including molecule graph restriction or remove that empty node from the query.", "Please either specify a value for the not including process graph restriction or remove that empty node from the query." },
                //                                                  ref childPtr);
                //notIncludingRestrictions = new List<INode>();
                //notIncludingRestrictions.AddRange(notIncludingRestrictionsDict["path_restriction_not_including_molecule"]);
                //notIncludingRestrictions.AddRange(notIncludingRestrictionsDict["path_restriction_not_including_process"]);

                //// Grab the configuration settings for the max result limit, max graph limit, and timeout limit.
                //maxResultLimit = PathQuerierUtilities.GetIntConfigValue("PathQueryMaxResultLimit", int.MaxValue);
                //maxGraphLimit = PathQuerierUtilities.GetIntConfigValue("PathQueryMaxGraphLimit", int.MaxValue);
                //timeoutLimit = PathQuerierUtilities.GetIntConfigValue("PathQueryTimeoutLimit", int.MaxValue);

                #endregion
            }
            catch(PathQuerierException e)
            {
                return new PathQueryResults(e.Message);
            }

            /* ----- Execute Query ----- */

            // Load and register the requested graph (if necessary)
            GraphLoader loader = GraphLoader.Instance;
            string graphName;
            loader.LoadMetabolicNetworkGraph(manager, false, !useCommonMolecules, pathwayRestrictionsIds,  organismRestrictionsIds,  out graphName);

            // Setup query parameters
            IQueryParameters parameters = new QueryNeighborhoodParameters(minLength, maxLength, fromNodeId, includingRestrictions, notIncludingRestrictions, findType, maxResultLimit, maxGraphLimit, timeoutLimit);

            // Register the query on the server (if necessary)
            if(!manager.ContainsQuery("neighborhood_simple"))
                manager.RegisterQuery("neighborhood_simple", new QueryNeighborhoodSimple());

            // Execute the query
            PathQueryLib.IQueryResults qResults = manager.Execute("neighborhood_simple", graphName, searchDirection, parameters);
            if (qResults is QueryFailureResult)
                return new PathQueryResults(((QueryFailureResult)qResults).Message);

            /* ----- Return the Results ----- */

            return new PathQueryResults(qResults);
        }


    }
}