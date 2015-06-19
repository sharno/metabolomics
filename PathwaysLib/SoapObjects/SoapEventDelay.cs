#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion


namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapEventDelay")]
    public class SoapEventDelay : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapEventDelay()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapEventDelay(string metaId, string sboTerm, string notes, string annotation, string math)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class
            this.math = math;

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapEventDelay()
        {
        }

        #endregion

        #region Member Variables

        Guid id = Guid.Empty;
        private string math = null;

        #endregion

        #region Properties


        /// <summary>
        /// Compartment ID.
        /// </summary>
        public override Guid ID
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    //set base class's ID as well
                    base.ID = value;
                    id = value;
                    Status = ObjectStatus.Update;
                }
            }
        }


        /// <summary>
        /// math
        /// </summary>
        public string Math
        {
            get { return math; }
            set
            {
                if (math != value)
                {
                    math = value;
                    Status = ObjectStatus.Update;
                }
            }
        }


        #endregion

    } // end class

} // end namespace




