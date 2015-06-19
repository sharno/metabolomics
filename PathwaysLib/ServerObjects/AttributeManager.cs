#region Using Declarations
using System;
using System.Data;
using System.Collections;

using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
#endregion

namespace PathwaysLib.ServerObjects
{
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/Server/AttributeManager.cs</filepath>
    ///		<creation>2005/11/29</creation>
    ///		<author>
    ///			<name>Brendan Elliott</name>
    ///			<initials>BE</initials>
    ///			<email>bxe7@cwru.edu</email>
    ///		</author>
    ///		<contributors>
    ///			<contributor>
    ///				<name>none</name>
    ///				<initials>none</initials>
    ///				<email>none</email>
    ///			</contributor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: ali $</cvs_author>
    ///			<cvs_date>$Date: 2009/03/26 04:00:50 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/AttributeManager.cs,v 1.2 2009/03/26 04:00:50 ali Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.2 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Provides static methods for managing the attribute_names and 
    /// attribute_values table to provide a dictionary-like interface for generic item
    /// attributes that can be used with any table with a GUID primary key.
    /// </summary>
    #endregion		
    public class AttributeManager
	{
        /// <summary>
        /// The length of the longest string that will be stored in the attribute value's 
        /// varchar field.  Anything longer will be stored in the text field and 
        /// will not be searchable.
        /// </summary>
        public const int MAX_SEARCHABLE_ATTRIBUTE_VALUE = 799;

		private AttributeManager()
		{
		}

        #region Attribute Get/Set interface (attribute_values table)

        /// <summary>
        /// Sets a value for the given attribute for the specified item.
        /// If the string is longer than MAX_SEARCHABLE_ATTRIBUTE_VALUE size (799),
        /// then it is stored in a varchar field and will be searchable,
        /// otherwise it will be stored in a text field and will NOT be searchable 
        /// (i.e. will not be found by the FindItems function).
        /// </summary>
        /// <param name="attribName"></param>
        /// <param name="itemId"></param>
        /// <param name="value"></param>
        public static void SetAttribute(string attribName, Guid itemId, string value)
        {
            int attribId = GetAttributeId(attribName, true);

            string varCharValue = null;
            string textValue = null;

            if (value == null)
                value = "";

            value = value.Trim();

            if (value.Length <= MAX_SEARCHABLE_ATTRIBUTE_VALUE)
            {
                // can fit in the varchar field
                varCharValue = value;
            }
            else
            {
                // can only fit in the text field (won't be searchable!)
                textValue = value;
            }

            if (HasAttribute(attribName, itemId))
            {
                // update
                DBWrapper.Instance.ExecuteNonQuery("UPDATE attribute_values SET [value] = @value, textValue = @textValue WHERE attributeId = @attributeId AND itemId = @itemId",
                    "@value", SqlDbType.VarChar, varCharValue,
                    "@textValue", SqlDbType.Text, textValue,
                    "@attributeId", SqlDbType.Int, attribId,
                    "@itemId", SqlDbType.UniqueIdentifier, itemId);
            }
            else
            {
                // insert
                DBWrapper.Instance.ExecuteNonQuery("INSERT INTO attribute_values (attributeId, itemId, [value], textValue) VALUES (@attributeId, @itemId, @value, @textValue)",
                    "@attributeId", SqlDbType.Int, attribId,
                    "@itemId", SqlDbType.UniqueIdentifier, itemId,
                    "@value", SqlDbType.VarChar, varCharValue,
                    "@textValue", SqlDbType.Text, textValue);
            }
        }

