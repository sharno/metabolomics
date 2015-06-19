#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapRule")]
    public class SoapRule : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapRule()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapRule(string metaId, string sboTerm, string notes, string annotation, Guid modelId, string variable, string math, short ruleTypeId)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class
            this.modelId = modelId;
            this.variable = variable;
            this.math = math;
            this.ruleTypeId = ruleTypeId;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapRule()
        {
        }

        #endregion

        #region Member Variables

        Guid id = Guid.Empty;
        private Guid modelId = Guid.Empty;
        private string variable = null;
        private string math = null;
        private short ruleTypeId = 0;

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
        /// variable
        /// </summary>
        public string Variable
        {
            get { return variable; }
            set
            {
                if (variable != value)
                {
                    variable = value;
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

        /// <summary>
        /// ruleTypeId
        /// </summary>
        public  short RuleTypeId
        {
            get { return ruleTypeId; }
            set
            {
                if (ruleTypeId != value)
                {
                    ruleTypeId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        #endregion

    } // end class

} // end namespace




