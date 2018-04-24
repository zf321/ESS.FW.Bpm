
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     A <seealso cref="IVariableInstance" /> represents a variable in the execution of
    ///     a process instance.
    ///     
    /// </summary>
    public interface IVariableInstance
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
        ///     Returns the TypedValue of this variable instance.
        /// </summary>
        ITypedValue TypedValue { get; }

        /// <summary>
        ///     Returns the corresponding process instance id.
        /// </summary>
        string ProcessInstanceId { get;  }

        /// <summary>
        ///     Returns the corresponding execution id.
        /// </summary>
        string ExecutionId { get; }

        /// <summary>
        ///     Returns the corresponding case instance id.
        /// </summary>
        string CaseInstanceId { get; }

        /// <summary>
        ///     Returns the corresponding case execution id.
        /// </summary>
        string CaseExecutionId { get; }

        /// <summary>
        ///     Returns the corresponding task id.
        /// </summary>
        string TaskId { get; }

        /// <summary>
        ///     Returns the corresponding activity instance id.
        /// </summary>
        string ActivityInstanceId { get; }

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
    }
}