        /// <summary>
        /// Returns the value of a given attribute for a given item.
        /// </summary>
        /// <param name="attribName"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static string GetAttribute(string attribName, Guid itemId)
        {
            int attribId = GetAttributeId(attribName, false);

            if (attribId == -1)
                return null;

            DataSet results;
            if (DBWrapper.Instance.ExecuteQuery(out results, "SELECT [value], textValue FROM attribute_values WHERE attributeId = @attributeId AND itemId = @itemId ",
                "@attributeId", SqlDbType.Int, attribId,
                "@itemId", SqlDbType.UniqueIdentifier, itemId) < 1)
            {
                return null; // not found
            }

            string varCharValue = results.Tables[0].Rows[0]["value"] is DBNull ? null : (string)results.Tables[0].Rows[0]["value"];
            string textValue = results.Tables[0].Rows[0]["textValue"] is DBNull ? null : (string)results.Tables[0].Rows[0]["textValue"];

            if (varCharValue != null)
                return varCharValue;
            else
                return textValue;
        }

        /// <summary>
        /// Returns true if the given item has an attribute value.
        /// </summary>
        /// <param name="attribName"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool HasAttribute(string attribName, Guid itemId)
        {
            int attribId = GetAttributeId(attribName, false);

            if (attribId == -1)
                return false;

            DataSet results;
            return DBWrapper.Instance.ExecuteQuery(out results, "SELECT attributeId, itemId FROM attribute_values WHERE attributeId = @attributeId AND itemId = @itemId ",
                "@attributeId", SqlDbType.Int, attribId,
                "@itemId", SqlDbType.UniqueIdentifier, itemId) > 0;
        }

        #endregion

        #region Attribute value searching interface (attribute_values table)

        /// <summary>
        /// Find all items with the given attribute set to the given value.
        /// Will only find item values shored in the varchar field (i.e. shorter than MAX_SEARCHABLE_ATTRIBUTE_VALUE).
        /// </summary>
        /// <param name="attribName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid[] FindItems(string attribName, string value)
        {
            if (value == null)
                return new Guid[0];

            int attribId = GetAttributeId(attribName, false);
            if (attribId == -1)
                return new Guid[0];

            DataSet results;
            int rc = DBWrapper.Instance.ExecuteQuery(out results, "SELECT itemId FROM attribute_values WHERE attributeId = @attributeId AND [value] = @value ",
                "@attributeId", SqlDbType.Int, attribId,
                "@value", SqlDbType.VarChar, value);

            ArrayList items = new ArrayList(rc);

            if (rc > 0)
            {
                foreach(DataRow r in results.Tables[0].Rows)
                {
                    items.Add((Guid)r["itemId"]);
                }
            }
            
            return (Guid[])items.ToArray(typeof(Guid));
        }

        /// <summary>
        /// Find all items with the given attribute set to the given value.
        /// Will only find item values shored in the varchar field (i.e. shorter than MAX_SEARCHABLE_ATTRIBUTE_VALUE).
        /// </summary>
        /// <param name="attribName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid[] FindItemsEndsWith(string attribName, string value)
        {
            if (value == null)
                return new Guid[0];

            int attribId = GetAttributeId(attribName, false);
            if (attribId == -1)
                return new Guid[0];

            DataSet results;
            int rc = DBWrapper.Instance.ExecuteQuery(out results, "SELECT itemId FROM attribute_values WHERE attributeId = @attributeId AND [value] like " +"\'%" + value +"\'",
                "@attributeId", SqlDbType.Int, attribId);
                //"@value", SqlDbType.VarChar, value);

            ArrayList items = new ArrayList(rc);

            if (rc > 0)
            {
                foreach (DataRow r in results.Tables[0].Rows)
                {
                    items.Add((Guid)r["itemId"]);
                }
            }

            return (Guid[])items.ToArray(typeof(Guid));
        }

        /// <summary>
        /// Finds the first item returned with the given attribute value.
        /// Will only find item values shored in the varchar field (i.e. shorter than MAX_SEARCHABLE_ATTRIBUTE_VALUE).
        /// </summary>
        /// <param name="attribName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid FindSingleItem(string attribName, string value)
        {
            Guid[] items = FindItems(attribName, value);

            if (items.Length > 0)
            {
                return items[0];
            }
            else
            {
                return Guid.Empty;
            }
        }

