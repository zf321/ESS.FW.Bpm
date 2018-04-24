using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;

namespace Engine.Tests.Bpmn.MultiInstance
{
    /// <summary>
    /// </summary>
    public class DelegateEvent : IDelegateExecution
    {
        protected internal static readonly IList<DelegateEvent> RECORDED_EVENTS = new List<DelegateEvent>();

        protected internal string currentActivityId;
        protected internal string currentActivityName;
        protected internal string currentTransitionId;
        protected internal string parentActivityInstanceId;
        protected internal string parentId;

        protected internal string processDefinitionId;

        //protected internal string ProcessInstanceId;
        protected internal string tenantId;

        public static IList<DelegateEvent> Events => RECORDED_EVENTS;


        public IBpmnModelInstance IBpmnModelInstance
        {
            get { throw NotYetImplemented(); }
        }

        public IDelegateExecution IProcessInstance { get { throw NotYetImplemented(); } }

        public string Id { get; set; }

        public string EventName { get; set; }

        public string BusinessKey { get; set; }

        public string VariableScopeKey { get; set; }

        public IDictionary<string, object> Variables
        {
            get { throw NotYetImplemented(); }
            set { throw NotYetImplemented(); }
        }

        public IVariableMap VariablesTyped
        {
            get { throw NotYetImplemented(); }
        }

        public IVariableMap GetVariablesTyped(bool deserializeValues)
        {
            throw NotYetImplemented();
        }

        public IDictionary<string, object> VariablesLocal
        {
            get { throw NotYetImplemented(); }
            set { throw CannotModifyState(); }
        }

        public IVariableMap VariablesLocalTyped
        {
            get { throw NotYetImplemented(); }
        }

        public IVariableMap GetVariablesLocalTyped(bool deserializeValues)
        {
            throw NotYetImplemented();
        }

        public object GetVariable(string VariableName)
        {
            throw NotYetImplemented();
        }

        public object GetVariableLocal(string VariableName)
        {
            throw NotYetImplemented();
        }


        public ISet<string> VariableNames
        {
            get
            {
                throw NotYetImplemented();
            }
        }

        public ISet<string> VariableNamesLocal { get { throw NotYetImplemented(); } }

        public void SetVariable(string VariableName, object value)
        {
            throw CannotModifyState();
        }

        public IFlowElement BpmnModelElementInstance { get { throw NotYetImplemented(); } }

        public IProcessEngineServices ProcessEngineServices { get { throw NotYetImplemented(); } }

        public virtual string ActivityInstanceId { get; set; }

        public virtual string CurrentActivityId => currentActivityId;

        public virtual string CurrentActivityName => currentActivityName;

        public virtual string CurrentTransitionId => currentTransitionId;

        public virtual string ParentActivityInstanceId => parentActivityInstanceId;

        public virtual string ParentId => parentId;

        public virtual string ProcessBusinessKey { get; set; }

        public virtual string ProcessDefinitionId
        {
            get { return processDefinitionId; }
            set { processDefinitionId = value; }
        }


        public virtual string ProcessInstanceId { get; set; }

        public virtual string TenantId
        {
            get { return tenantId; }
            set { tenantId = value; }
        }

        public void SetVariable(string VariableName, object value, string activityId)
        {
            CannotModifyState();
        }

        public T GetVariableTyped<T>(string variableName)
        {
            throw new NotImplementedException();
        }

        public T GetVariableTyped<T>(string variableName, bool deserializeValue)
        {
            throw new NotImplementedException();
        }

        public T GetVariableLocalTyped<T>(string variableName)
        {
            throw new NotImplementedException();
        }

        public T GetVariableLocalTyped<T>(string variableName, bool deserializeValue)
        {
            throw new NotImplementedException();
        }

        public void SetVariableLocal(string variableName, object value)
        {
            throw new NotImplementedException();
        }

        public bool HasVariables()
        {
            throw new NotImplementedException();
        }

        public bool HasVariablesLocal()
        {
            throw new NotImplementedException();
        }

        public bool HasVariable(string variableName)
        {
            throw new NotImplementedException();
        }

        public bool HasVariableLocal(string variableName)
        {
            throw new NotImplementedException();
        }

        public void RemoveVariable(string variableName)
        {
            throw new NotImplementedException();
        }

        public void RemoveVariableLocal(string variableName)
        {
            throw new NotImplementedException();
        }

        public void RemoveVariables(ICollection<string> variableNames)
        {
            throw new NotImplementedException();
        }

        public void RemoveVariablesLocal(ICollection<string> variableNames)
        {
            throw new NotImplementedException();
        }

        public void RemoveVariables()
        {
            throw new NotImplementedException();
        }

        public void RemoveVariablesLocal()
        {
            throw new NotImplementedException();
        }

        public IDelegateExecution SuperExecution { get { throw NotYetImplemented(); } }

        public bool Canceled { get { throw NotYetImplemented(); } }

        public IDelegateExecution ProcessInstance
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IBpmnModelInstance BpmnModelInstance {
            get { throw new NotImplementedException();}
        }

        public static DelegateEvent FromExecution(IDelegateExecution delegateExecution)
        {
            var @event = new DelegateEvent();

            @event.ActivityInstanceId = delegateExecution.ActivityInstanceId;
            @event.BusinessKey = delegateExecution.BusinessKey;
            @event.currentActivityId = delegateExecution.CurrentActivityId;
            @event.currentActivityName = delegateExecution.CurrentActivityName;
            @event.currentTransitionId = delegateExecution.CurrentTransitionId;
            @event.EventName = delegateExecution.EventName;
            @event.Id = delegateExecution.Id;
            @event.parentActivityInstanceId = delegateExecution.ParentActivityInstanceId;
            @event.parentId = delegateExecution.ParentId;
            @event.ProcessBusinessKey = delegateExecution.ProcessBusinessKey;
            @event.ProcessDefinitionId = delegateExecution.ProcessDefinitionId;
            @event.ProcessInstanceId = delegateExecution.ProcessInstanceId;
            @event.TenantId = delegateExecution.TenantId;
            @event.VariableScopeKey = delegateExecution.VariableScopeKey;

            return @event;
        }

        public static void ClearEvents()
        {
            RECORDED_EVENTS.Clear();
        }

        public static void RecordEventFor(IDelegateExecution execution)
        {
            RECORDED_EVENTS.Add(FromExecution(execution));
        }

        protected internal virtual System.Exception NotYetImplemented()
        {
            return new System.Exception("Recording this method is not implemented");
        }

        protected internal virtual System.Exception CannotModifyState()
        {
            return new System.Exception("This event is read-only; cannot Modify state");
        }

        public IIncident CreateIncident(string incidentType, string configuration)
        {
            throw NotYetImplemented();
        }

        public void ResolveIncident(string incidentId)
        {
            throw NotYetImplemented();
        }

        public IIncident CreateIncident(string incidentType, string configuration, string message)
        {
            throw NotYetImplemented();
        }

    }
}