

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class DataPath : FormalExpressionImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<DataPath>(/*typeof(DataPath),*/ BpmnModelConstants.BpmnElementDataPath)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IFormalExpression))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<DataPath>
        {
            public virtual DataPath NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DataPath(instanceContext);
            }
        }

        public DataPath(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}