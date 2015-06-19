using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;

namespace AQILib
{
    /// <summary>
    /// A base type for an AQI node
    /// 
    /// Query building notes: 
    ///     {0} = unique NodeID, 
    ///     {1} = display ID [unique per type], 
    ///     {2} = field value, 
    ///     {3} = join parent NodeID, 
    ///     {4} = join child NodeID
    /// </summary>
    public abstract class QNode
    {
        #region Static Members: Node Construction --- _nodeConstructors, InitializeTypes, CreateInstance, GetTypes

        private static Dictionary<string, ConstructorInfo> _nodeConstructors = new Dictionary<string,ConstructorInfo>();
        
        public static void InitializeTypes(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();

            foreach(Type t in types)
            {
                if(t.IsSubclassOf(typeof(QNode)) && t.GetCustomAttributes(typeof(QNodeTypeAttribute), false).Length > 0)
                {
                    _nodeConstructors[((QNodeTypeAttribute[]) t.GetCustomAttributes(typeof(QNodeTypeAttribute), false))[0].NodeTypeName] = t.GetConstructor(new Type[] { typeof(QNode) });
                    CreateInstance(((QNodeTypeAttribute[]) t.GetCustomAttributes(typeof(QNodeTypeAttribute), false))[0].NodeTypeName, null);
                }
            }
        }

        public static QNode CreateInstance(string nodeType, QNode parent)
        {
            if(!_nodeConstructors.ContainsKey(nodeType))
                throw new ArgumentException("Invalid nodeType: " + nodeType);

            return (QNode) _nodeConstructors[nodeType].Invoke(new object[] { parent });
        }

        public static List<string> GetTypes()
        {
            return new List<string>(_nodeConstructors.Keys);
        }

        #endregion

        #region Static Member: Parsing --- Parse

        public static QNode Parse(XmlElement query, QNode parent)
        {
            QNode rootNode = QNode.CreateInstance(query.Attributes["type"].Value, parent);
            rootNode.Parse(query);
            return rootNode;
        }

        #endregion

        #region Data Structures: private data types and public properties

        private string _nodeTypeName;
        private int _nodeTypeId;
        private string _title;

        private Dictionary<string, QField> _fields;
        private QNode _parent;
        private List<QNode> _children;
        private Dictionary<string, QLink> _links; // <displayThisAfterFieldId, QLink Object>
        private Dictionary<string, QRelationship> _relationships;
        private bool _negated;
        private bool _editable;

        private Querier _querier;
        private IQueryResults _queryResult;
        private bool _queryResultIsValid;
        private IQueryRenderer _queryRenderer;

        private QNodeRenderer _rendererNode;
        private QFieldRenderer _rendererField;

        public QNode this[string id]
        {
            get
            {
                Queue<QNode> nodes = new Queue<QNode>();
                nodes.Enqueue(this);

                while(nodes.Count > 0)
                {
                    QNode currentNode = nodes.Dequeue();
                    if(currentNode.Id == id)
                        return currentNode;

                    foreach(QNode child in currentNode.Children)
                        nodes.Enqueue(child);
                }

                return null;
            }
        }

        public string NodeTypeName
        {
            get { return _nodeTypeName; }
        }

        public int NodeTypeId
        {
            get { return _nodeTypeId; }
        }

        public string Id
        {
            get
            {
                if(this.Parent == null)
                    return String.Format("{0}-{1}", this.NodeTypeName, this.NodeTypeId);
                else
                    return String.Format("{0}{1}{2}-{3}", this.Parent.Id, this.Parent.Id.Length == 0 ? "" : ":", this.NodeTypeName, this.NodeTypeId);
            }
        }

        public string Title
        {
            get { return _title; }
        }

        public Dictionary<string, QField> Fields
        {
            get { return _fields; }
        }

        public QNode Parent
        {
            get { return _parent; }
        }

        public List<QNode> Children
        {
            get { return _children; }
        }

        public Dictionary<string, QLink> Links
        {
            get { return _links; }
        }

        public bool Negated
        {
            get { return _negated; }
        }

        public bool Editable
        {
            get { return _editable; }
        }

        public Querier Querier
        {
            get { InvalidateQueryResults(); return _querier; }
        }

        public IQueryRenderer QueryRenderer
        {
            get { return _queryRenderer; }
        }

        public QNodeRenderer RendererNode
        {
            get { return _rendererNode; }
        }

        public QFieldRenderer RendererField
        {
            get { return _rendererField; }
        }

        #endregion

        #region Constructors

        protected QNode() { }

