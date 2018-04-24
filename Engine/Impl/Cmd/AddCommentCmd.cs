using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    /// </summary>
    [Serializable]
    public class AddCommentCmd : ICommand<IComment>
    {
        private const long SerialVersionUid = 1L;
        protected internal string Message;
        protected internal string ProcessInstanceId;

        protected internal string TaskId;

        public AddCommentCmd(string taskId, string processInstanceId, string message)
        {
            this.TaskId = taskId;
            this.ProcessInstanceId = processInstanceId;
            this.Message = message;
        }

        public virtual IComment Execute(CommandContext commandContext)
        {
            if (ReferenceEquals(ProcessInstanceId, null) && ReferenceEquals(TaskId, null))
                throw new ProcessEngineException("Process instance id and ITask id is null");

            EnsureUtil.EnsureNotNull("Message", Message);

            var userId = commandContext.AuthenticatedUserId;
            var comment = new CommentEntity();
            comment.UserId = userId;
            //comment.Type = CommentEntity.TYPE_COMMENT;
            comment.Time = ClockUtil.CurrentTime;
            comment.TaskId = TaskId;
            comment.ProcessInstanceId = ProcessInstanceId;
            comment.Action = EventFields.ActionAddComment;

            var eventMessage = Message.Replace("\\s+", " ");
            if (eventMessage.Length > 163)
                eventMessage = eventMessage.Substring(0, 160) + "...";
            comment.Message = eventMessage;

            comment.FullMessage = Message;

            commandContext.CommentManager.Insert(comment);

            return (IComment) comment;
        }
    }
}