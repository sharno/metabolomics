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
	/// A process AQI node.
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QNodeAQIProcess : QNode
    {
        public override string Title { get { return "Process"; } }
        public override QNodeType Type { get { return QNodeType.AQINode; } }
        public override string[] AddTip
        {
            get
            {
                return new string[] {
			"A pathway in which this process takes place",
			"Cut it out.",
			"A molecular entity involved in this process",
			"An organism in which this process takes place" };
            }
        }

        /// <summary>
        /// Default constructor; sets up parameters
        /// </summary>
        public QNodeAQIProcess(string id, string name)
            : base(id, name)
        {
            Parameters.Add(new QParam("Name:", new QFieldProcessName(string.Empty)));
            Parameters.Add(new QParam("Reversible:", new QFieldProcessReversible(string.Empty)));
        }

        public override void JoinWith( ref QueryBuilder ownQuery, QueryBuilder childQuery, int parentType )
        {
            string parentNodeID = ownQuery.NodeID;
            string childNodeID = childQuery.NodeID;

            string tblPR = "PR" + parentNodeID;
            string tblC  = "C"  + parentNodeID + "_" + childNodeID;
            string tblPP = "PP" + parentNodeID + "_" + childNodeID;
            string tblME = "ME" + childNodeID;
            string tblOG = "OG" + childNodeID;
            string tblPE = "PE" + childNodeID;
            string tblPW = "PW" + childNodeID;

            switch ( parentType )
            {
                case 0:  // Pathway
                    ownQuery.AddFrom( new string[] { "processes " + tblPR,
                                                     "pathway_processes " + tblPP,
						                             "pathways " + tblPW } );
                    ownQuery.AddWhere( tblPR + ".id = " + tblPP + ".process_id" );
                    ownQuery.AddWhere( tblPP + ".pathway_id = " + tblPW + ".id" );
                    break;

                // case 1 is not permitted

                case 2:  // Molecule
                    ownQuery.AddFrom( new string[] { "processes " + tblPR,
                                                     "process_entities " + tblPE,
						                             "molecular_entities " + tblME } );
                    ownQuery.AddWhere( tblPR + ".id = " + tblPE + ".process_id" );
                    ownQuery.AddWhere( tblPE + ".entity_id = " + tblME + ".id" );
                    break;

                case 3:  // Organism
                    ownQuery.AddFrom( new string[] { "processes " + tblPR,
                                                     "catalyzes " + tblC,
						                             "organism_groups " + tblOG } );
                    ownQuery.AddWhere( tblPR + ".id = " + tblC + ".process_id" );
                    ownQuery.AddWhere( tblC + ".organism_group_id = " + tblOG + ".id" );
                    break;

                default: return;
            }

            ownQuery.MergeWith( childQuery );
        }
    }
}