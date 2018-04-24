using System.Collections.Generic;
using System.IO;

namespace ESS.FW.Bpm.Engine.Form.Impl.Engine
{
    /// <summary>
    ///     <para>
    ///         The <seealso cref="HtmlDocumentBuilder" /> is part of the <seealso cref="HtmlFormEngine" />
    ///         and maintains a stack of element which are written out to a <seealso cref="StringWriter" />.
    ///     </para>
    ///     <para>Actual writing of the html elements is delegated to the <seealso cref="HtmlElementWriter" />.</para>
    ///     
    /// </summary>
    public class HtmlDocumentBuilder
    {
        private readonly bool _instanceFieldsInitialized;
        protected internal HtmlWriteContext Context;

        protected internal Stack<HtmlElementWriter> Elements = new Stack<HtmlElementWriter>();
        protected internal StringWriter Writer = new StringWriter();

        public HtmlDocumentBuilder(HtmlElementWriter documentElement)
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
            StartElement(documentElement);
        }

        public virtual string HtmlString
        {
            get { return Writer.ToString(); }
        }

        private void InitializeInstanceFields()
        {
            Context = new HtmlWriteContext(this);
        }

        public virtual HtmlDocumentBuilder StartElement(HtmlElementWriter renderer)
        {
            renderer.WriteStartTag(Context);
            Elements.Push(renderer);
            return this;
        }

        public virtual HtmlDocumentBuilder EndElement()
        {
            var renderer = Elements.Pop();
            renderer.WriteContent(Context);
            renderer.WriteEndTag(Context);
            return this;
        }

        public class HtmlWriteContext
        {
            private readonly HtmlDocumentBuilder _outerInstance;

            public HtmlWriteContext(HtmlDocumentBuilder outerInstance)
            {
                this._outerInstance = outerInstance;
            }


            public virtual StringWriter Writer
            {
                get { return _outerInstance.Writer; }
            }

            public virtual int ElementStackSize
            {
                get { return _outerInstance.Elements.Count; }
            }
        }
    }
}