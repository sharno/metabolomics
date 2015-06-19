using System;

namespace PathwaysLib.Exceptions
{
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>/PathwaysLib/Exceptions/DataModelException.cs</filepath>
    ///		<creation>2005-06-10</creation>
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
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/Exceptions/DataModelException.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Thrown when an exceptional condition occurs in the data model code (such as from ServerObjects).
    /// </summary>
    #endregion    
    public class DataModelException : PathwayException 
    {

        #region Constructors

        /// <summary>
        /// Default constructor with no additional description.
        /// </summary>
        public DataModelException() : base()
        {
        }
        
        /// <summary>
        /// Create an exception with the following detailed description.
        /// </summary>
        /// <param name="message"></param>
        public DataModelException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create an exception with the following detailed description from a format string.
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <param name="args"></param>
        public DataModelException(string messageFormat, params object[] args) : base(messageFormat, args)
        {
        }

        /// <summary>
        /// Create an exception with the following detailed description that wraps another exception. 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public DataModelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }

}

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: DataModelException.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: DataModelException.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.3  2005/06/24 21:57:32  brendan
	Checkin of work on database object design.
	
	Revision 1.2  2005/06/22 22:02:24  brendan
	Added InvalidColumnException and added a format string constructor to the exception classes.
	
	Revision 1.1  2005/06/10 21:28:32  brendan
	*** empty log message ***
			
------------------------------------------------------------------------*/
#endregion