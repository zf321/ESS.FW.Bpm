using System;

namespace ESS.FW.Bpm.Engine.Authorization
{
    /// <summary>
    ///     <para>
    ///         Resources are entities for which a user or a group is authorized. Examples of
    ///         resources are applications, process-definitions, process-instances, tasks ...
    ///     </para>
    ///     <para>
    ///         A resource has a type and an id. The type (<seealso cref="#setResource(String)" />)
    ///         allows to group all resources of the same kind. A resource id is the identifier of
    ///         an individual resource instance (<seealso cref="#setResourceId(String)" />). For example:
    ///         the resource type could be "processDefinition" and the resource-id could be the
    ///         id of an individual process definition.
    ///     </para>
    ///     <para>See <seealso cref="Resources" /> for a set of built-in resource constants.</para>
    ///     
    /// </summary>
    [Flags]
    public enum Resources
    {
        Application=0, //(org.camunda.bpm.engine.EntityTypes.APPLICATION, 0),
        User, //(org.camunda.bpm.engine.EntityTypes.USER, 1),
        Group, //(org.camunda.bpm.engine.EntityTypes.GROUP, 2),
        GroupMembership, //(org.camunda.bpm.engine.EntityTypes.GROUP_MEMBERSHIP, 3),
        Authorization, //(org.camunda.bpm.engine.EntityTypes.AUTHORIZATION, 4),
        Filter, //(org.camunda.bpm.engine.EntityTypes.FILTER, 5),
        ProcessDefinition, //(org.camunda.bpm.engine.EntityTypes.PROCESS_DEFINITION, 6),
        Task, //(org.camunda.bpm.engine.EntityTypes.ITask, 7),
        ProcessInstance, //(org.camunda.bpm.engine.EntityTypes.PROCESS_INSTANCE, 8),
        Deployment, //(org.camunda.bpm.engine.EntityTypes.DEPLOYMENT, 9),
        DecisionDefinition, //(org.camunda.bpm.engine.EntityTypes.DECISION_DEFINITION, 10),
        Tenant, //(org.camunda.bpm.engine.EntityTypes.TENANT, 11),
        TenantMembership, //(org.camunda.bpm.engine.EntityTypes.TENANT_MEMBERSHIP, 12),
        Batch, //(org.camunda.bpm.engine.EntityTypes.BATCH, 13),
        DecisionRequirementsDefinition //(org.camunda.bpm.engine.EntityTypes.DECISION_REQUIREMENTS_DEFINITION, 14);
    }
}