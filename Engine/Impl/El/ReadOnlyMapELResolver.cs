using System;
using System.Collections.Generic;
using System.Reflection;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     An <seealso cref="ELResolver" /> that exposed object values in the map, under the name of the entry's key.
    ///     The values in the map are only returned when requested property has no 'base', meaning
    ///     it's a root-object.
    ///     
    /// </summary>
    public class ReadOnlyMapElResolver : ELResolver
    {
        protected internal IDictionary<object, object> WrappedMap;

        public ReadOnlyMapElResolver(IDictionary<object, object> map)
        {
            WrappedMap = map;
        }

        public override object GetValue(ELContext context, object @base, object property)
        {
            if (@base == null)
                if (WrappedMap.ContainsKey(property))
                {
                    context.PropertyResolved = true;
                    return WrappedMap[property];
                }
            return null;
        }

        public override bool IsReadOnly(ELContext context, object @base, object property)
        {
            return true;
        }

        public override void SetValue(ELContext context, object @base, object property, object value)
        {
            if (@base == null)
                if (WrappedMap.ContainsKey(property))
                    throw new ProcessEngineException("Cannot set value of '" + property + "', it's readonly!");
        }

        public override Type GetCommonPropertyType(ELContext context, object arg)
        {
            return typeof(object);
        }

        public override IEnumerator<MemberInfo> GetMemberInfos(ELContext context, object arg)
        {
            return null;
        }

        public override Type GetType(ELContext context, object arg1, object arg2)
        {
            return typeof(object);
        }
    }
}