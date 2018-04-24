using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    ///     Defines property resolution behavior on instances of java.Util.List. This resolver handles base
    ///     objects of type java.Util.List. It accepts any object as a property and coerces that object into
    ///     an integer index into the list. The resulting value is the value in the list at that index. This
    ///     resolver can be constructed in read-only mode, which means that isReadOnly will always return
    ///     true and <seealso cref="#setValue(ELContext, Object, Object, Object)" /> will always throw
    ///     PropertyNotWritableException. ELResolvers are combined together using <seealso cref="CompositeELResolver" />
    ///     s, to define rich semantics for evaluating an expression. See the javadocs for <seealso cref="ELResolver" />
    ///     for details.
    /// </summary>
    public class ListELResolver : ELResolver
    {
        private readonly bool _readOnly;

        /// <summary>
        ///     Creates a new read/write ListELResolver.
        /// </summary>
        public ListELResolver() : this(false)
        {
        }

        /// <summary>
        ///     Creates a new ListELResolver whose read-only status is determined by the given parameter.
        /// </summary>
        /// <param name="readOnly">
        ///     true if this resolver cannot modify lists; false otherwise.
        /// </param>
        public ListELResolver(bool readOnly)
        {
            this._readOnly = readOnly;
        }

        /// <summary>
        ///     If the base object is a list, returns the most general type that this resolver accepts for
        ///     the property argument. Otherwise, returns null. Assuming the base is a List, this method will
        ///     always return Integer.class. This is because Lists accept integers as their index.
        /// </summary>
        /// <param name="context">
        ///     The context of this evaluation.
        /// </param>
        /// <param name="base">
        ///     The list to analyze. Only bases of type List are handled by this resolver.
        /// </param>
        /// <returns> null if base is not a List; otherwise Integer.class. </returns>
        public override Type GetCommonPropertyType(ELContext context, object @base)
        {
            return IsResolvable(@base) ? typeof(int) : null;
        }

        /// <summary>
        ///     Always returns null, since there is no reason to iterate through set set of all integers.
        /// </summary>
        /// <param name="context">
        ///     The context of this evaluation.
        /// </param>
        /// <param name="base">
        ///     The list to analyze. Only bases of type List are handled by this resolver.
        /// </param>
        /// <returns> null. </returns>
        public override IEnumerator<MemberInfo> GetMemberInfos(ELContext context, object @base)
        {
            return null;
        }
        /// <summary>
        ///     If the base object is a list, returns the most general acceptable type for a value in this
        ///     list. If the base is a List, the propertyResolved property of the ELContext object must be
        ///     set to true by this resolver, before returning. If this property is not true after this
        ///     method is called, the caller should ignore the return value. Assuming the base is a List,
        ///     this method will always return Object.class. This is because Lists accept any object as an
        ///     element.
        /// </summary>
        /// <param name="context">
        ///     The context of this evaluation.
        /// </param>
        /// <param name="base">
        ///     The list to analyze. Only bases of type List are handled by this resolver.
        /// </param>
        /// <param name="property">
        ///     The index of the element in the list to return the acceptable type for. Will be
        ///     coerced into an integer, but otherwise ignored by this resolver.
        /// </param>
        /// <returns>
        ///     If the propertyResolved property of ELContext was set to true, then the most general
        ///     acceptable type; otherwise undefined.
        /// </returns>
        /// <exception cref="PropertyNotFoundException">
        ///     if the given index is out of bounds for this list.
        /// </exception>
        /// <exception cref="IllegalArgumentException">
        ///     if the property could not be coerced into an integer.
        /// </exception>
        /// <exception cref="NullPointerException">
        ///     if context is null
        /// </exception>
        /// <exception cref="ELException">
        ///     if an exception was thrown while performing the property or variable resolution.
        ///     The thrown exception must be included as the cause property of this exception, if
        ///     available.
        /// </exception>
        public override Type GetType(ELContext context, object @base, object property)
        {
            if (context == null)
                throw new NullReferenceException("context is null");
            Type result = null;
            if (IsResolvable(@base))
            {
                ToIndex((IList<object>) @base, property);
                result = typeof(object);
                context.PropertyResolved = true;
            }
            return result;
        }

        /// <summary>
        ///     If the base object is a list, returns the value at the given index. The index is specified by
        ///     the property argument, and coerced into an integer. If the coercion could not be performed,
        ///     an IllegalArgumentException is thrown. If the index is out of bounds, null is returned. If
        ///     the base is a List, the propertyResolved property of the ELContext object must be set to true
        ///     by this resolver, before returning. If this property is not true after this method is called,
        ///     the caller should ignore the return value.
        /// </summary>
        /// <param name="context">
        ///     The context of this evaluation.
        /// </param>
        /// <param name="base">
        ///     The list to analyze. Only bases of type List are handled by this resolver.
        /// </param>
        /// <param name="property">
        ///     The index of the element in the list to return the acceptable type for. Will be
        ///     coerced into an integer, but otherwise ignored by this resolver.
        /// </param>
        /// <returns>
        ///     If the propertyResolved property of ELContext was set to true, then the value at the
        ///     given index or null if the index was out of bounds. Otherwise, undefined.
        /// </returns>
        /// <exception cref="PropertyNotFoundException">
        ///     if the given index is out of bounds for this list.
        /// </exception>
        /// <exception cref="IllegalArgumentException">
        ///     if the property could not be coerced into an integer.
        /// </exception>
        /// <exception cref="NullPointerException">
        ///     if context is null
        /// </exception>
        /// <exception cref="ELException">
        ///     if an exception was thrown while performing the property or variable resolution.
        ///     The thrown exception must be included as the cause property of this exception, if
        ///     available.
        /// </exception>
        public override object GetValue(ELContext context, object @base, object property)
        {
            if (context == null)
                throw new NullReferenceException("context is null");
            object result = null;
            if (IsResolvable(@base))
            {
                var index = ToIndex<int>(null, property);
                //var list = (IList<object>) @base;
                //result = (index < 0) || (index >= list.Count) ? null : list[index];
                var list =  @base as IList;
                if (list == null) return result;
                result = (index < 0) || (index >= list.Count) ? null : list[index];
                context.PropertyResolved = true;
            }
            return result;
        }

        /// <summary>
        ///     If the base object is a list, returns whether a call to
        ///     <seealso cref="#setValue(ELContext, Object, Object, Object)" /> will always fail. If the base is a List,
        ///     the propertyResolved property of the ELContext object must be set to true by this resolver,
        ///     before returning. If this property is not true after this method is called, the caller should
        ///     ignore the return value. If this resolver was constructed in read-only mode, this method will
        ///     always return true. If a List was created using java.Util.Collections.unmodifiableList(List),
        ///     this method must return true. Unfortunately, there is no Collections API method to detect
        ///     this. However, an implementation can create a prototype unmodifiable List and query its
        ///     runtime type to see if it matches the runtime type of the base object as a workaround.
        /// </summary>
        /// <param name="context">
        ///     The context of this evaluation.
        /// </param>
        /// <param name="base">
        ///     The list to analyze. Only bases of type List are handled by this resolver.
        /// </param>
        /// <param name="property">
        ///     The index of the element in the list to return the acceptable type for. Will be
        ///     coerced into an integer, but otherwise ignored by this resolver.
        /// </param>
        /// <returns>
        ///     If the propertyResolved property of ELContext was set to true, then true if calling
        ///     the setValue method will always fail or false if it is possible that such a call may
        ///     succeed; otherwise undefined.
        /// </returns>
        /// <exception cref="PropertyNotFoundException">
        ///     if the given index is out of bounds for this list.
        /// </exception>
        /// <exception cref="IllegalArgumentException">
        ///     if the property could not be coerced into an integer.
        /// </exception>
        /// <exception cref="NullPointerException">
        ///     if context is null
        /// </exception>
        /// <exception cref="ELException">
        ///     if an exception was thrown while performing the property or variable resolution.
        ///     The thrown exception must be included as the cause property of this exception, if
        ///     available.
        /// </exception>
        public override bool IsReadOnly(ELContext context, object @base, object property)
        {
            if (context == null)
                throw new NullReferenceException("context is null");
            if (IsResolvable(@base))
            {
                ToIndex((IList<object>) @base, property);
                context.PropertyResolved = true;
            }
            return _readOnly;
        }

        /// <summary>
        ///     If the base object is a list, attempts to set the value at the given index with the given
        ///     value. The index is specified by the property argument, and coerced into an integer. If the
        ///     coercion could not be performed, an IllegalArgumentException is thrown. If the index is out
        ///     of bounds, a PropertyNotFoundException is thrown. If the base is a List, the propertyResolved
        ///     property of the ELContext object must be set to true by this resolver, before returning. If
        ///     this property is not true after this method is called, the caller can safely assume no value
        ///     was set. If this resolver was constructed in read-only mode, this method will always throw
        ///     PropertyNotWritableException. If a List was created using
        ///     java.Util.Collections.unmodifiableList(List), this method must throw
        ///     PropertyNotWritableException. Unfortunately, there is no Collections API method to detect
        ///     this. However, an implementation can create a prototype unmodifiable List and query its
        ///     runtime type to see if it matches the runtime type of the base object as a workaround.
        /// </summary>
        /// <param name="context">
        ///     The context of this evaluation.
        /// </param>
        /// <param name="base">
        ///     The list to analyze. Only bases of type List are handled by this resolver.
        /// </param>
        /// <param name="property">
        ///     The index of the element in the list to return the acceptable type for. Will be
        ///     coerced into an integer, but otherwise ignored by this resolver.
        /// </param>
        /// <param name="value">
        ///     The value to be set at the given index.
        /// </param>
        /// <exception cref="ClassCastException">
        ///     if the class of the specified element prevents it from being added to this list.
        /// </exception>
        /// <exception cref="PropertyNotFoundException">
        ///     if the given index is out of bounds for this list.
        /// </exception>
        /// <exception cref="PropertyNotWritableException">
        ///     if this resolver was constructed in read-only mode, or if the set operation is
        ///     not supported by the underlying list.
        /// </exception>
        /// <exception cref="IllegalArgumentException">
        ///     if the property could not be coerced into an integer.
        /// </exception>
        /// <exception cref="NullPointerException">
        ///     if context is null
        /// </exception>
        /// <exception cref="ELException">
        ///     if an exception was thrown while performing the property or variable resolution.
        ///     The thrown exception must be included as the cause property of this exception, if
        ///     available.
        /// </exception>
        public void SetValue(ELContext context, object @base, object property, int value)
        {
            if (context == null)
                throw new NullReferenceException("context is null");
            if (IsResolvable(@base))
            {
                if (_readOnly)
                    throw new PropertyNotWritableException("resolver is read-only");
                var list = (IList<int>) @base;
                var index = ToIndex(list, property);
                try
                {
                    list[index] = value;
                }
                catch (NotSupportedException e)
                {
                    throw new PropertyNotWritableException(e);
                }
                //catch (ArrayStoreException e)
                //{
                //    throw new ArgumentException(e);
                //}
                context.PropertyResolved = true;
            }
        }

        /// <summary>
        ///     Test whether the given base should be resolved by this ELResolver.
        /// </summary>
        /// <param name="base">
        ///     The bean to analyze.
        /// </param>
        /// <param name="property">
        ///     The name of the property to analyze. Will be coerced to a String.
        /// </param>
        /// <returns> base instanceof List </returns>
        private static bool IsResolvable(object @base)
        {
            return @base is IEnumerable;
        }

        /// <summary>
        ///     Convert the given property to an index in (list) base.
        /// </summary>
        /// <param name="base">
        ///     The bean to analyze.
        /// </param>
        /// <param name="property">
        ///     The name of the property to analyze. Will be coerced to a String.
        /// </param>
        /// <returns> The index of property in base. </returns>
        /// <exception cref="IllegalArgumentException">
        ///     if base property cannot be coerced to an integer.
        /// </exception>
        /// <exception cref="PropertyNotFoundException">
        ///     if base is not null and the computed index is out of bounds for base.
        /// </exception>
        private static int ToIndex<T1>(IList<T1> @base, object property)
        {
            var index = 0;
            if (property is decimal || property is int || property is long)
                index = Convert.ToInt32(property);
            else if (property is string)
                try
                {
                    int.TryParse((string)property, out index);
                }
                catch (FormatException)
                {
                    throw new ArgumentException("Cannot parse list index: " + property);
                }
            else if (property is char?)
                index = ((char?) property).Value;
            else if (property is bool?)
                index = ((bool?) property).Value ? 1 : 0;
            else
                throw new ArgumentException("Cannot coerce property to list index: " + property);
            if ((@base != null) && ((index < 0) || (index >= @base.Count)))
                throw new PropertyNotFoundException("List index out of bounds: " + index);
            return index;
        }

        public override void SetValue(ELContext context, object @base, object property, object value)
        {
            throw new NotImplementedException();
        }
    }
}