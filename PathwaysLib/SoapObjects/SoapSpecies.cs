#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapSpecies")]
    public class SoapSpecies : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapSpecies()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapSpecies(string metaId, string sboTerm, string notes, string annotation, Guid modelId, string sbmlId, string name, Guid speciesTypeId, Guid compartmentId,
            double initialAmount, double initialConcentration, Guid substanceUnitsId, bool hasOnlySubstanceUnits,
            bool boundaryCondition, int charge, bool constant)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class

            this.modelId = modelId;
            this.sbmlId = sbmlId;
            this.name = name;
            this.speciesTypeId = speciesTypeId;
            this.compartmentId = compartmentId;
            this.initialAmount = initialAmount;
            this.initialConcentration = initialConcentration;
            this.substanceUnitsId = substanceUnitsId;
            this.hasOnlySubstanceUnits = hasOnlySubstanceUnits;
            this.boundaryCondition = boundaryCondition;
            this.charge = charge;
            this.constant = constant;
        }


        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapSpecies()
        {
        }

        #endregion

        #region Member Variables
        Guid id = Guid.Empty;
        private Guid modelId = Guid.Empty;
        private string sbmlId = null;
        private string name = null;
        private Guid speciesTypeId = Guid.Empty;
        private Guid compartmentId = Guid.Empty;
        private double initialAmount = 0;
        private double initialConcentration = 0;
        private Guid substanceUnitsId = Guid.Empty;
        private bool hasOnlySubstanceUnits = false;
        private bool boundaryCondition = false;
        private int charge = 0;
        private bool constant = false;

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
        /// speciesTypeId
        /// </summary>
        public Guid SpeciesTypeId
        {
            get { return speciesTypeId; }
            set
            {
                if (speciesTypeId != value)
                {
                    speciesTypeId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// compartmentId
        /// </summary>
        public Guid CompartmentId
        {
            get { return compartmentId; }
            set
            {
                if (compartmentId != value)
                {
                    compartmentId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// initialAmount
        /// </summary>
        public double InitialAmount
        {
            get { return initialAmount; }
            set
            {
                if (initialAmount != value)
                {
                    initialAmount = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// initialConcentration
        /// </summary>
        public double InitialConcentration
        {
            get { return initialConcentration; }
            set
            {
                if (initialConcentration != value)
                {
                    initialConcentration = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// substanceUnitsId
        /// </summary>
        public Guid SubstanceUnitsId
        {
            get { return substanceUnitsId; }
            set
            {
                if (substanceUnitsId != value)
                {
                    substanceUnitsId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// hasOnlySubstanceUnits
        /// </summary>
        public bool HasOnlySubstanceUnits
        {
            get { return hasOnlySubstanceUnits; }
            set
            {
                if (hasOnlySubstanceUnits != value)
                {
                    hasOnlySubstanceUnits = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// boundaryCondition
        /// </summary>
        public bool BoundaryCondition
        {
            get { return boundaryCondition; }
            set
            {
                if (boundaryCondition != value)
                {
                    boundaryCondition = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// charge
        /// </summary>
        public int Charge
        {
            get { return charge; }
            set
            {
                if (charge != value)
                {
                    charge = value;
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



