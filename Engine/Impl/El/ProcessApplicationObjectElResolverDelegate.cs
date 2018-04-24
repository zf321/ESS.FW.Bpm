using System;
using System.Collections.Generic;
using System.Reflection;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     <para>
    ///         Resolves a <seealso cref="ObjectELResolver" /> from the current process application.
    ///         This allows to cache resolvers on the process application level. Such a resolver
    ///         cannot be cached globally as <seealso cref="ObjectELResolver" /> keeps a cache of classes
    ///         involved in expressions.
    ///     </para>
    ///     <para>
    ///         If resolution is attempted outside the context of a process application,
    ///         then always a new resolver instance is returned (i.e. no caching in these cases).
    ///     </para>
    ///     
    /// </summary>
    public class ProcessApplicationObjectElResolverDelegate : AbstractElResolverDelegate
    {
        protected internal override ELResolver ElResolverDelegate
        {
            get
            {
                var processApplicationReference = Context.CurrentProcessApplication;
                if (processApplicationReference != null)
                {
                    try
                    {
                        var processApplication = processApplicationReference.ProcessApplication;
                        return processApplication.ObjectELResolver;
                    }
                    catch (ProcessApplicationUnavailableException e)
                    {
                        throw new ProcessEngineException(
                            "Cannot access process application '" + processApplicationReference.Name + "'", e);
}
                }
                return new ObjectELResolver();
            }
        }
        
    }
}