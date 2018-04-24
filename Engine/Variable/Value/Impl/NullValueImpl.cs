using System;
using ESS.FW.Bpm.Engine.Variable.Type;

namespace ESS.FW.Bpm.Engine.Variable.Value.Impl
{
    /// <summary>
    ///     Untyped Null
    /// </summary>
    [Serializable]
    public class NullValueImpl : ITypedValue
    {
        private const long SerialVersionUid = 1L;

        // null is always null
        public static readonly NullValueImpl Instance = new NullValueImpl();

        private NullValueImpl()
        {
            // hide
        }

        public virtual object Value
        {
            get { return null; }
        }

        public virtual IValueType Type
        {
            get { return ValueTypeFields.Null; }
        }

        public override string ToString()
        {
            return "Untyped 'null' value";
        }
        public static explicit operator string(NullValueImpl value)
        {
            return null;
        }
    }
}