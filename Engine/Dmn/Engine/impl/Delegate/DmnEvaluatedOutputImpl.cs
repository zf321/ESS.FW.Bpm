using System;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.@delegate
{
    public class DmnEvaluatedOutputImpl : IDmnEvaluatedOutput
    {
        protected internal string id;
        protected internal string name;
        protected internal string outputName;
        protected internal ITypedValue value;

        public DmnEvaluatedOutputImpl(DmnDecisionTableOutputImpl decisionTableOutput, ITypedValue value)
        {
            id = decisionTableOutput.Id;
            name = decisionTableOutput.Name;
            outputName = decisionTableOutput.OutputName;
            this.value = value;
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


        public virtual string OutputName
        {
            get { return outputName; }
            set { outputName = value; }
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

            var that = (DmnEvaluatedOutputImpl) o;

            if (!ReferenceEquals(id, null) ? !id.Equals(that.id) : !ReferenceEquals(that.id, null))
                return false;
            if (!ReferenceEquals(name, null) ? !name.Equals(that.name) : !ReferenceEquals(that.name, null))
                return false;
            if (!ReferenceEquals(outputName, null)
                ? !outputName.Equals(that.outputName)
                : !ReferenceEquals(that.outputName, null))
                return false;
            return !(value != null ? !value.Equals(that.value) : that.value != null);
        }

        public override int GetHashCode()
        {
            var result = !ReferenceEquals(id, null) ? id.GetHashCode() : 0;
            result = 31*result + (!ReferenceEquals(name, null) ? name.GetHashCode() : 0);
            result = 31*result + (!ReferenceEquals(outputName, null) ? outputName.GetHashCode() : 0);
            result = 31*result + (value != null ? value.GetHashCode() : 0);
            return result;
        }

        public override string ToString()
        {
            return "DmnEvaluatedOutputImpl{" + "id='" + id + '\'' + ", name='" + name + '\'' + ", outputName='" +
                   outputName + '\'' + ", value=" + value + '}';
        }
    }
}