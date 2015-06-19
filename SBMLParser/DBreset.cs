using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PathwaysLib.ServerObjects;
using PathwaysLib.SoapObjects;
using libsbmlcs;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Data.SqlTypes;
using System.Configuration;

namespace PathwaysLib.SBMLParser
{
    class DBreset
    {
        public void ClearDB()// wipes all data from all tables
        {

            /******************************************************************
             * delete everything in the database.
             *******************************************************************/
            ServerSbase.DeleteAll("MapModelsPathways");
            ServerSbase.DeleteAll("MapSBaseGO");
            ServerSbase.DeleteAll("MapSpeciesMolecularEntities");
            ServerSbase.DeleteAll("MapReactionsProcessEntities");
            ServerSbase.DeleteAll("MapReactionECNumber");
            ServerSbase.DeleteAll("ModelOrganism");
            ServerSbase.DeleteAll("Event");
            ServerSbase.DeleteAll("EventAssignment");
            ServerSbase.DeleteAll("EventTrigger");
            ServerSbase.DeleteAll("EventDelay");
            ServerSbase.DeleteAll("ReactionSpecies");
            ServerSbase.DeleteAll("Parameter");
            ServerSbase.DeleteAll("Reaction"); // This one also kept for keeping frozen model layouts
            ServerSbase.DeleteAll("KineticLaw");
            ServerSbase.DeleteAll("[Constraint]");
            ServerSbase.DeleteAll("InitialAssignment");
            ServerSbase.DeleteAll("[Rule]");
            ServerSbase.DeleteAll("RuleType");
            ServerSbase.DeleteAll("Species"); // These two will be kept for keeping frozen model layouts
            ServerSbase.DeleteAll("Compartment");
            ServerSbase.DeleteAll("SpeciesType");
            ServerSbase.DeleteAll("CompartmentType");
            ServerSbase.DeleteAll("FunctionDefinition");
            ServerUnit.DeleteAll();
            ServerSbase.DeleteAll("ModelLayout"); // These two will be kept for keeping frozen model layouts
            ServerSbase.DeleteAll("Model"); // The original IDs are kept to not lose model layout information
            ServerSbase.DeleteAll("ReactionSpeciesRole");
            ServerSbase.DeleteAll("SBase"); //SBML parser will delete all SBase instances except the one belongs to the frozen layout needed tables
            ServerSbase.DeleteAll("DesignedBy");
            ServerSbase.DeleteAll("Author");
            ServerSbase.DeleteAll("ModelMetadata");
        }
        /// <summary>
        ///  This function clears the foreign key relations 
        ///  to the tables which are going to be deleted by ClearDB function
        ///  This function should be used together with ClearDB, should be called
        ///  before ClearDB.
        ///  Murat Kurtcephe 03/05/2011
        /// </summary>
        /// <returns></returns>
        public void clearFKRelations() {
            
            SqlCommand command = new SqlCommand("SELECT * FROM Reaction;");

            DataSet[]  ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                ServerReaction FkToClearObject = new ServerReaction(new DBRow(d));
                FkToClearObject.KineticLawId = Guid.Empty;
                FkToClearObject.UpdateDatabase();
            }
            command = new SqlCommand("SELECT * FROM Compartment;");
            ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            results = new ArrayList();
            foreach (DataSet d in ds)
            {
                ServerCompartment FkToClearObject = new ServerCompartment(new DBRow(d));
                FkToClearObject.UnitsId = Guid.Empty;
                FkToClearObject.CompartmentTypeId = Guid.Empty;
                FkToClearObject.UpdateDatabase();
            }
            command = new SqlCommand("SELECT * FROM Species;");
            ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            results = new ArrayList();
            foreach (DataSet d in ds)
            {
                ServerSpecies FkToClearObject = new ServerSpecies(new DBRow(d));
                FkToClearObject.SpeciesTypeId = Guid.Empty;
                FkToClearObject.UpdateDatabase();
            }
            
        }

        public void copyExistingData()
        {
            string oldDBConnectionString;
            string newDBConnectionString;
            
            // Collect the tables: attribute_names, attribute_values, molecular_entity_types, pathway_types, process_entity_roles
            if (ConfigurationManager.AppSettings.Get("oldDbConnectString") != null || ConfigurationManager.AppSettings.Get("dbConnectString") != null)
            {
                oldDBConnectionString = ConfigurationManager.AppSettings.Get("olddbConnectString");
                newDBConnectionString = ConfigurationManager.AppSettings.Get("dbConnectString");
            }
            else
            {
                throw new Exception("Database connection failed");
            }

            //BE: *** bulk copying the following tables from the old to the new database
            /*"attribute_names", "attribute_values", */
            ArrayList tableArray = new ArrayList(new string[9] { "Sbase", "Model", "ModelLayout", "Reaction", "Species",
                "Compartment","common_species","CompartmentClass","CompartmentClassDictionary"});
            SqlConnection connection1 = new SqlConnection(oldDBConnectionString);
            connection1.Open();

            SqlConnection connection2 = new SqlConnection(newDBConnectionString);
            connection2.Open();

            SqlDataReader reader; SqlCommand commandSourceData;
            SqlBulkCopy bcp;

            foreach (string table in tableArray)
            {
                commandSourceData = new SqlCommand(
                        "SELECT *" +
                        "FROM " + table + ";", connection1);
                reader = commandSourceData.ExecuteReader();
                bcp = new SqlBulkCopy(connection2);

                bcp.DestinationTableName =
                    table;

                // Write from the source to the 
                // destination.
                bcp.BulkCopyTimeout = 6000;
                bcp.WriteToServer(reader);
                bcp.Close();

                reader.Close();
            }
            
        }

    }
}
