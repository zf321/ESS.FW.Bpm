using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using System.Linq;


namespace ESS.FW.Bpm.Model.Xml.impl.type
{

    /// <summary>
    /// 
    /// ModelElement元素据的描述，包含childElementType
    /// </summary>
    public class ModelElementTypeImpl : IModelElementType
    {
        #region members

        //父
        private readonly ModelImpl _model;

        private readonly string _typeName;

        private readonly Type _instanceType;

        private string _typeNamespace;

        private ModelElementTypeImpl _baseType;

        private readonly IList<IModelElementType> _extendingTypes = new List<IModelElementType>();
        
        private readonly IList<IAttribute> _attributes = new List<IAttribute>();

        /// <summary>
        /// 子IModelElementType
        /// </summary>
        private readonly IList<IModelElementType> _childElementTypes = new List<IModelElementType>();

        private readonly IList<IChildElementCollection> _childElementCollections = new List<IChildElementCollection>();
        

        #endregion


        /// <summary>
        /// 创建ModelElementTypeImpl实例
        /// </summary>
        /// <param name="model">Model实例</param>
        /// <param name="name">ModelElement类型名称</param>
        /// <param name="instanceType">实例Type</param>
        public ModelElementTypeImpl(ModelImpl model, string name, Type instanceType)
        {
            this._model = model;
            this._typeName = name;
            this._instanceType = instanceType;
        }


        public virtual IModelElementInstance NewInstance(IModelInstance modelInstance)
        {
            ModelInstanceImpl modelInstanceImpl = (ModelInstanceImpl)modelInstance;
            IDomDocument document = modelInstanceImpl.Document;
            IDomElement domElement = document.CreateElement(_typeNamespace, _typeName);
            return NewInstance(modelInstanceImpl, domElement);
        }

        public virtual IModelElementInstance NewInstance(IModelInstance modelInstance, IDomElement domElement)
        {
            ModelTypeInstanceContext modelTypeInstanceContext = new ModelTypeInstanceContext(domElement, modelInstance, this);
            return CreateModelElementInstance(modelTypeInstanceContext);
        }

        public virtual void RegisterAttribute(IAttribute attribute)
        {
            if (!_attributes.Contains(attribute))
            {
                _attributes.Add(attribute);
            }
        }

        public virtual void RegisterChildElementType(IModelElementType childElementType)
        {
            if (!_childElementTypes.Contains(childElementType))
            {
                _childElementTypes.Add(childElementType);
            }
        }

        public virtual void RegisterChildElementCollection(IChildElementCollection/*<IModelElementInstance>*/ childElementCollection)
        {
            if (!_childElementCollections.Contains(childElementCollection))
            {
                _childElementCollections.Add(childElementCollection);
            }
        }        

        public virtual void RegisterExtendingType(IModelElementType modelType)
        {
            if (!_extendingTypes.Contains(modelType))
            {
                _extendingTypes.Add(modelType);
            }
        }

        protected internal virtual IModelElementInstance CreateModelElementInstance(ModelTypeInstanceContext instanceContext)
        {
            if (Abstract)
            {
                throw new ModelTypeException("Model element type " + TypeName + " is abstract and no instances can be created.");
            }
            return InstanceProvider.NewInstance(instanceContext);
        }

        public IList<IAttribute> Attributes => _attributes;        

        public virtual string TypeName => _typeName;

        public virtual Type InstanceType => _instanceType;

        public virtual T GetInstanceType<T>() where T : IModelElementInstance {
            return (T)Convert.ChangeType(_instanceType, typeof(T));
        }

        public virtual string TypeNamespace
        {
            set => this._typeNamespace = value;
            get => _typeNamespace;
        }


        public virtual void SetBaseType(ModelElementTypeImpl baseType)
        {
            if (this._baseType == null)
            {
                this._baseType = baseType;
            }
            else if (!this._baseType.Equals(baseType))
            {
                throw new ModelException("Type can not have multiple base types. " + this.GetType() + " already extends type " + this._baseType.GetType() + " and can not also extend type " + baseType.GetType());
            }
        }

        public IModelElementType BaseType => _baseType;


        public IModelTypeInstanceProvider<IModelElementInstance> InstanceProvider { private get; set; }        


        public virtual bool Abstract { get; set; }


        public virtual ICollection<IModelElementType> ExtendingTypes
        {
            get
            {
                var t = new List<IModelElementType>();
                t.AddRange(_extendingTypes);
                return t;
            }
        }

        public virtual ICollection<IModelElementType> AllExtendingTypes
        {
            get
            {
                HashSet<IModelElementType> extendingTypes = new HashSet<IModelElementType>();
                extendingTypes.Add(this);
                ResolveExtendingTypes(extendingTypes);
                return extendingTypes;
            }
        }
        
