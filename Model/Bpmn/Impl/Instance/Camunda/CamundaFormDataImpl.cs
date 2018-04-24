using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaFormDataImpl : BpmnModelElementInstanceImpl, ICamundaFormData
    {

        protected internal static IChildElementCollection/*<ICamundaFormField>*/ CamundaFormFieldCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaFormData>(/*typeof(ICamundaFormData),*/ BpmnModelConstants.CamundaElementFormData)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            CamundaFormFieldCollection = sequenceBuilder.ElementCollection<ICamundaFormField>(/*typeof(ICamundaFormField)*/).Build/*<ICamundaFormField>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaFormData>
        {
            public virtual ICamundaFormData NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaFormDataImpl(instanceContext);
            }
        }

        public CamundaFormDataImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<ICamundaFormField> CamundaFormFields => CamundaFormFieldCollection.Get<ICamundaFormField>(this);
    }

}