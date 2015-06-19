#region Using Declarations
using System;
using PathwaysLib.Exceptions;
#endregion

namespace PathwaysLib.SoapObjects
{
	/// <summary>
	/// The return type for handling client-side error messages.
	/// </summary>
    public enum ReturnType
    {
		/// <summary>
		/// unknown type.
		/// </summary>
        Unknown,

		/// <summary>
		/// Successful return.
		/// </summary>
        Success = 0,
		/// <summary>
		/// Unsuccessful return.
		/// </summary>
        Failure = 1,

		/// <summary>
		/// Requires login.
		/// </summary>
        LoginRequired,

		/// <summary>
		/// Exception thrown.
		/// </summary>
        Exception,
		/// <summary>
		/// Pathway Exception thrown.
		/// </summary>
        PathwayException,
		/// <summary>
		/// Database exception thrown.
		/// </summary>
        DBWrapperException
    }

    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/SoapObjects/ReturnValue.cs</filepath>
    ///		<creation>2005/06/16</creation>
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
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/SoapObjects/ReturnValue.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Packages additional information to help the client application
    /// present error messages to the user.
    /// </summary>
    #endregion    
    public class ReturnValue
    {
        #region Constructor

		/// <summary>
		/// ReturnValue constructor
		/// </summary>
		/// <param name="type">
		/// The return type.
		/// </param>
        public ReturnValue(ReturnType type)
        {
            this.type = type;
            this.message = null;
        }

		/// <summary>
		/// ReturnValue constructor.
		/// </summary>
		/// <param name="type">
		/// The return type.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
        public ReturnValue(ReturnType type, string message)
        {
            this.type = type;
            this.message = message;
        }

        #endregion

        #region Member Variables

        private ReturnType type;
        private string message;

        #endregion

        #region Properties

		/// <summary>
		/// Get/set the return type.
		/// </summary>
        public ReturnType Type
        {
            get {return type;}
            set {type = value;}
        }

		/// <summary>
		/// Get/set the returned message.
		/// </summary>
        public string Message
        {
            get {return message;}
            set {message = value;}
        }

        #endregion

        #region Static Methods and Properties

		/// <summary>
		/// Return a success message.
		/// </summary>
        public static ReturnValue Success
        {
            get
            {
                return new ReturnValue(ReturnType.Success);
            }
        }

		/// <summary>
		/// Return a LoginRequired message.
		/// </summary>
        public static ReturnValue LoginRequired
        {
            get
            {
                return new ReturnValue(ReturnType.LoginRequired);
            }
        }

		/// <summary>
		/// Create a return value based on an exception.
		/// </summary>
		/// <param name="e">
		/// The source exception.
		/// </param>
		/// <returns>
		/// ReturnValue to be sent to the user.
		/// </returns>
        public static ReturnValue Exception(Exception e)
        {
            if (e is PathwayException)
                return new ReturnValue(ReturnType.PathwayException, e.ToString());
            else if (e is DBWrapperException)
                return new ReturnValue(ReturnType.DBWrapperException, e.ToString());

            return new ReturnValue(ReturnType.Exception, e.ToString());
        }

        #endregion
    }
}

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ReturnValue.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: ReturnValue.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.2  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.1  2005/06/24 21:57:32  brendan
	Checkin of work on database object design.
	
------------------------------------------------------------------------*/
#endregion