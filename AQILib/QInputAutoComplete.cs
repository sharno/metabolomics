using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    public abstract class QInputAutoComplete : QInput
    {
        private QInputAutoComplete() { }

        public QInputAutoComplete(string name, QInputRenderer renderer)
            : base(name, renderer)
        { }

        public QInputAutoComplete(string name, string defaultValue, QInputRenderer renderer)
            : base(name, defaultValue, renderer)
        { }

        protected QInputAutoComplete(QInputAutoComplete input)
            : base(input)
        { }

        //public abstract Dictionary<string, string> GetValues(string currentValue, IAQIUtil util);
    }
}