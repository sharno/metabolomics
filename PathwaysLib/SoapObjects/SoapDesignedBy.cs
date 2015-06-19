#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
	[Serializable(), SoapType("SoapDesignedBy")]
	public class SoapDesignedBy : SoapObject
	{
		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor, create a catalyze relation with nothing initialized.
		/// </summary>
		public SoapDesignedBy()
		{
		}	

		/// <summary>
		/// Constructor, create a new catalyze relation with all fields initialized.
		/// </summary>
		/// <param name="gene_product_id"></param>
		/// <param name="process_id"></param>
		/// <param name="orgGroupId"></param>
		/// <param name="ec_number"></param>
		public SoapDesignedBy(Guid Id, Guid ModelMetadataId, Guid AuthorId)
		{
			this.Id = Id;
			this.ModelMetadataId = ModelMetadataId;
			this.AuthorId = AuthorId;
		}

		/// <summary>
		/// Destructor
		/// </summary>
        ~SoapDesignedBy()
		{
		}

		#endregion

		#region Member Variables
		Guid id = Guid.Empty;
		Guid modelMetadataId = Guid.Empty;
		Guid authorId = Guid.Empty;

		#endregion

		#region Properties
      
		/// <summary>
		/// Get set the  ID
		/// </summary>
		public Guid Id
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

		/// <summary>
		/// Get/set the ModelMetadataId
		/// </summary>
		public Guid ModelMetadataId
		{
			get {return modelMetadataId;}
			set 
			{
				if (modelMetadataId != value)
				{
					modelMetadataId = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Guid AuthorId
		{
			get{return authorId;}
			set{if (authorId != value){authorId=value; Status = ObjectStatus.Update;}}
		}

		#endregion

	} // end class

} // end namespace

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: SoapDesignedBy.cs,v 1.1 2010/04/29 01:08:54 ercument Exp $
	$Log: SoapDesignedBy.cs,v $
	Revision 1.1  2010/04/29 01:08:54  ercument
	*** empty log message ***
	
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
