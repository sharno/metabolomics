using System;
using System.Collections;
using System.Web.UI;
using PathwaysLib.QueryObjects;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.QueryObjects
{
	/// <summary>
	/// A query field for pathways
	/// </summary>
	public class QFieldPathway : QField
	{
		/// <summary>
		/// Constructor; requires all of the parameters.
		/// </summary>
		/// <param name="idName">The field's unique ID</param>
		/// <param name="initValue">The initial value; use string.Empty for none</param>
		/// <param name="useControls">0: no controls, 1: "or" control, 2: "or" and "delete" controls</param>
		/// <param name="deps">Array of dependencies in the form of "type", "value", ...</param>
		public QFieldPathway( string idName, string initValue, int useControls, params string[] deps ) : 
			base( idName, initValue, useControls, deps ) {}

		private ArrayList Populate()
		{
			ArrayList items = new ArrayList();
			ServerPathway spw;

			// Were we supplied the inital value?
			if( init != string.Empty )
			{
				// Display just this one item
				spw = ServerPathway.Load( new Guid( init ) );
				items.Add( new DictionaryEntry( spw.ID, spw.Name ) );
			}
			else
			{
				// Get the full list of pathways based on the dependency type.
				
				// TODO: For now, just get a full list
				ServerPathway[] pathways;
				//if( DependencyID == ServerOrganism.UnspecifiedOrganism )
				//{
					pathways = ServerPathway.AllPathways();
				//}
				//else
				//{
				//	pathways = ServerOrganismGroup.LoadFromID( new Guid( DependencyID ) ).GetPathways();
				//}

				foreach( ServerPathway p in pathways )
				{
					items.Add( new DictionaryEntry( p.ID, p.Name ) );
				}
			}

			return items;
		}

		protected override void RenderField( HtmlTextWriter output )
		{
			output.WriteBeginTag( "select" );
			output.WriteAttribute( "id", id );
			//output.WriteAttribute( "onchange", "something" );
			if( init != string.Empty ) output.WriteAttribute( "disabled", "disabled" );
			output.Write( HtmlTextWriter.TagRightChar );
			output.Write( "\n" );

			foreach( DictionaryEntry de in Populate() )
			{
				output.WriteBeginTag( "option" );
				output.WriteAttribute( "value", de.Key.ToString() );
				if( de.Key.ToString() == init ) output.WriteAttribute( "selected", "selected" );
				output.Write( HtmlTextWriter.TagRightChar );
				output.Write( de.Value.ToString() );
				output.WriteEndTag( "option" );
				output.Write( "\n" );
			}

			output.WriteEndTag( "select" );
			output.Write( "\n" );
		}
	}
}

			