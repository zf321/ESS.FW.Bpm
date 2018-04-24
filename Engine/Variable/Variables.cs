using System;
using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Bpm.Engine.Variable.Context.Impl;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Builder;
using ESS.FW.Bpm.Engine.Variable.Value.Builder.Impl;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using Newtonsoft.Json.Linq;

namespace ESS.FW.Bpm.Engine.Variable
{


    /// <summary>
	/// <para>This class is the entry point to the process engine's typed variables API.
	/// Users can import the methods provided by this class using a static import:</para>
	/// 
	/// <code>
	/// import static org.camunda.bpm.engine.variable.Variables.*;
	/// </code>
	/// 
	/// 
	/// </summary>
	public class Variables
    {

        /// <summary>
        /// <para>A set of builtin serialization dataformat constants. These constants can be used to specify
        /// how java object variables should be serialized by the process engine:</para>
        /// 
        /// <pre>
        /// CustomerData customerData = new CustomerData();
        /// // ...
        /// ObjectValue customerDataValue = Variables.objectValue(customerData)
        ///   .serializationDataFormat(Variables.SerializationDataFormats.JSON)
        ///   .create();
        /// 
        /// execution.setVariable("someVariable", customerDataValue);
        /// </pre>
        /// 
        /// <para>Note that not all of the formats provided here are supported out of the box.</para>
        /// 
        /// 
        /// </summary>
        public enum SerializationDataFormats
        {

            /// <summary>
            /// <para>The Java Serialization Data format. If this data format is used for serializing an object,
            /// the object is serialized using default Java <seealso cref="Serializable"/>.</para>
            /// 
            /// <para>The process engine provides a serializer for this dataformat out of the box.</para>
            /// </summary>
            Net,//("application/x-java-serialized-object"),

