using System;
using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     
    /// </summary>
    public interface IProcessInstantiationBuilder : IActivityInstantiationBuilder<IProcessInstantiationBuilder>,
        IInstantiationBuilder<IProcessInstantiationBuilder>
    {
        /// <summary>
        ///     Specify the id of the tenant the process definition belongs to. Can only be
        ///     used when the definition is referenced by <code>key</code> and not by <code>id</code>.
        /// </summary>
        IProcessInstantiationBuilder SetProcessDefinitionTenantId(string tenantId);

        /// <summary>
        ///     Specify that the process definition belongs to no tenant. Can only be
        ///     used when the definition is referenced by <code>key</code> and not by <code>id</code>.
        /// </summary>
        IProcessInstantiationBuilder ProcessDefinitionWithoutTenantId();

        /// <summary>
        ///     Set the business key for the process instance
        /// </summary>
        IProcessInstantiationBuilder SetBusinessKey(string businessKey);

        /// <summary>
        ///     Associate a case instance with the process instance
        /// </summary>
        IProcessInstantiationBuilder SetCaseInstanceId(string caseInstanceId);

        /// <summary>
        ///     Start the process instance.
        /// </summary>
        /// <returns> the newly created process instance </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE" /> permission on
        ///     <seealso cref="Resources#PROCESS_INSTANCE" /> and no
        ///     <seealso cref="Permissions#CREATE_INSTANCE" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        /// @deprecated use
        /// <seealso cref="#executeWithVariablesInReturn()" />
        /// instead.
        [Obsolete("use <seealso cref=\"#executeWithVariablesInReturn()\"/> instead.")]
        IProcessInstance Execute();

        /// <summary>
        ///     Start the process instance.
        /// </summary>
        /// <param name="skipCustomListeners">
        ///     specifies whether custom listeners (task and execution) should be
        ///     invoked when executing the instructions. Only supported for
        ///     instructions.
        /// </param>
        /// <param name="skipIoMappings">
        ///     specifies whether input/output mappings for tasks should be
        ///     invoked throughout the transaction when executing the
        ///     instructions. Only supported for instructions.
        /// </param>
        /// <returns> the newly created process instance </returns>
        /// @deprecated use
        /// <seealso cref="#executeWithVariablesInReturn(boolean, boolean)" />
        /// instead.
        [Obsolete("use <seealso cref=\"#executeWithVariablesInReturn(boolean, boolean)\"/> instead.")]
        IProcessInstance Execute(bool skipCustomListeners, bool skipIoMappings);

        /// <summary>
        ///     Start the process instance. If no instantiation instructions are set then
        ///     the instance start at the default start activity. Otherwise, all
        ///     instructions are executed in the order they are submitted. Custom execution
        ///     and task listeners, as well as task input output mappings are triggered.
        /// </summary>
        /// <returns>
        ///     the newly created process instance with the latest variables
        /// </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE" /> permission on
        ///     <seealso cref="Resources#PROCESS_INSTANCE" /> and no
        ///     <seealso cref="Permissions#CREATE_INSTANCE" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        IProcessInstanceWithVariables ExecuteWithVariablesInReturn();

        /// <summary>
        ///     Start the process instance. If no instantiation instructions are set then
        ///     the instance start at the default start activity. Otherwise, all
        ///     instructions are executed in the order they are submitted.
        /// </summary>
        /// <param name="skipCustomListeners">
        ///     specifies whether custom listeners (task and execution) should be
        ///     invoked when executing the instructions. Only supported for
        ///     instructions.
        /// </param>
        /// <param name="skipIoMappings">
        ///     specifies whether input/output mappings for tasks should be
        ///     invoked throughout the transaction when executing the
        ///     instructions. Only supported for instructions.
        /// </param>
        /// <returns>
        ///     the newly created process instance with the latest variables
        /// </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE" /> permission on
        ///     <seealso cref="Resources#PROCESS_INSTANCE" /> and no
        ///     <seealso cref="Permissions#CREATE_INSTANCE" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        /// <exception cref="ProcessEngineException">
        ///     if {@code skipCustomListeners} or {@code skipIoMappings} is set
        ///     to true but no instructions are submitted. Both options are not
        ///     supported when the instance starts at the default start activity.
        ///     Use <seealso cref="#execute()" /> instead.
        /// </exception>
        IProcessInstanceWithVariables ExecuteWithVariablesInReturn(bool skipCustomListeners, bool skipIoMappings);
    }
}