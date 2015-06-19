#region Using Declarations
using System;
using System.Collections;
using System.Configuration;

using System.IO;
#endregion

namespace PathwaysLib.Utilities
{
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/Utilities/GraphCacheManager.cs</filepath>
    ///		<creation>2006/02/06</creation>
    ///		<author>
    ///			<name>Brendan Elliott</name>
    ///			<initials>BE</initials>
    ///			<email>bxe7@cwru.edu</email>
    ///		</author>
    ///		<contributors>
    ///			<contributor>
    ///				<name>Brian Lauber</name>
    ///				<initials>bml</initials>
    ///				<email>Brian.Lauber@case.edu</email>
    ///			</contributor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: mustafa $</cvs_author>
    ///			<cvs_date>$Date: 2008/05/16 21:15:58 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/Utilities/GraphCacheManager.cs,v 1.1 2008/05/16 21:15:58 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Performs file-based caching of pathway/process layout XML documents.
    /// </summary>
    #endregion
    public class GraphCacheManager
	{

        #region Constructor, Destructor, ToString

        /// <summary>
        /// Singlon object, no public constructor
        /// </summary>
		private GraphCacheManager()
		{
		}


        #endregion

        #region Member Variables
        private static GraphCacheManager instance = null;
        private static string cacheRootPath = ConfigurationManager.AppSettings.Get("GraphCacheFolder");
        #endregion

        #region Properties

         /// <summary>
        /// Get the singleton instance
        /// </summary>
        public static GraphCacheManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GraphCacheManager();

                    if (cacheRootPath == null || cacheRootPath.Length < 1)
                        throw new ConfigurationErrorsException("Graph Cache Manager requires a 'GraphCacheFolder' key to be specified in the config file!");

                    if (cacheRootPath[cacheRootPath.Length - 1] != '\\')
                    {
                        // add final slash to path if it isn't there
                        cacheRootPath += "\\";
                    }

                    // ensure cache root folder exists
                    if (!Directory.Exists(cacheRootPath))
                    {
                        Directory.CreateDirectory(cacheRootPath);
                    }
                }                

                return instance;
            }
        }        

        #endregion

        #region Methods

        private string MakePathString(string type, Guid entity)
        {
            // Grab first 4 digits of guid.  Split these with a \
            string entityString = entity.ToString();
            entityString = "\\" + entityString.Substring(0, 2) + "\\" + entityString.Substring(2, 2) + "\\" + entityString + ".xml";

            return cacheRootPath + type + entityString;
        }

        /// <summary>
        /// Attempt to retrieve an item in the cache by its type and Guid identifier.
        /// 
        /// Returns null if the item is not in the cache.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string Get(string type, Guid entity)
        {
            //TODO: allow multiple readers, but single writers

            string path = MakePathString(type, entity);

            lock(this)
            {

                try
                {
                    if (!File.Exists(path))
                    {
                        return null;
                    }

                    StreamReader sr = File.OpenText(path);
                    return sr.ReadToEnd();
                }
                catch(Exception)
                {
                    // cache is broken!
                    return null;
                }
            
            }
        }

		/// <summary>
        /// Attempt to retrieve a single item value in the cache by its type (without Guid).
        /// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public string Get ( string type )
		{
			return Get(type, Guid.Empty );
		}

        /// <summary>
        /// Sets the value of an item in the cache, overwriting an old value, if present.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <param name="value"></param>
        public void Set(string type, Guid entity, string value)
        {
            //TODO: Throw an exception if value is empty??
            string path = MakePathString(type, entity);

            lock(this)
            {
                // ensure directory exists
                FileInfo fi = new FileInfo(path);

                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                
                StreamWriter sw = new StreamWriter(path, false);
                sw.Write(value);
                sw.Close();
            }
        }

        /// <summary>
        /// Sets the value of a single value item in the cache, overwriting an old value, if present.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
		public void Set ( string type, string value )
		{
			Set(type,Guid.Empty,value);
		}

        /// <summary>
        /// Removes an item from the cache.  Does nothing if the item is not in the cache.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        public void Remove(string type, Guid entity)
        {
            string path = MakePathString(type, entity);

            lock(this)
            {
                try                
                { 
                    if (!File.Exists(path))
                        return; 

                    File.Delete(path); 
                }
                catch(DirectoryNotFoundException)
                { 
                    /* Not really a fatal error.  Should we log this? */ 
                }
            }
        }

        /// <summary>
        /// Returns all cache items of a certain type.
        /// </summary>
        /// <param name="type"></param>
        public void ClearCache(string type)
        {
            string path = cacheRootPath + type;

            lock(this)
            {
                try
                {
                    if (!Directory.Exists(path))
                        return; 

                    Directory.Delete(path, true);
                }
                catch(DirectoryNotFoundException)
                {
                    // Once again, not really an error.  Maybe just log this event?
                }
            }
        }

        /// <summary>
        /// Clears all cache items, regardless of type.
        /// </summary>
        public void ClearCache()
        {
            // Recursively remove cacheRootPath, then recreate the base
            lock(this)
            {
                if (!Directory.Exists(cacheRootPath))
                    return; 

                Directory.Delete(cacheRootPath, true);
                Directory.CreateDirectory(cacheRootPath);
            }

            // Note: Any errors that are thrown must be handled by calling functions
        }

        #endregion
    }
}

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: GraphCacheManager.cs,v 1.1 2008/05/16 21:15:58 mustafa Exp $
	$Log: GraphCacheManager.cs,v $
	Revision 1.1  2008/05/16 21:15:58  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:44  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.3  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.2  2006/02/17 17:25:53  michael
	*** empty log message ***
	
	Revision 1.1  2006/02/07 23:22:26  brendan
	Added drawing support for generic co-factors.
	
	Added graph caching support.  Will require a new value in the .config file.
	
------------------------------------------------------------------------*/
#endregion
