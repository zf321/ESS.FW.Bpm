using System.Threading.Tasks;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Delegate
{
    //using Stage = org.camunda.bpm.model.cmmn.instance.Stage;
    //using ITask = org.camunda.bpm.model.cmmn.instance.ITask;

    /// <summary>
    ///     Listener interface implemented by user code which wants to be notified
    ///     when a state transition happens on a <seealso cref="ICaseExecution" />.
    ///     <para>
    ///         The following state transition are supported on a <seealso cref="ICaseInstance" />:
    ///         <ul>
    ///             <li>
    ///                 <seealso cref="#CREATE" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#COMPLETE" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#TERMINATE" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#SUSPEND" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#RE_ACTIVATE" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#CLOSE" />
    ///             </li>
    ///         </ul>
    ///     </para>
    ///     <para>
    ///         And on a <seealso cref="ICaseExecution" /> which is not a <seealso cref="ICaseInstance" /> and which
    ///         is associated with a <seealso cref="ITask" /> or a <seealso cref="Stage" /> the following state transition
    ///         are supported:
    ///         <ul>
    ///             <li>
    ///                 <seealso cref="#CREATE" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#ENABLE" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#DISABLE" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#RE_ENABLE" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#START" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#MANUAL_START" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#COMPLETE" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#TERMINATE" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#EXIT" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#SUSPEND" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#RESUME" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#PARENT_SUSPEND" />
    ///             </li>
    ///             <li>
    ///                 <seealso cref="#PARENT_RESUME" />
    ///             </li>
    ///         </ul>
    ///     </para>
    ///     
    /// </summary>
    public interface ICaseExecutionListener : IDelegateListener<IDelegateCaseExecution>
    {
    }

    public static class CaseExecutionListenerFields
    {
        public const string Create = "create";
        public const string Enable = "enable";
        public const string Disable = "disable";
        public const string ReEnable = "reenable";
        public const string Start = "start";
        public const string ManualStart = "manualStart";
        public const string Complete = "complete";
        public const string ReActivate = "reactivate";
        public const string Terminate = "terminate";
        public const string Exit = "exit";
        public const string ParentTerminate = "parentTerminate";
        public const string Suspend = "suspend";
        public const string Resume = "resume";
        public const string ParentSuspend = "parentSuspend";
        public const string ParentResume = "parentResume";
        public const string Close = "close";
        public const string Occur = "occur";
    }
}