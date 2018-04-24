//using System;
//using ESS.FW.Bpm.Model.Bpmn.instance;



//namespace ESS.FW.Bpm.Model.Bpmn.builder
//{
//    /// <summary>
//	/// 
//	/// </summary>
//	public abstract class AbstractProcessBuilder : AbstractCallableElementBuilder<IProcess>, IProcessBuilder
//    {

//        protected internal AbstractProcessBuilder(IBpmnModelInstance modelInstance, IProcess element) : base(modelInstance, element)
//        {
//        }

//        /// <summary>
//        /// Sets the process type for this.
//        /// </summary>
//        /// <param name="processType">  the process type to set </param>
//        /// <returns> the builder object </returns>
//        public virtual IProcessBuilder ProcessType(ProcessType processType)
//        {
//            element.ProcessType = processType;
//            return this;
//        }

//        /// <summary>
//        /// Sets this closed.
//        /// </summary>
//        /// <returns> the builder object </returns>
//        public virtual IProcessBuilder Closed()
//        {
//            element.Closed = true;
//            return this;
//        }

//        /// <summary>
//        /// Sets this executable.
//        /// </summary>
//        /// <returns> the builder object </returns>
//        public virtual IProcessBuilder Executable()
//        {
//            element.Executable = true;
//            return this;
//        }

//        public virtual IProcessBuilder CamundaJobPriority(string jobPriority)
//        {
//            element.CamundaJobPriority = jobPriority;
//            return this;
//        }

//        /// <summary>
//        /// Set the camunda task priority attribute.
//        /// The priority is only used for service tasks which have as type value
//        /// <code>external</code>
//        /// </summary>
//        /// <param name="taskPriority"> the task priority which should used for the external tasks </param>
//        /// <returns> the builder object </returns>
//        public virtual IProcessBuilder CamundaTaskPriority(string taskPriority)
//        {
//            element.CamundaTaskPriority = taskPriority;
//            return this;
//        }

//    }

//}