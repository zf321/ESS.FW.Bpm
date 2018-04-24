using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///      
    /// </summary>
    public abstract class ClassNameUtil
    {
        protected internal static readonly IDictionary<Type, string> CachedNames =
            new ConcurrentDictionary<Type, string>();

        public static string GetClassNameWithoutPackage(object @object)
        {
            return GetClassNameWithoutPackage(@object.GetType());
        }

        public static string GetClassNameWithoutPackage(Type clazz)
        {
            var unqualifiedClassName =CachedNames.ContainsKey(clazz)? CachedNames[clazz]:null;
            if (unqualifiedClassName== null)
            {
                unqualifiedClassName = GetTypeName(clazz);
                CachedNames[clazz] = unqualifiedClassName;
            }
            return unqualifiedClassName;
        }
        private static string GetTypeName(Type type)
        {
            string name = type.Name;
            if (type.IsGenericType)
            {
                name = name.Substring(0, name.IndexOf('`')) + "<";
                foreach (var item in type.GenericTypeArguments)
                {
                    name += GetTypeName(item);
                }
                name += ">";
            }
            return name;
        }
    }
}