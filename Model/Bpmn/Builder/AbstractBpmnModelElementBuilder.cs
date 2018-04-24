using System;
using ESS.FW.Bpm.Model.Bpmn.instance;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// </summary>
	public abstract class AbstractBpmnModelElementBuilder<TE>: IBpmnModelElementBuilder<TE> where TE : IBpmnModelElementInstance
    {
        protected internal readonly IBpmnModelInstance modelInstance;
        protected internal readonly TE element;
        //protected internal readonly TB myself; this

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelInstance"></param>
        /// <param name="element"></param>
        /// <param name="selfType"></param>
        protected internal AbstractBpmnModelElementBuilder(IBpmnModelInstance modelInstance, TE element/*, Type selfType*/)
        {
            this.modelInstance = modelInstance;
            this.element = element;
        }

        /// <summary>
        /// Finishes the process building.
        /// </summary>
        /// <returns> the model instance with the build process </returns>
        public virtual IBpmnModelInstance Done()
        {
            return modelInstance;
        }

        /// <summary>
        /// Finishes the building of an embedded sub-process.
        /// </summary>
        /// <returns> the parent sub-process builder </returns>
        /// <exception cref="BpmnModelException"> if no parent sub-process can be found </exception>
        public virtual SubProcessBuilder SubProcessDone()
        {
            IBpmnModelElementInstance lastSubProcess = element.Scope;
            if (lastSubProcess != null && lastSubProcess is ISubProcess)
            {
                return ((ISubProcess)lastSubProcess).Builder();
            }
            else
            {
                throw new BpmnModelException("Unable to find a parent subProcess.");
            }
        }

        public virtual TransactionBuilder TransactionDone()
        {
            IBpmnModelElementInstance lastTransaction = element.Scope;
            if (lastTransaction != null && lastTransaction is ITransaction)
            {
                return new TransactionBuilder(modelInstance, (ITransaction)lastTransaction);
            }
            else
            {
                throw new BpmnModelException("Unable to find a parent transaction.");
            }
        }
    }

}