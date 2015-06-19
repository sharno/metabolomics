using System;

namespace PathwaysLib.Exceptions
{
	/// <summary>
	/// An exception class for exceptions thrown by AjaxMethod
	/// </summary>
	public class AjaxMethodException : Exception
	{
		/// <summary>
		/// An exception thrown by an AjaxMethod
		/// </summary>
		public AjaxMethodException() : base()
		{
		}

		/// <summary>
		/// An exception thrown by an AjaxMethod method
		/// </summary>
		/// <param name="message">The message to pass</param>
		public AjaxMethodException(string message) : base(message)
		{
		}

		/// <summary>
		/// An exception thrown by an AjaxMethod method
		/// </summary>
		/// <param name="message">The message to pass</param>
		/// <param name="innerException">The calling exception</param>
		public AjaxMethodException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// An exception thrown by an AjaxMethod method
		/// </summary>
		/// <param name="message">The message to pass</param>
		/// <param name="args">Arguments to send along with the exception</param>
		public AjaxMethodException(string message, params object[] args)
			: base(string.Format(message, args))
		{
		}
	}
}