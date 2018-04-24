using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Application
{
    /// <summary>
    ///     <para>SPI interface that allows providing a custom ElResolver implementation.</para>
    ///     <para>
    ///         Implementations of this interface are looked up through the Java SE <seealso cref="ServiceLoader" />
    ///         facilities.
    ///         If you want to provide a custom implementation in your application, place a file named
    ///         <code>META-INF/org.camunda.bpm.application.ProcessApplicationElResolver</code> inside your application
    ///         which contains the fully qualified classname of your implementation.
    ///     </para>
    ///     
    /// </summary>
    public interface IProcessApplicationElResolver
    {
        // precedences for known providers

        /// <summary>
        ///     Allows to set a precedence to the ElResolver. Resolver with a lower precedence will be invoked first.
        /// </summary>
        int? Precedence { get; }

        /// <summary>
        ///     return the Resolver. May be null.
        /// </summary>
        ELResolver GetElResolver(AbstractProcessApplication processApplication);

        /// Comparator used for sorting providers

        /// <summary>
        /// </summary>
        /// <seealso cref= ProcessApplicationElResolver#getPrecedence() </seealso>
    }

    public static class ProcessApplicationElResolverFields
    {
        public const int SpringResolver = 100;
        public const int CdiResolver = 200;
    }

    public class ProcessApplicationElResolverProcessApplicationElResolverSorter :
        IComparer<IProcessApplicationElResolver>
    {
        public virtual int Compare(IProcessApplicationElResolver o1, IProcessApplicationElResolver o2)
        {
            return -1*o1.Precedence.Value.CompareTo(o2.Precedence);
        }
    }
}