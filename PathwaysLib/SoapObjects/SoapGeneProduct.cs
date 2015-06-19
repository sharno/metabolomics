#region Using Declarations
using System;
using System.Xml.Serialization;
using PathwaysLib.Utilities;
#endregion

namespace PathwaysLib.SoapObjects
{	
	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Data/GeneProduct.cs</filepath>
	///		<creation>2005/06/30</creation>
	///		<author>
	///			<name>Brandon Evans</name>
	///			<initials>bse</initials>
	///			<email>brandon.evans@case.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name>Brendan Elliott</name>
	///				<initials>BE</initials>
	///				<email>bxe7@cwru.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapGeneProduct.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents a biological gene product.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapGeneProduct")]
	public abstract class SoapGeneProduct : SoapMolecularEntity
	{


		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create a gene product with nothing initialized.
		/// </summary>
		protected SoapGeneProduct()
		{
		}

		/// <summary>
		/// Creation constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="molecular_entity_notes">
		/// base class notes
		/// </param>
		/// <param name="gene_product_notes"></param>
        public SoapGeneProduct(string name, string type, string molecular_entity_notes, string gene_product_notes)
            : base(name, type, molecular_entity_notes)
        {
            this.id = base.ID; // get ID of base class

            //this.type = type; // (BE) removed Type, as it is currently a duplicate of SoapMolecularEntity.Type
            this.notes = gene_product_notes;
        }

//		/// <summary>
//		/// Create a new gene product.
//		/// </summary>
//		/// <param name="name"></param>
//		/// <param name="type"></param>
		//        public SoapGeneProduct(string notes, string type)
		//        {
		//            this.notes = notes;
		//            this.type = type;
		//        }

		/// <summary>
		/// Constructor with status given
		/// </summary>
		/// <param name="status"></param>
		public SoapGeneProduct(ObjectStatus status)
		{
			this.Status = status;
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapGeneProduct()
		{
		}

		/// <summary>
		/// Returns the id of the gene product.
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
//		private string type = null; //(bse) since Type was removed, this isn't being used
		private string notes = null;

		#endregion


		#region Properties

		/// <summary>
		/// GeneProduct database ID.
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

        // (BE) removed Type, as it is currently a duplicate of SoapMolecularEntity.Type
//		/// <summary>
//		/// Type of gene product
//		/// </summary>
//		public override string Type
//		{
//			get {return type;}
//			set 
//			{
//				if (type != value)
//				{
//					type = value;
//					Status = ObjectStatus.Update;
//				}
//			}
//		}

		/// <summary>
		/// Notes for gene product
		/// </summary>
		public string GeneProductNotes
		{
			get {return notes;}
			set 
			{
				if (notes != value)
				{
					notes = value;
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
	$Id: SoapGeneProduct.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapGeneProduct.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.3  2008/05/09 20:23:22  divya
	*** empty log message ***
	
	Revision 1.2  2008/05/01 17:53:29  divya
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.3  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.2  2005/07/11 22:28:57  brendan
	Added inheritance support for GeneProduct and Protein, but they are not fully tested yet.
	
	Revision 1.1  2005/06/30 19:46:32  brandon
	added a bunch of new classes for all the boxed on the ER diagram, even the ones that only have an id attribute
	

------------------------------------------------------------------------*/
#endregion