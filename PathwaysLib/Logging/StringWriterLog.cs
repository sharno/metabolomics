using System;
using System.Text;

namespace PathwaysLib.Logging
{
	/// <summary>
    /// Logs to a string, useful for output to web pages.
    /// 
	/// Originally developed for logging in PicWeb by Brendan Elliott.
	/// </summary>
    public class StringBuilderLog : PathwaysLib.Logging.LogTarget
	{
        StringBuilder sb = new StringBuilder();
        bool closed = false;

        string lineBreak = "\r\n";

        public StringBuilderLog()
        {
        }

		public StringBuilderLog(string lineBreak)
		{
            this.lineBreak = lineBreak;
        }

        #region LogTarget Members

        public void WriteLine(string format, params object[] arg)
        {
            if (!closed)
            {
                sb.Append(string.Format(format, arg));
                sb.Append(lineBreak);
            }
        }

        public void Flush()
        {
        }

        public void Close()
        {
            closed = true;
        }

        public bool Closed
        {
            get
            {
                return closed;
            }
        }

        #endregion

        public override string ToString()
        {
            return sb.ToString();
        }

    }
}
