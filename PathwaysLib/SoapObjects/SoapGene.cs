#region Using Declarations
using System;
using System.Xml.Serialization;
using PathwaysLib.Utilities;

using PathwaysLib.ServerObjects;
#endregion

namespace PathwaysLib.SoapObjects
{	
	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/SoapObjects/SoapGene.cs</filepath>
	///		<creation>2005/06/23</creation>
	///		<author>
	///			<name>Brandon S. Evans</name>
	///			<initials>bse</initials>
	///			<email>brandon.evans@case.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name>Brendan Elliott</name>
	///				<initials>BE</initials>
	///				<email>bxe7@cwru.edu</email>
	///			</contributor>
	///			<contributor>
	///				<name>Michael F. Starke</name>
	///				<initials>mfs</initials>
	///				<email>michael.starke@case.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapGene.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents a molecular entity.
	/// </summary>
	#endregion
    [Serializable(), SoapType("SoapGene")]
    public class SoapGene : SoapMolecularEntity
    {


        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a pathway with nothing initialized.
        /// </summary>
        public SoapGene()
        {
        }

        /// <summary>
        /// Creates a new Gene.
        /// 
        /// The notes fields may be null.  
        /// Specify ChromosomeID (gene is in a specific organism) or 
        /// OrganismGroupID (gene is common to a group of organisms), but not both.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="molecular_entity_notes">May be null</param>
        /// <param name="gene_group_id">If there is a homologous gene(s), use the GeneGroupId of that gene, otherwise leave this empty (Guid.Empty) to have a new gene group id be generated for this gene</param>
        /// <param name="chromosome_id">Make this non-empty (not Guid.Empty) if this gene is present in a specific organism instead of in an Organism Group.  If this is entered, OrganismGroupId should be set to Guid.Empty!</param>
        /// <param name="cytogenic_address"></param>
        /// <param name="genetic_address"></param>
        /// <param name="organism_group_id">Make this non-empty (not Guid.Empty) if this gene is present in a general organism group instead of in a specific organism.  If this is entered, ChromosomeId should be set to Guid.Empty!</param>
        /// <param name="gene_notes">May be null</param>
        /// <param name="relative_address"></param>
        /// <param name="rawAddress"></param>
        public SoapGene(string name, string molecular_entity_notes, Guid gene_group_id, Guid chromosome_id
			, string cytogenic_address, long genetic_address, Guid organism_group_id, string gene_notes, long relative_address, string rawAddress)
            : base(name, "genes", molecular_entity_notes)
        {
			this.id = base.ID; // get ID of base class

			this.gene_group_id = gene_group_id;
			this.chromosome_id = chromosome_id;
			this.cytogenic_address = cytogenic_address;
			this.genetic_address = genetic_address;
			this.organism_group_id = organism_group_id;
			this.geneNotes = gene_notes;
			this.relative_address = relative_address;
			this.raw_address = rawAddress;
        }
        public SoapGene(string id, string name, string relative_address, string cytogenic_address, string generic_process_id, string chromosome_id)
        {
            this.id = new Guid(id);
            this.Name = name;
            this.RawAddress = relative_address;
            this.cytogenic_address = cytogenic_address;
            this.gene_group_id = new Guid(generic_process_id);
            this.ChromosomeID = string.IsNullOrEmpty(chromosome_id) ? Guid.Empty : new Guid(chromosome_id);
        }
        public SoapGene(string name, string relative_address, string cytogenic_address, string generic_process_id, string chromosome_id)
        {
            this.Name = name;
            this.RawAddress = relative_address;
            this.cytogenic_address = cytogenic_address;
            this.gene_group_id = new Guid(generic_process_id);
            this.ChromosomeID = string.IsNullOrEmpty(chromosome_id) ? Guid.Empty : new Guid(chromosome_id);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapGene()
        {
        }

        /// <summary>
        /// Returns the id of the gene.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string retval;
            //retval = base.ToString();
            retval = id.ToString();

            return retval;
        }

        #endregion


        #region Member Variables

        Guid id = Guid.Empty;
		Guid gene_group_id = Guid.Empty;
		Guid chromosome_id = Guid.Empty;
		private string cytogenic_address = null;
		private long genetic_address = 0;
		Guid organism_group_id = Guid.Empty;
		private string geneNotes = null;
		private long relative_address = 0;
		private string raw_address;

        #endregion


        #region Properties

