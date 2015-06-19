using System;

namespace PathwaysLib.Exceptions
{
	/// <summary>
	/// An exception class for exceptions thrown by graph-related methods
	/// </summary>
	public class GraphException : PathwayException 
	{
		#region Constructors

		/// <summary>
		/// Default constructor with no additional description.
		/// </summary>
		public GraphException() : base()
		{
		}
        
		/// <summary>
		/// Create an exception with the following detailed description.
		/// </summary>
		/// <param name="message"></param>
		public GraphException(string message) : base(message)
		{
		}

		/// <summary>
		/// Create an exception with the following detailed description from a format string.
		/// </summary>
		/// <param name="messageFormat"></param>
		/// <param name="args"></param>
		public GraphException(string messageFormat, params object[] args) : base(string.Format(messageFormat, args))
		{
		}

		/// <summary>
		/// Create an exception with the following detailed description that wraps another exception. 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public GraphException(string message, Exception innerException) : base(message, innerException)
		{
		}

		#endregion
	}
}