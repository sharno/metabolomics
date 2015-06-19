namespace PathwaysLib.WebControls
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Xml;
	using PathwaysLib.QueryObjects;
	using PathwaysLib.ServerObjects;
	using PathwaysLib.Utilities;

	/// <summary>
	/// A pathway AQI node.
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QNodeAQIPathway : QNode
	{
		public override string Title { get { return "Pathway"; } }
		public override QNodeType Type { get { return QNodeType.AQINode; } }
		public override string[] AddTip { get { return new string[] {
			"Cut it out.",
			"A process that takes place in this pathway",
			"A molecular entity that takes place in a process in this pathway",
			"An organism in which a process in this pathway takes place" }; } }

		/// <summary>
		/// Default constructor; sets up parameters
		/// </summary>
		public QNodeAQIPathway( string id, string name ) : base( id, name )
		{
            // TODO: Take out ID string formatting and tell QField what its id is!

			Parameters.Add( new QParam( "Name:", new QFieldPathwayName( string.Empty ) ) );
		}

		public override void JoinWith( ref QueryBuilder ownQuery, QueryBuilder childQuery, int parentType )
		{
            string parentNodeID = ownQuery.NodeID;
            string childNodeID = childQuery.NodeID;

            string tblPW = "PW" + parentNodeID;
            string tblC  = "C"  + parentNodeID + "_" + childNodeID;
            string tblPP = "PP" + parentNodeID + "_" + childNodeID;
            string tblME = "ME" + childNodeID;
            string tblOG = "OG" + childNodeID;
            string tblPE = "PE" + childNodeID;
            string tblPR = "PR" + childNodeID;

			switch( parentType )
			{
				// case 0 is not permitted

				case 1:  // Process
					ownQuery.AddFrom( new string[] { "pathways " + tblPW,
                                                     "pathway_processes " + tblPP,
						                             "processes " + tblPR } );
                    ownQuery.AddWhere( tblPW + ".id = " + tblPP + ".pathway_id" );
                    ownQuery.AddWhere( tblPP + ".process_id = " + tblPR + ".id" );
					break;

				case 2:  // Molecule
                    ownQuery.AddFrom( new string[] { "pathways " + tblPW,
                                                     "pathway_processes " + tblPP,
						                             "process_entities " + tblPE,
                                                     "molecular_entities " + tblME } );
					ownQuery.AddWhere( tblPW + ".id = " + tblPP + ".pathway_id" );
					ownQuery.AddWhere( tblPP + ".process_id = " + tblPE + ".process_id" );
                    ownQuery.AddWhere( tblPE + ".entity_id = " + tblME + ".id" );
					break;

				case 3:  // Organism
					ownQuery.AddFrom( new string[] { "pathways " + tblPW,
                                                     "pathway_processes " + tblPP,
						                             "catalyzes " + tblC,
                                                     "organism_groups " + tblOG } );
					ownQuery.AddWhere( tblPW + ".id = " + tblPP + ".pathway_id" );
					ownQuery.AddWhere( tblPP + ".process_id = " + tblC + ".process_id" );
                    ownQuery.AddWhere( tblC + ".organism_group_id = " + tblOG + ".id" );
					break;

				default: return;
			}

            ownQuery.MergeWith( childQuery );
		}
	}
}