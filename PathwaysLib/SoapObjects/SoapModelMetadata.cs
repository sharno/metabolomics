#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapModelMetadata")]
    public class SoapModelMetadata : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapModelMetadata()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapModelMetadata(string metaId, string sboTerm, string notes, string annotation, String modelName, Int32 publicationId, DateTime creationDate, DateTime modificationDate, String modelNotes)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class
            this.modelName = modelName;
            this.publicationId = publicationId;
            this.creationDate = creationDate;
            this.modificationDate = modificationDate;
            this.notes = modelNotes;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapModelMetadata()
        {
        }

        #endregion

        #region Member Variables

        Guid id = Guid.Empty;
        String modelName = "";
        int publicationId = 0;
        DateTime creationDate;
        DateTime modificationDate;
        String notes = "";

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
        public String ModelName
        {
            get { return modelName; }
            set
            {
                if (modelName != value)
                {
                    modelName = value;
                    Status = ObjectStatus.Update;
                }
            }
        }
        public int PublicationId
        {
            get { return publicationId; }
            set
            {
                if (publicationId != value)
                {
                    publicationId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }
        public DateTime CreationDate
        {
            get { return creationDate; }
            set
            {
                if (creationDate != value)
                {
                    creationDate = value;
                    Status = ObjectStatus.Update;
                }
            }
        }
        public DateTime ModificationDate
        {
            get { return modificationDate; }
            set
            {
                if (modificationDate != value)
                {
                    modificationDate = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        public String Notes
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



        #endregion

    } // end class

} // end namespace



