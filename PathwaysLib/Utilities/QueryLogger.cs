using System;
using System.Configuration;
using System.IO;

namespace PathwaysLib.Utilities
{
	/// <summary>
	/// Logs DBWrapper queries, their calling functions, and the time they take.
	/// </summary>
	public class QueryLogger
	{
		private static string filename = string.Empty;
		private static string delimeter = string.Empty;
		private static DateTime timer;
		private static StreamWriter logger;
		private static bool timing = false;
		private static bool open = false;
		private static string currenturl = string.Empty;

		/// <summary>
		/// Calling URL
		/// </summary>
		public static string CurrentURL
		{
			get {return currenturl;}
			set {currenturl = value;}
		}

		/// <summary>
		/// Default constructor; not really useful
		/// </summary>
		public QueryLogger() {}

		/// <summary>
		/// Opens a log for writing.
		/// </summary>
		/// <returns>Whether the open was successful</returns>
		public static bool OpenLog()
		{
			if( open ) return true;

			try
			{
				filename = ConfigurationManager.AppSettings["QueryLoggerFile"];
				delimeter = ConfigurationManager.AppSettings["QueryLoggerDelimeter"];
				logger = new StreamWriter( new FileStream( filename, FileMode.Append, FileAccess.Write, FileShare.Write ) );
			}
			catch( Exception e )
			{
				throw new Exception( "Query logger failed to start: ", e );
			}

			logger.AutoFlush = true;
			open = true;

			return true;
		}

		/// <summary>
		/// Closes an open log.
		/// </summary>
		public static void CloseLog()
		{
			if( open ) logger.Close();
			open = false;
			timing = false;
		}

		/// <summary>
		/// Begin the log timer.
		/// </summary>
		public static void StartTimer()
		{
			timer = DateTime.Now;
			timing = true;
		}

		/// <summary>
		/// Stop the timer and update the logfile.
		/// </summary>
		/// <param name="function">The calling function</param>
		/// <param name="sql">The SQL statement executed</param>
		public static void UpdateLog( string function, string sql )
		{
			OpenLog();

			if( !timing ) StartTimer();

			TimeSpan elapsed = DateTime.Now.Subtract( timer );
			timing = false;

			sql = sql.Replace("\n", " ").Replace("\t", "").Replace("\r", "");

			logger.WriteLine( "{1}{0}{2}{0}{3}{0}{4}{0}{5}", delimeter, DateTime.Now, currenturl, function, sql, elapsed.TotalMilliseconds );

			CloseLog();
		}
	}
}