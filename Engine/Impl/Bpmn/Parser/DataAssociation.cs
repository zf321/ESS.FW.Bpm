using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     A data association (Input or Output) between a source and a target
    /// </summary>
    public class DataAssociation
    {
        protected internal IExpression businessKeyExpression;

        protected internal string source;

        protected internal IExpression sourceExpression;

        protected internal string target;

        protected internal string variables;

        protected internal DataAssociation(string source, string target)
        {
            this.source = source;
            this.target = target;
        }

        protected internal DataAssociation(IExpression sourceExpression, string target)
        {
            this.sourceExpression = sourceExpression;
            this.target = target;
        }

        protected internal DataAssociation(string variables)
        {
            this.variables = variables;
        }

        protected internal DataAssociation(IExpression businessKeyExpression)
        {
            this.businessKeyExpression = businessKeyExpression;
        }

        public virtual string Source
        {
            get { return source; }
        }

        public virtual string Target
        {
            get { return target; }
        }


        public virtual IExpression SourceExpression
        {
            get { return sourceExpression; }
        }

        public virtual string Variables
        {
            get { return variables; }
        }

        public virtual IExpression BusinessKeyExpression
        {
            get { return businessKeyExpression; }
        }
    }
}