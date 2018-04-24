using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;

namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///     Interceptor responsible for handling calls to 'user code'. User code
    ///     represents external code (e.g. services and listeners) invoked by
    ///     activity. The following is a list of classes that represent user code:
    ///     <ul>
    ///         <li>
    ///             <seealso cref="JavaDelegate" />
    ///         </li>
    ///         <li>
    ///             <seealso cref="CaseExecutionListener" />
    ///         </li>
    ///         <li>
    ///             <seealso cref="ExecutionListener" />
    ///         </li>
    ///         <li>
    ///             <seealso cref="Expression" />
    ///         </li>
    ///         <li>
    ///             <seealso cref="TaskListener" />
    ///         </li>
    ///         <li>
    ///             <seealso cref="IDmnDecision" />
    ///         </li>
    ///     </ul>
    ///     The interceptor is passed in an instance of <seealso cref="DelegateInvocation" />.
    ///     Implementations are responsible for calling
    ///     <seealso cref="DelegateInvocation#proceed()" />.
    ///     
    /// </summary>
    public interface IDelegateInterceptor
    {
        void HandleInvocation(DelegateInvocation invocation);
    }
}