        protected QNode(string nodeType, string title, QNode parent, Querier querier, IQueryRenderer queryRenderer, QNodeRenderer rendererNode, QFieldRenderer rendererField)
        {
            _nodeTypeName = nodeType;
            _title = title;

            _fields = new Dictionary<string, QField>();
            _parent = parent;
            _children = new List<QNode>();
            _links = new Dictionary<string, QLink>();
            _relationships = new Dictionary<string, QRelationship>();
            _negated = false;
            _editable = true;

            _querier = querier;
            _queryResult = null;
            _queryResultIsValid = false;
            _queryRenderer = queryRenderer;

            _rendererNode = rendererNode;
            _rendererField = rendererField;
        }

        #endregion

        protected void AddField(string fieldTypeId, string title, QInput[] inputs, string[] descriptions, QConnector[] connectors)
        {
            AddField(fieldTypeId, title, false, false, inputs, descriptions, connectors);
        }

        protected void AddField(string fieldTypeId, string title, bool displayed, QInput[] inputs, string[] descriptions, QConnector[] connectors)
        {
            AddField(fieldTypeId, title, displayed, true, inputs, descriptions, connectors);
        }

        private void AddField(string fieldTypeId, string title, bool displayed, bool displayedActive, QInput[] inputs, string[] descriptions, QConnector[] connectors)
        {
            List<QInput> i = new List<QInput>();
            i.AddRange(inputs);

            List<string> d = new List<string>();
            d.AddRange(descriptions);
            if(i.Count + 1 != d.Count)
                throw new ArgumentOutOfRangeException("descriptions", descriptions, String.Format("Descriptions array has an invalid length of {0}; must have length {1} (based on inputs)", d.Count, i.Count + 1));

            List<QConnector> c = new List<QConnector>();
            c.AddRange(connectors);
            //if(i.Count != c.Count)
            //    throw new ArgumentOutOfRangeException("connectors", connectors, String.Format("Connectors array has an invalid length of {0}; must have length {1} (based on inputs)", c.Count, i.Count));

            _fields.Add(fieldTypeId, new QField(fieldTypeId, this, title, displayed, displayedActive, i, d, c));

            foreach(QInput input in _fields[fieldTypeId].InputTemplate)
                input.Parent = _fields[fieldTypeId];

            InvalidateQueryResults();
        }

        protected void AddLink(string linkTypeId, int maxCardinality, string displayThisAfterFieldId, string title, string[] linkedNodeTypes, string[] linkTexts, string[] linkMouseOverTexts, string[] descriptions)
        {
            List<string> lnt = new List<string>(linkedNodeTypes);

            List<string> lt = new List<string>(linkTexts);
            if(lnt.Count != lt.Count)
                throw new ArgumentOutOfRangeException("linkTexts", linkTexts, String.Format("Link Texts array has an invalid length of {0}; must have length {1} (based on linkedNodeTypes)", lt.Count, lnt.Count));

            List<string> lmot = new List<string>(linkMouseOverTexts);
            if(lnt.Count != lmot.Count)
                throw new ArgumentOutOfRangeException("linkMouseOverTexts", linkMouseOverTexts, String.Format("Link Mouse Over Texts array has an invalid length of {0}; must have length {1} (based on linkedNodeTypes)", lmot.Count, lnt.Count));

            List<string> d = new List<string>(descriptions);
            if(lnt.Count + 1 != d.Count)
                throw new ArgumentOutOfRangeException("descriptions", descriptions, String.Format("Descriptions array has an invalid length of {0}; must have length {1} (based on linkedNodeTypes)", d.Count, lnt.Count));

            _links.Add(displayThisAfterFieldId, new QLink(linkTypeId, displayThisAfterFieldId, maxCardinality, title, lnt, lt, lmot, d));

            InvalidateQueryResults();
        }

        protected void AddChild(XmlElement query)
        {
            QNode child = QNode.Parse(query, this);
            _children.Add(child);
            InvalidateQueryResults();
        }

        protected void AddRelationship(string childNodeType, QRelationship.RelationshipConstraintDelegate[] relationshipConstraints)
        {
            List<QRelationship.RelationshipConstraintDelegate> rc = new List<QRelationship.RelationshipConstraintDelegate>();
            rc.AddRange(relationshipConstraints);
            _relationships[childNodeType] = new QRelationship(this, childNodeType, rc);
            InvalidateQueryResults();
        }

