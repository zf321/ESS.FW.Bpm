

using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class DataStateImpl : BaseElementImpl, IDataState
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDataState>(/*typeof(IDataState),*/ BpmnModelConstants.BpmnElementDataState)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IBaseElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDataState>
        {
            public virtual IDataState NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DataStateImpl(instanceContext);
            }
        }

        public DataStateImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get
            {
                return NameAttribute.GetValue<String>(this);
            }

            set
            {
                NameAttribute.SetValue(this, value);
            }
        }

    }

}