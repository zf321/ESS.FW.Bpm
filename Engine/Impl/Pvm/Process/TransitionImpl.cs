using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Core.Model;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Process
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class TransitionImpl : CoreModelElement, IPvmTransition
    {
        //protected internal ActivityImpl Destination;

        protected internal ProcessDefinitionImpl processDefinition;





        public TransitionImpl(string id, ProcessDefinitionImpl processDefinition) : base(id)
        {
            this.processDefinition = processDefinition;
        }

        public virtual IPvmActivity Source { get; set; }
        public IPvmActivity Destination { get; set; }
        [Obsolete]
        public virtual IList<IDelegateListener<IBaseDelegateExecution>> ExecutionListeners
        {
            get {
               return GetListeners(ExecutionListenerFields.EventNameTake);
            }
            set
            {
                foreach (var executionListener in value)
                    AddExecutionListener(executionListener);
            }
        }
        /// <summary>
        ///     Graphical information: a list of waypoints: x1, y1, x2, y2, x3, y3, ..
        /// </summary>
        public virtual IList<int> Waypoints { get; set; } = new List<int>();

        // getters and setters //////////////////////////////////////////////////////

        public virtual IPvmProcessDefinition ProcessDefinition { get { return processDefinition; } }

        //IPvmActivity IPvmTransition.Source => source;

        //public IPvmActivity Destination => destination;

        public virtual void SetDestination(ActivityImpl destination)
        {
            this.Destination = destination;
            destination.IncomingTransitions.Add(this);
        }

        [Obsolete]
        public virtual void AddExecutionListener(IDelegateListener<IBaseDelegateExecution> executionListener)
        {
            AddListener(ExecutionListenerFields.EventNameTake, executionListener);
        }


        public override string ToString()
        {
            return "(" + Source.Id + ")--" + (!ReferenceEquals(id, null) ? id + "-->(" : ">(") + Destination.Id + ")";
        }


        public virtual IPvmActivity GetDestination()
        {
            return Destination;
        }
    }
}