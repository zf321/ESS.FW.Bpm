using System;
using System.Threading;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.context.Impl
{
    /// <summary>
    ///     Wrapps the user-provided <seealso cref="Callable" /> and records the
    ///     Thread Context Classloader after the context switch has been performed.
    ///     This allows detecting if the Thread Context has been manipulated by the container after that
    ///     (Usually due to cross application EJB invocations).
    ///     
    /// </summary>
    public class ProcessApplicationClassloaderInterceptor<T> /*: ICallable<T>*/
    {
        //private static readonly ThreadLocal<ClassLoader> PA_CLASSLOADER = new ThreadLocal<ClassLoader>();

        protected internal Func<T> @delegate;

        public ProcessApplicationClassloaderInterceptor(Func<T> @delegate)
        {
            this.@delegate = @delegate;
        }

        //public static ClassLoader ProcessApplicationClassLoader
        //{
        //    get { return PA_CLASSLOADER.get(); }
        //}
        
        public virtual T Call()
        {
            try
            {
                // record thread context right after context switch
                //PA_CLASSLOADER.set(ClassLoaderUtil.ContextClassloader);

                // proceed with delegate callable invocation
                return @delegate();
            }
            finally
            {
                //PA_CLASSLOADER.remove();
            }
        }
    }
}