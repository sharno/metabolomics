using System;

namespace PathwaysLib.Logging
{
	/// <summary>
    /// Originally developed for logging in PicWeb by Brendan Elliott.
    /// </summary>
    public class LogException : Exception
    {
        public LogException(string message) : base(message)
        {
            DebugLogger.Log(this.GetType().Name + ": " + message);
        }

        public LogException(string message, Exception innerException) : base(message, innerException)
        {
            DebugLogger.Log(this.GetType().Name + ": " + message);
        }

    }
}
