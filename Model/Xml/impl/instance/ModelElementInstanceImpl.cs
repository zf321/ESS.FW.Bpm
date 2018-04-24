using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.type;
using ESS.FW.Bpm.Model.Xml.impl.type.attribute;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;


namespace ESS.FW.Bpm.Model.Xml.impl.instance
{
    public class ModelElementInstanceImpl : IModelElementInstance
    {
        public virtual string Id { get; set; }
        /// <summary>
        /// the containing model instance </summary>
        protected IModelInstance modelInstance;
        /// <summary>
        /// the wrapped DOM <seealso cref="DomElement"/> </summary>
        private IDomElement _domElement;
        /// <summary>
        /// the implementing model element type </summary>
        private readonly ModelElementTypeImpl _elementType;

        public static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IModelElementInstance>("").AbstractType();

            typeBuilder.Build();
        }

        public ModelElementInstanceImpl(ModelTypeInstanceContext instanceContext)
        {
            this._domElement = instanceContext.DomElement;
            this.modelInstance = instanceContext.Model;
            this._elementType = instanceContext.ModelType;
        }

        public virtual IDomElement DomElement => _domElement;

        public virtual IModelInstance ModelInstance => modelInstance;

        public virtual IModelElementInstance ParentElement
        {
            get
            {
                IDomElement parentElement = _domElement.ParentElement;
                if (parentElement != null)
                {
                    return ModelUtil.GetModelElement(parentElement, modelInstance);
                }
                return null;
            }
        }

        public virtual IModelElementType ElementType => _elementType;

        public virtual string GetAttributeValue(string attributeName)
        {
            return _domElement.GetAttribute(attributeName);
        }

        public virtual string GetAttributeValueNs(string namespaceUri, string attributeName)
        {
            return _domElement.GetAttribute(namespaceUri, attributeName);
        }

        public virtual void SetAttributeValue(string attributeName, string xmlValue)
        {
            SetAttributeValue(attributeName, xmlValue, false);
        }

        public virtual void SetAttributeValue(string attributeName, string xmlValue, bool isIdAttribute)
        {
            string oldValue = GetAttributeValue(attributeName);
            if (isIdAttribute)
            {
                _domElement.SetIdAttribute(attributeName, xmlValue);
            }
            else
            {
                _domElement.SetAttribute(attributeName, xmlValue);
            }

            IAttribute attribute = _elementType.GetAttribute(attributeName);
            if (attribute != null)
            {
                ((AttributeImpl)attribute).UpdateIncomingReferences(this, xmlValue, oldValue);
            }
        }

        public virtual void SetAttributeValueNs(string namespaceUri, string attributeName, string xmlValue)
        {
            SetAttributeValueNs(namespaceUri, attributeName, xmlValue, false);
        }

        public virtual void SetAttributeValueNs(string namespaceUri, string attributeName, string xmlValue, bool isIdAttribute)
        {
            string namespaceForSetting = namespaceUri;
            if (HasValueToBeSetForAlternativeNs(namespaceUri, attributeName))
            {
                namespaceForSetting = modelInstance.Model.GetAlternativeNamespace(namespaceUri);
            }
            string oldValue = GetAttributeValueNs(namespaceForSetting, attributeName);
            if (isIdAttribute)
            {
                _domElement.SetIdAttribute(namespaceForSetting, attributeName, xmlValue);
            }
            else
            {
                _domElement.SetAttribute(namespaceForSetting, attributeName, xmlValue);
            }

            IAttribute attribute = _elementType.GetAttribute(attributeName);
            if (attribute != null)
            {
                ((AttributeImpl)attribute).UpdateIncomingReferences(this, xmlValue, oldValue);
            }
        }

        private bool HasValueToBeSetForAlternativeNs(string namespaceUri, string attributeName)
        {
            string alternativeNs = modelInstance.Model.GetAlternativeNamespace(namespaceUri);
            return string.ReferenceEquals(GetAttributeValueNs(namespaceUri, attributeName), null) && !string.ReferenceEquals(alternativeNs, null) && !string.ReferenceEquals(GetAttributeValueNs(alternativeNs, attributeName), null);
        }

        public virtual void RemoveAttribute(string attributeName)
        {
            var attribute = _elementType.GetAttribute(attributeName);
            if (attribute != null)
            {
                object identifier = attribute.GetValue<object>(this);
                if (identifier != null)
                {
                    ((AttributeImpl)attribute).UnlinkReference(this, identifier);
                }
            }
            _domElement.RemoveAttribute(attributeName);
        }

        public virtual void RemoveAttributeNs(string namespaceUri, string attributeName)
        {
            IAttribute attribute = _elementType.GetAttribute(attributeName);
            if (attribute != null)
            {
                object identifier = attribute.GetValue<Object>(this);
                if (identifier != null)
                {
                    ((AttributeImpl)attribute).UnlinkReference(this, identifier);
                }
            }
            _domElement.RemoveAttribute(namespaceUri, attributeName);
        }

