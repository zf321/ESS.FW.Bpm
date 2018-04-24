using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
    /// </summary>
    public class ProcessBuilder : AbstractCallableElementBuilder<IProcess>, IProcessBuilder
    {
        public ProcessBuilder(IBpmnModelInstance modelInstance, IProcess process) : base(modelInstance, process)
        {
        }

        protected internal override IBpmnShape Coordinates
        {
            set
            {
                var bounds = value.Bounds;
                bounds.SetX(100);
                bounds.SetY(100);
            }
        }

        public virtual StartEventBuilder StartEvent()
        {
            return StartEvent(null);
        }

        public virtual StartEventBuilder StartEvent(string id)
        {
            var start = CreateChild<IStartEvent>(typeof(IStartEvent), id);
            var bpmnShape = CreateBpmnShape(start);
            Coordinates = bpmnShape;
            return start.Builder<StartEventBuilder,IStartEvent>();
        }
        /// <summary>
        /// Sets the process type for this.
        /// </summary>
        /// <param name="processType">  the process type to set </param>
        /// <returns> the builder object </returns>
        public virtual IProcessBuilder ProcessType(ProcessType processType)
        {
            element.ProcessType = processType;
            return this;
        }

        /// <summary>
        /// Sets this closed.
        /// </summary>
        /// <returns> the builder object </returns>
        public virtual IProcessBuilder Closed()
        {
            element.Closed = true;
            return this;
        }

        /// <summary>
        /// Sets this executable.
        /// </summary>
        /// <returns> the builder object </returns>
        public virtual IProcessBuilder Executable()
        {
            element.Executable = true;
            return this;
        }

        public virtual IProcessBuilder CamundaJobPriority(string jobPriority)
        {
            element.CamundaJobPriority = jobPriority;
            return this;
        }

        /// <summary>
        /// Set the camunda task priority attribute.
        /// The priority is only used for service tasks which have as type value
        /// <code>external</code>
        /// </summary>
        /// <param name="taskPriority"> the task priority which should used for the external tasks </param>
        /// <returns> the builder object </returns>
        public virtual IProcessBuilder CamundaTaskPriority(string taskPriority)
        {
            element.CamundaTaskPriority = taskPriority;
            return this;
        }
    }
}