using System;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.type
{
    /// <summary>
    ///     Transform the given value into untyped value.
    /// </summary>
    public class IdentityDataTypeTransformer : IDmnDataTypeTransformer
    {
        public virtual ITypedValue Transform(object value)
        {
            return Variables.UnTypedValue(value);
        }
    }
}