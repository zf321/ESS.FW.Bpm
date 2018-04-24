using System;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.@delegate
{
    public class DmnEvaluatedInputImpl : IDmnEvaluatedInput
    {
        protected internal string id;
        protected internal string inputVariable;
        protected internal string name;
        protected internal ITypedValue value;

        public DmnEvaluatedInputImpl(DmnDecisionTableInputImpl input)
        {
            id = input.Id;
            name = input.Name;
            inputVariable = input.InputVariable;
        }

        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }


        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }


        public virtual string InputVariable
        {
            get { return inputVariable; }
            set { inputVariable = value; }
        }


        public virtual ITypedValue Value
        {
            get { return value; }
            set { this.value = value; }
        }


        public override bool Equals(object o)
        {
            if (this == o)
                return true;
            if ((o == null) || (GetType() != o.GetType()))
                return false;

            var that = (DmnEvaluatedInputImpl) o;

            if (!ReferenceEquals(id, null) ? !id.Equals(that.id) : !ReferenceEquals(that.id, null))
                return false;
            if (!ReferenceEquals(name, null) ? !name.Equals(that.name) : !ReferenceEquals(that.name, null))
                return false;
            if (!ReferenceEquals(inputVariable, null)
                ? !inputVariable.Equals(that.inputVariable)
                : !ReferenceEquals(that.inputVariable, null))
                return false;
            return !(value != null ? !value.Equals(that.value) : that.value != null);
        }

        public override int GetHashCode()
        {
            var result = !ReferenceEquals(id, null) ? id.GetHashCode() : 0;
            result = 31*result + (!ReferenceEquals(name, null) ? name.GetHashCode() : 0);
            result = 31*result + (!ReferenceEquals(inputVariable, null) ? inputVariable.GetHashCode() : 0);
            result = 31*result + (value != null ? value.GetHashCode() : 0);
            return result;
        }

        public override string ToString()
        {
            return "DmnEvaluatedInputImpl{" + "id='" + id + '\'' + ", name='" + name + '\'' + ", inputVariable='" +
                   inputVariable + '\'' + ", value=" + value + '}';
        }
    }
}