        /// <summary>
        /// Gene database ID.
        /// </summary>
        public override Guid ID
        {
            get {return id;}
            set 
            {
                if (id != value)
                {
                    // (BE) set base class's ID as well
                    base.ID = value;
                    id = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

		/// <summary>
		/// Gene Group ID.
        /// 
        /// A Gene group is used to identify homologous genes
        /// between different organisms.  This should be either
        /// set to the same value as another gene's GeneGroupId
        /// if they are homologous, or left empty and a new 
        /// GeneGroupId will be created at insertion.
        /// </summary>
		public Guid GeneGroupID
		{
			get {return gene_group_id;}
			set 
            {
                if (gene_group_id != value)
                {
                    gene_group_id = value;
                    Status = ObjectStatus.Update;
                }
            }
		}

		/// <summary>
		/// Gene chromosome ID.
		/// </summary>
		public Guid ChromosomeID
		{
			get {return chromosome_id;}
			set 
            {
                if (chromosome_id != value)
                {
                    chromosome_id = value;

                    // can't have both organism group ID and chromosome ID! (see check constraint CK_genes)
                    if (chromosome_id != Guid.Empty)
                        organism_group_id = Guid.Empty; 

                    Status = ObjectStatus.Update;
                }
            }
		}

        /// <summary>
        /// cytogenic address of gene
        /// </summary>
        public string CytogenicAddress
        {
            get {return cytogenic_address;}
            set 
            {
                if (cytogenic_address != value)
                {
                    cytogenic_address = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

		/// <summary>
		/// Gene's genetic address.
		/// </summary>
		public long GeneticAddress
		{
			get {return genetic_address;}
			set 
            {
                if (genetic_address != value)
                {
                    genetic_address = value;
                    Status = ObjectStatus.Update;
                }
            }
		}

		/// <summary>
		/// Gene database ID.
		/// </summary>
		public Guid OrganismGroupID
		{
			get {return organism_group_id;}
			set 
            {
                if (organism_group_id != value)
                {
                    organism_group_id = value;
               
                    // can't have both organism group ID and chromosome ID! (see check constraint CK_genes)
                    if (organism_group_id != Guid.Empty)
                        chromosome_id = Guid.Empty; 

                    Status = ObjectStatus.Update;
                }
            }
		}

        /// <summary>
        /// Type of pathway
        /// </summary>
        public string GeneNotes
        {
            get {return geneNotes;}
            set 
            {
                if (geneNotes != value)
                {
                    geneNotes = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

		/// <summary>
		/// Type of pathway
		/// </summary>
		public long RelativeAddress
		{
			get {return relative_address;}
			set 
            {
                if (relative_address != value)
                {
                    relative_address = value;
                    Status = ObjectStatus.Update;
                }
            }
		}

		/// <summary>
		/// Get the molecular entity type, returns "genes"
		/// </summary>
        public override string Type
        {
            get {return "genes";}
        }

		/// <summary>
		/// 
		/// </summary>
		public string RawAddress
		{
			get {return raw_address;}
			set 
			{
				if (raw_address != value)
				{
					raw_address = value;
					Status = ObjectStatus.Update;
				}
			}
		}

        #endregion


        #region Methods


        #endregion


    } // End class

} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: SoapGene.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapGene.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.10  2008/05/09 20:23:22  divya
	*** empty log message ***
	
	Revision 1.9  2008/05/01 17:53:29  divya
	*** empty log message ***
	
	Revision 1.8  2008/04/25 14:06:45  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.7  2006/12/01 04:48:29  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.6  2006/11/26 21:49:25  ali
	*** empty log message ***
	
	Revision 1.5  2006/10/19 21:03:36  brendan
	New graph drawing code ... performs bulk-loading of server objects to reduce the number of queries and filling an object cache.  Also provides an interface for alternative data sources (i.e. XML biopax doc).  Other misc bug fixes.
	
	Revision 1.4  2006/10/13 19:38:20  ali
	*** empty log message ***
	
	Revision 1.3  2006/09/06 14:41:40  pathwaysdeploy
	A new web method for gene viewer has been added.
	
	Revision 1.2  2006/08/17 15:04:43  ali
	A new web method "GetGeneMappingForPathway" is added for the gene viewer.
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.11  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.10  2006/04/21 17:37:29  michael
	*** empty log message ***
	
	Revision 1.9  2005/10/31 06:10:17  fatih
	*** empty log message ***
	
	Revision 1.8  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.7  2005/07/13 22:14:12  brandon
	bug fixes, updated ServerBasicMolecule for inheritance, but it's not finished so don't use it yet.
	
	Revision 1.6  2005/07/11 22:28:57  brendan
	Added inheritance support for GeneProduct and Protein, but they are not fully tested yet.
	
	Revision 1.5  2005/07/11 21:01:52  brendan
	Inheritance now working for Gene/MolecularEntity and the ServerObjectInheritance tests completes successfully.
	
	Revision 1.4  2005/07/01 20:47:39  brendan
	Work on inheritance & the object wrapper.
	
	Revision 1.3  2005/06/29 22:06:10  brendan
	Working on adding support for inheritance between MolecularEntity and Gene.
	
	Revision 1.2  2005/06/24 21:57:32  brendan
	Checkin of work on database object design.
	
	Revision 1.1  2005/06/23 21:55:49  brandon
	new soap objects, still need a LoadLong and LoadDecimal in DBRow.cs
	
	Revision 1.2  2005/06/20 17:53:15  michael
	Bug fixes
	
	Revision 1.1  2005/06/16 20:23:28  michael
	renaming Pathway.cs to SoapPathway.cs
	
	Revision 1.3  2005/06/10 20:31:52  brendan
	Added ServerObject and SoapObject base classes and code in progress for ServerPathway and Pathway.
	
	Revision 1.2  2005/06/08 22:51:40  brendan
	Added default constructor for use with web service.
	
	Revision 1.1  2005/06/08 20:44:10  brendan
	Adding skeleton projects for refactoring into the 3.0 version.
		
------------------------------------------------------------------------*/
#endregion