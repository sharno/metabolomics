#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapUnitComposition")]
    public class SoapUnitComposition : SoapObject
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapUnitComposition()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapUnitComposition(Guid unitDefinitionId, Guid unitId)
        {
            //this.id = Guid.Empty; // created on insert into the DB
            this.unitDefinitionId = unitDefinitionId;
            this.unitId = unitId;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapUnitComposition()
        {
        }

        #endregion

        #region Member Variables

       // Guid id = Guid.Empty;
        private Guid unitDefinitionId = Guid.Empty;
        private Guid unitId = Guid.Empty;

        #endregion

        #region Properties


        ///// <summary>
        ///// Compartment ID.
        ///// </summary>
        //public Guid ID
        //{
        //    get { return id; }
        //    set
        //    {
        //        if (id != value)
        //        {
        //            id = value;
        //            Status = ObjectStatus.Update;
        //        }
        //    }
        //}


        /// <summary>
        /// unitDefinitionId 
        /// </summary>
        public Guid UnitDefinitionId
        {
            get { return unitDefinitionId; }
            set
            {
                if (unitDefinitionId != value)
                {
                    unitDefinitionId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// unitId
        /// </summary>
        public Guid UnitId
        {
            get { return unitId; }
            set
            {
                if (unitId != value)
                {
                    unitId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        

        #endregion

    } // end class

} // end namespace



