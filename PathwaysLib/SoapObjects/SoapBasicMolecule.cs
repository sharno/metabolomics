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
	///		<filepath>PathwaysLib/Data/SoapBasicMolecule.cs</filepath>
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
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapBasicMolecule.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents a basic molecule.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapBasicMolecule")]
	public class SoapBasicMolecule : SoapMolecularEntity
	{


		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create a basic molecule with nothing initialized.
		/// </summary>
		public SoapBasicMolecule()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="molecular_entity_notes">may be null</param>
		public SoapBasicMolecule(string name, string molecular_entity_notes)
			: base(name, "basic_molecules", molecular_entity_notes)
		{
			this.id = base.ID; // get ID of base class
		}

//		public SoapBasicMolecule(ObjectStatus status)
//		{
//			this.Status = status;
//		}

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapBasicMolecule()
		{
		}

		/// <summary>
		/// Returns the id of the basic molecule.
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
		private bool isCommon = false;

		#endregion


		#region Properties

		/// <summary>
		/// Basic molecule database ID.
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
		/// Set to true if this is a common molecule, default is false
		/// </summary>
		public bool IsCommon
		{
			get {return isCommon;}
			set
			{
				if (isCommon != value)
				{
					isCommon = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// Type of the basic molecule as a molecular entity. The type field of a basic molecule is always "basic_molecule". 
		/// </summary>
		public override string Type
		{
			get
			{
				return "basic_molecules";
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
	$Id: SoapBasicMolecule.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapBasicMolecule.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.3  2008/05/09 20:23:21  divya
	*** empty log message ***
	
	Revision 1.2  2008/05/01 17:53:29  divya
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.5  2005/10/31 00:39:36  fatih
	*** empty log message ***
	
	Revision 1.4  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.3  2005/07/15 14:30:43  brandon
	Added ECNumberName objects and made changes to ServerECNumber for name lookups.
	Also added functions for common molecules in BasicMolecule objects
	
	Revision 1.2  2005/07/13 22:14:12  brandon
	bug fixes, updated ServerBasicMolecule for inheritance, but it's not finished so don't use it yet.
	
	Revision 1.1  2005/06/30 19:46:32  brandon
	added a bunch of new classes for all the boxed on the ER diagram, even the ones that only have an id attribute
	

		
------------------------------------------------------------------------*/
#endregion