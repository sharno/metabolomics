#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion


namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapModelOrganism")]
    public class SoapModelOrganism : SoapObject
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapModelOrganism()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapModelOrganism(Guid modelId, Guid organismGroupId, int taxoId, int qualifierId)
        {
            // this.id = Guid.Empty; // created on insert into the DB

            this.modelId = modelId;
            this.organismGroupId = organismGroupId;
            this.ncbiTaxonomyId = taxoId;
            this.qualifierId = qualifierId;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapModelOrganism()
        {
        }

        #endregion

        #region Member Variables
        private Guid modelId = Guid.Empty;
        private int ncbiTaxonomyId, qualifierId;
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

        public int NCBITaxonomyId
        {
            get { return ncbiTaxonomyId; }
            set
            {
                if (ncbiTaxonomyId != value)
                {
                    ncbiTaxonomyId = value;
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
        #endregion

    } // end class

} // end namespace







