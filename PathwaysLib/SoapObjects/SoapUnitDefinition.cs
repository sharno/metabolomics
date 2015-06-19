#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapUnitDefinition")]
    public class SoapUnitDefinition : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapUnitDefinition()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapUnitDefinition(string metaId, string sboTerm, string notes, string annotation, Guid modelId, string sbmlId, string name, bool isBaseUnit)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class
            this.modelId = modelId;
            this.sbmlId = sbmlId;
            this.name = name;
            this.isBaseUnit = isBaseUnit;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapUnitDefinition()
        {
        }

        #endregion

        #region Member Variables

        Guid id = Guid.Empty;
        private Guid modelId = Guid.Empty;
        private string sbmlId = null;
        private string name = null;
        private bool isBaseUnit = false;


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
        /// sbmlId 
        /// </summary>
        public  string  SbmlId
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
        /// name 
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
        /// isBaseUnit 
        /// </summary>
        public bool IsBaseUnit
        {
            get { return isBaseUnit; }
            set
            {
                if (isBaseUnit != value)
                {
                    isBaseUnit = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        #endregion

    } // end class

} // end namespace



