using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;


namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaInputOutputImpl : BpmnModelElementInstanceImpl, ICamundaInputOutput
    {

        protected internal static IChildElementCollection/*<ICamundaInputParameter>*/ CamundaInputParameterCollection;
        protected internal static IChildElementCollection/*<ICamundaOutputParameter>*/ CamundaOutputParameterCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaInputOutput>(/*typeof(ICamundaInputOutput),*/ BpmnModelConstants.CamundaElementInputOutput)
                    .NamespaceUri(BpmnModelConstants.CamundaNs)
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            CamundaInputParameterCollection = sequenceBuilder.ElementCollection<ICamundaInputParameter>(/*typeof(ICamundaInputParameter)*/).Build/*<ICamundaInputParameter>*/();

            CamundaOutputParameterCollection = sequenceBuilder.ElementCollection<ICamundaOutputParameter>(/*typeof(ICamundaOutputParameter)*/).Build/*<ICamundaOutputParameter>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaInputOutput>
        {
            public virtual ICamundaInputOutput NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaInputOutputImpl(instanceContext);
            }
        }

        public CamundaInputOutputImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<ICamundaInputParameter> CamundaInputParameters => CamundaInputParameterCollection.Get<ICamundaInputParameter>(this);

        public virtual ICollection<ICamundaOutputParameter> CamundaOutputParameters => CamundaOutputParameterCollection.Get<ICamundaOutputParameter>(this);
    }
}