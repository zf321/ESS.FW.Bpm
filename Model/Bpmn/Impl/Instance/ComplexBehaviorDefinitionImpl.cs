using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class ComplexBehaviorDefinitionImpl : BaseElementImpl, IComplexBehaviorDefinition
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IComplexBehaviorDefinition>(/*typeof(IComplexBehaviorDefinition),*/ BpmnModelConstants.BpmnElementComplexBehaviorDefinition)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IComplexBehaviorDefinition>
        {
            public virtual IComplexBehaviorDefinition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ComplexBehaviorDefinitionImpl(instanceContext);
            }
        }

        public ComplexBehaviorDefinitionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

    }

}