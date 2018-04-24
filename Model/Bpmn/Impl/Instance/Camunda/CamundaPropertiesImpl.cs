using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaPropertiesImpl : BpmnModelElementInstanceImpl, ICamundaProperties
    {

        protected internal static IChildElementCollection/*<ICamundaProperty>*/ CamundaPropertyCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaProperties>(/*typeof(ICamundaProperties),*/ BpmnModelConstants.CamundaElementProperties)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            CamundaPropertyCollection = sequenceBuilder.ElementCollection<ICamundaProperty>(/*typeof(ICamundaProperty)*/).Build/*<ICamundaProperty>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaProperties>
        {
            public virtual ICamundaProperties NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaPropertiesImpl(instanceContext);
            }
        }

        public CamundaPropertiesImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<ICamundaProperty> CamundaProperties => CamundaPropertyCollection.Get<ICamundaProperty>(this);
    }

}