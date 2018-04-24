

using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    /// <summary>
    /// The BPMN element activationCondition of the BPMN tComplexGateway type
    /// 
    /// 
    /// </summary>
    public class ActivationConditionImpl : ExpressionImpl, IActivationCondition
    {
        public ActivationConditionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IActivationCondition>(/*typeof(IActivationCondition),*/ BpmnModelConstants.BpmnElementActivationCondition)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IExpression))
                .InstanceProvider/*<IModelElementInstance>*/(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }
        
        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IActivationCondition>
        {
            public IActivationCondition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ActivationConditionImpl(instanceContext);
            }
        }
    }
}