        public virtual string TextContent
        {
            get => RawTextContent.Trim();
            set => _domElement.TextContent = value;
        }

        public virtual string RawTextContent => _domElement.TextContent;

        public virtual IModelElementInstance GetUniqueChildElementByNameNs(string namespaceUri, string elementName)
        {
            IModel model = modelInstance.Model;
            IList<IDomElement> childElements = _domElement.GetChildElementsByNameNs(AsSet(namespaceUri, model.GetAlternativeNamespace(namespaceUri)), elementName);
            if (childElements.Count > 0)
            {
                return ModelUtil.GetModelElement(childElements[0], modelInstance);
            }
            return null;
        }


        public virtual IModelElementInstance GetUniqueChildElementByType(Type elementType)
        {
            IList<IDomElement> childElements = _domElement.GetChildElementsByType(modelInstance, elementType);

            if (childElements.Count > 0)
            {
                var t = ModelUtil.GetModelElement(childElements[0], modelInstance);
                return t;
            }
            return null;
        }

        public virtual IModelElementInstance UniqueChildElementByNameNs
        {
            set
            {
                ModelUtil.EnsureInstanceOf(value, typeof(ModelElementInstanceImpl));
                ModelElementInstanceImpl newChildElement = (ModelElementInstanceImpl)value;

                IDomElement childElement = newChildElement.DomElement;
                IModelElementInstance existingChild = GetUniqueChildElementByNameNs(childElement.NamespaceUri, childElement.LocalName);
                if (existingChild == null)
                {
                    AddChildElement(value);
                }
                else
                {
                    ReplaceChildElement(existingChild, newChildElement);
                }
            }
        }

        public virtual void ReplaceChildElement(IModelElementInstance existingChild, IModelElementInstance newChild)
        {
            IDomElement existingChildDomElement = existingChild.DomElement;
            IDomElement newChildDomElement = newChild.DomElement;

            // unlink (remove all references) of child elements
            ((ModelElementInstanceImpl)existingChild).UnlinkAllChildReferences();

            // update incoming references from old to new child element
            UpdateIncomingReferences(existingChild, newChild);

            // replace the existing child with the new child in the DOM
            _domElement.ReplaceChild(newChildDomElement, existingChildDomElement);

            // execute after replacement updates
            newChild.UpdateAfterReplacement();
        }

        private void UpdateIncomingReferences(IModelElementInstance oldInstance, IModelElementInstance newInstance)
        {
            string oldId = oldInstance.GetAttributeValue("id");
            string newId = newInstance.GetAttributeValue("id");

            if (string.ReferenceEquals(oldId, null) || string.ReferenceEquals(newId, null))
            {
                return;
            }

            ICollection<IAttribute> attributes = ((ModelElementTypeImpl)oldInstance.ElementType).AllAttributes;

            foreach (var attribute in attributes)
            {
                if (attribute.IdAttribute)
                {
                    foreach (var incomingReference in attribute.IncomingReferences)
                    {
                        ((ReferenceImpl)incomingReference).ReferencedElementUpdated(newInstance, oldId, newId);
                    }
                }
            }

        }

        public virtual void ReplaceWithElement(IModelElementInstance newElement)
        {
            ModelElementInstanceImpl parentElement = (ModelElementInstanceImpl)ParentElement;
            if (parentElement != null)
            {
                parentElement.ReplaceChildElement(this, newElement);
            }
            else
            {
                throw new ModelException("Unable to remove replace without parent");
            }
        }

        public virtual void AddChildElement(IModelElementInstance newChild)
        {
            ModelUtil.EnsureInstanceOf(newChild, typeof(ModelElementInstanceImpl));
            IModelElementInstance elementToInsertAfter = FindElementToInsertAfter(newChild);
            InsertElementAfter(newChild, elementToInsertAfter);
        }

        public virtual bool RemoveChildElement(IModelElementInstance child)
        {
            ModelElementInstanceImpl childImpl = (ModelElementInstanceImpl)child;
            childImpl.UnlinkAllReferences();
            childImpl.UnlinkAllChildReferences();
            return _domElement.RemoveChild(child.DomElement);
        }

        public IList<IModelElementInstance> GetChildElementsByType(Type childElementType)
        {
            return GetChildElementsByType(ModelInstance.Model.GetType(childElementType));
        }

