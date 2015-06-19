using System;
using System.IO;

namespace PathwaysLib.Logging
{
    /// <summary>
    /// Interface for writing to a log of some kind.
    /// 
    /// Originally developed for logging in PicWeb by Brendan Elliott.
    /// </summary>
    public interface LogTarget
    {
        void WriteLine(string format, params object[] arg);

        void Flush();

        void Close();

        bool Closed
        {
            get;
        }
    }

    /// <summary>
    /// Logs to a TextWriter (typically a file)
    /// 
    /// Originally developed for logging in PicWeb by Brendan Elliott.
    /// </summary>
    public class TextWriterLog : LogTarget
    {
        TextWriter log;
        bool closed = false;

        public TextWriterLog(TextWriter o)
        {
            log = o;
        }

        public void WriteLine(string format, params object[] arg)
        {
			if (!closed)
				log.WriteLine(format, arg);
        }

        public void Flush()
        {
			if (!closed)
				log.Flush();
        }

        public void Close()
        {
            if (log == null)
                return; 
            if (closed)
                return;
            closed = true;
            try
            {
                log.Close();
            }
            catch(Exception)
            {
            }
        }

        public bool Closed
        {
            get {return closed;}
        }
    }

    /// <summary>
    /// Logs to a file.
    /// 
    /// Originally developed for logging in PicWeb by Brendan Elliott.
    /// </summary>
    public class FileLog : TextWriterLog
    {
        public FileLog(string logFile) : base(new StreamWriter(new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.Write)))
        {
        }
    }

    /// <summary>
    /// Logs to the console.
    /// 
    /// Originally developed for logging in PicWeb by Brendan Elliott.
    /// </summary>
    public class ConsoleLog : LogTarget
    {
        public ConsoleLog()
        {
        }

        public void WriteLine(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
        }

        public void Flush()
        {
        }

        public void Close()
        {
        }

        public bool Closed
        {
            get {return false;}
        }
    }
}
