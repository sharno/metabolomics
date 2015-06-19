#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapCompartment")]
    public class SoapCompartment : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapCompartment()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>


        public SoapCompartment(string metaId, string sboTerm, string notes, string annotation, Guid modelId, string sbmlId, string name, Guid compartmentTypeId, int spatialDimensions, float size, Guid unitsId, Guid outside, Guid compartmentClassId, bool constant)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class

            this.modelId = modelId;
            this.sbmlId = sbmlId;
            this.name = name;
            this.compartmentTypeId = compartmentTypeId;
            this.spatialDimensions = spatialDimensions;
            this.size = size;
            this.unitsId = unitsId;
            this.outside = outside;
            this.constant = constant;
            this.compartmentClassId = compartmentClassId;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapCompartment()
        {
        }

        #endregion

        #region Member Variables
        Guid id = Guid.Empty;
        private Guid modelId = Guid.Empty;
        private string sbmlId = null;
        private string name = null;
        private Guid compartmentTypeId = Guid.Empty;
        private int spatialDimensions = 0;
        private double size = 0;
        private Guid unitsId = Guid.Empty;
        private Guid outside = Guid.Empty;
        private Guid compartmentClassId = Guid.Empty;
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
        /// compartmentTypeId
        /// </summary>
        public Guid CompartmentTypeId
        {
            get { return compartmentTypeId; }
            set
            {
                if (compartmentTypeId != value)
                {
                    compartmentTypeId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// spatialDimensions
        /// </summary>
        public int SpatialDimensions
        {
            get { return spatialDimensions; }
            set
            {
                if (spatialDimensions != value)
                {
                    spatialDimensions = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// size
        /// </summary>
        public double Size
        {
            get { return size; }
            set
            {
                if (size != value)
                {
                    size = value;
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
        /// outside
        /// </summary>
        public Guid Outside
        {
            get { return outside; }
            set
            {
                if (outside != value)
                {
                    outside = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// compartmentClassId
        /// </summary>
        public Guid CompartmentClassId
        {
            get { return compartmentClassId; }
            set
            {
                if (compartmentClassId != value)
                {
                    compartmentClassId = value;
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



