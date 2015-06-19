using System;

using PathwaysLib.Exceptions;

namespace PathwaysLib.ServerObjects
{
    /// <summary>
    /// Wraps the value type 'long' to allow nulls.
    /// </summary>
	public class Long
	{
		/// <summary>
		/// Long type constructor.
		/// </summary>
        public Long()
        {
            this.data = 0;
        }

		/// <summary>
		/// Long type constructor.
		/// </summary>
		/// <param name="value">
		/// Variable of long type to load from.
		/// </param>
		public Long(long value)
		{
            this.data = value;
		}

        long data;

		/// <summary>
		/// Implicit conversion to long type.
		/// </summary>
		/// <param name="l">
		/// Long type to be converted.
		/// </param>
		/// <returns>
		/// Long l converted to long.
		/// </returns>
        public static implicit operator long (Long l)
        {
            if (l == null)
                throw new DataModelException("Cannot cast a null-valued Long into a normal long.  Use the Long class instead.");
            return l.data;
        }

		/// <summary>
		/// Implicit conversion to Long type.
		/// </summary>
		/// <param name="l">
		/// Variable long to convert to Long.
		/// </param>
		/// <returns>
		/// Long version of long.
		/// </returns>
        public static implicit operator Long (long l)
        {
            return new Long(l);
        }

		/// <summary>
		/// Get/set the Long type's value.
		/// </summary>
        public long Value
        {
            get {return data;}
            set {data = value;}
        }
	}
}
