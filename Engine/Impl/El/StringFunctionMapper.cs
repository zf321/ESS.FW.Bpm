using System;
using System.Collections.Generic;
using System.Reflection;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.EL
{

    /// <summary>
    ///     
    /// </summary>
    public class StringFunctionMapper : FunctionMapper
    {
        public static IDictionary<string, MethodInfo> StringFunctionMap;

        public override MethodInfo ResolveFunction(string prefix, string localName)
        {
            // Context functions are used un-prefixed
            EnsureContextFunctionMapInitialized();
            MethodInfo m;
            StringFunctionMap.TryGetValue(localName, out m);
            return m;
        }

        protected internal virtual void EnsureContextFunctionMapInitialized()
        {
            if (StringFunctionMap == null)
                lock (typeof(StringFunctionMapper))
                {
                    if (StringFunctionMap == null)
                    {
                        StringFunctionMap = new Dictionary<string, MethodInfo>();
                        CreateMethodBindings();
                    }
                }
        }

        protected internal virtual void CreateMethodBindings()
        {
            StringFunctionMap["IsNullOrEmpty"] = ReflectUtil.GetMethod(typeof(string), "IsNullOrEmpty",new Type[]{typeof(string)});
        }
        
    }
}