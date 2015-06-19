using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// An exception thrown when data is invalid
    /// </summary>
    public class DataValidationException : Exception
    {
        public DataValidationException(string message) : base(message) { }
    }
}