#region Using Statements
using System;
using System.Collections;
#endregion

namespace PathwaysLib.ServerObjects
{
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Server/ServerGOTermProcess.cs</filepath>
	///		<creation>2005/02/16</creation>
	///		<author>
	///			<name>Marc R. Reynolds</name>
	///			<initials>mrr</initials>
	///			<email>marc.reynolds@cwru.edu</email>
	///		</author>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerGOTermProcess.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Encapsulates database access to relationships between GO Terms and GenericProcesses
	/// </summary>
	#endregion
	public class ServerGOTermProcess
	{

		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public ServerGOTermProcess()
		{
		}

		/// <summary>
		/// Constructor for ServerGOTermProcess with initialization.
		/// List of ServerGOTerm(s) is sorted by GOID
		/// </summary>
		/// <param name="terms">The GO Terms</param>
		/// <param name="genericProcesses">The processes</param>
		public ServerGOTermProcess(ServerGOTerm[] terms, ServerGenericProcess[] genericProcesses)
		{
			this._goTerms = terms;
			this._genericProcesses = genericProcesses;

			SortGOTerms();
		}

		#endregion

		#region Member Variables
		private ServerGOTerm[] _goTerms;
		private ServerGenericProcess[] _genericProcesses;
		#endregion

		#region Properties

		/// <summary>
		/// get the array of GO terms associated with the processes
		/// </summary>
		public ServerGOTerm[] GOTerms
		{
			get{return _goTerms;}
		}

