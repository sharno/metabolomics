#region Using Statements
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using PathwaysLib.SoapObjects;
using PathwaysLib.Exceptions;
using PathwaysLib.Utilities;
#endregion

namespace PathwaysLib.ServerObjects
{
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Server/ServerGOTerm.cs</filepath>
	///		<creation>2005/02/14</creation>
	///		<author>
	///			<name>Marc R. Reynolds</name>
	///			<initials>mrr</initials>
	///			<email>marc.reynolds@cwru.edu</email>
	///		</author>
	///		<cvs>
	///			<cvs_author>$Author: murat $</cvs_author>
	///			<cvs_date>$Date: 2010/11/19 21:13:29 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerGOTerm.cs,v 1.8 2010/11/19 21:13:29 murat Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.8 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Encapsulates database access to GOTerms and the GOHierarchy
	/// </summary>
	#endregion
	public class ServerGOTerm : ServerObject
	{
		#region Constructors

		private ServerGOTerm(){}

		/// <summary>
		/// Constructor for ServerGOTerm with fields initialized
		/// </summary>
		/// <param name="id">7-digit, leading zero'd GOID of the term</param>
		/// <param name="name">The name of the GOTerm</param>
		/// <param name="totalDescendants">The total number of descendants in this term's subtree</param>
		/// <param name="maximumSubtreeHeight">The maximum depth of this term's subtree</param>
		public ServerGOTerm(string id, string name, int totalDescendants, int maximumSubtreeHeight)
		{
			__DBRow = new DBRow(__TableName);
			this.ID = id;
			this.Name = name;
			this.TotalDescendants = totalDescendants;
			this.MaximumSubtreeHeight = maximumSubtreeHeight;
		}

		/// <summary>
		/// Constructor for server GOTerm wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerGOTerm object from a
		/// SoapGOTerm object.
		/// </remarks>
		/// <param name="data">
		/// A SoapGOTerm object from which to construct the
		/// ServerGOTerm object.
		/// </param>
		public ServerGOTerm(SoapGOTerm data)
		{
			switch(data.Status)
			{
				case ObjectStatus.Insert:
					//create a new row
					__DBRow = new DBRow(__TableName);
					break;
				case ObjectStatus.ReadOnly:
				case ObjectStatus.Update:
				case ObjectStatus.NoChanges:
					//load in the data first
					__DBRow = LoadRow(data.ID);
					break;
				default:
					throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
			}
			// get potential updates from Soap object, unless it's supposed to be read only
			if (data.Status != ObjectStatus.ReadOnly)
				UpdateFromSoap(data);
		}

		/// <summary>
		/// Constructor for ServerGOTerm wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerGOTerm object from a
		/// DataSet.
		/// </remarks>
		/// <param name="data">
		/// DataSet to load into the object.
		/// </param>
		public ServerGOTerm( DBRow data )
		{
			// setup object
			__DBRow = data;
		}

		#endregion

		#region Member Variables

		/// <summary>
		/// True if the term is a leaf term in the GO hierarchy, False if not
		/// Null if it hasn't been set by the Property yet
		/// </summary>
		private Tribool _isLeafTerm = Tribool.Null;
		
		private int _cachedMaximumLevel = int.MinValue;

		#region Static Member Variables
		private static readonly string __TableName = "go_terms";
		private static readonly string __HierarchyTableName = "go_terms_hierarchy";
        private static readonly string __view = "View_go_terms_nodecodes";
		/// <summary>
		/// The id of the root GO term.  This is used in methods like LoadRootTerm()
		/// </summary>
		private static readonly string __RootTermID = "all";
		/// <summary>
		/// The name of the table containing the 'GO--EC Number' relation 
		/// </summary>
		public static readonly string __GO_EC_AnnotationTable = "ec_go";
		#endregion

		#endregion

		#region Properties

		/// <summary>
		/// get/set the 7-digit ID with leading zeros
		/// </summary>
		public string ID
		{
			get{return __DBRow.GetString("ID");}
			set{__DBRow.SetString("ID", value);}
		}

		/// <summary>
		/// get/set the string Name of the GO Term
		/// </summary>
		public string Name
		{
			get{return __DBRow.GetString("Name");}
			set{__DBRow.SetString("Name", value);}
		}

		/// <summary>
		/// get/set the total number of GO terms in this term's subtree
		/// </summary>
		public int TotalDescendants
		{
			get{return __DBRow.GetInt("TotalDescendants");}
			set{__DBRow.SetInt("TotalDescendants", value);}
		}

		/// <summary>
		/// get/set the maximum depth of this term's subtree
		/// </summary>
		public int MaximumSubtreeHeight
		{
			get{return __DBRow.GetInt("SubtreeHeight");}
			set{__DBRow.SetInt("SubtreeHeight", value);}
		}

		/// <summary>
		/// Returns true if this term has no children
		/// </summary>
		public bool IsLeafTerm
		{
			get
			{
				return TotalDescendants == 0;
			}
		}

		/// <summary>
		/// The number representing the maximum level of the GO hierarchy
		/// </summary>
		public static int MaxHierarchyLevel
		{
			get{return int.MaxValue;}
		}

		#endregion

		#region Methods

        /// <summary>
        /// Once go_terms table is repopulated, their node codes also need to be recomputed.
        /// </summary>
        public static void ComputeNodeCodes()
        {            
            ServerGONodeCode.Instance.PopulateNodeCodes("go_terms_hierarchy", "childId", "parentId", true);
        }

		/// <summary>
		/// Creates new ServerGOTerm objects for each GO Term 
		/// which acts as a parent to this term
		/// </summary>
		/// <returns>The parent ServerGOTerm(s)</returns>
		public ServerGOTerm[] LoadParentGOTerms()
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT terms.*
					FROM ( SELECT *
							FROM " + __HierarchyTableName + @"
							WHERE ChildID = @id) hierarchy
					INNER JOIN go_terms terms ON hierarchy.ParentID = terms.ID ",
				"@id", SqlDbType.VarChar, ID);

