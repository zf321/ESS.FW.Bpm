using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class DefaultVariableSerializers : IVariableSerializers
    {
        private const long serialVersionUID = 1L;
        
        protected internal IDictionary<string, ITypedValueSerializer> serializerMap =
            new Dictionary<string, ITypedValueSerializer>();

        public DefaultVariableSerializers()
        {
        }

        public DefaultVariableSerializers(DefaultVariableSerializers serializers)
        {
            foreach (var typedValueSerializer in serializers.serializerMap)
            {
                serializerMap.Add(typedValueSerializer.Key, typedValueSerializer.Value);
            }
        }
        
        public virtual IList<ITypedValueSerializer> Serializers
        {
            get
            {
                return new List<ITypedValueSerializer>(serializerMap.Values);
            }
        }
        public virtual ITypedValueSerializer GetSerializerByName(string serializerName)
        {
            ITypedValueSerializer serializer;
            serializerMap.TryGetValue(serializerName, out serializer);
            return serializer;
        }
        
        public virtual ITypedValueSerializer FindSerializerForValue(ITypedValue value,
            IVariableSerializerFactory fallBackSerializerFactory)
        {
            var defaultSerializationFormat = Context.ProcessEngineConfiguration.DefaultSerializationFormat;
            
            IList<ITypedValueSerializer> matchedSerializers =
                new List<ITypedValueSerializer>();

            IValueType type = value.Type;
            if (type != null && type.IsAbstract)
                throw new ProcessEngineException("Cannot serialize value of abstract type " + type.Name);
            
            foreach (var serializer in serializerMap.Values)
                if (type == null || serializer.Type.Equals(type))
                    if (serializer.CanHandle(value))
                    {
                        matchedSerializers.Add(serializer);
                        if (serializer.Type.IsPrimitiveValueType)
                            break;
                    }

            if (matchedSerializers.Count == 0)
            {
                if (fallBackSerializerFactory != null)
                {
                    ITypedValueSerializer serializer = fallBackSerializerFactory.GetSerializer<ITypedValue>(value);
                    if (serializer != null)
                        return serializer;
                }

                throw new ProcessEngineException("Cannot find serializer for value '" + value + "'.");
            }
            if (matchedSerializers.Count == 1)
                return matchedSerializers[0];
            // ambiguous match, use default serializer
            if (!ReferenceEquals(defaultSerializationFormat, null))
                foreach (var typedValueSerializer in matchedSerializers)
                    if (defaultSerializationFormat.Equals(typedValueSerializer.SerializationDataFormat))
                        return typedValueSerializer;
            // no default serialization dataformat defined or default dataformat cannot serialize this value => use first serializer
            return matchedSerializers[0];
        }
        
        public virtual ITypedValueSerializer FindSerializerForValue(ITypedValue value)
        {
            return FindSerializerForValue(value, null);
        }
        

        public virtual IVariableSerializers AddSerializer(ITypedValueSerializer serializer) 
        {
            return AddSerializer(serializer, serializerMap.Count);
        }

        public virtual IVariableSerializers AddSerializer(ITypedValueSerializer serializer, int index) 
        {
            serializerMap[serializer.Name] =  serializer;
            return this;
        }
        
        
        public virtual IVariableSerializers RemoveSerializer(ITypedValueSerializer serializer)
        {
            serializerMap.Remove(serializer.Name);
            return this;
        }

        public virtual IVariableSerializers Join(IVariableSerializers other)
        {
            var copy = new DefaultVariableSerializers();

            // "other" serializers override existing ones if their names match
            foreach (var thisSerializer in serializerMap.Values)
            {
                ITypedValueSerializer serializer = other.GetSerializerByName(thisSerializer.Name);

                if (serializer == null)
                    serializer = thisSerializer;

                copy.AddSerializer(serializer);
            }

            // add all "other" serializers that did not exist before to the end of the list
            foreach (ITypedValueSerializer otherSerializer in other.Serializers)
                if (!copy.serializerMap.ContainsKey(otherSerializer.Name))
                    copy.AddSerializer(otherSerializer);


            return copy;
        }
        

    }
}