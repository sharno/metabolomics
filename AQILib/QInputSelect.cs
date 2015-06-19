using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    public abstract class QInputSelect : QInput
    {
        private QInputSelect() { }

        public QInputSelect(string name, QInputRenderer renderer)
            : base(name, renderer)
        { }

        public QInputSelect(string name, string defaultValue, QInputRenderer renderer)
            : base(name, defaultValue, renderer)
        { }

        protected QInputSelect(QInputSelect input)
            : base(input)
        { }

        public override string ParseValue(string value)
        {
            return GetKeyFromValue(GetInitialValues(), value);
        }

        protected static string GetKeyFromValue(Dictionary<string,string> dic, string value)
        {
            foreach (KeyValuePair<string,string> kvp in dic)
            {
                if (kvp.Value == value)
                    return kvp.Key; // returns first key with the value!
            }
            return value; // return value unchanged
        }
    }
}