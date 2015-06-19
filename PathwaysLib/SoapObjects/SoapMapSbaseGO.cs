#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapMapSbaseGO")]
    public class SoapMapSbaseGO : SoapObject
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapMapSbaseGO()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapMapSbaseGO(Guid sbaseId, string goId, short qualifierId)
        {
            this.sbaseId = Guid.Empty;
            this.goId = null;
            this.qualifierId = 0;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapMapSbaseGO()
        {
        }

        #endregion

        #region Member Variables
        private Guid sbaseId = Guid.Empty;
        private string goId = null;
        private short qualifierId = 0;

        #endregion

        #region Properties

        public Guid SbaseId
        {
            get { return sbaseId; }
            set
            {
                if (sbaseId != value)
                {
                    sbaseId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        public string GoId
        {
            get { return goId; }
            set
            {
                if (goId != value)
                {
                    goId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }
      

        public short QualifierId
        {
            get { return qualifierId; }
            set
            {
                if (qualifierId != value)
                {
                    qualifierId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        #endregion

    } // end class

} // end namespace

