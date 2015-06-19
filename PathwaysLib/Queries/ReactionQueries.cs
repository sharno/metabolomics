using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using PathwaysLib.ServerObjects;
//using PathwaysLib.SBObjects;
using PathwaysLib.Utilities;

namespace PathwaysLib.Queries
{
    /// <summary>
    /// Contains utility functions related to pathways used by the
    /// built-in queries and other services to help build up information
    /// for tabular queries.
    /// Transferred by Greg Strnad from the old pathways service.
    /// </summary>
    public class ReactionQueries
    {

        private ReactionQueries() { }

        private static DataTable NewTable()
        {
            DataTable ReactionsTable = new DataTable();
            ReactionsTable.Columns.Add("Reaction_Id", typeof(string));
            ReactionsTable.Columns.Add("Step", typeof(string));
            ReactionsTable.Columns.Add("Direction", typeof(string));

            return ReactionsTable;
        }

        private static DataTable ModelSpeciesTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Model_ID", typeof(string));
            table.Columns.Add("Species_ID", typeof(string));
            table.Columns.Add("Compartment_ID", typeof(string));

            return table;
        }

        private static DataTable ModelCompartmentTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Model_ID", typeof(string));
            table.Columns.Add("Compartment_ID", typeof(string));
            table.Columns.Add("Species_ID", typeof(string));

