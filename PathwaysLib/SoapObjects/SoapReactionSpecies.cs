#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapReactionSpecies")]
    public class SoapReactionSpecies : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapReactionSpecies()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapReactionSpecies(string metaId, string sboTerm, string notes, string annotation, Guid reactionId, Guid speciesId, short roleId, double stoichiometry, Guid stoichiometryMathId, string sbmlId, string name)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class

            this.reactionId = reactionId;
            this.speciesId = speciesId;
            this.roleId = roleId;
            this.stoichiometry = stoichiometry;
            this.stoichiometryMathId = stoichiometryMathId;
            this.sbmlId = sbmlId;
            this.name = name;
        }




        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapReactionSpecies()
        {
        }

        #endregion

        #region Member Variables

        Guid id = Guid.Empty;
        private Guid reactionId = Guid.Empty;
        private Guid speciesId = Guid.Empty;
        private short roleId = 0;
        private double stoichiometry = 0;
        private Guid stoichiometryMathId = Guid.Empty;
        private string sbmlId = null;
        private string name = null;

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
        /// reactionId
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
        /// speciesId 
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
        /// roleId 
        /// </summary>
        public short RoleId
        {
            get { return roleId; }
            set
            {
                if (roleId != value)
                {
                    roleId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// stoichiometry 
        /// </summary>
        public double Stoichiometry 
        {
            get { return stoichiometry; }
            set
            {
                if (stoichiometry != value)
                {
                    stoichiometry = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// stoichiometryMathId 
        /// </summary>
        public Guid StoichiometryMathId
        {
            get { return stoichiometryMathId; }
            set
            {
                if (stoichiometryMathId != value)
                {
                    stoichiometryMathId = value;
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
        /// name
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

        #endregion

    } // end class

} // end namespace

