#region Using Declarations
using System;
using System.Xml.Serialization;
#endregion

namespace PathwaysLib.SoapObjects
{	
	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Data/SoapCommonMolecule.cs</filepath>
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
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapCommonMolecule.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents a common molecule.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapCommonMolecule")]
	public class SoapCommonMolecule : SoapObject
	{


		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create a common molecule with nothing initialized.
		/// </summary>
		public SoapCommonMolecule()
		{
		}

		/// <summary>
		/// Create a new common molecule.
		/// </summary>
		/// <param name="status"></param>
		//        public SoapCommonMolecule(string name, string type)
		//        {
		//            this.name = name;
		//            this.type = type;
		//        }

		public SoapCommonMolecule(ObjectStatus status)
		{
			this.Status = status;
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapCommonMolecule()
		{
		}

		/// <summary>
		/// Returns the name of the common molecule.
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
		/// Common molecule database ID.
		/// </summary>
		public Guid ID
		{
			get {return id;}
			set 
			{
				if (id != value)
				{
					id = value;
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
	$Id: SoapCommonMolecule.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapCommonMolecule.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.2  2006/07/07 19:28:04  greg
	The bulk of this update focuses on integrating Ajax browsing into the content browser bar on the left.  It currently only works from the pathways dropdown option, but the framework is now in place for the other lists to function in the same manner.
	
	Revision 1.1  2005/06/30 19:46:32  brandon
	added a bunch of new classes for all the boxed on the ER diagram, even the ones that only have an id attribute
	

		
------------------------------------------------------------------------*/
#endregion