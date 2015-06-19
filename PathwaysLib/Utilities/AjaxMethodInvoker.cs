using System;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Web;
using PathwaysLib.Exceptions;

namespace PathwaysLib.Utilities
{
	/// <summary>
	/// Invokes Ajax functions called from the client.
	/// </summary>
	public class AjaxMethodInvoker
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="classType">Custom classType</param>
		public AjaxMethodInvoker( Type classType )
		{
			this.classType = classType;
		}

		Type classType;

		/// <summary>
		/// Methods the invoker understands
		/// </summary>
		protected Hashtable methodTable = null;

		/// <summary>
		/// Execute an Ajax function
		/// </summary>
		/// <param name="obj">Generic object on which to call the opcode</param>
		/// <param name="op">Opcode to call</param>
		/// <param name="request">Current HttpRequest object</param>
		/// <returns>Whether the execution was successful</returns>
		public bool Execute( object obj, string op, HttpRequest request )
		{
			if( methodTable == null )
			{
				LoadMethodInfo();
			}

			if( !methodTable.ContainsKey( op ) )
			{
				throw new AjaxMethodException( "Invalid method." );
			}

			ExecuteMethod( obj, (MethodInfo)methodTable[op], request );
			return true;
		}

		/// <summary>
		/// Gets methods understood by the AjaxMethodInvoker
		/// </summary>
		public ArrayList Methods
		{
			get
			{
				if( methodTable == null )
				{
					LoadMethodInfo();
				}
				return new ArrayList( methodTable.Values );
			}
		}

		private void LoadMethodInfo()
		{
			methodTable = new Hashtable();

			MethodInfo[] methods = classType.GetMethods();

			foreach( MethodInfo method in methods )
			{
				AjaxMethodAttribute[] ajaxAttributes =
					(AjaxMethodAttribute[])method.GetCustomAttributes( typeof( AjaxMethodAttribute ), false );
				if( ajaxAttributes != null && ajaxAttributes.Length > 0 )
				{
					// this is an ajax method, add to table
					methodTable.Add( method.Name, method );
				}
			}
		}

		private void ExecuteMethod( object obj, MethodInfo method, HttpRequest request )
		{
			method.GetParameters();
			ParameterInfo[] parameters = method.GetParameters();
			ArrayList args = new ArrayList();

			foreach( ParameterInfo param in parameters )
			{
				if( !param.IsOptional )
				{
					args.Add( GetParamValue( param.Name, param.ParameterType, request.Params[param.Name] ) );
				}
			}

			method.Invoke( obj, args.ToArray() );
		}

		private object GetParamValue( string param, Type type, string value )
		{
			try
			{
				if( value == null)// || value == string.Empty )
				{
					throw new AjaxMethodException( "Missing parameter {0} ({1})!", param, type.Name );
				}

				if( type.FullName == typeof( string ).FullName )
				{
					return value;
				}
				else if( type == typeof( string ) )
				{
					return value;
				}

                if (value == string.Empty)
                {
					throw new AjaxMethodException( "Empty non-string parameter {0} ({1})!", param, type.Name );
                }

				if( type == typeof( int ) )
				{
					return int.Parse( value );
				}
				else if( type == typeof( long ) )
				{
					return long.Parse( value );
				}
				else if( type == typeof( short ) )
				{
					return short.Parse( value );
				}
				else if( type == typeof( double ) )
				{
					return double.Parse( value );
				}
				else if( type == typeof( float ) )
				{
					return float.Parse( value );
				}
				else if( type == typeof( bool ) )
				{
					value = value.ToLower().Trim();
					return ( value == "1" || value == "true" );
				}
				else if( type == typeof( DateTime ) )
				{
					return DateTime.Parse( value );
				}
				else
				{
					throw new AjaxMethodException( "Unexpected type {0} for parameter {1}!", type.Name, param );
				}
			}
			catch( Exception e )
			{
				throw new AjaxMethodException( e.ToString() );
			}
		}
	}
}