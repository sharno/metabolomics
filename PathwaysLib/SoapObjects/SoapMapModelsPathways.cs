#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion


namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapMapModelsPathways")]
    public class SoapMapModelsPathways : SoapObject
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapMapModelsPathways()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapMapModelsPathways(Guid modelId, Guid pathwayId, int qualifierId, Guid organismGroupId)
        {
            // this.id = Guid.Empty; // created on insert into the DB

            this.modelId = modelId;
            this.pathwayId = pathwayId;
            this.qualifierId = qualifierId;
            this.organismGroupId = organismGroupId;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapMapModelsPathways()
        {
        }

        #endregion

        #region Member Variables
        private Guid modelId = Guid.Empty;
        private Guid pathwayId = Guid.Empty;
        private int qualifierId;
        private Guid organismGroupId = Guid.Empty;

        #endregion

        #region Properties

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
        ///  pathwayId.
        /// </summary>
        public Guid PathwayId
        {
            get { return pathwayId; }
            set
            {
                if (pathwayId != value)
                {
                    pathwayId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        ///  qualifierId.
        /// </summary>
        public int QualifierId
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

        /// <summary>
        ///  organismGroupId.
        /// </summary>
        public Guid OrganismGroupId
        {
            get { return organismGroupId; }
            set
            {
                if (organismGroupId != value)
                {
                    organismGroupId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }
        #endregion

    } // end class

} // end namespace







