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
    /// A querier for the path query on the pathway links graph
    /// </summary>
    public class PathPWLinksQuerier : PathQuerierPWLinks
    {
        private PathPWLinksQuerier()
            : base()
        { }

        public PathPWLinksQuerier(IAQIUtil util)
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
            List<QueryPathParametersToNode> toNodes = new List<QueryPathParametersToNode>();
            int minLength;
            int maxLength;
            List<Guid> includingRestrictions;
            List<Guid> excludingRestrictions;
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

                // Parse From
                fromNodeId = PathQuerierUtilities.FetchFrom(node, null, true);

                // Parse To
                List<QNode> toQNodes = node.GetChildren("path_to_pathway", "path_to_molecule");
                foreach (QNode q in toQNodes)
                {
                    toNodes.Add(PathQuerierUtilities.FetchTo(q, null, true));
                }

                if (toNodes.Count <= 0)
                    throw new PathQuerierException(@"Please specify one or more ""to"" entities.");

                // Parse Length
                PathQuerierUtilities.FetchLengths(node, out minLength, out maxLength);

                // not including
                excludingRestrictions = PathQuerierUtilities.FetchNotIncluding(node);

                // including
                includingRestrictions = PathQuerierUtilities.FetchIncluding(node, null);

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

                //// Parse To
                //toNodes = PathQuerierUtilities.FetchMultipleNodesToNodes(node,

                //                                                         new string[] { "path_to_pathway", "path_to_molecule" },
                //                                                         new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Pathway, PathQuerierUtilities.EntityType.Molecule },
                //                                                         new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                         new string[] { @"Please specify a value for the ""to pathway"" entity.", @"Please specify a value for the ""to molecule"" entity." },

                //                                                         new string[] { "path_restriction_including_pathway", "path_restriction_including_molecule" },
                //                                                         new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Pathway, PathQuerierUtilities.EntityType.Molecule },
                //                                                         new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                         new string[] { "Please either specify a value for the including pathway graph restriction or remove that empty node from the segment ending at <i>{0}</i>.", "Please either specify a value for the including molecule graph restriction or remove that empty node from the segment ending at <i>{0}</i>." },

                //                                                         new string[] { "path_restriction_not_including_pathway", "path_restriction_not_including_molecule" },
                //                                                         new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Pathway, PathQuerierUtilities.EntityType.Molecule },
                //                                                         new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                         new string[] { "Please either specify a value for the not including pathway graph restriction or remove that empty node from the segment ending at <i>{0}</i>.", "Please either specify a value for the not including molecule graph restriction or remove that empty node from the segment ending at <i>{0}</i>." },

                //                                                         ref childPtr);
                //if(toNodes.Count <= 0)
                //    throw new PathQuerierException(@"Please specify one or more ""to"" entities.");

                //// Parse Length
                //PathQuerierUtilities.FetchLengths(node, out minLength, out maxLength);

                //// Parse Incl/NotIncl
                //Dictionary<string, List<INode>> includingRestrictionsDict =
                //    PathQuerierUtilities.FetchMultipleNodesINodes(node,
                //                                                  new string[] { "path_restriction_including_pathway", "path_restriction_including_molecule" },
                //                                                  new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Pathway, PathQuerierUtilities.EntityType.Molecule },
                //                                                  new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                  new string[] { "Please either specify a value for the including pathway graph restriction or remove that empty node from the query.", "Please either specify a value for the including molecule graph restriction or remove that empty node from the query." },
                //                                                  ref childPtr);
                //includingRestrictions = new List<INode>();
                //includingRestrictions.AddRange(includingRestrictionsDict["path_restriction_including_pathway"]);
                //includingRestrictions.AddRange(includingRestrictionsDict["path_restriction_including_molecule"]);

                //Dictionary<string, List<INode>> excludingRestrictionsDict =
                //    PathQuerierUtilities.FetchMultipleNodesINodes(node,
                //                                                  new string[] { "path_restriction_not_including_pathway", "path_restriction_not_including_molecule" },
                //                                                  new PathQuerierUtilities.EntityType[] { PathQuerierUtilities.EntityType.Pathway, PathQuerierUtilities.EntityType.Molecule },
                //                                                  new NodeType[] { NodeType.Node, NodeType.Edge },
                //                                                  new string[] { "Please either specify a value for the not including pathway graph restriction or remove that empty node from the query.", "Please either specify a value for the not including molecule graph restriction or remove that empty node from the query." },
                //                                                  ref childPtr);
                //excludingRestrictions = new List<INode>();
                //excludingRestrictions.AddRange(excludingRestrictionsDict["path_restriction_not_including_pathway"]);
                //excludingRestrictions.AddRange(excludingRestrictionsDict["path_restriction_not_including_molecule"]);

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