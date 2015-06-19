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
    /// A querier for the neighborhood query on the pathway links graph
    /// </summary>
    public class PathNeighborhoodPWLinksQuerier : PathQuerierPWLinks
    {
        private PathNeighborhoodPWLinksQuerier()
        { }

        public PathNeighborhoodPWLinksQuerier(IAQIUtil util)
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
            List<Guid> organismRestrictionsIds = new List<Guid>();
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
                //BE: not including common molecules for pathway links graph
                // Parse common molecule checkbox
                useCommonMolecules = false; //= PathQuerierUtilities.FetchUseCommonMolecules(node);

                // Parse search direction dropdown
                searchDirection = PathQuerierUtilities.FetchSearchDirection(node);

                // organism restrictions
                List<string> orgResNames = node.SelectInputValues("path_graph_restriction_organism@organism_name:organism_name");
                foreach (string orgName in orgResNames)
                {
                    if (string.IsNullOrEmpty(orgName))
                        throw new PathQuerierException("Please either specify a value for the organism graph restriction or remove that empty node from the query.");
                    organismRestrictionsIds.AddRange(PathQuerierUtilities.FetchIdsFromName(orgName, PathQuerierUtilities.EntityType.Organism, Guid.Empty, "Organism '{0}' not found in database."));
                }

                // from molecule
                fromNodeId = PathQuerierUtilities.FetchFrom(node, new List<Guid>(), true);
                //List<string> fromMolNames = node.SelectInputValues("path_from_molecule@molecule_name:molecule_name");
                //List<string> fromMolPathways = node.SelectInputValues("path_from_molecule@pathway_name:pathway_name");

                //List<string> fromPathwayNames = node.SelectInputValues("path_from_pathway@pathway_name:pathway_name");

                //if (fromMolNames.Count == 1 && fromPathwayNames.Count == 0)
                //{
                //    // starting from a molecule
                //    //fromType = NodeType.Node;
                //    Guid molPwId = Guid.Empty;
                //    if (fromMolPathways.Count != fromMolNames.Count)
                //    {
                //        throw new PathQuerierException(@"Please specify the pathway context for molecule '{0}'.", fromMolNames[0]);
                //    }
                //    else
                //        molPwId = PathQuerierUtilities.FetchIdsFromName(fromMolPathways[0], PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database.")[0];

                //    fromNodeId = PathQuerierUtilities.FetchIdsFromName(fromMolNames[0], PathQuerierUtilities.EntityType.Molecule, molPwId, "Molecule '{0}' not found in database.")[0];
                //}
                //else if (fromMolNames.Count == 0 && fromPathwayNames.Count == 1)
                //{
                //    // starting from a pathway
                //    fromNodeId = PathQuerierUtilities.FetchIdsFromName(fromPathwayNames[0], PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database.")[0];
                //}
                //else
                //    throw new PathQuerierException(@"Please specify a single ""from"" entity.");

                // Parse Length
                PathQuerierUtilities.FetchLengths(node, out minLength, out maxLength);

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

                //// not including pathway
                //List<string> notIncludingPathwayNames = node.SelectInputValues("path_restriction_not_including_pathway@pathway_name:pathway_name");
                //foreach (string pathwayName in notIncludingPathwayNames)
                //{
                //    if (string.IsNullOrEmpty(pathwayName))
                //        throw new PathQuerierException("Please either specify a value for a pathway restriction or remove that empty node from the query.");

                //    notIncludingRestrictions.AddRange(PathQuerierUtilities.FetchIdsFromName(pathwayName, PathQuerierUtilities.EntityType.Pathway, Guid.Empty, "Pathway '{0}' not found in database."));
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
                //                                new string[] { "path_graph_restriction_organism" },
                //                                new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Organism },
                //                                new string[] { "Please either specify a value for the organism graph restriction or remove that empty node from the query." },
                //                                ref childPtr, out restrictionIds, out restrictionNames);
                //organismRestrictionsIds = restrictionIds["path_graph_restriction_organism"];
                //organismRestrictionsNames = restrictionNames["path_graph_restriction_organism"];

                //// Parse From
                //Dictionary<string, List<INode>> fromNodes =
                //    PathQuerierUtilities.FetchMultipleNodesINodes(node,
                //                                                  new string[] { "path_from_pathway", "path_from_molecule" },
                //                                                  new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Pathway, PathQuerierUtilities.EntityType.Molecule },
                //                                                  new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                  new string[] { @"Please specify a value for the ""from pathway"" entity.", @"Please specify a value for the ""from molecule"" entity." },
                //                                                  ref childPtr);
                //List<INode> fromPathwayNodes = fromNodes["path_from_pathway"];
                //List<INode> fromMoleculeNodes = fromNodes["path_from_molecule"];

                //if(fromPathwayNodes.Count == 1 && fromMoleculeNodes.Count == 0)
                //    fromNode = fromPathwayNodes[0];
                //else if(fromPathwayNodes.Count == 0 && fromMoleculeNodes.Count == 1)
                //    fromNode = fromMoleculeNodes[0];
                //else
                //    throw new PathQuerierException(@"Please specify a single ""from"" entity.");

                //// Parse Length
                //PathQuerierUtilities.FetchLengths(node, out minLength, out maxLength);

                //// Parse Incl/NotIncl
                ///*Dictionary<string, List<INode>> includingRestrictionsDict =
                //    PathQuerierUtilities.FetchMultipleNodesINodes(node,
                //                                                  new string[] { "path_restriction_including_pathway", "path_restriction_including_molecule" },
                //                                                  new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Pathway, PathQuerierUtilities.EntityType.Molecule },
                //                                                  new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                  new string[] { "Please either specify a value for the including pathway graph restriction or remove that empty node from the query.", "Please either specify a value for the including molecule graph restriction or remove that empty node from the query." },
                //                                                  ref childPtr);*/
                //includingRestrictions = new List<INode>();
                ////includingRestrictions.AddRange(includingRestrictionsDict["path_restriction_including_pathway"]);
                ////includingRestrictions.AddRange(includingRestrictionsDict["path_restriction_including_molecule"]);

                //Dictionary<string, List<INode>> notIncludingRestrictionsDict =
                //    PathQuerierUtilities.FetchMultipleNodesINodes(node,
                //                                                  new string[] { "path_restriction_not_including_pathway", "path_restriction_not_including_molecule" },
                //                                                  new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Pathway, PathQuerierUtilities.EntityType.Molecule },
                //                                                  new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                  new string[] { "Please either specify a value for the not including pathway graph restriction or remove that empty node from the query.", "Please either specify a value for the not including molecule graph restriction or remove that empty node from the query." },
                //                                                  ref childPtr);
                //notIncludingRestrictions = new List<INode>();
                //notIncludingRestrictions.AddRange(notIncludingRestrictionsDict["path_restriction_not_including_pathway"]);
                //notIncludingRestrictions.AddRange(notIncludingRestrictionsDict["path_restriction_not_including_molecule"]);

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
            loader.LoadPathwayLinksGraph(manager, false, !useCommonMolecules, organismRestrictionsIds, out graphName);

            // Setup query parameters and verify satisfiability
            IQueryParameters parameters = new QueryNeighborhoodParameters(minLength, maxLength, fromNodeId, includingRestrictions, notIncludingRestrictions, NodeType.Any, maxResultLimit, maxGraphLimit, timeoutLimit);

            // Register the query on the server (if necessary)
            if (!manager.ContainsQuery("neighborhood_simple"))
                manager.RegisterQuery("neighborhood_simple", new QueryNeighborhoodSimple());

            // Execute the query
            PathQueryLib.IQueryResults qResults = manager.Execute("neighborhood_simple", graphName, searchDirection, parameters);

            /* ----- Return the Results ----- */

            return new PathQueryResults(qResults);
        }
    }
}