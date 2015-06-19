using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    public class QInputText : QInput
    {
        private QInputText() { }

        public QInputText(string name, QInputRenderer renderer)
            : base(name, renderer)
        { }

        public QInputText(string name, string defaultValue, QInputRenderer renderer)
            : base(name, defaultValue, renderer)
        { }

        protected QInputText(QInputText input)
            : base(input)
        { }

        public override object Clone()
        {
            return new QInputText(this);
        }
    }
}