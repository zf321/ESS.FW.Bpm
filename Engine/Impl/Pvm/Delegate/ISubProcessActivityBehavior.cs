using System;
using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Delegate
{
    /// <summary>
    ///     behavior for activities that delegate to a complete separate execution of
    ///     a process definition.  In BPMN terminology this can be used to implement a reusable subprocess.
    ///      
    /// </summary>
    public interface ISubProcessActivityBehavior : IActivityBehavior
    {
        /// <summary>
        ///     Pass the output variables from the process instance of the subprocess to the given execution.
        ///     This should be called before the process instance is destroyed.
        /// </summary>
        /// <param name="targetExecution"> execution of the calling process instance to pass the variables to </param>
        /// <param name="calledElementInstance"> instance of the called element that serves as the variable source </param>
        void PassOutputVariables(IActivityExecution targetExecution, IVariableScope calledElementInstance);

        /// <summary>
        ///     Called after the process instance is destroyed for
        ///     this activity to perform its outgoing control flow logic.
        /// </summary>
        /// <param name="execution"> </param>
        /// <exception cref="Exception"> </exception>
        void Completed(IActivityExecution execution);
    }
}