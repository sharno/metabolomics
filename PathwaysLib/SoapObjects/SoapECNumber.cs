#region Using Declarations
using System;
using System.Xml.Serialization;
#endregion

namespace PathwaysLib.SoapObjects
{	
	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Data/SoapECNumber.cs</filepath>
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
	///			</contributor>contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: ali $</cvs_author>
	///			<cvs_date>$Date: 2009/04/30 16:18:00 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapECNumber.cs,v 1.2 2009/04/30 16:18:00 ali Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.2 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents a ec number.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapECNumber")]
	public class SoapECNumber : SoapObject
	{


		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create a ec number with nothing initialized.
		/// </summary>
		public SoapECNumber()
		{
		}

		/// <summary>
		/// Create a new ec number.
		/// </summary>
		/// <param name="ec_number"></param>
		/// <param name="name"></param>
		/// <param name="nodeCode"></param>
		public SoapECNumber(string ec_number, string name, string nodeCode)
		{
		    this.ec_number = ec_number;
			this.name = name;
		    this.nodeCode = nodeCode;
		}

		/// <summary>
		/// SoapECNumber constructor.
		/// </summary>
		/// <param name="status">
		/// The status of the object.
		/// </param>
		public SoapECNumber(ObjectStatus status)
		{
			this.Status = status;
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapECNumber()
		{
		}

		/// <summary>
		/// Returns the ec number as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string retval;
			//retval = base.ToString();
			retval = ec_number;

			return retval;
		}

		#endregion


		#region Member Variables

		private string ec_number = null;
		private string name = null;
		private string nodeCode = null;

		#endregion


		#region Properties

		/// <summary>
		/// ec number
		/// </summary>
		public string ECNumber
		{
			get {return ec_number;}
			set 
			{
				if (ec_number != value)
				{
					ec_number = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// Name of pathway
		/// </summary>
		public string Name
		{
			get {return name;}
			set 
			{
				if (name != value)
				{
					name = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// Type of pathway
		/// </summary>
		public string NodeCode
		{
			get {return nodeCode;}
			set 
			{
				if (nodeCode != value)
				{
					nodeCode = value;
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
	$Id: SoapECNumber.cs,v 1.2 2009/04/30 16:18:00 ali Exp $
	$Log: SoapECNumber.cs,v $
	Revision 1.2  2009/04/30 16:18:00  ali
	*** empty log message ***
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.5  2005/10/31 19:25:11  fatih
	*** empty log message ***
	
	Revision 1.4  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.3  2005/07/13 16:41:45  brandon
	Added a bunch of XML comments where they were missing.
	Changed ServerCatalyze so that it loads a single object from a given ec number instead of an array.
	
	Revision 1.2  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.1  2005/07/08 14:43:12  brandon
	added SoapECNumber and SoapRNA
	
		
------------------------------------------------------------------------*/
#endregion