#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapDataSource")]
    public class SoapDataSource : SoapObject
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapDataSource()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>
    
        public SoapDataSource(string name, string url)
        {
           
            this.id = 0;
            this.name = name;
            this.url = url;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapDataSource()
        {
        }

        #endregion

        #region Member Variables
        short id = 0;
        private string name = null;
        private string url = null;

        #endregion

        #region Properties


        /// <summary>
        /// datasource ID.
        /// </summary>
        public short ID
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// Name of datasource
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
        /// Url of datasource
        /// </summary>
        public string Url
        {
            get { return url; }
            set
            {
                if (url != value)
                {
                    url = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        #endregion

    } // end class

} // end namespace

