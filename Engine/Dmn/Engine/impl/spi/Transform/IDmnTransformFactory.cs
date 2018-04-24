namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform
{
    /// <summary>
    ///     Factory to create a <seealso cref="IDmnTransform" /> from a <seealso cref="IDmnTransformer" />
    /// </summary>
    public interface IDmnTransformFactory
    {
        /// <summary>
        ///     Create a <seealso cref="IDmnTransform" /> for a <seealso cref="IDmnTransformer" />
        /// </summary>
        /// <param name="transformer"> the <seealso cref="IDmnTransformer" /> to use </param>
        /// <returns> the <seealso cref="IDmnTransform" /> </returns>
        IDmnTransform createTransform(IDmnTransformer transformer);
    }
}