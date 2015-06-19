#region Using Declarations
using System;
#endregion

namespace PathwaysLib.Utilities
{
    /// <summary>
    /// Represents a boolean type in the database that may also be null.
    /// </summary>
    public enum Tribool
    {
        /// <summary>
        /// The boolean value false.
        /// </summary>
        False = 0,

        /// <summary>
        /// The boolean value true.
        /// </summary>
        True = 1,

        /// <summary>
        /// The value DBNull.
        /// </summary>
        Null = 2
    }

}

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: Tribool.cs,v 1.1 2008/05/16 21:15:58 mustafa Exp $
	$Log: Tribool.cs,v $
	Revision 1.1  2008/05/16 21:15:58  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/10/20 13:59:01  brendan
	Missing files that should have been part of the last graph generation update checkin
	
------------------------------------------------------------------------*/
#endregion