        public virtual IList<IModelElementInstance> GetChildElementsByType(IModelElementType childElementType)
        {
            IList<IModelElementInstance> instances = new List<IModelElementInstance>();
            foreach (IModelElementType extendingType in childElementType.ExtendingTypes)
            {
                ((List<IModelElementInstance>)instances).AddRange(GetChildElementsByType(extendingType));
            }
            IModel model = modelInstance.Model;
            string alternativeNamespace = model.GetAlternativeNamespace(childElementType.TypeNamespace);
            IList<IDomElement> elements = _domElement.GetChildElementsByNameNs(AsSet(childElementType.TypeNamespace, alternativeNamespace), childElementType.TypeName);
            ((List<IModelElementInstance>)instances).AddRange(ModelUtil.GetModelElementCollection<IModelElementInstance>(elements, modelInstance));
            return instances;
        }
        /// <summary>
        /// 添加通用泛型方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="childElementType"></param>
        /// <returns></returns>
        public virtual IList<T> GetChildElementsByType<T>(IModelElementType childElementType) where T : IModelElementInstance
        {
            List<T> instances = new List<T>();
            foreach (IModelElementType extendingType in childElementType.ExtendingTypes)
            {
                foreach (var item in GetChildElementsByType(extendingType))
                {
                    instances.Add((T)item);
                }
                //instances.AddRange(getChildElementsByType(extendingType) as IList<T>);
            }
            IModel model = modelInstance.Model;
            string alternativeNamespace = model.GetAlternativeNamespace(childElementType.TypeNamespace);
            IList<IDomElement> elements = _domElement.GetChildElementsByNameNs(AsSet(childElementType.TypeNamespace, alternativeNamespace), childElementType.TypeName);
            //var t = ModelUtil.getModelElementCollection<T>(elements, modelInstance);
            instances.AddRange(ModelUtil.GetModelElementCollection<T>(elements, modelInstance));
            return instances;
        }
        public virtual IList<T> GetChildElementsByType<T>(System.Type childElementClass) where T : IModelElementInstance
        {
            return GetChildElementsByType<T>(ModelInstance.Model.GetType(childElementClass));
        }

        /// <summary>
        /// Returns the element after which the new element should be inserted in the DOM document.
        /// </summary>
        /// <param name="elementToInsert">  the new element to insert </param>
        /// <returns> the element to insert after or null </returns>
        private IModelElementInstance FindElementToInsertAfter(IModelElementInstance elementToInsert)
        {
            IList<IModelElementType> childElementTypes = _elementType.AllChildElementTypes;
            IList<IDomElement> childDomElements = _domElement.ChildElements;
            ICollection<IModelElementInstance> childElements = ModelUtil.GetModelElementCollection<IModelElementInstance>(childDomElements, modelInstance);

            IModelElementInstance insertAfterElement = null;
            int newElementTypeIndex = ModelUtil.GetIndexOfElementType(elementToInsert, childElementTypes);
            foreach (IModelElementInstance childElement in childElements)
            {
                int childElementTypeIndex = ModelUtil.GetIndexOfElementType(childElement, childElementTypes);
                if (newElementTypeIndex >= childElementTypeIndex)
                {
                    insertAfterElement = childElement;
                }
                else
                {
                    break;
                }
            }
            return insertAfterElement;
        }

        public virtual void InsertElementAfter(IModelElementInstance elementToInsert, IModelElementInstance insertAfterElement)
        {
            if (insertAfterElement == null || insertAfterElement.DomElement == null)
            {
                _domElement.InsertChildElementAfter(elementToInsert.DomElement, null);
            }
            else
            {
                _domElement.InsertChildElementAfter(elementToInsert.DomElement, insertAfterElement.DomElement);
            }
        }

        public virtual void UpdateAfterReplacement()
        {
            // do nothing
        }

        /// <summary>
        /// Removes all reference to this.
        /// </summary>
        private void UnlinkAllReferences()
        {
            ICollection<IAttribute> attributes = _elementType.AllAttributes;

            foreach (var attribute in attributes)
            {
                object identifier = attribute.GetValue<Object>(this);
                if (identifier != null)
                {
                    ((AttributeImpl)attribute).UnlinkReference(this, identifier);
                }
            }
        }

        /// <summary>
        /// Removes every reference to children of this.
        /// </summary>
        private void UnlinkAllChildReferences()
        {
            IList<IModelElementType> childElementTypes = _elementType.AllChildElementTypes;
            foreach (IModelElementType type in childElementTypes)
            {
                ICollection<IModelElementInstance> childElementsForType = GetChildElementsByType(type);
                foreach (IModelElementInstance childElement in childElementsForType)
                {
                    ((ModelElementInstanceImpl)childElement).UnlinkAllReferences();
                }
            }
        }

        protected internal virtual ISet<T> AsSet<T>(params T[] elements)
        {
            return new HashSet<T>(elements);
        }

        public override int GetHashCode()
        {
            return _domElement.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj == this)
            {
                return true;
            }
            if (!(obj is ModelElementInstanceImpl))
            {
                return false;
            }
            ModelElementInstanceImpl other = (ModelElementInstanceImpl)obj;
            return other._domElement.Equals(_domElement);
        }

    }
}