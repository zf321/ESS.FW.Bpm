using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.impl.validation;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.validation;
using System.Linq;

namespace ESS.FW.Bpm.Model.Xml.impl
{
    
    public class ModelInstanceImpl : IModelInstance
    {

        protected internal readonly IDomDocument document;
        protected internal ModelImpl model;
        protected internal readonly ModelBuilder ModelBuilder;

        public ModelInstanceImpl(ModelImpl model, ModelBuilder modelBuilder, IDomDocument document)
        {
            this.model = model;
            this.ModelBuilder = modelBuilder;
            this.document = document;
        }

        public virtual IDomDocument Document => document;

        public virtual IModelElementInstance DocumentElement
        {
            get
            {
                IDomElement rootElement = document.RootElement;
                return rootElement != null ? ModelUtil.GetModelElement(rootElement, this) : null;
            }
            set
            {
                ModelUtil.EnsureInstanceOf(value, typeof(ModelElementInstanceImpl));
                IDomElement domElement = value.DomElement;
                document.RootElement = domElement;
            }
        }

        public virtual T NewInstance<T>(Type type) where T : IModelElementInstance
        {
            IModelElementType modelElementType = model.GetType(type);
            if (modelElementType != null)
            {
                return NewInstance<T>(modelElementType);
            }
            throw new ModelException("Cannot create instance of ModelType " + type + ": no such type registered.");
        }
        
        public virtual T NewInstance<T>(IModelElementType type) where T : IModelElementInstance
        {
            IModelElementInstance modelElementInstance = type.NewInstance(this);
            ModelUtil.SetGeneratedUniqueIdentifier(type, modelElementInstance);
            return (T)modelElementInstance;
        }

        public virtual IModel Model => model;

        public virtual IModelElementType RegisterGenericType(string namespaceUri, string localName)
        {
            IModelElementType elementType = model.GetTypeForName(namespaceUri, localName);
            if (elementType == null)
            {
                elementType = ModelBuilder.DefineGenericType<IModelElementInstance>(localName, namespaceUri);
                model = (ModelImpl)ModelBuilder.Build();
            }
            return elementType;
        }
        
        public virtual IModelElementInstance GetModelElementById(string id) 
        {
            if (id == null)
            {
                return null;
            }
            IDomElement element = document.GetElementById(id);
            if (element != null)
            {
                return ModelUtil.GetModelElement(element, this);
            }
            return null;
        }

        public virtual IEnumerable<IModelElementInstance> GetModelElementsByType(IModelElementType type)
        {
            IEnumerable<IModelElementType> extendingTypes = type.AllExtendingTypes;

            List<IModelElementInstance> instances = new List<IModelElementInstance>();
            foreach (IModelElementType modelElementType in extendingTypes)
            {
                if (!modelElementType.Abstract)
                {
                    instances.AddRange(modelElementType.GetInstances(this));
                }
            }
            return instances;
        }
        
        public virtual IEnumerable<T> GetModelElementsByType<T>(Type referencingClass) where T : IModelElementInstance
        {
            return GetModelElementsByType(Model.GetType(referencingClass)).Cast<T>().ToList();
        }

        public virtual IModelInstance Clone()
        {
            return new ModelInstanceImpl(model, ModelBuilder, document.Clone());
        }

        public virtual IValidationResults Validate(ICollection<IModelElementValidator<IModelElementInstance>> validators)
        {
            return new ModelInstanceValidator(this, validators).Validate();
        }
    }
}