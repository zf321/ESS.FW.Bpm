using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Application.Impl;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     <para>
    ///         This is an <seealso cref="ELResolver" /> implementation that delegates to a ProcessApplication-provided
    ///         <seealso cref="ELResolver" />. The idea is that in a multi-application setup, a shared process engine may
    ///         orchestrate
    ///         multiple process applications. In this setting we want to delegate to the current process application
    ///         for performing expression resolving. This also allows individual process applications to integrate with
    ///         different kinds of Di Containers or other expression-context providing frameworks. For instance, a first
    ///         process application may use the spring application context for resolving Java Delegate implementations
    ///         while a second application may use CDI or even an Apache Camel Context.
    ///     </para>
    ///     <para>
    ///         The behavior of this implementation is as follows: if we are not currently running in the context of
    ///         a process application, we are skipped. If we are, this implementation delegates to the underlying
    ///         application-provided <seealso cref="ELResolver" /> which may itself be a <seealso cref="CompositeELResolver" />
    ///         .
    ///     </para>
    ///     
    /// </summary>
    public class ProcessApplicationElResolverDelegate : AbstractElResolverDelegate
    {
        protected internal override ELResolver ElResolverDelegate
        {
            get
            {
                return DefaultElResolverLookup.LookupResolver();
               
            }
        }
    }
}