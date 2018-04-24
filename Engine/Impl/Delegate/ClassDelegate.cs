using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;

namespace ESS.FW.Bpm.Engine.Impl.Delegate
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class ClassDelegate
    {
        protected internal string className;
        protected internal IList<FieldDeclaration> fieldDeclarations;

        public ClassDelegate(string className, IList<FieldDeclaration> fieldDeclarations)
        {
            this.className = className;
            this.fieldDeclarations = fieldDeclarations;
        }

        public ClassDelegate(Type clazz, IList<FieldDeclaration> fieldDeclarations)
            : this(clazz.FullName, fieldDeclarations)
        {
        }

        public virtual string ClassName
        {
            get { return className; }
        }

        public virtual IList<FieldDeclaration> FieldDeclarations
        {
            get { return fieldDeclarations; }
        }
    }
}