            /// <summary>
            /// <para>The Json Serialization Data format. If this data format is used for serializing an object,
            /// the object is serialized as Json text.</para>
            /// 
            /// <para><strong>NOTE:</strong> the process does NOT provide a serializer for this dataformat out of the box.
            /// If you want to serialize objects using the Json dataformat, you need to provide a serializer. The optinal
            /// camunda Spin process engine plugin provides such a serializer.</para>
            /// </summary>
            Json,//("application/json"),
                 /// <summary>
                 /// <para>The Xml Serialization Data format. If this data format is used for serializing an object,
                 /// the object is serialized as Xml text.</para>
                 /// 
                 /// <para><strong>NOTE:</strong> the process does NOT provide a serializer for this dataformat out of the box.
                 /// If you want to serialize objects using the Xml dataformat, you need to provide a serializer. The optinal
                 /// camunda Spin process engine plugin provides such a serializer.</para>
                 /// </summary>
            Xml//("application/xml");

        }
        /// <summary>
        /// Returns a new <seealso cref="IVariableMap"/> instance.
        /// </summary>
        public static IVariableMap CreateVariables()
        {
            return new Variable.VariableMapImpl();
        }
        /// <summary>
        /// If the given map is not a variable map, adds all its entries as untyped
        /// values to a new <seealso cref="IVariableMap"/>. If the given map is a <seealso cref="IVariableMap"/>,
        /// it is returned as is.
        /// </summary>
        public static IVariableMap FromMap(IDictionary<string, ITypedValue> map)
        {
            if (map is IVariableMap)
            {
                return (IVariableMap)map;
            }
            else
            {
                return new Variable.VariableMapImpl(map);
            }
        }
        public static IVariableMap FromMap(IDictionary<string, object> map)
        {
            return new VariableMapImpl(map);
        }
        /// <summary>
        /// Shortcut for {@code Variables.createVariables().putValue(name, value)}
        /// </summary>
        public static IVariableMap PutValue(string name, object value)
        {
            return CreateVariables().PutValue(name, value);
        }
        /// <summary>
        /// Shortcut for {@code Variables.createVariables().putValueTyped(name, value)}
        /// </summary>
        public static IVariableMap PutValueTyped(string name, ITypedValue value)
        {
            return CreateVariables().PutValueTyped(name, value);
        }
        /// <summary>
        /// Returns a builder to create a new <seealso cref="IObjectValue"/> that encapsulates
        /// the given {@code value}.
        /// </summary>
        public static IObjectValueBuilder ObjectValue(object value)
        {
            return new ObjectVariableBuilderImpl(value);
        }
        /// <summary>
        /// Returns a builder to create a new <seealso cref="IObjectValue"/> from a serialized
        /// object representation.
        /// </summary>
        public static ISerializedObjectValueBuilder SerializedObjectValue()
        {
            return new SerializedObjectValueBuilderImpl();
        }
        /// <summary>
        /// Shortcut for {@code Variables.serializedObjectValue().serializedObjectValue(value)}
        /// </summary>
        public static ISerializedObjectValueBuilder SerializedObjectValue(string value)
        {
            return SerializedObjectValue().SerializedValue(value);
        }
        /// <summary>
        /// Creates a new <seealso cref="INtegerValue"/> that encapsulates the given <code>integer</code>
        /// </summary>
        public static IIntegerValue IntegerValue(int? integer)
        {
            return new IntegerValueImpl(integer);
        }
        /// <summary>
        /// Creates a new <seealso cref="IStringValue"/> that encapsulates the given <code>stringValue</code>
        /// </summary>
        public static IStringValue StringValue(string stringValue)
        {
            return new StringValueImpl(stringValue);
        }
        /// <summary>
        /// Creates a new <seealso cref="IBooleanValue"/> that encapsulates the given <code>booleanValue</code>
        /// </summary>
        public static IBooleanValue BooleanValue(bool? booleanValue)
        {
            return new BooleanValueImpl(booleanValue);
        }
        /// <summary>
        /// Creates a new <seealso cref="IBytesValue"/> that encapsulates the given <code>bytes</code>
        /// </summary>
        public static IBytesValue ByteArrayValue(byte[] bytes)
        {
            return new BytesValueImpl(bytes);
        }
        /// <summary>
        /// Creates a new <seealso cref="IDateValue"/> that encapsulates the given <code>date</code>
        /// </summary>
        public static IDateValue DateValue(DateTime date)
        {
            return new DateValueImpl(date);
        }
        /// <summary>
        /// Creates a new <seealso cref="ILongValue"/> that encapsulates the given <code>longValue</code>
        /// </summary>
        public static ILongValue LongValue(long? longValue)
        {
            return new LongValueImpl(longValue);
        }
        /// <summary>
        /// Creates a new <seealso cref="IShortValue"/> that encapsulates the given <code>shortValue</code>
        /// </summary>
        public static IShortValue ShortValue(short? shortValue)
        {
            return new ShortValueImpl(shortValue);
        }
        /// <summary>
        /// Creates a new <seealso cref="IDoubleValue"/> that encapsulates the given <code>doubleValue</code>
        /// </summary>
        public static IDoubleValue DoubleValue(double? doubleValue)
        {
            return new DoubleValueImpl(doubleValue);
        }
        /// <summary>
        /// Creates an abstract Number value. Note that this value cannot be used to set variables.
        /// Use the specific methods <seealso cref="Variables#integerValue(Integer)"/>, <seealso cref="#shortValue(Short)"/>,
        /// <seealso cref="#longValue(Long)"/> and <seealso cref="#doubleValue(Double)"/> instead.
        /// </summary>
        public static INumberValue NumberValue(decimal numberValue)
        {
            return new NumberValueImpl(numberValue);
        }
        /// <summary>
        /// Creates a <seealso cref="ITypedValue"/> with value {@code null} and type <seealso cref="IValueType#NULL"/>
        /// </summary>
        public static ITypedValue UntypedNullValue()
        {
            return NullValueImpl.Instance;
        }
        /// <summary>
        /// Creates an untyped value, i.e. <seealso cref="ITypedValue#getType()"/> returns <code>null</code>
        /// for the returned instance.
        /// </summary>
        public static ITypedValue UnTypedValue(object value)
        {
            if (value == null)
            {
                return UntypedNullValue();
            }
            else if (value is ITypedValueBuilder<object>)
            {
                return (ITypedValue)((ITypedValueBuilder<object>)value).Create();
            }
            else if (value is ITypedValue)
            {
                //var v= (ITypedValue)value;
                //if (v.Type == null && v.Value is ITypedValue && ((ITypedValue)v.Value).Type != null)
                //{
                //    return ((ITypedValue)v.Value);
                //}
                return (ITypedValue)value;
            }
            else if(value is JObject)
            {
                var jo = value as JObject;
                var t = jo["Type"].ToObject(jo["Type"]["$type"].ToObject<System.Type>()) as IValueType;
                if (t.IsPrimitiveValueType)
                {
                    var pt = t as IPrimitiveValueType;
                    if (pt.NetType == typeof(string))
                    {
                        return new StringValueImpl(jo["Value"].ToObject<string>());
                    }
                    else if (pt.NetType == typeof(decimal))
                    {
                        return new NumberValueImpl(jo["Value"].ToObject<decimal>());
                    }
                    else if (pt.NetType == typeof(bool))
                    {
                        return new BooleanValueImpl(jo["Value"].ToObject<bool>());
                    }
                    else if (pt.NetType == typeof(double))
                    {
                        return new DoubleValueImpl(jo["Value"].ToObject<double>());
                    }
                    else if (pt.NetType == typeof(int))
                    {
                        return new IntegerValueImpl(jo["Value"].ToObject<int>());
                    }
                    else if (pt.NetType == typeof(long))
                    {
                        return new LongValueImpl(jo["Value"].ToObject<long>());
                    }
                    else if (pt.NetType == typeof(DateTime))
                    {
                        return new DateValueImpl(jo["Value"].ToObject<DateTime>());
                    }
                }
                return new ObjectValueImpl(jo["Value"].ToObject(jo["ObjectType"].ToObject<System.Type>()));
            }
            else
            {
                // unknown value
                return new UntypedValueImpl(value);
            }
        }
        /// <summary>
        /// Returns a builder to create a new <seealso cref="IFileValue"/> with the given
        /// {@code filename}.
        /// </summary>
        public static IFileValueBuilder FileValue(string filename)
        {
            return new FileValueBuilderImpl(filename);
        }
        /// <summary>
        /// Shortcut for calling {@code Variables.fileValue(name).file(file).mimeType(type).create()}.
        /// The name is set to the file name and the mime type is detected via <seealso cref="MimetypesFileTypeMap"/>.
        /// </summary>
        public static IFileValue FileValue(FileInfo file)
        {
            return null;
            //string contentType = MimetypesFileTypeMap.DefaultFileTypeMap.getContentType(file);
            //return (new FileValueBuilderImpl(file.Name)).file(file).MimeType(contentType).Create();
        }
        /// <returns> an empty <seealso cref="IVariableContext"/> (from which no variables can be resolved). </returns>
        public static IVariableContext emptyVariableContext()
        {
            return EmptyVariableContext.Instance;
        }
    }
}