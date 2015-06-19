using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
using System.Data;

namespace PathwaysLib.ServerObjects
{     
    public class AnnotationQualifierManager
	{
        private AnnotationQualifierManager()
		{
            AnnotationQualifierManager.init();           
		}

        private static void init()
        {
            if (complements == null)
                complements = new Dictionary<string, string>();
            else if (complements.Count > 0)
                return;
            string[] qualifiers ={"is", "isDescribedBy", "encodes", "hasPart", "hasVersion", 
                                        "isEncodedBy", "isHomologTo", "isPartOf", "isVersionOf", "occursIn", "unknown"};
            string[] qualifierCompliments ={"is", "describes", "isEncodedBy", "isPartOf", "isVersionOf", 
                                        "encodes", "isHomologTo", "hasPart", "hasVersion", "contains", "unknown"};
            for(int i = 0; i < qualifiers.Length; i++)
            {
                complements.Add(qualifiers[i], qualifierCompliments[i]);
            }
        }

        private static AnnotationQualifierManager instance = new AnnotationQualifierManager();
        static NameIdManager manager = new NameIdManager("AnnotationQualifier", "name", "id", SqlDbType.SmallInt);
        public static readonly string UnspecifiedQualifier = "00000000-0000-0000-0000-000000000000";
        private static Dictionary<string, string> complements;
        /// <summary>
        /// Returns the name associated with an id.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string GetQualifierName(int id)
        {
            return manager.GetName(id);
        }

        /// <summary>
        /// Returns the complement name associated with an id.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string GetComplementQualifierName(int id)
        {           
            if(complements.ContainsKey(manager.GetName(id)))
                return complements[manager.GetName(id)];
            else
                return "noComplementingQualifier (" + manager.GetName(id) + ")";
        }

        /// <summary>
        /// Returns the id associated with a name.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static int GetQualifierId(string name)
        {
            return manager.GetNameId(name);
        }

        /// <summary>
        /// Returns the id associated with a name, 
        /// optionally creating it if it doesn't exist.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public static int GetQualifierId(string name, bool create)
        {
            return manager.GetNameId(name, create);
        }

        /// <summary>
        /// Remove a name (and it's id) from the lookup table.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static bool DeleteQualifier(string name)
        {
            return manager.DeleteName(name);
        }

        /// <summary>
        /// Remove an id (and it's name) from the lookup table.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static bool DeleteQualifier(int id)
        {
            return manager.DeleteNameId(id);
        }

        /// <summary>
        /// Return all names.
        /// </summary>
        public static string[] AllQualifiers
        {
            get
            {
                return manager.AllNames;
            }
        }

        /// <summary>
        /// Return all name Ids
        /// </summary>
        public static int[] AllQualifierIds
        {
            get
            {
                return manager.AllNameIds;
            }
        }
	}
}
