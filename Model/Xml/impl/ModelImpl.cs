using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Xml.impl
{
    public class ModelImpl : IModel
    {
        private readonly string _modelName;
        private readonly IDictionary<Type, IModelElementType> _typesByClass = new Dictionary<Type, IModelElementType>();
        private readonly IDictionary<QName, IModelElementType> _typesByName = new Dictionary<QName, IModelElementType>();
        protected internal readonly IDictionary<string, string> ActualNsToAlternative = new Dictionary<string, string>();
        protected internal readonly IDictionary<string, string> AlternativeNsToActual = new Dictionary<string, string>();
        
        public ModelImpl(string modelName)
        {
            _modelName = modelName;
        }

        public virtual string GetAlternativeNamespace(string actualNs)
        {
            if (ActualNsToAlternative.ContainsKey(actualNs))
                return ActualNsToAlternative[actualNs];
            return null;
        }

        public virtual string GetActualNamespace(string alternativeNs)
        {
            return AlternativeNsToActual[alternativeNs];
        }

        public virtual ICollection<IModelElementType> Types => new List<IModelElementType>(_typesByName.Values);

        public virtual IModelElementType GetTypeForName(string typeName)
        {
            return GetTypeForName(null, typeName);
        }

        public virtual IModelElementType GetTypeForName(string namespaceUri, string typeName)
        {
            return _typesByName[ModelUtil.GetQName(namespaceUri, typeName)];
        }

        public virtual string ModelName => _modelName;
        
        public virtual void DeclareAlternativeNamespace(string alternativeNs, string actualNs)
        {
            if (ActualNsToAlternative.ContainsKey(actualNs) || AlternativeNsToActual.ContainsKey(alternativeNs))
                throw new ArgumentException("Cannot register two alternatives for one namespace! Actual Ns: " +
                                            actualNs + " second alternative: " + alternativeNs);
            ActualNsToAlternative[actualNs] = alternativeNs;
            AlternativeNsToActual[alternativeNs] = actualNs;
        }

        public virtual void UndeclareAlternativeNamespace(string alternativeNs)
        {
            if (!AlternativeNsToActual.ContainsKey(alternativeNs))
                return;
            var actual = AlternativeNsToActual[alternativeNs];
            AlternativeNsToActual.Remove(alternativeNs);
            AlternativeNsToActual.Remove(actual);
        }

        public virtual IModelElementType GetType(Type instanceClass)
        {
            return _typesByClass[instanceClass];
        }
        
        public virtual void RegisterType(IModelElementType modelElementType, Type instanceType)
        {
            var qName = ModelUtil.GetQName(modelElementType.TypeNamespace, modelElementType.TypeName);
            _typesByName[qName] = modelElementType;
            _typesByClass[instanceType] = modelElementType;
        }

        public override int GetHashCode()
        {
            var prime = 31;
            var result = 1;
            result = prime * result + (ReferenceEquals(_modelName, null) ? 0 : _modelName.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            var other = (ModelImpl)obj;
            if (ReferenceEquals(_modelName, null))
            {
                if (!ReferenceEquals(other._modelName, null))
                    return false;
            }
            else if (!_modelName.Equals(other._modelName))
            {
                return false;
            }
            return true;
        }
    }
}