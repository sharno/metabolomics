#region Using Declarations
using System;
using System.Xml.Serialization;

using PathwaysLib.ServerObjects;
#endregion
namespace PathwaysLib.SoapObjects
{
    [Serializable(), SoapType("SoapModel")]
    public class SoapModel : SoapSbase
    {
        #region Constructor, Destructor, ToString

        /// <summary>
        /// Default constructor, create a DataSource relation with nothing initialized.
        /// </summary>
        public SoapModel()
        {
        }

        /// <summary>
        /// Constructor, create a new DataSource relation with all fields initialized.
        /// </summary>


        public SoapModel(string metaId, string sboTerm, string notes, string annotation, string sbmlId, string name, int sbmlLevel, int sbmlVersion, int dataSourceId, string sbmlFile, string sbmlFileName)
            :base(metaId, sboTerm, notes, annotation)
        {
            this.id = base.ID; // get ID of base class

            this.sbmlId = sbmlId;
            this.name = name;
            this.sbmlLevel = sbmlLevel;
            this.sbmlVersion = sbmlVersion;
            this.dataSourceId = dataSourceId;
            this.sbmlFile = sbmlFile;
            this.sbmlFileName = sbmlFileName;   
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SoapModel()
        {
        }

        #endregion

        #region Member Variables
        Guid id = Guid.Empty;        
        private string sbmlId = null;
        private string name = null;
        private int sbmlLevel = 0;
        private int sbmlVersion = 0;
        private int dataSourceId = 0;
        private string sbmlFile = null;
        private string sbmlFileName = null;

        #endregion

        #region Properties


        /// <summary>
        /// Model ID.
        /// </summary>
        /// 
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
        /// sbmlId of Model
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
        /// Name of Model
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
        /// SbmlLevel of datasource
        /// </summary>
        public int SbmlLevel
        {
            get { return sbmlLevel; }
            set
            {
                if (sbmlLevel != value)
                {
                    sbmlLevel = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// SbmlVersion of datasource
        /// </summary>
        public int SbmlVersion
        {
            get { return sbmlVersion; }
            set
            {
                if (sbmlVersion != value)
                {
                    sbmlVersion = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// dataSourceId of datasource
        /// </summary>
        public int DataSourceId
        {
            get { return dataSourceId; }
            set
            {
                if (dataSourceId != value)
                {
                    dataSourceId = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// sbmlFile
        /// </summary>
        public string SbmlFile
        {
            get { return sbmlFile; }
            set
            {
                if (sbmlFile != value)
                {
                    sbmlFile = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

        /// <summary>
        /// sbmlFileName
        /// </summary>
        public string SbmlFileName
        {
            get { return sbmlFileName; }
            set
            {
                if (sbmlFileName != value)
                {
                    sbmlFileName = value;
                    Status = ObjectStatus.Update;
                }
            }
        }

       
        #endregion

    } // end class

} // end namespace


