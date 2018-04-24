using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class AbstractElResolverDelegate : ELResolver
    {
        protected internal abstract ELResolver ElResolverDelegate { get; }

        public override Type GetCommonPropertyType(ELContext context, object @base)
        {
            var @delegate = ElResolverDelegate;
            if (@delegate == null)
                return null;
            return @delegate.GetCommonPropertyType(context, @base);
        }

        public override IEnumerator<MemberInfo> GetMemberInfos(ELContext context, object @base)
        {
            ELResolver @delegate = ElResolverDelegate;
            if (@delegate == null)
            {
                return System.Linq.Enumerable.Empty<MemberInfo>().GetEnumerator();
            }
            else
            {
                return @delegate.GetMemberInfos(context, @base);
            }
        }


        public override Type GetType(ELContext context, object @base, object property)
        {
            context.PropertyResolved = false;
            var @delegate = ElResolverDelegate;
            if (@delegate == null)
                return null;
            return @delegate.GetType(context, @base, property);
        }


        public override object GetValue(ELContext context, object @base, object property)
        {
            context.PropertyResolved = false;
            var @delegate = ElResolverDelegate;
            if (@delegate == null)
                return null;
            return @delegate.GetValue(context, @base, property);
        }

        public override bool IsReadOnly(ELContext context, object @base, object property)
        {
            context.PropertyResolved = false;
            var @delegate = ElResolverDelegate;
            if (@delegate == null)
                return true;
            return @delegate.IsReadOnly(context, @base, property);
        }

        public override void SetValue(ELContext context, object @base, object property, object value)
        {
            context.PropertyResolved = false;
            var @delegate = ElResolverDelegate;
            if (@delegate != null)
                @delegate.SetValue(context, @base, property, value);
        }

        public override object Invoke(ELContext context, object @base, object method, Type[] paramTypes,
            object[] @params)
        {
            context.PropertyResolved = false;
            var @delegate = ElResolverDelegate;
            if (@delegate == null)
                return null;
            return @delegate.Invoke(context, @base, method, paramTypes, @params);
        }
    }
}