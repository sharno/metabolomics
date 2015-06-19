#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion


namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapInitialAssignment")]
    public class SoapInitialAssignment : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapInitialAssignment()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapInitialAssignment(string metaId, string sboTerm, string notes, string annotation, Guid modelId, string symbol, string math)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class
            this.modelId = modelId;
            this.symbol = symbol;
            this.math = math;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapInitialAssignment()
        {
        }

        #endregion

        #region Member Variables

        Guid id = Guid.Empty;
        private Guid modelId = Guid.Empty;
        private string symbol = null;
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
        /// modelId
        /// </summary>
        public Guid ModelId
        {
            get { return modelId; }
            set
            {
                if (modelId != value)
                {
                    modelId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// symbol
        /// </summary>
        public string Symbol
        {
            get { return symbol; }
            set
            {
                if (symbol != value)
                {
                    symbol = value;
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




