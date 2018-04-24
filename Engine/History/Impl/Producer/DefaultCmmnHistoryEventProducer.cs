//using ESS.FW.Bpm.Engine.context.Impl;
//using ESS.FW.Bpm.Engine.Delegate;
//using ESS.FW.Bpm.Engine.Impl.History.Event;
//using ESS.FW.Bpm.Engine.Impl.Util;

//namespace ESS.FW.Bpm.Engine.Impl.History.Producer
//{
//    /// <summary>
//    ///     
//    /// </summary>
//    public class DefaultCmmnHistoryEventProducer : ICmmnHistoryEventProducer
//    {
//        public virtual HistoryEvent CreateCaseInstanceCreateEvt(IDelegateCaseExecution caseExecution)
//        {
////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = (org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity) caseExecution;
//            var caseExecutionEntity = (CaseExecutionEntity) caseExecution;

//            // create event instance
//            var evt = NewCaseInstanceEventEntity(caseExecutionEntity);

//            // initialize event
//            InitCaseInstanceEvent(evt, caseExecutionEntity, HistoryEventTypes.CaseInstanceCreate);

//            // set create time
//            evt.CreateTime = ClockUtil.CurrentTime;

//            // set create user id
//            evt.CreateUserId = Context.CommandContext.AuthenticatedUserId;

//            // set super case instance id
//            //CmmnExecution superCaseExecution = caseExecutionEntity.SuperCaseExecution;
//            //if (superCaseExecution != null)
//            //{
//            //    evt.SuperCaseInstanceId = superCaseExecution.CaseInstanceId;
//            //}

//            //// set super process instance id
//            //ExecutionEntity superExecution = caseExecutionEntity.getSuperExecution();
//            //if (superExecution != null)
//            //{
//            //    evt.SuperProcessInstanceId = superExecution.ProcessInstanceId;
//            //}

//            return evt;
//        }

//        public virtual HistoryEvent CreateCaseInstanceUpdateEvt(DelegateCaseExecution caseExecution)
//        {
////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = (org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity) caseExecution;
//            var caseExecutionEntity = (CaseExecutionEntity) caseExecution;

//            // create event instance
//            var evt = LoadCaseInstanceEventEntity(caseExecutionEntity);

//            // initialize event
//            InitCaseInstanceEvent(evt, caseExecutionEntity, HistoryEventTypes.CaseInstanceUpdate);

//            return evt;
//        }

//        public virtual HistoryEvent CreateCaseInstanceCloseEvt(DelegateCaseExecution caseExecution)
//        {
////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = (org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity) caseExecution;
//            var caseExecutionEntity = (CaseExecutionEntity) caseExecution;

//            // create event instance
//            var evt = LoadCaseInstanceEventEntity(caseExecutionEntity);

//            // initialize event
//            InitCaseInstanceEvent(evt, caseExecutionEntity, HistoryEventTypes.CaseInstanceClose);

//            // set end time
//            evt.EndTime = ClockUtil.CurrentTime;

//            if (evt.StartTime != null)
//                evt.DurationInMillis = evt.EndTime.Ticks - evt.StartTime.Ticks;

//            return evt;
//        }

//        public virtual HistoryEvent CreateCaseActivityInstanceCreateEvt(DelegateCaseExecution caseExecution)
//        {
////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = (org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity) caseExecution;
//            var caseExecutionEntity = (CaseExecutionEntity) caseExecution;

//            // create event instance
//            var evt = NewCaseActivityInstanceEventEntity(caseExecutionEntity);

//            // initialize event
//            InitCaseActivityInstanceEvent(evt, caseExecutionEntity, HistoryEventTypes.CaseActivityInstanceCreate);

//            // set start time
//            evt.CreateTime = ClockUtil.CurrentTime;

//            return evt;
//        }

//        public virtual HistoryEvent CreateCaseActivityInstanceUpdateEvt(DelegateCaseExecution caseExecution)
//        {
////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = (org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity) caseExecution;
//            var caseExecutionEntity = (CaseExecutionEntity) caseExecution;

//            // create event instance
//            var evt = LoadCaseActivityInstanceEventEntity(caseExecutionEntity);

//            // initialize event
//            InitCaseActivityInstanceEvent(evt, caseExecutionEntity, HistoryEventTypes.CaseActivityInstanceUpdate);

