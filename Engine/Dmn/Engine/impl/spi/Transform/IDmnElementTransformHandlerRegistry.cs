using ESS.FW.Bpm.Model.Dmn.instance;
using Type = System.Type;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform
{
    /// <summary>
    ///     Registry of DMN model element transformers
    /// </summary>
    public interface IDmnElementTransformHandlerRegistry
    {
        /// <summary>
        ///     Get the transformer for a source type
        /// </summary>
        /// <param name="sourceClass"> the class of the source type </param>
        /// @param
        /// <Source>
        ///     the type of the transformation input </param>
        ///     @param
        ///     <Target>
        ///         the type of the transformation output </param>
        ///         <returns>
        ///             the <seealso cref="IDmnElementTransformHandler{Source,Target}" /> or null if none is registered for
        ///             this source type
        ///         </returns>
        //JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        IDmnElementTransformHandler<Source, Target> getHandler<Source, Target>(Type sourceClass)
            where Source : IDmnModelElementInstance;


        /// <summary>
        ///     Register a <seealso cref="IDmnElementTransformHandler{Source,Target}" /> for a source type
        /// </summary>
        /// <param name="sourceClass"> the class of the source type </param>
        /// <param name="handler"> the handler to register </param>
        /// @param
        /// <Source>
        ///     the type of the transformation input </param>
        ///     @param <Target> the type of the transformation output </param>
        //JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        void addHandler<Source, Target>(Type sourceClass, IDmnElementTransformHandler<Source, Target> handler)
            where Source : IDmnModelElementInstance;
    }
}