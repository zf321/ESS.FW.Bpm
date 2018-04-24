using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.type
{
    /// <summary>
    ///     <seealso cref="IDmnDataTypeTransformerRegistry" /> for the built-in <seealso cref="IDmnDataTypeTransformer" />s.
    ///     
    /// </summary>
    public class DefaultDataTypeTransformerRegistry : IDmnDataTypeTransformerRegistry
    {
        protected internal static readonly DmnEngineLogger Log = DmnLogger.ENGINE_LOGGER;

        protected internal static readonly IDictionary<string, IDmnDataTypeTransformer> Transformers =
            DefaultTransformers;

        protected internal static IDictionary<string, IDmnDataTypeTransformer> DefaultTransformers
        {
            get
            {
                IDictionary<string, IDmnDataTypeTransformer> transformers =
                    new Dictionary<string, IDmnDataTypeTransformer>();

                transformers["string"] = new StringDataTypeTransformer();
                transformers["boolean"] = new BooleanDataTypeTransformer();
                transformers["integer"] = new IntegerDataTypeTransformer();
                transformers["long"] = new LongDataTypeTransformer();
                transformers["double"] = new DoubleDataTypeTransformer();
                //transformers["date"] = new DateDataTypeTransformer();

                return transformers;
            }
        }

        public virtual void AddTransformer(string typeName, IDmnDataTypeTransformer transformer)
        {
            Transformers[typeName] = transformer;
        }

        public virtual IDmnDataTypeTransformer GetTransformer(string typeName)
        {
            if (!ReferenceEquals(typeName, null) && Transformers.ContainsKey(typeName.ToLower()))
                return Transformers[typeName.ToLower()];
            Log.unsupportedTypeDefinitionForClause(typeName);
            return new IdentityDataTypeTransformer();
        }
    }
}