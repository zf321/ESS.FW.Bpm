using System;
using System.Collections;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Calendar;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using System.Linq;

namespace ESS.FW.Bpm.Engine.Impl.task
{
    /// <summary>
    ///     
    /// </summary>
    public class TaskDecorator
    {
        protected internal ExpressionManager expressionManager;

        protected internal TaskDefinition taskDefinition;

        public TaskDecorator(TaskDefinition taskDefinition, ExpressionManager expressionManager)
        {
            this.taskDefinition = taskDefinition;
            this.expressionManager = expressionManager;
        }

        // getters ///////////////////////////////////////////////////////////////

        public virtual TaskDefinition TaskDefinition
        {
            get { return taskDefinition; }
        }

        public virtual ExpressionManager ExpressionManager
        {
            get { return expressionManager; }
        }

        protected internal virtual IBusinessCalendar BusinessCalender
        {
            get
            {
                return
                    Context.ProcessEngineConfiguration.BusinessCalendarManager.GetBusinessCalendar(
                        DueDateBusinessCalendar.Name);
            }
        }

        public virtual void Decorate(TaskEntity task, IVariableScope variableScope)
        {
            // set the taskDefinition
            task.TaskDefinition = taskDefinition;

            // name
            InitializeTaskName(task, variableScope);
            // description
            InitializeTaskDescription(task, variableScope);
            // dueDate
            InitializeTaskDueDate(task, variableScope);
            // followUpDate
            InitializeTaskFollowUpDate(task, variableScope);
            // priority
            InitializeTaskPriority(task, variableScope);
            // assignments
            InitializeTaskAssignments(task, variableScope);
        }

        protected internal virtual void InitializeTaskName(TaskEntity task, IVariableScope variableScope)
        {
            var nameExpression = taskDefinition.NameExpression;
            if (nameExpression != null)
            {
                var name = (string)nameExpression.GetValue(variableScope);
                task.Name = name;
            }
        }

        protected internal virtual void InitializeTaskDescription(TaskEntity task, IVariableScope variableScope)
        {
            var descriptionExpression = taskDefinition.DescriptionExpression;
            if (descriptionExpression != null)
            {
                var description = (string) descriptionExpression.GetValue(variableScope);
                task.Description = description;
            }
        }

        protected internal virtual void InitializeTaskDueDate(TaskEntity task, IVariableScope variableScope)
        {
            var dueDateExpression = taskDefinition.DueDateExpression;
            if (dueDateExpression != null)
            {
                var dueDate = dueDateExpression.GetValue(variableScope);
                if (dueDate != null)
                    if (dueDate is DateTime)
                    {
                        task.DueDate = (DateTime)dueDate;
                    }
                    else if (dueDate is string)
                    {
                        var businessCalendar = BusinessCalender;
                        task.DueDate = businessCalendar.ResolveDuedate((string)dueDate);
                    }
                    else
                    {
                        throw new ProcessEngineException(
                            "Due date expression does not resolve to a Date or Date string: " +
                            dueDateExpression.ExpressionText);
                    }
            }
        }

        protected internal virtual void InitializeTaskFollowUpDate(TaskEntity task, IVariableScope variableScope)
        {
            var followUpDateExpression = taskDefinition.FollowUpDateExpression;
            if (followUpDateExpression != null)
            {
                var followUpDate = followUpDateExpression.GetValue(variableScope);
                if (followUpDate != null)
                    if (followUpDate is DateTime)
                    {
                        task.FollowUpDate = (DateTime)followUpDate;
                    }
                    else if (followUpDate is string)
                    {
                        var businessCalendar = BusinessCalender;
                        task.FollowUpDate = businessCalendar.ResolveDuedate((string)followUpDate);
                    }
                    else
                    {
                        throw new ProcessEngineException(
                            "Follow up date expression does not resolve to a Date or Date string: " +
                            followUpDateExpression.ExpressionText);
                    }
            }
        }

        protected internal virtual void InitializeTaskPriority(TaskEntity task, IVariableScope variableScope)
        {
            var priorityExpression = taskDefinition.PriorityExpression;
            if (priorityExpression != null)
            {
                var priority = priorityExpression.GetValue(variableScope);

                if (priority != null)
                    if (priority is string)
                    {
                        try
                        {
                            task.Priority = Convert.ToInt32((string)priority);
                        }
                        catch (FormatException e)
                        {
                            throw new ProcessEngineException("Priority does not resolve to a number: " + priority, e);
                        }
                    }
                    else if (priority is decimal || priority is int)
                    {
                        task.Priority = Convert.ToInt32(priority);
                    }
                    else
                    {
                        throw new ProcessEngineException("Priority expression does not resolve to a number: " +
                                                         priorityExpression.ExpressionText);
                    }
            }
        }

        protected internal virtual void InitializeTaskAssignments(TaskEntity task, IVariableScope variableScope)
        {
            // assignee
            InitializeTaskAssignee(task, variableScope);
            // candidateUsers
            InitializeTaskCandidateUsers(task, variableScope);
            // candidateGroups
            InitializeTaskCandidateGroups(task, variableScope);
        }

        protected internal virtual void InitializeTaskAssignee(TaskEntity task, IVariableScope variableScope)
        {
            var assigneeExpression = taskDefinition.AssigneeExpression;
            if (assigneeExpression != null)
            {
                //TODO 表达式相关
                task.Assignee = (string) assigneeExpression.GetValue(variableScope);
            }
        }
        
        protected internal virtual void InitializeTaskCandidateGroups(TaskEntity task, IVariableScope variableScope)
        {
            var candidateGroupIdExpressions = taskDefinition.CandidateGroupIdExpressions;

            foreach (var groupIdExpr in candidateGroupIdExpressions)
            {
                var value = groupIdExpr.GetValue(variableScope);

                if (value is string)
                {
                    var candiates = ExtractCandidates((string) value);
                    task.AddCandidateGroups(candiates);
                }
                else if (value is ICollection<string>)
                {
                    task.AddCandidateGroups(((ICollection<string>)value).Distinct().ToArray());
                }
            }
        }
        
        protected internal virtual void InitializeTaskCandidateUsers(TaskEntity task, IVariableScope variableScope)
        {
            var candidateUserIdExpressions = taskDefinition.CandidateUserIdExpressions;
            foreach (var userIdExpr in candidateUserIdExpressions)
            {
                var value = userIdExpr.GetValue(variableScope);

                if (value is string)
                {
                    var candiates = ExtractCandidates((string) value);
                    task.AddCandidateUsers(candiates);
                }
                else if (value is ICollection)
                {
                    task.AddCandidateUsers(((ICollection<string>) value).Distinct().ToArray());
                }
                else
                {
                    throw new ProcessEngineException("Expression did not resolve to a string or collection of strings");
                }
            }
        }


        /// <summary>
        ///     Extract a candidate list from a string.
        /// </summary>
        protected internal virtual IList<string> ExtractCandidates(string str)
        {
            return str.Split("[\\s]*,[\\s]*", true);
        }
    }
}