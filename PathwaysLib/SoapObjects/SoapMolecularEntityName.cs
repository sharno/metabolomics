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
    ///		<filepath>PathwaysLib/SoapObjects/SoapMolecularEntityName.cs</filepath>
    ///		<creation>2005/07/07</creation>
    ///		<author>
    ///			<name>Brendan Elliott</name>
    ///			<initials>BE</initials>
    ///			<email>bxe7@case.edu</email>
    ///		</author>
    ///		<contributors>
    ///			<contributor>
    ///				<name>none</name>
    ///				<initials>none</initials>
    ///				<email>none</email>
    ///			</contributor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: mustafa $</cvs_author>
    ///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapMolecularEntityName.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Represents one of possibly many typed names for a molecular entity.
    /// </summary>
    #endregion
    [Serializable(), SoapType("SoapMolecularEntityName")]	
    public class SoapMolecularEntityName : SoapObject
	{
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create an entity name with nothing initialized.
        /// </summary>
        public SoapMolecularEntityName()
        {
        }	

        /// <summary>
        /// Create a new entity name.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public SoapMolecularEntityName(Guid entityId, string name, string type)
        {
            this.entity_id = entityId;
            this.name = name;
            this.type = type;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapMolecularEntityName()
        {
        }

        #endregion

        #region Member Variables

        Guid entity_id = Guid.Empty;
        string name;
        string type;

        #endregion

        #region Properties

		/// <summary>
		/// Get/set the molecular entity id
		/// </summary>
        public Guid MolecularEntityId
        {
            get {return entity_id;}
            set {entity_id = value;}
        }

		/// <summary>
		/// Get/set a molecular entity name
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
	$Id: SoapMolecularEntityName.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapMolecularEntityName.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.3  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.2  2005/07/08 21:55:07  brendan
	Debugging MolecularEntity/EntityNames/Gene inheritance.  Inheritance test not passing yet.
	
	Revision 1.1  2005/07/07 23:30:51  brendan
	Work in progress on entity names.  MolecularEntityName virtually complete, but not tested.
	
------------------------------------------------------------------------*/
#endregion
