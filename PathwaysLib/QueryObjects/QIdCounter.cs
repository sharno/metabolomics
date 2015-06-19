using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.Utilities
{
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QIdCounter
    {
        private int _counter;

        public QIdCounter()
        {
            _counter = 0;
        }

        public int Value
        {
            get { return _counter; }
        }

        public QIdCounter increment()
        {
            _counter++;
            return this;
        }
    }
}