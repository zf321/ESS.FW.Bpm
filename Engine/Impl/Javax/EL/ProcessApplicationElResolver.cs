using System;
using System.Collections.Generic;
using System.Reflection;
using ESS.FW.Common.Components;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessApplicationElResolver : ELResolver
    {
        public override object GetValue(ELContext context, object @base, object property)
        {
            if (property != null)
            {
                var @object = context.GetContext(typeof(IScope));
                if (@object != null)
                {
                    var variableScope = (Dictionary<string,Type>)@object;
                    Type type;
                    variableScope.TryGetValue(property.ToString(), out type);
                    if (type != null)
                    {
                        if (IsResolvable(type))
                        {
                            if (Context.CommandContext.Scope != null)
                            {
                                if (Context.CommandContext.Scope.IsRegistered(type))
                                {
                                    var val = Context.CommandContext.Scope.Resolve(type);
                                    context.PropertyResolved = true;
                                    return val;
                                }
                            }
                            else
                            {
                                using (var scope = ObjectContainer.BeginLifetimeScope())
                                {
                                    if (scope.IsRegistered(type))
                                    {
                                        var val = scope.Resolve(type);
                                        context.PropertyResolved = true;
                                        return val;
                                    }
                                }
                            }
                        }
                    }
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
        /// <summary>
        /// Test whether the given base should be resolved by this ELResolver.
        /// </summary>
        /// <param name="base">
        ///            The Object to analyze. </param>
        /// <param name="property">
        ///            The name of the property to analyze. Will be coerced to a String. </param>
        /// <returns> base != null </returns>
        private bool IsResolvable(Type @base)
        {
            return ObjectContainer.IsRegistered(@base);
        }
        //protected internal virtual object unpack(ITypedValue typedValue)
        //{
        //    if (typedValue != null)
        //    {
        //        return typedValue.Value;

        //    }
        //    return null;
        //}
    }
}