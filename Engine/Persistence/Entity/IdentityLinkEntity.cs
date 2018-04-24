using System;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Task;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class IdentityLinkEntity : IIdentityLink, IDbEntity
    {
        private const long SerialVersionUid = 1L;
        private static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        private string _groupId;

        private string _userId;
        //[NotMapped]
        //private TaskEntity _task;
        [NotMapped]//[ForeignKey("TaskId")]
        public virtual TaskEntity Task {
            //get
            //{
            //    if ((_task == null) && (_taskId != null))
            //        _task = Context.CommandContext.TaskManager.FindTaskById(_taskId);
            //    return _task;
            //}
            set
            {
                //_task = value;
                TaskId = value.Id;
            } }
        [NotMapped]
        private ProcessDefinitionEntity _processDef;
        [NotMapped]//[ForeignKey("ProcessDefId")]
        public virtual ProcessDefinitionEntity ProcessDef {
            get
            {
                if ((_processDef == null) && (ProcessDefId != null))
                    _processDef =
                        Context.CommandContext.ProcessDefinitionManager.FindLatestProcessDefinitionById(ProcessDefId);
                return _processDef;
            }
            set
            {
                _processDef = value;
                ProcessDefId = value.Id;
            }
        }

        public virtual bool User
        {
            get { return _userId != null; }
        }

        public virtual bool Group
        {
            get { return _groupId != null; }
        }

        public virtual object GetPersistentState()
        {
            return Type;
        }

        public virtual string Id { get; set; }


        public virtual string Type { get; set; }


        public virtual string UserId
        {
            get { return _userId; }
            set
            {
                if ((_groupId != null) && (value != null))
                    throw Log.TaskIsAlreadyAssignedException("userId", "groupId");
                _userId = value;
            }
        }


        public virtual string GroupId
        {
            get { return _groupId; }
            set
            {
                if ((_userId != null) && (value != null))
                    throw Log.TaskIsAlreadyAssignedException("groupId", "userId");
                _groupId = value;
            }
        }


        public virtual string TaskId { get; set; }


        public virtual string ProcessDefId { get; set; }


        public virtual string TenantId { get; set; }

        public static IdentityLinkEntity CreateAndInsert()
        {
            var identityLinkEntity = new IdentityLinkEntity();
            identityLinkEntity.Insert();
            return identityLinkEntity;
        }

        public static IdentityLinkEntity NewIdentityLink()
        {
            var identityLinkEntity = new IdentityLinkEntity();
            return identityLinkEntity;
        }

        public virtual void Insert()
        {
            Context.CommandContext.IdentityLinkManager.Add(this);
            FireHistoricIdentityLinkEvent(HistoryEventTypes.IdentityLinkAdd);
        }

        public virtual void Delete()
        {
            Context.CommandContext.IdentityLinkManager.Delete(this);
            FireHistoricIdentityLinkEvent(HistoryEventTypes.IdentityLinkDelete);
        }
        
        
        
        

        public virtual void FireHistoricIdentityLinkEvent(HistoryEventTypes eventType)
        {
            var processEngineConfiguration = Context.ProcessEngineConfiguration;

            var historyLevel = processEngineConfiguration.HistoryLevel;
            if (historyLevel.IsHistoryEventProduced(eventType, this))
                HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClassHelper(this,
                    eventType));
        }

        public override string ToString()
        {
            return GetType().Name + "[id=" + Id + ", type=" + Type + ", userId=" + _userId + ", groupId=" + _groupId +
                   ", taskId=" + TaskId + ", processDefId=" + ProcessDefId + ", task=" + TaskId + ", processDef=" +
                   ProcessDef + ", tenantId=" + TenantId + "]";
        }

        private class HistoryEventCreatorAnonymousInnerClassHelper : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly IdentityLinkEntity _outerInstance;

            private readonly HistoryEventTypes _eventType;

            public HistoryEventCreatorAnonymousInnerClassHelper(IdentityLinkEntity outerInstance,
                HistoryEventTypes eventType)
            {
                _outerInstance = outerInstance;
                _eventType = eventType;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                HistoryEvent @event = null;
                if (HistoryEventTypes.IdentityLinkAdd.Equals(_eventType))
                    @event = producer.CreateHistoricIdentityLinkAddEvent(_outerInstance);
                else if (HistoryEventTypes.IdentityLinkDelete.Equals(_eventType))
                    @event = producer.CreateHistoricIdentityLinkDeleteEvent(_outerInstance);
                return @event;
            }
        }
    }
}