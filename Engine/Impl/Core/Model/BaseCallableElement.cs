using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;

namespace ESS.FW.Bpm.Engine.Impl.Core.Model
{
    public class BaseCallableElement
    {
        protected internal CallableElementBinding binding;

        protected internal IParameterValueProvider definitionKeyValueProvider;
        protected internal string deploymentId;
        protected internal IParameterValueProvider tenantIdProvider;
        protected internal IParameterValueProvider versionValueProvider;

        public virtual IParameterValueProvider DefinitionKeyValueProvider
        {
            get { return definitionKeyValueProvider; }
            set { definitionKeyValueProvider = value; }
        }


        public virtual CallableElementBinding Binding
        {
            get { return binding; }
            set { binding = value; }
        }


        public virtual bool LatestBinding
        {
            get
            {
                var binding = Binding;
                return (binding == null) || CallableElementBinding.Latest.Equals(binding);
            }
        }

        public virtual bool DeploymentBinding
        {
            get
            {
                var binding = Binding;
                return CallableElementBinding.Deployment.Equals(binding);
            }
        }

        public virtual bool VersionBinding
        {
            get
            {
                var binding = Binding;
                return CallableElementBinding.Version.Equals(binding);
            }
        }

        public virtual IParameterValueProvider VersionValueProvider
        {
            get { return versionValueProvider; }
            set { versionValueProvider = value; }
        }


        public virtual IParameterValueProvider TenantIdProvider
        {
            set { tenantIdProvider = value; }
            get { return tenantIdProvider; }
        }

        public virtual string DeploymentId
        {
            get { return deploymentId; }
            set { deploymentId = value; }
        }

        public virtual string GetDefinitionKey(IVariableScope variableScope)
        {
            var result = definitionKeyValueProvider.GetValue(variableScope);

            if ((result != null) && !(result is string))
                throw new InvalidCastException("Cannot cast '" + result + "' to String");

            return (string) result;
        }

        public virtual int? GetVersion(IVariableScope variableScope)
        {
            var result = versionValueProvider.GetValue(variableScope);

            if (result != null)
            {
                if (result is string)
                    return Convert.ToInt32((string) result);
                if (result is int?)
                    return (int?) result;
                throw new ProcessEngineException("It is not possible to transform '" + result + "' into an integer.");
            }

            return null;
        }


        public virtual string GetDefinitionTenantId(IVariableScope variableScope)
        {
            return (string) tenantIdProvider.GetValue(variableScope);
        }

        public sealed class CallableElementBinding
        {
            public enum InnerEnum
            {
                Latest,
                Deployment,
                Version
            }

            public static readonly CallableElementBinding Latest = new CallableElementBinding("LATEST", InnerEnum.Latest,
                "latest");

            public static readonly CallableElementBinding Deployment = new CallableElementBinding("DEPLOYMENT",
                InnerEnum.Deployment, "deployment");

            public static readonly CallableElementBinding Version = new CallableElementBinding("VERSION",
                InnerEnum.Version, "version");

            private static readonly IList<CallableElementBinding> ValueList = new List<CallableElementBinding>();
            private static int _nextOrdinal;
            private readonly InnerEnum _innerEnumValue;

            private readonly string _nameValue;
            private readonly int _ordinalValue;

            internal string value;

            static CallableElementBinding()
            {
                ValueList.Add(Latest);
                ValueList.Add(Deployment);
                ValueList.Add(Version);
            }

            internal CallableElementBinding(string name, InnerEnum innerEnum, string value)
            {
                this.value = value;

                _nameValue = name;
                _ordinalValue = _nextOrdinal++;
                _innerEnumValue = innerEnum;
            }

            public string Value
            {
                get { return value; }
            }

            public static IList<CallableElementBinding> Values()
            {
                return ValueList;
            }

            public InnerEnum InnerEnumValue()
            {
                return _innerEnumValue;
            }

            public int Ordinal()
            {
                return _ordinalValue;
            }

            public override string ToString()
            {
                return _nameValue;
            }

            public static CallableElementBinding ValueOf(string name)
            {
                foreach (var enumInstance in Values())
                    if (enumInstance._nameValue == name)
                        return enumInstance;
                throw new ArgumentException(name);
            }
        }
    }
}