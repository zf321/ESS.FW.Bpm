using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    ///     A <seealso cref="ELResolver" /> for dynamic bean properties
    ///     
    /// </summary>
    public class DynamicBeanPropertyElResolver : ELResolver
    {
        protected internal string ReadMethodName;

        protected internal bool ReadOnly;

        protected internal Type Subject;

        protected internal string WriteMethodName;

        public DynamicBeanPropertyElResolver(bool readOnly, Type subject, string readMethodName, string writeMethodName)
        {
            this.ReadOnly = readOnly;
            this.Subject = subject;
            this.ReadMethodName = readMethodName;
            this.WriteMethodName = writeMethodName;
        }

        public DynamicBeanPropertyElResolver(Type subject, string readMethodName, string writeMethodName)
            : this(false, subject, readMethodName, writeMethodName)
        {
        }

        public override Type GetCommonPropertyType(ELContext context, object @base)
        {
            if (Subject.IsInstanceOfType(@base))
                return typeof(object);
            return null;
        }

        public override IEnumerator<MemberInfo> GetMemberInfos(ELContext context, object @base)
        {
            return null;
        }

        public override Type GetType(ELContext context, object @base, object property)
        {
            return typeof(object);
        }

        public override object GetValue(ELContext context, object @base, object property)
        {
            if ((@base == null) || (GetCommonPropertyType(context, @base) == null))
                return null;

            var propertyName = property.ToString();

            try
            {
                var value = ReflectUtil.Invoke(@base, ReadMethodName, new object[] {propertyName});
                context.PropertyResolved = true;
                return value;
            }
            catch (System.Exception e)
            {
                throw new ELException(e);
            }
        }

        public override bool IsReadOnly(ELContext context, object @base, object property)
        {
            return ReadOnly;
        }

        public override void SetValue(ELContext context, object @base, object property, object value)
        {
            if ((@base == null) || (GetCommonPropertyType(context, @base) == null))
                return;

            var propertyName = property.ToString();
            try
            {
                ReflectUtil.Invoke(@base, WriteMethodName, new[] {propertyName, value});
                context.PropertyResolved = true;
            }
            catch (System.Exception e)
            {
                throw new ELException(e);
            }
        }
    }
}