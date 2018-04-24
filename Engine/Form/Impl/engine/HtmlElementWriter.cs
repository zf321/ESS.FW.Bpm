using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Form.Impl.Engine
{
    /// <summary>
    ///     <para>Simple writer for html elements. Used by the <seealso cref="HtmlDocumentBuilder" />.</para>
    ///     
    /// </summary>
    public class HtmlElementWriter
    {
        protected internal IDictionary<string, string> Attributes = new Dictionary<string, string>();

        /// <summary>
        ///     selfClosing means that the element should not be rendered as a
        ///     start + end tag pair but as a single tag using "/" to close the tag
        ///     inline
        /// </summary>
        protected internal bool IsSelfClosing;

        protected internal string TagName;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string TextContentRenamed;

        public HtmlElementWriter(string tagName)
        {
            this.TagName = tagName;
            IsSelfClosing = false;
        }

        public HtmlElementWriter(string tagName, bool isSelfClosing)
        {
            this.TagName = tagName;
            this.IsSelfClosing = isSelfClosing;
        }

        public virtual void WriteStartTag(HtmlDocumentBuilder.HtmlWriteContext context)
        {
            WriteLeadingWhitespace(context);
            WriteStartTagOpen(context);
            WriteAttributes(context);
            WriteStartTagClose(context);
            WriteEndLine(context);
        }

        public virtual void WriteContent(HtmlDocumentBuilder.HtmlWriteContext context)
        {
            if (!ReferenceEquals(TextContentRenamed, null))
            {
                WriteLeadingWhitespace(context);
                WriteTextContent(context);
                WriteEndLine(context);
            }
        }

        public virtual void WriteEndTag(HtmlDocumentBuilder.HtmlWriteContext context)
        {
            if (!IsSelfClosing)
            {
                WriteLeadingWhitespace(context);
                WriteEndTagElement(context);
                WriteEndLine(context);
            }
        }

        protected internal virtual void WriteEndTagElement(HtmlDocumentBuilder.HtmlWriteContext context)
        {
            var writer = context.Writer;
            writer.Write("</");
            writer.Write(TagName);
            writer.Write(">");
        }

        protected internal virtual void WriteTextContent(HtmlDocumentBuilder.HtmlWriteContext context)
        {
            var writer = context.Writer;
            writer.Write("  "); // add additional whitespace
            writer.Write(TextContentRenamed);
        }

        protected internal virtual void WriteStartTagOpen(HtmlDocumentBuilder.HtmlWriteContext context)
        {
            var writer = context.Writer;
            writer.Write("<");
            writer.Write(TagName);
        }

        protected internal virtual void WriteAttributes(HtmlDocumentBuilder.HtmlWriteContext context)
        {
            var writer = context.Writer;
            foreach (var attribute in Attributes)
            {
                writer.Write(" ");
                writer.Write(attribute.Key);
                if (attribute.Value != null)
                {
                    writer.Write("=\"");
                    var attributeValue = EscapeQuotes(attribute.Value);
                    writer.Write(attributeValue);
                    writer.Write("\"");
                }
            }
        }

        protected internal virtual string EscapeQuotes(string attributeValue)
        {
            var escapedHtmlQuote = "&quot;";
            var escapedJavaQuote = "\"";
            return attributeValue.Replace(escapedJavaQuote, escapedHtmlQuote);
        }

        protected internal virtual void WriteEndLine(HtmlDocumentBuilder.HtmlWriteContext context)
        {
            var writer = context.Writer;
            writer.Write("\n");
        }

        protected internal virtual void WriteStartTagClose(HtmlDocumentBuilder.HtmlWriteContext context)
        {
            var writer = context.Writer;
            if (IsSelfClosing)
                writer.Write(" /");
            writer.Write(">");
        }

        protected internal virtual void WriteLeadingWhitespace(HtmlDocumentBuilder.HtmlWriteContext context)
        {
            var stackSize = context.ElementStackSize;
            var writer = context.Writer;
            for (var i = 0; i < stackSize; i++)
                writer.Write("  ");
        }

        // builder /////////////////////////////////////

        public virtual HtmlElementWriter Attribute(string name, string value)
        {
            Attributes[name] = value;
            return this;
        }

        public virtual HtmlElementWriter TextContent(string text)
        {
            if (IsSelfClosing)
                throw new InvalidOperationException("Self-closing element cannot have text content.");
            TextContentRenamed = text;
            return this;
        }
    }
}