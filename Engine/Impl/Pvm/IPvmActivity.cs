using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Pvm
{
    /// <summary>
    ///     Defines an activity insisde a process. Note that the term "activity" is meant to be
    ///     understood in a broader sense than in BPMN: everything inside a process which can have incoming
    ///     or outgoing sequence flows (transitions) are activities. Examples: events, tasks, gateways,
    ///     subprocesses ...
    ///      
    ///     
    /// </summary>
    public interface IPvmActivity : IPvmScope
    {
        /// <summary>
        ///     The inner behavior of an activity. The inner behavior is the logic which is executed after
        ///     the <seealso cref="IExecutionListener#EVENTNAME_START start" /> listeners have been executed.
        ///     In case the activity <seealso cref="#isScope() is scope" />, a new execution will be created
        /// </summary>
        /// <returns> the inner behavior of the activity </returns>
        IActivityBehavior ActivityBehavior { get;  }

        /// <summary>
        ///     The start behavior of an activity. The start behavior is executed before the
        ///     <seealso cref="IExecutionListener#EVENTNAME_START start" /> listeners of the activity are executed.
        /// </summary>
        /// <returns> the start behavior of an activity. </returns>
        ActivityStartBehavior ActivityStartBehavior { get; }

        /// <returns> the list of outgoing sequence flows (transitions) </returns>
        IList<IPvmTransition> OutgoingTransitions { get; }

        /// <returns> the list of incoming sequence flows (transitions) </returns>
        IList<IPvmTransition> IncomingTransitions { get; }

        /// <summary>
        ///     Indicates whether the activity is executed asynchronously.
        ///     This can be done <em>after</em> the <seealso cref="#getActivityStartBehavior() activity start behavior" /> and
        ///     <em>before</em> the <seealso cref="IExecutionListener#EVENTNAME_START start" /> listeners are invoked.
        /// </summary>
        /// <returns> true if the activity is executed asynchronously. </returns>
        bool AsyncBefore { get; }

        /// <summary>
        ///     Indicates whether execution after this execution should continue asynchronously.
        ///     This can be done <em>after</em> the <seealso cref="IExecutionListener#EVENTNAME_END end" /> listeners are invoked.
        /// </summary>
        /// <returns> true if execution after this activity continues asynchronously. </returns>
        bool AsyncAfter { get; }

        string Name { get; set; }
        bool IsScope { get; set; }
        IoMapping IoMapping { get; set; }

        /// <summary>
        ///     Finds and returns an outgoing sequence flow (transition) by it's id.
        /// </summary>
        /// <param name="transitionId"> the id of the transition to find </param>
        /// <returns> the transition or null in case it cannot be found </returns>
        IPvmTransition FindOutgoingTransition(string transitionId);
        bool CompensationHandler { get; }
    }
}