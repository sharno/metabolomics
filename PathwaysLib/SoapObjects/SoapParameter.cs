#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapParameter")]
    public class SoapParameter : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapParameter()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapParameter(string metaId, string sboTerm, string notes, string annotation, Guid modelId, Guid reactionId, string sbmlId, string name, double pvalue, Guid unitsId, bool constant)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class

            this.modelId = modelId;
            this.reactionId = reactionId;
            this.sbmlId = sbmlId;
            this.name = name;
            this.pvalue = pvalue;
            this.unitsId = unitsId;
            this.constant = constant;
        }


        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapParameter()
        {
        }

        #endregion

        #region Member Variables
        Guid id = Guid.Empty;
        private Guid modelId = Guid.Empty;
        private Guid reactionId = Guid.Empty;
        private string sbmlId = null;
        private string name = null;
        private double pvalue = 0;
        private Guid unitsId = Guid.Empty;
        private bool constant = true;

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
        /// reactionId
        /// </summary>
        public Guid ReactionId
        {
            get { return reactionId; }
            set
            {
                if (reactionId != value)
                {
                    reactionId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// sbmlId 
        /// </summary>
        public string SbmlId
        {
            get { return sbmlId; }
            set
            {
                if (sbmlId != value)
                {
                    sbmlId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

      
        /// <summary>
        /// value
        /// </summary>
        public double Value
        {
            get { return pvalue; }
            set
            {
                if (pvalue != value)
                {
                    pvalue = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// unitsId
        /// </summary>
        public Guid UnitsId
        {
            get { return unitsId; }
            set
            {
                if (unitsId != value)
                {
                    unitsId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// constant
        /// </summary>
        public bool Constant
        {
            get { return constant; }
            set
            {
                if (constant != value)
                {
                    constant = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        #endregion

    } // end class

} // end namespace