        public virtual void ResolveExtendingTypes(ISet<IModelElementType> allExtendingTypes)
        {
            foreach (IModelElementType modelElementType in _extendingTypes)
            {
                ModelElementTypeImpl modelElementTypeImpl = (ModelElementTypeImpl)modelElementType;
                if (!allExtendingTypes.Contains(modelElementTypeImpl))
                {
                    allExtendingTypes.Add(modelElementType);
                    modelElementTypeImpl.ResolveExtendingTypes(allExtendingTypes);
                }
            }
        }
        
        public virtual void ResolveBaseTypes(IList<IModelElementType> baseTypes)
        {
            if (_baseType != null)
            {
                baseTypes.Add(_baseType);
                _baseType.ResolveBaseTypes(baseTypes);
            }
        }

        public virtual IModel Model => _model;

        public virtual IList<IModelElementType> ChildElementTypes => _childElementTypes;

        public virtual IList<IModelElementType> AllChildElementTypes
        {
            get
            {
                var allChildElementTypes = new List<IModelElementType>();
                if (_baseType != null)
                {
                    allChildElementTypes.AddRange(_baseType.AllChildElementTypes);
                }
                allChildElementTypes.AddRange(_childElementTypes);
                return allChildElementTypes;
            }
        }

        public virtual IList<IChildElementCollection> ChildElementCollections => _childElementCollections;

        public virtual IList<IChildElementCollection> AllChildElementCollections
        {
            get
            {
                List<IChildElementCollection> allChildElementCollections = new List<IChildElementCollection>();
                if (_baseType != null)
                {
                    allChildElementCollections.AddRange(_baseType.AllChildElementCollections);
                }
                allChildElementCollections.AddRange(_childElementCollections);
                return allChildElementCollections;
            }
        }

        public virtual ICollection<IModelElementInstance> GetInstances(IModelInstance modelInstance)
        {
            ModelInstanceImpl modelInstanceImpl = (ModelInstanceImpl)modelInstance;
            IDomDocument document = modelInstanceImpl.Document;

            IList<IDomElement> elements = GetElementsByNameNs(document, _typeNamespace);

            IList<IModelElementInstance> resultList = new List<IModelElementInstance>();
            foreach (IDomElement element in elements)
            {
                resultList.Add(ModelUtil.GetModelElement(element, modelInstanceImpl));
            }
            return resultList;
        }
       
        protected internal virtual IList<IDomElement> GetElementsByNameNs(IDomDocument document, string namespaceUri)
        {
            IList<IDomElement> elements = document.GetElementsByNameNs(namespaceUri, _typeName);

            if (elements.Count == 0)
            {
                string alternativeNamespaceUri = Model.GetAlternativeNamespace(namespaceUri);
                if (!string.IsNullOrEmpty(alternativeNamespaceUri))
                {
                    elements = GetElementsByNameNs(document, alternativeNamespaceUri);
                }
            }

            return elements;
        }
        
        public virtual bool IsBaseTypeOf(IModelElementType elementType)
        {
            if (this.Equals(elementType))
            {
                return true;
            }
            else
            {
                ICollection<IModelElementType> baseTypes = ModelUtil.CalculateAllBaseTypes(elementType);
                return baseTypes.Contains(this);
            }
        }
        
        public virtual ICollection<IAttribute> AllAttributes
        {
            get
            {
                List<IAttribute> allAttributes = new List<IAttribute>();
                allAttributes.AddRange(Attributes);
                ICollection<IModelElementType> baseTypes = ModelUtil.CalculateAllBaseTypes(this);
                foreach (IModelElementType baseType in baseTypes)
                {
                    allAttributes.AddRange(baseType.Attributes);
                }
                return allAttributes;
            }
        }        

        public virtual IAttribute GetAttribute(string attributeName)
        {
            return AllAttributes.FirstOrDefault(c => c.AttributeName.Equals(attributeName));            
        }

        public virtual IChildElementCollection GetChildElementCollection(IModelElementType childElementType)
        {
            foreach (IChildElementCollection childElementCollection in ChildElementCollections)
            {
                if (childElementType.Equals(childElementCollection.GetChildElementType(_model)))
                {
                    return childElementCollection;
                }
            }
            return null;
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + ((_model == null) ? 0 : _model.GetHashCode());
            result = prime * result + ((string.ReferenceEquals(_typeName, null)) ? 0 : _typeName.GetHashCode());
            result = prime * result + ((string.ReferenceEquals(_typeNamespace, null)) ? 0 : _typeNamespace.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            ModelElementTypeImpl other = (ModelElementTypeImpl)obj;
            if (_model == null)
            {
                if (other._model != null)
                {
                    return false;
                }
            }
            else if (!_model.Equals(other._model))
            {
                return false;
            }
            if (string.ReferenceEquals(_typeName, null))
            {
                if (!string.ReferenceEquals(other._typeName, null))
                {
                    return false;
                }
            }
            else if (!_typeName.Equals(other._typeName))
            {
                return false;
            }
            if (string.ReferenceEquals(_typeNamespace, null))
            {
                if (!string.ReferenceEquals(other._typeNamespace, null))
                {
                    return false;
                }
            }
            else if (!_typeNamespace.Equals(other._typeNamespace))
            {
                return false;
            }
            return true;
        }

    }
}