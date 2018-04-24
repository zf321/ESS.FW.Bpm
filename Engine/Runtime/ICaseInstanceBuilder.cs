using System.Collections.Generic;
using ESS.FW.Bpm.Engine.exception;

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     <para>A fluent builder to create a new case instance.</para>
    ///     
    /// </summary>
    public interface ICaseInstanceBuilder
    {
        /// <summary>
        ///     <para>
        ///         A business key can be provided to associate the case instance with a
        ///         certain identifier that has a clear business meaning. This business key can
        ///         then be used to easily look up that case instance, see
        ///         <seealso cref="IQueryable<ICaseInstance>#caseInstanceBusinessKey(String)" />. Providing such a
        ///         business key is definitely a best practice.
        ///     </para>
        ///     <para>
        ///         Note that a business key MUST be unique for the given case definition WHEN
        ///         you have added a database constraint for it. In this case, only case instance
        ///         from different case definition are allowed to have the same business key and
        ///         the combination of caseDefinitionKey-businessKey must be unique.
        ///     </para>
        /// </summary>
        /// <param name="businessKey">
        ///     a key that uniquely identifies the case instance in the context
        ///     of the given case definition.
        /// </param>
        /// <returns>
        ///     the builder
        /// </returns>
        ICaseInstanceBuilder BusinessKey(string businessKey);

        /// <summary>
        ///     Specify the id of the tenant the case definition belongs to. Can only be
        ///     used when the definition is referenced by <code>key</code> and not by <code>id</code>.
        /// </summary>
        ICaseInstanceBuilder CaseDefinitionTenantId(string tenantId);

        /// <summary>
        ///     Specify that the case definition belongs to no tenant. Can only be
        ///     used when the definition is referenced by <code>key</code> and not by <code>id</code>.
        /// </summary>
        ICaseInstanceBuilder CaseDefinitionWithoutTenantId();

        /// <summary>
        ///     <para>Pass a variable to the case instance.</para>
        ///     <para>Invoking this method multiple times allows passing multiple variables.</para>
        /// </summary>
        /// <param name="variableName"> the name of the variable to set </param>
        /// <param name="variableValue">
        ///     the value of the variable to set
        /// </param>
        /// <returns>
        ///     the builder
        /// </returns>
        /// <exception cref="NotValidException"> when the given variable name is null </exception>
        ICaseInstanceBuilder SetVariable(string variableName, object variableValue);

        /// <summary>
        ///     <para>Pass a map of variables to the case instance.</para>
        ///     <para>Invoking this method multiple times allows passing multiple variables.</para>
        /// </summary>
        /// <param name="variables"> the map of variables </param>
        /// <returns> the builder </returns>
        ICaseInstanceBuilder SetVariables(IDictionary<string, object> variables);

        /// <summary>
        ///     <para>Creates a new <seealso cref="ICaseInstance" />, which will be in the <code>ACTIVE</code> state.</para>
        /// </summary>
        /// <exception cref="NotValidException"> when the given case definition key or id is null or </exception>
        /// <exception cref="NotFoundException"> when no case definition is deployed with the given key or id. </exception>
        /// <exception cref="ProcessEngineException"> when an internal exception happens during the execution of the command </exception>
        ICaseInstance Create();
    }
}