		/// <summary>
		/// get the array of GenericProcesses associated with
		/// the GO term
		/// </summary>
		public ServerGenericProcess[] GenericProcesses
		{
			get{return _genericProcesses;}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Transpose all the GO Terms up the hierarchy, removing duplicates
		/// </summary>
		/// <param name="hierarchyLevel">The level of the GO hierarchy to which terms
		/// will be transposed</param>
		public void TransposeAllGOTerms(int hierarchyLevel)
		{
			ArrayList listTermIDs = new ArrayList();
			ArrayList listTerms = new ArrayList();
			for(int i=0; i<this._goTerms.Length; i++)
			{
				ServerGOTerm[] transposedTerms = this._goTerms[i].TransposeTermInHierarchy(hierarchyLevel);
				foreach(ServerGOTerm transposedTerm in transposedTerms)
				{
					if(listTermIDs.Contains(transposedTerm.ID)) continue;
					else
					{
						listTermIDs.Add(transposedTerm.ID);
						listTerms.Add(transposedTerm);
					}
				}
			}

			this._goTerms = (ServerGOTerm[])listTerms.ToArray(typeof(ServerGOTerm));
		}

		/// <summary>
		/// Method used to transpose all GO terms up to the given hierarchy level
		/// </summary>
		/// <param name="toTranspose">Contains the Terms to transpose</param>
		/// <param name="hierarchyLevel">The level to transpose the terms to</param>
		/// <returns>A transposed ServerGOTermProcess</returns>
		public static ServerGOTermProcess TransposeAllGOTerms(ServerGOTermProcess toTranspose, int hierarchyLevel)
		{
			ArrayList listTermIDs = new ArrayList();
			ArrayList listTerms = new ArrayList();
			for(int i=0; i<toTranspose._goTerms.Length; i++)
			{
				ServerGOTerm[] transposedTerms = toTranspose._goTerms[i].TransposeTermInHierarchy(hierarchyLevel);
				foreach(ServerGOTerm transposedTerm in transposedTerms)
				{
					if(listTermIDs.Contains(transposedTerm.ID)) continue;
					else
					{
						listTermIDs.Add(transposedTerm.ID);
						listTerms.Add(transposedTerm);
					}
				}
			}

			return new ServerGOTermProcess((ServerGOTerm[])listTerms.ToArray(typeof(ServerGOTerm)), toTranspose._genericProcesses);
		}

		/// <summary>
		/// Transpose all GO Terms in all ServerGOTermProcess objects in the array
		/// </summary>
		/// <param name="toTranspose">The ServerGOTermProcess objects to transpose</param>
		/// <param name="hierarchyLevel">The level of the GO hierarchy to transpose annotations</param>
		/// <param name="mergeSameAnnotationProcesses">--not used--</param>
		/// <returns>A list of transposed ServerGOTermProcesses.  Position of transposed versions is the same as the un-transposed version</returns>
		/// <remarks>!!--NOT TESTED--!!</remarks>
		public static ServerGOTermProcess[] TransposeAllGOTerms(ServerGOTermProcess[] toTranspose, int hierarchyLevel, bool mergeSameAnnotationProcesses)
		{
			ServerGOTermProcess[] listServerGOTermProcesses = new ServerGOTermProcess[toTranspose.Length];
			for(int i=0; i<toTranspose.Length; i++)
				listServerGOTermProcesses[i] = TransposeAllGOTerms(toTranspose[i], hierarchyLevel);			

			return listServerGOTermProcesses;
		}

		/// <summary>
		/// Returns true if the annotations in 'left' are the same as those in 'right'
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns>True if the annotations in 'left' are the same as those in 'right'</returns>
		private static bool HaveSameAnnotations(ServerGOTermProcess left, ServerGOTermProcess right)
		{
			if(left.GOTerms.Length != right.GOTerms.Length) return false;
			for(int i=0; i<left.GOTerms.Length; i++)
				if(left.GOTerms[i].ID != right.GOTerms[i].ID) return false;
			return true;
		}

		/// <summary>
		/// Create a new ServerGOTermProcess which contains the generic process from both 'left' and 'right'
		/// GOTerms are take arbitrarily from the 'left' ServerGOTermProcess
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns>The new ServerGOTermProcess</returns>
		private static ServerGOTermProcess CombineGenericProcesses(ServerGOTermProcess left, ServerGOTermProcess right)
		{
			ArrayList listGenericProcesses = new ArrayList(left.GenericProcesses);
			listGenericProcesses.AddRange(right.GenericProcesses);
			return new ServerGOTermProcess(left.GOTerms, (ServerGenericProcess[])listGenericProcesses.ToArray(typeof(ServerGenericProcess)));
		}

		/// <summary>
		/// Sorts the ServerGOTerms by GOID
		/// </summary>
		private void SortGOTerms()
		{
			//sort the GO terms by id
			ArrayList listTerms = new ArrayList(this._goTerms);
			listTerms.Sort(new GOIDComparer());
			this._goTerms = (ServerGOTerm[])listTerms.ToArray(typeof(ServerGOTerm));
		}


		/// <summary>
		/// Generates a hash code from a list of ServerGenericProcesses
		/// by concatenating the Guid IDs into a string and getting
		/// the hashcode of that string
		/// </summary>
		public override int GetHashCode()
		{
			string idKey = string.Empty;
			foreach(ServerGenericProcess process in this._genericProcesses)
				idKey += process.ID.ToString();
			return idKey.GetHashCode();
		}

		/// <summary>
		/// Get the maximum level of all annotating terms
		/// </summary>
		/// <returns></returns>
		public int GetMaximumAnnotationlevel()
		{
			int max = int.MinValue;
			foreach(ServerGOTerm term in _goTerms)
				if(term.GetMaximumTermLevel() > max)
					max = term.GetMaximumTermLevel();
			return max;
		}

		#region Static Methods

		/// <summary>
		/// Get the maximum level of all annotating terms in the collection
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		public static int GetMaximumAnnotationLevel(ServerGOTermProcess[] col)
		{
			int max = int.MinValue;
			foreach(ServerGOTermProcess gtp in col)
			{
				int gtpMax = gtp.GetMaximumAnnotationlevel();
				if(gtpMax > max)
					max = gtpMax;
			}
			return max;
		}

		/// <summary>
		/// Calculate the total number of GO relationships in the collection
		/// </summary>
		/// <param name="col">The collection of ServerGOTermProcess relationships</param>
		/// <returns></returns>
		public static int GetTotalNumberGOAnnotations(ServerGOTermProcess[] col)
		{
			int total = 0;
			foreach(ServerGOTermProcess gtp in col)
				total += gtp.GOTerms.Length *gtp.GenericProcesses.Length;
			return total;
		}

		/// <summary>
		/// Return a hashtable keyed on ServerGOTerms where the value is
		/// an ArrayList of ServerGenericProcesses which the term annotates.
		/// Note that concurrent annotations are split up, so there is a
		/// loss of information in this transformation.
		/// </summary>
		/// <param name="col">The collection of ServerGOTermProcesses</param>
		/// <returns>
		/// Hashtable
		/// key: ServerGOTerm
		/// value: ArrayList of ServerGenericProcesses the ServerGOTerm annotates
		/// </returns>
		public static Hashtable GetAnnotationHash(ServerGOTermProcess[] col)
		{
			//key: ServerGOTerm
			//value: ArrayList of ServerGenericProcesses the ServerGOTerm annotates
			Hashtable hashAnnotations = new Hashtable();

			foreach(ServerGOTermProcess gtp in col)
			{
				foreach(ServerGOTerm term in gtp.GOTerms)
					if(hashAnnotations[term] == null)
						hashAnnotations.Add(term, new ArrayList(gtp.GenericProcesses));
					else ((ArrayList)hashAnnotations[term]).AddRange(gtp.GenericProcesses);
			}

			return hashAnnotations;
		}

		/// <summary>
		/// Return a hashtable keyed on ServerGOTerms where the value is
		/// an ArrayList of ServerGenericProcesses which the term annotates.
		/// Note that concurrent annotations are split up, so there is a
		/// loss of information in this transformation.
		/// </summary>
		/// <param name="col">The collection of ServerGOTermProcesses</param>
		/// <param name="sortedKeyArray"></param>
		/// <returns>
		/// Hashtable
		/// key: ServerGOTerm
		///  value: ArrayList of ServerGenericProcesses the ServerGOTerm annotates
		/// </returns>
		public static Hashtable GetAnnotationHash(ServerGOTermProcess[] col, out ServerGOTerm[] sortedKeyArray)
		{
			Hashtable hashAnnotations = GetAnnotationHash(col);

			//now sort the keys
			ArrayList listTerms = new ArrayList(hashAnnotations.Keys);
			listTerms.Sort(new ServerGOTerm.CompareGOID());
			sortedKeyArray = (ServerGOTerm[])listTerms.ToArray(typeof(ServerGOTerm));

			return hashAnnotations;
		}

		/// <summary>
		/// Calculate the total number of GO relationships in the collection
		/// for a specific go term
		/// </summary>
		/// <param name="col">The collection of ServerGOTermProcess relationships</param>
		/// <param name="goid">The specific go term to check</param>
		/// <returns></returns>
		public static int GetTotalNumberGOAnnotations(ServerGOTermProcess[] col, string goid)
		{
			int total = 0;
			foreach(ServerGOTermProcess gtp in col)
				foreach(ServerGOTerm term in gtp.GOTerms)
					if(term.ID == goid)
						total += gtp.GenericProcesses.Length;
			return total;
		}

		//TODO: Make sure that this method is getting all the processes
		//from the ecnumbers... the GO->pathways isn't working because
		//we aren't getting enough processes back from the EC numbers

		/// <summary>
		/// Retrieves the GO Term->Process relationships from the given GO term
		/// </summary>
		/// <param name="term">The GO term</param>
		/// <returns>A ServerGOTermProcess object with represents the 1->Many
		/// relationship between the given GO term and the processes it annotates</returns>
		public static ServerGOTermProcess LoadFromGOTerm(ServerGOTerm term)
		{
			//Get the EC Numbers
			ServerECNumber[] ecNumbers = term.GetAnnotatedECNumbers();

			ArrayList listProcesses = new ArrayList();
			foreach(ServerECNumber ecNumber in ecNumbers)
			{
				//get all processes
				ServerCatalyze catalyze;
				try{catalyze = ecNumber.GetAllProcessGeneProducts();}
				catch(PathwaysLib.Exceptions.DataModelException)
				{
#if DEBUG
					System.Diagnostics.Debug.WriteLine(ecNumber.ECNumber);
#endif
					continue;
				}
				if(catalyze == null)	//there was no catalyzing enzyme
					continue;

				ServerProcess process = ServerProcess.Load(catalyze.ProcessID);
				listProcesses.Add(process);
			}

			//get the set of GenericProcesses
			ServerGenericProcess[] genProcesses = ServerGenericProcess.LoadFromProcessSet((ServerProcess[])listProcesses.ToArray(typeof(ServerProcess)));

			return new ServerGOTermProcess(
				new ServerGOTerm[]{term},
				genProcesses);
		}

		/// <summary>
		/// Retrieves the Process->GO Term relationships from the given
		/// specific process using the Maximum level of the GO hierarchy
		/// </summary>
		/// <param name="process">The Process</param>
		/// <returns>A ServerGOTermProcess object with represents the 1->Many
		/// relationship between the given Process and the GO Terms which annotate it</returns>
		public static ServerGOTermProcess LoadFromProcess(ServerProcess process)
		{
			return LoadFromProcess(process, ServerGOTerm.MaxHierarchyLevel);
		}

		/// <summary>
		/// Retrieves the Process->GO Term relationships from the given
		/// specific process
		/// </summary>
		/// <param name="process">The Process</param>
		/// <param name="hierarchyLevel">The level of the GO hierarchy from which
		/// annotations will come</param>
		/// <returns>A ServerGOTermProcess object with represents the 1->Many
		/// relationship between the given Process and the GO Terms which annotate it</returns>
		public static ServerGOTermProcess LoadFromProcess(ServerProcess process, int hierarchyLevel)
		{
			//get the EC Numbers
			ServerECNumber[] ecNumbers = process.GetECNumbers();

			ArrayList listGOTerms = new ArrayList();
			ArrayList listGOTermIDs = new ArrayList();

			foreach(ServerECNumber ecNumber in ecNumbers)
			{
				//get the annotations
				ServerGOTerm[] goTerms = ecNumber.GetAnnotatingGOTerms();
				foreach(ServerGOTerm term in goTerms)
				{
					ServerGOTerm[] transposedTerms = hierarchyLevel == ServerGOTerm.MaxHierarchyLevel ?
						//transpose the term up the hierarchy
						term.TransposeTermInHierarchy(hierarchyLevel)
						: new ServerGOTerm[]{term};

					foreach(ServerGOTerm transposedTerm in transposedTerms)
					{
						//don't add this term if we've already seen it
						if(listGOTermIDs.Contains(transposedTerm.ID)) continue;

						//add the new term to the lists
						listGOTerms.Add(transposedTerm);
						listGOTermIDs.Add(transposedTerm.ID);
					}
				}
			}

			ServerGenericProcess[] genProcesses = ServerGenericProcess.LoadFromProcessSet(new ServerProcess[]{process});

			//transpose all terms to the given level

			return new ServerGOTermProcess(
				(ServerGOTerm[])listGOTerms.ToArray(typeof(ServerGOTerm)),
				genProcesses);
		}

		/// <summary>
		/// Retrieves the Process->GO Term relationships from the given
		/// generic process process.  GO Annotations are taken from the maximum
		/// level of the hierarhcy
		/// </summary>
		/// <param name="genericProcess">The generic process</param>
		/// <returns>A ServerGOTermProcess object with represents the 1->Many
		/// relationship between the given generic process and the GO Terms which annotate it</returns>
		public static ServerGOTermProcess LoadFromGenericProcess(ServerGenericProcess genericProcess)
		{
			return LoadFromGenericProcess(genericProcess, ServerGOTerm.MaxHierarchyLevel);
		}

		/// <summary>
		/// Retrieves the Process->GO Term relationships from the given
		/// generic process process
		/// </summary>
		/// <param name="genericProcess">The generic process</param>
		/// <param name="hierarchyLevel">The level of the GO hierarchy from which
		/// annotations will come</param>
		/// <returns>A ServerGOTermProcess object with represents the 1->Many
		/// relationship between the given generic process and the GO Terms which annotate it</returns>
		public static ServerGOTermProcess LoadFromGenericProcess(ServerGenericProcess genericProcess, int hierarchyLevel)
		{
			//get the specific processes
			ServerProcess[] processes = genericProcess.GetAllProcesses();

			ArrayList listTouchedECNumbers = new ArrayList();
			ArrayList listGOTerms = new ArrayList();
			ArrayList listGOTermIDs = new ArrayList();
			foreach(ServerProcess process in processes)
			{
				//get the ECNumbers
				ServerECNumber[] ecNumbers = process.GetECNumbers();
				
				foreach(ServerECNumber ecNumber in ecNumbers)
				{
					if(listTouchedECNumbers.Contains(ecNumber.ECNumber))
						continue;
					else
					{
						//get the GO annotations
						ServerGOTerm[] goTerms;
						if(hierarchyLevel == ServerGOTerm.MaxHierarchyLevel)
							//get the annotations from the max level
							goTerms = ecNumber.GetAnnotatingGOTerms();
						else
						{
							//transpose each annotating GO Term up the hierarchy
							ArrayList listGOECTerms = new ArrayList();
							foreach(ServerGOTerm term in ecNumber.GetAnnotatingGOTerms())
								listGOECTerms.AddRange(term.TransposeTermInHierarchy(hierarchyLevel));
							goTerms = (ServerGOTerm[])listGOECTerms.ToArray(typeof(ServerGOTerm));
						}

						foreach(ServerGOTerm goTerm in goTerms)
						{
							if(listGOTermIDs.Contains(goTerm.ID)) continue;
							else 
							{
								listGOTerms.Add(goTerm);
								listGOTermIDs.Add(goTerm.ID);
							}
						}	//end loop over EC Number's annotations
					}
				}	//end loop over process's EC Numbers

			}	//end loop over processes corresponding to the generic process

			return new ServerGOTermProcess((ServerGOTerm[])listGOTerms.ToArray(typeof(ServerGOTerm)), new ServerGenericProcess[]{genericProcess});
		}

		/// <summary>
		/// Returns a set of GenericProcessID / GOTerm annotation sets
		/// </summary>
		/// <param name="pathway">The pathway to query</param>
		/// <param name="mergeProcessesWithSameGOAnnotations">true to merge set of generic processes 
		/// into one ServerGOTermProcess object if they share the same GO annotations</param>
		/// <returns>A set of GenericProcessID / GOTerm annotation sets</returns>
		public static ServerGOTermProcess[] LoadFromPathway(ServerPathway pathway, bool mergeProcessesWithSameGOAnnotations)
		{
			return LoadFromPathway(pathway, ServerGOTerm.MaxHierarchyLevel, mergeProcessesWithSameGOAnnotations);
		}
		/// <summary>
		/// Returns a set of GenericProcessID / GOTerm annotation sets
		/// </summary>
		/// <param name="pathway">The pathway to query</param>
		/// <param name="hierarchyLevel">The level of the GO hierarchy from which annotations come</param>
		/// <param name="mergeProcessesWithSameGOAnnotations">true to merge set of generic processes 
		/// into one ServerGOTermProcess object if they share the same GO annotations</param>
		/// <returns>A set of GenericProcessID / GOTerm annotation sets</returns>
		public static ServerGOTermProcess[] LoadFromPathway(ServerPathway pathway, int hierarchyLevel, bool mergeProcessesWithSameGOAnnotations)
		{
			//get all the processes, generic processes
			ServerGenericProcess[] genProcesses = ServerGenericProcess.LoadFromProcessSet(pathway.GetAllProcesses());
            
			//the list in which to store our results
			ArrayList listGOTermProcesses = new ArrayList();

			//get the annotations via EC number
			foreach(ServerGenericProcess genProcess in genProcesses)
			{
				ArrayList listGOTerms = new ArrayList();
				//get the annotations for this generic process's EC Numbers
				ServerECNumber[] ecNumbers = genProcess.GetECNumbers();

				if(ecNumbers.Length == 0) continue;

				foreach(ServerECNumber ecNumber in ecNumbers)
				{
					ServerGOTerm[] terms = ecNumber.GetAnnotatingGOTerms();
					if(terms.Length == 0 ) continue;

					if(hierarchyLevel >= ServerGOTerm.MaxHierarchyLevel)
						//get annotations from the maximum level
						listGOTerms.AddRange(terms);
					else
						//get annotations from the specified level
						foreach(ServerGOTerm ecTerm in terms)
							listGOTerms.AddRange(ecTerm.TransposeTermInHierarchy(hierarchyLevel));
				}

				if(listGOTerms.Count == 0) continue;

				listGOTermProcesses.Add(
					new ServerGOTermProcess(
					(ServerGOTerm[])listGOTerms.ToArray(typeof(ServerGOTerm)),
					new ServerGenericProcess[]{genProcess}));
			}

			if(mergeProcessesWithSameGOAnnotations)
			{
				Hashtable hashGOTerms = new Hashtable(listGOTermProcesses.Count);
				foreach(ServerGOTermProcess goTermProcess in listGOTermProcesses)
				{
					int key = goTermProcess.GenerateGOBasedHashCode();
					if(hashGOTerms.ContainsKey(key))
					{
						ServerGOTermProcess original = (ServerGOTermProcess)hashGOTerms[key];

						//the generic processes
						ArrayList listProcesses = new ArrayList(original.GenericProcesses);
						listProcesses.AddRange(goTermProcess.GenericProcesses);

						hashGOTerms[key] = new ServerGOTermProcess(original.GOTerms, (ServerGenericProcess[])listProcesses.ToArray(typeof(ServerGenericProcess)));
					}
					else
						hashGOTerms.Add(key, goTermProcess);
				}
				ServerGOTermProcess[] ret = new ServerGOTermProcess[hashGOTerms.Values.Count];
				hashGOTerms.Values.CopyTo(ret, 0);
				return ret;
			}
			else 
				return (ServerGOTermProcess[])listGOTermProcesses.ToArray(typeof(ServerGOTermProcess));
		}

		/// <summary>
		/// Return a hashcode based upon the the concatenation
		/// of the ids of the GO terms
		/// </summary>
		/// <returns>The hashcode</returns>
		private int GenerateGOBasedHashCode()
		{
			string idKey = string.Empty;
			foreach(ServerGOTerm term in this._goTerms)
				idKey += term.ID.ToString();
			return idKey.GetHashCode();
		}

		#endregion

		#endregion

		/// <summary>
		/// Used to sort terms by GOID
		/// </summary>
		internal class GOIDComparer : IComparer
		{
			#region IComparer Members

			public int Compare(object x, object y)
			{
				ServerGOTerm term1 = x as ServerGOTerm;
				ServerGOTerm term2 = y as ServerGOTerm;
				if(term1 == null || term2 == null) throw new InvalidCastException("GOIDComparer can only compare objects of type ServerGOTerm");
				int id1 = int.Parse(term1.ID);
				int id2 = int.Parse(term2.ID);
				return id1.CompareTo(id2);
			}

			#endregion

		}
	}
}