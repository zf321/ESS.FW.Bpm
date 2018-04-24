using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///     Implemented by classes which provide access to the <seealso cref="BpmnModelInstance" />
    ///     and the currently executed <seealso cref="IFlowElement" />.
    ///     
    ///     
    /// </summary>
    public interface IBpmnModelExecutionContext
    {
        /// <summary>
        ///     Returns the <seealso cref="BpmnModelInstance" /> for the currently executed Bpmn Model
        /// </summary>
        /// <returns> the current <seealso cref="BpmnModelInstance" /> </returns>
        IBpmnModelInstance BpmnModelInstance { get; }

        /// <summary>
        ///     <para>
        ///         Returns the currently executed Element in the BPMN Model. This method returns a <seealso cref="IFlowElement" />
        ///         which may be casted
        ///         to the concrete type of the Bpmn Model Element currently executed.
        ///     </para>
        ///     <para>
        ///         If called from a Service <seealso cref="IExecutionListener" />, the method will return the corresponding
        ///         <seealso cref="IFlowNode" />
        ///         for <seealso cref="IExecutionListener#EVENTNAME_START" /> and
        ///         <seealso cref="IExecutionListener#EVENTNAME_END" />
        ///         and the corresponding
        ///         <seealso cref="SequenceFlow" /> for <seealso cref="IExecutionListener#EVENTNAME_TAKE" />.
        ///     </para>
        /// </summary>
        /// <returns> the <seealso cref="IFlowElement" /> corresponding to the current Bpmn Model Element </returns>
        IFlowElement BpmnModelElementInstance { get; }
    }
}