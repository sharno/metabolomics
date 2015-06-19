#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapEvent")]
    public class SoapEvent : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapEvent()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapEvent(string metaId, string sboTerm, string notes, string annotation, Guid modelId, string sbmlId, string name, Guid eventTriggerId, Guid eventDelayId)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class
            this.modelId = modelId;
            this.sbmlId = sbmlId;
            this.name = name;
            this.eventTriggerId = eventTriggerId;
            this.eventDelayId = eventDelayId;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapEvent()
        {
        }

        #endregion

        #region Member Variables
        Guid id = Guid.Empty;
        private Guid modelId = Guid.Empty;
        private string sbmlId = null;
        private string name = null;
        private Guid eventTriggerId = Guid.Empty;
        private Guid eventDelayId = Guid.Empty;

        #endregion

        #region Properties


        /// <summary>
        /// Reaction ID.
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
        /// modelId.
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
        /// eventTriggerId.
        /// </summary>
        public Guid EventTriggerId
        {
            get { return eventTriggerId; }
            set
            {
                if (eventTriggerId != value)
                {
                    eventTriggerId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }


        /// <summary>
        /// eventDelayId.
        /// </summary>
        public Guid EventDelayId
        {
            get { return eventDelayId; }
            set
            {
                if (eventDelayId != value)
                {
                    eventDelayId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }


        #endregion

    } // end class

} // end namespace


