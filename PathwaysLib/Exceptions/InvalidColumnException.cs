using System;

namespace PathwaysLib.Exceptions
{
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>/PathwaysLib/Exceptions/InvalidColumnException.cs</filepath>
    ///		<creation>2005-06-15</creation>
    ///		<author>
    ///			<name>Brendan Elliott</name>
    ///			<initials>BE</initials>
    ///			<email>bxe7@cwru.edu</email>
    ///		</author>
    ///		<contributors>
    ///			<contributor>
    ///				<name>none</name>
    ///				<initials>none</initials>
    ///				<email>none</email>
    ///			</contributor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: mustafa $</cvs_author>
    ///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/Exceptions/InvalidColumnException.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Thrown by the DBWrapper class to indicate that the user-request column in DBRow does not exist.
    /// </summary>
    #endregion    
    public class InvalidColumnException : DBWrapperException 
    {

        #region Constructors

        /// <summary>
        /// Default constructor with no additional description.
        /// </summary>
        public InvalidColumnException() : base()
        {
        }
        
        /// <summary>
        /// Create an exception with the following detailed description.
        /// </summary>
        /// <param name="message"></param>
        public InvalidColumnException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create an exception with the following detailed description from a format string.
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <param name="args"></param>
        public InvalidColumnException(string messageFormat, params object[] args) : base(messageFormat, args)
        {
        }

        /// <summary>
        /// Create an exception with the following detailed description that wraps another exception. 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public InvalidColumnException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }

}

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: InvalidColumnException.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: InvalidColumnException.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.2  2005/06/24 21:57:32  brendan
	Checkin of work on database object design.
	
	Revision 1.1  2005/06/22 22:02:24  brendan
	Added InvalidColumnException and added a format string constructor to the exception classes.
	
------------------------------------------------------------------------*/
#endregion