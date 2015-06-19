using System;

namespace PathwaysLib.ServerObjects
{
    /// <summary>
    /// Enum used by many search functions to determine what type of search to use.
    /// </summary>
    public enum SearchMethod
    {
        /// <summary>
        /// Return results where the string CONTAINS a value (i.e. y LIKE '%x%')
        /// </summary>
        Contains,

        /// <summary>
        /// Return results where the string ENDS WITH a value (i.e. y LIKE 'x%')
        /// </summary>
        EndsWith,

        /// <summary>
        /// Return results where the string STARTS WITH a value (i.e. y LIKE '%x')
        /// </summary>
        StartsWith,

        /// <summary>
        /// Return results where the string matches exactly (i.e. y = x)
        /// </summary>
        ExactMatch
    }


    public enum SimulationInputMode
    {
        BioModels, UserUpload
    }

}
