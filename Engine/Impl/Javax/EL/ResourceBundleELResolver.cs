using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    /// 依赖ResourceBundle 不启用
    ///     Defines property resolution behavior on instances of java.Util.ResourceBundle. This resolver
    ///     handles base objects of type java.Util.ResourceBundle. It accepts any object as a property and
    ///     coerces it to a java.lang.String for invoking java.Util.ResourceBundle.getObject(String). This
    ///     resolver is read only and will throw a <seealso cref="PropertyNotWritableException" /> if setValue is
    ///     called. ELResolvers are combined together using <seealso cref="CompositeELResolver" />s, to define rich
    ///     semantics for evaluating an expression. See the javadocs for <seealso cref="ELResolver" /> for details.
    /// </summary>

    public class ResourceBundleElResolver : ELResolver
    {
        /// <summary>
        ///     If the base object is a ResourceBundle, returns the most general type that this resolver
        ///     accepts for the property argument. Otherwise, returns null. Assuming the base is a
        ///     ResourceBundle, this method will always return String.class.
        /// </summary>
        /// <param name="context">
        ///     The context of this evaluation.
        /// </param>
        /// <param name="base">
        ///     The bundle to analyze. Only bases of type ResourceBundle are handled by this
        ///     resolver.
        /// </param>
        /// <returns> null if base is not a ResourceBundle; otherwise String.class. </returns>
        public override Type GetCommonPropertyType(ELContext context, object @base)
        {
            return IsResolvable(@base) ? typeof(string) : null;
        }

        /// <summary>
        /// If the base object is a ResourceBundle, returns an Iterator containing the set of keys
        /// available in the ResourceBundle. Otherwise, returns null. The Iterator returned must contain
        /// zero or more instances of java.beans.FeatureDescriptor. Each info object contains information
        /// about a key in the ResourceBundle, and is initialized as follows:
        /// <ul>
        /// <li>displayName - The String key name</li>
        /// <li>name - Same as displayName property</li>
        /// <li>shortDescription - Empty string</li>
        /// <li>expert - false</li>
        /// <li>hidden - false</li>
        /// <li>preferred - true</li>
        /// </ul>
        /// In addition, the following named attributes must be set in the returned FeatureDescriptors:
        /// <ul>
        /// <li><seealso cref="ELResolver#TYPE"/> - String.class.</li>
        /// <li><seealso cref="ELResolver#RESOLVABLE_AT_DESIGN_TIME"/> - true</li>
        /// </ul>
        /// </summary>
        /// <param name="context">
        ///            The context of this evaluation. </param>
        /// <param name="base">
        ///            The bundle to analyze. Only bases of type ResourceBundle are handled by this
        ///            resolver. </param>
        /// <returns> An Iterator containing zero or more (possibly infinitely more) FeatureDescriptor
        ///         objects, each representing a key in this bundle, or null if the base object is not a
        ///         ResourceBundle. </returns>
        public override IEnumerator<MemberInfo> GetMemberInfos(ELContext context, object @base)
        {
            if (IsResolvable(@base))
            {
                //IEnumerator<string> keys = ((ResourceBundle)@base).Keys;
                //return new IteratorAnonymousInnerClass(this, keys);
            }
            return null;
        }

        //private class IteratorAnonymousInnerClass : IEnumerator<FeatureDescriptor>
        //{
        //    private readonly MissingClass outerInstance;

        //    private IEnumerator<string> keys;

        //    public IteratorAnonymousInnerClass(MissingClass outerInstance, IEnumerator<string> keys)
        //    {
        //        this.outerInstance = outerInstance;
        //        this.keys = keys;
        //    }

        //    public virtual bool HasNext()
        //    {
        //        //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
        //        return keys.HasMoreElements();
        //    }
        //    public virtual FeatureDescriptor Next()
        //    {
        //        FeatureDescriptor feature = new FeatureDescriptor();
        //        //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
        //        feature.DisplayName = keys.NextElement();
        //        feature.Name = feature.DisplayName;
        //        feature.ShortDescription = "";
        //        feature.Expert = true;
        //        feature.Hidden = false;
        //        feature.Preferred = true;
        //        feature.SetValue(TYPE, typeof(string));
        //        feature.SetValue(RESOLVABLE_AT_DESIGN_TIME, true);
        //        return feature;
        //    }
        //    public virtual void Remove()
        //    {
        //        throw new System.NotSupportedException("Cannot remove");

        //    }
        //}

        /// <summary>
        ///     If the base object is a ResourceBundle, returns an Iterator containing the set of keys
        ///     available in the ResourceBundle. Otherwise, returns null. The Iterator returned must contain
        ///     zero or more instances of java.beans.MemberDescriptor. Each info object contains information
        ///     about a key in the ResourceBundle, and is initialized as follows:
        ///     <ul>
        ///         <li>displayName - The String key name</li>
        ///         <li>name - Same as displayName property</li>
        ///         <li>shortDescription - Empty string</li>
        ///         <li>expert - false</li>
        ///         <li>hidden - false</li>
        ///         <li>preferred - true</li>
        ///     </ul>
        ///     In addition, the following named attributes must be set in the returned MemberInfos:
        ///     <ul>
        ///         <li><seealso cref="ELResolver#TYPE" /> - String.class.</li>
        ///         <li><seealso cref="ELResolver#RESOLVABLE_AT_DESIGN_TIME" /> - true</li>
        ///     </ul>
        /// </summary>
        /// <param name="context">
        ///     The context of this evaluation.
        /// </param>
        /// <param name="base">
        ///     The bundle to analyze. Only bases of type ResourceBundle are handled by this
        ///     resolver.
        /// </param>
        /// <returns>
        ///     An Iterator containing zero or more (possibly infinitely more) MemberDescriptor
        ///     objects, each representing a key in this bundle, or null if the base object is not a
        ///     ResourceBundle.
        /// </returns>
        /// <summary>
        ///     If the base object is an instance of ResourceBundle, return null, since the resolver is read
        ///     only. If the base is ResourceBundle, the propertyResolved property of the ELContext object
        ///     must be set to true by this resolver, before returning. If this property is not true after
        ///     this method is called, the caller should ignore the return value.
        /// </summary>
        /// <param name="context">
        ///     The context of this evaluation.
        /// </param>
        /// <param name="base">
        ///     The bundle to analyze. Only bases of type ResourceBundle are handled by this
        ///     resolver.
        /// </param>
        /// <param name="property">
        ///     The name of the property to analyze.
        /// </param>
        /// <returns>
        ///     If the propertyResolved property of ELContext was set to true, then null; otherwise
        ///     undefined.
        /// </returns>
        /// <exception cref="NullPointerException">
        ///     if context is null
        /// </exception>
        public override Type GetType(ELContext context, object @base, object property)
        {
            if (context == null)
                throw new NullReferenceException("context is null");
            if (IsResolvable(@base))
                context.PropertyResolved = true;
            return null;
        }

        /// <summary>
        ///     If the base object is an instance of ResourceBundle, the provided property will first be
        ///     coerced to a String. The Object returned by getObject on the base ResourceBundle will be
        ///     returned. If the base is ResourceBundle, the propertyResolved property of the ELContext
        ///     object must be set to true by this resolver, before returning. If this property is not true
        ///     after this method is called, the caller should ignore the return value.
        /// </summary>
        /// <param name="context">
        ///     The context of this evaluation.
        /// </param>
        /// <param name="base">
        ///     The bundle to analyze. Only bases of type ResourceBundle are handled by this
        ///     resolver.
        /// </param>
        /// <param name="property">
        ///     The name of the property to analyze. Will be coerced to a String.
        /// </param>
        /// <returns>
        ///     If the propertyResolved property of ELContext was set to true, then null if property
        ///     is null; otherwise the Object for the given key (property coerced to String) from the
        ///     ResourceBundle. If no object for the given key can be found, then the String "???" +
        ///     key + "???".
        /// </returns>
        /// <exception cref="NullPointerException">
        ///     if context is null.
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
                context.PropertyResolved = true;
            return result;
        }

        /// <summary>
        ///     If the base object is not null and an instanceof java.Util.ResourceBundle, return true.
        /// </summary>
        /// <returns>
        ///     If the propertyResolved property of ELContext was set to true, then true; otherwise
        ///     undefined.
        /// </returns>
        /// <exception cref="NullPointerException">
        ///     if context is null.
        /// </exception>
        public override bool IsReadOnly(ELContext context, object @base, object property)
        {
            if (context == null)
                throw new NullReferenceException("context is null");
            if (IsResolvable(@base))
                context.PropertyResolved = true;
            return true;
        }

        /// <summary>
        ///     If the base object is a ResourceBundle, throw a <seealso cref="PropertyNotWritableException" />.
        /// </summary>
        /// <param name="context">
        ///     The context of this evaluation.
        /// </param>
        /// <param name="base">
        ///     The bundle to analyze. Only bases of type ResourceBundle are handled by this
        ///     resolver.
        /// </param>
        /// <param name="property">
        ///     The name of the property to analyze. Will be coerced to a String.
        /// </param>
        /// <param name="value">
        ///     The value to be set.
        /// </param>
        /// <exception cref="NullPointerException">
        ///     if context is null.
        /// </exception>
        /// <exception cref="PropertyNotWritableException">
        ///     Always thrown if base is an instance of ResourceBundle.
        /// </exception>
        public override void SetValue(ELContext context, object @base, object property, object value)
        {
            if (context == null)
                throw new NullReferenceException("context is null");
            if (IsResolvable(@base))
                throw new PropertyNotWritableException("resolver is read-only");
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
        /// <returns> base instanceof ResourceBundle </returns>
        private bool IsResolvable(object @base)
        {
            return false;
            //return @base is ResourceBundle;
        }
        
    }
}