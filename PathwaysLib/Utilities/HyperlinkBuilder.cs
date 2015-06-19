using System;
using System.Configuration;
using PathwaysLib.ServerObjects;
using PathwaysLib.Utilities;
//using PathwaysLib.SBObjects;

namespace PathwaysLib.Utilities
{
	/// <sourcefile>
	///		<project>PathwaysLib</project>
	///		<filepath>PathwaysLib/HyperlinkBuilder.cs</filepath>
	///		<creation>2006/06/23</creation>
	///		<author>
	///			<name>Greg Strnad</name>
	///			<initials>GJS</initials>
	///			<email>gjs4@case.edu</email>
	///		</author>
	/// </sourcefile>
	/// <summary>
	/// Generates hyperlinks to processes, pathways, queries, etc. without having to query the
	/// database for viewIDs.  This adds an additional dependency (DetailForwarding.aspx), but
	/// this optimization alone speeds up page loading times significantly.  The tradeoff is that
	/// URLs constructed with this class will be a bit longer than usual (at least 40-some bytes).
	/// However, the time saved by not repeatedly querying the database should more than make up
	/// for this slight increase in page size approximately 99.999999999999999999% of the time.  :p
	/// </summary>
	public class HyperlinkBuilder
	{
		/// <summary>
		/// Default constructor; not really used
		/// </summary>
		public HyperlinkBuilder() {}

		/// <summary>
		/// Construct a redirecting hyperlink to the detail page for a process
		/// </summary>
		/// <param name="LH">A LinkHelper from which to pull query string parameters</param>
		/// <param name="proc">The process to link to</param>
		/// <returns>A hyperlink string</returns>
		public static string constructProcessHyperlink( LinkHelper LH, ServerProcess proc )
		{
			return constructRedirectHyperlink( LH, proc.GenericProcessID.ToString(), proc.Name, "pc" );
		}
        /// <summary>
        /// Construct a redirecting hyperlink to the detail page for a Species
        /// </summary>
        /// <param name="LH">A LinkHelper from which to pull query string parameters</param>
        /// <param name="proc">The Species to link to</param>
        /// <returns>A hyperlink string</returns>
        public static string constructSpeciesHyperlink(LinkHelper LH, ServerSpecies proc)
        {
            return constructRedirectHyperlink(LH, proc.ID.ToString(), proc.Name, "sp");
        }

		/// <summary>
		/// Construct a redirecting hyperlink to the detail page for a process, using a specific title
		/// </summary>
		/// <param name="LH">A LinkHelper from which to pull query string parameters</param>
		/// <param name="proc">The process to link to</param>
		/// <param name="label">The text to display</param>
		/// <returns>A hyperlink string</returns>
		public static string constructProcessHyperlink( LinkHelper LH, ServerProcess proc, string label )
		{
			return constructRedirectHyperlink( LH, proc.GenericProcessID.ToString(), label, "pc" );
		}

		/// <summary>
		/// Construct a redirecting hyperlink to the detail page for a (generic) process
		/// </summary>
		/// <param name="LH">A LinkHelper from which to pull query string parameters</param>
		/// <param name="proc">The (generic) process to link to</param>
		/// <returns>A hyperlink string</returns>
		public static string constructProcessHyperlink( LinkHelper LH, ServerGenericProcess proc )
		{
			return constructRedirectHyperlink( LH, proc.GenericProcessID.ToString(), proc.Name, "pc" );
		}

		/// <summary>
		/// Construct a redirecting hyperlink to the detail page for a process
		/// </summary>
		/// <param name="LH">A LinkHelper from which to pull query string parameters</param>
		/// <param name="id">The ID of the process to link to</param>
		/// <param name="name">The text to display</param>
		/// <returns>A hyperlink string</returns>
		public static string constructProcessHyperlink( LinkHelper LH, string id, string name )
		{
			return constructRedirectHyperlink( LH, id, name, "pc" );
		}

		/// <summary>
		/// Construct a redirecting hyperlink to the detail page for connecting pathways
		/// </summary>
		/// <param name="LH">A LinkHelper from which to pull query string parameters</param>
		/// <param name="path">The pathway to connect from</param>
		/// <returns>A hyperlink string</returns>
		public static string constructPathwayHyperlink( LinkHelper LH, ConnectedPathwayAndCommonProcesses path )
		{
			return constructRedirectHyperlink( LH, path.ConnectedPathway.ID.ToString(), path.ConnectedPathway.Name, "pw" );
		}

