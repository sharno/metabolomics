#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/SoapObjects/SoapECNumberName.cs</filepath>
	///		<creation>2005/07/07</creation>
	///		<author>
	///			<name>Brandon S. Evans</name>
	///			<initials>bse</initials>
	///			<email>brandon.evans@case.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name>Brendan Elliott</name>
	///				<initials>BE</initials>
	///				<email>bxe7@case.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapECNumberName.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents one of possibly many typed names for an ec number.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapECNumberName")]	
	public class SoapECNumberName : SoapObject
	{
		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create an entity name with nothing initialized.
		/// </summary>
		public SoapECNumberName()
		{
		}	

		/// <summary>
		/// Create a new name for an ec number.
		/// </summary>
		/// <param name="ec_number"></param>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public SoapECNumberName(string ec_number, string name, string type)
		{
			this.ec_number = ec_number;
			this.name = name;
			this.type = type;
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapECNumberName()
		{
		}

		#endregion

		#region Member Variables

		string ec_number;
		string name;
		string type;

		#endregion

		#region Properties

		/// <summary>
		/// Get set the ec number
		/// </summary>
		public string ECNumber
		{
			get {return ec_number;}
			set {ec_number = value;}
		}

		/// <summary>
		/// Get/set the name
		/// </summary>
		public string Name
		{
			get {return name;}
			set {name = value;}
		}

		/// <summary>
		/// Get/set the type
		/// </summary>
		public string Type
		{
			get {return type;}
			set {type = value;}
		}

		#endregion

	} // end class

} // end namespace

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: SoapECNumberName.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapECNumberName.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.2  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.1  2005/07/15 14:30:43  brandon
	Added ECNumberName objects and made changes to ServerECNumber for name lookups.
	Also added functions for common molecules in BasicMolecule objects
	
	
------------------------------------------------------------------------*/
#endregion
