using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PathwaysLib.Utilities
{
    public class HighlightKeywords
    {
        public static void HighlightKW(ref string strSource, string strTermKW)
        {
            strSource = Regex.Replace(strSource, strTermKW, "<b><span style=\"color:#8B0000; font-weight:bolder;\">" + "$&" + "</span></b>", RegexOptions.IgnoreCase);
            //strSource = Regex.Replace(strSource, strTermKW, "<b><font color=\"8B0000\" font-weight=\"bolder\">" + "$&" + "</font></b>", RegexOptions.IgnoreCase);
        }

    }
}
