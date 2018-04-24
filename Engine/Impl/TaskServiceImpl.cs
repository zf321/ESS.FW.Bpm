using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Impl
{

    /// <summary>
    ///      
    ///     
    /// </summary>
    public class TaskServiceImpl : ServiceImpl, ITaskService
    {
        public virtual ITask NewTask()
        {
            return NewTask(null);
        }

        public virtual ITask NewTask(string taskId)
        {
            return CommandExecutor.Execute(new CreateTaskCmd(taskId));
        }

        public virtual void SaveTask(ITask task)
        {
            CommandExecutor.Execute(new SaveTaskCmd(task));
        }

        public virtual void DeleteTask(string taskId)
        {
            CommandExecutor.Execute(new DeleteTaskCmd(taskId, null, false));
        }

        public virtual void DeleteTasks(ICollection<string> taskIds)
        {
            CommandExecutor.Execute(new DeleteTaskCmd(taskIds, null, false));
        }

        public virtual void DeleteTask(string taskId, bool cascade)
        {
            CommandExecutor.Execute(new DeleteTaskCmd(taskId, null, cascade));
        }

        public virtual void DeleteTasks(ICollection<string> taskIds, bool cascade)
        {
            CommandExecutor.Execute(new DeleteTaskCmd(taskIds, null, cascade));
        }

        public virtual void DeleteTask(string taskId, string deleteReason)
        {
            CommandExecutor.Execute(new DeleteTaskCmd(taskId, deleteReason, false));
        }

        public virtual void DeleteTasks(ICollection<string> taskIds, string deleteReason)
        {
            CommandExecutor.Execute(new DeleteTaskCmd(taskIds, deleteReason, false));
        }

        public virtual void SetAssignee(string taskId, string userId)
        {
            CommandExecutor.Execute(new AssignTaskCmd(taskId, userId));
        }

        public virtual void SetOwner(string taskId, string userId)
        {
            CommandExecutor.Execute(new SetTaskOwnerCmd(taskId, userId));
        }

        public virtual void AddCandidateUser(string taskId, string userId)
        {
            CommandExecutor.Execute(new AddUserIdentityLinkCmd(taskId, userId, IdentityLinkType.Candidate));
        }

        public virtual void AddCandidateGroup(string taskId, string groupId)
        {
            CommandExecutor.Execute(new AddGroupIdentityLinkCmd(taskId, groupId, IdentityLinkType.Candidate));
        }

        public virtual void AddUserIdentityLink(string taskId, string userId, string identityLinkType)
        {
            CommandExecutor.Execute(new AddUserIdentityLinkCmd(taskId, userId, identityLinkType));
        }

        public virtual void AddGroupIdentityLink(string taskId, string groupId, string identityLinkType)
        {
            CommandExecutor.Execute(new AddGroupIdentityLinkCmd(taskId, groupId, identityLinkType));
        }

        public virtual void DeleteCandidateGroup(string taskId, string groupId)
        {
            CommandExecutor.Execute(new DeleteGroupIdentityLinkCmd(taskId, groupId, IdentityLinkType.Candidate));
        }

        public virtual void DeleteCandidateUser(string taskId, string userId)
        {
            CommandExecutor.Execute(new DeleteUserIdentityLinkCmd(taskId, userId, IdentityLinkType.Candidate));
        }

        public virtual void DeleteGroupIdentityLink(string taskId, string groupId, string identityLinkType)
        {
            CommandExecutor.Execute(new DeleteGroupIdentityLinkCmd(taskId, groupId, identityLinkType));
        }

        public virtual void DeleteUserIdentityLink(string taskId, string userId, string identityLinkType)
        {
            CommandExecutor.Execute(new DeleteUserIdentityLinkCmd(taskId, userId, identityLinkType));
        }

        public virtual IList<IIdentityLink> GetIdentityLinksForTask(string taskId)
        {
            return CommandExecutor.Execute(new GetIdentityLinksForTaskCmd(taskId));
        }

        public virtual void Claim(string taskId, string userId)
        {
            CommandExecutor.Execute(new ClaimTaskCmd(taskId, userId));
        }

        public virtual void Complete(string taskId)
        {
            CommandExecutor.Execute(new CompleteTaskCmd(taskId, null));
        }

        public virtual void Complete(string taskId, IDictionary<string, object> variables)
        {
            CommandExecutor.Execute(new CompleteTaskCmd(taskId, variables));
        }

        public virtual void DelegateTask(string taskId, string userId)
        {
            CommandExecutor.Execute(new DelegateTaskCmd(taskId, userId));
        }

        public virtual void ResolveTask(string taskId)
        {
            CommandExecutor.Execute(new ResolveTaskCmd(taskId, null));
        }

        public virtual void ResolveTask(string taskId, IDictionary<string, object> variables)
        {
            CommandExecutor.Execute(new ResolveTaskCmd(taskId, variables));
        }

        public virtual void SetPriority(string taskId, int priority)
        {
            CommandExecutor.Execute(new SetTaskPriorityCmd(taskId, priority));
        }

        //public virtual ITaskQuery CreateTaskQuery()
        //{
        //    return new TaskQueryImpl(CommandExecutor);
        //}

        public IQueryable<ITask> CreateTaskQuery(Expression<Func<TaskEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<TaskEntity>(expression));

        }
        public IQueryable<TaskEntity> CreateTaskEntityQuery(Expression<Func<TaskEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<TaskEntity>(expression));
        }
        public ITaskManager GetTaskManager()
        {
            return CommandExecutor.Execute(new GetTaskManagerCmd());
        }
        //public virtual INativeTaskQuery CreateNativeTaskQuery()
        //{
        //    return new NativeTaskQueryImpl(CommandExecutor);
        //}

        public virtual IVariableMap GetVariablesTyped(string executionId)
        {
            return GetVariablesTyped(executionId, true);
        }

        public virtual IVariableMap GetVariablesTyped(string taskId, bool deserializeValues)
        {
            return CommandExecutor.Execute(new GetTaskVariablesCmd(taskId, null, false, deserializeValues));
        }

        public virtual IVariableMap GetVariablesLocalTyped(string taskId)
        {
            return GetVariablesLocalTyped(taskId, true);
        }

        public virtual IVariableMap GetVariablesLocalTyped(string taskId, bool deserializeValues)
        {
            return CommandExecutor.Execute(new GetTaskVariablesCmd(taskId, null, true, deserializeValues));
        }

        public virtual IVariableMap GetVariablesTyped(string executionId, ICollection<string> variableNames,
            bool deserializeValues)
        {
            return CommandExecutor.Execute(new GetTaskVariablesCmd(executionId, variableNames, false, deserializeValues));
        }

        public virtual IVariableMap GetVariablesLocalTyped(string executionId, ICollection<string> variableNames,
            bool deserializeValues)
        {
            return CommandExecutor.Execute(new GetTaskVariablesCmd(executionId, variableNames, true, deserializeValues));
        }

        public virtual object GetVariable(string executionId, string variableName)
        {
            return CommandExecutor.Execute(new GetTaskVariableCmd(executionId, variableName, false));
        }

        public virtual object GetVariableLocal(string executionId, string variableName)
        {
            return CommandExecutor.Execute(new GetTaskVariableCmd(executionId, variableName, true));
        }

        public virtual T GetVariableTyped<T>(string taskId, string variableName)
        {
            return GetVariableTyped<T>(taskId, variableName, false, true);
        }

        public virtual T GetVariableTyped<T>(string taskId, string variableName, bool deserializeValue)
        {
            return GetVariableTyped<T>(taskId, variableName, false, deserializeValue);
        }

        public virtual T GetVariableLocalTyped<T>(string taskId, string variableName)
        {
            return GetVariableTyped<T>(taskId, variableName, true, true);
        }

        public virtual T GetVariableLocalTyped<T>(string taskId, string variableName, bool deserializeValue)
        {
            return GetVariableTyped<T>(taskId, variableName, true, deserializeValue);
        }

        public virtual void SetVariable(string executionId, string variableName, object value)
        {
            EnsureUtil.EnsureNotNull("variableName", variableName);
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables[variableName] = value;
            CommandExecutor.Execute(new SetTaskVariablesCmd(executionId, variables, false));
        }

        public virtual void SetVariableLocal(string executionId, string variableName, object value)
        {
            EnsureUtil.EnsureNotNull("variableName", variableName);
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables[variableName] = value;
            CommandExecutor.Execute(new SetTaskVariablesCmd(executionId, variables, true));
        }

        public virtual void RemoveVariable(string taskId, string variableName)
        {
            ICollection<string> variableNames = new List<string>();
            variableNames.Add(variableName);
            CommandExecutor.Execute(new RemoveTaskVariablesCmd(taskId, variableNames, false));
        }

        public virtual void RemoveVariableLocal(string taskId, string variableName)
        {
            ICollection<string> variableNames = new List<string>(1);
            variableNames.Add(variableName);
            CommandExecutor.Execute(new RemoveTaskVariablesCmd(taskId, variableNames, true));
        }

        public virtual void RemoveVariables(string taskId, ICollection<string> variableNames)
        {
            CommandExecutor.Execute(new RemoveTaskVariablesCmd(taskId, variableNames, false));
        }

        public virtual void RemoveVariablesLocal(string taskId, ICollection<string> variableNames)
        {
            CommandExecutor.Execute(new RemoveTaskVariablesCmd(taskId, variableNames, true));
        }

        public virtual void AddComment(string taskId, string processInstance, string message)
        {
            CreateComment(taskId, processInstance, message);
        }

        public virtual IComment CreateComment(string taskId, string processInstance, string message)
        {
            return CommandExecutor.Execute(new AddCommentCmd(taskId, processInstance, message));
        }

        public virtual IList<IComment> GetTaskComments(string taskId)
        {
            return CommandExecutor.Execute(new GetTaskCommentsCmd(taskId));
        }

        public virtual IComment GetTaskComment(string taskId, string commentId)
        {
            return CommandExecutor.Execute(new GetTaskCommentCmd(taskId, commentId));
        }

        public virtual IList<IEvent> GetTaskEvents(string taskId)
        {
            return CommandExecutor.Execute(new GetTaskEventsCmd(taskId));
        }

        public virtual IList<IComment> GetProcessInstanceComments(string processInstanceId)
        {
            return CommandExecutor.Execute(new GetProcessInstanceCommentsCmd(processInstanceId));
        }

        public virtual IAttachment CreateAttachment(string attachmentType, string taskId, string processInstanceId,
            string attachmentName, string attachmentDescription, Stream content)
        {
            return
                CommandExecutor.Execute(new CreateAttachmentCmd(attachmentType, taskId, processInstanceId,
                    attachmentName, attachmentDescription, content, null));
        }

        public virtual IAttachment CreateAttachment(string attachmentType, string taskId, string processInstanceId,
            string attachmentName, string attachmentDescription, string url)
        {
            return
                CommandExecutor.Execute(new CreateAttachmentCmd(attachmentType, taskId, processInstanceId,
                    attachmentName, attachmentDescription, null, url));
        }

        public virtual Stream GetAttachmentContent(string attachmentId)
        {
            return CommandExecutor.Execute(new GetAttachmentContentCmd(attachmentId));
        }

        public virtual Stream GetTaskAttachmentContent(string taskId, string attachmentId)
        {
            return CommandExecutor.Execute(new GetTaskAttachmentContentCmd(taskId, attachmentId));
        }

        public virtual void DeleteAttachment(string attachmentId)
        {
            CommandExecutor.Execute(new DeleteAttachmentCmd(attachmentId));
        }

        public virtual void DeleteTaskAttachment(string taskId, string attachmentId)
        {
            CommandExecutor.Execute(new DeleteTaskAttachmentCmd(taskId, attachmentId));
        }

        public virtual IAttachment GetAttachment(string attachmentId)
        {
            return CommandExecutor.Execute(new GetAttachmentCmd(attachmentId));
        }

        public virtual IAttachment GetTaskAttachment(string taskId, string attachmentId)
        {
            return CommandExecutor.Execute(new GetTaskAttachmentCmd(taskId, attachmentId));
        }

        public virtual IList<IAttachment> GetTaskAttachments(string taskId)
        {
            return CommandExecutor.Execute(new GetTaskAttachmentsCmd(taskId));
        }

        public virtual IList<IAttachment> GetProcessInstanceAttachments(string processInstanceId)
        {
            return CommandExecutor.Execute(new GetProcessInstanceAttachmentsCmd(processInstanceId));
        }

        public virtual void SaveAttachment(IAttachment attachment)
        {
            CommandExecutor.Execute(new SaveAttachmentCmd(attachment));
        }

        public virtual IList<ITask> GetSubTasks(string parentTaskId)
        {
            return CommandExecutor.Execute(new GetSubTasksCmd(parentTaskId));
        }

        public virtual ITaskReport CreateTaskReport()
        {
            return new TaskReportImpl(CommandExecutor);
        }

        //public void SetVariables<T1>(string taskId, IDictionary<string, T1> variables)
        //{

        //    throw new NotImplementedException();
        //}

        //public void SetVariablesLocal(string executionId, IDictionary<string, object> variables)
        //{
        //    SetVariables(executionId, variables, true);
        //}
        protected void SetVariables(String executionId, IDictionary<string, object> variables, bool local)
        {
            try
            {
                CommandExecutor.Execute(new SetTaskVariablesCmd(executionId, variables, local));
            }
            catch (ProcessEngineException ex)
            {
                //if (ExceptionUtil.CheckValueTooLongException(ex))
                //{
                    //throw new BadUserRequestException("Variable value is too long", ex);
                //}
                throw ex;
            }
        }
        public virtual IDictionary<string, object> GetVariables(string executionId)
        {
            return GetVariablesTyped(executionId);
        }

        public virtual IDictionary<string, object> GetVariablesLocal(string taskId)
        {
            return GetVariablesLocalTyped(taskId);
        }

        public virtual IDictionary<string, object> GetVariables(string executionId, ICollection<string> variableNames)
        {
            return GetVariablesTyped(executionId, variableNames, true);
        }

        public virtual IDictionary<string, object> GetVariablesLocal(string executionId, ICollection<string> variableNames)
        {
            return GetVariablesLocalTyped(executionId, variableNames, true);
        }

        protected internal virtual T GetVariableTyped<T>(string taskId, string variableName, bool isLocal,
            bool deserializeValue)
        {
            return (T)CommandExecutor.Execute(new GetTaskVariableCmdTyped(taskId, variableName, isLocal, deserializeValue));
        }

        public virtual void SetVariables(string executionId, IDictionary<string, object> variables)
        {
            SetVariables(executionId, variables, false);
            //CommandExecutor.Execute(new SetTaskVariablesCmd(executionId, variables, false));
        }

        public virtual void SetVariablesLocal(string executionId, IDictionary<string, object> variables)
        {
            SetVariables(executionId, variables, true);
            //CommandExecutor.Execute(new SetTaskVariablesCmd(executionId, variables, true));
        }

        public virtual void UpdateVariablesLocal(string taskId, IDictionary<string, object> modifications,
            ICollection<string> deletions)
        {
            CommandExecutor.Execute(new PatchTaskVariablesCmd(taskId, modifications, deletions, true));
        }

        public virtual void UpdateVariables<T1>(string taskId, IDictionary<string, object> modifications,
            ICollection<string> deletions)
        {
            CommandExecutor.Execute(new PatchTaskVariablesCmd(taskId, modifications, deletions, false));
        }

    }
}