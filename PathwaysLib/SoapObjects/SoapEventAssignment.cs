#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapEventAssignment")]
    public class SoapEventAssignment : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapEventAssignment()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapEventAssignment(string metaId, string sboTerm, string notes, string annotation, Guid eventId, string variable, string math)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class
            this.eventId = eventId;
            this.variable = variable;
            this.math = math;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapEventAssignment()
        {
        }

        #endregion

        #region Member Variables

        Guid id = Guid.Empty;
        private Guid eventId = Guid.Empty;
        private string variable = null;
        private string math = null;

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
        /// eventId
        /// </summary>
        public Guid EventId
        {
            get { return eventId; }
            set
            {
                if (eventId != value)
                {
                    eventId = value;
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

        #endregion

    } // end class

} // end namespace





