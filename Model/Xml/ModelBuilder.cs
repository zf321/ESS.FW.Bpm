using ESS.FW.Bpm.Model.Xml.impl;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Xml
{
	public abstract class ModelBuilder
    {
        public abstract ModelBuilder AlternativeNamespace(string alternativeNs, string actualNs);
        
        public abstract IModelElementTypeBuilder DefineType<T>(string typeName) where T:IModelElementInstance;

        public abstract IModelElementType DefineGenericType<T>(string typeName, string typeNamespaceUri) where T : IModelElementInstance;

        public abstract IModel Build();

        public static ModelBuilder CreateInstance(string modelName)
        {
            return new ModelBuilderImpl(modelName);
        }
    }
}