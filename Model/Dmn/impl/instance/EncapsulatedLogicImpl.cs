﻿

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN11_NS;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN_ELEMENT_ENCAPSULATED_LOGIC;

    using ModelBuilder = ModelBuilder;
    using ModelTypeInstanceContext = ModelTypeInstanceContext;

    public class EncapsulatedLogicImpl : FunctionDefinitionImpl, IEncapsulatedLogic
    {

        public EncapsulatedLogicImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IEncapsulatedLogic>(/*typeof(IEncapsulatedLogic),*/ DmnModelConstants.DmnElementEncapsulatedLogic).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IFunctionDefinition)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IEncapsulatedLogic>
        {
            public ModelTypeInstanceProviderAnonymousInnerClass()
            {
            }

            public virtual IEncapsulatedLogic NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new EncapsulatedLogicImpl(instanceContext);
            }
        }
    }
}