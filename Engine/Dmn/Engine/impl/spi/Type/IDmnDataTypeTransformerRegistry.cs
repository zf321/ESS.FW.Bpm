namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type
{
    /// <summary>
    ///     Provide <seealso cref="IDmnDataTypeTransformer" />s for specific type names.
    ///     
    /// </summary>
    public interface IDmnDataTypeTransformerRegistry
    {
        /// <summary>
        ///     Returns the matching transformer for the given type.
        /// </summary>
        /// <param name="typeName"> name of the type </param>
        /// <returns> the matching transformer </returns>
        IDmnDataTypeTransformer GetTransformer(string typeName);

        void AddTransformer(string typeName, IDmnDataTypeTransformer transformer);
    }
}