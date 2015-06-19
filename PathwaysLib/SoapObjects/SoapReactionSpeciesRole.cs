#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion


namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapReactionSpeciesRole")]
    public class SoapReactionSpeciesRole : SoapObject
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapReactionSpeciesRole()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapReactionSpeciesRole(string role)
        {
            // this.id = Guid.Empty; // created on insert into the DB

            this.id = 0;
            this.role = role;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapReactionSpeciesRole()
        {
        }

        #endregion

        #region Member Variables

        short id = 0;
        private string role = null;

        #endregion

        #region Properties


        /// <summary>
        ///  ID.
        /// </summary>
        public short ID
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// role
        /// </summary>
        public string Role
        {
            get { return role; }
            set
            {
                if (role != value)
                {
                    role = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        #endregion

    } // end class

} // end namespace





