using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;


namespace ESS.FW.Bpm.Model.Xml.impl.type.attribute
{    
    public abstract class AttributeImpl : IAttribute
    {
        private Object _defaultValue;

        private readonly IList<IReference> _outgoingReferences = new List<IReference>();

        private readonly IList<IReference> _incomingReferences = new List<IReference>();

        private readonly IModelElementType _owningElementType;

        internal AttributeImpl(IModelElementType owningElementType)
        {
            this._owningElementType = owningElementType;
        }
        
        protected abstract T ConvertXmlValueToModelValue<T>(string rawValue);
        
        protected abstract string ConvertModelValueToXmlValue<T>(T modelValue);

        public virtual IModelElementType OwningElementType => _owningElementType;

        /// <summary>
        /// returns the value of the attribute.
        /// </summary>
        /// <returns> the value of the attribute. </returns>
        public virtual T GetValue<T>(IModelElementInstance modelElement)
        {
            string value;
            if (NamespaceUri == null)
            {
                value = modelElement.GetAttributeValue(AttributeName);
            }
            else
            {
                value = modelElement.GetAttributeValueNs(NamespaceUri, AttributeName);
                if (value == null)
                {
                    string alternativeNamespace = _owningElementType.Model.GetAlternativeNamespace(NamespaceUri);
                    if (alternativeNamespace != null)
                    {
                        value = modelElement.GetAttributeValueNs(alternativeNamespace, AttributeName);
                    }
                }
            }

            if ((value == null) && _defaultValue != null)
            {
                return GetDefaultValue<T>();
            }
            return ConvertXmlValueToModelValue<T>(value);
        }
        
        public virtual void SetValue<T>(IModelElementInstance modelElement, T value)
        {
            string xmlValue = ConvertModelValueToXmlValue(value);
            if (string.ReferenceEquals(NamespaceUri, null))
            {
                modelElement.SetAttributeValue(AttributeName, xmlValue, IdAttribute);
            }
            else
            {
                modelElement.SetAttributeValueNs(NamespaceUri, AttributeName, xmlValue, IdAttribute);
            }
        }

        public T GetDefaultValue<T>()
        {
            return (T)_defaultValue;
        }

        public void SetDefaultValue<T>(T defaultValue)
        {
            _defaultValue = defaultValue;
        }

        public virtual void UpdateIncomingReferences(IModelElementInstance modelElement, string newIdentifier, string oldIdentifier)
        {
            if (_incomingReferences.Count > 0)
            {
                foreach (IReference incomingReference in _incomingReferences)
                {
                    ((ReferenceImpl)incomingReference).ReferencedElementUpdated(modelElement, oldIdentifier, newIdentifier);
                }
            }
        }

        public virtual bool Required { get; set; } = false;

        public virtual string NamespaceUri { set; get; }

        public virtual bool IdAttribute { get; private set; } = false;
        
        public virtual void SetId()
        {
            this.IdAttribute = true;
        }

        /// <returns> the attributeName </returns>
        public virtual string AttributeName { get; set; }


        public virtual void RemoveAttribute(IModelElementInstance modelElement)
        {
            if (string.ReferenceEquals(NamespaceUri, null))
            {
                modelElement.RemoveAttribute(AttributeName);
            }
            else
            {
                modelElement.RemoveAttributeNs(NamespaceUri, AttributeName);
            }
        }

        public virtual void UnlinkReference(IModelElementInstance modelElement, object referenceIdentifier)
        {
            if (_incomingReferences.Count > 0)
            {
                foreach (IReference incomingReference in _incomingReferences)
                {
                    ((ReferenceImpl)incomingReference).ReferencedElementRemoved(modelElement, referenceIdentifier);
                }
            }
        }

        public virtual IList<IReference> IncomingReferences => _incomingReferences;


        public virtual IList<IReference> OutgoingReferences => _outgoingReferences;

        public virtual void RegisterOutgoingReference(IReference Ref)
        {
            _outgoingReferences.Add(Ref);
        }

        public virtual void RegisterIncoming(IReference Ref)
        {
            _incomingReferences.Add(Ref);
        }

    }

}