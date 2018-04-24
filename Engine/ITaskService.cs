using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine
{

    /// <summary>
    ///     Service which provides access to <seealso cref="ITask" /> and form related operations.
    ///      
    ///     
    ///     
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        ///     Creates a new task that is not related to any process instance.
        ///     The returned task is transient and must be saved with <seealso cref=".saveTask(Task)" /> 'manually'.
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions.CREATE" /> permission on <seealso cref="Resources.Task" />.
        /// </exception>
        ITask NewTask();

        /// <summary>
        ///     create a new task with a user defined task id
        /// </summary>
        ITask NewTask(string taskId);

        /// <summary>
        ///     Saves the given task to the persistent data store. If the task is already
        ///     present in the persistent store, it is updated.
        ///     After a new task has been saved, the task instance passed into this method
        ///     is updated with the id of the newly created task.
        /// </summary>
        /// <param name="task">
        ///     the task, cannot be null.
        /// </param>
        /// <exception cref="AuthorizationException">
        ///     If the task is already present and the user has no <seealso cref="Permissions.Update" /> permission
        ///     on <seealso cref="Resources.Task" /> or no <seealso cref="Permissions.Update_TASK" /> permission on
        ///     <seealso cref="Resources.ProcessDefinition" />.
        ///     Or if the task is not present and the user has no <seealso cref="Permissions.CREATE" /> permission
        ///     on <seealso cref="Resources.Task" />.
        /// </exception>
        void SaveTask(ITask task);

        /// <summary>
        ///     Deletes the given task, not deleting historic information that is related to this task.
        /// </summary>
        /// <param name="taskId">
        ///     The id of the task that will be deleted, cannot be null. If no task
        ///     exists with the given taskId, the operation is ignored.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when an error occurs while deleting the task or in case the task is part
        ///     of a running process or case instance.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.DELETE" /> permission on <seealso cref="Resources.Task" />.
        /// </exception>
        void DeleteTask(string taskId);

        /// <summary>
        ///     Deletes all tasks of the given collection, not deleting historic information that is related
        ///     to these tasks.
        /// </summary>
        /// <param name="taskIds">
        ///     The id's of the tasks that will be deleted, cannot be null. All
        ///     id's in the list that don't have an existing task will be ignored.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when an error occurs while deleting the tasks or in case one of the tasks
        ///     is part of a running process or case instance.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.DELETE" /> permission on <seealso cref="Resources.Task" />.
        /// </exception>
        void DeleteTasks(ICollection<string> taskIds);

        /// <summary>
        ///     Deletes the given task.
        /// </summary>
        /// <param name="taskId">
        ///     The id of the task that will be deleted, cannot be null. If no task
        ///     exists with the given taskId, the operation is ignored.
        /// </param>
        /// <param name="cascade">
        ///     If cascade is true, also the historic information related to this task is deleted.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when an error occurs while deleting the task or in case the task is part
        ///     of a running process or case instance.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.DELETE" /> permission on <seealso cref="Resources.Task" />.
        /// </exception>
        void DeleteTask(string taskId, bool cascade);

        /// <summary>
        ///     Deletes all tasks of the given collection.
        /// </summary>
        /// <param name="taskIds">
        ///     The id's of the tasks that will be deleted, cannot be null. All
        ///     id's in the list that don't have an existing task will be ignored.
        /// </param>
        /// <param name="cascade">
        ///     If cascade is true, also the historic information related to this task is deleted.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when an error occurs while deleting the tasks or in case one of the tasks
        ///     is part of a running process or case instance.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.DELETE" /> permission on <seealso cref="Resources.Task" />.
        /// </exception>
        void DeleteTasks(ICollection<string> taskIds, bool cascade);

        /// <summary>
        ///     Deletes the given task, not deleting historic information that is related to this task.
        /// </summary>
        /// <param name="taskId">
        ///     The id of the task that will be deleted, cannot be null. If no task
        ///     exists with the given taskId, the operation is ignored.
        /// </param>
        /// <param name="deleteReason">
        ///     reason the task is deleted. Is recorded in history, if enabled.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when an error occurs while deleting the task or in case the task is part
        ///     of a running process or case instance.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.DELETE" /> permission on <seealso cref="Resources.Task" />.
        /// </exception>
        void DeleteTask(string taskId, string deleteReason);

        /// <summary>
        ///     Deletes all tasks of the given collection, not deleting historic information that is related to these tasks.
        /// </summary>
        /// <param name="taskIds">
        ///     The id's of the tasks that will be deleted, cannot be null. All
        ///     id's in the list that don't have an existing task will be ignored.
        /// </param>
        /// <param name="deleteReason">
        ///     reason the task is deleted. Is recorded in history, if enabled.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when an error occurs while deleting the tasks or in case one of the tasks
        ///     is part of a running process or case instance.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.DELETE" /> permission on <seealso cref="Resources.Task" />.
        /// </exception>
        void DeleteTasks(ICollection<string> taskIds, string deleteReason);

        /// <summary>
        ///     Claim responsibility for a task:
        ///     the given user is made <seealso cref="Task.Assignee assignee" /> for the task.
        ///     The difference with <seealso cref=".setAssignee(String, String)" /> is that here
        ///     a check is done if the task already has a user assigned to it.
        ///     No check is done whether the user is known by the identity component.
        /// </summary>
        /// <param name="taskId"> task to claim, cannot be null. </param>
        /// <param name="userId">
        ///     user that claims the task. When userId is null the task is unclaimed,
        ///     assigned to no one.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist or when the task is already claimed by another user.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void Claim(string taskId, string userId);

        /// <summary>
        ///     Marks a task as done and continues process execution.
        ///     This method is typically called by a task list user interface
        ///     after a task form has been submitted by the
        ///     <seealso cref="Task.Assignee assignee" />.
        /// </summary>
        /// <param name="taskId">
        ///     the id of the task to complete, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when no task exists with the given id or when this task is <seealso cref="DelegationState.PENDING" /> delegation.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void Complete(string taskId);

        /// <summary>
        ///     Delegates the task to another user.
        ///     This means that the <seealso cref="Task.Assignee assignee" /> is set
        ///     and the <seealso cref="Task.DelegationState delegation state" /> is set to
        ///     <seealso cref="DelegationState.Pending" />.
        ///     If no owner is set on the task, the owner is set to the current
        ///     <seealso cref="Task.Assignee " /> of the task.
        ///     The new assignee must use <seealso cref="ResolveTask(String)" />
        ///     to report back to the owner.
        ///     Only the owner can <seealso cref="ITaskService.Complete(String) complete" /> the task.
        /// </summary>
        /// <param name="taskId"> The id of the task that will be delegated. </param>
        /// <param name="userId">
        ///     The id of the user that will be set as assignee.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when no task exists with the given id.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void DelegateTask(string taskId, string userId);

        /// <summary>
        ///     Marks that the <seealso cref="Task.Assignee assignee" /> is done with the task
        ///     <seealso cref="DelegateTask(String, String) delegated" />
        ///     to her and that it can be sent back to the <seealso cref="Task.Owner owner" />.
        ///     Can only be called when this task is <seealso cref="DelegationState.Pending" /> delegation.
        ///     After this method returns, the <seealso cref="DelegationState" />
        ///     is set to <seealso cref="DelegationState.Resolved" /> and the task can be
        ///     <seealso cref="ITaskService.Complete(String) completed" />.
        /// </summary>
        /// <param name="taskId">
        ///     the id of the task to resolve, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when no task exists with the given id.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void ResolveTask(string taskId);

        /// <summary>
        ///     Marks that the <seealso cref="Task.Assignee assignee" /> is done with the task
        ///     <seealso cref="ITaskService.DelegateTask(String, String) delegated" />
        ///     to her and that it can be sent back to the <seealso cref="Task.getOwner() owner" />
        ///     with the provided variables.
        ///     Can only be called when this task is <seealso cref="DelegationState.PENDING" /> delegation.
        ///     After this method returns, the <seealso cref="Task.getDelegationState() delegation state" />
        ///     is set to <seealso cref="DelegationState.RESOLVED" /> and the task can be
        ///     <seealso cref="ITaskService.complete(String) completed" />.
        /// </summary>
        /// <param name="taskId"> </param>
        /// <param name="variables">
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when no task exists with the given id.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void ResolveTask(string taskId, IDictionary<string, object> variables);

        /// <summary>
        ///     Marks a task as done and continues process execution.
        ///     This method is typically called by a task list user interface
        ///     after a task form has been submitted by the
        ///     <seealso cref="Task.Assignee assignee" />
        ///     and the required task parameters have been provided.
        /// </summary>
        /// <param name="taskId"> the id of the task to complete, cannot be null. </param>
        /// <param name="variables">
        ///     task parameters. May be null or empty.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when no task exists with the given id.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.UpdateTask" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void Complete(string taskId, IDictionary<string, object> variables);

        /// <summary>
        ///     Changes the assignee of the given task to the given userId.
        ///     No check is done whether the user is known by the identity component.
        /// </summary>
        /// <param name="taskId"> id of the task, cannot be null. </param>
        /// <param name="userId">
        ///     id of the user to use as assignee.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task or user doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void SetAssignee(string taskId, string userId);

        /// <summary>
        ///     Transfers ownership of this task to another user.
        ///     No check is done whether the user is known by the identity component.
        /// </summary>
        /// <param name="taskId"> id of the task, cannot be null. </param>
        /// <param name="userId">
        ///     of the person that is receiving ownership.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task or user doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void SetOwner(string taskId, string userId);

        /// <summary>
        ///     Retrieves the <seealso cref="IdentityLink" />s associated with the given task.
        ///     Such an <seealso cref="IdentityLink" /> informs how a certain identity (eg. group or user)
        ///     is associated with a certain task (eg. as candidate, assignee, etc.)
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        IList<IIdentityLink> GetIdentityLinksForTask(string taskId);

        /// <summary>
        ///     Convenience shorthand for <seealso cref=".addUserIdentityLink(String, String, String)" />; with type
        ///     <seealso cref="IdentityLinkType.CANDIDATE" />
        /// </summary>
        /// <param name="taskId"> id of the task, cannot be null. </param>
        /// <param name="userId">
        ///     id of the user to use as candidate, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task or user doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void AddCandidateUser(string taskId, string userId);

        /// <summary>
        ///     Convenience shorthand for <seealso cref=".addGroupIdentityLink(String, String, String)" />; with type
        ///     <seealso cref="IdentityLinkType.CANDIDATE" />
        /// </summary>
        /// <param name="taskId"> id of the task, cannot be null. </param>
        /// <param name="groupId">
        ///     id of the group to use as candidate, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task or group doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void AddCandidateGroup(string taskId, string groupId);

        /// <summary>
        ///     Involves a user with a task. The type of identity link is defined by the
        ///     given identityLinkType.
        /// </summary>
        /// <param name="taskId"> id of the task, cannot be null. </param>
        /// <param name="userId"> id of the user involve, cannot be null. </param>
        /// </param>
        /// <param name="identityLinkType">
        ///     type of identityLink, cannot be null (<seealso cref= <seealso cref="IdentityLinkType" />).
        ///     </seealso>
        ///     <exception cref="ProcessEngineException">
        ///         when the task or user doesn't exist.
        ///     </exception>
        ///     <exception cref="AuthorizationException">
        ///         If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///         or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///         (if the task is part of a running process instance).
        ///     </exception>
        void AddUserIdentityLink(string taskId, string userId, string identityLinkType);

        /// <summary>
        ///     Involves a group with a task. The type of identityLink is defined by the
        ///     given identityLink.
        /// </summary>
        /// <param name="taskId"> id of the task, cannot be null. </param>
        /// <param name="groupId"> id of the group to involve, cannot be null. </param>
        /// </param>
        /// <param name="identityLinkType">
        ///     type of identity, cannot be null (<seealso cref= <seealso cref="IdentityLinkType" />).
        ///     </seealso>
        ///     <exception cref="ProcessEngineException">
        ///         when the task or group doesn't exist.
        ///     </exception>
        ///     <exception cref="AuthorizationException">
        ///         If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///         or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///         (if the task is part of a running process instance).
        ///     </exception>
        void AddGroupIdentityLink(string taskId, string groupId, string identityLinkType);

        /// <summary>
        ///     Convenience shorthand for <seealso cref=".deleteUserIdentityLink(String, String, String)" />; with type
        ///     <seealso cref="IdentityLinkType.CANDIDATE" />
        /// </summary>
        /// <param name="taskId"> id of the task, cannot be null. </param>
        /// <param name="userId">
        ///     id of the user to use as candidate, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task or user doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void DeleteCandidateUser(string taskId, string userId);

        /// <summary>
        ///     Convenience shorthand for <seealso cref=".deleteGroupIdentityLink(String, String, String)" />; with type
        ///     <seealso cref="IdentityLinkType.CANDIDATE" />
        /// </summary>
        /// <param name="taskId"> id of the task, cannot be null. </param>
        /// <param name="groupId">
        ///     id of the group to use as candidate, cannot be null.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task or group doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void DeleteCandidateGroup(string taskId, string groupId);

        /// <summary>
        ///     Removes the association between a user and a task for the given identityLinkType.
        /// </summary>
        /// <param name="taskId"> id of the task, cannot be null. </param>
        /// <param name="userId"> id of the user involve, cannot be null. </param>
        /// </param>
        /// <param name="identityLinkType">
        ///     type of identityLink, cannot be null (<seealso cref= <seealso cref="IdentityLinkType" />).
        ///     </seealso>
        ///     <exception cref="ProcessEngineException">
        ///         when the task or user doesn't exist.
        ///     </exception>
        ///     <exception cref="AuthorizationException">
        ///         If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///         or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///         (if the task is part of a running process instance).
        ///     </exception>
        void DeleteUserIdentityLink(string taskId, string userId, string identityLinkType);

        /// <summary>
        ///     Removes the association between a group and a task for the given identityLinkType.
        /// </summary>
        /// <param name="taskId"> id of the task, cannot be null. </param>
        /// <param name="groupId"> id of the group to involve, cannot be null. </param>
        /// </param>
        /// <param name="identityLinkType">
        ///     type of identity, cannot be null (<seealso cref= <seealso cref="IdentityLinkType" />).
        ///     </seealso>
        ///     <exception cref="ProcessEngineException">
        ///         when the task or group doesn't exist.
        ///     </exception>
        ///     <exception cref="AuthorizationException">
        ///         If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///         or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///         (if the task is part of a running process instance).
        ///     </exception>
        void DeleteGroupIdentityLink(string taskId, string groupId, string identityLinkType);

        /// <summary>
        ///     Changes the priority of the task.
        ///     Authorization: actual owner / business admin
        /// </summary>
        /// <param name="taskId"> id of the task, cannot be null. </param>
        /// <param name="priority">
        ///     the new priority for the task.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void SetPriority(string taskId, int priority);

        /// <summary>
        ///     Returns a new <seealso cref="TaskQuery" /> that can be used to dynamically query tasks.
        /// </summary>
        //ITaskQuery CreateTaskQuery();
        IQueryable<ITask> CreateTaskQuery(Expression<Func<TaskEntity, bool>> expression = null);

        ITaskManager GetTaskManager();
        /// <summary>
        ///     Returns a new
        /// </summary>
        //INativeTaskQuery CreateNativeTaskQuery();

        /// <summary>
        ///     Set variable on a task. If the variable is not already existing, it will be created in the
        ///     most outer scope.  This means the process instance in case this task is related to an
        ///     execution.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void SetVariable(string taskId, string variableName, object value);

        /// <summary>
        ///     Set variables on a task. If the variable is not already existing, it will be created in the
        ///     most outer scope.  This means the process instance in case this task is related to an
        ///     execution.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void SetVariables(string taskId, IDictionary<string, object> variables);

        /// <summary>
        ///     Set variable on a task. If the variable is not already existing, it will be created in the
        ///     task.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void SetVariableLocal(string taskId, string variableName, object value);

        /// <summary>
        ///     Set variables on a task. If the variable is not already existing, it will be created in the
        ///     task.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void SetVariablesLocal(string taskId, IDictionary<string, object> variables);

        /// <summary>
        ///     Get a variables and search in the task scope and if available also the execution scopes.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        object GetVariable(string taskId, string variableName);

        /// <summary>
        ///     Get a variables and search in the task scope and if available also the execution scopes.
        /// </summary>
        /// <param name="taskId"> the id of the task </param>
        /// <param name="variableName">
        ///     the name of the variable to fetch
        /// </param>
        /// <returns>
        ///     the TypedValue for the variable or 'null' in case no such variable exists.
        /// </returns>
        /// <exception cref="ClassCastException">
        ///     in case the value is not of the requested type
        /// </exception>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        ///     @since 7.2
        /// </exception>
        T GetVariableTyped<T>(string taskId, string variableName);

        /// <summary>
        ///     Get a variables and search in the task scope and if available also the execution scopes.
        /// </summary>
        /// <param name="taskId"> the id of the task </param>
        /// <param name="variableName"> the name of the variable to fetch </param>
        /// <param name="deserializeValue">
        ///     if false a, <seealso cref="SerializableValue" /> will not be deserialized.
        /// </param>
        /// <returns>
        ///     the TypedValue for the variable or 'null' in case no such variable exists.
        /// </returns>
        /// <exception cref="ClassCastException">
        ///     in case the value is not of the requested type
        /// </exception>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        ///     @since 7.2
        /// </exception>
//JAVA TO C. CONVERTER TODO TASK: The following line could not be converted:
        T GetVariableTyped<T>(string taskId, string variableName, bool deserializeValue);

        /// <summary>
        ///     Get a variables and only search in the task scope.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        object GetVariableLocal(string taskId, string variableName);

        /// <summary>
        ///     Get a variables and only search in the task scope.
        /// </summary>
        /// <param name="taskId"> the id of the task </param>
        /// <param name="variableName">
        ///     the name of the variable to fetch
        /// </param>
        /// <returns>
        ///     the TypedValue for the variable or 'null' in case no such variable exists.
        /// </returns>
        /// <exception cref="ClassCastException">
        ///     in case the value is not of the requested type
        /// </exception>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        ///     @since 7.2
        /// </exception>
//JAVA TO C. CONVERTER TODO TASK: The following line could not be converted:
        T GetVariableLocalTyped<T>(string taskId, string variableName);

        /// <summary>
        ///     Get a variables and only search in the task scope.
        /// </summary>
        /// <param name="taskId"> the id of the task </param>
        /// <param name="variableName"> the name of the variable to fetch </param>
        /// <param name="deserializeValue">
        ///     if false a, <seealso cref="SerializableValue" /> will not be deserialized.
        /// </param>
        /// <returns>
        ///     the TypedValue for the variable or 'null' in case no such variable exists.
        /// </returns>
        /// <exception cref="ClassCastException">
        ///     in case the value is not of the requested type
        /// </exception>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        ///     @since 7.2
        /// </exception>
//JAVA TO C. CONVERTER TODO TASK: The following line could not be converted:
        T GetVariableLocalTyped<T>(string taskId, string variableName, bool deserializeValue);

        /// <summary>
        ///     Get all variables and search in the task scope and if available also the execution scopes.
        ///     If you have many variables and you only need a few, consider using
        ///     <seealso cref=".getVariables(String, Collection)" />
        ///     for better performance.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        IDictionary<string, object> GetVariables(string taskId);

        /// <summary>
        ///     Get all variables and search in the task scope and if available also the execution scopes.
        ///     If you have many variables and you only need a few, consider using
        ///     <seealso cref=".getVariables(String, Collection)" />
        ///     for better performance.
        /// </summary>
        /// <param name="taskId">
        ///     the id of the task
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        ///     @since 7.2
        /// </exception>
        IVariableMap GetVariablesTyped(string taskId);

        /// <summary>
        ///     Get all variables and search in the task scope and if available also the execution scopes.
        ///     If you have many variables and you only need a few, consider using
        ///     <seealso cref=".getVariables(String, Collection)" />
        ///     for better performance.
        /// </summary>
        /// <param name="taskId"> the id of the task </param>
        /// <param name="deserializeValues">
        ///     if false, <seealso cref="SerializableValue SerializableValues" /> will not be deserialized.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        ///     @since 7.2
        /// </exception>
        IVariableMap GetVariablesTyped(string taskId, bool deserializeValues);

        /// <summary>
        ///     Get all variables and search only in the task scope.
        ///     If you have many task local variables and you only need a few, consider using
        ///     <seealso cref=".getVariablesLocal(String, Collection)" />
        ///     for better performance.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        IDictionary<string, object> GetVariablesLocal(string taskId);

        /// <summary>
        ///     Get all variables and search only in the task scope.
        ///     If you have many task local variables and you only need a few, consider using
        ///     <seealso cref=".getVariablesLocal(String, Collection)" />
        ///     for better performance.
        /// </summary>
        /// <param name="taskId"> the id of the task </param>
        /// <param name="deserializeValues">
        ///     if false, <seealso cref="SerializableValue SerializableValues" /> will not be deserialized.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        ///     @since 7.2
        /// </exception>
        IVariableMap GetVariablesLocalTyped(string taskId);

        /// <summary>
        ///     Get all variables and search only in the task scope.
        ///     If you have many task local variables and you only need a few, consider using
        ///     <seealso cref=".getVariablesLocal(String, Collection)" />
        ///     for better performance.
        /// </summary>
        /// <param name="taskId"> the id of the task </param>
        /// <param name="deserializeValues">
        ///     if false, <seealso cref="SerializableValue SerializableValues" /> will not be deserialized.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        ///     @since 7.2
        /// </exception>
        IVariableMap GetVariablesLocalTyped(string taskId, bool deserializeValues);

        /// <summary>
        ///     Get values for all given variableNames
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        IDictionary<string, object> GetVariables(string taskId, ICollection<string> variableNames);

        /// <summary>
        ///     Get values for all given variableName
        /// </summary>
        /// <param name="taskId"> the id of the task </param>
        /// <param name="variableNames"> only fetch variables whose names are in the collection. </param>
        /// <param name="deserializeValues">
        ///     if false, <seealso cref="SerializableValue SerializableValues" /> will not be deserialized.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        ///     @since 7.2
        /// </exception>
        IVariableMap GetVariablesTyped(string taskId, ICollection<string> variableNames, bool deserializeValues);

        /// <summary>
        ///     Get a variable on a task
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        IDictionary<string, object> GetVariablesLocal(string taskId, ICollection<string> variableNames);

        /// <summary>
        ///     Get values for all given variableName. Only search in the local task scope.
        /// </summary>
        /// <param name="taskId"> the id of the task </param>
        /// <param name="variableNames"> only fetch variables whose names are in the collection. </param>
        /// <param name="deserializeValues">
        ///     if false, <seealso cref="SerializableValue SerializableValues" /> will not be deserialized.
        /// </param>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.READ" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.READ_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        ///     @since 7.2
        /// </exception>
        IVariableMap GetVariablesLocalTyped(string taskId, ICollection<string> variableNames, bool deserializeValues);

        /// <summary>
        ///     Removes the variable from the task.
        ///     When the variable does not exist, nothing happens.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void RemoveVariable(string taskId, string variableName);

        /// <summary>
        ///     Removes the variable from the task (not considering parent scopes).
        ///     When the variable does not exist, nothing happens.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void RemoveVariableLocal(string taskId, string variableName);

        /// <summary>
        ///     Removes all variables in the given collection from the task.
        ///     Non existing variable names are simply ignored.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void RemoveVariables(string taskId, ICollection<string> variableNames);

        /// <summary>
        ///     Removes all variables in the given collection from the task (not considering parent scopes).
        ///     Non existing variable names are simply ignored.
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     when the task doesn't exist.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     If the user has no <seealso cref="Permissions.Update" /> permission on <seealso cref="Resources.Task" />
        ///     or no <seealso cref="Permissions.Update_TASK" /> permission on <seealso cref="Resources.ProcessDefinition" />
        ///     (if the task is part of a running process instance).
        /// </exception>
        void RemoveVariablesLocal(string taskId, ICollection<string> variableNames);

        /// <summary>
        ///     Add a comment to a task and/or process instance.
        /// </summary>
        /// @deprecated Use
        /// <seealso cref=".createComment(String, String, String)" />
        /// instead
        [Obsolete("Use <seealso cref=\".createComment(String, String, String)\"/> instead")]
        void AddComment(string taskId, string processInstanceId, string message);

        /// <summary>
        ///     Creates a comment to a task and/or process instance and returns the comment.
        /// </summary>
        IComment CreateComment(string taskId, string processInstanceId, string message);

        /// <summary>
        ///     The comments related to the given task.
        /// </summary>
        IList<IComment> GetTaskComments(string taskId);

        /// <summary>
        ///     Retrieve a particular task comment
        /// </summary>
        IComment GetTaskComment(string taskId, string commentId);

        /// <summary>
        ///     <para>The all events related to the given task.</para>
        ///     <para>
        ///         As of Camunda BPM 7.4 task events are only written in context of a logged in
        ///         user. This behavior can be toggled in the process engine configuration using the
        ///         property <code>legacyUserOperationLog</code> (default false). To restore the engine's
        ///         previous behavior, set the flag to <code>true</code>.
        ///     </para>
        /// </summary>
        /// @deprecated This method has been deprecated as of camunda BPM 7.1. It has been replaced with
        /// the operation log. See
        /// <seealso cref="UserOperationLogEntry" />
        /// and
        /// <seealso cref="UserOperationLogQuery" />
        /// .
        /// <seealso cref= HistoryService. createUserOperationLogQuery
        /// (
        /// )
        /// </seealso>
        [Obsolete("This method has been deprecated as of camunda BPM 7.1. It has been replaced with")]
        IList<IEvent> GetTaskEvents(string taskId);

        /// <summary>
        ///     The comments related to the given process instance.
        /// </summary>
        IList<IComment> GetProcessInstanceComments(string processInstanceId);

        /// <summary>
        ///     Add a new IAttachment to a task and/or a process instance and use an input stream to provide the content
        ///     please use method in runtime service to operate on process instance.
        ///     Either taskId or processInstanceId has to be provided
        /// </summary>
        /// <param name="taskId"> - task that should have an IAttachment </param>
        /// <param name="processInstanceId"> - id of a process to use if task id is null </param>
        /// <param name="attachmentType"> - name of the IAttachment, can be null </param>
        /// <param name="attachmentName"> - name of the IAttachment, can be null </param>
        /// <param name="attachmentDescription">  - full text description, can be null </param>
        /// <param name="content">
        ///     - byte array with content of IAttachment
        /// </param>
        IAttachment CreateAttachment(string attachmentType, string taskId, string processInstanceId,
            string attachmentName, string attachmentDescription, Stream content);

        /// <summary>
        ///     Add a new IAttachment to a task and/or a process instance and use an url as the content
        ///     please use method in runtime service to operate on process instance
        ///     Either taskId or processInstanceId has to be provided
        /// </summary>
        /// <param name="taskId"> - task that should have an IAttachment </param>
        /// <param name="processInstanceId"> - id of a process to use if task id is null </param>
        /// <param name="attachmentType"> - name of the IAttachment, can be null </param>
        /// <param name="attachmentName"> - name of the IAttachment, can be null </param>
        /// <param name="attachmentDescription">  - full text description, can be null </param>
        /// <param name="url">
        ///     - url of the IAttachment, can be null
        /// </param>
        IAttachment CreateAttachment(string attachmentType, string taskId, string processInstanceId,
            string attachmentName, string attachmentDescription, string url);

        /// <summary>
        ///     Update the name and decription of an IAttachment
        /// </summary>
        void SaveAttachment(IAttachment IAttachment);

        /// <summary>
        ///     Retrieve a particular IAttachment
        /// </summary>
        IAttachment GetAttachment(string attachmentId);

        /// <summary>
        ///     Retrieve a particular IAttachment to the given task id and IAttachment id
        /// </summary>
        IAttachment GetTaskAttachment(string taskId, string attachmentId);

        /// <summary>
        ///     Retrieve stream content of a particular IAttachment
        /// </summary>
        Stream GetAttachmentContent(string attachmentId);

        /// <summary>
        ///     Retrieve stream content of a particular IAttachment to the given task id and IAttachment id
        /// </summary>
        Stream GetTaskAttachmentContent(string taskId, string attachmentId);

        /// <summary>
        ///     The list of attachments associated to a task
        /// </summary>
        IList<IAttachment> GetTaskAttachments(string taskId);

        /// <summary>
        ///     The list of attachments associated to a process instance
        /// </summary>
        IList<IAttachment> GetProcessInstanceAttachments(string processInstanceId);

        /// <summary>
        ///     Delete an IAttachment
        /// </summary>
        void DeleteAttachment(string attachmentId);

        /// <summary>
        ///     Delete an IAttachment to the given task id and IAttachment id
        /// </summary>
        void DeleteTaskAttachment(string taskId, string attachmentId);

        /// <summary>
        ///     The list of subtasks for this parent task
        /// </summary>
        IList<ITask> GetSubTasks(string parentTaskId);

        /// <summary>
        ///     Instantiate a task report
        /// </summary>
        ITaskReport CreateTaskReport();
        IQueryable<TaskEntity> CreateTaskEntityQuery(Expression<Func<TaskEntity, bool>> expression =null);
    }
}