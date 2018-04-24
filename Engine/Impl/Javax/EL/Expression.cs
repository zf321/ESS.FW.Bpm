

using ESS.FW.Bpm.Engine.Impl.EL;
using System;
using System.Collections;

namespace ESS.FW.Bpm.Engine.Impl.Javax.EL
{
    /// <summary>
    ///     Base class for the expression subclasses <seealso cref="ValueExpression" /> and <seealso cref="MethodExpression" />
    ///     ,
    ///     implementing characterstics common to both. All expressions must implement the equals() and
    ///     hashCode() methods so that two expressions can be compared for equality. They are redefined
    ///     abstract in this class to force their implementation in subclasses. All expressions must also be
    ///     Serializable so that they can be saved and restored. Expressions are also designed to be
    ///     immutable so that only one instance needs to be created for any given expression String /
    ///     <seealso cref="FunctionMapper" />. This allows a container to pre-create expressions and not have to reparse
    ///     them each time they are evaluated.
    /// </summary>
    [Serializable]
    public abstract class Expression
    {
        private const long SerialVersionUid = 1L;

        /// <summary>
        ///     Returns the original String used to create this Expression, unmodified. This is used for
        ///     debugging purposes but also for the purposes of comparison (e.g. to ensure the expression in
        ///     a configuration file has not changed). This method does not provide sufficient information to
        ///     re-create an expression. Two different expressions can have exactly the same expression
        ///     string but different function mappings. Serialization should be used to save and restore the
        ///     state of an Expression.
        /// </summary>
        /// <returns> The original expression String. </returns>
        public abstract string ExpressionString { get; }

        /// <summary>
        ///     Returns whether this expression was created from only literal text. This method must return
        ///     true if and only if the expression string this expression was created from contained no
        ///     unescaped EL delimeters (${...} or #{...}).
        /// </summary>
        /// <returns> true if this expression was created from only literal text; false otherwise. </returns>
        public abstract bool IsLiteralText { get; }

        /// <summary>
        ///     Determines whether the specified object is equal to this Expression. The result is true if
        ///     and only if the argument is not null, is an Expression object that is the of the same type
        ///     (ValueExpression or MethodExpression), and has an identical parsed representation. Note that
        ///     two expressions can be equal if their expression Strings are different. For example,
        ///     ${fn1:foo()} and ${fn2:foo()} are equal if their corresponding FunctionMappers mapped fn1:foo
        ///     and fn2:foo to the same method.
        /// </summary>
        /// <param name="obj">
        ///     the Object to test for equality.
        /// </param>
        /// <returns> true if obj equals this Expression; false otherwise. </returns>
        public abstract override bool Equals(object obj);

        /// <summary>
        ///     Returns the hash code for this Expression. See the note in the <seealso cref="#equals(Object)" /> method
        ///     on how two expressions can be equal if their expression Strings are different. Recall that if
        ///     two objects are equal according to the equals(Object) method, then calling the hashCode
        ///     method on each of the two objects must produce the same integer result. Implementations must
        ///     take special note and implement hashCode correctly.
        /// </summary>
        /// <returns> The hash code for this Expression. </returns>
        /// <seealso cref= "#equals( Object)"></seealso>
        /// <seealso cref= "Hashtable"></seealso>
        /// <seealso cref= "Object# hashCode()"></seealso>
        public abstract override int GetHashCode();
    }
}