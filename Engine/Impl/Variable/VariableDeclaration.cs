using System;
using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Variable
{
    /// <summary>
    ///      
    /// </summary>
    [Obsolete]
    [Serializable]
    public class VariableDeclaration
    {
        private const long SerialVersionUid = 1L;
        protected internal IExpression destinationExpression;
        protected internal string destinationVariableName;
        protected internal string link;
        protected internal IExpression linkExpression;

        protected internal string name;
        protected internal IExpression sourceExpression;
        protected internal string sourceVariableName;
        protected internal string type;

        public VariableDeclaration(string name, string type)
        {
            this.name = name;
            this.type = type;
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }


        public virtual string Type
        {
            get { return type; }
            set { type = value; }
        }


        public virtual string SourceVariableName
        {
            get { return sourceVariableName; }
            set { sourceVariableName = value; }
        }


        public virtual IExpression SourceExpression
        {
            get { return sourceExpression; }
            set { sourceExpression = value; }
        }


        public virtual string DestinationVariableName
        {
            get { return destinationVariableName; }
            set { destinationVariableName = value; }
        }


        public virtual IExpression DestinationExpression
        {
            get { return destinationExpression; }
            set { destinationExpression = value; }
        }


        public virtual string Link
        {
            get { return link; }
            set { link = value; }
        }


        public virtual IExpression LinkExpression
        {
            get { return linkExpression; }
            set { linkExpression = value; }
        }


        public virtual void Initialize(IVariableScope innerScopeInstance, IVariableScope outerScopeInstance)
        {
            if (!ReferenceEquals(sourceVariableName, null))
                if (outerScopeInstance.HasVariable(sourceVariableName))
                {
                    var value = outerScopeInstance.GetVariable(sourceVariableName);
                    innerScopeInstance.SetVariable(destinationVariableName, value);
                }
                else
                {
                    throw new ProcessEngineException("Couldn't create variable '" + destinationVariableName +
                                                     "', since the source variable '" + sourceVariableName +
                                                     "does not exist");
                }

            if (sourceExpression != null)
            {
                var value = sourceExpression.GetValue(outerScopeInstance);
                innerScopeInstance.SetVariable(destinationVariableName, value);
            }

            if (!ReferenceEquals(link, null))
                if (outerScopeInstance.HasVariable(sourceVariableName))
                {
                    var value = outerScopeInstance.GetVariable(sourceVariableName);
                    innerScopeInstance.SetVariable(destinationVariableName, value);
                }
                else
                {
                    throw new ProcessEngineException("Couldn't create variable '" + destinationVariableName +
                                                     "', since the source variable '" + sourceVariableName +
                                                     "does not exist");
                }

            if (linkExpression != null)
            {
                var value = sourceExpression.GetValue(outerScopeInstance);
                innerScopeInstance.SetVariable(destinationVariableName, value);
            }
        }

        public virtual void Destroy(IVariableScope innerScopeInstance, IVariableScope outerScopeInstance)
        {
            if (!ReferenceEquals(destinationVariableName, null))
                if (innerScopeInstance.HasVariable(sourceVariableName))
                {
                    var value = innerScopeInstance.GetVariable(sourceVariableName);
                    outerScopeInstance.SetVariable(destinationVariableName, value);
                }
                else
                {
                    throw new ProcessEngineException("Couldn't destroy variable " + sourceVariableName +
                                                     ", since it does not exist");
                }

            if (destinationExpression != null)
            {
                var value = destinationExpression.GetValue(innerScopeInstance);
                outerScopeInstance.SetVariable(destinationVariableName, value);
            }

            if (!ReferenceEquals(link, null))
                if (innerScopeInstance.HasVariable(sourceVariableName))
                {
                    var value = innerScopeInstance.GetVariable(sourceVariableName);
                    outerScopeInstance.SetVariable(destinationVariableName, value);
                }
                else
                {
                    throw new ProcessEngineException("Couldn't destroy variable " + sourceVariableName +
                                                     ", since it does not exist");
                }

            if (linkExpression != null)
            {
                var value = sourceExpression.GetValue(innerScopeInstance);
                outerScopeInstance.SetVariable(destinationVariableName, value);
            }
        }

        public override string ToString()
        {
            return "VariableDeclaration[" + name + ":" + type + "]";
        }
    }
}