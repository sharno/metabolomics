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
    public class ModelQueries
    {

        private ModelQueries() { }

        private static DataTable NewTable()
        {
            DataTable SpeciesTable = new DataTable();
            SpeciesTable.Columns.Add("Species_Id", typeof(string));
            SpeciesTable.Columns.Add("Step", typeof(string));
            SpeciesTable.Columns.Add("Direction", typeof(string));

            return SpeciesTable;
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

        public static DataSet[] FindModelsContainMetabolitesforGivenPathway(string pathwayID, string SM_mappingQualifier)
        {
            Dictionary<Guid, ServerMolecularEntity> MoleculeDict = new Dictionary<Guid, ServerMolecularEntity>();

            ServerProcess[] AllProcesses = ServerProcess.GetAllProcessesHasModels(new Guid(pathwayID));

            foreach (ServerProcess proc in AllProcesses)
              {
                  ServerMolecularEntity[] Molecules = proc.GetAllMolecularEntities();
                  
                  foreach (ServerMolecularEntity me in Molecules)
                  {
                      if (!MoleculeDict.ContainsKey(me.ID))
                      {
                          MoleculeDict.Add(me.ID, me);
                      }
                  }
              }

              DataTable dtSpecies = new DataTable();
              dtSpecies.Columns.Add("Model_Id", typeof(string));
              dtSpecies.Columns.Add("Species_Id", typeof(string));
              dtSpecies.Columns.Add("Qualifier_Id", typeof(string));
              dtSpecies.Columns.Add("MolecularEntity_Id", typeof(string));

              ServerMapSpeciesMolecularEntities[] speciesMolecularEntities;
              foreach (Guid moleculeID in MoleculeDict.Keys)
              {
                  if (SM_mappingQualifier != AnnotationQualifierManager.UnspecifiedQualifier)
                  {
                      int qualifierId = Convert.ToInt32(SM_mappingQualifier);
                      speciesMolecularEntities = ServerMapSpeciesMolecularEntities.GetMapSpeciesMolecularEntities(moleculeID, qualifierId);
                  }
                  else  
                  {
                      speciesMolecularEntities = ServerMapSpeciesMolecularEntities.GetMapSpeciesMolecularEntities(moleculeID);
                  }

                  if (speciesMolecularEntities.Length > 0)
                  {
                      foreach (ServerMapSpeciesMolecularEntities sme in speciesMolecularEntities)
                      {
                          Guid species_id = sme.SpeciesId;
                          Guid molecularEntity_id = sme.MolecularEntityId;
                          int qualifier_Id = sme.QualifierId;

                          ServerSpecies species = ServerSpecies.Load(species_id);

                          DataRow NewRow = dtSpecies.NewRow();
                          NewRow[0] = species.ModelId.ToString();
                          NewRow[1] = species.ID.ToString();
                          NewRow[2] = qualifier_Id.ToString();
                          NewRow[3] = molecularEntity_id.ToString();
                          dtSpecies.Rows.Add(NewRow); 
                      }
                  }
              }

               // Convert into a DataSet[]
              ArrayList dsSpeciesPairs = new ArrayList();
              foreach (DataRow drTemp in dtSpecies.Rows)
               {
                   DataSet dsTemp = new DataSet();
                   DataTable dtTemp = dtSpecies.Clone();
                   dsTemp.Tables.Add(dtTemp);
                   dsTemp.Tables[0].ImportRow(drTemp);
                   dsSpeciesPairs.Add(dsTemp);
               }

               return (DataSet[])dsSpeciesPairs.ToArray(typeof(DataSet));  

        }


        public static DataSet[] FindModelsContainReactionsforGivenPathway(string pathwayID, string PR_mappingQualifier)
        {
            ArrayList dsReactionPairs = new ArrayList();

            DataTable dtReaction = new DataTable();
            dtReaction.Columns.Add("Model_Id", typeof(string));
            dtReaction.Columns.Add("Reaction_Id", typeof(string));
            dtReaction.Columns.Add("KineticLaw_Id", typeof(string));
            dtReaction.Columns.Add("Species_Id", typeof(string));
            dtReaction.Columns.Add("Role_Id", typeof(string));
            dtReaction.Columns.Add("Qualifier_Id", typeof(string));
            dtReaction.Columns.Add("Process_Id", typeof(string));

            ServerMapReactionsProcessEntities[] reactionProcessEntities;

            ServerProcess[] AllProcesses = ServerProcess.GetAllProcessesHasModels(new Guid(pathwayID));
            
            foreach (ServerProcess process in AllProcesses)
            {
                if (PR_mappingQualifier != AnnotationQualifierManager.UnspecifiedQualifier)
                {
                    int qualifierId = Convert.ToInt32(PR_mappingQualifier);
                    reactionProcessEntities = ServerMapReactionsProcessEntities.AllMapReactionsProcessEntities(process.ID, qualifierId);
                }
                else
                {
                    reactionProcessEntities = ServerMapReactionsProcessEntities.AllMapReactionsProcessEntities(process.ID);
                }

                if (reactionProcessEntities.Length > 0)
                {
                    foreach (ServerMapReactionsProcessEntities rpe in reactionProcessEntities)
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
                            NewRow[6] = process.ID.ToString();
                            dtReaction.Rows.Add(NewRow);
                        }
                    }
                }
            }

            // Convert into a DataSet[]
            
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

        public static DataSet[] FindModelsAndReactionsforGivenProcess(string processID, string mappingQualifier)
        {
            ServerMapReactionsProcessEntities[] reactionProcessEntities;

            if (mappingQualifier != AnnotationQualifierManager.UnspecifiedQualifier)
            {
                int qualifierId = Convert.ToInt32(mappingQualifier);
                reactionProcessEntities = ServerMapReactionsProcessEntities.AllMapReactionsProcessEntities(new Guid(processID), qualifierId);
            }
            else
            {
                reactionProcessEntities = ServerMapReactionsProcessEntities.AllMapReactionsProcessEntities(new Guid(processID));
            }

            DataTable dtReaction = new DataTable();
            dtReaction.Columns.Add("Model_Id", typeof(string));
            dtReaction.Columns.Add("Reaction_Id", typeof(string));
            dtReaction.Columns.Add("KineticLaw_Id", typeof(string));
            dtReaction.Columns.Add("Species_Id", typeof(string));
            dtReaction.Columns.Add("Role_Id", typeof(string));
            dtReaction.Columns.Add("Qualifier_Id", typeof(string));

            if (reactionProcessEntities.Length > 0)
            {
                foreach (ServerMapReactionsProcessEntities rpe in reactionProcessEntities)
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


        public static DataSet[] FindModelsAndCompartmentsforGivenSpecies(string organismID, string speciesName, string option)
        {
            switch (option)
            {
                case "0":
                    return FindModelsWithSpeciesforGivenSpeciesByName(organismID, speciesName);
                    break;
                default: return null;

            }
        }

        public static DataSet[] FindModelsWithSpeciesforGivenSpeciesByName(string organismID, string speciesName)
        {
            ServerModel[] models;
            if (organismID.Equals( ServerOrganism.UnspecifiedOrganism))
            {
                models = ServerModel.AllModels();
            }
            else
            {
                models = ServerModel.GetModelsOfOrganism(new Guid(organismID));
            }

            Dictionary<Guid, int> modelDict = new Dictionary<Guid, int>();

            foreach (ServerModel model in models)
            {
                if (!modelDict.ContainsKey(model.ID))
                    modelDict.Add(model.ID, 1);
            }

            ArrayList dsSpeciesPairs = new ArrayList();

            ServerSpecies[] AllSpecies = ServerSpecies.AllSpeciesByName(speciesName);

            DataTable dtSpecies = ModelCompartmentTable();

            foreach (ServerSpecies species in AllSpecies)
            {
                if (modelDict.ContainsKey(species.ModelId))
                {
                    DataRow NewRow = dtSpecies.NewRow();
                    NewRow[0] = species.ModelId.ToString();
                    NewRow[1] = species.CompartmentId.ToString();
                    NewRow[2] = species.ID.ToString();
                    dtSpecies.Rows.Add(NewRow);
                }
            }

            // Convert into a DataSet[]
            foreach (DataRow drTemp in dtSpecies.Rows)
            {
                DataSet dsTemp = new DataSet();
                DataTable dtTemp = dtSpecies.Clone();
                dsTemp.Tables.Add(dtTemp);
                dsTemp.Tables[0].ImportRow(drTemp);
                dsSpeciesPairs.Add(dsTemp);
            }

            return (DataSet[])dsSpeciesPairs.ToArray(typeof(DataSet));

        }

        public static DataSet[] FindModelsWithSpeciesInGivenDomain(string organismID, string compartmentName, string option)
        {
            if (!compartmentName.Equals(ServerCompartment.UnspecifiedCompartment))
            {
                switch (option)
                {
                    case "0":
                        return FindModelsWithSpeciesInGivenDomainByName(organismID, compartmentName);
                        break;
                    case "1":
                        return FindModelsWithSpeciesInGivenDomainByGO(organismID, compartmentName);
                        break;
                    default: return null;

                }
            }
            else
            {
                //find all models given an organism without compartment constraints
                return FindModelsWithSpeciesGivenOrganism(organismID);
            }
            
        }
       
        public static DataSet[] FindModelsWithSpeciesGivenOrganism(string organismID)
        {
            ServerModel[] models;
            if (organismID.Equals(ServerOrganism.UnspecifiedOrganism))
            {
                models = ServerModel.AllModels();
            }
            else
            {
                models = ServerModel.GetModelsOfOrganism(new Guid(organismID));
            }

            ArrayList dsSpeciesPairs = new ArrayList();
            DataTable dtSpecies = ModelSpeciesTable();

            foreach (ServerModel model in models)
            {
                ServerSpecies[] AllSpecies = ServerSpecies.GetAllSpeciesForModel(model.ID);

                foreach (ServerSpecies species in AllSpecies)
                {
                     DataRow NewRow = dtSpecies.NewRow();
                        NewRow[0] = species.ModelId.ToString();
                        NewRow[1] = species.ID.ToString();
                        NewRow[2] = species.CompartmentId.ToString();
                        dtSpecies.Rows.Add(NewRow);
                       
                }
            }

            // Convert into a DataSet[]
            foreach (DataRow drTemp in dtSpecies.Rows)
            {
                DataSet dsTemp = new DataSet();
                DataTable dtTemp = dtSpecies.Clone();
                dsTemp.Tables.Add(dtTemp);
                dsTemp.Tables[0].ImportRow(drTemp);
                dsSpeciesPairs.Add(dsTemp);
            }

            return (DataSet[])dsSpeciesPairs.ToArray(typeof(DataSet));
        }

        public static DataSet[] FindModelsWithSpeciesInGivenDomainByGO(string organismID, string compartmentName)
        {

            ServerModel[] models;
            if (organismID.Equals(ServerOrganism.UnspecifiedOrganism))
            {
                models = ServerModel.AllModels();
            }
            else
            {
                models = ServerModel.GetModelsOfOrganism(new Guid(organismID));
            }

            Dictionary<Guid, int> modelDict = new Dictionary<Guid, int>();
            foreach (ServerModel model in models)
            {
                if (!modelDict.ContainsKey(model.ID))
                    modelDict.Add(model.ID, 1);
            }

            ArrayList dsSpeciesPairs = new ArrayList();
            ServerCompartment[] AllCompartments = ServerCompartment.AllCompartmentsByName(compartmentName);

            // Using GO-Mapping

            //compartment.id - sbaseId - goId

            Dictionary<Guid, int> compartmentDict = new Dictionary<Guid, int>();

            foreach (ServerCompartment compartment in AllCompartments)
            {
                if (!compartmentDict.ContainsKey(compartment.ID))
                {
                    compartmentDict.Add(compartment.ID, 1);
                }
                if (ServerMapSbaseGO.Exists(compartment.ID))
                {
                    ServerMapSbaseGO[] AllMaps = ServerMapSbaseGO.FindMappingBySbaseId(compartment.ID);

                    foreach (ServerMapSbaseGO mapSbaseGO in AllMaps)
                    {
                        ServerMapSbaseGO[] AllMapSbaseGOs = ServerMapSbaseGO.FindMappingByGOId(mapSbaseGO.GOId);
                        foreach (ServerMapSbaseGO map in AllMapSbaseGOs)
                        {
                            if (!compartmentDict.ContainsKey(map.SbaseId))
                            {
                                compartmentDict.Add(map.SbaseId, 1);
                            }

                        }
                    }
                }
            }


            DataTable dtSpecies = ModelSpeciesTable();

            foreach (KeyValuePair<Guid, int> pair in compartmentDict)
            {
                Guid compartment_ID = pair.Key;
                ServerSpecies[] AllSpecies = ServerSpecies.GetAllSpeciesGivenCompartment(compartment_ID);

                foreach (ServerSpecies species in AllSpecies)
                {
                    if (modelDict.ContainsKey(species.ModelId))
                    {
                        DataRow NewRow = dtSpecies.NewRow();
                        NewRow[0] = species.ModelId.ToString();
                        NewRow[1] = species.ID.ToString();
                        NewRow[2] = species.CompartmentId.ToString();
                        dtSpecies.Rows.Add(NewRow);
                    }

                }
            }

            // Convert into a DataSet[]
            foreach (DataRow drTemp in dtSpecies.Rows)
            {
                DataSet dsTemp = new DataSet();
                DataTable dtTemp = dtSpecies.Clone();
                dsTemp.Tables.Add(dtTemp);
                dsTemp.Tables[0].ImportRow(drTemp);
                dsSpeciesPairs.Add(dsTemp);
            }

            return (DataSet[])dsSpeciesPairs.ToArray(typeof(DataSet));

        }

        public static DataSet[] FindModelsWithSpeciesInGivenDomainByName(string organismID, string compartmentName)
        {
            ServerModel[] models;
            if (organismID.Equals(ServerOrganism.UnspecifiedOrganism))
            {
                models = ServerModel.AllModels();
            }
            else
            {
                models = ServerModel.GetModelsOfOrganism(new Guid(organismID));
            }

            Dictionary<Guid, int> modelDict = new Dictionary<Guid, int>();
            foreach (ServerModel model in models)
            {
                if(!modelDict.ContainsKey(model.ID))
                    modelDict.Add (model.ID, 1);
            }

            ArrayList dsSpeciesPairs = new ArrayList();
            ServerCompartment[] AllCompartments = ServerCompartment.AllCompartmentsByName(compartmentName);
            DataTable dtSpecies = ModelSpeciesTable();

            foreach(ServerCompartment compartment in AllCompartments )
            {
                ServerSpecies[] AllSpecies = ServerSpecies.GetAllSpeciesGivenCompartment(compartment.ID);

                foreach (ServerSpecies species in AllSpecies)
                {
                    if (modelDict.ContainsKey(species.ModelId))
                    {
                        DataRow NewRow = dtSpecies.NewRow();
                        NewRow[0] = species.ModelId.ToString();
                        NewRow[1] = species.ID.ToString();
                        NewRow[2] = species.CompartmentId.ToString();
                        dtSpecies.Rows.Add(NewRow);
                    }
                 
                }
            }

              // Convert into a DataSet[]
            foreach (DataRow drTemp in dtSpecies.Rows)
              {
                  DataSet dsTemp = new DataSet();
                  DataTable dtTemp = dtSpecies.Clone();
                  dsTemp.Tables.Add(dtTemp);
                  dsTemp.Tables[0].ImportRow(drTemp);
                  dsSpeciesPairs.Add(dsTemp);
              }

           return (DataSet[])dsSpeciesPairs.ToArray(typeof(DataSet));
             
        }

        public static DataSet[]  FindSpeciesInGivenStepsInModel(string model_id, string species_id, int direction, int steps, int radiusSpec)
      {
          ArrayList dsSpeciesPairs = new ArrayList();
          Dictionary<Guid, int> speciesDictionary = new Dictionary<Guid, int>();

          ServerSpecies[] AllSpecies = ServerSpecies.GetAllSpeciesForModel(new Guid(model_id));
          int index = 0;
          foreach (ServerSpecies species in AllSpecies)
          {
              if (!speciesDictionary.ContainsKey(species.ID))
              {
                  speciesDictionary.Add(species.ID, index);
                  index++;
              }
          }

          DataTable dtSpecies = NewTable();

          switch (direction)
          {
              case 0:
                  int[][] downStreamGraph = DownStreamGraphforModel(new Guid(model_id));
                  Dictionary<Guid, int> speciesAndDistanceList = DownstreamSearchGraph(downStreamGraph, speciesDictionary, new Guid(species_id), steps);
                  foreach (KeyValuePair<Guid, int> pair in speciesAndDistanceList)
                  {
                      if (radiusSpec == 1)//radius specification: exactly
                      {
                          if (pair.Value == steps)
                          {
                              DataRow NewRow = dtSpecies.NewRow();
                              NewRow[0] = pair.Key.ToString();
                              NewRow[1] = pair.Value.ToString();
                              NewRow[2] = "Downstream";
                              dtSpecies.Rows.Add(NewRow);
                          }

                      }
                      else //radius specification: at most
                      {
                          DataRow NewRow = dtSpecies.NewRow();
                          NewRow[0] = pair.Key.ToString();
                          NewRow[1] = pair.Value.ToString();
                          NewRow[2] = "Downstream";
                          dtSpecies.Rows.Add(NewRow);
                      }
                
                  }

                  break;

              case 1:
                  int[][] upStreamGraph = UpStreamGraphforModel(new Guid(model_id));
                  speciesAndDistanceList = UpstreamSearchGraph(upStreamGraph, speciesDictionary, new Guid(species_id), steps);
                  foreach (KeyValuePair<Guid, int> pair in speciesAndDistanceList)
                  {
                      if (radiusSpec == 1)//radius specification: exactly
                      {
                          if (pair.Value == steps)
                          {
                              DataRow NewRow = dtSpecies.NewRow();
                              NewRow[0] = pair.Key.ToString();
                              NewRow[1] = pair.Value.ToString();
                              NewRow[2] = "Upstream";
                              dtSpecies.Rows.Add(NewRow);
                          }

                      }
                      else //radius specification: at most
                      {
                          DataRow NewRow = dtSpecies.NewRow();
                          NewRow[0] = pair.Key.ToString();
                          NewRow[1] = pair.Value.ToString();
                          NewRow[2] = "Upstream";
                          dtSpecies.Rows.Add(NewRow);
                      }

                  }

                  break;

              case 2:
                  downStreamGraph = DownStreamGraphforModel(new Guid(model_id));
                  speciesAndDistanceList = DownstreamSearchGraph(downStreamGraph, speciesDictionary, new Guid(species_id), steps);
                  foreach (KeyValuePair<Guid, int> pair in speciesAndDistanceList)
                  {
                      if (radiusSpec == 1)//radius specification: exactly
                      {
                          if (pair.Value == steps)
                          {
                              DataRow NewRow = dtSpecies.NewRow();
                              NewRow[0] = pair.Key.ToString();
                              NewRow[1] = pair.Value.ToString();
                              NewRow[2] = "Downstream";
                              dtSpecies.Rows.Add(NewRow);
                          }

                      }
                      else //radius specification: at most
                      {
                          DataRow NewRow = dtSpecies.NewRow();
                          NewRow[0] = pair.Key.ToString();
                          NewRow[1] = pair.Value.ToString();
                          NewRow[2] = "Downstream";
                          dtSpecies.Rows.Add(NewRow);
                      }

                  }


                  upStreamGraph = UpStreamGraphforModel(new Guid(model_id));
                  speciesAndDistanceList = UpstreamSearchGraph(upStreamGraph, speciesDictionary, new Guid(species_id), steps);
                  foreach (KeyValuePair<Guid, int> pair in speciesAndDistanceList)
                  {
                      if (radiusSpec == 1)//radius specification: exactly
                      {
                          if (pair.Value == steps)
                          {
                              DataRow NewRow = dtSpecies.NewRow();
                              NewRow[0] = pair.Key.ToString();
                              NewRow[1] = pair.Value.ToString();
                              NewRow[2] = "Upstream";
                              dtSpecies.Rows.Add(NewRow);
                          }

                      }
                      else //radius specification: at most
                      {
                          DataRow NewRow = dtSpecies.NewRow();
                          NewRow[0] = pair.Key.ToString();
                          NewRow[1] = pair.Value.ToString();
                          NewRow[2] = "Upstream";
                          dtSpecies.Rows.Add(NewRow);
                      }

                  }


                  break;

              default: break;
                 
          }
          
          // Convert into a DataSet[]
          foreach (DataRow drTemp in dtSpecies.Rows)
          {
              DataSet dsTemp = new DataSet();
              DataTable dtTemp = dtSpecies.Clone();
              dsTemp.Tables.Add(dtTemp);
              dsTemp.Tables[0].ImportRow(drTemp);
              dsSpeciesPairs.Add(dsTemp);
          }

          return (DataSet[])dsSpeciesPairs.ToArray(typeof(DataSet));
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

              int num_species = speciesDict.Count;

              int[][] downstreamGraph = new int[num_species][];

              for (int i = 0; i < num_species; i++)
              {
                  downstreamGraph[i] = new int[num_species];
              }

              List<Guid> speciesList = new List<Guid>();
            
              foreach (KeyValuePair<Guid, string> pair in speciesDict)
              {
                  speciesList.Add(pair.Key);
              }

              Dictionary<Guid, int> speciesDictionary = new Dictionary<Guid, int>();
              for (int i = 0; i < num_species; i++)
              {
                  speciesDictionary.Add(speciesList[i], i);
              }

              for (int i = 0; i < num_species; i++)
              {
                  Guid species_id = speciesList[i];
                  List<Guid> reactionIdList = new List<Guid>();
                  List<Guid> otherReactionIdList = new List<Guid>();

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
             
                  List<Guid> productIdList = new List<Guid>();
                  if (reactionIdList.Count > 0)
                  {
                      foreach (Guid reaction_id in reactionIdList)
                      {
                          if(reactionIdAsKeyProductDict.ContainsKey (reaction_id ))
                          {
                              List<Guid> tempList = reactionIdAsKeyProductDict[reaction_id];
                              if (tempList.Count > 0)
                              {
                                  foreach (Guid id in tempList)
                                      productIdList.Add(id);
                              }
                          }
                          
                      }
                  }

                  // Graph construction, as we have reactant and product information

                  //Todo:  check if it is reversible???

                  foreach (Guid product_id in productIdList)
                  {
                      int index = speciesDictionary[product_id];
                      downstreamGraph[i][index] = 1;
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

            int num_species = speciesDict.Count;

            int[][] upstreamGraph = new int[num_species][];

            for (int i = 0; i < num_species; i++)
            {
                upstreamGraph[i] = new int[num_species];
            }

            List<Guid> speciesList = new List<Guid>();

            foreach (KeyValuePair<Guid, string> pair in speciesDict)
            {
                speciesList.Add(pair.Key);
            }

            Dictionary<Guid, int> speciesDictionary = new Dictionary<Guid, int>();
            for (int i = 0; i < num_species; i++)
            {
                speciesDictionary.Add(speciesList[i], i);
            }

            for (int i = 0; i < num_species; i++)
            {
                Guid species_id = speciesList[i];
                List<Guid> reactionIdList = new List<Guid>();
                List<Guid> otherReactionIdList = new List<Guid>();

                if (productIdAsKeyReactionDict.ContainsKey(species_id))
                {
                    reactionIdList = productIdAsKeyReactionDict[species_id];
                }

                //if (modifierIdAsKeyReactionDict.ContainsKey(species_id))
                //{
                //    otherReactionIdList = modifierIdAsKeyReactionDict[species_id];
                //    foreach (Guid id in otherReactionIdList)
                //    {
                //        reactionIdList.Add(id);
                //    }
                //}

                List<Guid> reactantIdList = new List<Guid>();
                if (reactionIdList.Count > 0)
                {
                    foreach (Guid reaction_id in reactionIdList)
                    {
                        if (reactionIdAsKeyReactantDict.ContainsKey(reaction_id))
                        {
                            List<Guid> tempList = reactionIdAsKeyReactantDict[reaction_id];
                            if (tempList.Count > 0)
                            {
                                foreach (Guid id in tempList)
                                    reactantIdList.Add(id);
                            }
                        }

                    }
                }

                // Graph construction, as we have reactant and product information

                //Todo:  check if it is reversible???

                foreach (Guid reactant_id in reactantIdList)
                {
                    int index = speciesDictionary[reactant_id];
                    upstreamGraph[i][index] = 1;
                }
            }

            return upstreamGraph;
        }

        public static Dictionary<Guid, int> DownstreamSearchGraph(int[][] graph, Dictionary<Guid, int> speciesDictionary, Guid species_id, int maxStep)
        {
            Dictionary<Guid, int> result = new Dictionary<Guid, int>();
            Dictionary<int, int> id_based_dict = new Dictionary<int, int>();

            Dictionary<int, int> stepDict = new Dictionary<int, int>();

            for (int i = 0; i < graph.Length; i++)
            {
                stepDict.Add(i, 0);
            }

            int step = 0;
            Stack speciesStack = new Stack();

            speciesStack.Push(speciesDictionary[species_id]);
            while (speciesStack.Count > 0 && step < maxStep)
            {
                int sp_ind = (int)speciesStack.Pop();
      
                for (int j = 0; j < graph[sp_ind].Length; j++)
                {
                    if (graph[sp_ind][j] == 1 && (!id_based_dict.ContainsKey(j)))
                    {
                        step = stepDict[sp_ind] + 1;
                        id_based_dict.Add(j, step);
                        stepDict[j] = step;
                        speciesStack.Push(j);
                    }
                }
            }


            Dictionary<int, Guid> reverseSpeciesDict = new Dictionary<int, Guid>();

            foreach (KeyValuePair<Guid, int> pair in speciesDictionary)
            {
                reverseSpeciesDict.Add(pair.Value, pair.Key);
            }


            foreach (KeyValuePair<int, int> pair in id_based_dict)
            {
                if (reverseSpeciesDict[pair.Key] == species_id)
                    continue;
                result.Add(reverseSpeciesDict[pair.Key], pair.Value);
            }

            return result;
        }

        public static Dictionary<Guid, int> UpstreamSearchGraph(int[][] graph, Dictionary<Guid, int> speciesDictionary, Guid species_id, int maxStep)
        {
            Dictionary<Guid, int> result = new Dictionary<Guid, int>();
            Dictionary<int, int> id_based_dict = new Dictionary<int, int>();

            Dictionary<int, int> stepDict = new Dictionary<int, int>();

            for (int i = 0; i < graph.Length; i++)
            {
                stepDict.Add(i, 0);
            }

            int step = 0;
            Stack speciesStack = new Stack();

            speciesStack.Push(speciesDictionary[species_id]);
            while (speciesStack.Count > 0 && step < maxStep)
            {
                int sp_ind = (int)speciesStack.Pop();

                for (int j = 0; j < graph[sp_ind].Length; j++)
                {
                    if (graph[sp_ind][j] == 1 && (!id_based_dict.ContainsKey(j)))
                    {
                        step = stepDict[sp_ind] + 1;
                        id_based_dict.Add(j, step);
                        stepDict[j] = step;
                        speciesStack.Push(j);
                    }
                }
            }


            Dictionary<int, Guid> reverseSpeciesDict = new Dictionary<int, Guid>();

            foreach (KeyValuePair<Guid, int> pair in speciesDictionary)
            {
                reverseSpeciesDict.Add(pair.Value, pair.Key);
            }


            foreach (KeyValuePair<int, int> pair in id_based_dict)
            {
                if (reverseSpeciesDict[pair.Key] == species_id)
                    continue;
                result.Add(reverseSpeciesDict[pair.Key], pair.Value);
            }

            return result;
        }

        private static DataTable NewModelPathwayTable()
        {
            DataTable ModelPathwayTable = new DataTable();

            DataColumn PathwayIdColumn = new DataColumn();
            PathwayIdColumn.DataType = System.Type.GetType("System.String");
            PathwayIdColumn.ColumnName = "pathwayid";
            ModelPathwayTable.Columns.Add(PathwayIdColumn);

            DataColumn PathwayNameColumn = new DataColumn();
            PathwayNameColumn.DataType = System.Type.GetType("System.String");
            PathwayNameColumn.ColumnName = "pathwayname";
            ModelPathwayTable.Columns.Add(PathwayNameColumn);

            DataColumn OrganismIdColumn = new DataColumn();
            OrganismIdColumn.DataType = System.Type.GetType("System.String");
            OrganismIdColumn.ColumnName = "organismid";
            ModelPathwayTable.Columns.Add(OrganismIdColumn);

            DataColumn OrganismNameColumn = new DataColumn();
            OrganismNameColumn.DataType = System.Type.GetType("System.String");
            OrganismNameColumn.ColumnName = "organismname";
            ModelPathwayTable.Columns.Add(OrganismNameColumn);

            DataColumn ModelIdColumn = new DataColumn();
            ModelIdColumn.DataType = System.Type.GetType("System.String");
            ModelIdColumn.ColumnName = "modelid";
            ModelPathwayTable.Columns.Add(ModelIdColumn);

            DataColumn ModelNameColumn = new DataColumn();
            ModelNameColumn.DataType = System.Type.GetType("System.String");
            ModelNameColumn.ColumnName = "modelname";
            ModelPathwayTable.Columns.Add(ModelNameColumn);


            return ModelPathwayTable;
        }
        
        public static DataSet[] FindPathwaysOfGivenModel(string sourceModelId, string orgId)
        {
            ServerModel sm = ServerModel.Load(new Guid(sourceModelId));
            ServerPathway[] pws = sm.GetAllPathways();
            ServerOrganismGroup org = new Guid(orgId)== Guid.Empty ? null : ServerOrganismGroup.Load(new Guid(orgId));
            if (org == null)
            {
                org = new ServerOrganismGroup("Unspecified", Guid.Empty, "");
            }

            DataTable modelPathwayTable = NewModelPathwayTable();
            DataRow newRow = null;
            foreach (ServerPathway pw in pws)
            {
                string pathwayid = pw.ID.ToString();
                string pathwayname = pw.Name;

                newRow = modelPathwayTable.NewRow();
                modelPathwayTable.Rows.Add(newRow);

                newRow["pathwayid"] = pathwayid;
                newRow["pathwayname"] = pathwayname;
                newRow["organismid"] = org.ID.ToString();
                newRow["organismname"] = org.Name;
                newRow["modelid"] = sm.ID.ToString();
                newRow["modelname"] = sm.Name;
            }

            ArrayList result = new ArrayList();
            foreach (DataRow drTemp in modelPathwayTable.Rows)
            {
                DataSet dsTemp = new DataSet();
                DataTable dtTemp = modelPathwayTable.Clone();
                dsTemp.Tables.Add(dtTemp);
                dsTemp.Tables[0].ImportRow(drTemp);
                result.Add(dsTemp);
            }

            return (DataSet[])result.ToArray(typeof(DataSet));
        }

        private static DataTable NewModelProcessTable()
        {
            DataTable ModelProcessTable = new DataTable();

            ModelProcessTable.Columns.Add("processid", typeof(string));
            ModelProcessTable.Columns.Add("processname", typeof(string));

            ModelProcessTable.Columns.Add("modelid", typeof(string));
            ModelProcessTable.Columns.Add("modelname", typeof(string));

            return ModelProcessTable;
        }

        public static DataSet[] FindReactionsOfGivenModel(string sourceModelId)
        {
            ServerModel sm = ServerModel.Load(new Guid(sourceModelId));
            ServerProcess[] sps = sm.GetAllProcesses();

            DataTable modelProcessTable = NewModelProcessTable();
            DataRow newRow = null;

            foreach (ServerProcess sp in sps)
            {
                string processid = sp.ID.ToString();
                string processname = sp.Name;

                newRow = modelProcessTable.NewRow();
                modelProcessTable.Rows.Add(newRow);

                newRow["processid"] = processid;
                newRow["processname"] = processname;
                //TO-DO: in which pathway is this reaction
                newRow["modelid"] = sm.ID.ToString();
                newRow["modelname"] = sm.Name;
            }

            ArrayList result = new ArrayList();
            foreach (DataRow drTemp in modelProcessTable.Rows)
            {
                DataSet dsTemp = new DataSet();
                DataTable dtTemp = modelProcessTable.Clone();
                dsTemp.Tables.Add(dtTemp);
                dsTemp.Tables[0].ImportRow(drTemp);
                result.Add(dsTemp);
            }

            return (DataSet[])result.ToArray(typeof(DataSet));
        }

        private static DataTable NewModelMoleculeTable()
        {
            DataTable ModelMoleculeTable = new DataTable();

            ModelMoleculeTable.Columns.Add("moleculeid", typeof(string));
            ModelMoleculeTable.Columns.Add("moleculename", typeof(string));
            ModelMoleculeTable.Columns.Add("typename", typeof(string));
            ModelMoleculeTable.Columns.Add("rolename", typeof(string));
            ModelMoleculeTable.Columns.Add("processid", typeof(string));
            ModelMoleculeTable.Columns.Add("processname", typeof(string));
            ModelMoleculeTable.Columns.Add("modelid", typeof(string));
            ModelMoleculeTable.Columns.Add("modelname", typeof(string));

            return ModelMoleculeTable;
        }

        public static DataSet[] FindMoleculesOfGivenModel(string sourceModelId)
        {
            ServerModel sm = ServerModel.Load(new Guid(sourceModelId));
            ArrayList mes = sm.GetAllMolecules();

            DataTable modelMoleculeTable = NewModelMoleculeTable();
            DataRow newRow = null;

            // lazy!  why 2 loops?
            foreach (ArrayList me_databag in mes)
            {
                newRow = modelMoleculeTable.NewRow();
                modelMoleculeTable.Rows.Add(newRow);

                newRow["moleculeid"] = me_databag[0].ToString();
                newRow["moleculename"] = me_databag[1].ToString();
                newRow["typename"] = me_databag[2].ToString();
                newRow["rolename"] = me_databag[3].ToString();
                newRow["processid"] = me_databag[4].ToString();
                newRow["processname"] = me_databag[5].ToString();
                newRow["modelid"] = sm.ID.ToString();
                newRow["modelname"] = sm.Name;
            }

            ArrayList result = new ArrayList();
            foreach (DataRow drTemp in modelMoleculeTable.Rows)
            {
                DataSet dsTemp = new DataSet();
                DataTable dtTemp = modelMoleculeTable.Clone();
                dsTemp.Tables.Add(dtTemp);
                dsTemp.Tables[0].ImportRow(drTemp);
                result.Add(dsTemp);
            }

            return (DataSet[])result.ToArray(typeof(DataSet));
        }
  
    }
}
