using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Model.Dmn;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform
{
    /// <summary>
    ///     A transform of a DMN model instance
    /// </summary>
    public interface IDmnTransform
    {
        /// <summary>
        ///     Set the DMN model instance to transform as file.
        /// </summary>
        /// <param name="file"> the file of the DMN model instance </param>
        /// <summary>
        ///     Set the DMN model instance to transform as file.
        /// </summary>
        /// <param name="file"> the file of the DMN model instance </param>
        /// <returns> this DmnTransform </returns>
        /// <summary>
        ///     Set the DMN model instance to transform as input stream.
        /// </summary>
        /// <param name="inputStream"> the input stream of the DMN model instance </param>
        Stream ModelInstance { set; }

        /// <summary>
        ///     Set the DMN model instance to transform as input stream.
        /// </summary>
        /// <param name="inputStream"> the input stream of the DMN model instance </param>
        /// <returns> this DmnTransform </returns>
        IDmnTransform modelInstance(Stream inputStream);

        ///// <summary>
        ///// Set the DMN model instance to transform.
        ///// </summary>
        ///// <param name="modelInstance"> the DMN model instance </param>
        //DmnModelInstance ModelInstance { set; }

        /// <summary>
        ///     Set the DMN model instance to transform.
        /// </summary>
        /// <param name="modelInstance"> the DMN model instance </param>
        /// <returns> this DmnTransform </returns>
        IDmnTransform modelInstance(IDmnModelInstance modelInstance);

        /// <summary>
        ///     Transform all decisions of the DMN model instance.
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        List<T> transformDecisions<T>() where T : IDmnDecision;

        /// <summary>
        ///     Transform the decision requirements graph and all containing decisions of
        ///     the DMN model instance.
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        T transformDecisionRequirementsGraph<T>() where T : IDmnDecisionRequirementsGraph;
    }
}