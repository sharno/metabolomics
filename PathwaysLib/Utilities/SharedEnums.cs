using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.Utilities
{
    #region Graph Layout

    public enum LayoutAlgorithm
    {
        Hierarchical,
        Symmetric,
        Circular,
        Orthogonal,
        Tree
    }

    public enum MoleculeDisplayType
    {
        //NoMolecules, //TODO: implement (needs edits in AddIProcess & AddCollapsedPathway)
        NoCommonMolecules,
        AllMolecules
    }

    #endregion


    #region Compose Models
    public enum AttributeTypeEnum
    {
        SubstanceUnits = 0,
        InitialAmount = 1,
        ParameterValue = 2,
        ReactionFast = 3,
        ReactionReversible = 4,
        CompartmentUnits = 5,
        CompartmentSize = 6,
        Stoichiometry = 7,
        Value = 8,
        Separator = 9,
        Rename = 10,
    }

    #endregion

    #region reaction matching
    public enum ReactionSpeciesRoleEnum
    {
        substrate = 1,
        product = 2,
        modifier = 3
    }
    #endregion
}

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: SharedEnums.cs,v 1.1 2008/05/16 21:15:58 mustafa Exp $
	$Log: SharedEnums.cs,v $
	Revision 1.1  2008/05/16 21:15:58  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/08/10 20:54:25  brendan
	Updated graph drawing interface to provide more layouts and ability to hide/show common molecules, regulator details, and cofactors.
	
------------------------------------------------------------------------*/
#endregion