        #endregion

        #region Attribute deletion interface (attribute_values table)

        /// <summary>
        /// Deletes all attribute values associated with a single item GUID.
        /// This is useful for attribute cleanup when deleting an object.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static int DeleteItemAttributes(Guid itemId)
        {
            return DBWrapper.Instance.ExecuteNonQuery("DELETE FROM attribute_values WHERE itemId = @itemId",
                "@itemId", SqlDbType.UniqueIdentifier, itemId);
        }

        /// <summary>
        /// Deletes an attribute from all items.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static int DeleteAttribute(string attributeName)
        {
            int attribId = GetAttributeId(attributeName, false);
            if (attribId == -1)
                return 0;

            return DBWrapper.Instance.ExecuteNonQuery("DELETE FROM attribute_names WHERE attributeId = @attributeId",
                "@attributeId", SqlDbType.Int, attribId);
        }

        /// <summary>
        /// From the specified attribute value for a given item.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static int DeleteAttribute(string attributeName, Guid itemId)
        {
            int attribId = GetAttributeId(attributeName, false);
            if (attribId == -1)
                return 0;

            return DBWrapper.Instance.ExecuteNonQuery("DELETE FROM attribute_values WHERE attributeId = @attributeId AND itemId = @itemId",
                "@attributeId", SqlDbType.Int, attribId,
                "@itemId", SqlDbType.UniqueIdentifier, itemId);
        }

        #endregion

        #region Attribute name lookup interface (attribute_names table)

        /// <summary>
        /// Gets the attribute ID associated with an attribute name.
        /// If create is true and the attribute name is not found, it is created.
        /// </summary>
        /// <param name="attribName"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public static int GetAttributeId(string attribName, bool create)
        {
            DataSet results;
            if (DBWrapper.Instance.ExecuteQuery(out results, "SELECT attributeId FROM attribute_names WHERE [name] = @name",
                "@name", SqlDbType.VarChar, attribName) > 0)
            {
                // found
                return (int)results.Tables[0].Rows[0]["attributeId"];
            }
            else
            {
                // not found
                if (create)
                {
                    if (DBWrapper.Instance.ExecuteNonQuery("INSERT INTO attribute_names (name) VALUES (@name)",
                        "@name", SqlDbType.VarChar, attribName) > 0)
                    {
                        if (DBWrapper.Instance.ExecuteQuery(out results, "SELECT attributeId FROM attribute_names WHERE [name] = @name",
                            "@name", SqlDbType.VarChar, attribName) > 0)
                        {
                            // found
                            return (int)results.Tables[0].Rows[0]["attributeId"];
                        }
                        else
                        {
                            throw new DataModelException("Insertion of new attribute name failed: " + attribName);
                        }
                    }
                    else
                    {
                        throw new DataModelException("Insertion of new attribute name failed: " + attribName);
                    }


                }
                else
                {
                    return -1;
                }

            }
        }

        #endregion
    }
}

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: AttributeManager.cs,v 1.2 2009/03/26 04:00:50 ali Exp $
	$Log: AttributeManager.cs,v $
	Revision 1.2  2009/03/26 04:00:50  ali
	*** empty log message ***
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.2  2006/05/18 19:01:47  greg
	 - Pathway menu expansion
	When going through the pathways menu in the control bar, the menu that should be open isn't.  Now it is.
	
	 - SQL injection stuff
	More queries were rewritten to prevent SQL injection, and some were also rewritten to be a little more aesthetically pleasing.
	
	 - Finding SQL bugs
	SQL bugs that have arisen as a result of schema changes are still being checked for.  Any that I find are being sent to Ali for rewriting and reintegration.
	
	Revision 1.1  2005/11/29 23:01:26  brendan
	Added generic AttributeManager class for managing arbitrary string pairs associated with a GUID id.
	
------------------------------------------------------------------------*/
#endregion