        /// <summary>
        /// Parse a query
        /// </summary>
        /// <param name="query">The 'node' element in the XML where the parsing will begin</param>
        public void Parse(XmlElement query)
        {
            // Set the node negation attribute
            string negate = query.Attributes["negate"].Value;
            if(negate.Equals("yes", StringComparison.OrdinalIgnoreCase) || negate.Equals("y", StringComparison.OrdinalIgnoreCase) || negate.Equals("1", StringComparison.OrdinalIgnoreCase))
                _negated = true;
            else
                _negated = false;

            // Set the node editable attribute
            if(query.HasAttribute("editable"))
            {
                string editable = query.Attributes["editable"].Value;
                if(editable.Equals("yes", StringComparison.OrdinalIgnoreCase) || editable.Equals("y", StringComparison.OrdinalIgnoreCase) || editable.Equals("1", StringComparison.OrdinalIgnoreCase))
                    _editable = true;
                else
                    _editable = false;
            }
            else
            {
                _editable = true;
            }

            // Set the type ID attribute
            _nodeTypeId = int.Parse(query.Attributes["typeId"].Value);

            // For each field and sub-node,
            foreach(XmlElement child in query.ChildNodes)
            {
                // If it's a field, then parse it
                if(child.Name.Equals("field"))
                {
                    // Get the field and store in f
                    QField f = Fields[child.Attributes["type"].Value];

                    // Set f's display attribute
                    if(f.DisplayedActive)
                    {
                        string display = child.Attributes["display"].Value;
                        if(display.Equals("yes", StringComparison.OrdinalIgnoreCase) || display.Equals("y", StringComparison.OrdinalIgnoreCase) || display.Equals("1", StringComparison.OrdinalIgnoreCase))
                            f.Displayed = true;
                        else
                            f.Displayed = false;
                    }

                    // Set f's negation attribute
                    negate = child.Attributes["negate"].Value;
                    if(negate.Equals("yes", StringComparison.OrdinalIgnoreCase) || negate.Equals("y", StringComparison.OrdinalIgnoreCase) || negate.Equals("1", StringComparison.OrdinalIgnoreCase))
                        f.Negated = true;
                    else
                        f.Negated = false;

                    // Set f's visible attribute
                    if(child.HasAttribute("visible"))
                    {
                        string visible = child.Attributes["visible"].Value;
                        if(visible.Equals("yes", StringComparison.OrdinalIgnoreCase) || visible.Equals("y", StringComparison.OrdinalIgnoreCase) || visible.Equals("1", StringComparison.OrdinalIgnoreCase))
                            f.Visible = true;
                        else
                            f.Visible = false;
                    }
                    else
                    {
                        f.Visible = true;
                    }

                    // Set f's connector attribute
                    f.Connector = child.Attributes["connector"].Value;

                    // For each of the value sets, add it to f
                    foreach(XmlElement valueset in child.ChildNodes)
                    {
                        f.AddValueset();

                        foreach(XmlElement valuePair in valueset.ChildNodes)
                        {
                            List<QInput> inputList;
                            if(f.Inputs.ContainsKey(valuePair.Attributes["name"].Value))
                            {
                                inputList = f.Inputs[valuePair.Attributes["name"].Value];
                            }
                            else
                            {
                                inputList = null;
                                foreach(string inputName in f.Inputs.Keys)
                                    if(String.Format("{0}{1}", f.FieldTypeId, inputName).Equals(valuePair.Attributes["name"].Value))
                                        inputList = f.Inputs[inputName];
                            }
                            inputList[inputList.Count - 1].Value = inputList[inputList.Count - 1].ParseValue(valuePair.Attributes["value"].Value);
                        }
                    }
                }

                // If it's a node, then it's a child of this node. So parse it and add it to this node.
                if(child.Name.Equals("node"))
                    AddChild(child);
            }
        }

        public IQueryResults Query()
        {
            if(!_queryResultIsValid)
                _queryResult = _querier.Query(this);
            return _queryResult;
        }

        public IGuiComponent RenderQuery(IQueryResults results, IGuiData data, out IGuiData dataOut)
        {
            return _queryRenderer.Render(this, results, data, out dataOut);
        }

        public IGuiComponent RenderNode(IGuiData data)
        {
            return _rendererNode.Render(this, data);
        }

        public IGuiComponent RenderField(IGuiData data)
        {
            return _rendererField.Render(this, data);
        }

        public Dictionary<string, string> GetInputValues(string fieldId, string nodeType, string fieldTypeId, string inputType, string queryString, string treeXmlString, int topkMatches)
        {
            return Querier.GetInputValues(this, fieldId, nodeType, fieldTypeId, inputType, queryString, treeXmlString, topkMatches);
        }

        private void InvalidateQueryResults()
        {
            _queryResult = null;
            _queryResultIsValid = false;
        }

        public bool IsValidRelationship(string childNodeType)
        {
            return _relationships.ContainsKey(childNodeType) && _relationships[childNodeType].IsValid;
        }

        public override string ToString()
        {
            return String.Format("Node {0}: ({1},{2})", NodeTypeName, Fields.Count, Children.Count);
        }

        /// <summary>
        /// Get the first immediate child QNode by node type identifier
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public QNode GetOneChild(string nodeType)
        {
            foreach (QNode child in Children)
            {
                if (child.NodeTypeName == nodeType)
                    return child;
            }
            return null;
        }

