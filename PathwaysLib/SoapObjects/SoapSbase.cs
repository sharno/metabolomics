#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapSbase")]
    public abstract class SoapSbase: SoapObject
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapSbase()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapSbase(string metaId, string sboTerm, string notes, string annotation)
        {
            this.id = Guid.Empty; // created on insert into the DB
            this.metaId = metaId;
            this.sboTerm = sboTerm;
            this.notes = notes;
            this.annotation = annotation;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapSbase()
        {
        }

        #endregion

        #region Member Variables

        Guid id = Guid.Empty;
        private string metaId = null;
        private string sboTerm = null;
        private string notes = null;
        private string annotation = null;

        #endregion

        #region Properties


        /// <summary>
        /// Compartment ID.
        /// </summary>
        public virtual Guid ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// metaId
        /// </summary>
        public string MetaId
        {
            get { return metaId; }
            set
            {
                if (metaId != value)
                {
                    metaId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// sboTerm
        /// </summary>
        public string SboTerm
        {
            get { return sboTerm; }
            set
            {
                if (sboTerm != value)
                {
                    sboTerm = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// notes
        /// </summary>
        public string Notes
        {
            get { return notes; }
            set
            {
                if (notes != value)
                {
                    notes = value;
                    Status = ObjectStatus.Update;
                }
            }
        }
        /// <summary>
        /// annotation
        /// </summary>
        public string Annotation
        {
            get { return annotation; }
            set
            {
                if (annotation != value)
                {
                    annotation = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        
        #endregion

    } // end class

} // end namespace





