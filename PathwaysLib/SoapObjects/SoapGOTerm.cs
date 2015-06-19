using System;
using System.Xml;
using System.Xml.Serialization;

namespace PathwaysLib.SoapObjects
{
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/SoapObjects/SoapGOTerm.cs</filepath>
	///		<creation>2005/02/</creation>
	///		<author>
	///			<name>Marc Reynolds</name>
	///			<initials>mrr</initials>
	///			<email>marc.reynolds@case.edu</email>
	///		</author>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/SoapGOTerm.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents a Gene Ontology Term
	/// </summary>
	#endregion

	[Serializable(), SoapType("SoapGOTerm")]
	public class SoapGOTerm : SoapObject
	{
		#region Constructor, Destructor, ToString

		/// <summary>
		/// Default constructor in which nothing is initialized
		/// </summary>
		public SoapGOTerm(){}

        public SoapGOTerm(string termId, string name, int termLevel) 
        {
            this.id = termId;
            this.name = name;
            this.termLevel = termLevel;
            totalDescendants = 0;
        }

		#endregion

		#region Member Variables

		private string id;
		private string name;
		private int totalDescendants;
        private int termLevel;
		private int maximumSubtreeHeight;

		#endregion

		#region Properties

		/// <summary>
		/// The 7-digit id with leading zeros
		/// </summary>
		public string ID
		{
			get{return id;}
			set
			{
				if(id != value)
				{
					id = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// The name of the Gene Ontology Term
		/// </summary>
		public string Name
		{
			get{return name;}
			set
			{
				if(name != value)
				{
					name = value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// The total number of descendants in this term's subtree
		/// </summary>
		public int TotalDescendants
		{
			get{return totalDescendants;}
			set
			{
				if(totalDescendants != value)
				{
					totalDescendants= value;
					Status = ObjectStatus.Update;
				}
			}
		}

		/// <summary>
		/// The maximum height of this term's subtree
		/// </summary>
		public int MaximumSubtreeHeight
		{
			get{return maximumSubtreeHeight;}
			set
			{
				if(maximumSubtreeHeight != value)
				{
					maximumSubtreeHeight = value;
					Status = ObjectStatus.Update;
				}
			}
		}

        /// <summary>
        /// Level of the the term in GO
        /// </summary>
        public int TermLevel
        {
            get { return termLevel; }
            set
            {
                if (termLevel != value)
                {
                    termLevel = value;
                    Status = ObjectStatus.Update;
                }
            }
        }


		#endregion

		#region Methods

		#endregion




	}
}
