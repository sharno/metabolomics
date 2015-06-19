#region Using Declarations
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using PathwaysLib.SoapObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
using System.Collections.Generic;
using PathwaysLib.GraphObjects;
#endregion

namespace PathwaysLib.ServerObjects
{

    public class ServerModel : ServerSbase, IGraphModel
    {

        #region Constructor, Destructor, ToString
        private ServerModel()
        {
        }

        /// <summary>
        /// Constructor for server Model wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerModel object from a
        /// SoapModel object.
        /// </remarks>
        public ServerModel(SoapModel data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __ModelRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __ModelRow = LoadRow(data.ID);
                    break;
                default:
                    throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
            }

            // get potential updates from Soap object, unless it's supposed to be read only
            if (data.Status != ObjectStatus.ReadOnly)
                UpdateFromSoap(data);

        }

        /// <summary>
        /// Constructor for server DataSource wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerDataSource object from a
        /// DataSet.
        /// </remarks>
        /// <param name="data">
        /// DataSet to load into the object.
        /// </param>
        public ServerModel(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __ModelRow = data;

        }

        public ServerModel(DBRow modelRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __ModelRow = modelRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerModel()
        {
        }
        #endregion


        #region Member Variables
        public static readonly string UnspecifiedModel = "00000000-0000-0000-0000-000000000000";
        private static readonly string __TableName = "Model";
        protected DBRow __ModelRow;
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the Model ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __ModelRow.GetGuid("id");
            }
            set
            {
                base.ID = value; //  update base class ID as well
                __ModelRow.SetGuid("id", value);
            }
        }


        /// <summary>
        /// Get/set the Model sbmlId.
        /// </summary>
        public string SbmlId
        {
            get
            {
                return __ModelRow.GetString("sbmlId");
            }
            set
            {
                __ModelRow.SetString("sbmlId", value);
            }
        }


        /// <summary>
        /// Get/set the Model name.
        /// </summary>
        public string Name
        {
            get
            {
                return __ModelRow.GetString("name");
            }
            set
            {
                __ModelRow.SetString("name", value);
            }
        }

        /// <summary>
        /// Get/set the Model sbmlLevel.
        /// </summary>
        public int SbmlLevel
        {
            get
            {
                return __ModelRow.GetInt("sbmlLevel");
            }
            set
            {
                __ModelRow.SetInt("sbmlLevel", value);
            }
        }

        /// <summary>
        /// Get/set the Model sbmlVersion.
        /// </summary>
        public int SbmlVersion
        {
            get
            {
                return __ModelRow.GetInt("sbmlVersion");
            }
            set
            {
                __ModelRow.SetInt("sbmlVersion", value);
            }
        }

        /// <summary>
        /// Get/set the Model dataSourceId.
        /// </summary>
        public int DataSourceId
        {
            get
            {
                return __ModelRow.GetInt("dataSourceId");
            }
            set
            {
                __ModelRow.SetInt("dataSourceId", value);
            }
        }

        public string SbmlFile
        {
            get
            {
                return __ModelRow.GetString("sbmlFile");
            }
            set
            {
                __ModelRow.SetString("sbmlFile", value);
            }
        }

        public string SbmlFileName
        {
            get
            {
                return __ModelRow.GetString("sbmlFileName");
            }
            set
            {
                __ModelRow.SetString("sbmlFileName", value);
            }
        }

        #endregion


        #region Methods
        /// <summary>
        /// Returns a representation of this object suitable for being
        /// sent to a client via SOAP.
        /// </summary>
        /// <returns>
        /// A SoapObject object capable of being passed via SOAP.
        /// </returns>
        public override SoapObject PrepareForSoap(SoapObject derived)
        {
            SoapModel retval = (derived == null) ?
                retval = new SoapModel() : retval = (SoapModel)derived;


            //fill base class properties
            base.PrepareForSoap(retval);

            retval.ID = this.ID;
            retval.SbmlId = this.SbmlId;
            retval.Name = this.Name;
            retval.SbmlLevel = this.SbmlLevel;
            retval.SbmlVersion = this.SbmlVersion;
            retval.DataSourceId = this.DataSourceId;
            retval.SbmlFile = this.SbmlFile;
            retval.SbmlFileName = this.SbmlFileName;
            retval.Status = ObjectStatus.NoChanges;

            return retval;
        }

        /// <summary>
        /// Consumes a SoapObject object and updates the ServerModel
        /// from it.
        /// </summary>
        /// <param name="o">
        /// The SoapObject object to update from, potentially containing
        /// changes to the Model relation.
        /// </param>
        protected override void UpdateFromSoap(SoapObject o)
        {
            SoapModel c = o as SoapModel;

            // set base class properties
            base.UpdateFromSoap(o);

            // use the ID set in the base class or the new ID generated by the base class
            this.ID = base.ID;


            if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
                c.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = c.ID;
            this.SbmlId = c.SbmlId;
            this.Name = c.Name;
            this.SbmlLevel = c.SbmlLevel;
            this.SbmlVersion = c.SbmlVersion;
            this.DataSourceId = c.DataSourceId;
            this.SbmlFile = c.SbmlFile;
            this.SbmlFileName = c.SbmlFileName;

        }

        /// <summary>
        /// Update the base class's data row, then the derived class's row
        /// </summary>
        public override void UpdateDatabase()
        {
            base.UpdateDatabase();
            __ModelRow.UpdateDatabase();
        }

        public ServerProcess[] GetAllProcesses()
        {
            // distinct processes
            string query = @"   select pr.*
                                from processes pr
                                where pr.id IN
                                    (	Select distinct pr.id
                                        from model_process mpr, processes pr 
                                        where mpr.model_id=@modelid and mpr.process_id = pr.id )
                                order by pr.name";

            SqlCommand command = DBWrapper.BuildCommand(query, "@modelid", SqlDbType.UniqueIdentifier, this.ID);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerProcess(new DBRow(d)));
            }
            return (ServerProcess[])results.ToArray(typeof(ServerProcess));
        }


        /// <summary>
        /// Checks if Pathways related to a model exist!!
        /// </summary>
        public Boolean CheckPathwaysExist()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"select TOP 1 p.Id
                from  mapmodelspathways mp,Pathways p
                where mp.modelId = @mid	and mp.pathwayId = p.Id	                
                ", "@mid", SqlDbType.UniqueIdentifier, this.ID);

            DataSet ds = new DataSet();
            if (DBWrapper.LoadSingle(out ds, ref command, true) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets all the pathways related with "this" model
        /// <returns>
        /// ServerPathway[]
        /// </returns>
        /// </summary>
        /// <returns></returns>
        public ServerPathway[] GetAllPathways()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"select p.*
                from  mapmodelspathways mp,Pathways p
                where mp.modelId = @mid	and mp.pathwayId = p.Id	                
                ", "@mid", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerPathway(new DBRow(d)));
            }

            return (ServerPathway[])results.ToArray(typeof(ServerPathway));
        }

        public static ServerModel[] FindModelsByCompartments(string withKeggData, string substring, SearchMethod searchMethod, int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;
            String s = substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            string particularSetOfModels = QueryStringForSetOfModels(withKeggData);

            string commandText = @" SELECT *
					                FROM model  
                                    WHERE model.id in 
                                    (Select distinct modelId 
                                    From Compartment
                                    WHERE name='" + s + "')";

            SqlCommand command = DBWrapper.BuildCommand(commandText, "@substring1", SqlDbType.VarChar, s);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModel(new DBRow(d)));
            }

            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }

        public static ServerModel[] FindModels(string withKeggData, string substring, SearchMethod searchMethod, int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;

            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            string particularSetOfModels = QueryStringForSetOfModels(withKeggData);

            string commandText = @" SELECT *
					                FROM ( SELECT TOP " + maxRecords.ToString() + @" *
							               FROM ( SELECT TOP " + bigNum.ToString() + @" *
								                  FROM " + __TableName + @" m 
                                                  WHERE [name] " +
                                                        (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring " +
                                                  "and" +
                                                  "     m.id in (" + particularSetOfModels + ") " +
                                                  "ORDER BY name " + @"  
                                                ) " + __TableName + @"  
							               ORDER BY name DESC" + @"  
                                         ) " + __TableName + @"
                                    ORDER BY NAME";

            SqlCommand command = DBWrapper.BuildCommand(commandText, "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModel(new DBRow(d)));
            }

            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }

        private static string QueryStringForSetOfModels(string withKeggData)
        {
            string queryString = "";
            switch (withKeggData)
            {
                case "keggreactions":
                    queryString = @"            (select distinct m.id  -- is_common
                                                from	model m, 
                                                        model_process mp, 
                                                        external_database_links exk, 
                                                        process_relationship prel
                                                where	m.id = mp.model_id and
                                                        mp.process_id = prel.process1_id

                                                UNION

                                                select distinct m.id   -- source should be kegg
                                                from	model m, 
                                                        model_process mp
                                                where	m.id = mp.model_id and
                                                        mp.process_id NOT IN (select local_id from external_database_links)
                                                )";
                    break;
                case "keggmolecules":
                    queryString = @"(select distinct m.id           -- is_common
                                    from	model m, 
                                            model_process mp, 
                                            process_entities pe,
	                                        molecular_entity_relationship mer
                                    where	m.id = mp.model_id and
                                            mp.process_id = pe.process_id and
                                            pe.entity_id = mer.model_entity

                                    UNION

                                    select distinct m.id           -- source should be kegg
                                    from	model m, 
                                            model_process mp, 
                                            process_entities pe
                                    where	m.id = mp.model_id and
                                            mp.process_id = pe.process_id and
                                            pe.entity_id not IN (select local_id from external_database_links)
						           )";
                    break;
                case "nokeggdata":
                    queryString = @"    select m.id
                                        from model m
                                        where m.id not IN (  -- models with kegg reactions
                                                            (select distinct m.id  -- is_common
                                                            from	model m, 
	                                                                model_process mp, 
	                                                                external_database_links exk, 
	                                                                process_relationship prel
                                                            where	m.id = mp.model_id and
	                                                                mp.process_id = prel.process1_id

                                                            UNION

                                                            select distinct m.id   -- source should be kegg
                                                            from	model m, 
	                                                                model_process mp
                                                            where	m.id = mp.model_id and
	                                                                mp.process_id NOT IN (select local_id from external_database_links)
                                                            )
                                                            UNION
                                                            -- models with kegg molecules
                                                            (select distinct m.id           -- is_common
                                                            from	model m, 
	                                                                model_process mp, 
	                                                                process_entities pe,
	                                                                molecular_entity_relationship mer
                                                            where	m.id = mp.model_id and
	                                                                mp.process_id = pe.process_id and
	                                                                pe.entity_id = mer.model_entity

                                                            UNION

                                                            select distinct m.id           -- source should be kegg
                                                            from	model m, 
	                                                                model_process mp, 
	                                                                process_entities pe
                                                            where	m.id = mp.model_id and
	                                                                mp.process_id = pe.process_id and
	                                                                pe.entity_id not IN (select local_id from external_database_links)
                                                            ))";
                    break;
            }
            return queryString;
        }

        public static int CountFindModelsByCompartments(string withKeggData, string substring, SearchMethod searchMethod)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            string particularSetOfModels = QueryStringForSetOfModels(withKeggData);

            string models = @"  SELECT Count(id)
					            FROM Compartment WHERE outside='a50694fc-e256-44f5-910a-f7f68c52f9c1' AND id!='a50694fc-e256-44f5-910a-f7f68c52f9c1'";// WHERE [name] " +
            //(searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring";

            string commandText = @" Select Count(distinct modelId)
                                    From Compartment";//+ @" m
            //Where   m.id  IN (" + particularSetOfModels + " INTERSECT " + models + ") ";

            SqlCommand command = DBWrapper.BuildCommand(models, "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }

        public static int CountFindModels(string withKeggData, string substring, SearchMethod searchMethod)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            string particularSetOfModels = QueryStringForSetOfModels(withKeggData);

            string models = @"  SELECT id
					            FROM " + __TableName + " WHERE [name] " +
                                            (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring";

            string commandText = @" Select Count(*)
                                    From " + __TableName + @" m
                                    Where   m.id  IN (" + particularSetOfModels + " INTERSECT " + models + ") ";

            SqlCommand command = DBWrapper.BuildCommand(commandText, "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }

        public static ServerModel LoadP(Guid id)
        {
            string commandText = @" SELECT *
					                FROM compartment WHERE id=@id;";
            SqlCommand command = DBWrapper.BuildCommand(commandText, "@id", SqlDbType.UniqueIdentifier, id);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new ServerModel(new DBRow(ds));
        }

        public ServerModel[] GetImmediateChildren()
        {
            //return GetImmediateChildren(this.ID);
            string commandText = @" SELECT *
					                FROM compartment WHERE outside=@id ORDER BY size;";

            //@" SELECT DISTINCT name
            //					                FROM compartment  
            //                                    WHERE name<>'Blood'";

            SqlCommand command = DBWrapper.BuildCommand(commandText, "@id", SqlDbType.UniqueIdentifier, this.ID);
            //SqlCommand command = DBWrapper.BuildCommand(commandText, "@id", SqlDbType.UniqueIdentifier, strPID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModel(new DBRow(d)));
            }

            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ServerModel[] GetImmediateChildren(Guid compID)
        {
            string commandText = @" SELECT *
					                FROM compartment WHERE outside=@id ORDER BY size;";

            //@" SELECT DISTINCT name
            //					                FROM compartment  
            //                                    WHERE name<>'Blood'";

            SqlCommand command = DBWrapper.BuildCommand(commandText, "@id", SqlDbType.UniqueIdentifier, compID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModel(new DBRow(d)));
            }

            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }

        public static ServerModel[] FindCompartments(string withKeggData, string substring, SearchMethod searchMethod, int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;

            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            string particularSetOfModels = QueryStringForSetOfModels(withKeggData);

            string commandText = @" SELECT *
					                FROM compartment 
            WHERE outside='a50694fc-e256-44f5-910a-f7f68c52f9c1' AND id!='a50694fc-e256-44f5-910a-f7f68c52f9c1'";
            //WHERE name<>'Blood' ORDER BY size";

            //@" SELECT DISTINCT name
            //					                FROM compartment  
            //                                    WHERE name<>'Blood'";

            SqlCommand command = DBWrapper.BuildCommand(commandText, "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModel(new DBRow(d)));
            }

            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }

        public ArrayList GetAllMolecules()
        {
            string query = @"   select	
		                                me.id as molid, 
		                                me.name as molname,
                                        met.name as typename, 
		                                per.name as rolename, 
		                                pr.id as processid, 
		                                pr.name as processname
                                from	
		                                molecular_entities me, 
		                                molecular_entity_types met, 
		                                process_entity_roles per, 
		                                process_entities pe, 
		                                processes pr, 
		                                model_process mp
                                where
		                                mp.model_id = @modelid and
		                                mp.process_id = pr.id and
		                                pr.id = pe.process_id and
		                                pe.role_id = per.role_id and
		                                pe.entity_id = me.id and
		                                me.type_id = met.type_id
                                order by me.name";

            SqlCommand command = DBWrapper.BuildCommand(query, "@modelid", SqlDbType.UniqueIdentifier, this.ID);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(ExtractTheMoleculeRowRepresentative(d));
            }
            return results;
        }

        private ArrayList ExtractTheMoleculeRowRepresentative(DataSet d)
        {
            ArrayList molRowRep = new ArrayList();
            if (d.Tables != null && d.Tables[0].Rows != null && d.Tables[0].Rows[0].ItemArray.Length == 6)
            {
                molRowRep.Add(d.Tables[0].Rows[0].ItemArray[0]);
                molRowRep.Add(d.Tables[0].Rows[0].ItemArray[1]);
                molRowRep.Add(d.Tables[0].Rows[0].ItemArray[2]);
                molRowRep.Add(d.Tables[0].Rows[0].ItemArray[3]);
                molRowRep.Add(d.Tables[0].Rows[0].ItemArray[4]);
                molRowRep.Add(d.Tables[0].Rows[0].ItemArray[5]);
            }
            return molRowRep;
        }

        public ServerProcess[] GetAllKeggProcesses()
        {
            string queryString1 = @"select pre.process1_id
                                    from process_relationship pre, model_process mp
                                    where   mp.model_id = @modelid and 
                                            pre.process1_id = mp.process_id and 
                                            pre.relationship_id='1'"; // common
            string queryString2 = @"Select mp.process_id
                                    from external_database_links ex, model_process mp
                                    where mp.model_id=@modelid and mp.process_id = ex.local_id"; // external

            string queryString = @" Select p.*
                                    from model_process mp, processes p
                                    where mp.model_id = @modelid and mp.process_id not in (" + queryString1 +
                                          ") and mp.process_id not in (" + queryString2 + ") and mp.process_id = p.id;";

            SqlCommand command = DBWrapper.BuildCommand(queryString, "@modelid", SqlDbType.UniqueIdentifier, this.ID);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerProcess(new DBRow(d)));
            }
            return (ServerProcess[])results.ToArray(typeof(ServerProcess));
        }

        public ServerProcess[] GetCommonProcesses()
        {
            string queryString1 = @"select pre.process1_id
                                    from process_relationship pre, model_process mp
                                    where   mp.model_id = @modelid and 
                                            pre.process1_id = mp.process_id and 
                                            pre.relationship_id='1'"; // common

            string queryString = @" Select p.*
                                    from model_process mp, processes p
                                    where mp.model_id = @modelid " +
                                          " and mp.process_id in (" + queryString1 +
                                          ") and mp.process_id = p.id;";
            SqlCommand command = DBWrapper.BuildCommand(queryString, "@modelid", SqlDbType.UniqueIdentifier, this.ID);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerProcess(new DBRow(d)));
            }
            return (ServerProcess[])results.ToArray(typeof(ServerProcess));
        }

        public ServerProcess[] GetAllBiomodelsProcesses()
        {
            string queryString1 = @"select pre.process1_id
                                    from process_relationship pre, model_process mp
                                    where   mp.model_id = @modelid and 
                                            pre.process1_id = mp.process_id and 
                                            pre.relationship_id='1'"; // common
            string queryString2 = @"Select mp.process_id
                                    from external_database_links ex, model_process mp
                                    where mp.model_id=@modelid and mp.process_id = ex.local_id"; // external

            string queryString = @" Select p.*
                                    from model_process mp, processes p
                                    where mp.model_id = @modelid and mp.process_id not in (" + queryString1 +
                                          ") and mp.process_id in (" + queryString2 + ") and mp.process_id = p.id;";


            SqlCommand command = DBWrapper.BuildCommand(queryString, "@modelid", SqlDbType.UniqueIdentifier, this.ID);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerProcess(new DBRow(d)));
            }
            return (ServerProcess[])results.ToArray(typeof(ServerProcess));
        }

        public static ServerModel[] GetModelsOfOrganism(Guid orgId)
        {
            string queryString = @" SELECT model.* 
                                    FROM Model model, ModelOrganism modelOrganism
                                    WHERE model.id = modelOrganism.modelId AND modelOrganism.organismGroupId = @orgId 
                                    ORDER BY model.name;";

            SqlCommand command = DBWrapper.BuildCommand(queryString, "@orgId ", SqlDbType.UniqueIdentifier, orgId);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModel(new DBRow(d)));
            }

            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }

        public static ServerModel[] GetAllModels()
        {
            SqlCommand command = DBWrapper.BuildCommand("Select * from " + __TableName + " order by name");
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModel(new DBRow(d)));
            }

            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }
        public static ServerModel[] GetAllModelNames()
        {
            SqlCommand command = DBWrapper.BuildCommand("Select Id, Name from " + __TableName + " order by name");
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModel(new DBRow(d)));
            }

            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }
        public ServerFunctionDefinition[] GetAllFunctionDefinitions()
        {
            string commandText = @" SELECT *
					                FROM FunctionDefinition WHERE modelId=@id ;";

            //@" SELECT DISTINCT name
            //					                FROM compartment  
            //                                    WHERE name<>'Blood'";

            SqlCommand command = DBWrapper.BuildCommand(commandText, "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerFunctionDefinition(new DBRow(d)));
            }

            return (ServerFunctionDefinition[])results.ToArray(typeof(ServerFunctionDefinition));
        }

        public ServerUnitDefinition[] GetAllUnitDefinitions()
        {
            string commandText = @" SELECT *
					                FROM UnitDefinition WHERE modelId=@id ;";

            //@" SELECT DISTINCT name
            //					                FROM compartment  
            //                                    WHERE name<>'Blood'";

            SqlCommand command = DBWrapper.BuildCommand(commandText, "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerUnitDefinition(new DBRow(d)));
            }

            return (ServerUnitDefinition[])results.ToArray(typeof(ServerUnitDefinition));
        }

        public DataSet GetAllParameter()
        {
            string commandText = @" SELECT name, value, sbmlId, constant
					                FROM Parameter WHERE modelId=@id ;";

            SqlCommand command = DBWrapper.BuildCommand(commandText, "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet ds = new DataSet();
            int r = DBWrapper.Instance.ExecuteQuery(out ds, ref command);

            return ds;
        }

        public ServerParameter[] GetAllParameters()
        {
            string commandText = @" SELECT *
					                FROM Parameter WHERE modelId=@id ;";

            //@" SELECT DISTINCT name
            //					                FROM compartment  
            //                                    WHERE name<>'Blood'";

            SqlCommand command = DBWrapper.BuildCommand(commandText, "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerParameter(new DBRow(d)));
            }

            return (ServerParameter[])results.ToArray(typeof(ServerParameter));
        }


        /// <summary>
        /// Checks if Compartments related to a model exist!!
        /// </summary>
        public Boolean CheckCompartmentsExist()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT TOP 1 re.[modelId]
				FROM Compartment re
				WHERE re.[modelId] = @id;",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet ds = new DataSet();
            if (DBWrapper.LoadSingle(out ds, ref command, true) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets all the Pathways related to a model!! not implemented
        /// </summary>

        /*public ServerPathway[] GetAllPathways()
        {
            ServerPathway[] ser = new ServerPathway[10];
            return ser;
        }*/
        /// <summary>
        /// Gets all the Compartments related to a model!!
        /// </summary>

        public ServerCompartment[] GetAllCompartments()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT re.*
				FROM Compartment re
				WHERE re.[modelId] = @id 
				ORDER BY re.[name];",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCompartment(new DBRow(d)));

            }

            return (ServerCompartment[])results.ToArray(typeof(ServerCompartment));
        }
        public ServerCompartment[] GetAllCompartmentNames()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT re.Id, re.Name
				FROM Compartment re
				WHERE re.[modelId] = @id",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCompartment(new DBRow(d)));
            }

            return (ServerCompartment[])results.ToArray(typeof(ServerCompartment));
        }

        /// <summary>
        /// Checks if Reactions related to a model exist!!
        /// </summary>
        public Boolean CheckReactionsExist()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT TOP 1 re.[modelId]
				FROM Reaction re
				WHERE re.[modelId] = @id",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet ds = new DataSet();
            if (DBWrapper.LoadSingle(out ds, ref command, true) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Gets all the Reactions related to a model!!
        /// </summary>

        public ServerReaction[] GetAllReactions()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT re.*
				FROM Reaction re
				WHERE re.[modelId] = @id 
				ORDER BY re.[name];",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReaction(new DBRow(d)));

            }

            return (ServerReaction[])results.ToArray(typeof(ServerReaction));
        }

        /// <summary>
        /// Gets all the Reaction names related to a model!!
        /// </summary>

        public ServerReaction[] GetAllReactionNames()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT re.Id, re.Name
				FROM Reaction re
				WHERE re.[modelId] = @id;",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReaction(new DBRow(d)));
            }
            return (ServerReaction[])results.ToArray(typeof(ServerReaction));
        }


        /// <summary>
        /// Checks if Species related to a model exist!!
        /// </summary>
        public Boolean CheckSpeciesExist()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT TOP 1 spe.[modelId]
				FROM Species spe
				WHERE spe.[modelId] = @id;",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet ds = new DataSet();
            if (DBWrapper.LoadSingle(out ds, ref command, true) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets all the Species related to a model!! not implemented
        /// </summary>
        public ServerSpecies[] GetAllSpecies()
        {

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT spe.*
				FROM Species spe
				WHERE spe.[modelId] = @id 
				ORDER BY spe.[name];",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerSpecies(new DBRow(d)));

            }

            return (ServerSpecies[])results.ToArray(typeof(ServerSpecies));

        }

        /// <summary>
        /// Gets all the Species related to a model!! not implemented
        /// </summary>
        public ServerSpecies[] GetAllSpeciesNames()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT spe.Id, spe.Name
				FROM Species spe
				WHERE spe.[modelId] = @id",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerSpecies(new DBRow(d)));
            }

            return (ServerSpecies[])results.ToArray(typeof(ServerSpecies));
        }

        /// <summary>
        /// Gets all the Processes related to a model!! not implemented
        /// </summary>

        /*public ServerProcess[] GetAllProcesses()
        {
          ServerProcess[] ser =new ServerProcess[10];
          return ser;
        }*/
        /// <summary>
        /// Gets all the molecules related to a model!! not implemented
        /// </summary>
        /*public ArrayList GetAllMolecules()
        {
            ArrayList ser = new ArrayList();
            return ser;
        }*/

        /// <summary>
        /// Get the freezed layout for a single pathway.
        /// </summary>
        public static string GetModelLayout(Guid modelid)
        {
            try
            {
                DBWrapper.Instance = new DBWrapper();
                //construct select sql
                String select = "select layout from ModelLayout where id='";
                select += (modelid.ToString() + "';");
                SqlCommand cmd = new SqlCommand(select);

                return DBWrapper.Instance.ExecuteScalar(ref cmd).ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("when retrieving model layout field: " + e);
                return "Error when Quering";//null;
            }
            finally
            {
                DBWrapper.Instance.Close();
            }
        }

        public static string SaveModelLayout(Guid modelid, String layout)
        {
            try
            {
                DBWrapper.Instance = new DBWrapper();
                //construct select sql
                //String select = "UPDATE ModelLayout SET layout='";
                //select += (layout + "';");
                //select += "IF @@ROWCOUNT=0  INSERT INTO ModelLayout VALUES (" + modelid + "," + layout + ");";
                String select = "SaveLayout";

                SqlCommand cmd = new SqlCommand(select);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@id", modelid));
                cmd.Parameters.Add(new SqlParameter("@layout", layout));
                //SqlParameter parameter = cmd.Parameters.Add("@result", SqlDbType.NVarChar);
                //parameter.Direction = ParameterDirection.Output;                
                if (DBWrapper.Instance.ExecuteScalar(ref cmd) == null)
                    return "null";
                else
                    return DBWrapper.Instance.ExecuteScalar(ref cmd).ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("when retrieving model layout field: " + e);
                return "Error when Quering";//null;
            }
            finally
            {
                DBWrapper.Instance.Close();
            }
        }

        /// <summary>
        /// Get all models of a given pathway.
        /// </summary>
        /// <param name="id">Id of the pathway that is going to be searched for models.</param>
        /// <returns>Array of models in the given pathway, empty array is returned if there are no models.</returns>
        public static ServerModel[] GetModelsFromPathwayId(Guid id)
        {
            string query = @"SELECT * FROM dbo.MapModelsPathways MMP, dbo.Model M
                WHERE MMP.modelId = M.Id AND MMP.pathwayId = @pathwayId";

            SqlCommand command;
            command = DBWrapper.BuildCommand(query, "@pathwayId", SqlDbType.UniqueIdentifier, id);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModel(new DBRow(d)));
            }
            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }

        /// <summary>
        /// Get organism groups of the model.
        /// </summary>
        /// <returns>Array of organism group, if there is not an organism group for the model.</returns>
        public ServerOrganismGroup[] GetOrganismGroups()
        {
            string query = @"SELECT OG.* FROM dbo.Organism_Groups OG, dbo.ModelOrganism MO
                WHERE OG.id = MO.organismGroupId AND MO.modelId = @modelId";

            SqlCommand command;
            command = DBWrapper.BuildCommand(query, "@modelId", SqlDbType.UniqueIdentifier, ID);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerOrganismGroup(new DBRow(d)));
            }
            return (ServerOrganismGroup[])results.ToArray(typeof(ServerOrganismGroup));
        }

        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();

            __ModelRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, sbmlId, name, sbmlLevel, sbmlVersion, dataSourceId, sbmlFile, sbmlFileName) VALUES (@id, @sbmlId, @name, @sbmlLevel, @sbmlVersion, @dataSourceId, @sbmlFile, @sbmlFileName);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@sbmlLevel", SqlDbType.TinyInt, SbmlLevel,
                "@sbmlVersion", SqlDbType.TinyInt, SbmlVersion,
                "@dataSourceId", SqlDbType.SmallInt, DataSourceId,
                "@sbmlFile", SqlDbType.NText, SbmlFile,
                "@sbmlFileName", SqlDbType.VarChar, SbmlFileName
                );

            __ModelRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __ModelRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET sbmlId = @sbmlId , name = @name , sbmlLevel = @sbmlLevel , sbmlVersion = @sbmlVersion , dataSourceId = @dataSourceId , sbmlFile = @sbmlFile , sbmlFileName = @sbmlFileName where id = @id ;",
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@sbmlLevel", SqlDbType.TinyInt, SbmlLevel,
                "@sbmlVersion", SqlDbType.TinyInt, SbmlVersion,
                "@dataSourceId", SqlDbType.SmallInt, DataSourceId,
                "@sbmlFile", SqlDbType.NText, SbmlFile,
                "@sbmlFileName", SqlDbType.VarChar, SbmlFileName,
                "@id", SqlDbType.UniqueIdentifier, ID
                );

            __ModelRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);
        }

        #endregion


        #region Static Methods
        /// <summary>
        /// Return all Models from the system.
        /// </summary>
        /// <returns>
        /// Array of SoapModel objects ready to be sent via SOAP.
        /// </returns>
        public static ServerModel[] AllModels()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModel(new DBRow(d)));
            }

            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }

        //public static ServerModel[] GetAllModels()
        //{
        //    SqlCommand command = DBWrapper.BuildCommand("Select * from " + __TableName + " order by name");
        //    DataSet[] ds = new DataSet[0];
        //    DBWrapper.LoadMultiple(out ds, ref command);

        //    ArrayList results = new ArrayList();
        //    foreach (DataSet d in ds)
        //    {
        //        results.Add(new ServerModel(new DBRow(d)));
        //    }

        //    return (ServerModel[])results.ToArray(typeof(ServerModel));
        //}

        /// <summary>
        /// Returns a single ServerModel object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerModel Load(Guid id)
        {
            return new ServerModel(LoadRow(id));
        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(Guid id)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                 "@id", SqlDbType.UniqueIdentifier, id);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }

        public static ServerModel LoadFromBaseRow(DBRow sbaseRow)
        {
            return new ServerModel(LoadRow(sbaseRow.GetGuid("id")), sbaseRow);
        }



        public static bool Exists(Guid id)
        {
            //BE: check if this is a graph node ID instead
            id = GraphNodeManager.GetEntityId(id); // if not a graph node ID, returns same ID

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, id);

            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
                return false;
            return true;
        }

        /// <summary>
        /// Count of the number of Models that would be found with the supplied search parameters.
        /// </summary>
        /// <param name="substring">
        /// The substring we're searching for.
        /// </param>
        /// <param name="searchMethod">
        /// The search method.
        /// </param>
        /// <returns>
        /// Count of found models.
        /// </returns>
        public static int CountFindModels(string substring, SearchMethod searchMethod)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT COUNT(*) FROM " + __TableName + " WHERE [name] " +
                    (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring;",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }
        /// <summary>
        /// A search function for paging
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="searchMethod"></param>
        /// <param name="startRecord"></param>
        /// <param name="maxRecords"></param>
        /// <returns></returns>
        public static ServerModel[] FindModels(string substring, SearchMethod searchMethod, int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT * FROM ( SELECT TOP " + maxRecords.ToString() + @" *
									FROM ( SELECT TOP " + bigNum.ToString() + @" *
											FROM " + __TableName + @"
											WHERE [name] " + (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + @" @substring
											ORDER BY [name] ) " + __TableName + @"
									ORDER BY [name] DESC ) " + __TableName + @"
				ORDER BY [name]",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModel(new DBRow(d)));
            }

            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }

        /// <summary>
        /// A search function for SBML Parser
        /// </summary>
        /// <param name="sbmlFileName"></param>
        /// <returns></returns>
        public static ServerModel[] FindModelsByFileName(string sbmlFileName)
        {
            string queryString = @" select *
                                    from  " + __TableName + " WHERE sbmlFileName = @sbmlFileName";

            System.Data.SqlClient.SqlCommand command = DBWrapper.BuildCommand(queryString, "@sbmlFileName", System.Data.SqlDbType.VarChar, sbmlFileName);

            DataSet[] dsArray = new DataSet[0];
            DBWrapper.LoadMultiple(out dsArray, ref command);
            ArrayList results = new ArrayList();
            ServerModel[] existingModels = new ServerModel[dsArray.Length]; // If the model is not in the database null will return
            int index = 0;
            foreach (DataSet ds in dsArray)
            {
                existingModels[index] = new ServerModel(new DBRow(ds));
                index++;
            }
            return existingModels;
        }

        public static string getQualifier(ServerModel sm, ServerPathway sp)
        {

            string queryString = @" select r.*
                                    from  mapmodelspathways r
                                    where	r.modelId = @modelid
                                    and r.pathwayId = '" + sp.ID.ToString() + "'";

            System.Data.SqlClient.SqlCommand command = DBWrapper.BuildCommand(queryString, "@modelid", System.Data.SqlDbType.UniqueIdentifier, sm.ID);
            DataSet[] dsArray = new DataSet[0];
            DBWrapper.LoadMultiple(out dsArray, ref command);
            ArrayList results = new ArrayList();
            ServerMapModelsPathways smp;
            string qualifier = string.Empty;
            // this does not make sense 
            foreach (DataSet ds in dsArray)
            {
                smp = new ServerMapModelsPathways(new DBRow(ds));
                qualifier = AnnotationQualifierManager.GetQualifierName(smp.QualifierId);
            }


            return qualifier;
        }
        #endregion

    }// End class



} // End namespace
