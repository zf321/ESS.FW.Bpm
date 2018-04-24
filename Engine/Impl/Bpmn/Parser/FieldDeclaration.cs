namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     Represents a field declaration in object form:
    ///     &lt;field name='someField&gt; &lt;string ...
    ///     
    ///     
    /// </summary>
    public class FieldDeclaration
    {
        public FieldDeclaration(string name, string type, object value)
        {
            this.Name = name;
            this.Type = type;
            this.Value = value;
        }

        public FieldDeclaration()
        {
        }

        public virtual string Name { get; set; }

        public virtual string Type { get; set; }

        public virtual object Value { get; set; }
    }
}