        /// <summary>
        /// Returns all immediate children QNodes by node type identifier.
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public List<QNode> GetChildren(params string[] nodeTypes)
        {
            List<QNode> results = new List<QNode>();
            foreach (QNode child in Children)
            {
                foreach (string nodeType in nodeTypes)
                {
                    if (child.NodeTypeName == nodeType)
                    {
                        results.Add(child);
                        break;
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Returns all immediate child QFields by field type identifier
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        public List<QField> GetFields(string fieldType)
        {
            List<QField> results = new List<QField>();
            foreach (QField f in Fields.Values)
            {
                if (f.FieldTypeId == fieldType)
                    results.Add(f);
            }
            return results;
        }

        /// <summary>
        /// Find a QField by unique ID located in this node or one of its descendants
        /// </summary>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public QField GetField(string fieldId)
        {
            // Split the fieldId into its subparts
            string fieldIdNode = fieldId.Split(';')[0];
            string fieldIdField = fieldId.Split(';')[1];

            QNode node = GetNode(fieldIdNode);

            if (node == null)
                return null;

            // Get the field type from the fieldId
            string fieldIdFieldType = fieldIdField.Split('-')[0];

            if (!node.Fields.ContainsKey(fieldIdFieldType))
                return null;

            return node.Fields[fieldIdFieldType];
        }

        /// <summary>
        /// Find a QNode by unique ID that may be this node or one of its descendants
        /// </summary>
        /// <param name="descendantOrSelfNodeId"></param>
        /// <returns></returns>
        public QNode GetNode(string descendantOrSelfNodeId)
        {
            return this[descendantOrSelfNodeId];
        }

        public QNode Root
        {
            get
            {
                QNode p = this;
                while (p.Parent != null)
                    p = p.Parent;
                return p;
            }
        }

        #region QNode Path Querying

        /// <summary>
        /// QNode Path Querying format:
        /// Find child with label 'Node1': 'Node1'
        /// Find field with label 'Field1': '@Field1'
        /// Find field 'F1' of child 'N1': 'N1@F1'
        /// Find children of children: 'Node1/Node2'
        /// Find input 'I1' of field 'F1': '@F1:I1'
        /// 
        /// More generally queries take the form:
        /// [(/NodeLabel)*][@FieldLabel][:InputLabel]
        /// </summary>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public List<QNode> SelectNodes(string path)
        {
            List<QNode> currentLevel = new List<QNode>();
            currentLevel.Add(this);

            string[] steps = path.Split('/');

            List<QNode> nextLevel = new List<QNode>();

            for (int i = 0; i < steps.Length; i++)
            {
                string step = steps[i];
                if (step == "")
                    continue;

                foreach (QNode n in currentLevel)
                {
                    nextLevel.AddRange(n.GetChildren(step));
                }
                currentLevel.Clear();
                currentLevel.AddRange(nextLevel);
                nextLevel.Clear();
            }
            return currentLevel;
        }

        public List<QField> SelectFields(string path)
        {
            string nodePath = path.Split('@')[0];
            string fieldLabel = path.Split('@')[1];

            List<QField> results = new List<QField>();
            List<QNode> nodes = SelectNodes(nodePath);
            foreach (QNode n in nodes)
            {
                results.AddRange(n.GetFields(fieldLabel));
            }
            return results;
        }

        public List<QInput> SelectInputs(string path)
        {
            string fieldPath = path.Split(':')[0];
            string inputLabel = path.Split(':')[1];

            List<QInput> results = new List<QInput>();
            List<QField> fields = SelectFields(fieldPath);
            foreach (QField f in fields)
            {
                if (f.Inputs.ContainsKey(inputLabel))
                {
                    results.AddRange(f.Inputs[inputLabel]);
                }
            }
            return results;
        }

        public List<string> SelectInputValues(string nodePath)
        {
            List<string> results = new List<string>();
            List<QInput> inputs = SelectInputs(nodePath);

            foreach (QInput i in inputs)
            {
                if (!string.IsNullOrEmpty(i.Value))
                    results.Add(i.Value);
            }
            return results;
        }

        public QNode SelectSingleNode(string nodePath)
        {
            List<QNode> n = SelectNodes(nodePath);
            if (n.Count < 1)
                return null;
            return n[0];
        }

        public QField SelectSingleField(string nodePath)
        {
            List<QField> f = SelectFields(nodePath);
            if (f.Count < 1)
                return null;
            return f[0];
        }

        public QInput SelectSingleInput(string nodePath)
        {
            List<QInput> i = SelectInputs(nodePath);
            if (i.Count < 1)
                return null;
            return i[0];
        }

        public string SelectSingleInputValue(string nodePath)
        {
            List<string> v = SelectInputValues(nodePath);
            if (v.Count < 1)
                return null;
            return v[0];
        }

        #endregion
    }
}