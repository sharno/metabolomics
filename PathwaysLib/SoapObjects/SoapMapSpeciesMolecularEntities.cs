#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion


namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapMapSpeciesMolecularEntities")]
    public class SoapMapSpeciesMolecularEntities : SoapObject
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapMapSpeciesMolecularEntities()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapMapSpeciesMolecularEntities(Guid speciesId, Guid molecularEntityId, int qualifierId)
        {
            // this.id = Guid.Empty; // created on insert into the DB
            
            this.speciesId = speciesId;
            this.molecularEntityId = molecularEntityId;
            this.qualifierId = qualifierId;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapMapSpeciesMolecularEntities()
        {
        }

        #endregion

        #region Member Variables
        
        private Guid speciesId = Guid.Empty;
        private Guid molecularEntityId = Guid.Empty;
        private int qualifierId;

        #endregion

        #region Properties       
        /// <summary>
        /// species
        /// </summary>
        public Guid SpeciesId
        {
            get { return speciesId; }
            set
            {
                if (speciesId != value)
                {
                    speciesId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        ///  molecularEntityId.
        /// </summary>
        public Guid MolecularEntityId
        {
            get { return molecularEntityId; }
            set
            {
                if (molecularEntityId != value)
                {
                    molecularEntityId = value;
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






