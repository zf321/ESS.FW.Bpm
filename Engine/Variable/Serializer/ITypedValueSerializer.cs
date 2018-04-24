using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using System;
using ESS.FW.Bpm.Engine.Variable.Type;

namespace ESS.FW.Bpm.Engine.Variable.Serializer
{

    /// <summary>
    /// A <seealso cref="ITypedValueSerializer{T}"/> persists <seealso cref="ITypedValue TypedValues"/> of a given
    /// <seealso cref="ValueType"/> to provided <seealso cref="IValueFields"/>.
    /// <para>
    /// Replaces the "VariableType" interface in previous versions.
    /// 
    /// 
    /// </para>
    /// </summary>
    public interface ITypedValueSerializer
    {

        /// <summary>
        /// The name of this serializer. The name is used when persisting the IValueFields populated by this serializer.
        /// </summary>
        /// <returns> the name of this serializer. </returns>
        string Name { get; }

        /// <summary>
        /// The <seealso cref="IValueType VariableType"/> supported </summary>
        /// <returns> the VariableType supported </returns>
        IValueType Type { get; }

        /// <summary>
        /// Serialize a <seealso cref="ITypedValue"/> to the <seealso cref="IValueFields"/>.
        /// </summary>
        /// <param name="value"> the <seealso cref="ITypedValue"/> to persist </param>
        /// <param name="valueFields"> the <seealso cref="IValueFields"/> to which the value should be persisted </param>
        void WriteValue(ITypedValue value, IValueFields valueFields);

        /// <summary>
        /// Retrieve a <seealso cref="ITypedValue"/> from the provided <seealso cref="IValueFields"/>.
        /// </summary>
        /// <param name="valueFields"> the <seealso cref="IValueFields"/> to retrieve the value from </param>
        /// <param name="deserializeValue"> indicates whether a <seealso cref="SerializableValue"/> should be deserialized.
        /// </param>
        /// <returns> the <seealso cref="ITypedValue"/> </returns>
        ITypedValue ReadValue(IValueFields valueFields, bool deserializeValue);

        /// <summary>
        /// Used for auto-detecting the value type of a variable.
        /// An implementation must return true if it is able to write values of the provided type.
        /// </summary>
        /// <param name="value"> the value </param>
        /// <returns> true if this <seealso cref="ITypedValueSerializer{T}"/> is able to handle the provided value </returns>
        bool CanHandle(ITypedValue value);

        /// <summary>
        /// Returns a typed value for the provided untyped value. This is used on cases where the user sets an untyped
        /// value which is then detected to be handled by this <seealso cref="ITypedValueSerializer{T}"/> (by invocation of <seealso cref="#canHandle(ITypedValue)"/>).
        /// </summary>
        /// <param name="untypedValue"> the untyped value </param>
        /// <returns> the corresponding typed value </returns>
        ITypedValue ConvertToTypedValue(UntypedValueImpl untypedValue);

        /// 
        /// <returns> the dataformat used by the serializer or null if this is not an object serializer </returns>
        string SerializationDataFormat { get; }

        /// <returns> whether values serialized by this serializer can be mutable and
        /// should be re-serialized if changed </returns>
        bool IsMutableValue(ITypedValue typedValue);

    }
}