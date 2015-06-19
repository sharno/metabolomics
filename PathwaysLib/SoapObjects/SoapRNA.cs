#region Using Declarations
using System;
using System.Xml.Serialization;
#endregion

namespace PathwaysLib.SoapObjects
{	
	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Data/SoapRNA.cs</filepath>
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
	///			<contributor>
	///				<name>Suleyman Fatih Akgul</name>
	///				<initials>sfa</initials>
	///				<email>fatih@case.edu</email>
	///			</contributor>
	///			<contributor>
	///				<name>Gokhan Yavas</name>
	///				<initials>gy</initials>
	///				<email>gokhan.yavas@case.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapRNA.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents a RNA.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapRNA")]
	public class SoapRNA : SoapGeneProduct
	{


		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create a RNA with nothing initialized.
		/// </summary>
		public SoapRNA()
		{
		}

		/// <summary>
		/// Create a new RNA
		/// </summary>
		/// <param name="name"></param>
		/// <param name="molecular_entity_notes"></param>
		/// <param name="gene_product_notes"></param>
		/// <param name="type"></param>
		public SoapRNA(string name, string molecular_entity_notes, string gene_product_notes, string type)
			: base(name, "rnas", molecular_entity_notes, gene_product_notes)
		{
			this.id = base.ID; // get ID of base class
			this.RNAType = type;
		}


		//        public SoapRNA(string name, string type)
		//        {
		//            this.name = name;
		//            this.type = type;
		//        }

		/// <summary>
		/// Constructor with status given
		/// </summary>
		/// <param name="status"></param>
		public SoapRNA(ObjectStatus status)
		{
			this.Status = status;
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapRNA()
		{
		}

		/// <summary>
		/// Returns the id of the RNA.
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
		private string type = null;

		#endregion


		#region Properties

		/// <summary>
		/// RNA database ID.
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
		/// The type for molecular entity
		/// </summary>
		public override string Type
		{
			get
			{
				return "rnas";
			}
		}

		/// <summary>
		/// Type of RNA
		/// </summary>
		public string RNAType
		{
			get {return type;}
			set 
			{
				if (type != value)
				{
					type = value;
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
	$Id: SoapRNA.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapRNA.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.5  2005/10/31 06:10:17  fatih
	*** empty log message ***
	
	Revision 1.4  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.3  2005/07/12 17:34:56  brandon
	Finished up (for the most part) the ProcessEntities relation (left out adding and removing stuff).  Created the ExternalDatabase objects.  fixed a bug in SoapRNA.
	
	Revision 1.2  2005/07/12 04:03:22  brandon
	Updated ServerRNA and SoapRNA to mimic the Protein objects for inheritance.
	
	Revision 1.1  2005/07/08 14:43:12  brandon
	added SoapECNumber and SoapRNA
	

		
------------------------------------------------------------------------*/
#endregion