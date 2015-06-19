#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapAuthor")]
    public class SoapAuthor : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapAuthor()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>

        public SoapAuthor(string metaId, string sboTerm, string notes, string annotation, String name, String surname, String email, String orgName)
            : base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class
            this.name = name;
            this.surname = surname;
            this.email = email;
            this.orgName = orgName;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapAuthor()
        {
        }

        #endregion

        #region Member Variables

        Guid id = Guid.Empty;
        String name = "";
        String surname = "";
        String email = "";
        String orgName = "";


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
        public String Name
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
        public String Surname
        {
            get { return surname; }
            set
            {
                if (surname != value)
                {
                    surname = value;
                    Status = ObjectStatus.Update;
                }
            }
        }
        public String EMail
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    Status = ObjectStatus.Update;
                }
            }
        }
        public String OrgName
        {
            get { return orgName; }
            set
            {
                if (orgName != value)
                {
                    orgName = value;
                    Status = ObjectStatus.Update;
                }
            }
        }



        #endregion

    } // end class

} // end namespace



