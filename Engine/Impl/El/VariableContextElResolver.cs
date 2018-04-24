using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     
    /// </summary>
    public class VariableContextElResolver : ELResolver
    {
        public const string VarCtxKey = "variableContext";

        public override object GetValue(ELContext context, object @base, object property)
        {
            var variableContext = (IVariableContext) context.GetContext(typeof(IVariableContext));
            if (variableContext != null)
            {
                if (VarCtxKey.Equals(property))
                {
                    context.PropertyResolved = true;
                    return variableContext;
                }
                var typedValue = variableContext.Resolve((string) property);
                if (typedValue != null)
                {
                    context.PropertyResolved = true;
                    return Unpack(typedValue);
                }
                    
            }
            return null;
        }

        public override void SetValue(ELContext context, object @base, object property, object value)
        {
            // read only
        }

        public override bool IsReadOnly(ELContext context, object @base, object property)
        {
            // always read only
            return true;
        }

        public override Type GetCommonPropertyType(ELContext arg0, object arg1)
        {
            return typeof(object);
        }

        public override IEnumerator<MemberInfo> GetMemberInfos(ELContext arg0, object arg1)
        {
            return null;
        }

        public override Type GetType(ELContext arg0, object arg1, object arg2)
        {
            return typeof(object);
        }

        protected internal virtual object Unpack(ITypedValue typedValue)
        {
            if (typedValue != null)
            {
                return typedValue.Value;

            }
            return null;
        }
    }
}