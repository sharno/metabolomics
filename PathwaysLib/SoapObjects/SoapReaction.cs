#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapReaction")]
    public class SoapReaction : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapReaction()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapReaction(string metaId, string sboTerm, string notes, string annotation, Guid modelId, string sbmlId, string name, bool reversible, bool fast, Guid kineticLawId)
           : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class

            this.modelId = modelId;
            this.sbmlId = sbmlId;
            this.name = name;
            this.reversible = reversible;
            this.fast = fast;
            this.kineticLawId = kineticLawId;
        }


        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapReaction()
        {
        }

        #endregion

        #region Member Variables
        Guid id = Guid.Empty;
        private Guid modelId = Guid.Empty;
        private string sbmlId = null;
        private string name = null;
        private bool reversible = true;
        private bool fast = false;
        private Guid kineticLawId = Guid.Empty;

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
        /// reversible of Reaction
        /// </summary>
        public bool Reversible
        {
            get { return reversible; }
            set
            {
                if (reversible != value)
                {
                    reversible = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// fast of Reaction
        /// </summary>
        public bool Fast
        {
            get { return fast; }
            set
            {
                if (fast != value)
                {
                    fast = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// kineticLawId 
        /// </summary>
        public Guid KineticLawId
        {
            get { return kineticLawId; }
            set
            {
                if (kineticLawId != value)
                {
                    kineticLawId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }


        #endregion

        

    } // end class

} // end namespace


