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
    ///     
    /// </summary>
    public class HistoricProcessInstanceReportImpl : IHistoricProcessInstanceReport
    {
        private const long SerialVersionUid = 1L;

        protected internal ICommandExecutor commandExecutor;

        protected internal PeriodUnit DurationPeriodUnit;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string[] _processDefinitionIdIn;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string[] _processDefinitionKeyIn;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal DateTime _startedAfter;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal DateTime _startedBefore;

        protected internal TenantCheck _tenantCheck = new TenantCheck();

        public HistoricProcessInstanceReportImpl(ICommandExecutor commandExecutor)
        {
            this.commandExecutor = commandExecutor;
        }

        // getter //////////////////////////////////////////////////////

        public virtual TenantCheck TenantCheck
        {
            get { return _tenantCheck; }
        }

        public virtual string ReportPeriodUnitName
        {
            get { return DurationPeriodUnit.ToString(); }
        }

        // query parameter ///////////////////////////////////////////////

        public virtual IHistoricProcessInstanceReport StartedAfter(DateTime startedAfter)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "startedAfter", startedAfter);
            _startedAfter = startedAfter;
            return this;
        }

        public virtual IHistoricProcessInstanceReport StartedBefore(DateTime startedBefore)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "startedBefore", startedBefore);
            _startedBefore = startedBefore;
            return this;
        }

        public virtual IHistoricProcessInstanceReport ProcessDefinitionIdIn(params string[] processDefinitionIds)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "", "processDefinitionIdIn", processDefinitionIds);
            _processDefinitionIdIn = processDefinitionIds;
            return this;
        }

        public virtual IHistoricProcessInstanceReport ProcessDefinitionKeyIn(params string[] processDefinitionKeys)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "", "processDefinitionKeyIn", processDefinitionKeys);
            _processDefinitionKeyIn = processDefinitionKeys;
            return this;
        }

        // report execution /////////////////////////////////////////////

        public virtual IList<IDurationReportResult> Duration(PeriodUnit periodUnit)
        {
            throw new NotImplementedException();
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "periodUnit", periodUnit);
            DurationPeriodUnit = periodUnit;

            var commandContext = context.Impl.Context.CommandContext;

            if (commandContext == null)
            {
                //return commandExecutor.execute(new CommandAnonymousInnerClass(this, commandContext));
            }
            //return ExecuteDurationReport(commandContext);
        }

        //public virtual IList<DurationReportResult> executeDurationReport(CommandContext commandContext)
        //{
        //    doAuthCheck(commandContext);

        //    if (areNotInAscendingOrder(startedAfter_Renamed, startedBefore_Renamed))
        //    {
        //        return Collections.emptyList();
        //    }

        //    return commandContext.HistoricReportManager.selectHistoricProcessInstanceDurationReport(this);
        //}

        protected internal virtual void DoAuthCheck(CommandContext commandContext)
        {
            // since a report does only make sense in context of historic
            // data, the authorization check will be performed here
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadHistoryAnyProcessDefinition();
        }

        private class CommandAnonymousInnerClass : ICommand<IList<IDurationReportResult>>
        {
            private readonly HistoricProcessInstanceReportImpl _outerInstance;

            private CommandContext _commandContext;

            public CommandAnonymousInnerClass(HistoricProcessInstanceReportImpl outerInstance,
                CommandContext commandContext)
            {
                this._outerInstance = outerInstance;
                this._commandContext = commandContext;
            }


            public virtual IList<IDurationReportResult> Execute(CommandContext commandContext)
            {
                //return outerInstance.executeDurationReport(commandContext);
                throw new NotImplementedException();
            }
        }
    }
}