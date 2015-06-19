#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion


namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapMapReactionsProcessEntities")]
    public class SoapMapReactionsProcessEntities : SoapObject
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapMapReactionsProcessEntities()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapMapReactionsProcessEntities(Guid reactionId, Guid processId, int qualifierId)
        {
            // this.id = Guid.Empty; // created on insert into the DB

            
            this.reactionId = reactionId;
            this.processId = processId;
            this.qualifierId = qualifierId;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapMapReactionsProcessEntities()
        {
        }

        #endregion

        #region Member Variables        
        private Guid reactionId = Guid.Empty;
        private Guid processId = Guid.Empty;
        int qualifierId;

        #endregion

        #region Properties
      
        /// <summary>
        ///reaction
        /// </summary>
        public Guid ReactionId
        {
            get { return reactionId; }
            set
            {
                if (reactionId != value)
                {
                    reactionId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        ///  processId.
        /// </summary>
        public Guid ProcessId
        {
            get { return processId; }
            set
            {
                if (processId != value)
                {
                    processId = value;
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






