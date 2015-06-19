#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion


namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapRuleType")]
    public class SoapRuleType : SoapObject
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapRuleType()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapRuleType(string type)
        {
           // this.id = Guid.Empty; // created on insert into the DB

            this.id = 0;
            this.type = type;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapRuleType()
        {
        }

        #endregion

        #region Member Variables

        short id = 0;
        private string type = null;

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
        /// type
        /// </summary>
        public string Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        #endregion

    } // end class

} // end namespace




