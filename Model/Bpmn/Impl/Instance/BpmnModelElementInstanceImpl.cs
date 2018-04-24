

using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml.impl.instance;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    /// <summary>
    /// Shared base class for all BPMN Model Elements. Provides implementation
    /// of the <seealso cref="IBpmnModelElementInstance"/> interface.
    /// 
    /// 
    /// </summary>
    public abstract class BpmnModelElementInstanceImpl : ModelElementInstanceImpl, IBpmnModelElementInstance
    {

        public BpmnModelElementInstanceImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual TB Builder<TB, TE>() where TB : IBpmnModelElementBuilder<TE> where TE : IBpmnModelElementInstance
        {
            throw new BpmnModelException("覆写基类实现 No builder implemented for " + this);
        }

        public virtual bool IsScope
        {
            get
            {
                return this is IProcess || this is ISubProcess;
            }
        }

        public virtual IBpmnModelElementInstance Scope
        {
            get
            {
                IBpmnModelElementInstance parentElement = (IBpmnModelElementInstance)ParentElement;
                if (parentElement != null)
                {
                    if (parentElement.IsScope)
                    {
                        return parentElement;
                    }
                    else
                    {
                        return parentElement.Scope;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }
}