            return table;
        }

        public static DataSet[] FindModelsContainingAssociatedReaction(string reactionID, string mappingQualifier)
        {
            ServerMapReactionsProcessEntities[] reactionProcessEntitiesGivenReaction = null;

            ServerMapReactionsProcessEntities[] reactionProcessEntitiesGivenProcess = null;

            if (mappingQualifier != AnnotationQualifierManager.UnspecifiedQualifier)
            {
                int qualifierId = Convert.ToInt32(mappingQualifier);
                reactionProcessEntitiesGivenReaction = ServerMapReactionsProcessEntities.GetProcessGivenReaction(new Guid(reactionID), qualifierId);
            }
            else
            {
                reactionProcessEntitiesGivenReaction = ServerMapReactionsProcessEntities.GetProcessGivenReaction(new Guid(reactionID));
            }

            // use the processID to get all reactions
            if (reactionProcessEntitiesGivenReaction.Length > 0)
            {

                foreach (ServerMapReactionsProcessEntities mapEntity in reactionProcessEntitiesGivenReaction)
                {
                    if (mappingQualifier != AnnotationQualifierManager.UnspecifiedQualifier)
                    {
                        int qualifierId = Convert.ToInt32(mappingQualifier);
                        reactionProcessEntitiesGivenProcess = ServerMapReactionsProcessEntities.AllMapReactionsProcessEntities(mapEntity.ProcessId, qualifierId);
                    }
                    else
                    {
                        reactionProcessEntitiesGivenProcess = ServerMapReactionsProcessEntities.AllMapReactionsProcessEntities(mapEntity.ProcessId);
                    }
                }
            }
            else
            {
                return null;
            }

            DataTable dtReaction = new DataTable();
            dtReaction.Columns.Add("Model_Id", typeof(string));
            dtReaction.Columns.Add("Reaction_Id", typeof(string));
            dtReaction.Columns.Add("KineticLaw_Id", typeof(string));
            dtReaction.Columns.Add("Species_Id", typeof(string));
            dtReaction.Columns.Add("Role_Id", typeof(string));
            dtReaction.Columns.Add("Qualifier_Id", typeof(string));

            if (reactionProcessEntitiesGivenProcess.Length > 0)
            {
                foreach (ServerMapReactionsProcessEntities rpe in reactionProcessEntitiesGivenProcess)
                {
                    Guid reaction_id = rpe.ReactionId;
                    int qualifier_Id = rpe.QualifierId;

                    ServerReaction reaction = ServerReaction.Load(reaction_id);
                    ServerReactionSpecies[] reactionSpecies = ServerReactionSpecies.GetAllReactionsSpeciesForOneReaction(reaction_id);

                    foreach (ServerReactionSpecies rs in reactionSpecies)
                    {
                        DataRow NewRow = dtReaction.NewRow();
                        NewRow[0] = reaction.ModelId.ToString();
                        NewRow[1] = reaction.ID.ToString();
                        NewRow[2] = reaction.KineticLawId.ToString();
                        NewRow[3] = rs.SpeciesId.ToString();
                        NewRow[4] = rs.RoleId.ToString();
                        NewRow[5] = qualifier_Id.ToString();
                        dtReaction.Rows.Add(NewRow);
                    }
                }
            }

            // Convert into a DataSet[]
            ArrayList dsReactionPairs = new ArrayList();
            foreach (DataRow drTemp in dtReaction.Rows)
            {
                DataSet dsTemp = new DataSet();
                DataTable dtTemp = dtReaction.Clone();
                dsTemp.Tables.Add(dtTemp);
                dsTemp.Tables[0].ImportRow(drTemp);
                dsReactionPairs.Add(dsTemp);
            }

            return (DataSet[])dsReactionPairs.ToArray(typeof(DataSet));
        }



        public static DataSet[]  FindReactionsInGivenStepsInModel(string model_id, string reaction_id, int direction, int steps, int radiusSpec)
          {
                                            
              ArrayList dsReactionsPairs = new ArrayList();
              Dictionary<Guid, int> reactionsDictionary = new Dictionary<Guid, int>();

              ServerReaction[] AllReactions = ServerReaction.GetAllReactionsForModel(new Guid(model_id));
              int index = 0;
              foreach (ServerReaction reaction in AllReactions)
              {
                  if (!reactionsDictionary.ContainsKey(reaction.ID))
                  {
                      reactionsDictionary.Add(reaction.ID, index);
                      index++;
                  }
              }

              DataTable dtReactions = NewTable();
    
              switch (direction)
              {
                  case 1: //Downstream
                      int[][] downStreamGraph = DownStreamGraphforModel(new Guid(model_id));
                      Dictionary<Guid, int> reactionAndDistanceList = DownstreamSearchGraph(downStreamGraph, reactionsDictionary, new Guid(reaction_id), steps);
                      foreach (KeyValuePair<Guid, int> pair in reactionAndDistanceList)
                      {
                          if (radiusSpec == 1)//radius specification: exactly
                          {
                              if (pair.Value == steps)
                              {
                                  DataRow NewRow = dtReactions.NewRow();
                                  NewRow[0] = pair.Key.ToString();
                                  NewRow[1] = pair.Value.ToString();
                                  NewRow[2] = "Downstream";
                                  dtReactions.Rows.Add(NewRow);
                              }
                          }
                          else //radius specification: at most
                          {
                              DataRow NewRow = dtReactions.NewRow();
                              NewRow[0] = pair.Key.ToString();
                              NewRow[1] = pair.Value.ToString();
                              NewRow[2] = "Downstream";
                              dtReactions.Rows.Add(NewRow);
                          }
                      }

                      break;

                  #region upstream and any
                  case 2://UpStream
                      int[][] upStreamGraph = UpStreamGraphforModel(new Guid(model_id));
                      reactionAndDistanceList = UpstreamSearchGraph(upStreamGraph, reactionsDictionary, new Guid(reaction_id), steps);
                      foreach (KeyValuePair<Guid, int> pair in reactionAndDistanceList)
                      {
                          if (radiusSpec == 1)//radius specification: exactly
                          {
                              if (pair.Value == steps)
                              {
                                  DataRow NewRow = dtReactions.NewRow();
                                  NewRow[0] = pair.Key.ToString();
                                  NewRow[1] = pair.Value.ToString();
                                  NewRow[2] = "Upstream";
                                  dtReactions.Rows.Add(NewRow);
                              }
                          }
                          else //radius specification: at most
                          {
                              DataRow NewRow = dtReactions.NewRow();
                              NewRow[0] = pair.Key.ToString();
                              NewRow[1] = pair.Value.ToString();
                              NewRow[2] = "Upstream";
                              dtReactions.Rows.Add(NewRow);
                          }
                      }

                      break;

                  case 0:
                      downStreamGraph = DownStreamGraphforModel(new Guid(model_id));
                      reactionAndDistanceList = DownstreamSearchGraph(downStreamGraph, reactionsDictionary, new Guid(reaction_id), steps);
                      foreach (KeyValuePair<Guid, int> pair in reactionAndDistanceList)
                      {
                          if (radiusSpec == 1)//radius specification: exactly
                          {
                              if (pair.Value == steps)
                              {
                                  DataRow NewRow = dtReactions.NewRow();
                                  NewRow[0] = pair.Key.ToString();
                                  NewRow[1] = pair.Value.ToString();
                                  NewRow[2] = "Downstream";
                                  dtReactions.Rows.Add(NewRow);
                              }

                          }
                          else //radius specification: at most
                          {
                              DataRow NewRow = dtReactions.NewRow();
                              NewRow[0] = pair.Key.ToString();
                              NewRow[1] = pair.Value.ToString();
                              NewRow[2] = "Downstream";
                              dtReactions.Rows.Add(NewRow);
                          }

                      }

                      upStreamGraph = UpStreamGraphforModel(new Guid(model_id));
                      reactionAndDistanceList = UpstreamSearchGraph(upStreamGraph, reactionsDictionary, new Guid(reaction_id), steps);
                      foreach (KeyValuePair<Guid, int> pair in reactionAndDistanceList)
                      {
                          if (radiusSpec == 1)//radius specification: exactly
                          {
                              if (pair.Value == steps)
                              {
                                  DataRow NewRow = dtReactions.NewRow();
                                  NewRow[0] = pair.Key.ToString();
                                  NewRow[1] = pair.Value.ToString();
                                  NewRow[2] = "Upstream";
                                  dtReactions.Rows.Add(NewRow);
                              }

                          }
                          else //radius specification: at most
                          {
                              DataRow NewRow = dtReactions.NewRow();
                              NewRow[0] = pair.Key.ToString();
                              NewRow[1] = pair.Value.ToString();
                              NewRow[2] = "Upstream";
                              dtReactions.Rows.Add(NewRow);
                          }
                      }

                      break;
                  #endregion

                  default: break;    
              }
              
              // Convert into a DataSet[]
              foreach (DataRow drTemp in dtReactions.Rows)
              {
                  DataSet dsTemp = new DataSet();
                  DataTable dtTemp = dtReactions.Clone();
                  dsTemp.Tables.Add(dtTemp);
                  dsTemp.Tables[0].ImportRow(drTemp);
                  dsReactionsPairs.Add(dsTemp);
              }

              return (DataSet[])dsReactionsPairs.ToArray(typeof(DataSet));
          }     

        public static int[][] DownStreamGraphforModel(Guid model_id)
          {
              Dictionary<Guid, string> speciesDict = new Dictionary<Guid, string>();

              Dictionary<Guid, List<Guid>> reactantIdAsKeyReactionDict = new Dictionary<Guid, List<Guid>>();
              Dictionary<Guid, List<Guid>> productIdAsKeyReactionDict = new Dictionary<Guid, List<Guid>>();
              Dictionary<Guid, List<Guid>> modifierIdAsKeyReactionDict = new Dictionary<Guid, List<Guid>>();

              Dictionary<Guid, List<Guid>> reactionIdAsKeyReactantDict = new Dictionary<Guid, List<Guid>>();
              Dictionary<Guid, List<Guid>> reactionIdAsKeyProductDict = new Dictionary<Guid, List<Guid>>();
              Dictionary<Guid, List<Guid>> reactionIdAsKeyModifierDict = new Dictionary<Guid, List<Guid>>();
              
            
              Dictionary<Guid, int> reactionsDict = new Dictionary<Guid, int>();

              ServerReaction[] AllReactions = ServerReaction.GetAllReactionsForModel(model_id);
              int index = 0;
              foreach (ServerReaction reaction in AllReactions)
              {
                  if (!reactionsDict.ContainsKey(reaction.ID))
                  {
                      reactionsDict.Add(reaction.ID, index);
                      index++;
                  }
              }

              //Step 1: given a model, obtain all species and store them in dictionary
              ServerSpecies[] AllSpecies = ServerSpecies.GetAllSpeciesForModel(model_id);

              foreach (ServerSpecies species in AllSpecies)
              {
                  if (!speciesDict.ContainsKey(species.ID))
                      speciesDict.Add(species.ID, species.Name);
              }
            
              //Step 2: for each species, collect the reaction
              foreach (KeyValuePair<Guid, string> pair in speciesDict)
              {
                  Guid species_id = pair.Key ;
                  
                  ServerReactionSpecies[] AllReationSpecies = ServerReactionSpecies.GetAllReactionsSpeciesForOneSpecies(species_id );
                  
                  foreach (ServerReactionSpecies reactionspecies in AllReationSpecies)
                  {
                      Guid reaction_id = reactionspecies.ReactionId;
                       switch (reactionspecies.RoleId)
                       {
                           case 1:
                               //role as a reactant
                               if (!reactantIdAsKeyReactionDict.ContainsKey(species_id))
                                   reactantIdAsKeyReactionDict[species_id] = new List<Guid>();

                               reactantIdAsKeyReactionDict[species_id].Add(reaction_id);

                               if (!reactionIdAsKeyReactantDict.ContainsKey(reaction_id))
                                   reactionIdAsKeyReactantDict[reaction_id] = new List<Guid>();

                               reactionIdAsKeyReactantDict[reaction_id].Add(species_id);

                               break;
                           case 2:
                                //role as a producct
                               if (!productIdAsKeyReactionDict.ContainsKey(species_id))
                                   productIdAsKeyReactionDict[species_id] = new List<Guid>();

                               productIdAsKeyReactionDict[species_id].Add(reaction_id);

                               if (!reactionIdAsKeyProductDict.ContainsKey(reaction_id))
                                   reactionIdAsKeyProductDict[reaction_id] = new List<Guid>();

                               reactionIdAsKeyProductDict[reaction_id].Add(species_id);

                               break;
                           case 3:
                               //role as a modifier
                               if (!modifierIdAsKeyReactionDict.ContainsKey(species_id))
                                   modifierIdAsKeyReactionDict[species_id] = new List<Guid>();

                               modifierIdAsKeyReactionDict[species_id].Add(reaction_id);

                               if (!reactionIdAsKeyModifierDict.ContainsKey(reaction_id))
                                   reactionIdAsKeyModifierDict[reaction_id] = new List<Guid>();

                               reactionIdAsKeyModifierDict[reaction_id].Add(species_id);

                               break;
                           default: break;
                       }
                  }
              }
        
              // Step 3: construct graph

              int num_reactions = reactionsDict.Count;

              int[][] downstreamGraph = new int[num_reactions][];

              for (int i = 0; i < num_reactions; i++)
              {
                  downstreamGraph[i] = new int[num_reactions];
              }

              List<Guid> reactionsList = new List<Guid>();

              foreach (KeyValuePair<Guid, int> pair in reactionsDict)
              {
                  reactionsList.Add(pair.Key);
              }

              for (int i = 0; i < num_reactions; i++)
              {
                  Guid reaction_id = reactionsList[i];
                  
                  List<Guid> productIdList = new List<Guid>();

                  if (reactionIdAsKeyProductDict.ContainsKey(reaction_id))
                  {
                      List<Guid> tempList = reactionIdAsKeyProductDict[reaction_id];
                      if (tempList.Count > 0)
                      {
                          foreach (Guid id in tempList)
                              productIdList.Add(id);
                      }
                  }
                  List<Guid> reactionIdList = new List<Guid>();
                  List<Guid> otherReactionIdList= new List<Guid>();
               
                  if (productIdList.Count > 0)
                  {
                      foreach (Guid species_id in productIdList)
                      {
                          if (reactantIdAsKeyReactionDict.ContainsKey(species_id))
                          {
                              reactionIdList = reactantIdAsKeyReactionDict[species_id];
                          }
                          if (modifierIdAsKeyReactionDict.ContainsKey(species_id))
                          {
                              otherReactionIdList = modifierIdAsKeyReactionDict[species_id];
                             
                              foreach (Guid id in otherReactionIdList)
                              {
                                  reactionIdList.Add(id);
                              }
                          }
                      }
                  }

                  // Graph construction

                  foreach (Guid re_id in reactionIdList)
                  {
                      int ind = reactionsDict[re_id];
                      downstreamGraph[i][ind] = 1;
                  }
              }

              return downstreamGraph;
          }
        
        public static int[][] UpStreamGraphforModel(Guid model_id)
        {
            Dictionary<Guid, string> speciesDict = new Dictionary<Guid, string>();

            Dictionary<Guid, List<Guid>> reactantIdAsKeyReactionDict = new Dictionary<Guid, List<Guid>>();
            Dictionary<Guid, List<Guid>> productIdAsKeyReactionDict = new Dictionary<Guid, List<Guid>>();
            Dictionary<Guid, List<Guid>> modifierIdAsKeyReactionDict = new Dictionary<Guid, List<Guid>>();

            Dictionary<Guid, List<Guid>> reactionIdAsKeyReactantDict = new Dictionary<Guid, List<Guid>>();
            Dictionary<Guid, List<Guid>> reactionIdAsKeyProductDict = new Dictionary<Guid, List<Guid>>();
            Dictionary<Guid, List<Guid>> reactionIdAsKeyModifierDict = new Dictionary<Guid, List<Guid>>();


            Dictionary<Guid, int> reactionsDict = new Dictionary<Guid, int>();

            ServerReaction[] AllReactions = ServerReaction.GetAllReactionsForModel(model_id);
            int index = 0;
            foreach (ServerReaction reaction in AllReactions)
            {
                if (!reactionsDict.ContainsKey(reaction.ID))
                {
                    reactionsDict.Add(reaction.ID, index);
                    index++;
                }
            }

            //Step 1: given a model, obtain all species and store them in dictionary
            ServerSpecies[] AllSpecies = ServerSpecies.GetAllSpeciesForModel(model_id);

            foreach (ServerSpecies species in AllSpecies)
            {
                if (!speciesDict.ContainsKey(species.ID))
                    speciesDict.Add(species.ID, species.Name);
            }

            //Step 2: for each species, collect the reaction
            foreach (KeyValuePair<Guid, string> pair in speciesDict)
            {
                Guid species_id = pair.Key;

                ServerReactionSpecies[] AllReationSpecies = ServerReactionSpecies.GetAllReactionsSpeciesForOneSpecies(species_id);

                foreach (ServerReactionSpecies reactionspecies in AllReationSpecies)
                {
                    Guid reaction_id = reactionspecies.ReactionId;
                    switch (reactionspecies.RoleId)
                    {
                        case 1:
                            //role as a reactant
                            if (!reactantIdAsKeyReactionDict.ContainsKey(species_id))
                                reactantIdAsKeyReactionDict[species_id] = new List<Guid>();

                            reactantIdAsKeyReactionDict[species_id].Add(reaction_id);

                            if (!reactionIdAsKeyReactantDict.ContainsKey(reaction_id))
                                reactionIdAsKeyReactantDict[reaction_id] = new List<Guid>();

                            reactionIdAsKeyReactantDict[reaction_id].Add(species_id);

                            break;
                        case 2:
                            //role as a producct
                            if (!productIdAsKeyReactionDict.ContainsKey(species_id))
                                productIdAsKeyReactionDict[species_id] = new List<Guid>();

                            productIdAsKeyReactionDict[species_id].Add(reaction_id);

                            if (!reactionIdAsKeyProductDict.ContainsKey(reaction_id))
                                reactionIdAsKeyProductDict[reaction_id] = new List<Guid>();

                            reactionIdAsKeyProductDict[reaction_id].Add(species_id);

                            break;
                        case 3:
                            //role as a modifier
                            if (!modifierIdAsKeyReactionDict.ContainsKey(species_id))
                                modifierIdAsKeyReactionDict[species_id] = new List<Guid>();

                            modifierIdAsKeyReactionDict[species_id].Add(reaction_id);

                            if (!reactionIdAsKeyModifierDict.ContainsKey(reaction_id))
                                reactionIdAsKeyModifierDict[reaction_id] = new List<Guid>();

                            reactionIdAsKeyModifierDict[reaction_id].Add(species_id);

                            break;
                        default: break;
                    }
                }
            }

            // Step 3: construct graph

            int num_reactions = reactionsDict.Count;

            int[][] upstreamGraph = new int[num_reactions][];

            for (int i = 0; i < num_reactions; i++)
            {
                upstreamGraph[i] = new int[num_reactions];
            }

            List<Guid> reactionsList = new List<Guid>();

            foreach (KeyValuePair<Guid, int> pair in reactionsDict)
            {
                reactionsList.Add(pair.Key);
            }

            for (int i = 0; i < num_reactions; i++)
            {
                Guid reaction_id = reactionsList[i];

                List<Guid> reactantIdList = new List<Guid>();

                if (reactionIdAsKeyReactantDict.ContainsKey(reaction_id))
                {
                    List<Guid> tempList = reactionIdAsKeyReactantDict[reaction_id];
                    if (tempList.Count > 0)
                    {
                        foreach (Guid id in tempList)
                            reactantIdList.Add(id);
                    }
                }
                List<Guid> reactionIdList = new List<Guid>();


                if (reactantIdList.Count > 0)
                {
                    foreach (Guid species_id in reactantIdList)
                    {
                        if (productIdAsKeyReactionDict.ContainsKey(species_id))
                        {
                            reactionIdList = productIdAsKeyReactionDict[species_id];
                        }
                    }
                }

                // Graph construction

                foreach (Guid re_id in reactionIdList)
                {
                    int ind = reactionsDict[re_id];
                    upstreamGraph[i][ind] = 1;
                }
            }

            return upstreamGraph;
        }
         
        public static Dictionary<Guid, int> DownstreamSearchGraph(int[][] graph, Dictionary<Guid, int> reactionsDictionary, Guid reaction_id, int maxStep)
        {
            Dictionary<Guid, int> result = new Dictionary<Guid, int>();

            Dictionary<int, int> id_based_dict = new Dictionary<int, int>();

            Dictionary<int, int> stepDict = new Dictionary<int, int>();

            for (int i = 0; i < graph.Length; i++)
            {
                stepDict.Add(i, 0);
            }

            int step = 0;
            Stack reactionsStack = new Stack();

            reactionsStack.Push(reactionsDictionary[reaction_id]);

            while (reactionsStack.Count > 0 && step < maxStep)
            {
                int re_ind = (int)reactionsStack.Pop();

                for (int j = 0; j < graph[re_ind].Length; j++)
                {
                    if (graph[re_ind][j] == 1 && (!id_based_dict.ContainsKey(j)))
                    {
                        step = stepDict[re_ind] + 1;
                        id_based_dict.Add(j, step);
                        stepDict[j] = step;
                        reactionsStack.Push(j);
                    }
                }
            }

            Dictionary<int, Guid> reverseReactionsDict = new Dictionary<int, Guid>();

            foreach (KeyValuePair<Guid, int> pair in reactionsDictionary)
            {
                reverseReactionsDict.Add(pair.Value, pair.Key);
            }


            foreach (KeyValuePair<int, int> pair in id_based_dict)
            {
                if (reverseReactionsDict[pair.Key] == reaction_id)
                    continue;

                result.Add(reverseReactionsDict[pair.Key], pair.Value);
            }

            return result;
        }

        public static Dictionary<Guid, int> UpstreamSearchGraph(int[][] graph, Dictionary<Guid, int> reactionsDictionary, Guid reaction_id, int maxStep)
        {
            Dictionary<Guid, int> result = new Dictionary<Guid, int>();
            Dictionary<int, int> id_based_dict = new Dictionary<int, int>();

            Dictionary<int, int> stepDict = new Dictionary<int, int>();

            for (int i = 0; i < graph.Length; i++)
            {
                stepDict.Add(i, 0);
            }

            int step = 0;
            Stack reactionsStack = new Stack();

            reactionsStack.Push(reactionsDictionary[reaction_id]);
            while (reactionsStack.Count > 0 && step < maxStep)
            {
                int sp_ind = (int)reactionsStack.Pop();

                for (int j = 0; j < graph[sp_ind].Length; j++)
                {
                    if (graph[sp_ind][j] == 1 && (!id_based_dict.ContainsKey(j)))
                    {
                        step = stepDict[sp_ind] + 1;
                        id_based_dict.Add(j, step);
                        stepDict[j] = step;
                        reactionsStack.Push(j);
                    }
                }
            }


            Dictionary<int, Guid> reverseReactionsDict = new Dictionary<int, Guid>();

            foreach (KeyValuePair<Guid, int> pair in reactionsDictionary)
            {
                reverseReactionsDict.Add(pair.Value, pair.Key);
            }


            foreach (KeyValuePair<int, int> pair in id_based_dict)
            {
                if (reverseReactionsDict[pair.Key] == reaction_id)
                    continue;
                result.Add(reverseReactionsDict[pair.Key], pair.Value);
            }

            return result;
        }


    }
}
