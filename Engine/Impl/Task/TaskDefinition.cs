using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Form.Impl.Handler;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.task
{
    /// <summary>
    ///     Container for task definition information gathered at parsing time.
    /// </summary>
    public class TaskDefinition
    {
        protected internal IDictionary<string, IList<ITaskListener>> builtinTaskListeners =
            new Dictionary<string, IList<ITaskListener>>();

        protected internal ISet<IExpression> candidateGroupIdExpressions = new HashSet<IExpression>();
        protected internal ISet<IExpression> candidateUserIdExpressions = new HashSet<IExpression>();

        // assignment fields

        // form fields

        // task listeners

        public TaskDefinition(ITaskFormHandler taskFormHandler)
        {
            this.TaskFormHandler = taskFormHandler;
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual IExpression NameExpression { get; set; }


        public virtual IExpression DescriptionExpression { get; set; }


        public virtual IExpression AssigneeExpression { get; set; }


        public virtual ISet<IExpression> CandidateUserIdExpressions
        {
            get { return candidateUserIdExpressions; }
        }

        public virtual ISet<IExpression> CandidateGroupIdExpressions
        {
            get { return candidateGroupIdExpressions; }
        }

        public virtual IExpression PriorityExpression { get; set; }


        public virtual ITaskFormHandler TaskFormHandler { get; set; }


        public virtual string Key { get; set; }


        public virtual IExpression DueDateExpression { get; set; }


        public virtual IExpression FollowUpDateExpression { get; set; }


        public virtual IDictionary<string, IList<ITaskListener>> TaskListeners { get; set; } = new Dictionary<string, IList<ITaskListener>>();

        public virtual IDictionary<string, IList<ITaskListener>> BuiltinTaskListeners
        {
            get { return builtinTaskListeners; }
        }


        public virtual IExpression FormKey { get; set; }

        public virtual void AddCandidateUserIdExpression(IExpression userId)
        {
            candidateUserIdExpressions.Add(userId);
        }

        public virtual void AddCandidateGroupIdExpression(IExpression groupId)
        {
            candidateGroupIdExpressions.Add(groupId);
        }


        public virtual IList<ITaskListener> GetTaskListeners(string eventName)
        {
            return TaskListeners.ContainsKey(eventName) ? TaskListeners[eventName] : null;
        }

        public virtual IList<ITaskListener> GetBuiltinTaskListeners(string eventName)
        {
            return builtinTaskListeners.ContainsKey(eventName)? builtinTaskListeners[eventName]:null;
        }

        public virtual void AddTaskListener(string eventName, ITaskListener taskListener)
        {
            CollectionUtil.AddToMapOfLists(TaskListeners, eventName, taskListener);
        }

        public virtual void AddBuiltInTaskListener(string eventName, ITaskListener taskListener)
        {
            CollectionUtil.AddToMapOfLists(TaskListeners, eventName, taskListener);
            CollectionUtil.AddToMapOfLists(builtinTaskListeners, eventName, taskListener);
        }
    }
}