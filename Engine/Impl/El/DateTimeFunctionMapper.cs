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
    public class DateTimeFunctionMapper : FunctionMapper
    {
        public static IDictionary<string, MethodInfo> DateTimeFunctionMap;

        public override MethodInfo ResolveFunction(string prefix, string localName)
        {
            // Context functions are used un-prefixed
            EnsureContextFunctionMapInitialized();
            MethodInfo m;
            DateTimeFunctionMap.TryGetValue(localName, out m);
            return m;
        }

        protected internal virtual void EnsureContextFunctionMapInitialized()
        {
            if (DateTimeFunctionMap == null)
                lock (typeof(DateTimeFunctionMapper))
                {
                    if (DateTimeFunctionMap == null)
                    {
                        DateTimeFunctionMap = new Dictionary<string, MethodInfo>();
                        CreateMethodBindings();
                    }
                }
        }

        protected internal virtual void CreateMethodBindings()
        {
            var mapperClass = GetType();
            DateTimeFunctionMap["Now"] = ReflectUtil.GetMethod(mapperClass, "Now");
            DateTimeFunctionMap["DateTime"] = ReflectUtil.GetMethod(mapperClass, "DateTime");
        }

        public static DateTime Now()
        {
            return ClockUtil.CurrentTime;
        }

        public static DateTime DateTime()
        {
            return new DateTime();
        }
    }
}