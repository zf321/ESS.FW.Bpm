using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;


namespace ESS.FW.Bpm.Model.Xml.impl
{
    public class ModelBuilderImpl : ModelBuilder
    {
        private readonly IList<ModelElementTypeBuilderImpl> _typeBuilders = new List<ModelElementTypeBuilderImpl>();
        private readonly ModelImpl _model;

        public ModelBuilderImpl(string modelName)
        {
            _model = new ModelImpl(modelName);
        }

        public override ModelBuilder AlternativeNamespace(string alternativeNs, string actualNs)
        {
            _model.DeclareAlternativeNamespace(alternativeNs, actualNs);
            return this;
        }
        
        public override IModelElementTypeBuilder DefineType<T>(string typeName)
        {
            ModelElementTypeBuilderImpl typeBuilder = new ModelElementTypeBuilderImpl(typeof(T), typeName, _model);
            _typeBuilders.Add(typeBuilder);
            return typeBuilder;
        }

        public override IModelElementType DefineGenericType<T>(string typeName, string typeNamespaceUri)
        {
            IModelElementTypeBuilder typeBuilder = DefineType<T>(typeName)
                .NamespaceUri(typeNamespaceUri)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass(this));

            return typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IModelElementInstance>
        {
            private readonly ModelBuilderImpl _outerInstance;

            public ModelTypeInstanceProviderAnonymousInnerClass(ModelBuilderImpl outerInstance)
            {
                this._outerInstance = outerInstance;
            }

            public virtual IModelElementInstance NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ModelElementInstanceImpl(instanceContext);
            }
        }
        /// <summary>
        /// 建造者
        /// </summary>
        /// <returns></returns>
        public override IModel Build()
        {
            foreach (ModelElementTypeBuilderImpl typeBuilder in _typeBuilders)
            {
                typeBuilder.BuildTypeHierarchy(_model);
            }
            foreach (ModelElementTypeBuilderImpl typeBuilder in _typeBuilders)
            {
                typeBuilder.PerformModelBuild(_model);
            }
            return _model;
        }
    }
}