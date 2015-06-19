using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AQILib
{
    /// <summary>
    /// A base field type; fields are contained within a node and contain one or more inputs with descriptions
    /// </summary>
    public class QField
    {
        private string _fieldTypeId;
        private QNode _parent;
        private string _title;

        private bool _displayed;
        private bool _displayedActive;
        private bool _queried;
        private bool _negated;
        private bool _visible;

        private string _connector;
        private Dictionary<string, QConnector> _connectors;

        private int _valuesetCount;
        private Dictionary<string, List<QInput>> _inputs;
        private List<QInput> _inputTemplate;
        private List<string> _descriptions;

        public string FieldTypeId
        {
            get { return _fieldTypeId; }
        }

        public QNode Parent
        {
            get { return _parent; }
        }

        public string Title
        {
            get { return _title; }
        }

        public bool Displayed
        {
            get { return _displayed; }
            set { _displayed = value; }
        }

        public bool DisplayedActive
        {
            get { return _displayedActive; }
            set { _displayedActive = value; }
        }

        public bool Queried
        {
            get { return _queried; }
        }

        public bool Negated
        {
            get { return _negated; }
            set { _negated = value; }
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public string Connector
        {
            get { return _connector; }
            set { _connector = value; }
        }

        public Dictionary<string, QConnector> Connectors
        {
            get { return _connectors; }
        }

        public int ValuesetCount
        {
            get { return _valuesetCount; }
        }

        public Dictionary<string, List<QInput>> Inputs
        {
            get { return _inputs; }
        }

        public List<QInput> InputTemplate
        {
            get { return _inputTemplate; }
        }

        public Dictionary<string, QInput> InputTemplateDictionary
        {
            get
            {
                Dictionary<string, QInput> retVal = new Dictionary<string, QInput>();
                foreach(QInput input in InputTemplate)
                    retVal.Add(input.Name, input);
                return retVal;
            }
        }

        public List<string> Descriptions
        {
            get { return _descriptions; }
        }

        private QField() { }

        public QField(string fieldTypeId, QNode parent, string title, bool displayed, bool displayedActive, List<QInput> inputs, List<string> descriptions, List<QConnector> connectors)
        {
            _fieldTypeId = fieldTypeId;
            _parent = parent;
            _title = title;
            _displayed = displayed;
            _displayedActive = displayedActive;
            _queried = false;
            _negated = false;
            _visible = true;

            _inputs = new Dictionary<string, List<QInput>>();
            foreach(QInput i in inputs)
                _inputs.Add(i.Name, new List<QInput>());
            _inputTemplate = inputs;

            _descriptions = descriptions;

            _connectors = new Dictionary<string, QConnector>();
            foreach(QConnector c in connectors)
                _connectors.Add(c.Name, c);

            _valuesetCount = 0;
        }

        public void Validate(string name, string value)
        {
            if(!_inputs.ContainsKey(name))
            {
                bool isValidInputName = false;
                foreach(string inputName in _inputs.Keys)
                    if(String.Format("{0}{1}", this.FieldTypeId, inputName).Equals(name))
                        isValidInputName = true;

                if(!isValidInputName)
                    throw new DataValidationException("Validation Error: Unexpected value type");
            }

            foreach(QInput i in _inputTemplate)
                if(i.Name.Equals(name))
                    i.Validate(value);
        }

        public void AddValueset()
        {
            _queried = true;
            _valuesetCount += 1;
            foreach(QInput i in _inputTemplate)
                _inputs[i.Name].Add((QInput) i.Clone());
        }

        public override string ToString()
        {
            List<string> inputDisplay = new List<string>();
            foreach(string key in Inputs.Keys)
            {
                inputDisplay.Add("");
                inputDisplay[inputDisplay.Count - 1] += key + " = {";

                List<string> inputDisplayVals = new List<string>();
                foreach(QInput vals in Inputs[key])
                    inputDisplayVals.Add(vals.Value);
                inputDisplay[inputDisplay.Count - 1] += String.Join(", ", inputDisplayVals.ToArray());

                inputDisplay[inputDisplay.Count - 1] += "}";
            }

            return String.Format("{0} Field: {1}", Title, String.Join("; ", inputDisplay.ToArray()));
        }

        public List<QInput> SelectInputs(string inputLabel)
        {
            List<QInput> results = new List<QInput>();

            if (Inputs.ContainsKey(inputLabel))
            {
                results.AddRange(Inputs[inputLabel]);
            }
            return results;
        }

        public List<string> SelectInputValues(string inputLabel)
        {
            List<string> results = new List<string>();
            if (Inputs.ContainsKey(inputLabel))
            {
                foreach (QInput i in Inputs[inputLabel])
                {
                    if (!string.IsNullOrEmpty(i.Value))
                        results.Add(i.Value);
                }
            }
            return results;
        }

        public QInput SelectSingleInput(string inputLabel)
        {
            List<QInput> i = SelectInputs(inputLabel);
            if (i.Count < 1)
                return null;
            return i[0];
        }

        public string SelectSingleInputValue(string inputLabel)
        {
            List<string> v = SelectInputValues(inputLabel);
            if (v.Count < 1)
                return null;
            return v[0];
        }
    }
}