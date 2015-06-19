namespace PathwaysLib.WebControls
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using PathwaysLib.QueryObjects;
	using PathwaysLib.ServerObjects;
	using PathwaysLib.Utilities;

	/// <summary>
	/// An organism AQI node.
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QNodeAQIOrganism : QNode
	{
		public override string Title { get { return "Organism"; } }
		public override QNodeType Type { get { return QNodeType.AQINode; } }
		public override string[] AddTip { get { return new string[] {
			"A pathway that contains a process that takes place in this organism",
			"A process that takes place in this organism",
			"A molecular entity involved in a process that takes place in this organism",
			"Cut it out." }; } }

		/// <summary>
		/// Default constructor; sets up parameters
		/// </summary>
		public QNodeAQIOrganism( string id, string name ) : base( id, name )
		{
            Parameters.Add( new QParam ( "Name:", new QFieldOrganismName ( string.Empty ) ) );
            Parameters.Add( new QParam ( "In Organism Group:", new QFieldOrganismInGroup ( string.Empty ) ) );
			//Parameters.Add( new QParam( "Common name:", new QFieldOrganismCommonName( string.Empty ) ) );
			//Parameters.Add( new QParam( "Scientific name:", new QFieldOrganismScientificName( string.Empty ) ) );
			//Parameters.Add( new QParam( "Group:", new QFieldOrganismGroup( string.Empty ) ) );
		}

		public override void JoinWith( ref QueryBuilder ownQuery, QueryBuilder childQuery, int parentType )
		{
            string parentNodeID = ownQuery.NodeID;
            string childNodeID = childQuery.NodeID;

            string tblOG = "OG" + parentNodeID;
            string tblC  = "C"  + parentNodeID + "_" + childNodeID;
            string tblPP = "PP" + parentNodeID + "_" + childNodeID;
            string tblME = "ME" + childNodeID;
            string tblPE = "PE" + childNodeID;
            string tblPR = "PR" + childNodeID;
            string tblPW = "PW" + childNodeID;

			switch( parentType )
			{
				case 0:  // Pathway
					ownQuery.AddFrom( new string[] { "organism_groups " + tblOG,
                                                     "catalyzes " + tblC,
						                             "pathway_processes " + tblPP,
                                                     "pathways " + tblPW } );
					ownQuery.AddWhere( tblOG + ".id = " + tblC + ".organism_group_id" );
					ownQuery.AddWhere( tblC + ".process_id = " + tblPP + ".process_id" );
					ownQuery.AddWhere( tblPP + ".pathway_id = " + tblPW + ".id" );
					break;

				case 1:  // Process
					ownQuery.AddFrom( new string[] { "organism_groups " + tblOG,
                                                     "catalyzes " + tblC,
						                             "processes " + tblPR } );
					ownQuery.AddWhere( tblOG + ".id = " + tblC + ".organism_group_id" );
					ownQuery.AddWhere( tblC + ".process_id = " + tblPR + ".id" );
					break;

				case 2:  // Molecule
					ownQuery.AddFrom( new string[] { "organism_groups " + tblOG,
                                                     "catalyzes " + tblC,
						                             "process_entities " + tblPE,
                                                     "molecular_entities " + tblME } );
					ownQuery.AddWhere( tblOG + ".id = " + tblC + ".organism_group_id" );
					ownQuery.AddWhere( tblC + ".process_id = " + tblPE + ".process_id" );
					ownQuery.AddWhere( tblPE + ".entity_id = " + tblME + ".id" );
					break;

				// case 3 is not permitted

				default: return;
			}

			ownQuery.MergeWith( childQuery );
		}
	}
}