		/// <summary>
		/// Construct a redirecting hyperlink to the detail page for a pathway
		/// </summary>
		/// <param name="LH">A LinkHelper from which to pull query string parameters</param>
		/// <param name="path">The pathway to link to</param>
		/// <returns>A hyperlink string</returns>
		public static string constructPathwayHyperlink( LinkHelper LH, ServerPathway path )
		{
			return constructRedirectHyperlink( LH, path.ID.ToString(), path.Name, "pw" );
		}

		/// <summary>
		/// Construct a redirecting hyperlink to the detail page for a pathway
		/// </summary>
		/// <param name="LH">A LinkHelper from which to pull query string parameters</param>
		/// <param name="id">The ID of the pathway to link to</param>
		/// <param name="name">The text to display</param>
		/// <returns>A hyperlink string</returns>
		public static string constructPathwayHyperlink( LinkHelper LH, string id, string name )
		{
			return constructRedirectHyperlink( LH, id, name, "pw" );
		}

        /// <summary>
        /// Construct a redirecting hyperlink to the detail page for an organism group
        /// </summary>
        /// <param name="LH">A LinkHelper from which to pull query string parameters</param>
        /// <param name="path">The pathway to link to</param>
        /// <returns>A hyperlink string</returns>
        public static string constructOrganismGroupHyperlink(LinkHelper LH, ServerOrganismGroup org)
        {
            return constructRedirectHyperlink(LH, org.ID.ToString(), org.Name, "org");
        }

        /// <summary>
        /// Construct a redirecting hyperlink to the detail page for an organism
        /// </summary>
        /// <param name="LH">A LinkHelper from which to pull query string parameters</param>
        /// <param name="path">The pathway to link to</param>
        /// <returns>A hyperlink string</returns>
        public static string constructOrganismHyperlink(LinkHelper LH, ServerOrganism org)
        {
            return constructRedirectHyperlink(LH, org.ID.ToString(), org.Name, "org");
           
        }


        /// <summary>
        /// Construct a redirecting hyperlink to the detail page for a pathway
        /// </summary>
        /// <param name="LH">A LinkHelper from which to pull query string parameters</param>
        /// <param name="id">The ID of the pathway to link to</param>
        /// <param name="name">The text to display</param>
        /// <returns>A hyperlink string</returns>
        public static string constructOrganismGroupHyperlink(LinkHelper LH, string id, string name)
        {
            return constructRedirectHyperlink(LH, id, name, "org");
        }

		/// <summary>
		/// Construct a redirecting hyperlink to the detail page for a molecular entity
		/// </summary>
		/// <param name="LH">A LinkHelper from which to pull query string parameters</param>
		/// <param name="mol">The molecular entity to link to</param>
		/// <returns>A hyperlink string</returns>
		public static string constructMoleculeHyperlink( LinkHelper LH, ServerMolecularEntity mol )
		{
			return constructRedirectHyperlink( LH, mol.ID.ToString(), mol.Name, "me" );
		}

		/// <summary>
		/// Construct a redirecting hyperlink to the detail page for a molecular entity
		/// </summary>
		/// <param name="LH">A LinkHelper from which to pull query string parameters</param>
		/// <param name="id">The ID of the molecular entity to link to</param>
		/// <param name="name">The text to display</param>
		/// <returns>A hyperlink string</returns>
		public static string constructMoleculeHyperlink( LinkHelper LH, string id, string name )
		{
			return constructRedirectHyperlink( LH, id, name, "me" );
		}

		/// <summary>
		/// Construct a redirecting hyperlink to the detail page for a gene
		/// </summary>
		/// <param name="LH">A LinkHelper from which to pull query string parameters</param>
		/// <param name="gene">The gene to link to</param>
		/// <returns>A hyperlink string</returns>
		public static string constructGeneHyperlink( LinkHelper LH, ServerGene gene )
		{
			return constructRedirectHyperlink( LH, gene.ID.ToString(), gene.Name, "me" );
		}

