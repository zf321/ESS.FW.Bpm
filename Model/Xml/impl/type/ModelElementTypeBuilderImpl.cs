using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.type.attribute;
using ESS.FW.Bpm.Model.Xml.impl.type.child;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;


namespace ESS.FW.Bpm.Model.Xml.impl.type
{
    public class ModelElementTypeBuilderImpl : IModelElementTypeBuilder, IModelBuildOperation
    {

        private readonly ModelElementTypeImpl _modelType;
        private readonly ModelImpl _model;
        private readonly Type _instanceType;

        private readonly IList<IModelBuildOperation> _modelBuildOperations = new List<IModelBuildOperation>();
        private Type _extendedType;

        public ModelElementTypeBuilderImpl(Type instanceType, string name, ModelImpl model)
        {
            this._instanceType = instanceType;
            this._model = model;
            _modelType = new ModelElementTypeImpl(model, name, instanceType);
        }

        public virtual IModelElementTypeBuilder ExtendsType(Type extendedType)
        {
            this._extendedType = extendedType;
            return this;
        }

        public IModelElementTypeBuilder ExtendsType<T>(T extendedType) where T : IModelElementInstance
        {
            this._extendedType = extendedType.GetType();
            return this;
        }

        public virtual IModelElementTypeBuilder InstanceProvider<T>(IModelTypeInstanceProvider<T> instanceProvider) where T : IModelElementInstance
        {
            _modelType.InstanceProvider = (IModelTypeInstanceProvider<IModelElementInstance>) instanceProvider;
            return this;
        }

        public virtual IModelElementTypeBuilder NamespaceUri(string namespaceUri)
        {
            _modelType.TypeNamespace = namespaceUri;
            return this;
        }

        public virtual IAttributeBuilder<Boolean> BooleanAttribute(string attributeName)
        {
            IAttributeBuilder<Boolean> builder = new BooleanAttributeBuilder(attributeName, _modelType);
            _modelBuildOperations.Add(builder as IModelBuildOperation);
            return builder;
        }

        public virtual IStringAttributeBuilder StringAttribute(string attributeName)
        {
            StringAttributeBuilderImpl builder = new StringAttributeBuilderImpl(attributeName, _modelType);
            _modelBuildOperations.Add(builder);
            return builder;
        }

        public virtual IAttributeBuilder<Int32?> IntegerAttribute(string attributeName)
        {
            IAttributeBuilder<Int32?> builder = new IntegerAttributeBuilder(attributeName, _modelType);
            _modelBuildOperations.Add(builder as IModelBuildOperation);
            return builder;
        }

        public virtual IAttributeBuilder<Double?> DoubleAttribute(string attributeName)
        {
            IAttributeBuilder<Double?> builder = new DoubleAttributeBuilder(attributeName, _modelType);
            _modelBuildOperations.Add(builder as IModelBuildOperation);
            return builder;
        }

        public virtual IAttributeBuilder<TV> EnumAttribute<TV>(string attributeName) where TV : struct
        {
            IAttributeBuilder<TV> builder = new EnumAttributeBuilder<TV>(attributeName, _modelType);
            _modelBuildOperations.Add(builder as IModelBuildOperation);
            return builder;
        }

        public virtual IAttributeBuilder<TV> NamedEnumAttribute<TV>(string attributeName) where TV : struct
        {
            IAttributeBuilder<TV> builder = new NamedEnumAttributeBuilder<TV>(attributeName, _modelType);
            _modelBuildOperations.Add(builder as IModelBuildOperation);
            return builder;
        }

        public virtual IModelElementType Build()
        {
            _model.RegisterType(_modelType, _instanceType);
            return _modelType;
        }

        public virtual IModelElementTypeBuilder AbstractType()
        {
            _modelType.Abstract = true;
            return this;
        }

        public virtual ISequenceBuilder Sequence()
        {
            SequenceBuilderImpl builder = new SequenceBuilderImpl(_modelType);
            _modelBuildOperations.Add(builder);
            return builder;
        }
        /// <summary>
        /// 实际就是添加IModelElementType
        /// </summary>
        /// <param name="model"></param>
        public virtual void BuildTypeHierarchy(IModel model)
        {

            // build type hierarchy
            if (_extendedType != null)
            {
                ModelElementTypeImpl extendedModelElementType = (ModelElementTypeImpl)model.GetType(_extendedType);
                if (extendedModelElementType == null)
                {
                    throw new ModelException("Type " + _modelType + " is defined to extend " + _extendedType + " but no such type is defined.");

                }
                else
                {
                    _modelType.SetBaseType(extendedModelElementType);
                    extendedModelElementType.RegisterExtendingType(_modelType);
                }
            }
        }

        public virtual void PerformModelBuild(IModel model)
        {
            foreach (IModelBuildOperation operation in _modelBuildOperations)
            {
                operation.PerformModelBuild(model);
            }
        }

    }
    
}