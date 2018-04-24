using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Variable.Value;
using HistoricDecisionInputInstanceEntity =
    ESS.FW.Bpm.Engine.History.Impl.Event.HistoricDecisionInputInstanceEntity;
using HistoricDecisionInstanceEntity = ESS.FW.Bpm.Engine.History.Impl.Event.HistoricDecisionInstanceEntity;
using HistoricDecisionOutputInstanceEntity =
    ESS.FW.Bpm.Engine.History.Impl.Event.HistoricDecisionOutputInstanceEntity;

namespace ESS.FW.Bpm.Engine.History.Impl.Producer
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public class DefaultDmnHistoryEventProducer : IDmnHistoryEventProducer
    {
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        protected internal virtual HistoricDecisionInstanceEntity UserId
        {
            set { value.UserId = Context.CommandContext.AuthenticatedUserId; }
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.history.event.HistoryEvent createDecisionEvaluatedEvt(final org.camunda.bpm.engine.delegate.DelegateExecution execution, final org.camunda.bpm.dmn.engine.delegate.DmnDecisionEvaluationEvent evaluationEvent)
        public virtual HistoryEvent CreateDecisionEvaluatedEvt(IDelegateExecution execution,
            IDmnDecisionEvaluationEvent evaluationEvent)
        {
            return CreateHistoryEvent(evaluationEvent,
                new HistoricDecisionInstanceSupplierAnonymousInnerClass(this, execution, evaluationEvent));
        }

        public HistoryEvent CreateDecisionEvaluatedEvt(IDelegateCaseExecution execution,
            IDmnDecisionEvaluationEvent decisionEvaluationEvent)
        {
            throw new NotImplementedException();
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.history.event.HistoryEvent createDecisionEvaluatedEvt(final org.camunda.bpm.engine.delegate.DelegateCaseExecution execution, final org.camunda.bpm.dmn.engine.delegate.DmnDecisionEvaluationEvent evaluationEvent)
        public virtual HistoryEvent createDecisionEvaluatedEvt(IDelegateCaseExecution execution,
            IDmnDecisionEvaluationEvent evaluationEvent)
        {
            return CreateHistoryEvent(evaluationEvent,
                new HistoricDecisionInstanceSupplierAnonymousInnerClass2(this, execution, evaluationEvent));
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.history.event.HistoryEvent createDecisionEvaluatedEvt(final org.camunda.bpm.dmn.engine.delegate.DmnDecisionEvaluationEvent evaluationEvent)
        public virtual HistoryEvent CreateDecisionEvaluatedEvt(IDmnDecisionEvaluationEvent evaluationEvent)
        {
            return CreateHistoryEvent(evaluationEvent,
                new HistoricDecisionInstanceSupplierAnonymousInnerClass3(this, evaluationEvent));
        }

        protected internal virtual HistoryEvent CreateHistoryEvent(IDmnDecisionEvaluationEvent evaluationEvent,
            IHistoricDecisionInstanceSupplier supplier)
        {
            var @event = NewDecisionEvaluationEvent(evaluationEvent);

            var rootDecisionEvent = supplier.CreateHistoricDecisionInstance(evaluationEvent.DecisionResult);
            @event.RootHistoricDecisionInstance = rootDecisionEvent;

            IList<HistoricDecisionInstanceEntity> requiredDecisionEvents = new List<HistoricDecisionInstanceEntity>();
            foreach (var requiredDecisionResult in evaluationEvent.RequiredDecisionResults)
            {
                var requiredDecisionEvent = supplier.CreateHistoricDecisionInstance(requiredDecisionResult);
                requiredDecisionEvents.Add(requiredDecisionEvent);
            }
            @event.RequiredHistoricDecisionInstances = requiredDecisionEvents;

            return @event;
        }

        protected internal virtual HistoricDecisionInstanceEntity CreateDecisionEvaluatedEvt(
            IDmnDecisionLogicEvaluationEvent evaluationEvent, ExecutionEntity execution)
        {
            // create event instance
            var @event = NewDecisionInstanceEventEntity(execution, evaluationEvent);
            // initialize event
            InitDecisionInstanceEvent(@event, evaluationEvent, HistoryEventTypes.DmnDecisionEvaluate);
            SetReferenceToProcessInstance(@event, execution);
            // set current time as evaluation time
            @event.EvaluationTime = ClockUtil.CurrentTime;

            var decisionDefinition = (IDecisionDefinition) evaluationEvent.Decision;
            var tenantId = execution.TenantId;
            if (ReferenceEquals(tenantId, null))
                tenantId = ProvideTenantId(decisionDefinition, @event);
            @event.TenantId = tenantId;
            return @event;
        }

        //protected internal virtual HistoricDecisionInstanceEntity CreateDecisionEvaluatedEvt(
        //    DmnDecisionLogicEvaluationEvent evaluationEvent, CaseExecutionEntity execution)
        //{
        //    // create event instance
        //    var @event = NewDecisionInstanceEventEntity(execution, evaluationEvent);
        //    // initialize event
        //    InitDecisionInstanceEvent(@event, evaluationEvent, HistoryEventTypes.DmnDecisionEvaluate);
        //    SetReferenceToCaseInstance(@event, execution);
        //    // set current time as evaluation time
        //    @event.EvaluationTime = ClockUtil.CurrentTime;

        //    var decisionDefinition = (IDecisionDefinition) evaluationEvent.Decision;
        //    var tenantId = execution.TenantId;
        //    if (ReferenceEquals(tenantId, null))
        //        tenantId = ProvideTenantId(decisionDefinition, @event);
        //    @event.TenantId = tenantId;
        //    return @event;
        //}

        protected internal virtual HistoricDecisionInstanceEntity CreateDecisionEvaluatedEvt(
            IDmnDecisionLogicEvaluationEvent evaluationEvent)
        {
            // create event instance
            var @event = NewDecisionInstanceEventEntity(evaluationEvent);
            // initialize event
            InitDecisionInstanceEvent(@event, evaluationEvent, HistoryEventTypes.DmnDecisionEvaluate);
            // set current time as evaluation time
            @event.EvaluationTime = ClockUtil.CurrentTime;
            // set the user id if there is an authenticated user and no process instance
            UserId = @event;

            var decisionDefinition = (IDecisionDefinition) evaluationEvent.Decision;
            var tenantId = decisionDefinition.TenantId;
            if (ReferenceEquals(tenantId, null))
                tenantId = ProvideTenantId(decisionDefinition, @event);
            @event.TenantId = tenantId;
            return @event;
        }

        protected internal virtual HistoricDecisionEvaluationEvent NewDecisionEvaluationEvent(
            IDmnDecisionEvaluationEvent evaluationEvent)
        {
            return new HistoricDecisionEvaluationEvent();
        }

        protected internal virtual HistoricDecisionInstanceEntity NewDecisionInstanceEventEntity(
            ExecutionEntity executionEntity, IDmnDecisionLogicEvaluationEvent evaluationEvent)
        {
            return new HistoricDecisionInstanceEntity();
        }

        //protected internal virtual HistoricDecisionInstanceEntity NewDecisionInstanceEventEntity(
        //    CaseExecutionEntity executionEntity, DmnDecisionLogicEvaluationEvent evaluationEvent)
        //{
        //    return new HistoricDecisionInstanceEntity();
        //}

        protected internal virtual HistoricDecisionInstanceEntity NewDecisionInstanceEventEntity(
            IDmnDecisionLogicEvaluationEvent evaluationEvent)
        {
            return new HistoricDecisionInstanceEntity();
        }

        protected internal virtual void InitDecisionInstanceEvent(HistoricDecisionInstanceEntity @event,
            IDmnDecisionLogicEvaluationEvent evaluationEvent, HistoryEventTypes eventType)
        {
            throw new NotImplementedException();
            //@event.EventType = eventType.EventName;

            //var decision = (IDecisionDefinition) evaluationEvent.Decision;
            //@event.DecisionDefinitionId = decision.Id;
            //@event.DecisionDefinitionKey = decision.Key;
            //@event.DecisionDefinitionName = decision.Name;

            //if (!ReferenceEquals(decision.DecisionRequirementsDefinitionId, null))
            //{
            //    @event.DecisionRequirementsDefinitionId = decision.DecisionRequirementsDefinitionId;
            //    @event.DecisionRequirementsDefinitionKey = decision.DecisionRequirementsDefinitionKey;
            //}

            //if (evaluationEvent is DmnDecisionTableEvaluationEvent)
            //    InitDecisionInstanceEventForDecisionTable(@event, (DmnDecisionTableEvaluationEvent) evaluationEvent);
            //else if (evaluationEvent is DmnDecisionLiteralExpressionEvaluationEvent)
            //    InitDecisionInstanceEventForDecisionLiteralExpression(@event,
            //        (DmnDecisionLiteralExpressionEvaluationEvent) evaluationEvent);
        }

        protected internal virtual void InitDecisionInstanceEventForDecisionTable(HistoricDecisionInstanceEntity @event,
            IDmnDecisionTableEvaluationEvent evaluationEvent)
        {
            if (evaluationEvent.CollectResultValue != null)
            {
                //double? collectResultValue = getCollectResultValue(evaluationEvent.CollectResultValue);
                //@event.CollectResultValue = collectResultValue;
            }

            var historicDecisionInputInstances = CreateHistoricDecisionInputInstances(evaluationEvent);
            @event.Inputs = historicDecisionInputInstances;

            var historicDecisionOutputInstances = CreateHistoricDecisionOutputInstances(evaluationEvent);
            @event.Outputs = historicDecisionOutputInstances;
        }

        protected internal virtual double? GetCollectResultValue(ITypedValue collectResultValue)
        {
            // the built-in collect aggregators return only numbers

            //if (collectResultValue is IntegerValue)
            //{
            //  return ((IntegerValue) collectResultValue).Value.doubleValue();

            //}
            //else if (collectResultValue is LongValue)
            //{
            //  return ((LongValue) collectResultValue).Value.doubleValue();

            //}
            //else if (collectResultValue is DoubleValue)
            //{
            //  return ((DoubleValue) collectResultValue).Value;

            //}
            //else
            //{
            throw Log.CollectResultValueOfUnsupportedTypeException(collectResultValue);
            //}
        }

        protected internal virtual IList<IHistoricDecisionInputInstance> CreateHistoricDecisionInputInstances(
            IDmnDecisionTableEvaluationEvent evaluationEvent)
        {
            IList<IHistoricDecisionInputInstance> inputInstances = new List<IHistoricDecisionInputInstance>();

            foreach (var inputClause in evaluationEvent.Inputs)
            {
                var inputInstance = new HistoricDecisionInputInstanceEntity();
                inputInstance.ClauseId = inputClause.Id;
                inputInstance.ClauseName = inputClause.Name;

                //ITypedValue typedValue = Variables.untypedValue(inputClause.Value);
                //inputInstance.setValue(typedValue);

                inputInstances.Add(inputInstance);
            }

            return inputInstances;
        }

        protected internal virtual IList<IHistoricDecisionOutputInstance> CreateHistoricDecisionOutputInstances(
            IDmnDecisionTableEvaluationEvent evaluationEvent)
        {
            IList<IHistoricDecisionOutputInstance> outputInstances = new List<IHistoricDecisionOutputInstance>();

            var matchingRules = evaluationEvent.MatchingRules;
            for (var index = 0; index < matchingRules.Count; index++)
            {
                var rule = matchingRules[index];

                var ruleId = rule.Id;
                int? ruleOrder = index + 1;

                //foreach (DmnEvaluatedOutput outputClause in rule.OutputEntries.values())
                {
                    //HistoricDecisionOutputInstanceEntity outputInstance = new HistoricDecisionOutputInstanceEntity();
                    //outputInstance.ClauseId = outputClause.Id;
                    //outputInstance.ClauseName = outputClause.Name;

                    //outputInstance.RuleId = ruleId;
                    //outputInstance.RuleOrder = ruleOrder;

                    //outputInstance.VariableName = outputClause.OutputName;
                    //outputInstance.setValue(outputClause.Value);

                    //outputInstances.Add(outputInstance);
                }
            }

            return outputInstances;
        }

        protected internal virtual void InitDecisionInstanceEventForDecisionLiteralExpression(
            HistoricDecisionInstanceEntity @event, IDmnDecisionLiteralExpressionEvaluationEvent evaluationEvent)
        {
            // no inputs for expression
            //@event.Inputs = System.Linq.Enumerable.Empty<IHistoricDecisionInputInstance> ();

            var outputInstance = new HistoricDecisionOutputInstanceEntity();
            outputInstance.VariableName = evaluationEvent.OutputName;
            //outputInstance.setValue(evaluationEvent.OutputValue);

            //@event.Outputs = Collections.singletonList<HistoricDecisionOutputInstance> (outputInstance);
        }

        protected internal virtual void SetReferenceToProcessInstance(HistoricDecisionInstanceEntity @event,
            ExecutionEntity execution)
        {
            @event.ProcessDefinitionKey = GetProcessDefinitionKey(execution);
            @event.ProcessDefinitionId = execution.ProcessDefinitionId;

            @event.ProcessInstanceId = execution.ProcessInstanceId;
            @event.ExecutionId = execution.Id;

            @event.ActivityId = execution.ActivityId;
            @event.ActivityInstanceId = execution.ActivityInstanceId;
        }

        protected internal virtual string GetProcessDefinitionKey(ExecutionEntity execution)
        {
            //ProcessDefinitionEntity definition = execution.getProcessDefinition();
            //if (definition != null)
            //{
            //  return definition.Key;
            //}
            //else
            //{
            return null;
            //}
        }

        //protected internal virtual void SetReferenceToCaseInstance(HistoricDecisionInstanceEntity @event,
        //    CaseExecutionEntity execution)
        //{
        //    @event.CaseDefinitionKey = GetCaseDefinitionKey(execution);
        //    @event.CaseDefinitionId = execution.CaseDefinitionId;

        //    @event.CaseInstanceId = execution.CaseInstanceId;
        //    @event.ExecutionId = execution.Id;

        //    @event.ActivityId = execution.ActivityId;
        //    @event.ActivityInstanceId = execution.Id;
        //}

        //protected internal virtual string GetCaseDefinitionKey(CaseExecutionEntity execution)
        //{
        //    //CaseDefinitionEntity definition = (CaseDefinitionEntity) execution.CaseDefinition;
        //    //if (definition != null)
        //    //{
        //    //  return definition.Key;
        //    //}
        //    //else
        //    //{
        //    return null;
        //    //}
        //}

        protected internal virtual string ProvideTenantId(IDecisionDefinition decisionDefinition,
            HistoricDecisionInstanceEntity @event)
        {
            var tenantIdProvider = Context.ProcessEngineConfiguration.TenantIdProvider;
            string tenantId = null;

            if (tenantIdProvider != null)
            {
                TenantIdProviderHistoricDecisionInstanceContext ctx = null;

                if (!ReferenceEquals(@event.ExecutionId, null))
                {
                    //ctx = new TenantIdProviderHistoricDecisionInstanceContext(decisionDefinition, getExecution(@event));
                }
                else if (!ReferenceEquals(@event.CaseExecutionId, null))
                {
                    //ctx = new TenantIdProviderHistoricDecisionInstanceContext(decisionDefinition, getCaseExecution(@event));
                }
                else
                {
                    ctx = new TenantIdProviderHistoricDecisionInstanceContext(decisionDefinition);
                }

                tenantId = tenantIdProvider.ProvideTenantIdForHistoricDecisionInstance(ctx);
            }

            return tenantId;
        }

        private class HistoricDecisionInstanceSupplierAnonymousInnerClass : IHistoricDecisionInstanceSupplier
        {
            private readonly IDelegateExecution _execution;
            private readonly DefaultDmnHistoryEventProducer _outerInstance;
            private IDmnDecisionEvaluationEvent _evaluationEvent;

            public HistoricDecisionInstanceSupplierAnonymousInnerClass(DefaultDmnHistoryEventProducer outerInstance,
                IDelegateExecution execution, IDmnDecisionEvaluationEvent evaluationEvent)
            {
                this._outerInstance = outerInstance;
                this._execution = execution;
                this._evaluationEvent = evaluationEvent;
            }


            public virtual HistoricDecisionInstanceEntity CreateHistoricDecisionInstance(
                IDmnDecisionLogicEvaluationEvent evaluationEvent)
            {
                throw new NotImplementedException();
                //return _outerInstance.createDecisionEvaluatedEvt(evaluationEvent, (ExecutionEntity) _execution);
            }
        }

        private class HistoricDecisionInstanceSupplierAnonymousInnerClass2 : IHistoricDecisionInstanceSupplier
        {
            private readonly IDelegateCaseExecution _execution;
            private readonly DefaultDmnHistoryEventProducer _outerInstance;
            private IDmnDecisionEvaluationEvent _evaluationEvent;

            public HistoricDecisionInstanceSupplierAnonymousInnerClass2(DefaultDmnHistoryEventProducer outerInstance,
IDelegateCaseExecution execution, IDmnDecisionEvaluationEvent evaluationEvent)
            {
                this._outerInstance = outerInstance;
                this._execution = execution;
                this._evaluationEvent = evaluationEvent;
            }


            public virtual HistoricDecisionInstanceEntity CreateHistoricDecisionInstance(
                IDmnDecisionLogicEvaluationEvent evaluationEvent)
            {
                throw new NotImplementedException();
                //return _outerInstance.createDecisionEvaluatedEvt(evaluationEvent, (CaseExecutionEntity) _execution);
            }
        }

        private class HistoricDecisionInstanceSupplierAnonymousInnerClass3 : IHistoricDecisionInstanceSupplier
        {
            private readonly DefaultDmnHistoryEventProducer _outerInstance;

            private IDmnDecisionEvaluationEvent _evaluationEvent;

            public HistoricDecisionInstanceSupplierAnonymousInnerClass3(DefaultDmnHistoryEventProducer outerInstance,
                IDmnDecisionEvaluationEvent evaluationEvent)
            {
                this._outerInstance = outerInstance;
                this._evaluationEvent = evaluationEvent;
            }


            public virtual HistoricDecisionInstanceEntity CreateHistoricDecisionInstance(
                IDmnDecisionLogicEvaluationEvent evaluationEvent)
            {
                return _outerInstance.CreateDecisionEvaluatedEvt(evaluationEvent);
            }
        }

        protected internal interface IHistoricDecisionInstanceSupplier
        {
            HistoricDecisionInstanceEntity CreateHistoricDecisionInstance(
                IDmnDecisionLogicEvaluationEvent evaluationEvent);
        }

        // }
        //  return Context.CommandContext.CaseExecutionManager.findCaseExecutionById(@event.CaseExecutionId);
        // {

        // protected internal virtual DelegateCaseExecution getCaseExecution(HistoricDecisionInstanceEntity @event)
        // }
        //return Context.CommandContext.ExecutionManager.findExecutionById(@event.ExecutionId);
        // {

        // protected internal virtual IDelegateExecution getExecution(HistoricDecisionInstanceEntity @event)
    }
}