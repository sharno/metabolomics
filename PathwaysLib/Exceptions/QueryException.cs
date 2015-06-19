using System;

namespace PathwaysLib.Exceptions
{
	/// <summary>
	/// An exception class for exceptions thrown by built-in queries
	/// </summary>
	public class QueryException : Exception
	{
		/// <summary>
		/// An exception thrown by a built-in query
		/// </summary>
		public QueryException() : base()
		{
		}

		/// <summary>
		/// An exception thrown by a built-in query
		/// </summary>
		/// <param name="message">The message to pass</param>
		public QueryException(string message) : base(message)
		{
		}

		/// <summary>
		/// An exception thrown by a built-in query
		/// </summary>
		/// <param name="message">The message to pass</param>
		/// <param name="innerException">The calling exception</param>
		public QueryException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// An exception thrown by a built-in query
		/// </summary>
		/// <param name="message">The message to pass</param>
		/// <param name="args">Arguments to send along with the exception</param>
		public QueryException( string message, params object[] args )
			: base(string.Format(message, args))
		{
		}
	}
}