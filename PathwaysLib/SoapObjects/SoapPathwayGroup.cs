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
	///		<filepath>PathwaysLib/SoapObjects/SoapPathwayGroup.cs</filepath>
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
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapPathwayGroup.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents a molecular entity.
	/// </summary>
	#endregion
	[Serializable(), SoapType("SoapMolecularEntity")]
	public class SoapPathwayGroup : SoapObject
	{


		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create a molecular entity with nothing initialized.
		/// </summary>
		public SoapPathwayGroup()
		{
		}

		/// <summary>
		/// Create a new molecular entity.
		/// </summary>
		/// <param name="name">
		/// The name of the entity.
		/// </param>
		/// <param name="notes">
		/// The entity's notes.
		/// </param>
		public SoapPathwayGroup( string name, string notes )
		{
			this.id = Guid.Empty; // created on insert into the DB
			this.name = name;
			this.notes = notes;
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~SoapPathwayGroup()
		{
		}

		/// <summary>
		/// Returns the name of the molecular entity.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string retval;
			//retval = base.ToString();
			retval = name;

			return retval;
		}

		#endregion


		#region Member Variables

		Guid id = Guid.Empty;
		private string name = null;
		private string notes = null;

		#endregion


		#region Properties

		/// <summary>
		/// Molecular entity database ID.
		/// This is virtual so derived classes can 
		/// set thier own row's copy of the ID as well
		/// </summary>
		public virtual Guid Id
		{
			get {return id;}
			set {id = value;}
		}

		/// <summary>
		/// Name of molecular entity
		/// </summary>
		public string Name
		{
			get {return name;}
			set {name = value;}
		}

		/// <summary>
		/// Get/set the entity's notes.
		/// </summary>
		public string Notes
		{
			get{return notes;}
			set{notes=value;}
		}
		
		#endregion


		#region Methods


		#endregion


	} // End class

} // End namespace


#region Change Log
	//----------------------------- END OF SOURCE ----------------------------

	/*------------------------------- Change Log -----------------------------
		$Id: SoapPathwayGroup.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
		$Log: SoapPathwayGroup.cs,v $
		Revision 1.1  2008/05/16 21:15:53  mustafa
		*** empty log message ***
		
		Revision 1.1  2006/07/31 19:37:44  greg
		Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
		
		Revision 1.2  2005/10/26 17:53:57  michael
		Updating doc comments
		
		Revision 1.1  2005/10/13 18:55:58  michael
		Adding groupings for Pathways
		
	------------------------------------------------------------------------*/
#endregion