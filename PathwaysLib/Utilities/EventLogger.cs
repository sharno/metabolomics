using System;
using System.Collections;
using System.Threading;

namespace PathwaysLib.Utilities
{
	/// <summary>
	/// Summary description for EventLogger.
	/// </summary>
	public class EventLogger
	{
		private EventLogger()
		{
		}

		static Hashtable componentNames = new Hashtable();  // list of instances per thread
		static Hashtable urls = new Hashtable();  // list of instances per thread

		/// <summary>
		/// Get/set the calling component
		/// </summary>
		public static string ComponentName
		{
			get 
			{ 
				if (!componentNames.ContainsKey(Thread.CurrentThread))
					return "PathCase (Unspecified)";
				return (string)componentNames[Thread.CurrentThread]; 
			}
			set 
			{
				if (value == null)
				{
					if (componentNames.ContainsKey(Thread.CurrentThread))
						componentNames.Remove(Thread.CurrentThread);

					if (urls.ContainsKey(Thread.CurrentThread))
						urls.Remove(Thread.CurrentThread);
				}
				else
				{
					componentNames[Thread.CurrentThread] = value;
				}
			}
		}

		/// <summary>
		/// Get/set the calling URL
		/// </summary>
		public static string Url
		{
			get
			{
				if (!urls.ContainsKey(Thread.CurrentThread))
					return "?";
				return (string)urls[Thread.CurrentThread];
			}
			set
			{
				if (value == null)
				{
					if (urls.ContainsKey(Thread.CurrentThread))
						urls.Remove(Thread.CurrentThread);

					if (componentNames.ContainsKey(Thread.CurrentThread))
						componentNames.Remove(Thread.CurrentThread);
				}
				else
				{
					urls[Thread.CurrentThread] = value;
				}
			}
		}

		static bool eventLogWrittingDisabled = false;

		/// <summary>
		/// Writes to the event log
		/// </summary>
		/// <param name="message"></param>
		public static void SystemEventLog(string message)
		{
			SystemEventLog(ComponentName, message, System.Diagnostics.EventLogEntryType.Warning);
		}

		/// <summary>
		/// Writes to the event log
		/// </summary>
		/// <param name="componentName"></param>
		/// <param name="message"></param>
		public static void SystemEventLog(string componentName, string message)
		{
			SystemEventLog(componentName, message, System.Diagnostics.EventLogEntryType.Warning);
		}

		/// <summary>
		/// Writes to the event log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="type"></param>
		public static void SystemEventLog(string message, System.Diagnostics.EventLogEntryType type)
		{
			SystemEventLog(ComponentName, message, type);
		}

		/// <summary>
		/// Writes to the event log
		/// </summary>
		/// <param name="componentName"></param>
		/// <param name="message"></param>
		/// <param name="type"></param>
		public static void SystemEventLog(string componentName, string message, System.Diagnostics.EventLogEntryType type)
		{
			if (eventLogWrittingDisabled)
			{
				return;
			}

			try
			{
				System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
				appLog.Source = "PathCase";
				appLog.WriteEntry("\n\n" + componentName + "\n\n" + message, type);
			}
			catch (Exception)
			{
				eventLogWrittingDisabled = true;
			}
		}
	}
}
