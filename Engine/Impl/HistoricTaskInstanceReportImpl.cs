using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Query;

namespace ESS.FW.Bpm.Engine.Impl
{

    /// <summary>
    ///     .
    /// </summary>
    public class HistoricTaskInstanceReportImpl : IHistoricTaskInstanceReport
    {
        protected internal ICommandExecutor CommandExecutor;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal DateTime CompletedAfterRenamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal DateTime CompletedBeforeRenamed;

        protected internal PeriodUnit DurationPeriodUnit;

        protected internal TenantCheck tenantCheck = new TenantCheck();

        public HistoricTaskInstanceReportImpl(ICommandExecutor commandExecutor)
        {
            this.CommandExecutor = commandExecutor;
        }


        public virtual TenantCheck TenantCheck
        {
            get { return tenantCheck; }
        }

        public virtual string ReportPeriodUnitName
        {
            get { return DurationPeriodUnit.ToString(); }
        }

        public virtual IList<IHistoricTaskInstanceReportResult> CountByProcessDefinitionKey()
        {
            var commandContext = context.Impl.Context.CommandContext;

            if (commandContext == null)
                return CommandExecutor.Execute(new CommandAnonymousInnerClass(this, commandContext));
            return ExecuteCountByProcessDefinitionKey(commandContext);
        }

        public virtual IList<IHistoricTaskInstanceReportResult> CountByTaskName()
        {
            var commandContext = context.Impl.Context.CommandContext;

            if (commandContext == null)
                return CommandExecutor.Execute(new CommandAnonymousInnerClass2(this, commandContext));
            return ExecuteCountByTaskName(commandContext);
        }

        public virtual IList<IDurationReportResult> Duration(PeriodUnit periodUnit)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "periodUnit", periodUnit);
            DurationPeriodUnit = periodUnit;

            var commandContext = context.Impl.Context.CommandContext;

            if (commandContext == null)
                return CommandExecutor.Execute(new CommandAnonymousInnerClass3(this, commandContext));
            return ExecuteDuration(commandContext);
        }

        public virtual IHistoricTaskInstanceReport CompletedAfter(DateTime completedAfter)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "completedAfter", completedAfter);
            CompletedAfterRenamed = completedAfter;
            return this;
        }

        public virtual IHistoricTaskInstanceReport CompletedBefore(DateTime completedBefore)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "completedBefore", completedBefore);
            CompletedBeforeRenamed = completedBefore;
            return this;
        }

        protected internal virtual IList<IHistoricTaskInstanceReportResult> ExecuteCountByProcessDefinitionKey(
            CommandContext commandContext)
        {
            DoAuthCheck(commandContext);
            return null;
            //return commandContext.TaskReportManager.selectHistoricTaskInstanceCountByProcDefKeyReport(this);
        }

        protected internal virtual IList<IHistoricTaskInstanceReportResult> ExecuteCountByTaskName(
            CommandContext commandContext)
        {
            DoAuthCheck(commandContext);
            return null;
            //return commandContext.TaskReportManager.selectHistoricTaskInstanceCountByTaskNameReport(this);
        }


        protected internal virtual IList<IDurationReportResult> ExecuteDuration(CommandContext commandContext)
        {
            DoAuthCheck(commandContext);
            return null;
            //return commandContext.TaskReportManager.createHistoricTaskDurationReport(this);
        }

        protected internal virtual void DoAuthCheck(CommandContext commandContext)
        {
            // since a report does only make sense in context of historic
            // data, the authorization check will be performed here
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadHistoryAnyProcessDefinition();
        }

        private class CommandAnonymousInnerClass : ICommand<IList<IHistoricTaskInstanceReportResult>>
        {
            private readonly HistoricTaskInstanceReportImpl _outerInstance;

            private CommandContext _commandContext;

            public CommandAnonymousInnerClass(HistoricTaskInstanceReportImpl outerInstance,
                CommandContext commandContext)
            {
                this._outerInstance = outerInstance;
                this._commandContext = commandContext;
            }


            public virtual IList<IHistoricTaskInstanceReportResult> Execute(CommandContext commandContext)
            {
                return _outerInstance.ExecuteCountByProcessDefinitionKey(commandContext);
            }
        }

        private class CommandAnonymousInnerClass2 : ICommand<IList<IHistoricTaskInstanceReportResult>>
        {
            private readonly HistoricTaskInstanceReportImpl _outerInstance;

            private CommandContext _commandContext;

            public CommandAnonymousInnerClass2(HistoricTaskInstanceReportImpl outerInstance,
                CommandContext commandContext)
            {
                this._outerInstance = outerInstance;
                this._commandContext = commandContext;
            }


            public virtual IList<IHistoricTaskInstanceReportResult> Execute(CommandContext commandContext)
            {
                return _outerInstance.ExecuteCountByTaskName(commandContext);
            }
        }

        private class CommandAnonymousInnerClass3 : ICommand<IList<IDurationReportResult>>
        {
            private readonly HistoricTaskInstanceReportImpl _outerInstance;

            private CommandContext _commandContext;

            public CommandAnonymousInnerClass3(HistoricTaskInstanceReportImpl outerInstance,
                CommandContext commandContext)
            {
                this._outerInstance = outerInstance;
                this._commandContext = commandContext;
            }


            public virtual IList<IDurationReportResult> Execute(CommandContext commandContext)
            {
                return _outerInstance.ExecuteDuration(commandContext);
            }
        }
    }
}