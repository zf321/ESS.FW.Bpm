using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     A single process variable containing the last value when its process instance has finished.
    ///     It is only available when HISTORY_LEVEL is set >= AUDIT
    ///     
    ///     
    /// </summary>
    public interface IHistoricVariableInstance
    {
        /// <returns> the Id of this variable instance </returns>
        string Id { get; }

        /// <summary>
        ///     Returns the name of this variable instance.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Returns the name of the type of this variable instance
        /// </summary>
        /// <returns> the type name of the variable </returns>
        string TypeName { get; }

        /// <summary>
        ///     Returns the value of this variable instance.
        /// </summary>
        object Value { get; }

        /// <summary>
        ///     Returns the <seealso cref="TypedValue" /> of this variable instance.
        /// </summary>
        ITypedValue TypedValue { get; }

        /// <summary>
        ///     Returns the name of this variable instance.
        ///     <para>Deprecated since 7.2: use <seealso cref="#getName()" /> instead.</para>
        /// </summary>
        [Obsolete]
        string VariableName { get; }

        /// <summary>
        ///     <para>Returns the name of the type of this variable instance</para>
        ///     <para>Deprecated since 7.2: use <seealso cref="#getTypeName()" /> instead.</para>
        /// </summary>
        [Obsolete]
        string VariableTypeName { get; }

        /// <summary>
        ///     The process definition key reference.
        /// </summary>
        string ProcessDefinitionKey { get; }

        /// <summary>
        ///     The process definition reference.
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        ///     The process instance reference.
        /// </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        ///     Return the corresponding execution id.
        /// </summary>
        string ExecutionId { get; }

        /// <summary>
        ///     Returns the corresponding activity instance id.
        /// </summary>
        [Obsolete]
        string ActivtyInstanceId { get; }

        /// <summary>
        ///     Returns the corresponding activity instance id.
        /// </summary>
        string ActivityInstanceId { get; }

        /// <summary>
        ///     The case definition key reference.
        /// </summary>
        string CaseDefinitionKey { get; }

        /// <summary>
        ///     The case definition reference.
        /// </summary>
        string CaseDefinitionId { get; }

        /// <summary>
        ///     The case instance reference.
        /// </summary>
        string CaseInstanceId { get; }

        /// <summary>
        ///     Return the corresponding case execution id.
        /// </summary>
        string CaseExecutionId { get; }

        /// <summary>
        ///     Return the corresponding ITask id.
        /// </summary>
        string TaskId { get; }

        /// <summary>
        ///     If the variable value could not be loaded, this returns the error message.
        /// </summary>
        /// <returns> an error message indicating why the variable value could not be loaded. </returns>
        string ErrorMessage { get; }

        /// <summary>
        ///     The id of the tenant this variable belongs to. Can be <code>null</code>
        ///     if the variable belongs to no single tenant.
        /// </summary>
        string TenantId { get; }
        /// <summary>
        /// The time the process was started. </summary>
        DateTime? StartTime { get; }

        /// <summary>
        /// The time the process was ended. </summary>
        DateTime? EndTime { get; }
        string SerializerName { get; set; }
    }
}