//            //if (caseExecutionEntity.ITask != null)
//            //{
//            //    evt.TaskId = caseExecutionEntity.ITask.Id;
//            //}

//            //if (caseExecutionEntity.getSubProcessInstance() != null)
//            //{
//            //    evt.CalledProcessInstanceId = caseExecutionEntity.getSubProcessInstance().Id;
//            //}

//            //if (caseExecutionEntity.getSubCaseInstance() != null)
//            //{
//            //    evt.CalledCaseInstanceId = caseExecutionEntity.getSubCaseInstance().Id;
//            //}

//            return evt;
//        }

//        public virtual HistoryEvent CreateCaseActivityInstanceEndEvt(DelegateCaseExecution caseExecution)
//        {
////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = (org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity) caseExecution;
//            var caseExecutionEntity = (CaseExecutionEntity) caseExecution;

//            // create event instance
//            var evt = LoadCaseActivityInstanceEventEntity(caseExecutionEntity);

//            // initialize event
//            InitCaseActivityInstanceEvent(evt, caseExecutionEntity, HistoryEventTypes.CaseActivityInstanceEnd);

//            // set end time
//            evt.EndTime = ClockUtil.CurrentTime;

//            // calculate duration
//            if (evt.StartTime != null)
//                evt.DurationInMillis = evt.EndTime.Ticks - evt.StartTime.Ticks;

//            return evt;
//        }

//        protected internal virtual HistoricCaseInstanceEventEntity NewCaseInstanceEventEntity(
//            CaseExecutionEntity caseExecutionEntity)
//        {
//            return new HistoricCaseInstanceEventEntity();
//        }

//        protected internal virtual HistoricCaseInstanceEventEntity LoadCaseInstanceEventEntity(
//            CaseExecutionEntity caseExecutionEntity)
//        {
//            return NewCaseInstanceEventEntity(caseExecutionEntity);
//        }

//        protected internal virtual void InitCaseInstanceEvent(HistoricCaseInstanceEventEntity evt,
//            CaseExecutionEntity caseExecutionEntity, HistoryEventTypes eventType)
//        {
//            evt.Id = caseExecutionEntity.CaseInstanceId;
//            //evt.EventType = eventType.EventName;
//            evt.CaseDefinitionId = caseExecutionEntity.CaseDefinitionId;
//            evt.CaseInstanceId = caseExecutionEntity.CaseInstanceId;
//            evt.CaseExecutionId = caseExecutionEntity.Id;
//            evt.BusinessKey = caseExecutionEntity.BusinessKey;
//            evt.State = caseExecutionEntity.State;
//            evt.TenantId = caseExecutionEntity.TenantId;
//        }

//        protected internal virtual HistoricCaseActivityInstanceEventEntity NewCaseActivityInstanceEventEntity(
//            CaseExecutionEntity caseExecutionEntity)
//        {
//            return new HistoricCaseActivityInstanceEventEntity();
//        }

//        protected internal virtual HistoricCaseActivityInstanceEventEntity LoadCaseActivityInstanceEventEntity(
//            CaseExecutionEntity caseExecutionEntity)
//        {
//            return NewCaseActivityInstanceEventEntity(caseExecutionEntity);
//        }

//        protected internal virtual void InitCaseActivityInstanceEvent(HistoricCaseActivityInstanceEventEntity evt,
//            CaseExecutionEntity caseExecutionEntity, HistoryEventTypes eventType)
//        {
//            evt.Id = caseExecutionEntity.Id;
//            evt.ParentCaseActivityInstanceId = caseExecutionEntity.ParentId;
//            //evt.EventType = eventType.EventName;
//            evt.CaseDefinitionId = caseExecutionEntity.CaseDefinitionId;
//            evt.CaseInstanceId = caseExecutionEntity.CaseInstanceId;
//            evt.CaseExecutionId = caseExecutionEntity.Id;
//            evt.CaseActivityInstanceState = caseExecutionEntity.State;

//            evt.Required = caseExecutionEntity.Required;

//            evt.CaseActivityId = caseExecutionEntity.ActivityId;
//            //evt.CaseActivityName = caseExecutionEntity.ActivityName;
//            //evt.CaseActivityType = caseExecutionEntity.ActivityType;

//            evt.TenantId = caseExecutionEntity.TenantId;
//        }
//    }
//}