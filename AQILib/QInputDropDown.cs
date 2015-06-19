using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    public abstract class QInputDropDown : QInput
    {
        private QInputDropDown() { }

        public QInputDropDown(string name, QInputRenderer renderer)
            : base(name, renderer)
        { }

        public QInputDropDown(string name, string defaultValue, QInputRenderer renderer)
            : base(name, defaultValue, renderer)
        { }

        protected QInputDropDown(QInputDropDown input)
            : base(input)
        { }

        //public abstract Dictionary<string, string> GetInitialValues();

        public override void Validate(string value)
        {
            base.Validate(value);

            if(!GetInitialValues().ContainsKey(value))
                throw new DataValidationException("Illegal input value for " + Name);

            return;
        }
    }
}