        /// <summary>
        /// Construct a redirecting hyperlink to the detail page for a gene
        /// </summary>
        /// <param name="LH">A LinkHelper from which to pull query string parameters</param>
        /// <param name="gene">The gene to link to</param>
        /// <returns>A hyperlink string</returns>
        public static string constructGeneHyperlink(LinkHelper LH, string geneName, string geneId)
        {
            return constructRedirectHyperlink(LH, geneId, geneName, "me");
        }

		/// <summary>
		/// Construct a generic redirecting hyperlink.
		/// </summary>
		/// <param name="LH">A LinkHelper from which to pull query string parameters</param>
		/// <param name="id">The link id</param>
		/// <param name="name">The text to display</param>
		/// <param name="type">The link type</param>
		/// <returns>A hyperlink string</returns>
		public static string constructRedirectHyperlink( LinkHelper LH, string id, string name, string type )
		{
            if (id == Util.NullGuid)
                return name;
            else
			    return string.Format( "<a href=\"{0}/Web/LinkForwarder.aspx?rid={1}&amp;rtype={2}{3}\">{4}</a>",
				    ConfigurationManager.AppSettings["PathwaysWebBaseUrl"], id, type, LH.RedirectParams, name );
		}

		/// <summary>
		/// Construct an external database hyperlink.
		/// </summary>
		/// <param name="extDb">The external database to link to</param>
		/// <returns>A hyperlink string</returns>
		public static string constructExternalDatabaseHyperlink( ServerExternalDatabase extDb )
		{
			return string.Format( "<a href=\"{0}\" onclick=\"window.open(this.href,'_blank');return false\">{1}</a>",
				extDb.URL, extDb.Name );
		}

		/// <summary>
		/// Construct an external database link hyperlink.
		/// </summary>
		/// <param name="extDb">The external database link to link to (heh)</param>
		/// <returns>A hyperlink string</returns>
		public static string constructExternalDbLinkHyperlink( ServerExternalDbLink extDb )
		{
			return string.Format( "<a href=\"{0}\" onclick=\"window.open(this.href,'_blank');return false\">{1}</a>",
				ServerExternalDatabase.TranslateUrlFromExternalDbLink( extDb, "pathway" ),
				extDb.NameInExternalDatabase );
		}

		/// <summary>
		/// Construct a browser section hyperlink.
		/// </summary>
		/// <param name="id">The link id</param>
		/// <returns>A hyperlink string</returns>
		public static string constructBrowserSectionHyperlink( string id )
		{
			return string.Format( "{0}/Web/LinkForwarder.aspx?rid={1}&rtype=br",
				ConfigurationManager.AppSettings["PathwaysWebBaseUrl"], id );
		}

        public static string constructBrowserSectionHyperlink(string id, string kegg)
        {
            string with_or_withoutKeggData = (kegg.ToLower().Trim() == "withkeggrelateddata") ? "true" : "false";
            
            return string.Format("{0}/Web/LinkForwarder.aspx?rid={1}&rtype=br&withkeggrelateddata={2}",
                ConfigurationManager.AppSettings["PathwaysWebBaseUrl"], id, with_or_withoutKeggData);
        }

        public static string constructModelHyperLink(LinkHelper LH, string modelid, string modelname)
        {
            return constructRedirectHyperlink(LH, modelid, modelname, "md");
        }

        // don't use! -ahmet
        public static string constructCompartmentHyperLink(LinkHelper LH, string compartmentid, string compartmentname)
        {
            return constructRedirectHyperlink(LH, compartmentid, compartmentname, "cp");
        }

        public static string constructReactionHyperlink(LinkHelper LH, string modelid, string modelname, string processid, string processname)
        {
            //return constructRedirectHyperlink(LH, sm.ID.ToString(), sm.Name.ToString(), "md");

            if (modelid == Util.NullGuid)
                return modelname;
            else
                return string.Format("<a href=\"{0}/Web/LinkForwarder.aspx?rid={1}&amp;rtype={2}&amp;pid={3}&amp;pname={5}{4}\">{5}</a>",
                    ConfigurationManager.AppSettings["PathwaysWebBaseUrl"], modelid, "md", processid, LH.RedirectParams, processname);
        }

        public static object constructCompartmentHyperLink(LinkHelper LH, ServerModel child)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}