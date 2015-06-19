using System;
using System.Collections;
using System.Collections.Specialized;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.Utilities
{
	/// <summary>
	/// Provides useful utility functions for the search pagination system.
	/// </summary>
	public class PaginationHelper
	{
		/// <summary>
		/// Default constructor; doesn't do anything useful.
		/// </summary>
		public PaginationHelper() {}

		/// <summary>
		/// Adds linebreak hints to the string following certain punctuation marks.
		/// </summary>
		/// <param name="s">A string to add hints to</param>
		/// <returns>A linebreak-hinted string</returns>
		public static string AddLinebreakHints( string s )
		{
			return s.Replace( "-", "-<wbr />" ).Replace( ":", ":<wbr />" ).Replace( ",", ",<wbr />" )
				.Replace( ")", ")<wbr />" );
		}
		
		/// <summary>
		/// Removes duplicate processes from from a list.
		/// </summary>
		/// <param name="list">An ArrayList of processes to process (heh)</param>
		public static void RemoveDuplicateProcesses( ref ArrayList list )
		{
			bool[] duplicate = new bool[list.Count];
			for( int i = 0; i < list.Count; ++i )
			{
				if( duplicate[i] ) continue;

				for( int j = i + 1; j < list.Count; ++j )
					if( ( (ServerProcess)list[i] ).GenericProcessID == ( (ServerProcess)list[j] ).GenericProcessID )
						duplicate[j] = true;
			}

			for( int i = list.Count - 1; i > 0; --i ) if( duplicate[i] ) list.RemoveAt( i );
		}

		/// <summary>
		/// Removes duplicate process entites from from a list.
		/// </summary>
		/// <param name="list">An ArrayList of process entities to process</param>
		public static void RemoveDuplicateProcessEntities( ref ArrayList list )
		{
			bool[] duplicate = new bool[list.Count];
			for( int i = 0; i < list.Count; ++i )
			{
				if( duplicate[i] ) continue;

				for( int j = i + 1; j < list.Count; ++j )
					if( ( (ServerProcessEntity)list[i] ).EntityID == ( (ServerProcessEntity)list[j] ).EntityID )
						duplicate[j] = true;
			}

			for( int i = list.Count - 1; i > 0; --i ) if( duplicate[i] ) list.RemoveAt( i );
		}

		/// <summary>
		/// Clears out display item nodes above the current level.
		/// </summary>
		/// <param name="args">A hashtable containing the nodes to clear</param>
		public static void ClearUpperNodes( ref Hashtable args )
		{
			int level = int.Parse( args["level"].ToString() ) + 1;

			while( level <= 3 )
			{
				args["node"+level] = Guid.Empty;
				args["node"+level+"type"] = "none";
				++level;
			}
		}

		/// <summary>
		/// Fills a hashtable with standard arguments used by all root-level pagination menus.
		/// </summary>
		/// <param name="LH">A LinkHelper object that represents the current viewstate</param>
		/// <param name="args">A hashtable to fill with values</param>
		public static void LoadStandardArgs( LinkHelper LH, ref Hashtable args )
		{
			args["viewid"] = LH.GetParameter( LH.ParameterName );
			args["level"] = 1;  // The current node level to consider
			args["displayitem"] = LH.DisplayItemID;
			args["displayitemtype"] = LH.DisplayItemType == null ? "none" : LH.DisplayItemType;
			args["node1"] = LH.OpenNode1ID;
			args["node1type"] = LH.OpenNode1Type == null ? "none" : LH.OpenNode1Type;
			args["node2"] = LH.OpenNode2ID;
			args["node2type"] = LH.OpenNode2Type == null ? "none" : LH.OpenNode2Type;
			args["node3"] = LH.OpenNode3ID;
			args["node3type"] = LH.OpenNode3Type == null ? "none" : LH.OpenNode3Type;
			args["viewgraph"] = LH.ViewGraph;

			if( LH.GetParameter( "terms" ) != string.Empty ) args["terms"] = LH.GetParameter( "terms" );
			if( LH.GetParameter( "type" ) != string.Empty && LH.GetParameter( "type" ) != "Contains" )
				args["type"] = LH.GetParameter( "type" );
			if( LH.GetParameter( "page" ) != string.Empty && LH.GetParameter( "page" ) != "1" )
				args["page"] = LH.GetParameter( "page" );
		}

		/// <summary>
		/// Converts a string into a SearchMethod enum.
		/// </summary>
		/// <param name="str">The string to convert</param>
		/// <returns> SearchMethod object whose value is equal to str</returns>
		public static SearchMethod string2SearchMethod( string str )
		{
			switch( str )
			{
				case "StartsWith": return SearchMethod.StartsWith;
				case "EndsWith": return SearchMethod.EndsWith;
				case "ExactMatch": return SearchMethod.ExactMatch;
				default: return SearchMethod.Contains;
			}
		}

		/// <summary>
		/// Converts an argument hashtable into a string to be sent via JavaScript.
		/// </summary>
		/// <returns>A JavaScript-ready string of additional arguments</returns>
		public static string ArgumentList( Hashtable arguments )
		{
			string arglist = string.Empty;

			if( arguments != null )
			{
				foreach( DictionaryEntry arg in arguments )
					arglist += string.Format( "{0}@{1}$", arg.Key, arg.Value );
				arglist = arglist.Substring( 0, arglist.Length-1 );
			}

			return arglist;
		}
	}
}