			DataSet[] dataSets;
			DBWrapper.LoadMultiple(out dataSets, ref command);

			ServerGOTerm[] ar = new ServerGOTerm[dataSets.Length];
			for(int i=0; i<ar.Length; i++)
				ar[i] = new ServerGOTerm(new DBRow(dataSets[i]));
			return ar;
		}

		/// <summary>
		/// Creates new ServerGOTerm objects for each GO Term 
		/// which acts as a child to this term
		/// </summary>
		/// <returns>The child ServerGOTerm(s)</returns>
		public ServerGOTerm[] LoadChildGOTerms()
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT terms.*
					FROM ( SELECT *
							FROM " + __HierarchyTableName + @"
							WHERE ParentID = @id) hierarchy
				INNER JOIN go_terms terms ON hierarchy.ChildID = terms.ID",
				"@id", SqlDbType.VarChar, ID);

			DataSet[] dataSets;
			DBWrapper.LoadMultiple(out dataSets, ref command);

			ServerGOTerm[] ar = new ServerGOTerm[dataSets.Length];
			for(int i=0; i<ar.Length; i++)
				ar[i] = new ServerGOTerm(new DBRow(dataSets[i]));

			//may as well set this variable now
			if(_isLeafTerm == Tribool.Null)
				_isLeafTerm = ar.Length == 0 ? Tribool.True : Tribool.False;

			return ar;
		}

        /// <summary>
        /// Returns set of child GO terms which (either directly or through an descendant) is mapped to a [model, compartment, or a reaction]
        /// </summary>
        /// <returns>The child ServerGOTerm(s) with a model</returns>
        public ServerGOTerm[] LoadChildGOTermsWithMapping(string type)
        {                      
            //if (ServerGONodeCode.Instance.IsEmpty())
            //    ServerGONodeCode.Instance.ComputeNodeCodes();

            /*
            string[] nodeCodes = ServerNodeCode.GetNodeCodes(ID);
            string likeStatement = "(";
            foreach (string nc in nodeCodes)
            {
                likeStatement += @"gnc.nodeCode like '" + nc + @"%' OR ";
            }
            likeStatement = likeStatement.Substring(0, likeStatement.Length - 4);
            likeStatement += ")";
            */

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT terms.*
					FROM ( SELECT *
							FROM " + __HierarchyTableName + @"
							WHERE ParentID = @id) hierarchy
				INNER JOIN go_terms terms ON hierarchy.ChildID = terms.ID
                WHERE EXISTS(SELECT m.id FROM MapSBaseGO ms, " + type + @" m, GONodeCodes gnc1, GONodeCodes gnc2 
                              WHERE ms.sbaseId = m.id 
                                AND ms.goId = gnc1.goId
                                AND gnc2.goid = hierarchy.ChildID
                                AND gnc1.nodeCode like gnc2.nodeCode + '%')",
                "@id", SqlDbType.VarChar, ID );


            DataSet[] dataSets;
            DBWrapper.LoadMultiple(out dataSets, ref command);

            ServerGOTerm[] ar = new ServerGOTerm[dataSets.Length];            
            for (int i = 0; i < ar.Length; i++)
            {
                ar[i] = new ServerGOTerm(new DBRow(dataSets[i]));
            }
            
            return ar;
        }
               /// <summary>
        /// Returns set of child GO terms which (either directly or through an descendant) is mapped to a [model, compartment, or a reaction]
        /// </summary>
        /// <returns>The child ServerGOTerm(s) with a model</returns>
        public ServerGOTerm[] LoadChildGOTermsWithMapping(string type, string substring , SearchMethod searchMethod)
        {                      
         
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";




            SqlCommand command;
            if (type.Contains("GO"))
            command = DBWrapper.BuildCommand(
               @" select distinct vgt3.id, vgt3.name,vgt3.SubtreeHeight,vgt3.TotalDescendants
                    from  " + __view + @" vgt3,
	                 " + __HierarchyTableName + @" gh,
                    (select distinct vgt1.* from " + __view + @" vgt1, 	
		                MapSbasego ms,    
		                " + type.Remove(0,2) + @" m 
                        where vgt1.name " +
					( searchMethod != SearchMethod.ExactMatch ? " LIKE " : " = " ) + " @substring" + @"  
		                AND vgt1.id = ms.goid
		                and ms.sbaseid = m.id)selectedgoterms
		                where gh.parentid = @id " +
		                @" and gh.childid = vgt3.id
		                and selectedgoterms.nodecode LIKE vgt3.nodecode+ '%'",
                "@id", SqlDbType.VarChar,ID,"@substring", SqlDbType.VarChar, substring);       
            else                       
             command = DBWrapper.BuildCommand(
                @"SELECT terms.*
					FROM ( SELECT *
							FROM " + __HierarchyTableName + @"
							WHERE ParentID = @id) hierarchy
				INNER JOIN go_terms terms ON hierarchy.ChildID = terms.ID
                WHERE EXISTS(SELECT m.id FROM MapSBaseGO ms, " + type + @" m, GONodeCodes gnc1, GONodeCodes gnc2 
                              WHERE ms.sbaseId = m.id 
                                AND m.name " +
                               (searchMethod != SearchMethod.ExactMatch ? " LIKE " : " = ") + "@substring" + @"
                                AND ms.goId = gnc1.goId
                                AND gnc2.goid = hierarchy.ChildID
                                AND gnc1.nodeCode like gnc2.nodeCode + '%')",
                "@id", SqlDbType.VarChar, ID, "@substring", SqlDbType.VarChar, substring);


            DataSet[] dataSets;
            DBWrapper.LoadMultiple(out dataSets, ref command);

            ServerGOTerm[] ar = new ServerGOTerm[dataSets.Length];            
            for (int i = 0; i < ar.Length; i++)
            {
                ar[i] = new ServerGOTerm(new DBRow(dataSets[i]));
            }
            
            return ar;
        }
        
        /// <summary>
        /// Return all entity type annotations for this organism.
        /// </summary>
        /// <returns></returns>

        public Dictionary<Guid, BiomodelAnnotation> GetAllMappingAnnotations(string type)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT sbaseId, qualifierId
				FROM MapSBaseGO ms, " + type + @" m 
				WHERE m.id = ms.sbaseId
                    AND goId = @id;",
                "@id", SqlDbType.VarChar, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            if (ds.Length == 0)
                return null;

            Dictionary<Guid, BiomodelAnnotation> anns = new Dictionary<Guid, BiomodelAnnotation>();
            BiomodelAnnotation ann = null;
            foreach (DataSet d in ds)
            {
                DataRow row = d.Tables[0].Rows[0];
                Guid sbaseId = new Guid(row["sbaseId"].ToString());
                if (anns.ContainsKey(sbaseId))
                    ann = anns[sbaseId];
                else
                {
                    ann = new BiomodelAnnotation(sbaseId);
                    anns.Add(ann.EntityId, ann);
                }
                ann.QualifierIds.AddFirst(int.Parse(row["qualifierId"].ToString()));
            }

            return anns;
        }

        /// <summary>
        /// Return all entity type annotations for this organism based on name search.
        /// </summary>
        /// <returns></returns>

        public Dictionary<Guid, BiomodelAnnotation> GetAllMappingAnnotations(string type,string substring , SearchMethod searchMethod)
        {                  
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT sbaseId, qualifierId
				FROM MapSBaseGO ms, " + type + @" m 
				WHERE m.id = ms.sbaseId
                    AND m.name " +
                    (searchMethod != SearchMethod.ExactMatch ? " LIKE " : " = ") + "@substring" + @"
                    AND goId = @id;",
                "@id", SqlDbType.VarChar, this.ID,"@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            if (ds.Length == 0)
                return null;

            Dictionary<Guid, BiomodelAnnotation> anns = new Dictionary<Guid, BiomodelAnnotation>();
            BiomodelAnnotation ann = null;
            foreach (DataSet d in ds)
            {
                DataRow row = d.Tables[0].Rows[0];
                Guid sbaseId = new Guid(row["sbaseId"].ToString());
                if (anns.ContainsKey(sbaseId))
                    ann = anns[sbaseId];
                else
                {
                    ann = new BiomodelAnnotation(sbaseId);
                    anns.Add(ann.EntityId, ann);
                }
                ann.QualifierIds.AddFirst(int.Parse(row["qualifierId"].ToString()));
            }

            return anns;
        }


        /// <summary>
        /// Returns the number of [models, compartments, or reactions] associated directly with a go term or with one its descendants
        /// </summary>
        /// <returns></returns>
        public int GetMappedEntityCount(string type)
        {
            //if (ServerGONodeCode.Instance.IsEmpty())
            //    ServerGONodeCode.Instance.ComputeNodeCodes();

            
            string[] nodeCodes = ServerGONodeCode.Instance.GetNodeCodes(ID);
            string likeStatement = "(";
            foreach (string nc in nodeCodes)            
                likeStatement += @"gnc.nodeCode like '" + nc + @"%' OR ";
                
            likeStatement = likeStatement.Substring(0, likeStatement.Length - 4);
            likeStatement += ")";
            
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT count(id) from " + type +
                @" WHERE id IN(SELECT m.id FROM MapSBaseGO ms, " + type + @" m, GONodeCodes gnc 
                              WHERE ms.sbaseId = m.id 
                                AND ms.goId = gnc.goId                                
                                AND " + likeStatement + ")");
            
            DataSet dataSet;
            DBWrapper.Instance.ExecuteQuery(out dataSet, ref command);
            int count = int.Parse(dataSet.Tables[0].Rows[0][0].ToString());
            
            return count;
        }



        /// <summary>
        /// Returns the number of [models, compartments, or reactions] associated directly with a go term or with one its descendants
        /// </summary>
        /// <returns></returns>
        public int GetMappedEntityCount(string type, string substring , SearchMethod searchMethod)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";


            string[] nodeCodes = ServerGONodeCode.Instance.GetNodeCodes(ID);
            string likeStatement = "(";
            foreach (string nc in nodeCodes)
                likeStatement += @"gnc.nodeCode like '" + nc + @"%' OR ";

            likeStatement = likeStatement.Substring(0, likeStatement.Length - 4);
            likeStatement += ")";

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT count(id) from " + type +
                @" WHERE id IN(SELECT m.id FROM MapSBaseGO ms, " + type + @" m, GONodeCodes gnc 
                              WHERE ms.sbaseId = m.id  
                               AND m.name "+
                    (searchMethod != SearchMethod.ExactMatch ? " LIKE " : " = ") + "@substring" + @"
                                AND ms.goId = gnc.goId                                
                                AND " + likeStatement + ")","@substring", SqlDbType.VarChar, substring);



            DataSet dataSet;
            DBWrapper.Instance.ExecuteQuery(out dataSet, ref command);
            int count = int.Parse(dataSet.Tables[0].Rows[0][0].ToString());

            return count;
        }
        /// <summary>
        /// Returns the number of models associated directly with a go term or with one its descendants
        /// </summary>
        /// <returns></returns>
        public string GetModelCountQuery()
        {
            string query = "EmptyQ";
            try
            {
                //ServerNodeCode.SetSchemaParameters("GONodeCodes", "goId", "nodeCode");
                //if (ServerNodeCode.IsEmpty())
                //    ServerGOTerm.ComputeNodeCodes();


                string[] nodeCodes = ServerGONodeCode.Instance.GetNodeCodes(ID);
                string likeStatement = "(";
                foreach (string nc in nodeCodes)
                {
                    likeStatement += "gnc.nodeCode like '" + nc + "%' OR ";
                    //likeStatement += String.Format("gnc.nodeCode like '@nc%' OR ", nc);
                }
                likeStatement = likeStatement.Substring(0, likeStatement.Length - 4);
                likeStatement += ")";

                //return 99;


                query = @"SELECT count(id) from Model
                  WHERE id IN (SELECT m.id FROM MapSBaseGO ms, Model m, GONodeCodes gnc 
                              WHERE ms.sbaseId = m.id 
                                AND ms.goId = gnc.goId                                
                                AND " + likeStatement + ")";

                DataSet dataSet;
                SqlCommand command = DBWrapper.BuildCommand(query);
                DBWrapper.Instance.ExecuteQuery(out dataSet, ref command);                
                int count = int.Parse(dataSet.Tables[0].Rows[0][0].ToString());

                return query;

            }
            catch (Exception e)
            {
                return e.Message + "---" + query + " ** ";
            }
           
        }


		/// <summary>
		/// Get all EC Numbers this GO term annotates
		/// </summary>
		/// <returns>The ServerECNumbers this term annotates</returns>
		public ServerECNumber[] GetAnnotatedECNumbers()
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __GO_EC_AnnotationTable + " where go_id = @go;",
				"@go", SqlDbType.VarChar, this.ID);

			DataSet[] dsets;
			DBWrapper.LoadMultiple(out dsets, ref command);

			ServerECNumber[] ecNumbers = new ServerECNumber[dsets.Length];
			//now load each ServerECNumber
			for(int i=0; i<dsets.Length; i++)
				ecNumbers[i] = new ServerECNumber(new DBRow(dsets[i]));

			return ecNumbers;
		}

		/// <summary>
		/// Returns a collection of pathways annotated by GO term and the number of times
		/// the GO term annotates that pathway
		/// </summary>
		/// <param name="pathways">An array of pathways</param>
		/// <param name="annotationCounts">An array of counts which matches the array of pathways</param>
		public void GetAnnotatedPathways(out Guid[] pathways, out int[] annotationCounts)
		{
			ArrayList goTermProcesses = new ArrayList();
			if(IsLeafTerm)
				goTermProcesses.Add(ServerGOTermProcess.LoadFromGOTerm(this));
			else
			{
				ArrayList listTouchedECNumbers = new ArrayList();
				ServerGOTerm[] leafTerms = GetLeafLevelDescendants();
				foreach(ServerGOTerm leaf in leafTerms)
				{
					ServerGOTermProcess goTermProcess = ServerGOTermProcess.LoadFromGOTerm(leaf);
					if(goTermProcess.GenericProcesses.Length > 0)
						goTermProcesses.Add(goTermProcess);
				}
			}

			//key: ServerPathway
			//value: count for participations by the **original** term
			Hashtable hashPathways = new Hashtable();

			//now find all pathways with these EC Numbers
			foreach(ServerGOTermProcess goTermProcess in goTermProcesses)
			{
				foreach(ServerGenericProcess genProcess in goTermProcess.GenericProcesses)
				{
					//get uniquely annotated pathways from this generic process
					foreach(ServerProcess process in genProcess.GetAllProcesses())
						foreach(ServerPathway pathway in process.GetAllPathways())
							//if(!listPathwayIDs.Contains(pathway.ID))
							//{
							if(hashPathways.ContainsKey(pathway.ID))
								hashPathways[pathway.ID] = (int)hashPathways[pathway.ID] + 1;
							else
								hashPathways.Add(pathway.ID, 1);
				}	//end loop over generic processes annotated by the term
			}	//end loop over annotating terms

			//assemble collection of pathways
			pathways = new Guid[hashPathways.Keys.Count];
			hashPathways.Keys.CopyTo(pathways, 0);
			//			pathways = new ServerPathway[hashPathways.Keys.Count];
			//			hashPathways.Keys.CopyTo(pathways, 0);

			//assemble collection of annotation counts
			annotationCounts = new int[hashPathways.Values.Count];
			for(int i=0; i<pathways.Length; i++)
				annotationCounts[i] = (int)hashPathways[pathways[i]];
		}

		/// <summary>
		/// Returns the maximum (most-general) level at which this term exists
		/// </summary>
		/// <returns>The number of the level</returns>
		public int GetMaximumTermLevel()
		{
			if(_cachedMaximumLevel != int.MinValue)
				return _cachedMaximumLevel;

			if(this.ID == "all") return 0;

			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT max(TermLevel) FROM go_terms_hierarchy " +
				"WHERE ChildID=@child",
				"@child", SqlDbType.VarChar, this.ID);

			return (int)DBWrapper.Instance.ExecuteScalar(ref command);
		}

		/// <summary>
		/// Loads all ServerGOTerms existing at the given level, sorted by Name
		/// </summary>
		/// <param name="hierarchyLevel">The level of the GO hierachy to query</param>
		/// <param name="includeLeavesAboveLevel">True if leaves which exist above the given level
		/// should be included in the return list</param>
		/// <returns>The list of ServerGOTerms</returns>
		public static ServerGOTerm[] Load(int hierarchyLevel, bool includeLeavesAboveLevel)
		{
			if(hierarchyLevel == int.MaxValue)
				return LoadLeafTerms();
			DataSet[] dsets;
			SqlCommand com = DBWrapper.BuildCommand(
				@"select distinct t2.* from " + __HierarchyTableName + @" t1
					inner join
					" + __TableName + @" t2
					on t1.childID =t2.id
					where t1.termlevel=@level " + (includeLeavesAboveLevel ? "or (t1.termlevel<@level and t2.SubtreeHeight=0)" : ""),
				"@level", SqlDbType.Int, hierarchyLevel);

			int count = DBWrapper.LoadMultiple(out dsets, ref com);
			ArrayList listTerms = new ArrayList(count);
			for(int i=0; i<count; i++)
				listTerms.Add(new ServerGOTerm(new DBRow(dsets[i])));
			listTerms.Sort(new CompareGOName());
			return (ServerGOTerm[])listTerms.ToArray(typeof(ServerGOTerm));
		}
    

        /// <summary>
        /// Loads ServerGOTerms which are directly or through a descendant associated with a model, compartment, or reaction
        /// </summary>
        /// <param name="hierarchyLevel">The level of the GO hierachy to query</param>
        /// <param name="includeLeavesAboveLevel">True if leaves which exist above the given level
        /// should be included in the return list</param>
        /// <returns>The list of ServerGOTerms</returns>
        public static ServerGOTerm[] LoadGOTermsWithMapping(int hierarchyLevel, bool includeLeavesAboveLevel, string type)
        {
            //if (hierarchyLevel == int.MaxValue)
            //    return LoadLeafTerms();
            DataSet[] dsets;

            SqlCommand com = DBWrapper.BuildCommand(
                @"select distinct t2.* from " + __HierarchyTableName + @" t1
					inner join
					" + __TableName + @" t2
					on t1.childID =t2.id
					where (t1.termlevel=@level " + (includeLeavesAboveLevel ? "or (t1.termlevel<@level and t2.SubtreeHeight=0)" : "") +
                    @") AND EXISTS(SELECT m.id FROM MapSBaseGO ms, " + type + @" m, GONodeCodes gnc1, GONodeCodes gnc2 
                                  WHERE ms.sbaseId = m.id 
                                    AND ms.goId = gnc1.goId
                                    AND gnc2.goid = t2.id
                                    AND gnc1.nodeCode like gnc2.nodeCode + '%')",
                "@level", SqlDbType.Int, hierarchyLevel);

            int count = DBWrapper.LoadMultiple(out dsets, ref com);
            ArrayList listTerms = new ArrayList(count);
            for (int i = 0; i < count; i++)
                listTerms.Add(new ServerGOTerm(new DBRow(dsets[i])));
            listTerms.Sort(new CompareGOName());
            return (ServerGOTerm[])listTerms.ToArray(typeof(ServerGOTerm));
        }
        /// <summary>
        /// Loads ServerGOTerms which are directly or through a descendant associated with a model, compartment, or reaction
        /// </summary>
        /// <param name="hierarchyLevel">The level of the GO hierachy to query</param>
        /// <param name="includeLeavesAboveLevel">True if leaves which exist above the given level</param>
        /// <param name="type"> string "model" "compartment" or "reaction"</param>
        /// <param name="substring"> search term</param>
        /// <param name="searchMethod"> SearchMethod enum type</param>
        /// should be included in the return list</param>
        /// <returns>The list of ServerGOTerms</returns>
        public static ServerGOTerm[] LoadGOTermsWithMapping(int hierarchyLevel, bool includeLeavesAboveLevel, string type, string substring, SearchMethod searchMethod)
        {
            //if (hierarchyLevel == int.MaxValue)
            //    return LoadLeafTerms();
            DataSet[] dsets;

            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand com;

            if (type.Contains("GO"))
            com = DBWrapper.BuildCommand(
               @" select distinct vgt3.id, vgt3.name,vgt3.SubtreeHeight,vgt3.TotalDescendants
                    from  " + __view + @" vgt3,
	                 " + __HierarchyTableName + @" gh,
                    (select distinct vgt1.* from " + __view + @" vgt1, 	
		                MapSbasego ms,    
		                " + type.Remove(0,2) + @" m 
                        where vgt1.name " +
					( searchMethod != SearchMethod.ExactMatch ? " LIKE " : " = " ) + " @substring" + @"  
		                AND vgt1.id = ms.goid
		                and ms.sbaseid = m.id)selectedgoterms
		                where (gh.termlevel = @level " +  (includeLeavesAboveLevel ? "or (gh.termlevel<@level and vgt3.SubtreeHeight=0)" : "") +
		                @" ) and gh.childid = vgt3.id
		                and selectedgoterms.nodecode LIKE vgt3.nodecode+ '%'",
                "@level", SqlDbType.Int, hierarchyLevel, "@substring", SqlDbType.VarChar, substring);       
            else 
            com = DBWrapper.BuildCommand(
                @"select distinct t2.* from " + __HierarchyTableName + @" t1
					inner join
					" + __TableName + @" t2
					on t1.childID =t2.id
					where (t1.termlevel=@level " + (includeLeavesAboveLevel ? "or (t1.termlevel<@level and t2.SubtreeHeight=0)" : "") +
                    @") AND EXISTS(SELECT m.id FROM MapSBaseGO ms, " + type + @" m, GONodeCodes gnc1, GONodeCodes gnc2 
                                  WHERE ms.sbaseId = m.id 
                                    AND m.name " +
					( searchMethod != SearchMethod.ExactMatch ? " LIKE " : " = " ) + " @substring" + @"
                                    AND ms.goId = gnc1.goId
                                    AND gnc2.goid = t2.id
                                    AND gnc1.nodeCode like gnc2.nodeCode + '%')",
                "@level", SqlDbType.Int, hierarchyLevel, "@substring", SqlDbType.VarChar,substring);

            int count = DBWrapper.LoadMultiple(out dsets, ref com);
            ArrayList listTerms = new ArrayList(count);
            for (int i = 0; i < count; i++)
                listTerms.Add(new ServerGOTerm(new DBRow(dsets[i])));
            listTerms.Sort(new CompareGOName());
            return (ServerGOTerm[])listTerms.ToArray(typeof(ServerGOTerm));
        }

		/// <summary>
		/// Retrieves all GO terms which exist at the leaf-level of the hierarchy,
		/// sorted by Name
		/// </summary>
		/// <returns></returns>
		public static ServerGOTerm[] LoadLeafTerms()
		{
			DataSet[] dsets;
			SqlCommand com = DBWrapper.BuildCommand("select * from " + __TableName + " where subtreeHeight=0");
			int count = DBWrapper.LoadMultiple(out dsets, ref com);

			ArrayList listTerms = new ArrayList(count);
			for(int i=0; i<count; i++)
				listTerms.Add(new ServerGOTerm(new DBRow(dsets[i])));
			listTerms.Sort(new CompareGOName());
			return (ServerGOTerm[])listTerms.ToArray(typeof(ServerGOTerm));
		}

        /// <summary>
        /// Retrieves all GO terms 
        /// </summary>
        /// <returns></returns>
        public static ServerGOTerm[] AllGOTerms()
        {
            DataSet[] dsets;
            SqlCommand com = DBWrapper.BuildCommand("select * from " + __TableName);
            int count = DBWrapper.LoadMultiple(out dsets, ref com);

            ArrayList listTerms = new ArrayList(count);
            for (int i = 0; i < count; i++)
                listTerms.Add(new ServerGOTerm(new DBRow(dsets[i])));
            listTerms.Sort(new CompareGOName());
            return (ServerGOTerm[])listTerms.ToArray(typeof(ServerGOTerm));
        }

        /// <summary>
        /// Return all mapped entities of specified type for this GO term.
        /// </summary>
        /// <returns></returns>
        public ServerObject[] GetAllMappedEntitiesOfType(string type)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT *
				FROM " + type + @"
				WHERE [id] IN ( SELECT sbaseId
									FROM MapSbaseGO
									WHERE goId = @id )
				ORDER BY sbmlId;",
                "@id", SqlDbType.VarChar, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            if (type == "Model")
            {
                foreach (DataSet d in ds)
                    results.Add(new ServerModel(new DBRow(d)));
                return (ServerModel[])results.ToArray(typeof(ServerModel));
            }
            else if (type == "Compartment")
            {
                foreach (DataSet d in ds)
                    results.Add(new ServerCompartment(new DBRow(d)));
                return (ServerCompartment[])results.ToArray(typeof(ServerCompartment));
            }
            else if (type == "Reaction")
            {
                foreach (DataSet d in ds)
                    results.Add(new ServerReaction(new DBRow(d)));
                return (ServerReaction[])results.ToArray(typeof(ServerReaction));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Return all mapped entities of specified type for this GO term .
        /// </summary>
        /// <returns></returns>
        public ServerObject[] GetAllMappedEntitiesOfType(string type, string substring , SearchMethod searchMethod)
        {


            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT m.*
				FROM " + type + @" m
				WHERE [id] IN ( SELECT sbaseId
									FROM MapSbaseGO
									WHERE goId = @id )
                AND m.name " +
                            (searchMethod != SearchMethod.ExactMatch ? " LIKE " : " = ") + " @substring" + @"
				ORDER BY m.name;",
                "@id", SqlDbType.VarChar, this.ID,"@substring", SqlDbType.VarChar,substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            if (type == "Model")
            {
                foreach (DataSet d in ds)
                    results.Add(new ServerModel(new DBRow(d)));
                return (ServerModel[])results.ToArray(typeof(ServerModel));
            }
            else if (type == "Compartment")
            {
                foreach (DataSet d in ds)
                    results.Add(new ServerCompartment(new DBRow(d)));
                return (ServerCompartment[])results.ToArray(typeof(ServerCompartment));
            }
            else if (type == "Reaction")
            {
                foreach (DataSet d in ds)
                    results.Add(new ServerReaction(new DBRow(d)));
                return (ServerReaction[])results.ToArray(typeof(ServerReaction));
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Return all models for this GO term.
        /// </summary>
        /// <returns></returns>
        public ServerModel[] GetAllModels()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT *
				FROM model
				WHERE [id] IN ( SELECT sbaseId
									FROM MapSbaseGO
									WHERE goId = @id )
				ORDER BY sbmlId;",
                "@id", SqlDbType.VarChar, this.ID);

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
		/// Used to sort GO terms by name so they can be displayed nicely
		/// in a list
		/// </summary>
		internal class CompareGOName : IComparer
		{
			#region IComparer Members

			public int Compare(object x, object y)
			{
				ServerGOTerm t1 = x as ServerGOTerm;
				ServerGOTerm t2 = y as ServerGOTerm;
				if(t1 == null || t2 == null) throw new ArgumentException("CompareGOName Comparer can only compare two ServerGOTerm objects");

				return t1.Name.CompareTo(t2.Name);
			}

			#endregion

		}

		/// <summary>
		/// Used to sort GO terms by ID so BinarySearch can be performed
		/// </summary>
		internal class CompareGOID : IComparer
		{
			#region IComparer Members

			public int Compare(object x, object y)
			{
				ServerGOTerm t1 = x as ServerGOTerm;
				ServerGOTerm t2 = y as ServerGOTerm;
				if(t1 == null || t2 == null) throw new ArgumentException("CompareGOID Comparer can only compare two ServerGOTerm objects");

				int parsedID1 = int.Parse(t1.ID);
				int parsedID2 = int.Parse(t2.ID);

				return parsedID1.CompareTo(parsedID2);
			}

			#endregion

		}



		/// <summary>
		/// Performs a DFS on the GO term's subtree to find
		/// all unique, leaf-level descendant terms
		/// </summary>
		/// <returns>An array all unique, leaf-level descendant terms</returns>
		public ServerGOTerm[] GetLeafLevelDescendants()
		{
			//if we're already at the leaf level, we
			//don't need to go any further
			if(IsLeafTerm) return new ServerGOTerm[0];

			//DFS on Children
			Stack stackChildren = new Stack(new ServerGOTerm[]{this});

			//a list of GO IDs we've seen so that we don't
			//go down a branch more than once
			ArrayList listTouchedGOTerms = new ArrayList();

			//a collection of the leaf-level children we find
			ArrayList listLeafLevelDescendants = new ArrayList();

			//loop over all child branches
			while(stackChildren.Count > 0)
			{
				//we assume that anything on the stack
				//needs to be expanded

				//grab the term on the top of the stack
				ServerGOTerm term = (ServerGOTerm)stackChildren.Pop();

				//if this term is at the leaf-level, add it to the list
				//and continue on
				if(term.IsLeafTerm)
				{
					listLeafLevelDescendants.Add(term);
					continue;
				}
				
				//get the term's children and push the new
				//children on the stack
				ServerGOTerm[] children = term.LoadChildGOTerms();
				foreach(ServerGOTerm child in children)
				{
					//continue on if we've already searched this branch
					if(listTouchedGOTerms.Contains(child.ID)) continue;

					//add the new term to the list and stack
					listTouchedGOTerms.Add(child.ID);
					stackChildren.Push(child);
				}
			}

			//return the leaf-level children
			return (ServerGOTerm[])listLeafLevelDescendants.ToArray(typeof(ServerGOTerm));
		}

		/// <summary>
		/// Find the unique terms relating to the current term and at the
		/// specified level
		/// </summary>
		/// <param name="level">The level of the hierarchy to which we want to transpose</param>
		/// <returns>The set of anscestor GO terms</returns>
		public ServerGOTerm[] TransposeTermInHierarchy(int level)
		{
			int termMaxLevel = this.GetMaximumTermLevel();
			//there may be no need to transpose
			if(termMaxLevel == level || (this.IsLeafTerm && level > termMaxLevel)) return new ServerGOTerm[]{this};
			else if(level > termMaxLevel)
				//transpose down
				return TransposeTermDownHierarchy(level);
			else
				//transpose up
				return TransposeTermUpHierarchy(level);
		}

		/// <summary>
		/// Find the unique terms below the current term
		/// </summary>
		/// <param name="level">The level of the hierarchy to which we want to transpose</param>
		/// <returns>The set of anscestor GO terms</returns>
		private ServerGOTerm[] TransposeTermDownHierarchy(int level)
		{
			if(level == this.GetMaximumTermLevel())
				return new ServerGOTerm[]{this};
			if(level < this.GetMaximumTermLevel())
				throw new ArgumentException("Given level is above the current term's level", "level");
			

			//a stack to aid in DFS
			Stack stackDescendants = new Stack();

			//list of checked branches
			ArrayList listPushedBranches = new ArrayList();

			//list to hold the terms which are at the target
			//hierarchy level
			ArrayList listTermsAtLevelX = new ArrayList();

			//push the immediate children onto the stack
			foreach(ServerGOTerm child in this.LoadChildGOTerms())
			{
				stackDescendants.Push(child);
				listPushedBranches.Add(child.ID);
			}

			while(stackDescendants.Count > 0)
			{
				//we need to check these terms to make sure they're
				//at the appropriate leve
				ServerGOTerm term = (ServerGOTerm)stackDescendants.Pop();

				int maxLevel = term.GetMaximumTermLevel();
				if(maxLevel == level)
				{
					listTermsAtLevelX.Add(term);
					continue;
				}
				else if (maxLevel > level) continue;	//we're too far down

				//otherwise, we need to keep going up the tree; push this
				//term's parents onto the stack
				foreach(ServerGOTerm child in term.LoadChildGOTerms())
				{
					//continue on if we've already searched that branch
					if(listPushedBranches.Contains(child.ID)) continue;

					//add the new branch onto the stack
					stackDescendants.Push(child);
					listPushedBranches.Add(child.ID);
				}
			}

			return (ServerGOTerm[])listTermsAtLevelX.ToArray(typeof(ServerGOTerm));
		}

        /// <summary>
        /// Find the unique terms below the current term
        /// </summary>
        /// <param name="level">The level of the hierarchy to which we want to transpose</param>
        /// <returns>The set of anscestor GO terms</returns>
        private Dictionary<string, string> GetDescendantTermIds()
        {
            Dictionary<string, string> descendantIds = new Dictionary<string, string>(); 

            //a stack to aid in DFS
            Stack stackDescendants = new Stack();            

            //push the immediate children onto the stack
            foreach (ServerGOTerm child in this.LoadChildGOTerms())
            {
                stackDescendants.Push(child);
                if(!descendantIds.ContainsKey(child.ID))
                    descendantIds.Add(child.ID, "");
            }

            while (stackDescendants.Count > 0)
            {
                //we need to check these terms to make sure they're
                //at the appropriate leve
                ServerGOTerm term = (ServerGOTerm)stackDescendants.Pop();

            
                //term's parents onto the stack
                foreach (ServerGOTerm child in term.LoadChildGOTerms())
                {
                    if (descendantIds.ContainsKey(child.ID))
                        continue;

                    descendantIds.Add(child.ID, "");

                    //add the new branch onto the stack
                    stackDescendants.Push(child);                    
                }
            }

            return descendantIds;
        }

		/// <summary>
		/// Find the unique terms above the current term
		/// </summary>
		/// <param name="level">The level of the hierarchy to which we want to transpose</param>
		/// <returns>The set of anscestor GO terms</returns>
		private ServerGOTerm[] TransposeTermUpHierarchy(int level)
		{
			if(level > this.GetMaximumTermLevel())
				if(IsLeafTerm)
					return new ServerGOTerm[]{this};
				else
					throw new ArgumentException("Given level is below the current term's level", "level");

			int termMaxLevel = this.GetMaximumTermLevel();

			//a stack to aid in DFS
			Stack stackAnscestors = new Stack();

			//list of checked branches
			ArrayList listPushedBranches = new ArrayList();

			//list to hold the terms which are at the target
			//hierarchy level
			ArrayList listTermsAtLevelX = new ArrayList();

			//push the immediate parents onto the stack
			foreach(ServerGOTerm parent in this.LoadParentGOTerms())
			{
				stackAnscestors.Push(parent);
				listPushedBranches.Add(parent.ID);
			}

			while(stackAnscestors.Count > 0)
			{
				//we need to check these terms to make sure they're
				//at the appropriate leve
				ServerGOTerm term = (ServerGOTerm)stackAnscestors.Pop();

				int maxLevel = term.GetMaximumTermLevel();
				if(maxLevel == level)
				{
					listTermsAtLevelX.Add(term);
					continue;
				}
				else if (maxLevel < level) continue;	//we're too far up

				//otherwise, we need to keep going up the tree; push this
				//term's parents onto the stack
				foreach(ServerGOTerm parent in term.LoadParentGOTerms())
				{
					//continue on if we've already searched that branch
					if(listPushedBranches.Contains(parent.ID)) continue;

					//add the new branch onto the stack
					stackAnscestors.Push(parent);
					listPushedBranches.Add(parent.ID);
				}
			}

			return (ServerGOTerm[])listTermsAtLevelX.ToArray(typeof(ServerGOTerm));
		}

		/// <summary>
		/// Returns a representation of this object suitable for being
		/// sent to a client via SOAP.
		/// </summary>
		/// <returns>
		/// A SoapObject object capable of being passed via SOAP.
		/// </returns>
		public override SoapObject PrepareForSoap(SoapObject derivedObject)
		{
			SoapGOTerm retVal = derivedObject == null ? new SoapGOTerm() : (SoapGOTerm)derivedObject;

			retVal.ID = this.ID;
			retVal.Name = this.Name;
			retVal.TotalDescendants = this.TotalDescendants;
			retVal.MaximumSubtreeHeight = this.MaximumSubtreeHeight;
            
			return retVal;
		}

		/// <summary>
		/// Consumes a SoapObject object and updates the object
		/// from it.
		/// </summary>
		/// <param name="o">
		/// The SoapObject object to update from, potentially containing
		/// changes to the object.
		/// </param>
		protected override void UpdateFromSoap ( SoapObject o )
		{
			SoapGOTerm s = o as SoapGOTerm;

			this.ID = s.ID;
			this.Name = s.Name;
			this.TotalDescendants = s.TotalDescendants;
			this.MaximumSubtreeHeight = s.MaximumSubtreeHeight;
		}

		/// <summary>
		/// Returns true if the IDs of the objects match.  Send to the base
		/// Equals method if obj is not a ServerGOTerm object
		/// </summary>
		/// <param name="obj">The object to compare</param>
		/// <returns>True if the objects are equal</returns>
		public override bool Equals(object obj)
		{
			ServerGOTerm term = obj as ServerGOTerm;
			if(term == null) return base.Equals(obj);
			return this.ID == term.ID;
		}

		/// <summary>
		/// HashCode generated on the GO ID of the term
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}


		#region Static Methods
		/// <summary>
		/// Create a new ServerGOTerm from the root term of the hierarchy
		/// </summary>
		/// <returns>The root ServerGOTerm of the hierarchy</returns>
		public static ServerGOTerm LoadRootTerm()
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * from " + __TableName + " where ID = '" + __RootTermID + "';"
				);

			DataSet ds;
			DBWrapper.LoadSingle(out ds, ref command, false);
			return new ServerGOTerm(new DBRow(ds));
		}

		/// <summary>
		/// Return the dataset for a GO Term with a given ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private static DBRow LoadRow ( string id )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE id = @id;",
				"@id", SqlDbType.VarChar, id);

			DataSet ds;
			DBWrapper.LoadSingle( out ds, ref command );
			return new DBRow(ds);
		}

		/// <summary>
		/// Constructor for loading an existing GOTerm by ID
		/// </summary>
		/// <param name="id">The 7-digit ID with leading zeros</param>
		public static ServerGOTerm Load(string id)
		{
			return new ServerGOTerm( LoadRow(id) );
		}
		
		/// <summary>
		/// Get all GO Terms
		/// </summary>
		/// <returns>All GO Terms</returns>
		public static ServerGOTerm[] GetAllGOTerms()
		{
			SqlCommand command = DBWrapper.BuildCommand("SELECT * FROM " + __TableName);

			DataSet[] dsets;
			ServerGOTerm[] goTerms = new ServerGOTerm[DBWrapper.LoadMultiple(out dsets, ref command)];

			for(int i=0; i<goTerms.Length; i++)
				goTerms[i] = new ServerGOTerm(new DBRow(dsets[i]));

			return goTerms;
		}
		#endregion

		#region ADO.NET SqlCommands
		/// <summary>
		/// Required function for setting up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
			__DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
				"INSERT INTO " + __TableName + @" (id, name, subtreeHeight, totalDescendants)
				VALUES (@id, @name, @subtreeheight, @totalDescendants);",
				"@id", SqlDbType.VarChar, ID,
				"@name", SqlDbType.VarChar, Name,
				"@subtreeHeight", SqlDbType.Int, MaximumSubtreeHeight,
				"@totalDescendants", SqlDbType.Int, TotalDescendants
				);

			__DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE id = @id;",
				"@id", SqlDbType.VarChar, ID);

			__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
				"UPDATE " + __TableName + @"
				SET name = @name, subtreeHeight = @subtreeHeight, totalDescendants = @totalDescendants
				WHERE id = @id;",
				"@name", SqlDbType.VarChar, Name,
				"@subtreeHeight", SqlDbType.Int, MaximumSubtreeHeight,
				"@id", SqlDbType.VarChar, ID, 
				"@totalDescendants", SqlDbType.Int, TotalDescendants
				);

			__DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
				"DELETE FROM " + __TableName + " WHERE id = @id;",
				"@id", SqlDbType.VarChar, ID);
		}
		#endregion

		#endregion
	}
}
