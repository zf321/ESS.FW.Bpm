using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.hitpolicy;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform
{
    /// <summary>
    ///     Context available during the DMN transformation
    /// </summary>
    public interface IDmnElementTransformContext
    {
        /// <returns> the transformed DMN model instance </returns>
        IDmnModelInstance ModelInstance { get; }

        /// <returns> the already transformed parent of the current transformed element </returns>
        object Parent { get; }

        /// <returns> the already transformed decision to which the current transformed element belongs </returns>
        IDmnDecision Decision { get; }

        /// <returns> the <seealso cref="IDmnDataTypeTransformerRegistry" /> to use </returns>
        IDmnDataTypeTransformerRegistry DataTypeTransformerRegistry { get; }

        /// <returns> the <seealso cref="IDmnHitPolicyHandlerRegistry" /> to use </returns>
        IDmnHitPolicyHandlerRegistry HitPolicyHandlerRegistry { get; }
    }
}