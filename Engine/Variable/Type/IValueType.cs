using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Type.Impl;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Variable.Type
{
    /// <summary>
    ///     
    /// </summary>
    public interface IValueType
    {
        /// <summary>
        ///     Returns the name of the variable type
        /// </summary>
        string Name { get; }
        

        /// <summary>
        ///     <para>Gets the parent value type.</para>
        ///     <para>
        ///         Value type hierarchy is only relevant for queries and has the
        ///         following meaning: When a value query is made
        ///         (e.g. all tasks with a certain variable value), a "child" type's value
        ///         also matches a parameter value of the parent type. This is only
        ///         supported when the parent value type's implementation of <seealso cref="#isAbstract()" />
        ///         returns <code>true</code>.
        ///     </para>
        /// </summary>
        IValueType Parent { get; }

        /// <summary>
        ///     <para>
        ///         Returns whether the value type is abstract. This is
        ///         <b>
        ///             not related
        ///             to the term <i>abstract</i> in the Java language.
        ///         </b>
        ///     </para>
        ///     Abstract value types cannot be used as types for variables but only used for querying.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        ///     Indicates whether this type is primitive valued. Primitive valued types can be handled
        ///     natively by the process engine.
        /// </summary>
        /// <returns> true if this is a primitive valued type. False otherwise </returns>
        bool IsPrimitiveValueType { get; }

        /// <summary>
        ///     Get the value info (meta data) for a <seealso cref="ITypedValue" />.
        ///     The keys of the returned map for a <seealso cref="ITypedValue" /> are available
        ///     as constants in the value's <seealso cref="IValueType" /> interface.
        /// </summary>
        /// <param name="typedValue">
        ///     @return
        /// </param>
        IDictionary<string, object> GetValueInfo(ITypedValue typedValue);

        /// <summary>
        ///     Creates a new TypedValue using this type.
        /// </summary>
        /// <param name="value"> the value </param>
        /// <returns> the typed value for the value </returns>
        ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo);

        /// <summary>
        ///     Determines whether the argument typed value can be converted to a
        ///     typed value of this value type.
        /// </summary>
        bool CanConvertFromTypedValue(ITypedValue typedValue);

        /// <summary>
        ///     Converts a typed value to a typed value of this type.
        ///     This does not suceed if <seealso cref="#canConvertFromTypedValue(ITypedValue)" />
        ///     returns <code>false</code>.
        /// </summary>
        ITypedValue ConvertFromTypedValue(ITypedValue typedValue);
    }

    public static class ValueTypeFields
    {
        public static readonly IPrimitiveValueType Null = new NullTypeImpl();
        public static readonly IPrimitiveValueType Boolean = new BooleanTypeImpl();
        public static readonly IPrimitiveValueType Short = new ShortTypeImpl();
        public static readonly IPrimitiveValueType Long = new LongTypeImpl();
        public static readonly IPrimitiveValueType Double = new DoubleTypeImpl();
        public static readonly IPrimitiveValueType String = new StringTypeImpl();
        public static readonly IPrimitiveValueType Integer = new IntegerTypeImpl();
        public static readonly IPrimitiveValueType Date = new DateTypeImpl();
        public static readonly IPrimitiveValueType Bytes = new BytesTypeImpl();
        public static readonly IPrimitiveValueType Number = new NumberTypeImpl();
        public static readonly ISerializableValueType Object = new ObjectTypeImpl();
        public static readonly IFileValueType File = new FileValueTypeImpl();
    }
}