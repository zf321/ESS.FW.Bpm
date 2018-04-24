

using System;

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    ///     This class encapsulates a base model object and one of its properties.
    ///     
    /// </summary>
    [Serializable]
    public class ValueReference
    {
        private const long SerialVersionUid = 1L;

        public ValueReference(object @base, object property)
        {
            Base = @base;
            Property = property;
        }

        public virtual object Base { get; }

        public virtual object Property { get; }
    }
}