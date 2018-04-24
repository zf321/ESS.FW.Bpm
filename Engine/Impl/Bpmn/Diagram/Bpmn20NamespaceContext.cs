using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Diagram
{
    /// <summary>
    ///     XML <seealso cref="NamespaceContext" /> containing the namespaces used by BPMN 2.0 XML documents.
    ///     Can be used in <seealso cref="XPath#setNamespaceContext(NamespaceContext)" />.
    ///     
    /// </summary>
    public class Bpmn20NamespaceContext //: NamespaceContext
    {
        public const string Bpmn = "bpmn";
        public const string Bpmndi = "bpmndi";
        public const string Omgdc = "omgdc";
        public const string Omgdi = "omgdi";

        /// <summary>
        ///     This is a protected filed so you can extend that context with your own namespaces if necessary
        /// </summary>
        protected internal IDictionary<string, string> NamespaceUris = new Dictionary<string, string>();

        public Bpmn20NamespaceContext()
        {
            NamespaceUris[Bpmn] = "http://www.omg.org/spec/BPMN/20100524/MODEL";
            NamespaceUris[Bpmndi] = "http://www.omg.org/spec/BPMN/20100524/DI";
            NamespaceUris[Omgdc] = "http://www.omg.org/spec/DD/20100524/DI";
            NamespaceUris[Omgdi] = "http://www.omg.org/spec/DD/20100524/DC";
        }

        public virtual string GetNamespaceUri(string prefix)
        {
            return NamespaceUris[prefix];
        }

        public virtual string GetPrefix(string namespaceUri)
        {
            return GetKeyByValue(NamespaceUris, namespaceUri);
        }

        public virtual IEnumerator<string> GetPrefixes(string namespaceUri)
        {
            return GetKeysByValue(NamespaceUris, namespaceUri).GetEnumerator();
        }

        private static ISet<T> GetKeysByValue<T, TE>(IDictionary<T, TE> map, TE value)
        {
            ISet<T> keys = new HashSet<T>();
            foreach (var entry in map)
                if (value.Equals(entry.Value))
                    keys.Add(entry.Key);
            return keys;
        }

        private static T GetKeyByValue<T, TE>(IDictionary<T, TE> map, TE value)
        {
            foreach (var entry in map)
                if (value.Equals(entry.Value))
                    return entry.Key;
            return default(T);
        }
    }
}