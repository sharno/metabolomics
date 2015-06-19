using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// A wrapper for a single input box
    /// </summary>
    public abstract class QInput : ICloneable
    {
        private string _name;
        private string _value;
        private string _defaultValue;

        private QField _parent;

        private QInputRenderer _renderer;

        public string Name
        {
            get { return _name; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public QField Parent
        {
            get { return _parent; }
            set
            {
                if(_parent == null)
                    _parent = value;
            }
        }

        public QInputRenderer Renderer
        {
            get { return _renderer; }
        }

        protected QInput() { }

        public QInput(string name, QInputRenderer renderer)
        {
            _name = name;
            _value = String.Empty;
            _defaultValue = String.Empty;

            _parent = null;

            _renderer = renderer;
        }

        public QInput(string name, string defaultValue, QInputRenderer renderer)
        {
            _name = name;
            _value = defaultValue;
            _defaultValue = defaultValue;

            _parent = null;

            _renderer = renderer;
        }

        protected QInput(QInput input)
        {
            _name = input._name;
            _value = input._value;
            _defaultValue = input._defaultValue;
            _parent = input._parent;
            _renderer = input._renderer;
        }

        public abstract object Clone();

        public virtual Dictionary<string, string> GetValues(string currentValue, IAQIUtil util)
        {
            return null;
        }

        public virtual Dictionary<string, string> GetInitialValues()
        {
            return null;
        }

        public virtual string GetPrettyValue(string value)
        {
            Dictionary<string, string> initialValues = this.GetInitialValues();

            if(initialValues != null && initialValues.ContainsKey(value))
                return initialValues[value];
            else
                return value;
        }

        public virtual void Validate(string value)
        {
            return;
        }

        public virtual string ParseValue(string value)
        {
            return value;
        }
    }
}