using System;
using System.Reflection;
using System.Collections;
using System.Threading;

namespace PathwaysLib.Logging
{
	/// <summary>
    /// Static logger useful for debugging.
    /// 
    /// Originally developed for logging in PicWeb by Brendan Elliott.
    /// </summary>
	public class DebugLogger
	{
		private DebugLogger()
		{
		}

        static Logger log = null;
        //static string applicationName = "Risu PicWebLib3";
        static Hashtable applicationNames = new Hashtable();  // list of instances per thread
        static Hashtable urls = new Hashtable();  // list of instances per thread

        public static string ApplicationName
        {
            get 
            { 
                if (!applicationNames.ContainsKey(Thread.CurrentThread))
                    return "Pathways PathwaysLib";
                return (string)applicationNames[Thread.CurrentThread]; 
            }
            set 
            {
                if (value == null)
                {
                    if (applicationNames.ContainsKey(Thread.CurrentThread))
                        applicationNames.Remove(Thread.CurrentThread);

                    if (urls.ContainsKey(Thread.CurrentThread))
                        urls.Remove(Thread.CurrentThread);
                }
                else
                {
                    applicationNames[Thread.CurrentThread] = value;
                }
            }
        }

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
                }
                else
                {
                    urls[Thread.CurrentThread] = value;
                }
            }
        }

        public static void Start(string applicationName)
        {
            DebugLogger.ApplicationName = applicationName;
        }

        public static void Start(string applicationName, LogTarget t)
        {
            DebugLogger.ApplicationName = applicationName;
            AddLogTarget(t);
        }

        public static void Start(string applicationName, LogTarget[] targets)
        {
            DebugLogger.ApplicationName = applicationName;
            AddLogTarget(targets);
        }

        public static void Stop()
        {
            if (log != null)
            {
                log.Close();
                log = null;
            }

            if (applicationNames.ContainsKey(Thread.CurrentThread))
            {
                applicationNames.Remove(Thread.CurrentThread);
                urls.Remove(Thread.CurrentThread);
            }
        }

        public static void AddLogTarget(LogTarget t)
        {
            if (log == null)
                log = new Logger(t);
            else
                log.AddLogTarget(t);
        }

        public static void AddLogTarget(LogTarget[] targets)
        {
            if (log == null)
                log = new Logger(targets);
            else
            {
                foreach (LogTarget t in targets)
                {
                    log.AddLogTarget(t);
                }
            }
        }

        public static void RemoveLogTarget(LogTarget t)
        {
            if (log != null)
                log.RemoveLogTarget(t);
        }

        public static void RemoveAllTargets()
        {
            if (log != null)
                log.RemoveAllTargets();
        }

        public static void Log(string message)
        {
            if (log == null)
                return;

            log.Log("[DEBUG] " + message);
        }

        public static void Log(string format, params object[] arg)
        {
            if (log == null)
                return;

            log.Log("[DEBUG] " + format, arg);
        }
            
        public static void LogError(string message)
        {
            SystemEventLog("[DEBUG] " + message);

            if (log == null)
                return;

            log.LogError("[DEBUG] " + message);

        }

        public static void LogError(string format, params object[] arg)
        {
            string message = string.Format(format, arg);
            SystemEventLog("[DEBUG] " + message);

            if (log == null)
                return;

            log.LogError("[DEBUG] " + message);

        }

        public static void Close()
        {
            Stop();
        }

        static bool eventLogWrittingDisabled = false;

        public static void SystemEventLog(string message)
        {
            SystemEventLog(ApplicationName, message, System.Diagnostics.EventLogEntryType.Warning);
        }

        public static void SystemEventLog(string applicationName, string message)
        {
            SystemEventLog(applicationName, message, System.Diagnostics.EventLogEntryType.Warning);
        }

        public static void SystemEventLog(string message, System.Diagnostics.EventLogEntryType type)
        {
            SystemEventLog(ApplicationName, message, type);
        }

        public static void SystemEventLog(string applicationName, string message, System.Diagnostics.EventLogEntryType type)
        {
            if (eventLogWrittingDisabled)
            {
                return;
            }

            try
            {
                System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                appLog.Source = "RisuPicWeb";
                appLog.WriteEntry("\n\n" + applicationName + "\n\n" + message, type);
            }
            catch (Exception)
            {
                eventLogWrittingDisabled = true;
            }
        }

    }
}
