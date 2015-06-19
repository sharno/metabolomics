using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// An exception that is triggered when an unsatisfiable condition is found
    /// </summary>
    public class SatisfiabilityException : Exception
    {
        public SatisfiabilityException(string message) : base(message) { }
    }
}