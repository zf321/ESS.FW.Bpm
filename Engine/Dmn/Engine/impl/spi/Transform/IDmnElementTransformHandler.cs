using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform
{
    /// <summary>
    ///     Handler to transform a DMN model element.
    /// </summary>
    /// @param
    /// <Source>
    ///     the type of the transformation input </param>
    ///     @param <Target> the type of the transformation output </param>
    public interface IDmnElementTransformHandler<in Source, out Target> where Source : IDmnModelElementInstance
    {
        /// <summary>
        ///     Transform a DMN model element
        /// </summary>
        /// <param name="context"> the transformation context </param>
        /// <param name="element"> the source element </param>
        /// <returns> the transformed element </returns>
        Target HandleElement(IDmnElementTransformContext context, Source element);
    }
}