using System;
using System.Reflection;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{


    public abstract class AstProperty : AstNode
    {
        protected internal readonly AstNode prefix;
        protected internal readonly bool lvalue;
        protected internal readonly bool strict; // allow null as property value?

        public AstProperty(AstNode prefix, bool lvalue, bool strict)
        {
            this.prefix = prefix;
            this.lvalue = lvalue;
            this.strict = strict;
        }

        protected internal abstract object GetProperty(Bindings bindings, ELContext context);

        protected internal virtual AstNode Prefix
        {
            get
            {
                return prefix;
            }
        }

        public override ValueReference GetValueReference(Bindings bindings, ELContext context)
        {
            return new ValueReference(prefix.Eval(bindings, context), GetProperty(bindings, context));
        }

		public override object Eval(Bindings bindings, ELContext context)
		{
			object @base = prefix.Eval(bindings, context);
			if (@base == null)
			{
				return null;
			}
			object property = GetProperty(bindings, context);
			if (property == null && strict)
			{
				return null;
			}
		    
                context.PropertyResolved = false;
                var result = context.ELResolver.GetValue(context, @base, property);
                if (!context.PropertyResolved)
                {
                    throw new PropertyNotFoundException(LocalMessages.Get("error.property.property.notfound", property, @base));
                }
			return result;
		}

        public sealed override bool LiteralText
        {
            get
            {
                return false;
            }
        }

        public sealed override bool LeftValue
        {
            get
            {
                return lvalue;
            }
        }

        public override bool MethodInvocation
        {
            get
            {
                return false;
            }
        }

        public override Type GetType(Bindings bindings, ELContext context)
        {
            if (!lvalue)
            {
                return null;
            }
            object @base = prefix.Eval(bindings, context);
            if (@base == null)
            {
                throw new PropertyNotFoundException(LocalMessages.Get("error.property.base.null", prefix));
            }
            object property = GetProperty(bindings, context);
            if (property == null && strict)
            {
                throw new PropertyNotFoundException(LocalMessages.Get("error.property.property.notfound", "null", @base));
            }
            context.PropertyResolved = false;
            Type result = context.ELResolver.GetType(context, @base, property);
            if (!context.PropertyResolved)
            {
                throw new PropertyNotFoundException(LocalMessages.Get("error.property.property.notfound", property, @base));
            }
            return result;
        }

        public override bool IsReadOnly(Bindings bindings, ELContext context)
        {
            if (!lvalue)
            {
                return true;
            }
            object @base = prefix.Eval(bindings, context);
            if (@base == null)
            {
                throw new PropertyNotFoundException(LocalMessages.Get("error.property.base.null", prefix));
            }
            object property = GetProperty(bindings, context);
            if (property == null && strict)
            {
                throw new PropertyNotFoundException(LocalMessages.Get("error.property.property.notfound", "null", @base));
            }
            context.PropertyResolved = false;
            bool result = context.ELResolver.IsReadOnly(context, @base, property);
            if (!context.PropertyResolved)
            {
                throw new PropertyNotFoundException(LocalMessages.Get("error.property.property.notfound", property, @base));
            }
            return result;
        }

        public override void SetValue(Bindings bindings, ELContext context, object value)
        {
            if (!lvalue)
            {
                throw new ELException(LocalMessages.Get("error.value.set.rvalue", GetStructuralId(bindings)));
            }
            object @base = prefix.Eval(bindings, context);
            if (@base == null)
            {
                throw new PropertyNotFoundException(LocalMessages.Get("error.property.base.null", prefix));
            }
            object property = GetProperty(bindings, context);
            if (property == null && strict)
            {
                throw new PropertyNotFoundException(LocalMessages.Get("error.property.property.notfound", "null", @base));
            }
            context.PropertyResolved = false;
            context.ELResolver.SetValue(context, @base, property, value);
            if (!context.PropertyResolved)
            {
                throw new PropertyNotFoundException(LocalMessages.Get("error.property.property.notfound", property, @base));
            }
        }

        protected internal virtual MethodInfo FindMethod(string name, Type clazz, Type returnType, Type[] paramTypes)
        {
            MethodInfo method = null;
            try
            {
                method = clazz.GetMethod(name, paramTypes);
            }
            catch (MethodNotFoundException)
            {
                throw new MethodNotFoundException(LocalMessages.Get("error.property.method.notfound", name, clazz));
            }
            if (returnType != null && !returnType.IsAssignableFrom(method.ReturnType))
            {
                throw new MethodNotFoundException(LocalMessages.Get("error.property.method.notfound", name, clazz));
            }
            return method;
        }

        public override MethodInfo GetMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
        {
            object @base = prefix.Eval(bindings, context);
            if (@base == null)
            {
                throw new PropertyNotFoundException(LocalMessages.Get("error.property.base.null", prefix));
            }
            object property = GetProperty(bindings, context);
            if (property == null && strict)
            {
                throw new PropertyNotFoundException(LocalMessages.Get("error.property.method.notfound", "null", @base));
            }
            string name = bindings.Convert<string>(property, typeof(string));
            MethodInfo method = FindMethod(name, @base.GetType(), returnType, paramTypes);
            //return new MethodInfo(method.Name, method.ReturnType, paramTypes);
            return method;
        }

        public override object Invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] paramValues)
        {
            object @base = prefix.Eval(bindings, context);
            if (@base == null)
            {
                throw new PropertyNotFoundException(LocalMessages.Get("error.property.base.null", prefix));
            }
            object property = GetProperty(bindings, context);
            if (property == null && strict)
            {
                throw new PropertyNotFoundException(LocalMessages.Get("error.property.method.notfound", "null", @base));
            }
            string name = bindings.Convert<string>(property, typeof(string));
            MethodInfo method = FindMethod(name, @base.GetType(), returnType, paramTypes);
            try
            {
                return method.Invoke(@base, paramValues);
            }
            catch (AccessViolationException)
            {
                throw new ELException(LocalMessages.Get("error.property.method.access", name, @base.GetType()));
            }
            catch (System.ArgumentException e)
            {
                throw new ELException(LocalMessages.Get("error.property.method.invocation", name, @base.GetType()), e);
            }
            catch (TargetInvocationException e)
            {
                throw new ELException(LocalMessages.Get("error.property.method.invocation", name, @base.GetType()), e.InnerException);
            }
        }

        public override INode GetChild(int i)
        {
            return i == 0 ? prefix : null;
        }
    }

}