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
	/// A molecular entity AQI node.
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QNodeAQIMolecule : QNode
	{
		public override string Title { get { return "Molecular Entity"; } }
		public override QNodeType Type { get { return QNodeType.AQINode; } }
		public override string[] AddTip { get { return new string[] {
			"A pathway that contains a process that involves this molecular entity",
			"A process that involves this molecular entity",
			"Cut it out.",
			"An organism in which a process that involves this molecular entity takes place" }; } }

		/// <summary>
		/// Default constructor; sets up parameters
		/// </summary>
		public QNodeAQIMolecule( string id, string name ) : base( id, name )
		{
			Parameters.Add( new QParam( "Name:", new QFieldMoleculeName( string.Empty ) ) );
			Parameters.Add( new QParam( "Name type:", new QFieldMoleculeNameType( string.Empty ) ) );
			Parameters.Add( new QParam( "Molecule type:", new QFieldMoleculeType( string.Empty ) ) );
			Parameters.Add( new QParam( "Role:", new QFieldMoleculeRole( string.Empty ) ) );
			Parameters.Add( new QParam( "Amount:", new QFieldMoleculeAmount( string.Empty ) ) );
		}

		public override void JoinWith( ref QueryBuilder ownQuery, QueryBuilder childQuery, int parentType )
		{
            string parentNodeID = ownQuery.NodeID;
            string childNodeID = childQuery.NodeID;

            string tblME = "ME" + parentNodeID;
            string tblPE = "PE" + parentNodeID;
            string tblC  = "C"  + parentNodeID + "_" + childNodeID;
            string tblPP = "PP" + parentNodeID + "_" + childNodeID;
            string tblOG = "OG" + childNodeID;
            string tblPR = "PR" + childNodeID;
            string tblPW = "PW" + childNodeID;

			switch( parentType )
			{
				case 0:  // Pathway
					ownQuery.AddFrom( new string[] { "molecular_entities " + tblME,
                                                     "process_entities " + tblPE,
						                             "pathway_processes " + tblPP,
                                                     "pathways " + tblPW } );
					ownQuery.AddWhere( tblME + ".id = " + tblPE + ".entity_id" );
					ownQuery.AddWhere( tblPE + ".process_id = " + tblPP + ".process_id" );
					ownQuery.AddWhere( tblPP + ".pathway_id = " + tblPW + ".id" );
					break;

				case 1:  // Process
					ownQuery.AddFrom( new string[] { "molecular_entities " + tblME,
                                                     "process_entities " + tblPE,
						                             "processes " + tblPR } );
					ownQuery.AddWhere( tblME + ".id = " + tblPE + ".entity_id" );
					ownQuery.AddWhere( tblPE + ".process_id = " + tblPR + ".id" );
					break;

				// case 2 is not permitted			

				case 3:  // Organism
					ownQuery.AddFrom( new string[] { "molecular_entities " + tblME,
                                                     "process_entities " + tblPE,
						                             "catalyzes " + tblC,
                                                     "organism_groups " + tblOG } );
					ownQuery.AddWhere( tblME + ".id = " + tblPE + ".entity_id" );
					ownQuery.AddWhere( tblPE + ".process_id = " + tblC + ".process_id" );
					ownQuery.AddWhere( tblC + ".organism_group_id = " + tblOG + ".id" );
					break;

				default: return;
			}

            ownQuery.MergeWith( childQuery );
		}
	}
}