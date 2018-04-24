using System.Collections.Generic;
using System.IO;
using System;
using System.Net.Mail;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    /// </summary>
    // Not Serializable
    public class CreateAttachmentCmd : ICommand<IAttachment>
    {
        protected internal string AttachmentDescription;
        protected internal string AttachmentName;
        protected internal string AttachmentType;
        protected internal Stream Content;
        protected internal ExecutionEntity ProcessInstance;
        protected internal string ProcessInstanceId;
        private TaskEntity _task;

        protected internal string TaskId;
        protected internal string Url;

        public CreateAttachmentCmd(string attachmentType, string taskId, string processInstanceId, string attachmentName,
            string attachmentDescription, Stream content, string url)
        {
            this.AttachmentType = attachmentType;
            this.TaskId = taskId;
            this.ProcessInstanceId = processInstanceId;
            this.AttachmentName = attachmentName;
            this.AttachmentDescription = attachmentDescription;
            this.Content = content;
            this.Url = url;
        }

        public virtual IAttachment Execute(CommandContext commandContext)
        {
            if (!ReferenceEquals(TaskId, null))
            {
                _task = commandContext.TaskManager.FindTaskById(TaskId);
            }
            else
            {
                EnsureUtil.EnsureNotNull("taskId or processInstanceId has to be provided", ProcessInstanceId);
                IList<ExecutionEntity> executionsByProcessInstanceId =
                    commandContext.ExecutionManager.FindExecutionsByProcessInstanceId(ProcessInstanceId);
                EnsureUtil.EnsureNumberOfElements("processInstances", executionsByProcessInstanceId, 1);
                ProcessInstance = executionsByProcessInstanceId[0];
            }

            var attachment = new AttachmentEntity();
            attachment.Name = AttachmentName;
            attachment.Description = AttachmentDescription;
            attachment.Type = AttachmentType;
            attachment.TaskId = TaskId;
            attachment.ProcessInstanceId = ProcessInstanceId;
            attachment.Url = Url;
            
            commandContext.AttachmentManager.Add(attachment);

            if (Content != null)
            {
                var bytes = IoUtil.ReadInputStream(Content, AttachmentName);
                var byteArray = new ResourceEntity(bytes);
                commandContext.ByteArrayManager.Add(byteArray);
                attachment.ContentId = byteArray.Id;
            }

            PropertyChange propertyChange = new PropertyChange("name", null, AttachmentName);

            if (_task != null)
            {
                commandContext.OperationLogManager.LogAttachmentOperation(
                    UserOperationLogEntryFields.OperationTypeAddAttachment, _task, propertyChange);
            }
            else if (ProcessInstance != null)
            {
                commandContext.OperationLogManager.LogAttachmentOperation(
                    UserOperationLogEntryFields.OperationTypeAddAttachment, ProcessInstance, propertyChange);
            }

            return attachment;
        }
    }
}