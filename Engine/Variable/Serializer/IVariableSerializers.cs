using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Variable.Serializer
{

    /// <summary>
    /// Interface describing a container for all available <seealso cref="TypedValueSerializer"/>s of variables.
    /// 
    /// 
    /// </summary>
    public interface IVariableSerializers
    {

        /// <summary>
        /// Selects the <seealso cref="TypedValueSerializer"/> which should be used for persisting a VariableValue.
        /// </summary>
        /// <param name="value"> the value to persist </param>
        /// <param name="fallBackSerializerFactory"> a factory to build a fallback serializer in case no suiting serializer
        ///   can be determined. If this factory is not able to build serializer either, an exception is thrown. May be null </param>
        /// <returns> the VariableValueserializer selected for persisting the value or 'null' in case no serializer can be found </returns>
        ITypedValueSerializer FindSerializerForValue(ITypedValue value, IVariableSerializerFactory fallBackSerializerFactory);

        /// <summary>
        /// Same as calling <seealso cref="IVariableSerializers#findSerializerForValue(ITypedValue, VariableSerializerFactory)"/>
        /// with no fallback serializer factory.
        /// </summary>
        ITypedValueSerializer FindSerializerForValue(ITypedValue value);

        /// 
        /// <returns> the serializer for the given serializerName name.
        /// Returns null if no type was found with the name. </returns>
        ITypedValueSerializer GetSerializerByName(string serializerName);

        IVariableSerializers AddSerializer(ITypedValueSerializer serializer);

        /// <summary>
        /// Add type at the given index. The index is used when finding a serializer for a VariableValue. When
        /// different serializers can store a specific variable value, the one with the smallest
        /// index will be used.
        /// </summary>
        IVariableSerializers AddSerializer(ITypedValueSerializer serializer, int index);

        IVariableSerializers RemoveSerializer(ITypedValueSerializer serializer);

        //int GetSerializerIndex(ITypedValueSerializer<ITypedValue> serializer);

        //int GetSerializerIndexByName(string serializerName);

        /// <summary>
        /// Merges two <seealso cref="IVariableSerializers"/> instances into one. Implementations may apply
        /// different merging strategies.
        /// </summary>
        IVariableSerializers Join(IVariableSerializers other);

        /// <summary>
        /// Returns the serializers as a list in the order of their indices.
        /// </summary>
        IList<ITypedValueSerializer> Serializers { get; }

    }
}