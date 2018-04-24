using System;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using static ESS.FW.Bpm.Engine.Impl.Juel.SimpleContext;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.type
{

    public class DmnTypeDefinitionImpl : IDmnTypeDefinition
    {
        protected internal static readonly DmnEngineLogger Log = DmnLogger.ENGINE_LOGGER;
        protected internal IDmnDataTypeTransformer transformer;

        protected internal string typeName;

        public DmnTypeDefinitionImpl(string typeName, IDmnDataTypeTransformer transformer)
        {
            this.typeName = typeName;
            this.transformer = transformer;
        }


        public virtual IDmnDataTypeTransformer Transformer
        {
            set { transformer = value; }
        }

        public virtual ITypedValue Transform(object value)
        {
            if (value == null)
            {
                return Variables.UntypedNullValue;
            }
            return TransformNotNullValue(value);
        }

        public virtual string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }


        protected internal virtual ITypedValue TransformNotNullValue(object value)
        {
            EnsureUtil.EnsureNotNull("transformer", transformer);

            try
            {
                return transformer.Transform(value);
            }
            catch (ArgumentException)
            {
                throw Log.invalidValueForTypeDefinition(typeName, value);
            }
        }

        public override string ToString()
        {
            return "DmnTypeDefinitionImpl{" + "typeName='" + typeName + '\'' + '}';
        }
    }
}