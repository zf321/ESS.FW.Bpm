using ESS.FW.Bpm.Engine.Application.Impl;

namespace ESS.FW.Bpm.Engine.Application
{
    /// <summary>
    ///     <para>
    ///         A utility to declare the process application in which subsequent engine API calls
    ///         are executed. Process application context is important for the engine
    ///         to access custom classes as well as process-application-level entities like
    ///         script engines or Spin data formats.
    ///     </para>
    ///     <para>
    ///         By default, the process engine only guarantees to switch into the context
    ///         of the process application when it executes custom code (e.g. a JavaDelegate).
    ///         This utility allows to declare a process application into which the process engine
    ///         then switches as soon as it begins executing a command.
    ///         Example using a variable that is serialized with a Camunda Spin data format:
    ///         <pre>
    ///             try {
    ///             ProcessApplicationContext.setCurrentProcessApplication("someProcessApplication");
    ///             runtimeService.setVariable(
    ///             "processInstanceId",
    ///             "variableName",
    ///             Variables.objectValue(anObject).serializationDataFormat(SerializationDataFormats.JSON).create());
    ///             } finally {
    ///             ProcessApplicationContext.clear();
    ///             }
    ///         </pre>
    ///     </para>
    ///     <para>
    ///         Declaring the process application context allows the engine to access the Spin JSON data format
    ///         as configured in that process application to serialize the object value. Without declaring the context,
    ///         the global json data format is used.
    ///     </para>
    ///     <para>
    ///         Declaring the context process application affects only engine API invocations. It DOES NOT affect
    ///         the context class loader for subsequent code.
    ///         
    ///     </para>
    /// </summary>
    public class ProcessApplicationContext
    {
        /// <summary>
        ///     Declares the context process application for all subsequent engine API invocations
        ///     until <seealso cref="#clear()" /> is called. The context is bound to the current thread.
        ///     This method should always be used in a try-finally block to ensure that <seealso cref="#clear()" />
        ///     is called under any circumstances.
        /// </summary>
        /// <param name="processApplicationName"> the name of the process application to switch into </param>
        public static string CurrentProcessApplication
        {
            set { ProcessApplicationContextImpl.Set(new ProcessApplicationIdentifier(value)); }
        }

        /// <summary>
        ///     Declares the context process application for all subsequent engine API invocations
        ///     until <seealso cref="#clear()" /> is called. The context is bound to the current thread.
        ///     This method should always be used in a try-finally block to ensure that <seealso cref="#clear()" />
        ///     is called under any circumstances.
        /// </summary>
        /// <param name="reference"> a reference to the process application to switch into </param>
        /// <summary>
        ///     Declares the context process application for all subsequent engine API invocations
        ///     until <seealso cref="#clear()" /> is called. The context is bound to the current thread.
        ///     This method should always be used in a try-finally block to ensure that <seealso cref="#clear()" />
        ///     is called under any circumstances.
        /// </summary>
        /// <param name="processApplication"> the process application to switch into </param>
        /// <summary>
        ///     Clears the currently declared context process application.
        /// </summary>
        public static void Clear()
        {
            ProcessApplicationContextImpl.Clear();
        }

        /// <summary>

        ///     <para>
        ///         Takes a callable and executes all engine API invocations within that callable in the context
        ///         of the given process application
        ///     </para>
        ///     <para>
        ///         Equivalent to
        ///         <pre>
        ///             try {
        ///             ProcessApplicationContext.setCurrentProcessApplication("someProcessApplication");
        ///             callable.call();
        ///             } finally {
        ///             ProcessApplicationContext.clear();
        ///             }
        ///         </pre>
        ///     </para>
        /// </summary>
        /// <param name="callable"> the callable to execute </param>
        /// <param name="name"> the name of the process application to switch into </param>
        /// <summary>
        ///     <para>
        ///         Takes a callable and executes all engine API invocations within that callable in the context
        ///         of the given process application
        ///     </para>
        ///     <para>
        ///         Equivalent to
        ///         <pre>
        ///             try {
        ///             ProcessApplicationContext.setCurrentProcessApplication("someProcessApplication");
        ///             callable.call();
        ///             } finally {
        ///             ProcessApplicationContext.clear();
        ///             }
        ///         </pre>
        ///     </para>
        /// </summary>
        /// <param name="callable"> the callable to execute </param>
        /// <param name="reference"> a reference of the process application to switch into </param>
//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T> T withProcessApplicationContext(java.Util.concurrent.Callable<T> callable, ProcessApplicationReference reference) throws Exception
        //public static T withProcessApplicationContext<T>(Callable<T> callable, ProcessApplicationReference reference)
        //{
        //    try
        //    {
        //        CurrentProcessApplication = reference;
        //        return callable.call();
        //    }
        //    finally
        //    {
        //        clear();
        //    }
        //}

        /// <summary>
        ///     <para>
        ///         Takes a callable and executes all engine API invocations within that callable in the context
        ///         of the given process application
        ///     </para>
        ///     <para>
        ///         Equivalent to
        ///         <pre>
        ///             try {
        ///             ProcessApplicationContext.setCurrentProcessApplication("someProcessApplication");
        ///             callable.call();
        ///             } finally {
        ///             ProcessApplicationContext.clear();
        ///             }
        ///         </pre>
        ///     </para>
        /// </summary>
        /// <param name="callable"> the callable to execute </param>
        /// <param name="processApplication"> the process application to switch into </param>
//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T> T withProcessApplicationContext(java.Util.concurrent.Callable<T> callable, ProcessApplicationInterface processApplication) throws Exception
        //public static T withProcessApplicationContext<T>(Callable<T> callable,
        //    IProcessApplicationInterface processApplication)
        //{
        //    try
        //    {
        //        CurrentProcessApplication = processApplication;
        //        return callable.call();
        //    }
        //    finally
        //    {
        //        clear();
        //    }
        //}
    }
}