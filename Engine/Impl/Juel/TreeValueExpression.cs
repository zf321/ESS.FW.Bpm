using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    /// <summary>
	/// A value expression is ready to be evaluated (by calling either
	/// <seealso cref="#getType(ELContext)"/>, <seealso cref="#getValue(ELContext)"/>, <seealso cref="#isReadOnly(ELContext)"/>
	/// or <seealso cref="#setValue(ELContext, Object)"/>.
	/// 
	/// Instances of this class are usually created using an <seealso cref="ExpressionFactoryImpl"/>.
	/// 
	/// 
	/// </summary>
	public sealed class TreeValueExpression : ValueExpression
    {
        private const long serialVersionUID = 1L;

        private readonly TreeBuilder builder;
        private readonly Bindings bindings;
        private readonly string expr;
        private readonly Type type;
        private readonly bool deferred;

        [NonSerialized]
        private IExpressionNode node;

        private string structure;

        /// <summary>
        /// Create a new value expression. </summary>
        /// <param name="store"> used to get the parse tree from. </param>
        /// <param name="functions"> the function mapper used to bind functions </param>
        /// <param name="variables"> the variable mapper used to bind variables </param>
        /// <param name="expr"> the expression string </param>
        /// <param name="type"> the expected type (may be <code>null</code>) </param>
        public TreeValueExpression(TreeStore store, FunctionMapper functions, VariableMapper variables, ITypeConverter converter, string expr, Type type) : base()
        {

            Tree tree = store.Get(expr);

            this.builder = store.Builder;
            this.bindings = tree.Bind(functions, variables, converter);
            this.expr = expr;
            this.type = type;
            this.node = tree.Root;
            this.deferred = tree.Deferred;

            if (type == null)
            {
                throw new System.NullReferenceException(LocalMessages.Get("error.value.notype"));
            }
        }

        private string StructuralId
        {
            get
            {
                if (string.ReferenceEquals(structure, null))
                {
                    structure = node.GetStructuralId(bindings);
                }
                return structure;
            }
        }

        public override Type ExpectedType
        {
            get
            {
                return type;
            }
        }

        public override string ExpressionString
        {
            get
            {
                return expr;
            }
        }

        /// <summary>
        /// Evaluates the expression as an lvalue and answers the result type. </summary>
        /// <param name="context"> used to resolve properties (<code>base.property</code> and <code>base[property]</code>)
        /// and to determine the result from the last base/property pair </param>
        /// <returns> lvalue evaluation type or <code>null</code> for rvalue expressions </returns>
        /// <exception cref="ELException"> if evaluation fails (e.g. property not found, type conversion failed, ...) </exception>
        public override Type GetType(ELContext context)
        {
            return node.GetType(bindings, context);
        }

        /// <summary>
        /// Evaluates the expression as an rvalue and answers the result. </summary>
        /// <param name="context"> used to resolve properties (<code>base.property</code> and <code>base[property]</code>)
        /// and to determine the result from the last base/property pair </param>
        /// <returns> rvalue evaluation result </returns>
        /// <exception cref="ELException"> if evaluation fails (e.g. property not found, type conversion failed, ...) </exception>
        public override object GetValue(ELContext context)
        {
            return node.GetValue(bindings, context, type);
        }

        /// <summary>
        /// Evaluates the expression as an lvalue and determines if <seealso cref="#setValue(ELContext, Object)"/>
        /// will always fail. </summary>
        /// <param name="context"> used to resolve properties (<code>base.property</code> and <code>base[property]</code>)
        /// and to determine the result from the last base/property pair </param>
        /// <returns> <code>true</code> if <seealso cref="#setValue(ELContext, Object)"/> always fails. </returns>
        /// <exception cref="ELException"> if evaluation fails (e.g. property not found, type conversion failed, ...) </exception>
        public override bool IsReadOnly(ELContext context)
        {
            return node.IsReadOnly(bindings, context);
        }

        /// <summary>
        /// Evaluates the expression as an lvalue and assigns the given value. </summary>
        /// <param name="context"> used to resolve properties (<code>base.property</code> and <code>base[property]</code>)
        /// and to perform the assignment to the last base/property pair </param>
        /// <exception cref="ELException"> if evaluation fails (e.g. property not found, type conversion failed, assignment failed...) </exception>
        public override void SetValue(ELContext context, object value)
        {
            node.SetValue(bindings, context, value);
        }

        /// <returns> <code>true</code> if this is a literal text expression </returns>
        public override bool IsLiteralText
        {
            get
            {
                return node.LiteralText;
            }
        }

        public override ValueReference GetValueReference(ELContext context)
        {
            return node.GetValueReference(bindings, context);
        }

        /// <summary>
        /// Answer <code>true</code> if this could be used as an lvalue.
        /// This is the case for eval expressions consisting of a simple identifier or
        /// a nonliteral prefix, followed by a sequence of property operators (<code>.</code> or <code>[]</code>)
        /// </summary>
        public bool LeftValue
        {
            get
            {
                return node.LeftValue;
            }
        }

        /// <summary>
        /// Answer <code>true</code> if this is a deferred expression (containing
        /// sub-expressions starting with <code>#{</code>)
        /// </summary>
        public bool Deferred
        {
            get
            {
                return deferred;
            }
        }

        /// <summary>
        /// Expressions are compared using the concept of a <em>structural id</em>:
        /// variable and function names are anonymized such that two expressions with
        /// same tree structure will also have the same structural id and vice versa.
        /// Two value expressions are equal if
        /// <ol>
        /// <li>their structural id's are equal</li>
        /// <li>their bindings are equal</li>
        /// <li>their expected types are equal</li>
        /// </ol>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == this.GetType())
            {
                TreeValueExpression other = (TreeValueExpression)obj;
                if (!builder.Equals(other.builder))
                {
                    return false;
                }
                if (type != other.type)
                {
                    return false;
                }
                return StructuralId.Equals(other.StructuralId) && bindings.Equals(other.bindings);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return StructuralId.GetHashCode();
        }

        public override string ToString()
        {
            return "TreeValueExpression(" + expr + ")";
        }

        /// <summary>
        /// Print the parse tree. </summary>
        /// <param name="writer"> </param>
        public void Dump(StreamWriter writer)
        {
            NodePrinter.Dump(writer, node);
        }
        
        private void ReadObject(StreamReader @in)
        {
            throw new NotImplementedException();
            //@in.DefaultReadObject();
            //try
            //{
            //    node = builder.Build(expr).Root;
            //}
            //catch (ELException e)
            //{
            //    throw new IOException(e.Message);
            //}
        }
    }
}
