using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;



namespace ESS.FW.Bpm.Model.Xml.impl.util
{


    /// <summary>
    /// 
    /// </summary>
    public sealed class StringUtil
    {

        private static readonly string Pattern = "(\\w[^,]*)|([#$]\\{[^}]*})";

        /// <summary>
        /// Splits a comma separated list in to single Strings. The list can
        /// contain expressions with commas in it.
        /// </summary>
        /// <param name="text">  the comma separated list </param>
        /// <returns> the Strings of the list or an empty List if text is empty or null </returns>
        public static IList<string> SplitCommaSeparatedList(string text)
        {
            if (string.ReferenceEquals(text, null) || text.Length == 0)
            {
                return new List<string>();
            }
            
            var matchers = Regex.Match(text, Pattern);
            IList<string> parts = new List<string>();
            foreach (Group t in matchers.Groups)
            {
                parts.Add(t.Value); 
            }
            return parts;
        }

        /// <summary>
        /// Joins a list of Strings to a comma separated single String.
        /// </summary>
        /// <param name="list">  the list to join </param>
        /// <returns> the resulting comma separated string or null if the list is null </returns>
        public static string JoinCommaSeparatedList(IList<string> list)
        {
            return JoinList(list, ", ");
        }

        public static IList<string> SplitListBySeparator(string text, char separator)
        {
            string[] result = new string[] { };
            if (!string.ReferenceEquals(text, null))
            {
                result = text.Split(separator);
            }
            return result;
        }

        public static string JoinList(IList<string> list, string separator)
        {
            if (list == null)
            {
                return null;
            }

            int size = list.Count;
            if (size == 0)
            {
                return "";
            }
            else if (size == 1)
            {
                return list[0];
            }
            else
            {
                StringBuilder builder = new StringBuilder(size * 8);
                builder.Append(list[0]);
                var ls = list.Take(size);
                foreach (object element in ls)
                {
                    builder.Append(separator);
                    builder.Append(element);
                }
                return builder.ToString();
            }
        }
    }
}