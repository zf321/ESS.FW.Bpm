using System.Collections.Generic;

namespace Engine.Tests.Api.Runtime.Util
{
    public class VariableSpec
    {
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IDictionary<string, object> configuration_Renamed;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string name_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal object serializedValue_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal bool? storesCustomObjects_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal object value_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string valueTypeName_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string variableTypeName_Renamed;

        public virtual string Name
        {
            get { return name_Renamed; }
        }

        public virtual object Value
        {
            get { return value_Renamed; }
        }

        public virtual string VariableTypeName
        {
            get { return variableTypeName_Renamed; }
        }

        public virtual string ValueTypeName
        {
            get { return valueTypeName_Renamed; }
        }

        public virtual object SerializedValue
        {
            get { return serializedValue_Renamed; }
        }

        public virtual IDictionary<string, object> Configuration
        {
            get { return configuration_Renamed; }
        }

        public virtual bool StoresCustomObjects
        {
            get { return storesCustomObjects_Renamed.Value; }
        }

        public virtual VariableSpec name(string name)
        {
            name_Renamed = name;
            return this;
        }

        public virtual VariableSpec value(object value)
        {
            value_Renamed = value;
            return this;
        }

        public virtual VariableSpec variableTypeName(string variableTypeName)
        {
            variableTypeName_Renamed = variableTypeName;
            return this;
        }

        public virtual VariableSpec valueTypeName(string valueTypeName)
        {
            valueTypeName_Renamed = valueTypeName;
            return this;
        }

        public virtual VariableSpec serializedValue(object serializedValue)
        {
            serializedValue_Renamed = serializedValue;
            return this;
        }

        public virtual VariableSpec configuration(IDictionary<string, object> configuration)
        {
            configuration_Renamed = configuration;
            return this;
        }

        public virtual VariableSpec storesCustomObjects(bool storesCustomObjects)
        {
            storesCustomObjects_Renamed = storesCustomObjects;
            return this;
        }
    }
}