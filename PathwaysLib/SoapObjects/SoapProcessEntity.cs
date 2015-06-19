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
	///		<filepath>PathwaysLib/SoapObjects/SoapProcessEntity.cs</filepath>
	///		<creation>2005/06/17</creation>
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
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapProcessEntity.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents a molecular entity.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapProcessEntity")]
	public class SoapProcessEntity : SoapObject
	{


		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create a molecular entity with nothing initialized.
		/// </summary>
		public SoapProcessEntity()
		{
		}

		/// <summary>
		/// Constructor, creates a new process entity object with all fields initiallized
		/// </summary>
		/// <param name="process_id"></param>
		/// <param name="entity_id"></param>
		/// <param name="role"></param>
		/// <param name="quantity"></param>
		/// <param name="notes"></param>
		public SoapProcessEntity(Guid process_id, Guid entity_id, string role, string quantity, string notes)
		{
			this.process_id = process_id;
			this.entity_id = entity_id;
			this.role = role;
			this.quantity = quantity;
			this.notes = notes;
            
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapProcessEntity()
		{
		}

		#endregion


		#region Member Variables

		Guid process_id = Guid.Empty;
		Guid entity_id = Guid.Empty;
        private string role = null;
		private string quantity = null;
		private string notes = null;
       
		#endregion


		#region Properties

		/// <summary>
		/// Process database ID.
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
		/// Molecular entity database ID.
		/// </summary>
		public Guid EntityID
		{
			get {return entity_id;}
			set 
			{
				if (entity_id != value)
				{
					entity_id = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// Role of molecular entity in process
		/// </summary>
		public string Role
		{
			get {return role;}
			set 
			{
				if (role != value)
				{
					role = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// Quantity of molecular entity in a given process
		/// </summary>
		public string Quantity
		{
			get {return quantity;}
			set 
			{
				if (quantity != value)
				{
					quantity = value;
					Status = ObjectStatus.Update;
				}
			}
		}
		
		/// <summary>
		/// Notes for the process_entities relation
		/// </summary>
		public string ProcessEntityNotes
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
		$Id: SoapProcessEntity.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
		$Log: SoapProcessEntity.cs,v $
		Revision 1.1  2008/05/16 21:15:53  mustafa
		*** empty log message ***
		
		Revision 1.4  2008/05/09 18:33:25  divya
		*** empty log message ***
		
		Revision 1.3  2008/05/01 17:53:29  divya
		*** empty log message ***
		
		Revision 1.2  2008/04/30 23:17:04  divya
		*** empty log message ***
		
		Revision 1.1  2006/07/31 19:37:44  greg
		Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
		
		Revision 1.2  2005/07/11 22:13:57  brandon
		Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
		
		Revision 1.1  2005/07/11 16:54:39  brandon
		Added ServerProcessEntity and Soap...  for the process_entities relation.  Added funtion GetAllProcesses in ServerMolecularEntity, but GetAllEntities won't work, maybe because the ServerMolecularEntity constructor is protected.  Haven't done any testing yet.
		


	------------------------------------------------------------------------*/
#endregion