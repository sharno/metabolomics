using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace PathwaysLib.ServerObjects
{
    /// <summary>
    /// Static class for mapping molecule & process ID's to unique ID's on the entire metabolic network.
    /// </summary>
    public class GraphNodeManager
    {
        static bool loaded = false;

        private static void Load()
        {
            lock (pwEntityToEntityNode)
            {
                if (!loaded)
                {
                    LoadEntities();
                    LoadProcesses();

                    loaded = true;
                }
            }
        }

        #region Molecular Entity Graph Nodes
        static Dictionary<Guid, Dictionary<Guid, Dictionary<int, Guid>>> pwEntityTissToEntityNode = new Dictionary<Guid, Dictionary<Guid, Dictionary<int, Guid>>>();
        static Dictionary<Guid, Dictionary<Guid, Guid>> pwEntityToEntityNode = new Dictionary<Guid,Dictionary<Guid,Guid>>();
        static Dictionary<Guid, Guid> entityNodeToPw = new Dictionary<Guid,Guid>();
        static Dictionary<Guid,Guid> entityNodeToEntity = new Dictionary<Guid,Guid>();
        static Dictionary<Guid, List<Guid>> entityPathways = new Dictionary<Guid, List<Guid>>();

       public static Guid GetEntityGraphNodeId(Guid pathwayId, Guid entityId)
        {
            if (!loaded) Load();

            if (!pwEntityToEntityNode.ContainsKey(pathwayId) || !pwEntityToEntityNode[pathwayId].ContainsKey(entityId))
                return entityId; // not found in table, use own id
            return pwEntityToEntityNode[pathwayId][entityId];
        }

        public static Guid GetAnyEntityGraphNodeId(Guid entityId)
        {
            if (!loaded) Load();

            List<Guid> pathways = GetEntityPathways(entityId);
            if (pathways.Count < 1)
                return entityId;  // not found in table, use own id
            return GetEntityGraphNodeId(pathways[0], entityId);
        }

        public static Guid GetEntityGraphNodeId(Guid pathwayId, Guid entityId, int tissueId)
        {
            if (!loaded) Load();

            if (pwEntityTissToEntityNode.ContainsKey(pathwayId))
                if (pwEntityTissToEntityNode[pathwayId].ContainsKey(entityId))
                    if (pwEntityTissToEntityNode[pathwayId][entityId].ContainsKey(tissueId))
                        return pwEntityTissToEntityNode[pathwayId][entityId][tissueId];
            return entityId; // not found in table, use own id
        }

        public static void GetPathwayAndEntityId(Guid graphNodeId, out Guid pathwaysId, out Guid entityId)
        {
            if (!loaded) Load();

            if (!entityNodeToPw.ContainsKey(graphNodeId))
            {
                pathwaysId = Guid.Empty;
                entityId = graphNodeId; // not found in table, use own id
                return;
            }

            pathwaysId = entityNodeToPw[graphNodeId];
            entityId = entityNodeToEntity[graphNodeId];
        }

        public static Guid GetEntityId(Guid graphNodeId)
        {
            if (!loaded) Load();

            if (!entityNodeToEntity.ContainsKey(graphNodeId))
            {
                return graphNodeId; // not found in table, use own id
            }
            return entityNodeToEntity[graphNodeId];
        }

        public static Guid GetEntityPathwayId(Guid graphNodeId)
        {
            if (!loaded) Load();

            if (!entityNodeToPw.ContainsKey(graphNodeId))
            {
                return graphNodeId; // not found in table, use own id
            }
            return entityNodeToPw[graphNodeId];
        }

        public static List<Guid> GetEntityPathways(Guid entityId)
        {
            if (!loaded) Load();

            if (!entityPathways.ContainsKey(entityId))
                return new List<Guid>();
            return entityPathways[entityId];
        }

        private static void LoadEntities()
        {
            DataSet results;
            DBWrapper.Instance.ExecuteQuery(out results,
                "SELECT pathwayId, entityId, graphNodeId FROM entity_graph_nodes");

            foreach (DataRow r in results.Tables[0].Rows)
            {
                Guid pwId = (r["pathwayId"] is DBNull) ? Guid.Empty : (Guid)r["pathwayId"];
                Guid entityId = (Guid)r["entityId"];
                Guid graphNodeId = (Guid)r["graphNodeId"];

                if (!pwEntityToEntityNode.ContainsKey(pwId))
                    pwEntityToEntityNode.Add(pwId, new Dictionary<Guid, Guid>());
                pwEntityToEntityNode[pwId][entityId] = graphNodeId;
                entityNodeToPw[graphNodeId] = pwId;
                entityNodeToEntity[graphNodeId] = entityId;

                if (!entityPathways.ContainsKey(entityId))
                    entityPathways[entityId] = new List<Guid>();
                if (pwId != Guid.Empty)
                    entityPathways[entityId].Add(pwId);
            }
        }

        public static List<Guid> GetEntitiesByPW(Guid pwid)
        {
            DataSet results;
            List<Guid> returnList = new List<Guid>();
            DBWrapper.Instance.ExecuteQuery(out results,
                "SELECT entityId FROM entity_graph_nodes where pathwayId='"+pwid+"'");

            foreach (DataRow r in results.Tables[0].Rows)
            {
                //Guid pwId = (r["pathwayId"] is DBNull) ? Guid.Empty : (Guid)r["pathwayId"];
                Guid entityId = (Guid)r["entityId"];
                //Guid graphNodeId = (Guid)r["graphNodeId"];
                returnList.Add(entityId);
            }
            return returnList;
        }


        #endregion

        #region Process Graph Nodes

        static Dictionary<Guid, Dictionary<Guid, Guid>> pwProcToProcNode = new Dictionary<Guid, Dictionary<Guid, Guid>>();
        static Dictionary<Guid, Guid> procNodeToPw = new Dictionary<Guid, Guid>();
        static Dictionary<Guid, Guid> procNodeToProc = new Dictionary<Guid, Guid>();
        static Dictionary<Guid, List<Guid>> procPathways = new Dictionary<Guid, List<Guid>>();
        static Dictionary<Guid, Dictionary<Guid, Dictionary<int, Guid>>> pwProcTissToProcNode = new Dictionary<Guid, Dictionary<Guid, Dictionary<int, Guid>>>();

        public static Guid GetProcessGraphNodeId(Guid pathwayId, Guid genericProcessId)
        {
            if (!loaded) Load();

            if (!pwProcToProcNode.ContainsKey(pathwayId) || !pwProcToProcNode[pathwayId].ContainsKey(genericProcessId))
                return genericProcessId; // not found in table, use own id
            return pwProcToProcNode[pathwayId][genericProcessId];
        }

        public static Guid GetProcessGraphNodeId(Guid pathwayId, Guid genericProcessId, int tissueId)
        {
            if (!loaded) Load();

            if (pwProcTissToProcNode.ContainsKey(pathwayId))
                if (pwProcTissToProcNode[pathwayId].ContainsKey(genericProcessId))
                    if (pwProcTissToProcNode[pathwayId][genericProcessId].ContainsKey(tissueId))
                        return pwProcTissToProcNode[pathwayId][genericProcessId][tissueId];

            return genericProcessId;// not found in table, use own id
        }


        public static Guid GetAnyProcessGraphNodeId(Guid genericProcessId)
        {
            if (!loaded) Load();

            List<Guid> pathways = GetProcessPathways(genericProcessId);
            if (pathways.Count < 1)
                return genericProcessId; // not found in table, use own id
            return GetProcessGraphNodeId(pathways[0], genericProcessId);
        }

        public static void GetPathwayAndGenericProcessId(Guid graphNodeId, out Guid pathwaysId, out Guid genericProcessId)
        {
            if (!loaded) Load();

            if (!procNodeToPw.ContainsKey(graphNodeId))
            {
                pathwaysId = Guid.Empty;
                genericProcessId = graphNodeId; // not found in table, use own id
                return;
            }

            pathwaysId = procNodeToPw[graphNodeId];
            genericProcessId = procNodeToProc[graphNodeId];
        }

        public static Guid GetGenericProcessId(Guid graphNodeId)
        {
            if (!loaded) Load();

            if (!procNodeToProc.ContainsKey(graphNodeId))
            {
                return graphNodeId; // not found in table, use own id
            }
            return procNodeToProc[graphNodeId];
        }

        public static Guid GetGenericProcessPathwayId(Guid graphNodeId)
        {
            if (!loaded) Load();

            if (!procNodeToPw.ContainsKey(graphNodeId))
            {
                return graphNodeId; // not found in table, use own id
            }
            return procNodeToPw[graphNodeId];
        }

        public static List<Guid> GetProcessPathways(Guid genericProcessId)
        {
            if (!loaded) Load();

            if (!procPathways.ContainsKey(genericProcessId))
                return new List<Guid>();
            return procPathways[genericProcessId];
        }

        private static void LoadProcesses()
        {
            DataSet results;
            DBWrapper.Instance.ExecuteQuery(out results,
                "SELECT pathwayId, genericProcessId, graphNodeId FROM process_graph_nodes");

            foreach (DataRow r in results.Tables[0].Rows)
            {
                Guid pwId = (r["pathwayId"] is DBNull) ? Guid.Empty : (Guid)r["pathwayId"];
                Guid procId = (Guid)r["genericProcessId"];
                Guid graphNodeId = (Guid)r["graphNodeId"];

                if (!pwProcToProcNode.ContainsKey(pwId))
                    pwProcToProcNode.Add(pwId, new Dictionary<Guid, Guid>());
                pwProcToProcNode[pwId][procId] = graphNodeId;
                procNodeToPw[graphNodeId] = pwId;
                procNodeToProc[graphNodeId] = procId;

                if (!procPathways.ContainsKey(procId))
                    procPathways[procId] = new List<Guid>();
                if (pwId != Guid.Empty)
                    procPathways[procId].Add(pwId);
            }
        }  

        #endregion

        public static Guid[] ConvertEntitiesToGraphEntities(Guid pathwayId, Guid[] moleculeIds)
        {
            Guid[] results = new Guid[moleculeIds.Length];

            for (int i = 0; i < moleculeIds.Length; i++)
            {
                results[i] = GetEntityGraphNodeId(pathwayId, moleculeIds[i]);
            }
            return results;
        }

        public static Guid[] ConvertProcessesToGraphProcesses(Guid pathwayId, Guid[] genericProcessIds)
        {
            Guid[] results = new Guid[genericProcessIds.Length];

            for (int i = 0; i < genericProcessIds.Length; i++)
            {
                results[i] = GetProcessGraphNodeId(pathwayId, genericProcessIds[i]);
            }
            return results;
        }
    }
}
