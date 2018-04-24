


using ESS.FW.Bpm.Engine.Form.Impl.Handler;

namespace ESS.FW.Bpm.Engine.Form.Impl
{
    /// <summary>
    ///      
    /// </summary>
    public class FormPropertyImpl : IFormProperty
    {
        protected internal string id;

        public FormPropertyImpl(FormPropertyHandler formPropertyHandler)
        {
            id = formPropertyHandler.Id;
            Name = formPropertyHandler.Name;
            Type = formPropertyHandler.getType();
            Required = formPropertyHandler.Required;
            Readable = formPropertyHandler.Readable;
            Writable = formPropertyHandler.Writable;
        }

        public virtual string Id => id;

        public virtual string Name { get; }

        public virtual IFormType Type { get; }

        public virtual string Value { get; set; }

        public virtual bool Required { get; }

        public virtual bool Readable { get; }


        public virtual bool Writable { get; }
    }
}