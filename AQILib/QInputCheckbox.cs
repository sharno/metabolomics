using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    public abstract class QInputCheckbox : QInput
    {
        private QInputCheckbox()
        { }

        public QInputCheckbox(string name, QInputRenderer renderer)
            : base(name, "false", renderer)
        { }

        public QInputCheckbox(string name, bool defaultValue, QInputRenderer renderer)
            : base(name, defaultValue ? "true" : "false", renderer)
        { }

        protected QInputCheckbox(QInputCheckbox input)
            : base(input)
        { }

        public override void Validate(string value)
        {
            base.Validate(value);

            if(!value.Equals("true") && !value.Equals("false"))
                throw new DataValidationException("Illegal input value for " + Name);

            return;
        }
    }
}