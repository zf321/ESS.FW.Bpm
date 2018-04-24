using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security;

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    /// Defines property resolution behavior on objects using the Object component architecture. This
    /// resolver handles base objects of any type, as long as the base is not null. It accepts any object
    /// as a property, and coerces it to a string. That string is then used to find a Object compliant
    /// property on the base object. The value is accessed using Object getters and setters. This
    /// resolver can be constructed in read-only mode, which means that isReadOnly will always return
    /// true and <seealso cref=".setValue(ELContext, Object, Object, Object)"/> will always throw
    /// PropertyNotWritableException. ELResolvers are combined together using <seealso cref="CompositeELResolver"/>
    /// s, to define rich semantics for evaluating an expression. See the javadocs for <seealso cref="ELResolver"/>
    /// for details. Because this resolver handles base objects of any type, it should be placed near the
    /// end of a composite resolver. Otherwise, it will claim to have resolved a property before any
    /// resolvers that come after it get a chance to test if they can do so as well.
    /// </summary>
    /// <seealso cref= "CompositeELResolver"> </seealso>
    /// <seealso cref= "ELResolver"> </seealso>
    public class ObjectELResolver : ELResolver
    {
        protected internal sealed class ObjectProperties
        {
            internal readonly IDictionary<string, ObjectProperty> map = new Dictionary<string, ObjectProperty>();

            public ObjectProperties(Type baseClass)
            {
                PropertyInfo[] descriptors;
                try
                {
                    descriptors = baseClass.GetProperties();
                }
                catch (System.Exception e)
                {
                    throw new ELException(e);
                }
                foreach (PropertyInfo descriptor in descriptors)
                {
                    map[descriptor.Name] = new ObjectProperty(descriptor);
                }
            }

            public ObjectProperty GetObjectProperty(string property)
            {
                ObjectProperty p;
                map.TryGetValue(property,out p);
                return p;
            }
        }

        protected internal sealed class ObjectProperty
        {
            internal readonly PropertyInfo descriptor;

            public ObjectProperty(PropertyInfo descriptor)
            {
                this.descriptor = descriptor;
            }

            public Type PropertyType
            {
                get
                {
                    return descriptor.PropertyType;
                }
            }

            public MethodInfo ReadMethod
            {
                get
                {
                    return FindAccessibleMethod(descriptor.GetMethod);
                }
            }

            public MethodInfo WriteMethod
            {
                get
                {
                    return FindAccessibleMethod(descriptor.SetMethod);
                }
            }

            public bool ReadOnly
            {
                get
                {
                    return FindAccessibleMethod(descriptor.SetMethod) == null;
                }
            }
        }

        private static MethodInfo FindAccessibleMethod(MethodInfo method)
        {
            //if (method == null || method.Accessible)
            //{
            //    return method;
            //}
            //try
            //{
            //    method.Accessible = true;
            //}
            //catch (SecurityException)
            //{
            //    foreach (Type cls in method.DeclaringClass.Interfaces)
            //    {
            //        MethodInfo mth = null;
            //        try
            //        {
            //            mth = cls.GetMethod(method.Name, method.ParameterTypes);
            //            mth = FindAccessibleMethod(mth);
            //            if (mth != null)
            //            {
            //                return mth;
            //            }
            //        }
            //        catch (MethodNotFoundException)
            //        {
            //            // do nothing
            //        }
            //    }
            //    Type cls = method.DeclaringClass.BaseType;
            //    if (cls != null)
            //    {
            //        MethodInfo mth = null;
            //        try
            //        {
            //            mth = cls.GetMethod(method.Name, method.ParameterTypes);
            //            mth = FindAccessibleMethod(mth);
            //            if (mth != null)
            //            {
            //                return mth;
            //            }
            //        }
            //        catch (MethodNotFoundException)
            //        {
            //            // do nothing
            //        }
            //    }
            //    return null;
            //}
            return method;
        }

        private readonly bool readOnly;

        private static readonly ConcurrentDictionary<Type, ObjectProperties> cache =
            new ConcurrentDictionary<Type, ObjectProperties>();

        private ExpressionFactory defaultFactory;

        /// <summary>
        /// Creates a new read/write ObjectELResolver.
        /// </summary>
        public ObjectELResolver() : this(false)
        {
        }

        /// <summary>
        /// Creates a new ObjectELResolver whose read-only status is determined by the given parameter.
        /// </summary>
        public ObjectELResolver(bool readOnly)
        {
            this.readOnly = readOnly;
        }

        /// <summary>
        /// If the base object is not null, returns the most general type that this resolver accepts for
        /// the property argument. Otherwise, returns null. Assuming the base is not null, this method
        /// will always return Object.class. This is because any object is accepted as a key and is
        /// coerced into a string.
        /// </summary>
        /// <param name="context">
        ///            The context of this evaluation. </param>
        /// <param name="base">
        ///            The Object to analyze. </param>
        /// <returns> null if base is null; otherwise Object.class. </returns>
        public override Type GetCommonPropertyType(ELContext context, object @base)
        {
            return IsResolvable(@base) ? typeof(object) : null;
        }

        /// <summary>
        /// If the base object is not null, returns an Iterator containing the set of Object
        /// properties available on the given object. Otherwise, returns null. The Iterator returned must
        /// contain zero or more instances of java.Objects.MemberInfo. Each info object contains
        /// information about a property in the Object, as obtained by calling the
        /// ObjectInfo.getPropertyInfos method. The MemberInfo is initialized using the same
        /// fields as are present in the PropertyInfo, with the additional required named
        /// attributes "type" and "resolvableAtDesignTime" set as follows:
        /// <ul>
        /// <li><seealso cref="ELResolver.TYPE"/> - The runtime type of the property, from
        /// PropertyInfo.getPropertyType().</li>
        /// <li><seealso cref="ELResolver.RESOLVABLE_AT_DESIGN_TIME"/> - true.</li>
        /// </ul>
        /// </summary>
        /// <param name="context">
        ///            The context of this evaluation. </param>
        /// <param name="base">
        ///            The Object to analyze. </param>
        /// <returns> An Iterator containing zero or more MemberInfo objects, each representing a
        ///         property on this Object, or null if the base object is null. </returns>
        public override IEnumerator<MemberInfo> GetMemberInfos(ELContext context, object @base)
        {
            if (IsResolvable(@base))
            {
                PropertyInfo[] properties;
                try
                {
                    properties = (@base.GetType()).GetProperties();
                }
                catch (System.Exception e)
                {
                    return System.Linq.Enumerable.Empty<MemberInfo>().GetEnumerator();
                }
                return new IteratorAnonymousInnerClass(this, properties);
            }
            return null;
        }

        private class IteratorAnonymousInnerClass : IEnumerator<PropertyInfo>
        {
            private readonly ObjectELResolver outerInstance;

            private PropertyInfo[] properties;

            public IteratorAnonymousInnerClass(ObjectELResolver outerInstance, PropertyInfo[] properties)
            {
                this.outerInstance = outerInstance;
                this.properties = properties;
                next = 0;
            }

            internal int next;

            public PropertyInfo Current {
               get { return properties?[next]; }
            }

            object IEnumerator.Current => properties?[next];

            public virtual bool HasNext()
            {
                return properties != null && next < properties.Length;
            }

            public virtual bool  MoveNext()
            {
                if (HasNext())
                {
                    next++;
                    return true;
                }
                return false;
                //PropertyInfo property = properties[next++];
                //MemberInfo feature = new MemberInfo();
                //feature.Name.DisplayName = property.DisplayName;
                //feature.Name = property.Name;
                //feature.ShortDescription = property.ShortDescription;
                //feature.Expert = property.Expert;
                //feature.Hidden = property.Hidden;
                //feature.Preferred = property.Preferred;
                //feature.SetValue(TYPE, property.PropertyType);
                //feature.SetValue(RESOLVABLE_AT_DESIGN_TIME, true);
                //return feature;
            }
            public virtual void Remove()
            {
                throw new System.NotSupportedException("cannot remove");
            }

            public void Dispose()
            {
                properties = null;
                next = 0;
            }

            public void Reset()
            {
                next = 0;
            }
        }

        /// <summary>
        /// If the base object is not null, returns the most general acceptable type that can be set on
        /// this Object property. If the base is not null, the propertyResolved property of the ELContext
        /// object must be set to true by this resolver, before returning. If this property is not true
        /// after this method is called, the caller should ignore the return value. The provided property
        /// will first be coerced to a String. If there is a ObjectInfoProperty for this property and there
        /// were no errors retrieving it, the propertyType of the PropertyInfo is returned.
        /// Otherwise, a PropertyNotFoundException is thrown.
        /// </summary>
        /// <param name="context">
        ///            The context of this evaluation. </param>
        /// <param name="base">
        ///            The Object to analyze. </param>
        /// <param name="property">
        ///            The name of the property to analyze. Will be coerced to a String. </param>
        /// <returns> If the propertyResolved property of ELContext was set to true, then the most general
        ///         acceptable type; otherwise undefined. </returns>
        /// <exception cref="NullPointerException">
        ///             if context is null </exception>
        /// <exception cref="PropertyNotFoundException">
        ///             if base is not null and the specified property does not exist or is not readable. </exception>
        /// <exception cref="ELException">
        ///             if an exception was thrown while performing the property or variable resolution.
        ///             The thrown exception must be included as the cause property of this exception, if
        ///             available. </exception>
        public override Type GetType(ELContext context, object @base, object property)
        {
            if (context == null)
            {
                throw new System.NullReferenceException();
            }
            Type result = null;
            if (IsResolvable(@base))
            {
                result = ToObjectProperty(@base, property).PropertyType;
                context.PropertyResolved = true;
            }
            return result;
        }

        /// <summary>
        /// If the base object is not null, returns the current value of the given property on this Object.
        /// If the base is not null, the propertyResolved property of the ELContext object must be set to
        /// true by this resolver, before returning. If this property is not true after this method is
        /// called, the caller should ignore the return value. The provided property name will first be
        /// coerced to a String. If the property is a readable property of the base object, as per the
        /// Object specification, then return the result of the getter call. If the getter throws an
        /// exception, it is propagated to the caller. If the property is not found or is not readable, a
        /// PropertyNotFoundException is thrown.
        /// </summary>
        /// <param name="context">
        ///            The context of this evaluation. </param>
        /// <param name="base">
        ///            The Object to analyze. </param>
        /// <param name="property">
        ///            The name of the property to analyze. Will be coerced to a String. </param>
        /// <returns> If the propertyResolved property of ELContext was set to true, then the value of the
        ///         given property. Otherwise, undefined. </returns>
        /// <exception cref="NullPointerException">
        ///             if context is null </exception>
        /// <exception cref="PropertyNotFoundException">
        ///             if base is not null and the specified property does not exist or is not readable. </exception>
        /// <exception cref="ELException">
        ///             if an exception was thrown while performing the property or variable resolution.
        ///             The thrown exception must be included as the cause property of this exception, if
        ///             available. </exception>
        public override object GetValue(ELContext context, object @base, object property)
        {
            if (context == null)
            {
                throw new System.NullReferenceException();
            }
            object result = null;
            if (IsResolvable(@base))
            {
                MethodInfo method = ToObjectProperty(@base, property).ReadMethod;
                if (method == null)
                {
                    throw new PropertyNotFoundException("Cannot read property " + property);
                }
                try
                {
                    result = method.Invoke(@base,null);
                }
                catch (TargetInvocationException e)
                {
                    throw new ELException(e.InnerException);
                }
                catch (System.Exception e)
                {
                    throw new ELException(e);
                }
                context.PropertyResolved = true;
            }
            return result;
        }

        /// <summary>
        /// If the base object is not null, returns whether a call to
        /// <seealso cref=".setValue(ELContext, Object, Object, Object)"/> will always fail. If the base is not
        /// null, the propertyResolved property of the ELContext object must be set to true by this
        /// resolver, before returning. If this property is not true after this method is called, the
        /// caller can safely assume no value was set.
        /// </summary>
        /// <param name="context">
        ///            The context of this evaluation. </param>
        /// <param name="base">
        ///            The Object to analyze. </param>
        /// <param name="property">
        ///            The name of the property to analyze. Will be coerced to a String. </param>
        /// <returns> If the propertyResolved property of ELContext was set to true, then true if calling
        ///         the setValue method will always fail or false if it is possible that such a call may
        ///         succeed; otherwise undefined. </returns>
        /// <exception cref="NullPointerException">
        ///             if context is null </exception>
        /// <exception cref="PropertyNotFoundException">
        ///             if base is not null and the specified property does not exist or is not readable. </exception>
        /// <exception cref="ELException">
        ///             if an exception was thrown while performing the property or variable resolution.
        ///             The thrown exception must be included as the cause property of this exception, if
        ///             available. </exception>
        public override bool IsReadOnly(ELContext context, object @base, object property)
        {
            if (context == null)
            {
                throw new System.NullReferenceException();
            }
            bool result = readOnly;
            if (IsResolvable(@base))
            {
                result |= ToObjectProperty(@base, property).ReadOnly;
                context.PropertyResolved = true;
            }
            return result;
        }

        /// <summary>
        /// If the base object is not null, attempts to set the value of the given property on this Object.
        /// If the base is not null, the propertyResolved property of the ELContext object must be set to
        /// true by this resolver, before returning. If this property is not true after this method is
        /// called, the caller can safely assume no value was set. If this resolver was constructed in
        /// read-only mode, this method will always throw PropertyNotWritableException. The provided
        /// property name will first be coerced to a String. If property is a writable property of base
        /// (as per the Object Specification), the setter method is called (passing value). If the
        /// property exists but does not have a setter, then a PropertyNotFoundException is thrown. If
        /// the property does not exist, a PropertyNotFoundException is thrown.
        /// </summary>
        /// <param name="context">
        ///            The context of this evaluation. </param>
        /// <param name="base">
        ///            The Object to analyze. </param>
        /// <param name="property">
        ///            The name of the property to analyze. Will be coerced to a String. </param>
        /// <param name="value">
        ///            The value to be associated with the specified key. </param>
        /// <exception cref="NullPointerException">
        ///             if context is null </exception>
        /// <exception cref="PropertyNotFoundException">
        ///             if base is not null and the specified property does not exist or is not readable. </exception>
        /// <exception cref="PropertyNotWritableException">
        ///             if this resolver was constructed in read-only mode, or if there is no setter for
        ///             the property </exception>
        /// <exception cref="ELException">
        ///             if an exception was thrown while performing the property or variable resolution.
        ///             The thrown exception must be included as the cause property of this exception, if
        ///             available. </exception>
        public override void SetValue(ELContext context, object @base, object property, object value)
        {
            if (context == null)
            {
                throw new System.NullReferenceException();
            }
            if (IsResolvable(@base))
            {
                if (readOnly)
                {
                    throw new PropertyNotWritableException("resolver is read-only");
                }
                MethodInfo method = ToObjectProperty(@base, property).WriteMethod;
                if (method == null)
                {
                    throw new PropertyNotWritableException("Cannot write property: " + property);
                }
                try
                {
                    method.Invoke(@base, (object[]) value);
                }
                catch (TargetInvocationException e)
                {
                    throw new ELException("Cannot write property: " + property, e.InnerException);
                }
                catch (AccessViolationException e)
                {
                    throw new PropertyNotWritableException("Cannot write property: " + property, e);
                }
                context.PropertyResolved = true;
            }
        }

        /// <summary>
        /// If the base object is not <code>null</code>, invoke the method, with the given parameters on
        /// this Object. The return value from the method is returned.
        /// 
        /// <para>
        /// If the base is not <code>null</code>, the <code>propertyResolved</code> property of the
        /// <code>ELContext</code> object must be set to <code>true</code> by this resolver, before
        /// returning. If this property is not <code>true</code> after this method is called, the caller
        /// should ignore the return value.
        /// </para>
        /// 
        /// <para>
        /// The provided method object will first be coerced to a <code>String</code>. The methods in the
        /// Object is then examined and an attempt will be made to select one for invocation. If no
        /// suitable can be found, a <code>MethodNotFoundException</code> is thrown.
        /// 
        /// If the given paramTypes is not <code>null</code>, select the method with the given name and
        /// parameter types.
        /// 
        /// Else select the method with the given name that has the same number of parameters. If there
        /// are more than one such method, the method selection process is undefined.
        /// 
        /// Else select the method with the given name that takes a variable number of arguments.
        /// 
        /// Note the resolution for overloaded methods will likely be clarified in a future version of
        /// the spec.
        /// 
        /// The provided parameters are coerced to the corresponding parameter types of the method, and
        /// the method is then invoked.
        /// 
        /// </para>
        /// </summary>
        /// <param name="context">
        ///            The context of this evaluation. </param>
        /// <param name="base">
        ///            The Object on which to invoke the method </param>
        /// <param name="method">
        ///            The simple name of the method to invoke. Will be coerced to a <code>String</code>.
        ///            If method is "&lt;init&gt;"or "&lt;clinit&gt;" a MethodNotFoundException is
        ///            thrown. </param>
        /// <param name="paramTypes">
        ///            An array of Class objects identifying the method's formal parameter types, in
        ///            declared order. Use an empty array if the method has no parameters. Can be
        ///            <code>null</code>, in which case the method's formal parameter types are assumed
        ///            to be unknown. </param>
        /// <param name="params">
        ///            The parameters to pass to the method, or <code>null</code> if no parameters. </param>
        /// <returns> The result of the method invocation (<code>null</code> if the method has a
        ///         <code>void</code> return type). </returns>
        /// <exception cref="MethodNotFoundException">
        ///             if no suitable method can be found. </exception>
        /// <exception cref="ELException">
        ///             if an exception was thrown while performing (base, method) resolution. The thrown
        ///             exception must be included as the cause property of this exception, if available.
        ///             If the exception thrown is an <code>TargetInvocationException</code>, extract its
        ///             <code>cause</code> and pass it to the <code>ELException</code> constructor.
        ///  </exception>
        public override object Invoke(ELContext context, object @base, object method, Type[] paramTypes, object[] @params)
        {
            if (context == null)
            {
                throw new System.NullReferenceException();
            }
            object result = null;
            if (IsResolvable(@base))
            {
                if (@params == null)
                {
                    @params = new object[0];
                }
                string name = method.ToString();
                MethodInfo target = FindMethod(@base, name, paramTypes, @params.Length);
                if (target == null)
                {
                    throw new MethodNotFoundException("Cannot find method " + name + " with " + @params.Length + " parameters in " + @base.GetType());
                }
                try
                {
                    result = target.Invoke(@base, CoerceParams(GetExpressionFactory(context), target, @params));
                }
                catch (TargetInvocationException e)
                {
                    throw new ELException(e.InnerException);
                }
                catch (AccessViolationException e)
                {
                    throw new ELException(e);
                }
                context.PropertyResolved = true;
            }
            return result;
        }

        private MethodInfo FindMethod(object @base, string name, Type[] types, int paramCount)
        {
            if (types != null)
            {
                try
                {
                    return FindAccessibleMethod(@base.GetType().GetMethod(name, types));
                }
                catch (MethodNotFoundException)
                {
                    return null;
                }
            }
            MethodInfo varArgsMethod = null;
            foreach (MethodInfo method in @base.GetType().GetMethods())
            {
                if (method.Name.Equals(name))
                {
                    int formalParamCount = method.GetParameters().Length;
                    if (method.ContainsGenericParameters && paramCount >= formalParamCount - 1)
                    {
                        varArgsMethod = method;
                    }
                    else if (paramCount == formalParamCount)
                    {
                        return FindAccessibleMethod(method);
                    }
                }
            }
            return varArgsMethod == null ? null : FindAccessibleMethod(varArgsMethod);
        }

        /// <summary>
        /// Lookup an expression factory used to coerce method parameters in context under key
        /// <code>"javax.el.ExpressionFactory"</code>.
        /// If no expression factory can be found under that key, use a default instance created with
        /// <seealso cref="ExpressionFactory.newInstance()"/>. </summary>
        /// <param name="context">
        ///            The context of this evaluation. </param>
        /// <returns> expression factory instance </returns>
        private ExpressionFactory GetExpressionFactory(ELContext context)
        {
            object obj = context.GetContext(typeof(ExpressionFactory));
            if (obj is ExpressionFactory)
            {
                return (ExpressionFactory)obj;
            }
            if (defaultFactory == null)
            {
                defaultFactory = ExpressionFactory.NewInstance();
            }
            return defaultFactory;
        }

        private object[] CoerceParams(ExpressionFactory factory, MethodInfo method, object[] @params)
        {
            Type[] types = method.GetParameters().Select(c=>c.ParameterType).ToArray();
            object[] args = new object[types.Length];
            if (method.ContainsGenericParameters)
            {
                int varargIndex = types.Length - 1;
                if (@params.Length < varargIndex)
                {
                    throw new ELException("Bad argument count");
                }
                for (int i = 0; i < varargIndex; i++)
                {
                    CoerceValue(args, i, factory, @params[i], types[i]);
                }
                Type varargType = types[varargIndex].GetElementType();
                int length = @params.Length - varargIndex;
                object array = null;
                if (length == 1)
                {
                    object source = @params[varargIndex];
                    if (source != null && source.GetType().IsArray)
                    {
                        if (types[varargIndex].IsInstanceOfType(source))
                        { // use source array as is
                            array = source;
                        }
                        else
                        { // coerce array elements
                            object[] ss = (object[]) source;
                            length = ss.Length;
                            array = Array.CreateInstance(varargType, length);
                            for (int i = 0; i < length; i++)
                            {
                                CoerceValue(array, i, factory, ss[i], varargType);
                            }
                        }
                    }
                    else
                    { // single element array
                        array = Array.CreateInstance(varargType, 1);
                        CoerceValue(array, 0, factory, source, varargType);
                    }
                }
                else
                {
                    array = Array.CreateInstance(varargType, length);
                    for (int i = 0; i < length; i++)
                    {
                        CoerceValue(array, i, factory, @params[varargIndex + i], varargType);
                    }
                }
                args[varargIndex] = array;
            }
            else
            {
                if (@params.Length != args.Length)
                {
                    throw new ELException("Bad argument count");
                }
                for (int i = 0; i < args.Length; i++)
                {
                    CoerceValue(args, i, factory, @params[i], types[i]);
                }
            }
            return args;
        }

        private void CoerceValue(object array, int index, ExpressionFactory factory, object value, Type type)
        {
            if (value != null || type.IsPrimitive)
            {
                ((System.Array)array).SetValue(factory.CoerceToType(value, type), index);
            }
        }

        /// <summary>
        /// Test whether the given base should be resolved by this ELResolver.
        /// </summary>
        /// <param name="base">
        ///            The Object to analyze. </param>
        /// <param name="property">
        ///            The name of the property to analyze. Will be coerced to a String. </param>
        /// <returns> base != null </returns>
        private bool IsResolvable(object @base)
        {
            return @base != null;
        }

        /// <summary>
        /// Lookup ObjectProperty for the given (base, property) pair.
        /// </summary>
        /// <param name="base">
        ///            The Object to analyze. </param>
        /// <param name="property">
        ///            The name of the property to analyze. Will be coerced to a String. </param>
        /// <returns> The ObjectProperty representing (base, property). </returns>
        /// <exception cref="PropertyNotFoundException">
        ///             if no ObjectProperty can be found. </exception>
        private ObjectProperty ToObjectProperty(object @base, object property)
        {
            ObjectProperties ObjectProperties = null;
            if(cache.Keys.Contains(@base.GetType()))
                ObjectProperties = cache[@base.GetType()];
            if (ObjectProperties == null)
            {
                ObjectProperties newObjectProperties = new ObjectProperties(@base.GetType());
                ObjectProperties = cache.GetOrAdd(@base.GetType(), newObjectProperties);
                if (ObjectProperties == null)
                { // put succeeded, use new value
                    ObjectProperties = newObjectProperties;
                }
            }
            ObjectProperty ObjectProperty = property == null ? null : ObjectProperties.GetObjectProperty(property.ToString());
            if (ObjectProperty == null)
            {
                throw new PropertyNotFoundException("Could not find property " + property + " in " + @base.GetType());
            }
            return ObjectProperty;
        }

        ///// <summary>
        ///// This method is not part of the API, though it can be used (reflectively) by clients of this
        ///// class to remove entries from the cache when the Objects are being unloaded.
        ///// 
        ///// Note: this method is present in the reference implementation, so we're adding it here to ease
        ///// migration.
        ///// </summary>
        ///// <param name="classloader">
        /////            The classLoader used to load the Objects. </param>
        //private void PurgeObjectClasses(ClassLoader loader)
        //{
        //    IEnumerator<Type> classes = cache.Keys.GetEnumerator();
        //    while (classes.MoveNext())
        //    {
        //        if (loader == classes.Current.ClassLoader)
        //        {
        //            //JAVA TO C. CONVERTER TODO TASK: .NET enumerators are read-only:
        //            classes.Remove();
        //        }
        //    }
        //}
    }
}
