#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion


namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapSpeciesType")]
    public class SoapSpeciesType : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapSpeciesType()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapSpeciesType(string metaId, string sboTerm, string notes, string annotation, Guid modelId, string sbmlId, string name)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = Guid.Empty; // created on insert into the DB
            this.modelId = modelId;
            this.sbmlId = sbmlId;
            this.name = name;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapSpeciesType()
        {
        }

        #endregion

        #region Member Variables

        Guid id = Guid.Empty;
        private Guid modelId = Guid.Empty;
        private string sbmlId = null;
        private string name = null;

        #endregion

        #region Properties


        /// <summary>
        /// Compartment ID.
        /// </summary>
        public Guid ID
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

        #endregion

    } // end class

} // end namespace




