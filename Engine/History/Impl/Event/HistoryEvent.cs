using System;
using ESS.FW.Bpm.Engine.Impl.DB;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     <para>The base class for all history events.</para>
    ///     <para>
    ///         A history event contains data about an event that has happened
    ///         in a process instance. Such an event may be the start of an activity,
    ///         the end of an activity, a Task instance that is created or other similar
    ///         events...
    ///     </para>
    ///     <para>
    ///         History events contain data in a serializable form. Some
    ///         implementations may persist events directly or may serialize
    ///         them as an intermediate representation for later processing
    ///         (ie. in an asynchronous implementation).
    ///     </para>
    ///     <para>
    ///         This class implements <seealso cref="IDbEntity" />. This was chosen so
    ///         that <seealso cref="HistoryEvent" />s can be easily persisted using the
    ///         <seealso cref="DbEntityManager" />. This may not be used by all <seealso cref="HistoryEventHandler" />
    ///         implementations but it does also not cause harm.
    ///     </para>
    /// </summary>
    [Serializable]
    public class HistoryEvent : IDbEntity, IHistoryEvent
    {
        private const long SerialVersionUid = 1L;
        #region 注释
        ///// <summary>
        /////     the id of the case definition
        ///// </summary>
        //protected internal string caseDefinitionId;

        ///// <summary>
        /////     the key of the case definition
        ///// </summary>
        //protected internal string caseDefinitionKey;

        ///// <summary>
        /////     the name of the case definition
        ///// </summary>
        //protected internal string caseDefinitionName;

        ///// <summary>
        /////     the id of the case execution in which the event has happened
        ///// </summary>
        //protected internal string caseExecutionId;

        ///// <summary>
        /////     the case instance in which the event has happened
        ///// </summary>
        //protected internal string caseInstanceId;

        ///// <summary>
        /////     The type of the activity audit event.
        ///// </summary>
        ///// <seealso cref= HistoryEventType# getEventName
        ///// (
        ///// )
        ///// </seealso>
        //protected internal string eventType;

        ///// <summary>
        /////     the id of the execution in which the event has happened
        ///// </summary>
        //protected internal string executionId;

        //// constants deprecated since 7.2

        ////[Obsolete]
        ////public static readonly string ACTIVITY_EVENT_TYPE_START = HistoryEventTypes.ACTIVITY_INSTANCE_START;
        ////[Obsolete]
        ////public static readonly string ACTIVITY_EVENT_TYPE_UPDATE = HistoryEventTypes.ACTIVITY_INSTANCE_END;
        ////[Obsolete]
        ////public static readonly string ACTIVITY_EVENT_TYPE_END = HistoryEventTypes.ACTIVITY_INSTANCE_END;

        ////[Obsolete]
        ////public static readonly string TASK_EVENT_TYPE_CREATE = HistoryEventTypes.TASK_INSTANCE_CREATE;
        ////[Obsolete]
        ////public static readonly string TASK_EVENT_TYPE_UPDATE = HistoryEventTypes.TASK_INSTANCE_UPDATE;
        ////[Obsolete]
        ////public static readonly string TASK_EVENT_TYPE_COMPLETE = HistoryEventTypes.TASK_INSTANCE_COMPLETE;
        ////[Obsolete]
        ////public static readonly string TASK_EVENT_TYPE_DELETE = HistoryEventTypes.TASK_INSTANCE_DELETE;

        ////[Obsolete]
        ////public static readonly string VARIABLE_EVENT_TYPE_CREATE = HistoryEventTypes.VariableInstanceCreate;
        ////[Obsolete]
        ////public static readonly string VARIABLE_EVENT_TYPE_UPDATE = HistoryEventTypes.VariableInstanceUpdate;
        ////[Obsolete]
        ////public static readonly string VARIABLE_EVENT_TYPE_DELETE = HistoryEventTypes.VariableInstanceDelete;

        ////[Obsolete]
        ////public static readonly string FORM_PROPERTY_UPDATE = HistoryEventTypes.FORM_PROPERTY_UPDATE;

        ////[Obsolete]
        ////public static readonly string INCIDENT_CREATE = HistoryEventTypes.INCIDENT_CREATE;
        ////[Obsolete]
        ////public static readonly string INCIDENT_DELETE = HistoryEventTypes.INCIDENT_DELETE;
        ////[Obsolete]
        ////public static readonly string INCIDENT_RESOLVE = HistoryEventTypes.INCIDENT_RESOLVE;

        ////public static readonly string IDENTITY_LINK_ADD = HistoryEventTypes.IDENTITY_LINK_ADD;

        ////public static readonly string IDENTITY_LINK_DELETE = HistoryEventTypes.IDENTITY_LINK_DELETE;

        ///// <summary>
        /////     each <seealso cref="HistoryEvent" /> has a unique id
        ///// </summary>
        //protected internal string id;

        ///// <summary>
        /////     the id of the process definition
        ///// </summary>
        //protected internal string processDefinitionId;

        ///// <summary>
        /////     the key of the process definition
        ///// </summary>
        //protected internal string processDefinitionKey;

        ///// <summary>
        /////     the name of the process definition
        ///// </summary>
        //protected internal string processDefinitionName;

        ///// <summary>
        /////     the version of the process definition
        ///// </summary>
        //protected internal int? processDefinitionVersion;

        ///// <summary>
        /////     the process instance in which the event has happened
        ///// </summary>
        //protected internal string processInstanceId;

        //protected internal long sequenceCounter;
        #endregion
        // getters / setters ///////////////////////////////////
        //[Column("PROC_INST_ID")]
        public virtual string ProcessInstanceId { get; set; }


        public virtual string ExecutionId { get; set; }

        [Column("PROC_DEF_ID")]
        public virtual string ProcessDefinitionId { get; set; }

        [Column("PROC_DEF_KEY")]
        public virtual string ProcessDefinitionKey { get; set; }
        public virtual string ProcessDefinitionName { get; set; }
        public virtual int? ProcessDefinitionVersion { get; set; }
        public virtual string CaseDefinitionName { get; set; }
        public virtual string CaseDefinitionKey { get; set; }
        public virtual string CaseDefinitionId { get; set; }
        public virtual string CaseInstanceId { get; set; }
        public virtual string CaseExecutionId { get; set; }
        [NotMapped]
        public virtual string EventType { get; set; }


        public virtual long SequenceCounter { get; set; }

        [Key]
        public virtual string Id { get; set; }


        // persistent object implementation ///////////////

        public virtual object GetPersistentState()
        {
            // events are immutable
            return typeof(HistoryEvent);
        }

        // state inspection

        public virtual bool IsEventOfType(HistoryEventTypes type)
        {
            return type.EventName == EventType;
        }

        public override string ToString()
        {
            return GetType().Name + "[id=" + Id + ", eventType=" + EventType + ", executionId=" + ExecutionId +
                   ", processDefinitionId=" + ProcessDefinitionId + ", processInstanceId=" + ProcessInstanceId + "]";
        }
    }
}