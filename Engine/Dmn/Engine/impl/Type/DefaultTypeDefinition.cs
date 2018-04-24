using System;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.type
{
    /// <summary>
    ///     
    /// </summary>
    public class DefaultTypeDefinition : IDmnTypeDefinition
    {
        public virtual ITypedValue Transform(object value)
        {
            return Variables.UnTypedValue(value);
        }

        public virtual string TypeName
        {
            get { return "untyped"; }
        }
        

        public override string ToString()
        {
            return "DefaultTypeDefinition []";
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            return true;
        }
    }
}