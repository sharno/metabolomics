#region Using Declarations
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using PathwaysLib.SoapObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
#endregion

namespace PathwaysLib.ServerObjects
{
    public class ServerGONodeCode : ServerNodeCode
    {

      #region Constructor, Destructor, ToString
        /// <summary>
        /// Constructor for server node code
        /// </summary>
        /// <param name="ecNumber"></param>
        /// <param name="name"></param>
        /// <param name="notes"></param>
        public ServerGONodeCode(string id, string nodeCode):base(id, nodeCode)
        {
            SetSchemaParameters("goNodeCodes", "goId", "nodeCode");
             
        }



        /// <summary>
        /// Constructor for server node code
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerNodeCode object from a
        /// DBRow.
        /// </remarks>
        /// <param name="data">
        /// DBRow to load into the object.
        /// </param>
        public ServerGONodeCode(DBRow data):base(data)
        {
            SetSchemaParameters("goNodeCodes", "goId", "nodeCode");
        }

        public ServerGONodeCode()
        {
            SetSchemaParameters("goNodeCodes", "goId", "nodeCode");
        }

        /// <summary>
        /// Destructor for the ServerECNumber class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerGONodeCode()
        {
        }
        #endregion


        #region Member Variables
        public static ServerGONodeCode Instance = new ServerGONodeCode();
       
        #endregion

    } // End class

}
