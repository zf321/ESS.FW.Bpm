using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Util.xml
{
    /// <summary>
    /// </summary>
    public class ParseHandler//: DefaultHandler
    {
        protected internal string defaultNamespace;
        protected internal Stack<Element> ElementStack = new Stack<Element>();
        //protected internal Locator locator;
        protected internal Parse Parse;

        public ParseHandler(Parse parse)
        {
            this.Parse = parse;
        }

        //public virtual Locator DocumentLocator
        //{
        //    set { locator = value; }
        //}

        public virtual string DefaultNamespace
        {
            set { defaultNamespace = value; }
        }
        
        public virtual void StartElement(string uri, string localName, string qName, IAttributes attributes,
            ILocator locator)
        {
            var element = new Element(uri, localName, qName, attributes, locator);
            if (ElementStack.Count == 0)
                Parse.rootElement = element;
            else
                ElementStack.Peek().Add(element);
            ElementStack.Push(element);
        }
        
        public virtual void Characters(char[] ch, int start, int length)
        {
            ElementStack.Peek().AppendText(ch.ToString().Substring(start, length));
        }
        
        public virtual void EndElement(string uri, string localName, string qName)
        {
            ElementStack.Pop();
        }

        public virtual void Error(System.Exception e)
        {
            Parse.AddError(e.Message, null);
        }

        public virtual void FatalError(System.Exception e)
        {
            Parse.AddError(e.Message, null);
        }

        public virtual void Warning(System.Exception e)
        {
            Parse.AddWarning(e.Message, null);
        }
    }
}