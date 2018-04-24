

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN11_NS;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN_ELEMENT_INPUT_VALUES;

    using ModelBuilder = ModelBuilder;
    using ModelTypeInstanceContext = ModelTypeInstanceContext;

    public class InputValuesImpl : UnaryTestsImpl, INputValues
    {

        public InputValuesImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<INputValues>(/*typeof(INputValues), */DmnModelConstants.DmnElementInputValues).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IUnaryTests)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<INputValues>
        {
            public ModelTypeInstanceProviderAnonymousInnerClass()
            {
            }

            public virtual INputValues NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new InputValuesImpl(instanceContext);
            }
        }
    }
}