#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapUnit")]
    public class SoapUnit : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapUnit()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapUnit(string metaId, string sboTerm, string notes, string annotation, Guid modelId, Guid kind, int exponent, int scale, double multiplier)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class
            this.modelId = modelId;
            this.kind = kind;
            this.exponent = exponent;
            this.scale = scale;
            this.multiplier = multiplier;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapUnit()
        {
        }

        #endregion

        #region Member Variables

        Guid id = Guid.Empty;
        private Guid modelId = Guid.Empty;
        private Guid kind = Guid.Empty;
        private int exponent = 0;
        private int scale = 0;
        private double multiplier = 0;

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
        /// kind
        /// </summary>
        public Guid Kind
        {
            get { return kind; }
            set
            {
                if (kind != value)
                {
                    kind = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// exponent 
        /// </summary>
        public int Exponent
        {
            get { return exponent; }
            set
            {
                if (exponent != value)
                {
                    exponent = value;
                    Status = ObjectStatus.Update;
                }
            }
        }


        /// <summary>
        /// scale 
        /// </summary>
        public int Scale 
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    scale = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// multiplier
        /// </summary>
        public double Multiplier
        {
            get { return multiplier; }
            set
            {
                if (multiplier != value)
                {
                    multiplier = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        #endregion

    } // end class

} // end namespace



