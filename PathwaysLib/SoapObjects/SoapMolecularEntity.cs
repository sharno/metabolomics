#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
using PathwaysLib.Utilities;
#endregion

namespace PathwaysLib.SoapObjects
{	
	
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/SoapObjects/SoapMolecularEntity.cs</filepath>
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
    ///         <contrubutor>
    ///             <name>Divya Babuji</name>
    ///             <initials>DB</initials>
    ///             <email>divya.babuji@case.edu</email>
    ///         </contrubutor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: mustafa $</cvs_author>
    ///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapMolecularEntity.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Represents a molecular entity.
    /// </summary>
    #endregion
    [Serializable(), SoapType("SoapMolecularEntity")]
    public abstract class SoapMolecularEntity : SoapObject
    {


        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a molecular entity with nothing initialized.
        /// </summary>
        public SoapMolecularEntity()
        {
        }

        /// <summary>
        /// Create a new molecular entity.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="molecular_entity_notes"></param>
        public SoapMolecularEntity(string name, string type, string molecular_entity_notes)
        {
            this.id = Guid.Empty; // created on insert into the DB
            this.name = name;
            this.type = type;
            this.molecular_entity_notes = molecular_entity_notes;
            
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapMolecularEntity()
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
        private string type = null;
        private string molecular_entity_notes = null;
        
        #endregion


        #region Properties

        /// <summary>
        /// Molecular entity database ID.
        /// This is virtual so derived classes can 
        /// set thier own row's copy of the ID as well
        /// </summary>
        public virtual Guid ID
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
        /// Type of molecular entity for inheritance
        /// </summary>
        public abstract string Type
        {
            get;
        }

        /// <summary>
        /// Type of molecular entity
        /// </summary>
        public string MolecularEntityNotes
        {
            get {return molecular_entity_notes;}
            set {molecular_entity_notes = value;}
        }
       		
        #endregion


        #region Methods


        #endregion


    } // End class

} // End namespace


#region Change Log
	//----------------------------- END OF SOURCE ----------------------------

	/*------------------------------- Change Log -----------------------------
		$Id: SoapMolecularEntity.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
		$Log: SoapMolecularEntity.cs,v $
		Revision 1.1  2008/05/16 21:15:53  mustafa
		*** empty log message ***
		
		Revision 1.4  2008/05/09 18:33:25  divya
		*** empty log message ***
		
		Revision 1.3  2008/05/01 17:53:29  divya
		*** empty log message ***
		
		Revision 1.2  2008/05/01 02:20:24  divya
		*** empty log message ***
		
		Revision 1.1  2006/07/31 19:37:44  greg
		Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
		
		Revision 1.8  2005/10/31 00:39:36  fatih
		*** empty log message ***
		
		Revision 1.7  2005/07/11 22:28:57  brendan
		Added inheritance support for GeneProduct and Protein, but they are not fully tested yet.
		
		Revision 1.6  2005/07/11 21:01:52  brendan
		Inheritance now working for Gene/MolecularEntity and the ServerObjectInheritance tests completes successfully.
		
		Revision 1.5  2005/07/01 20:47:39  brendan
		Work on inheritance & the object wrapper.
		
		Revision 1.4  2005/06/29 22:06:10  brendan
		Working on adding support for inheritance between MolecularEntity and Gene.
		
		Revision 1.3  2005/06/27 15:34:11  brandon
		updated SoapOrganism.cs to the new format, hope we stick with it
		
		Revision 1.2  2005/06/24 21:57:32  brendan
		Checkin of work on database object design.
		
		Revision 1.1  2005/06/23 21:55:49  brandon
		new soap objects, still need a LoadLong and LoadDecimal in DBRow.cs
		

	------------------------------------------------------------------------*/
#endregion