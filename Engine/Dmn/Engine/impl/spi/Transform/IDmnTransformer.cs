using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform
{
    /// <summary>
    ///     DMN Transformer which creates a <seealso cref="IDmnTransform" /> to transform a
    ///     DMN model instance
    /// </summary>
    public interface IDmnTransformer
    {
        /// <returns> the <seealso cref="IDmnTransform" /> factory </returns>
        IDmnTransformFactory TransformFactory { get; }

        /// <returns> the transform listeners </returns>
        IList<IDmnTransformListener> TransformListeners { get; set; }

        /// <returns> the <seealso cref="IDmnElementTransformHandlerRegistry" /> </returns>
        IDmnElementTransformHandlerRegistry ElementTransformHandlerRegistry { get; set; }

        /// <returns> the <seealso cref="IDmnDataTypeTransformerRegistry" /> </returns>
        IDmnDataTypeTransformerRegistry DataTypeTransformerRegistry { get; set; }

        /// <returns> the <seealso cref="IDmnHitPolicyHandlerRegistry" /> </returns>
        IDmnHitPolicyHandlerRegistry HitPolicyHandlerRegistry { get; set; }


        /// <summary>
        ///     Set the transform listeners
        /// </summary>
        /// <param name="transformListeners"> the transform listeners to use </param>
        /// <returns> this <seealso cref="IDmnTransform" /> </returns>
        IDmnTransformer transformListeners(IList<IDmnTransformListener> transformListeners);


        /// <summary>
        ///     Set the <seealso cref="IDmnElementTransformHandlerRegistry" />
        /// </summary>
        /// <param name="elementTransformHandlerRegistry"> the registry to use </param>
        /// <returns> this DmnTransformer </returns>
        IDmnTransformer elementTransformHandlerRegistry(
            IDmnElementTransformHandlerRegistry elementTransformHandlerRegistry);


        /// <summary>
        ///     Set the <seealso cref="IDmnDataTypeTransformerRegistry" />
        /// </summary>
        /// <param name="dataTypeTransformerRegistry"> the <seealso cref="IDmnDataTypeTransformerRegistry" /> to use </param>
        /// <returns> this DmnTransformer </returns>
        IDmnTransformer dataTypeTransformerRegistry(IDmnDataTypeTransformerRegistry dataTypeTransformerRegistry);


        /// <summary>
        ///     Set the <seealso cref="IDmnHitPolicyHandlerRegistry" />
        /// </summary>
        /// <param name="hitPolicyHandlerRegistry"> the <seealso cref="IDmnHitPolicyHandlerRegistry" /> to use </param>
        /// <returns> this DmnTransformer </returns>
        IDmnTransformer hitPolicyHandlerRegistry(IDmnHitPolicyHandlerRegistry hitPolicyHandlerRegistry);

        /// <summary>
        ///     Create a <seealso cref="IDmnTransform" />
        /// </summary>
        /// <returns> the <seealso cref="IDmnTransform" /> </returns>
        IDmnTransform createTransform();
    }
}