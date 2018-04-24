using System;
using System.Linq;
using System.Reflection;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    


	public class AstFunction : AstRightValue, IFunctionNode
	{
		private readonly int index;
		private readonly string name;
		private readonly AstParameters @params;
		private readonly bool varargs;

		public AstFunction(string name, int index, AstParameters @params) : this(name, index, @params, false)
		{
		}

		public AstFunction(string name, int index, AstParameters @params, bool varargs)
		{
			this.name = name;
			this.index = index;
			this.@params = @params;
			this.varargs = varargs;
		}

		/// <summary>
		/// Invoke method. </summary>
		/// <param name="bindings"> </param>
		/// <param name="context"> </param>
		/// <param name="base"> </param>
		/// <param name="method"> </param>
		/// <returns> method result </returns>
		/// <exception cref="TargetInvocationException"> </exception>
		/// <exception cref="AccessViolationException"> </exception>
		protected internal virtual object Invoke(Bindings bindings, ELContext context, object @base, MethodInfo method)
		{
			Type[] types = method.GetParameters().Select(c=>c.ParameterType).ToArray();
			object[] @params = null;
			if (types.Length > 0)
			{
				@params = new object[types.Length];
                //if (varargs && method.GetParameters().Any())
                if (method.ContainsGenericParameters && method.GetParameters().Any())
                {
                    for (int i = 0; i < @params.Length - 1; i++)
					{
						object param = GetParam(i).Eval(bindings, context);
						if (param != null || types[i].IsPrimitive)
						{
							@params[i] = bindings.Convert<object>(param, types[i]);
						}
					}
					int varargIndex = types.Length - 1;
					Type varargType = types[varargIndex];
					int length = ParamCount - varargIndex;
					object array = null;
					if (length == 1)
					{ // special: eventually use argument as is
						object param = GetParam(varargIndex).Eval(bindings, context);
						if (param != null && param.GetType().IsArray)
						{
							if (types[varargIndex].IsInstanceOfType(param))
							{
								array = param;
							}
							else
							{ // coerce array elements
								//length = Array.GetLength(param);
								//array = Array.CreateInstance(varargType, length);
								//for (int i = 0; i < length; i++)
								//{
								//	object elem = Array.Get(param, i);
								//	if (elem != null || varargType.IsPrimitive)
								//	{
								//		((System.Array)array).SetValue(bindings.Convert(elem, varargType), i);
								//	}
								//}
							}
						}
						else
						{ // single element array
							array = Array.CreateInstance(varargType, 1);
							if (param != null || varargType.IsPrimitive)
							{
								((System.Array)array).SetValue(bindings.Convert<object>(param, varargType), 0);
							}
						}
					}
					else
					{
						array = Array.CreateInstance(varargType, length);
						for (int i = 0; i < length; i++)
						{
							object param = GetParam(varargIndex + i).Eval(bindings, context);
							if (param != null || varargType.IsPrimitive)
							{
								((System.Array)array).SetValue(bindings.Convert<object>(param, varargType), i);
							}
						}
					}
					@params[varargIndex] = array;
				}
				else
				{
					for (int i = 0; i < @params.Length; i++)
					{
						object param = GetParam(i).Eval(bindings, context);
						if (param != null || types[i].IsPrimitive)
						{
							@params[i] = bindings.Convert<object>(param, types[i]);
						}
					}
				}
			}
			return method.Invoke(@base, @params);
		}

		public override object Eval(Bindings bindings, ELContext context)
		{
            MethodInfo method = bindings.GetFunction(index);
			try
			{
				return Invoke(bindings, context, null, method);
			}
			catch (AccessViolationException e)
			{
				throw new ELException(LocalMessages.Get("error.function.access", name), e);
			}
			catch (TargetInvocationException e)
			{
				throw new ELException(LocalMessages.Get("error.function.invocation", name), e.InnerException);
			}
		}

		public override string ToString()
		{
			return name;
		}

		public override void AppendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append(bindings != null && bindings.IsFunctionBound(index) ? "<fn>" : name);
			@params.AppendStructure(b, bindings);
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

		public virtual bool VarArgs
		{
			get
			{
				return varargs;
			}
		}

		public virtual int ParamCount
		{
			get
			{
				return @params.Cardinality;
			}
		}

		protected internal virtual AstNode GetParam(int i)
		{
			return (AstNode) @params.GetChild(i);
		}

		public override int Cardinality
		{
			get
			{
				return 1;
			}
		}

		public override INode GetChild(int i)
		{
			return i == 0 ? @params : null;
		}
	}

}