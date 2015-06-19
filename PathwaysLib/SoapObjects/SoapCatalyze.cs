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
	///		<filepath>PathwaysLib/SoapObjects/SoapCatalyze.cs</filepath>
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
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapCatalyze.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents one of possibly many typed names for a molecular entity.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapCatalyze")]	
	public class SoapCatalyze : SoapObject
	{
		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create a catalyze relation with nothing initialized.
		/// </summary>
		public SoapCatalyze()
		{
		}	

		/// <summary>
		/// Constructor, create a new catalyze relation with all fields initialized.
		/// </summary>
		/// <param name="gene_product_id"></param>
		/// <param name="process_id"></param>
		/// <param name="orgGroupId"></param>
		/// <param name="ec_number"></param>
		public SoapCatalyze( Guid gene_product_id, Guid process_id, Guid orgGroupId, string ec_number )
		{
			this.gene_product_id = gene_product_id;
			this.process_id = process_id;
			this.ec_number = ec_number;
			this.org_group_id = orgGroupId;
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapCatalyze()
		{
		}

		#endregion

		#region Member Variables

		Guid gene_product_id = Guid.Empty;
		Guid process_id = Guid.Empty;
		string ec_number = null;
		Guid org_group_id = Guid.Empty;

		#endregion

		#region Properties

		/// <summary>
		/// Get set the gene product ID
		/// </summary>
		public Guid GeneProductID
		{
			get {return gene_product_id;}
			set 
			{
				if (gene_product_id != value)
				{
					gene_product_id = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// Get/set the process ID
		/// </summary>
		public Guid ProcessID
		{
			get {return process_id;}
			set 
			{
				if (process_id != value)
				{
					process_id = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// Get/set the ec number
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
		/// 
		/// </summary>
		public Guid OrganismGroupID
		{
			get{return org_group_id;}
			set{if (org_group_id != value){org_group_id=value; Status = ObjectStatus.Update;}}
		}

		#endregion

	} // end class

} // end namespace

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: SoapCatalyze.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapCatalyze.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.7  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.6  2006/04/21 17:37:29  michael
	*** empty log message ***
	
	Revision 1.5  2005/07/13 16:41:45  brandon
	Added a bunch of XML comments where they were missing.
	Changed ServerCatalyze so that it loads a single object from a given ec number instead of an array.
	
	Revision 1.4  2005/07/12 22:14:04  brandon
	Bug fixes.      Also added external_database_links objects
	
	Revision 1.3  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.2  2005/07/11 16:54:39  brandon
	Added ServerProcessEntity and Soap...  for the process_entities relation.  Added funtion GetAllProcesses in ServerMolecularEntity, but GetAllEntities won't work, maybe because the ServerMolecularEntity constructor is protected.  Haven't done any testing yet.
	
	Revision 1.1  2005/07/08 19:32:05  brandon
	fixed ServerCatalyze, sort of,  and uh, this project builds now
	
	
------------------------------------------------------------------------*/
#endregion
