#region Using Declarations
using System;
using System.Xml.Serialization;
#endregion

namespace PathwaysLib.SoapObjects
{	
	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Data/SoapProtein.cs</filepath>
	///		<creation>2005/06/30</creation>
	///		<author>
	///			<name>Brendan Elliott</name>
	///			<initials>BE</initials>
	///			<email>bxe7@cwru.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name>Brandon Evans</name>
	///				<initials>bse</initials>
	///				<email>brandon.evans@case.edu</email>
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
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapProtein.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents a protein.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapProtein")]
	public class SoapProtein : SoapGeneProduct
	{


		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create a protein with nothing initialized.
		/// </summary>
		public SoapProtein()
		{
		}

		/// <summary>
		/// Creation constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="molecular_entity_notes"></param>
		/// <param name="gene_product_notes"></param>
        public SoapProtein(string name, string molecular_entity_notes, string gene_product_notes)
            : base(name, "proteins", molecular_entity_notes, gene_product_notes)
        {
            this.id = base.ID; // get ID of base class
        }

//		/// <summary>
//		/// Create a new protein.
//		/// </summary>
//		/// <param name="name"></param>
//		/// <param name="type"></param>
		//        public SoapProtein(string name, string type)
		//        {
		//            this.name = name;
		//            this.type = type;
		//        }

		/// <summary>
		/// Contructor
		/// </summary>
		/// <param name="status"></param>
		public SoapProtein(ObjectStatus status)
		{
			this.Status = status;
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapProtein()
		{
		}

		/// <summary>
		/// Returns the name of the protein.
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

		#endregion


		#region Properties

		/// <summary>
		/// protein database ID.
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
		/// Type for the base class
		/// </summary>
        public override string Type
        {
            get
            {
                return "proteins";
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
	$Id: SoapProtein.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapProtein.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.5  2005/10/31 06:10:17  fatih
	*** empty log message ***
	
	Revision 1.4  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.3  2005/07/13 16:41:45  brandon
	Added a bunch of XML comments where they were missing.
	Changed ServerCatalyze so that it loads a single object from a given ec number instead of an array.
	
	Revision 1.2  2005/07/11 22:28:57  brendan
	Added inheritance support for GeneProduct and Protein, but they are not fully tested yet.
	
	Revision 1.1  2005/06/30 21:07:01  brandon
	soap protein added
	
	Revision 1.1  2005/06/30 19:46:32  brandon
	added a bunch of new classes for all the boxed on the ER diagram, even the ones that only have an id attribute
	

		
------------------------------------------------------------------------*/
#endregion