using System;
using System.IO;
using System.Collections;

namespace PathwaysLib.Logging
{
	/// <summary>
	/// Basic logging class, allowing for multiple log targets to write to.
    /// 
    /// Originally developed for logging in PicWeb by Brendan Elliott.
    /// </summary>
	public class Logger
	{
//		TextWriter log;
//		TextWriter log2 = null;
		public static readonly string DateFormat = "yyyy-MM-dd HH:mm:ss.fff";	
		bool closed = false;

        ArrayList targets = new ArrayList();

		public Logger(string logFile)
		{
			//log = new StreamWriter(new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.Write));
            this.targets.Add(new ConsoleLog());
            this.targets.Add(new FileLog(logFile));
		}

		public Logger(TextWriter o)
		{
			//log = o;
            this.targets.Add(new ConsoleLog());
            this.targets.Add(new TextWriterLog(o));
		}
		
//		public Logger(string logFile, TextWriter secondaryLog)
//		{
//			log = new StreamWriter(new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.Write));
//            log2 = secondaryLog;
//		}
        public Logger(LogTarget target)
        {
            this.targets.Add(target);
        }

        public Logger(LogTarget target, params LogTarget[] moreTargets)
        {
            this.targets.Add(target);

            if (moreTargets != null && moreTargets.Length > 0)
            {
                targets.AddRange(moreTargets);
            }
        }

        public Logger(LogTarget[] targets)
        {
            this.targets.AddRange(targets);
        }

		~Logger()
		{
			Close();
		}

        public void AddLogTarget(LogTarget t)
        {
            lock(targets)
            {
                targets.Add(t);
            }
        }

        public void RemoveLogTarget(LogTarget t)
        {
            lock(targets)
            {
                targets.Remove(t);
            }
        }

        public void RemoveAllTargets()
        {
            lock(targets)
            {
                targets.Clear();
            }
        }

        public void RemoveClosedTargets()
        {
            lock(targets)
            {
                for(int i = targets.Count - 1; i >= 0; i--)
                {
                    LogTarget t = (LogTarget)targets[i];
                    if (t.Closed)
                    {
                        targets.RemoveAt(i);
                    }
                }
            }
        }

		public void Log(string message)
		{
            lock(targets)
            {
			    if (closed)
				    throw new LogException("Trying to write to a closed log!");

                foreach(LogTarget t in targets)
                {
                    try
                    {
                        t.WriteLine("[{0}] {1}", DateTime.Now.ToString(Logger.DateFormat), message);
                        t.Flush();
                    }
                    catch(FormatException)
                    {
                        t.WriteLine("LOG ERROR: INVALID MESSAGE FORMAT (bad characters?)");
                    }
                }
            }

//            Console.WriteLine("[" + DateTime.Now.ToString(Logger.DateFormat) + "] " + message);
//			  log.WriteLine("[" + DateTime.Now.ToString(Logger.DateFormat) + "] " + message);
//			  log.Flush();
//
//            if (log2 != null)
//            {
//			      log2.WriteLine("[" + DateTime.Now.ToString(Logger.DateFormat) + "] " + message);
//		          log2.Flush();
//            }
		}

		public void Log(string format, params object[] arg)
		{
			if (arg.Length > 0)
				Log(string.Format(format, arg));
			else
				Log(format);
		}

		public void LogError(string message)
		{
			Log("*** ERROR: " + message);
		}

		public void LogError(string format, params object[] arg)
		{
			if (arg.Length > 0)
				LogError(string.Format(format, arg));
			else
				LogError(format);
		}

		public void Close()
		{
			if (closed) return;

			closed = true;

            lock(targets)
            {
                foreach(LogTarget t in targets)
                {
                    t.Close();
                }
                targets.Clear();
            }

//			log.Flush();
//			log.Close();
//
//            if (log2 != null)
//            {
//                log2.Flush();
//                log2.Close();
//            }

		}

        // static System logger 

        private static Logger sysLog = null;
        public static Logger SysLog
        {
            get {return sysLog;}
            set {sysLog = value;}
        }
	}
}
