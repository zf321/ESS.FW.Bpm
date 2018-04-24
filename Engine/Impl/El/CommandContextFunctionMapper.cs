using System.Collections.Generic;
using System.Reflection;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     
    /// </summary>
    public class CommandContextFunctionMapper : FunctionMapper
    {
        public static IDictionary<string, MethodInfo> CommandContextFunctionMap;

        public override MethodInfo ResolveFunction(string prefix, string localName)
        {
            // Context functions are used un-prefixed
            EnsureContextFunctionMapInitialized();
            MethodInfo m;
            CommandContextFunctionMap.TryGetValue(localName, out m);
            return m;
        }

        protected internal virtual void EnsureContextFunctionMapInitialized()
        {
            if (CommandContextFunctionMap == null)
                lock (typeof(CommandContextFunctionMapper))
                {
                    if (CommandContextFunctionMap == null)
                    {
                        CommandContextFunctionMap = new Dictionary<string, MethodInfo>();
                        CreateMethodBindings();
                    }
                }
        }

        protected internal virtual void CreateMethodBindings()
        {
            var mapperClass = GetType();
            CommandContextFunctionMap["currentUser"] = ReflectUtil.GetMethod(mapperClass, "currentUser");
            CommandContextFunctionMap["currentUserGroups"] = ReflectUtil.GetMethod(mapperClass, "currentUserGroups");
        }

        public static string CurrentUser()
        {
            var commandContext = Context.CommandContext;
            if (commandContext != null)
                return commandContext.AuthenticatedUserId;
            return null;
        }

        public static IList<string> CurrentUserGroups()
        {
            var commandContext = Context.CommandContext;
            if (commandContext != null)
                return commandContext.AuthenticatedGroupIds;
            return null;
        }
    }
}