using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// A helper class that overrides the List<T> functionality by making adding nodes only occur if they aren't already in the list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UniqueList<T> : List<T>
    {
        /// <summary>
        /// Initializes a new instance of the PathwaysLib.AQI.UniqueList&lt;T&gt; class that is empty and has the default initial capacity.
        /// </summary>
        public UniqueList()
            : base()
        { }

        /// <summary/>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public UniqueList(IEnumerable<T> collection)
            : base(collection)
        { }

        /// <summary/>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public UniqueList(int capacity)
            : base(capacity)
        { }

        /// <summary>
        /// Adds an object to the end of the PathwaysLib.AQI.UniqueList&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to be added to the end of the System.Collections.Generic.List&lt;T&gt;. The value can be null for reference types.</param>
        public new void Add(T item)
        {
            if(!base.Contains(item))
                base.Add(item);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the PathwaysLib.AQI.UniqueList&lt;T&gt;.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"/>
        /// <param name="collection">The collection whose elements should be added to the end of the System.Collections.Generic.List&lt;T&gt;. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        public new void AddRange(IEnumerable<T> collection)
        {
            if(collection == null)
                throw new System.ArgumentNullException();

            foreach(T item in collection)
                if(!base.Contains(item)) //BE: this is O(n) on a list!
                    base.Add(item);
        }
    }
}