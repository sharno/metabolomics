using System;

namespace PathwaysLib.Exceptions
{
	/// <summary>
	/// An exception class for exceptions thrown by LinkHelper methods
	/// </summary>
	public class LinkException : PathwayException 
	{

		#region Constructors

		/// <summary>
		/// Default constructor with no additional description.
		/// </summary>
		public LinkException() : base()
		{
		}
        
		/// <summary>
		/// Create an exception with the following detailed description.
		/// </summary>
		/// <param name="message"></param>
		public LinkException(string message) : base(message)
		{
		}

		/// <summary>
		/// Create an exception with the following detailed description from a format string.
		/// </summary>
		/// <param name="messageFormat"></param>
		/// <param name="args"></param>
		public LinkException(string messageFormat, params object[] args) : base(string.Format(messageFormat, args))
		{
		}

		/// <summary>
		/// Create an exception with the following detailed description that wraps another exception. 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public LinkException(string message, Exception innerException) : base(message, innerException)
		{
		}

		#endregion
	}

}