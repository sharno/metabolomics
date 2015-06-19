using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.ServerObjects
{
    public class ServerECNodeCode : ServerNodeCode
    {

        #region Constructor, Destructor, ToString
        /// <summary>
        /// Constructor for server node code
        /// </summary>
        /// <param name="ecNumber"></param>
        /// <param name="name"></param>
        /// <param name="notes"></param>
        public ServerECNodeCode(string id, string nodeCode):base(id, nodeCode)
        {
            SetSchemaParameters("ec_numbers", "ec_number", "nodeCode");             
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
        public ServerECNodeCode(DBRow data):base(data)
        {
            SetSchemaParameters("ec_numbers", "ec_number", "nodeCode"); 
        }

        public ServerECNodeCode()         
        {
            SetSchemaParameters("ec_numbers", "ec_number", "nodeCode"); 
        }

        /// <summary>
        /// Destructor for the ServerECNumber class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerECNodeCode()
        {
        }
        #endregion


        #region Member Variables
        public static ServerECNodeCode Instance = new ServerECNodeCode();
        
        #endregion
    }
}
