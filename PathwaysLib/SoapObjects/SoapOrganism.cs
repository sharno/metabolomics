#region Using Declarations
using System;
using System.Xml.Serialization;
#endregion

namespace PathwaysLib.SoapObjects
{	
	
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/SoapObjects/SoapOrganism.cs</filepath>
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
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: mustafa $</cvs_author>
    ///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapOrganism.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Represents an organism.
    /// </summary>
    #endregion
    [Serializable(), SoapType("SoapOrganism")]
    public class SoapOrganism : SoapOrganismGroup
    {


        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a organism with nothing initialized.
        /// </summary>
        public SoapOrganism()
        {
        }

        /// <summary>
        ///  Create a new organism with all fields initiallized.
        /// </summary>
        /// <param name="common_name"></param>
        /// <param name="scientific_name"></param>
        /// <param name="taxonomy_id"></param>
        /// <param name="parent"></param>
        /// <param name="notes"></param>
        public SoapOrganism( string common_name, string scientific_name, string taxonomy_id, Guid parent, string notes, int cM_unit_length):
			base(scientific_name, common_name, parent, notes)
        {
			this.taxonomy_id= taxonomy_id;
            this.cM_unit_length = cM_unit_length;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapOrganism()
        {
        }

        /// <summary>
        /// Returns the name of the organism.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string retval = this.Name;
            return retval;
        }

        #endregion


        #region Member Variables

		private string taxonomy_id = null;
        private int cM_unit_length = 1000000;

        #endregion


        #region Properties


		/// <summary>
		/// Taxonomy ID of organism
		/// </summary>
		public string TaxonomyID
		{
			get {return taxonomy_id;}
			set            
			{
				if (taxonomy_id != value)
				{
					taxonomy_id = value;
					Status = ObjectStatus.Update;
				}
			}
		}


        /// <summary>
        /// CM_Unit_Length of organism
        /// </summary>
        public int CM_Unit_Length
        {
            get { return cM_unit_length; }
            set
            {
                if (cM_unit_length != value)
                {
                    cM_unit_length = value;
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
	$Id: SoapOrganism.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: SoapOrganism.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.2  2006/10/19 01:24:43  ali
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.7  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.6  2006/04/12 20:26:44  brian
	*** empty log message ***
	
	Revision 1.5.8.1  2006/03/20 19:26:32  brian
	*** empty log message ***
	
	Revision 1.5  2005/10/31 06:10:17  fatih
	*** empty log message ***
	
	Revision 1.4  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.3  2005/06/28 23:20:25  brendan
	Fixed Gene, MolecularEntity, Organism, and Process to reflect recent wrapper object design changes.
	
	Revision 1.2  2005/06/27 15:34:11  brandon
	updated SoapOrganism.cs to the new format, hope we stick with it
	
	Revision 1.1  2005/06/21 19:32:55  brandon
	SoapOrganism.cs, for use with ServerOrganism.cs, gonna take a break now
	

		
------------------------------------------------------------------------*/
#endregion