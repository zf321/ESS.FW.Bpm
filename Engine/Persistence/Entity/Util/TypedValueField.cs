using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using Newtonsoft.Json;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Util
{
    /// <summary>
    ///     A field what provide a typed version of a value. It can
    ///     be used in an entity which implements <seealso cref="valueFields" />.
    ///     
    /// </summary>
    public class TypedValueField : IDbEntityLifecycleAware, ICommandContextListener
    {
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        protected internal readonly IValueFields valueFields;

        protected internal ITypedValue cachedValue;

        protected internal string errorMessage;

        protected internal bool isNotifyOnImplicitUpdates;
        protected internal ITypedValueSerializer serializer;

        protected internal string serializerName;
        protected internal IList<ITypedValueUpdateListener> updateListeners;

        public TypedValueField(IValueFields valueFields, bool notifyOnImplicitUpdates)
        {
            this.valueFields = valueFields;
            this.isNotifyOnImplicitUpdates = notifyOnImplicitUpdates;
            updateListeners = new List<ITypedValueUpdateListener>();
        }

        public virtual ITypedValue TypedValue
        {
            get { return GetTypedValue(true); }
        }

        public virtual bool Mutable
        {
            get { return IsMutableValue(cachedValue); }
        }

        protected internal virtual bool ValuedImplicitlyUpdated
        {
            get
            {
                if ((cachedValue != null) && IsMutableValue(cachedValue))
                {
                    var byteArray = valueFields.ByteArrayValue;

                    var tempValueFields = new ValueFieldsImpl();
                    WriteValue(cachedValue, tempValueFields);

                    var byteArrayAfter = tempValueFields.ByteArrayValue;

                    return byteArray != byteArrayAfter;
                }

                return false;
            }
        }
        
        public virtual ITypedValueSerializer Serializer
        {
            get
            {
                EnsureSerializerInitialized();
                return serializer;
            }
        }

        public static IVariableSerializers Serializers
        {
            get
            {
                    var variableSerializers = Context.VariableSerializers;
                    var paSerializers = CurrentPaSerializers;

                    if (paSerializers != null)
                        return variableSerializers.Join(paSerializers);
                    return variableSerializers;
            }
        }

        protected internal static IVariableSerializers CurrentPaSerializers
        {
            get
            {
                if (Context.CurrentProcessApplication != null)
                {
                    var processApplicationReference = Context.CurrentProcessApplication;
                    try
                    {
                        var processApplicationInterface = processApplicationReference.ProcessApplication;

                        var rawPa = processApplicationInterface.RawObject;
                        if (rawPa is AbstractProcessApplication)
                            throw new NotImplementedException();
                        return null;
                    }
                    catch (ProcessApplicationUnavailableException e)
                    {
                        throw Log.CannotDeterminePaDataformats(e);
                    }
                }
                return null;
            }
        }

        public virtual string SerializerName
        {
            get { return serializerName; }
            set { serializerName = value; }
        }

        /// <returns> the type name of the value </returns>
        public virtual string TypeName
        {
            get
            {
                if (serializerName == null)
                    return "null"; //ITypedValue.NULL.Name;
                return Serializer.GetType().ToString(); //.Name;
            }
        }

        /// <summary>
        ///     If the variable value could not be loaded, this returns the error message.
        /// </summary>
        /// <returns> an error message indicating why the variable value could not be loaded. </returns>
        public virtual string ErrorMessage
        {
            get { return errorMessage; }
        }

        public void OnCommandContextClose(CommandContext commandContext)
        {
            if (ValuedImplicitlyUpdated)
                foreach (var typedValueImplicitUpdateListener in updateListeners)
                    typedValueImplicitUpdateListener.OnImplicitValueUpdate(cachedValue);
        }

        public void OnCommandFailed(CommandContext commandContext, System.Exception t)
        {
            // ignore
        }

        public void PostLoad()
        {
            // make sure the serializer is initialized
            EnsureSerializerInitialized();
        }

        public virtual object GetValue()
        {
            var typedValue = GetTypedValue();
            if (typedValue != null)
            {
                var r = typedValue.Value;
                if(r is UntypedValueImpl)
                {
                    return ((UntypedValueImpl)r).Value;
                }
                return typedValue.Value;
            }
               
            return null;
        }

        public virtual ITypedValue GetTypedValue(bool deserializeValue)
        {
            if ((cachedValue != null) && cachedValue is ISerializableValue && (Context.CommandContext != null))
            {
                ISerializableValue serializableValue = (ISerializableValue) cachedValue;
                if (deserializeValue && !serializableValue.IsDeserialized)
                {
                    // clear cached value in case it is not deserialized and user requests deserialized value
                    cachedValue = null;
                }
            }

            if ((cachedValue == null) && (errorMessage == null))
                //try
                {
                    cachedValue = Serializer.ReadValue(valueFields, deserializeValue);

                    if (isNotifyOnImplicitUpdates && IsMutableValue(cachedValue)&& Context.CommandContext!= null)
                    {
                        Context.CommandContext.RegisterCommandContextListener(this);
                    }
                       
                //}
               // catch (Exception e)
                //{
                    // intercept the error message
                //    errorMessage = e.Message;
                //    throw e;
                }
            return cachedValue;
        }

        public virtual ITypedValue GetTypedValue()
        {
            return GetTypedValue(true);
        }

        public virtual ITypedValue SetValue(ITypedValue value)
        {
            // determine serializer to use
            serializer = Serializers.FindSerializerForValue(value,
                Context.ProcessEngineConfiguration.FallbackSerializerFactory);
            serializerName = serializer.Name;

            if (value is UntypedValueImpl)
                value = serializer.ConvertToTypedValue((UntypedValueImpl)value);

            //// set new value
            WriteValue(value, valueFields);

            // cache the value
            cachedValue = value;

            // ensure that we serialize the object on command context flush
            // if it can be implicitly changed
            if (isNotifyOnImplicitUpdates && IsMutableValue(cachedValue))
                Context.CommandContext.RegisterCommandContextListener(this);

            return value;
        }
        protected internal virtual bool IsMutableValue(ITypedValue value)
        {
            return serializer.IsMutableValue(value);
        }
        
        protected internal virtual void WriteValue(ITypedValue value, IValueFields valueFields)
        {
            serializer.WriteValue(value, valueFields);
        }

        protected internal virtual void EnsureSerializerInitialized()
        {
            if ((serializerName != null) && (serializer == null))
            {
                serializer = Serializers.GetSerializerByName(serializerName);

                if (serializer == null)
                    serializer = GetFallbackSerializer(serializerName);

                if (serializer == null)
                    throw Log.SerializerNotDefinedException(this);
            }
        }

        public static ITypedValueSerializer GetFallbackSerializer(string serializerName)
        {
            if (Context.ProcessEngineConfiguration != null)
            {
                var fallbackSerializerFactory = Context.ProcessEngineConfiguration.FallbackSerializerFactory;
                if (fallbackSerializerFactory != null)
                    return fallbackSerializerFactory.GetSerializer<ITypedValue>(serializerName);
                return null;
            }
            throw Log.SerializerOutOfContextException();
        }


        public virtual void AddImplicitUpdateListener(ITypedValueUpdateListener listener)
        {
            updateListeners.Add(listener);
        }

        public virtual void Clear()
        {
            cachedValue = null;
        }
    }
}