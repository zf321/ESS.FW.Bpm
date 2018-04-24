using System;
using System.Linq;
using System.Reflection;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{

	public class AstIdentifier : AstNode, IIdentifierNode
	{
		private readonly string name;
		private readonly int index;

		public AstIdentifier(string name, int index)
		{
			this.name = name;
			this.index = index;
		}

		public override Type GetType(Bindings bindings, ELContext context)
		{
			ValueExpression expression = bindings.GetVariable(index);
			if (expression != null)
			{
				return expression.GetType(context);
			}
			context.PropertyResolved = false;
			Type result = context.ELResolver.GetType(context, null, name);
			if (!context.PropertyResolved)
			{
				throw new PropertyNotFoundException(LocalMessages.Get("error.identifier.property.notfound", name));
			}
			return result;
		}


		public override bool LeftValue
		{
			get
			{
				return true;
			}
		}

		public override bool MethodInvocation
		{
			get
			{
				return false;
			}
		}

		public override bool LiteralText
		{
			get
			{
				return false;
			}
		}

		public override ValueReference GetValueReference(Bindings bindings, ELContext context)
		{
			ValueExpression expression = bindings.GetVariable(index);
			if (expression != null)
			{
				return expression.GetValueReference(context);
			}
			return new ValueReference(null, name);
		}

		public override object Eval(Bindings bindings, ELContext context)
		{
			ValueExpression expression = bindings.GetVariable(index);
			if (expression != null)
			{
				return expression.GetValue(context);
			}
			context.PropertyResolved = false;
			object result = context.ELResolver.GetValue(context, null, name);
			if (!context.PropertyResolved)
			{
				throw new PropertyNotFoundException(LocalMessages.Get(string.Format("error.identifier.property.notfound:{0}", name)));
			}
			return result;
		}

		public override void SetValue(Bindings bindings, ELContext context, object value)
		{
			ValueExpression expression = bindings.GetVariable(index);
			if (expression != null)
			{
				expression.SetValue(context, value);
				return;
			}
			context.PropertyResolved = false;
			context.ELResolver.SetValue(context, null, name, value);
			if (!context.PropertyResolved)
			{
				throw new PropertyNotFoundException(LocalMessages.Get("error.identifier.property.notfound", name));
			}
		}

		public override bool IsReadOnly(Bindings bindings, ELContext context)
		{
			ValueExpression expression = bindings.GetVariable(index);
			if (expression != null)
			{
				return expression.IsReadOnly(context);
			}
			context.PropertyResolved = false;
			bool result = context.ELResolver.IsReadOnly(context, null, name);
			if (!context.PropertyResolved)
			{
				throw new PropertyNotFoundException(LocalMessages.Get("error.identifier.property.notfound", name));
			}
			return result;
		}

		protected internal virtual MethodInfo GetMethod(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
		{
			object value = Eval(bindings, context);
			if (value == null)
			{
				throw new MethodNotFoundException(LocalMessages.Get("error.identifier.method.notfound", name));
			}
			if (value is MethodInfo)
			{
                MethodInfo method = (MethodInfo)value;
				if (returnType != null && !returnType.IsAssignableFrom(method.ReturnType))
				{
					throw new MethodNotFoundException(LocalMessages.Get("error.identifier.method.notfound", name));
				}
				if (!Enumerable.SequenceEqual(method.GetParameters().Select(c=>c.ParameterType), paramTypes))
				{
					throw new MethodNotFoundException(LocalMessages.Get("error.identifier.method.notfound", name));
				}
				return method;
			}
			throw new MethodNotFoundException(LocalMessages.Get("error.identifier.method.notamethod", name, value.GetType()));
		}

		public override MethodInfo GetMethodInfo(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes)
		{
            MethodInfo method = GetMethod(bindings, context, returnType, paramTypes);
			//return new MethodInfo(method.Name, method.ReturnType, paramTypes);
            return method;
		}

		public override object Invoke(Bindings bindings, ELContext context, Type returnType, Type[] paramTypes, object[] @params)
		{
            MethodInfo method = GetMethod(bindings, context, returnType, paramTypes);
			try
			{
				return method.Invoke(null, @params);
			}
			catch (AccessViolationException)
			{
				throw new ELException(LocalMessages.Get("error.identifier.method.access", name));
			}
			catch (System.ArgumentException e)
			{
				throw new ELException(LocalMessages.Get("error.identifier.method.invocation", name, e));
			}
			catch (TargetInvocationException e)
			{
				throw new ELException(LocalMessages.Get("error.identifier.method.invocation", name, e.InnerException));
			}
		}

		public override string ToString()
		{
			return name;
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append(bindings != null && bindings.IsVariableBound(index) ? "<var>" : name);
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public override int Cardinality
		{
			get
			{
				return 0;
			}
		}

		public override INode GetChild(int i)
		{
			return null;
		}
	}

}