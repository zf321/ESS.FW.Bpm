using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Delegate
{
    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    public interface IDelegateTask : IVariableScope, IBpmnModelExecutionContext, IProcessEngineServicesAware
    {
        /// <summary>
        ///     DB id of the ITask.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Name or title of the ITask.
        /// </summary>
        string Name { get; set; }


        /// <summary>
        ///     Free text description of the ITask.
        /// </summary>
        string Description { get; set; }


        /// <summary>
        ///     indication of how important/urgent this ITask is with a number between
        ///     0 and 100 where higher values mean a higher priority and lower values mean
        ///     lower priority: [0..19] lowest, [20..39] low, [40..59] normal, [60..79] high
        ///     [80..100] highest
        /// </summary>
        int Priority { get; set; }


        /// <summary>
        ///     Reference to the process instance or null if it is not related to a process instance.
        /// </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        ///     Reference to the path of execution or null if it is not related to a process instance.
        /// </summary>
        string ExecutionId { get; }

        /// <summary>
        ///     Reference to the process definition or null if it is not related to a process.
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        ///     Reference to the case instance or null if it is not related to a case instance.
        /// </summary>
        string CaseInstanceId { get; }

        /// <summary>
        ///     Reference to the case execution or null if it is not related to a case instance.
        /// </summary>
        string CaseExecutionId { get; }

        /// <summary>
        ///     Reference to the case definition or null if it is not related to a case.
        /// </summary>
        string CaseDefinitionId { get; }

        /// <summary>
        ///     The date/time when this ITask was created
        /// </summary>
        DateTime CreateTime { get; }

        /// <summary>
        ///     The id of the activity in the process defining this ITask or null if this is not related to a process
        /// </summary>
        string TaskDefinitionKey { get; }

        /// <summary>
        ///     Returns the execution currently at the ITask.
        /// </summary>
        IDelegateExecution Execution { get; }

        /// <summary>
        ///     Returns the case execution currently at the ITask.
        /// </summary>
        IDelegateCaseExecution CaseExecution { get; }

        /// <summary>
        ///     Returns the event name which triggered the ITask listener to fire for this ITask.
        /// </summary>
        string EventName { get; }

        /// <summary>
        ///     The <seealso cref="User.getId() userId" /> of the person responsible for this ITask.
        /// </summary>
        string Owner { get; set; }


        /// <summary>
        ///     The <seealso cref="User.getId() userId" /> of the person to which this ITask is delegated.
        /// </summary>
        string Assignee { get; set; }


        /// <summary>
        ///     Due date of the ITask.
        /// </summary>
        DateTime? DueDate { get; set; }


        /// <summary>
        ///     Get delete reason of the ITask.
        /// </summary>
        string DeleteReason { get; }

        /// <summary>
        ///     Retrieves the candidate users and groups associated with the ITask.
        /// </summary>
        /// <returns> set of <seealso cref="IdentityLink" />s of type <seealso cref="IdentityLinkType#CANDIDATE" />. </returns>
        ISet<IIdentityLink> Candidates { get; }

        /// <summary>
        ///     Provides access to the current <seealso cref="UserTask" /> Element from the Bpmn Model.
        /// </summary>
        /// <returns> the current <seealso cref="UserTask" /> Element from the Bpmn Model. </returns>
        //IUserTask BpmnModelElementInstance { get; }

        /// <summary>
        ///     Return the id of the tenant this ITask belongs to. Can be <code>null</code>
        ///     if the ITask belongs to no single tenant.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        ///     Adds the given user as a candidate user to this ITask.
        /// </summary>
        void AddCandidateUser(string userId);

        /// <summary>
        ///     Adds multiple users as candidate user to this ITask.
        /// </summary>
        void AddCandidateUsers(ICollection<string> candidateUsers);

        /// <summary>
        ///     Adds the given group as candidate group to this ITask
        /// </summary>
        void AddCandidateGroup(string groupId);

        /// <summary>
        ///     Adds multiple groups as candidate group to this ITask.
        /// </summary>
        void AddCandidateGroups(ICollection<string> candidateGroups);

        /// <summary>
        ///     Involves a user with a ITask. The type of identity link is defined by the given identityLinkType.
        /// </summary>
        /// <param name="userId"> id of the user involve, cannot be null. </param>
        /// </param>
        /// <param name="identityLinkType">
        ///     type of identityLink, cannot be null (<seealso cref= <seealso cref="IdentityLinkType" />). </seealso>
        ///     <exception cref="ProcessEngineException"> when the ITask or user doesn't exist. </exception>
        void AddUserIdentityLink(string userId, string identityLinkType);

        /// <summary>
        ///     Involves a group with group ITask. The type of identityLink is defined by the given identityLink.
        /// </summary>
        /// <param name="groupId"> id of the group to involve, cannot be null. </param>
        /// </param>
        /// <param name="identityLinkType">
        ///     type of identity, cannot be null (<seealso cref= <seealso cref="IdentityLinkType" />). </seealso>
        ///     <exception cref="ProcessEngineException"> when the ITask or group doesn't exist. </exception>
        void AddGroupIdentityLink(string groupId, string identityLinkType);

        /// <summary>
        ///     Convenience shorthand for <seealso cref="#deleteUserIdentityLink(String, String)" />; with type
        ///     <seealso cref="IdentityLinkType#CANDIDATE" />
        /// </summary>
        /// <param name="userId"> id of the user to use as candidate, cannot be null. </param>
        /// <exception cref="ProcessEngineException"> when the ITask or user doesn't exist. </exception>
        void DeleteCandidateUser(string userId);

        /// <summary>
        ///     Convenience shorthand for <seealso cref="#deleteGroupIdentityLink(String, String, String)" />; with type
        ///     <seealso cref="IdentityLinkType#CANDIDATE" />
        /// </summary>
        /// <param name="groupId"> id of the group to use as candidate, cannot be null. </param>
        /// <exception cref="ProcessEngineException"> when the ITask or group doesn't exist. </exception>
        void DeleteCandidateGroup(string groupId);

        /// <summary>
        ///     Removes the association between a user and a ITask for the given identityLinkType.
        /// </summary>
        /// <param name="userId"> id of the user involve, cannot be null. </param>
        /// </param>
        /// <param name="identityLinkType">
        ///     type of identityLink, cannot be null (<seealso cref= <seealso cref="IdentityLinkType" />). </seealso>
        ///     <exception cref="ProcessEngineException"> when the ITask or user doesn't exist. </exception>
        void DeleteUserIdentityLink(string userId, string identityLinkType);

        /// <summary>
        ///     Removes the association between a group and a ITask for the given identityLinkType.
        /// </summary>
        /// <param name="groupId"> id of the group to involve, cannot be null. </param>
        /// </param>
        /// <param name="identityLinkType">
        ///     type of identity, cannot be null (<seealso cref= <seealso cref="IdentityLinkType" />). </seealso>
        ///     <exception cref="ProcessEngineException"> when the ITask or group doesn't exist. </exception>
        void DeleteGroupIdentityLink(string groupId, string identityLinkType);

        /// <summary>
        ///     set status to complete.
        /// </summary>
        /// <exception cref="IllegalStateException"> if performed on completion or deletion </exception>
        void Complete();
    }
}