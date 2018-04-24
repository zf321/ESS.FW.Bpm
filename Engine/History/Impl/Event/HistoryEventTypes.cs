namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     The set of built-in history event types.
    ///     枚举改为class
    ///     
    ///     @since 7.2
    /// </summary>
    public class HistoryEventTypes : IHistoryEventType
    {
        protected string entityType;
        protected string eventName;
        public HistoryEventTypes(string entityType, string eventName)
        {
            this.entityType = entityType;
            this.eventName = eventName;
        }
        /// <summary>
        /// entityType:eventName
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}", this.EntityType, this.EventName);
        }
        public override int GetHashCode()
        {
            return this.EntityType.GetHashCode() ^ this.EventName.GetHashCode();
        }
        public string EntityType
        {
            get
            {
                return entityType;
            }
        }
        public string EventName
        {
            get
            {
                return eventName;
            }
        }
        /// <summary>
        ///     fired when a process instance is started.
        /// </summary>
        public static HistoryEventTypes ProcessInstanceStart = new HistoryEventTypes("process-instance", "start"); //("process-instance", "start"),

        /// <summary>
        ///     fired when a process instance is updated
        /// </summary>
        public static HistoryEventTypes ProcessInstanceUpdate = new HistoryEventTypes("process-instance-update", "update"); //("process-instance-update", "update"),

        /// <summary>
        ///     fired when a process instance is migrated
        /// </summary>
        public static HistoryEventTypes ProcessInstanceMigrate = new HistoryEventTypes("process-instance", "migrate");

        /// <summary>
        ///     fired when a process instance is ended.
        /// </summary>
        public static HistoryEventTypes ProcessInstanceEnd = new HistoryEventTypes("process-instance", "end");

        /// <summary>
        ///     fired when an activity instance is started.
        /// </summary>
        public static HistoryEventTypes ActivityInstanceStart = new HistoryEventTypes("activity-instance", "start");

        /// <summary>
        ///     fired when an activity instance is updated.
        /// </summary>
        public static HistoryEventTypes ActivityInstanceUpdate = new HistoryEventTypes("activity-instance", "update");

        /// <summary>
        ///     fired when an activity instance is migrated.
        /// </summary>
        public static HistoryEventTypes ActivityInstanceMigrate = new HistoryEventTypes("activity-instance", "migrate");

        /// <summary>
        ///     fired when an activity instance is ended.
        /// </summary>
        public static HistoryEventTypes ActivityInstanceEnd = new HistoryEventTypes("activity-instance", "end");

        /// <summary>
        ///     fired when a ITask instance is created.
        /// </summary>
        public static HistoryEventTypes TaskInstanceCreate = new HistoryEventTypes("ITask-instance", "create");

        /// <summary>
        ///     fired when a ITask instance is updated.
        /// </summary>
        public static HistoryEventTypes TaskInstanceUpdate = new HistoryEventTypes("ITask-instance", "update");

        /// <summary>
        ///     fired when a ITask instance is migrated.
        /// </summary>
        public static HistoryEventTypes TaskInstanceMigrate = new HistoryEventTypes("ITask-instance", "migrate");

        /// <summary>
        ///     fired when a ITask instance is completed.
        /// </summary>
        public static HistoryEventTypes TaskInstanceComplete = new HistoryEventTypes("ITask-instance", "complete");

        /// <summary>
        ///     fired when a ITask instance is deleted.
        /// </summary>
        public static HistoryEventTypes TaskInstanceDelete = new HistoryEventTypes("ITask-instance", "delete");

        /// <summary>
        ///     fired when a variable instance is created.
        /// </summary>
        public static HistoryEventTypes VariableInstanceCreate = new HistoryEventTypes("variable-instance", "create");

        /// <summary>
        ///     fired when a variable instance is updated.
        /// </summary>
        public static HistoryEventTypes VariableInstanceUpdate = new HistoryEventTypes("variable-instance", "update");

        /// <summary>
        ///     fired when a variable instance is migrated.
        /// </summary>
        public static HistoryEventTypes VariableInstanceMigrate = new HistoryEventTypes("variable-instance", "migrate");

        /// <summary>
        ///     fired when a variable instance is updated.
        /// </summary>
        public static HistoryEventTypes VariableInstanceUpdateDetail = new HistoryEventTypes("variable-instance", "update-detail");

        /// <summary>
        ///     fired when a variable instance is deleted.
        /// </summary>
        public static HistoryEventTypes VariableInstanceDelete = new HistoryEventTypes("variable-instance", "delete");

        /// <summary>
        ///     fired when a form property is updated.
        /// </summary>
        public static HistoryEventTypes FormPropertyUpdate = new HistoryEventTypes("form-property", "form-property-update");

        /// <summary>
        ///     fired when an incident is created.
        /// </summary>
        public static HistoryEventTypes IncidentCreate = new HistoryEventTypes("incident", "create");

        /// <summary>
        ///     fired when an incident is migrated.
        /// </summary>
        public static HistoryEventTypes IncidentMigrate = new HistoryEventTypes("incident", "migrate");

        /// <summary>
        ///     fired when an incident is deleted.
        /// </summary>
        public static HistoryEventTypes IncidentDelete = new HistoryEventTypes("incident", "delete");

        /// <summary>
        ///     fired when an incident is resolved.
        /// </summary>
        public static HistoryEventTypes IncidentResolve = new HistoryEventTypes("incident", "resolve");

        /// <summary>
        ///     fired when a case instance is created.
        /// </summary>
        public static HistoryEventTypes CaseInstanceCreate = new HistoryEventTypes("case-instance", "create");

        /// <summary>
        ///     fired when a case instance is updated.
        /// </summary>
        public static HistoryEventTypes CaseInstanceUpdate = new HistoryEventTypes("case-instance", "update");

        /// <summary>
        ///     fired when a case instance is closed.
        /// </summary>
        public static HistoryEventTypes CaseInstanceClose = new HistoryEventTypes("case-instance", "close");

        /// <summary>
        ///     fired when a case activity instance is created.
        /// </summary>
        public static HistoryEventTypes CaseActivityInstanceCreate = new HistoryEventTypes("case-activity-instance", "create");

        /// <summary>
        ///     fired when a case activity instance is updated.
        /// </summary>
        public static HistoryEventTypes CaseActivityInstanceUpdate = new HistoryEventTypes("case-activity-instance", "update");

        /// <summary>
        ///     fired when a case instance is ended.
        /// </summary>
        public static HistoryEventTypes CaseActivityInstanceEnd = new HistoryEventTypes("case-activity_instance", "end");

        /// <summary>
        ///     fired when a job is created.
        ///     
        /// </summary>
        public static HistoryEventTypes JobCreate = new HistoryEventTypes("job", "create");

        /// <summary>
        ///     fired when a job is failed.
        ///     
        /// </summary>
        public static HistoryEventTypes JobFail = new HistoryEventTypes("job", "fail");

        /// <summary>
        ///     fired when a job is succeeded.
        ///     
        /// </summary>
        public static HistoryEventTypes JobSuccess = new HistoryEventTypes("job", "success");

        /// <summary>
        ///     fired when a job is deleted.
        ///     
        /// </summary>
        public static HistoryEventTypes JobDelete = new HistoryEventTypes("job", "delete");

        /// <summary>
        ///     fired when a decision is evaluated.
        ///     
        /// </summary>
        public static HistoryEventTypes DmnDecisionEvaluate = new HistoryEventTypes("decision", "evaluate");

        /// <summary>
        ///     fired when a batch was started.
        ///     
        /// </summary>
        public static HistoryEventTypes BatchStart = new HistoryEventTypes("batch", "start");

        /// <summary>
        ///     fired when a batch was completed.
        ///     
        /// </summary>
        public static HistoryEventTypes BatchEnd = new HistoryEventTypes("batch", "end");

        /// <summary>
        ///     fired when an identity link is added
        ///     
        /// </summary>
        public static HistoryEventTypes IdentityLinkAdd = new HistoryEventTypes("identity-link-add", "add-identity-link");

        /// <summary>
        ///     fired when an identity link is removed
        ///     
        /// </summary>
        public static HistoryEventTypes IdentityLinkDelete = new HistoryEventTypes("identity-link-delete", "delete-identity-link");

        /// <summary>
        ///     fired when an external ITask is created.
        ///     
        /// </summary>
        public static HistoryEventTypes ExternalTaskCreate = new HistoryEventTypes("external-ITask", "create");

        /// <summary>
        ///     fired when an external ITask has failed.
        ///     
        /// </summary>
        public static HistoryEventTypes ExternalTaskFail = new HistoryEventTypes("external-ITask", "fail");

        /// <summary>
        ///     fired when an external ITask has succeeded.
        ///     
        /// </summary>
        public static HistoryEventTypes ExternalTaskSuccess = new HistoryEventTypes("external-ITask", "success");

        /// <summary>
        ///     fired when an external ITask is deleted.
        ///     
        /// </summary>
        public static HistoryEventTypes ExternalTaskDelete = new HistoryEventTypes("external-ITask", "delete");
    }
}